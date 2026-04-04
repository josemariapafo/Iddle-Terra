using System;
using System.Collections.Generic;
using Terra.Core;
using Terra.Data;
using Terra.State;

namespace Terra.Systems
{
    /// <summary>
    /// Calculador de producción — lógica pura sin efectos secundarios.
    /// Dado un estado y unas definiciones, devuelve EV/s. Nada más.
    /// Fácil de testear en aislamiento.
    /// </summary>
    public class CalculadorProduccion
    {
        private readonly DefinicionMejora[] _todasMejoras;
        private readonly DefinicionSinergia[] _todasSinergias;
        private readonly DefinicionNodo[] _todosNodos;
        private SistemaCadenas _cadenas;

        public CalculadorProduccion(
            DefinicionMejora[] mejoras,
            DefinicionSinergia[] sinergias,
            DefinicionNodo[] nodos)
        {
            _todasMejoras   = mejoras;
            _todasSinergias = sinergias;
            _todosNodos     = nodos;
        }

        public void AsignarCadenas(SistemaCadenas cadenas) => _cadenas = cadenas;

        /// <summary>
        /// Calcula EV/s completo con todas las capas multiplicadoras.
        /// </summary>
        // Producción base mínima — garantiza que el juego siempre avanza
        // aunque el jugador no haya comprado ninguna mejora todavía
        private const double PRODUCCION_BASE_MINIMA = 1.0;

        public double Calcular(EstadoJuego estado)
        {
            double base_     = System.Math.Max(CalcularBaseProduccion(estado), PRODUCCION_BASE_MINIMA);
            double sinergias = CalcularMultiplicadorSinergias(estado);
            double nodos     = CalcularMultiplicadorNodos(estado);
            double prestige  = estado.Prestige.MultiplicadorTotal;
            double evento    = estado.MultiplicadorEvento;
            double racha     = estado.Racha.MultiplicadorRacha;
            double nocturno  = CalcularBonusNocturno();

            return base_ * sinergias * nodos * prestige * evento * racha * nocturno;
        }

        /// <summary>
        /// Producción base = suma por pilar, cada pilar limitado por su cadena.
        /// </summary>
        public double CalcularBaseProduccion(EstadoJuego estado)
        {
            double[] porPilar = CalcularProduccionBrutaPorPilar(estado);
            double total = 0;

            for (int p = 0; p < 4; p++)
            {
                double produccion = porPilar[p];
                if (_cadenas != null)
                {
                    double cap = _cadenas.CalcularCapPilar((TipoPilar)p);
                    if (cap < produccion) produccion = cap;
                }
                total += produccion;
            }

            return total;
        }

        /// <summary>
        /// Producción bruta por pilar SIN aplicar cap de cadena.
        /// </summary>
        private double[] CalcularProduccionBrutaPorPilar(EstadoJuego estado)
        {
            double[] resultado = new double[4];
            foreach (var def in _todasMejoras)
            {
                if (!estado.MejoraDesbloqueada(def.Id)) continue;
                int nivel = estado.NivelMejora(def.Id);
                if (nivel <= 0) continue;
                resultado[(int)def.Pilar] += def.ProduccionEnNivel(nivel) * def.MultiplicadorHito(nivel);
            }
            return resultado;
        }

        /// <summary>
        /// Multiplicador de sinergia = producto de todas las sinergias activas.
        /// </summary>
        public double CalcularMultiplicadorSinergias(EstadoJuego estado)
        {
            double mult = 1.0;

            foreach (var def in _todasSinergias)
                if (estado.SinergiaActiva(def.Id))
                    mult *= def.Multiplicador;

            return mult;
        }

        /// <summary>
        /// Multiplicador de nodos del árbol = producto de todos los nodos activos.
        /// </summary>
        public double CalcularMultiplicadorNodos(EstadoJuego estado)
        {
            double mult = 1.0;

            foreach (var def in _todosNodos)
            {
                int nivel = estado.NivelNodo(def.Id);
                if (nivel <= 0) continue;
                mult *= def.MultiplicadorTotal(nivel);
            }

            return mult;
        }

        /// <summary>
        /// ×2 entre las 22:00 y las 06:00 hora local.
        /// </summary>
        public double CalcularBonusNocturno()
        {
            int hora = DateTime.Now.Hour;
            return (hora >= 22 || hora <= 6) ? 2.0 : 1.0;
        }

        /// <summary>
        /// Desglose por pilar: producción bruta, cap de cadena y producción efectiva.
        /// </summary>
        public struct DesglosePilar
        {
            public double Potencial;
            public double CapCadena;
            public double Efectivo;
        }

        public DesglosePilar[] CalcularDesglosePorPilar(EstadoJuego estado)
        {
            double[] bruta = CalcularProduccionBrutaPorPilar(estado);
            var resultado = new DesglosePilar[4];

            for (int p = 0; p < 4; p++)
            {
                double cap = _cadenas != null
                    ? _cadenas.CalcularCapPilar((TipoPilar)p)
                    : double.MaxValue;

                resultado[p] = new DesglosePilar
                {
                    Potencial = bruta[p],
                    CapCadena = cap,
                    Efectivo  = Math.Min(bruta[p], cap)
                };
            }

            return resultado;
        }

        /// <summary>
        /// Producción efectiva por pilar (con cap aplicado). Retrocompatible.
        /// </summary>
        public double[] CalcularProduccionPorPilar(EstadoJuego estado)
        {
            var desglose = CalcularDesglosePorPilar(estado);
            return new[] { desglose[0].Efectivo, desglose[1].Efectivo, desglose[2].Efectivo, desglose[3].Efectivo };
        }

        /// <summary>
        /// Ganancia prestige basada en EV actual acumulada.
        /// </summary>
        public double EstimarGananciaPrestige(TipoPrestige tipo, EstadoJuego estado)
        {
            return CalcularGananciaPrestige(tipo, estado.EnergiaVital);
        }

        /// <summary>
        /// Proyecta ganancia prestige si el jugador espera X segundos más.
        /// </summary>
        public double ProyectarGananciaPrestige(TipoPrestige tipo, EstadoJuego estado, float segundosExtra)
        {
            double evProyectada = estado.EnergiaVital + estado.EVPorSegundo * segundosExtra;
            return CalcularGananciaPrestige(tipo, evProyectada);
        }

        private static double CalcularGananciaPrestige(TipoPrestige tipo, double ev)
        {
            // Divisores alineados con la progresión:
            //   Extincion (Era 3+, ~40K-800K EV): 40K→2, 200K→4, 800K→8
            //   Glaciacion (Era 5+, ~8M-800M EV): 8M→2, 80M→8, 800M→28
            //   BigBang (Era 7+): log10 escala bien sin cambios
            return tipo switch
            {
                TipoPrestige.Extincion  => Math.Floor(Math.Sqrt(ev / 10_000)),
                TipoPrestige.Glaciacion => Math.Floor(Math.Sqrt(ev / 1_000_000)),
                TipoPrestige.BigBang    => Math.Floor(Math.Log10(Math.Max(1, ev))),
                _                       => 0
            };
        }
    }
}
