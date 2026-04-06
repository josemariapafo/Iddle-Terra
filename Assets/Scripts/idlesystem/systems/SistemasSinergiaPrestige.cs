using System;
using System.Linq;
using Terra.Core;
using Terra.Data;
using Terra.State;
using System.Collections.Generic;

namespace Terra.Systems
{
    // ══════════════════════════════════════════════════════════════════════
    // SISTEMA SINERGIAS
    // ══════════════════════════════════════════════════════════════════════
    public class SistemaSinergias : ISistema
    {
        private readonly DefinicionSinergia[] _definiciones;
        private readonly SistemaMejoras _sistemaMejoras;
        private EstadoJuego _estado;

        public SistemaSinergias(DefinicionSinergia[] defs, SistemaMejoras mejoras)
        {
            _definiciones = defs;
            _sistemaMejoras = mejoras;
        }

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;
            foreach (var def in _definiciones)
                if (!_estado.Sinergias.ContainsKey(def.Id))
                    _estado.Sinergias[def.Id] = new EstadoSinergia(def.Id);
        }

        public void Inicializar() { }
        public void Actualizar(float delta) => Comprobar();

        public void Comprobar()
        {
            foreach (var def in _definiciones)
            {
                var est = _estado.Sinergias[def.Id];
                if (est.Activa) continue;

                int nivelA = _sistemaMejoras.NivelTotalPilar(def.PilarA);
                int nivelB = _sistemaMejoras.NivelTotalPilar(def.PilarB);

                bool cumple = nivelA >= def.NivelRequeridoA &&
                              nivelB >= def.NivelRequeridoB;

                if (def.RequiereCuatroPilares)
                {
                    // GAIA: todos los pilares deben superar el nivel requerido
                    foreach (TipoPilar pilar in Enum.GetValues(typeof(TipoPilar)))
                        if (_sistemaMejoras.NivelTotalPilar(pilar) < def.NivelRequeridoA)
                        { cumple = false; break; }
                }

                if (!cumple) continue;

                est.Activa = true;
                est.VecesActivada++;
                EventBus.Publicar(new EventoSinergiaActivada(def.Id));
            }
        }

        public int ContarActivas() => _estado.Sinergias.Values.Count(s => s.Activa);
    }

    // ══════════════════════════════════════════════════════════════════════
    // SISTEMA ERAS
    // ══════════════════════════════════════════════════════════════════════
    public class SistemaEras : ISistema
    {
        private readonly DefinicionEra[] _definiciones;
        private readonly SistemaMejoras _sistemaMejoras;
        private readonly SistemaSinergias _sistemaSinergias;
        private readonly SistemaCadenas _sistemaCadenas;
        private EstadoJuego _estado;

        public SistemaEras(DefinicionEra[] defs,
            SistemaMejoras mejoras, SistemaSinergias sinergias,
            SistemaCadenas cadenas)
        {
            _definiciones = defs;
            _sistemaMejoras = mejoras;
            _sistemaSinergias = sinergias;
            _sistemaCadenas = cadenas;
        }

        public void AsignarEstado(EstadoJuego estado) => _estado = estado;
        public void Inicializar() { }
        public void Actualizar(float delta) { }

        public bool PuedeAvanzar()
        {
            if (_estado.EraActual >= 8) return false;

            var condicion = _definiciones[_estado.EraActual].Condicion;

            // Comprobar nivel mínimo solo de los pilares desbloqueados
            foreach (TipoPilar pilar in Enum.GetValues(typeof(TipoPilar)))
            {
                if (!_sistemaCadenas.CadenaPilarDesbloqueada(pilar)) continue;
                if (_sistemaMejoras.NivelTotalPilar(pilar) < condicion.NivelTotalPorPilar)
                    return false;
            }

            // Comprobar EV requerida
            if (_estado.EnergiaVital < condicion.EVRequerida) return false;

            // Comprobar sinergias requeridas
            foreach (var idSin in condicion.SinergiasRequeridas)
                if (!_estado.SinergiaActiva(idSin)) return false;

            return true;
        }

        public void AvanzarEra()
        {
            if (!PuedeAvanzar()) return;

            // Gastar la EV requerida
            _estado.EnergiaVital -= _definiciones[_estado.EraActual].Condicion.EVRequerida;
            _estado.EraActual++;

            // Desbloquear cadenas de la nueva era
            _sistemaCadenas.ComprobarDesbloqueos();

            EventBus.Publicar(new EventoEraAvanzada(_estado.EraActual));
        }

        public DefinicionEra ObtenerDefinicionActual() =>
            _definiciones[_estado.EraActual - 1];

        public CondicionEra ObtenerCondicionSiguiente() =>
            _estado.EraActual < 8 ? _definiciones[_estado.EraActual].Condicion : null;

        public double ProgresoHaciaEra()
        {
            if (_estado.EraActual >= 8) return 1.0;

            var condicion = _definiciones[_estado.EraActual].Condicion;
            if (condicion.NivelTotalPorPilar == 0) return 1.0;

            double nivelMin = double.MaxValue;
            foreach (TipoPilar pilar in Enum.GetValues(typeof(TipoPilar)))
            {
                if (!_sistemaCadenas.CadenaPilarDesbloqueada(pilar)) continue;
                double progresoPilar = (double)_sistemaMejoras.NivelTotalPilar(pilar)
                    / condicion.NivelTotalPorPilar;
                nivelMin = Math.Min(nivelMin, progresoPilar);
            }

            return nivelMin == double.MaxValue ? 1.0 : Math.Min(nivelMin, 1.0);
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // SISTEMA PRESTIGE
    // ══════════════════════════════════════════════════════════════════════
    public class SistemaPrestige : ISistema, IReseteble
    {
        private readonly CalculadorProduccion _calculador;
        private EstadoJuego _estado;

        public SistemaPrestige(CalculadorProduccion calculador) =>
            _calculador = calculador;

        public void AsignarEstado(EstadoJuego estado) => _estado = estado;
        public void Inicializar() { }
        public void Actualizar(float delta) { }

        public bool PuedeHacer(TipoPrestige tipo) => tipo switch
        {
            TipoPrestige.Extincion => _estado.EraActual >= 3,
            TipoPrestige.Glaciacion => _estado.EraActual >= 5,
            TipoPrestige.BigBang => _estado.EraActual >= 7,
            _ => false
        };

        public double GananciaEstimada(TipoPrestige tipo) =>
            _calculador.EstimarGananciaPrestige(tipo, _estado);

        public double GananciaProyectada(TipoPrestige tipo, float segundos) =>
            _calculador.ProyectarGananciaPrestige(tipo, _estado, segundos);

        public void Realizar(TipoPrestige tipo)
        {
            if (!PuedeHacer(tipo)) return;

            double ganancia = GananciaEfectiva(tipo);
            if (ganancia <= 0) return;

            switch (tipo)
            {
                case TipoPrestige.Extincion:
                    _estado.Prestige.Fosiles = System.Math.Min(
                        _estado.Prestige.Fosiles + ganancia,
                        EstadoPrestige.CapFosiles);
                    _estado.Prestige.VecesExtincion++;
                    Resetear(TipoReset.Parcial);
                    break;

                case TipoPrestige.Glaciacion:
                    _estado.Prestige.Genes = System.Math.Min(
                        _estado.Prestige.Genes + ganancia,
                        EstadoPrestige.CapGenes);
                    _estado.Prestige.VecesGlaciacion++;
                    Resetear(TipoReset.Parcial);
                    break;

                case TipoPrestige.BigBang:
                    _estado.Prestige.Quarks = System.Math.Min(
                        _estado.Prestige.Quarks + ganancia,
                        EstadoPrestige.CapQuarks);
                    _estado.Prestige.VecesBigBang++;
                    Resetear(TipoReset.Total);
                    break;
            }

            EventBus.Publicar(new EventoPrestigeRealizado(tipo, ganancia));
        }

        /// <summary>
        /// Ganancia real teniendo en cuenta el cap restante.
        /// </summary>
        public double GananciaEfectiva(TipoPrestige tipo)
        {
            double formula = GananciaEstimada(tipo);
            double restante = tipo switch
            {
                TipoPrestige.Extincion  => EstadoPrestige.CapFosiles - _estado.Prestige.Fosiles,
                TipoPrestige.Glaciacion => EstadoPrestige.CapGenes   - _estado.Prestige.Genes,
                TipoPrestige.BigBang    => EstadoPrestige.CapQuarks  - _estado.Prestige.Quarks,
                _ => 0
            };
            return System.Math.Max(0, System.Math.Min(formula, restante));
        }

        public void Resetear(TipoReset tipo)
        {
            // Siempre se resetea
            _estado.EnergiaVital = 0;
            _estado.EVPorSegundo = 0;
            _estado.EraActual = 1;
            _estado.TiempoJugadoTotal = 0;
            _estado.MultiplicadorEvento = 1.0;
            _estado.TiempoRestanteEvento = 0;
            _estado.EventoActivoId = null;

            // Resetear mejoras
            foreach (var est in _estado.Mejoras.Values)
            {
                est.Nivel = 0;
                est.Desbloqueada = false;
            }

            // Resetear sinergias
            foreach (var est in _estado.Sinergias.Values)
            {
                est.Activa = false;
            }

            // Resetear cadenas
            foreach (var est in _estado.Cadenas.Values)
            {
                est.Nivel = 0;
                est.Desbloqueada = false;
            }

            if (tipo == TipoReset.Total)
            {
                // BigBang: también resetear árbol, recursos y Códice Fósil
                foreach (var est in _estado.Nodos.Values)
                {
                    est.Nivel = 0;
                    est.Desbloqueado = false;
                }
                foreach (var est in _estado.Recursos.Values)
                {
                    est.Cantidad = 0;
                    est.Desbloqueado = false;
                }
                foreach (var est in _estado.NodosCodice.Values)
                {
                    est.Nivel = 0;
                }
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // SISTEMA ESTANCAMIENTO
    // ══════════════════════════════════════════════════════════════════════
    public class SistemaEstancamiento : ISistema
    {
        private readonly SistemaPrestige _sistemaPrestige;
        private EstadoJuego _estado;

        private readonly Queue<double> _intervalosMejoras = new Queue<double>();
        private double _tiempoUltimaMejora;
        private double _mediaIntervalos;
        private bool _notificacionEnviada;

        private const int VENTANA_MEJORAS = 5;
        private const double FACTOR_ESTANCAMIENTO = 3.0;

        public SistemaEstancamiento(SistemaPrestige prestige)
        {
            _sistemaPrestige = prestige;
            EventBus.Suscribir<EventoMejoraComprada>(OnMejoraComprada);
        }

        public void AsignarEstado(EstadoJuego estado) => _estado = estado;
        public void Inicializar() { }

        public void Actualizar(float delta)
        {
            if (_notificacionEnviada) return;
            if (_mediaIntervalos <= 0) return;

            double tiempoSinMejora = _estado.TiempoJugadoTotal - _tiempoUltimaMejora;

            if (tiempoSinMejora > _mediaIntervalos * FACTOR_ESTANCAMIENTO)
            {
                // Sugerir el prestige más accesible
                TipoPrestige sugerido = ObtenerPrestigeSugerido();
                double ganancia = _sistemaPrestige.GananciaEstimada(sugerido);

                if (ganancia > 0)
                {
                    EventBus.Publicar(new EventoEstancamientoDetectado(sugerido, ganancia));
                    _notificacionEnviada = true;
                }
            }
        }

        private void OnMejoraComprada(EventoMejoraComprada _)
        {
            if (_tiempoUltimaMejora > 0)
            {
                double intervalo = _estado.TiempoJugadoTotal - _tiempoUltimaMejora;
                _intervalosMejoras.Enqueue(intervalo);
                if (_intervalosMejoras.Count > VENTANA_MEJORAS)
                    _intervalosMejoras.Dequeue();

                double suma = 0;
                foreach (var t in _intervalosMejoras) suma += t;
                _mediaIntervalos = suma / _intervalosMejoras.Count;
            }

            _tiempoUltimaMejora = _estado.TiempoJugadoTotal;
            _notificacionEnviada = false;  // resetear tras cada compra
        }

        private TipoPrestige ObtenerPrestigeSugerido()
        {
            if (_sistemaPrestige.PuedeHacer(TipoPrestige.BigBang)) return TipoPrestige.BigBang;
            if (_sistemaPrestige.PuedeHacer(TipoPrestige.Glaciacion)) return TipoPrestige.Glaciacion;
            return TipoPrestige.Extincion;
        }

        public bool EstaEstancado()
        {
            if (_mediaIntervalos <= 0) return false;
            double tiempoSinMejora = _estado.TiempoJugadoTotal - _tiempoUltimaMejora;
            return tiempoSinMejora > _mediaIntervalos * FACTOR_ESTANCAMIENTO;
        }
    }
}
