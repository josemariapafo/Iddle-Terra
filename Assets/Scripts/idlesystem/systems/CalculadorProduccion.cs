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

        public CalculadorProduccion(
            DefinicionMejora[] mejoras,
            DefinicionSinergia[] sinergias,
            DefinicionNodo[] nodos)
        {
            _todasMejoras   = mejoras;
            _todasSinergias = sinergias;
            _todosNodos     = nodos;
        }

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
        /// Producción base = suma de todas las mejoras con sus hitos y multiplicadores de pilar.
        /// </summary>
        public double CalcularBaseProduccion(EstadoJuego estado)
        {
            double total = 0;

            foreach (var def in _todasMejoras)
            {
                if (!estado.MejoraDesbloqueada(def.Id)) continue;
                int nivel = estado.NivelMejora(def.Id);
                if (nivel <= 0) continue;

                double prod = def.ProduccionEnNivel(nivel) * def.MultiplicadorHito(nivel);
                total += prod;
            }

            return total;
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
        /// Desglose de producción por pilar. Útil para la UI.
        /// </summary>
        public double[] CalcularProduccionPorPilar(EstadoJuego estado)
        {
            double[] resultado = new double[4];

            foreach (var def in _todasMejoras)
            {
                if (!estado.MejoraDesbloqueada(def.Id)) continue;
                int nivel = estado.NivelMejora(def.Id);
                if (nivel <= 0) continue;

                resultado[(int)def.Pilar] +=
                    def.ProduccionEnNivel(nivel) * def.MultiplicadorHito(nivel);
            }

            return resultado;
        }

        /// <summary>
        /// Ganancia prestige estimada. Cálculo independiente para mostrar en UI.
        /// </summary>
        public double EstimarGananciaPrestige(TipoPrestige tipo, EstadoJuego estado)
        {
            double evAcumuladaEstimada = estado.TiempoJugadoTotal * estado.EVPorSegundo;

            return tipo switch
            {
                TipoPrestige.Extincion  => Math.Floor(Math.Sqrt(evAcumuladaEstimada / 1_000_000)),
                TipoPrestige.Glaciacion => Math.Floor(Math.Sqrt(evAcumuladaEstimada / 1_000_000_000)),
                TipoPrestige.BigBang    => Math.Floor(Math.Log10(Math.Max(1, evAcumuladaEstimada))),
                _                       => 0
            };
        }
    }
}
