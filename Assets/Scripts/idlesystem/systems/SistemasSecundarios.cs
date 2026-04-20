using System;
using System.Linq;
using Terra.Core;
using Terra.Data;
using Terra.State;
using UnityEngine;

namespace Terra.Systems
{
    // ══════════════════════════════════════════════════════════════════════
    // SISTEMA EVENTOS ALEATORIOS
    // ══════════════════════════════════════════════════════════════════════
    public class SistemaEventos : ISistema
    {
        private readonly DefinicionEvento[] _definiciones;
        private EstadoJuego _estado;
        private SistemaCodiceGenetico _codiceGen;

        private float _timerProximoEvento;
        private const float INTERVALO_MIN = 300f;  // 5 min
        private const float INTERVALO_MAX = 900f;  // 15 min

        public SistemaEventos(DefinicionEvento[] defs) => _definiciones = defs;
        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;
            ResetearTimer();
        }
        public void AsignarCodiceGenetico(SistemaCodiceGenetico cg) => _codiceGen = cg;

        /// <summary>True si el Códice Genético ha desbloqueado la 4ª opción oculta.</summary>
        public bool OpcionExtraDesbloqueada() =>
            _codiceGen?.OpcionEventoExtraDesbloqueada() ?? false;

        public void Inicializar() { }

        public void Actualizar(float delta)
        {
            // Actualizar evento activo
            if (_estado.TiempoRestanteEvento > 0)
            {
                _estado.TiempoRestanteEvento -= delta;
                if (_estado.TiempoRestanteEvento <= 0)
                    TerminarEvento();
                return;
            }

            // Timer para próximo evento
            _timerProximoEvento -= delta;
            if (_timerProximoEvento <= 0)
                IntentarLanzarEvento();
        }

        public void AceptarEvento(string idEvento)
        {
            if (_estado.EventoActivoId != idEvento) return;

            var def = _definiciones.FirstOrDefault(d => d.Id == idEvento);
            if (def == null) return;

            _estado.MultiplicadorEvento = def.Multiplicador;
            _estado.TiempoRestanteEvento = def.DuracionSegundos;
        }

        /// <summary>
        /// Aplica el efecto de una de las opciones del evento activo.
        /// Compat: si el evento no tiene Opciones, cae al modo clásico (AceptarEvento).
        /// </summary>
        public void AceptarOpcion(string idEvento, int indiceOpcion)
        {
            if (_estado.EventoActivoId != idEvento) return;

            var def = _definiciones.FirstOrDefault(d => d.Id == idEvento);
            if (def == null) return;

            // Compat modo clásico
            if (def.Opciones == null || def.Opciones.Length == 0)
            {
                AceptarEvento(idEvento);
                return;
            }

            if (indiceOpcion < 0 || indiceOpcion >= def.Opciones.Length) return;

            var opcion = def.Opciones[indiceOpcion];

            // Coste/bono en porcentaje de EV actual
            // > 0 → gasta EV | < 0 → bonus instantáneo
            if (opcion.CostePorcentajeEV > 0)
            {
                double coste = _estado.EnergiaVital * opcion.CostePorcentajeEV;
                _estado.EnergiaVital = Math.Max(0, _estado.EnergiaVital - coste);
            }
            else if (opcion.CostePorcentajeEV < 0)
            {
                double bonus = _estado.EnergiaVital * Math.Abs(opcion.CostePorcentajeEV);
                _estado.EnergiaVital += bonus;
            }

            // Efecto temporal (si Duracion > 0 y Multiplicador != 1)
            if (opcion.Duracion > 0 && Math.Abs(opcion.Multiplicador - 1.0) > 0.0001)
            {
                _estado.MultiplicadorEvento = opcion.Multiplicador;
                _estado.TiempoRestanteEvento = opcion.Duracion;
            }
            else
            {
                // Opción instantánea (sin efecto temporal): cerrar evento inmediatamente
                _estado.EventoActivoId = null;
                ResetearTimer();
            }
        }

        public void RechazarEvento()
        {
            _estado.EventoActivoId = null;
            ResetearTimer();
        }

        private void IntentarLanzarEvento()
        {
            var disponibles = _definiciones
                .Where(d => d.EraMinima <= _estado.EraActual)
                .ToArray();

            if (disponibles.Length == 0) { ResetearTimer(); return; }

            int idx = UnityEngine.Random.Range(0, disponibles.Length);
            var evt = disponibles[idx];

            _estado.EventoActivoId = evt.Id;

            if (!evt.RequiereAccion)
            {
                // Aplicar automáticamente
                _estado.MultiplicadorEvento = evt.Multiplicador;
                _estado.TiempoRestanteEvento = evt.DuracionSegundos;
            }

            EventBus.Publicar(new EventoEventoActivado(evt.Id));
        }

        private void TerminarEvento()
        {
            _estado.MultiplicadorEvento = 1.0;
            _estado.TiempoRestanteEvento = 0;
            _estado.EventoActivoId = null;
            ResetearTimer();
        }

        private void ResetearTimer()
        {
            float min = INTERVALO_MIN;
            float max = INTERVALO_MAX;
            // Reducción cooldown (Mutación): acumulable hasta 0.45.
            double reduccion = _codiceGen?.ReduccionCooldownEventos() ?? 0.0;
            if (reduccion > 0.8) reduccion = 0.8;
            float factor = (float)(1.0 - reduccion);
            _timerProximoEvento = UnityEngine.Random.Range(min * factor, max * factor);
        }

        public DefinicionEvento ObtenerEventoActivo() =>
            _estado.EventoActivoId == null ? null
            : _definiciones.FirstOrDefault(d => d.Id == _estado.EventoActivoId);
    }

    // ══════════════════════════════════════════════════════════════════════
    // SISTEMA ÁRBOL DE EVOLUCIÓN
    // ══════════════════════════════════════════════════════════════════════
    public class SistemaArbol : ISistema
    {
        private readonly DefinicionNodo[] _definiciones;
        private EstadoJuego _estado;

        public SistemaArbol(DefinicionNodo[] defs) => _definiciones = defs;

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;
            foreach (var def in _definiciones)
                if (!_estado.Nodos.ContainsKey(def.Id))
                    _estado.Nodos[def.Id] = new EstadoNodo(def.Id);

            ComprobarDesbloqueos();
        }

        public void Inicializar() { }
        public void Actualizar(float delta) { }

        public bool ComprarNivel(string idNodo)
        {
            var def = _definiciones.FirstOrDefault(d => d.Id == idNodo);
            if (def == null) return false;

            var est = _estado.Nodos[idNodo];
            if (!est.Desbloqueado || est.Nivel >= def.NivelMax) return false;

            double coste = def.CosteEnNivel(est.Nivel);
            if (_estado.EnergiaVital < coste) return false;

            _estado.EnergiaVital -= coste;
            est.Nivel++;

            EventBus.Publicar(new EventoNodoDesbloqueado(idNodo));
            ComprobarDesbloqueos();
            return true;
        }

        public void ComprobarDesbloqueos()
        {
            foreach (var def in _definiciones)
            {
                var est = _estado.Nodos[def.Id];
                if (est.Desbloqueado) continue;
                if (def.EraRequerida > _estado.EraActual) continue;

                bool ok = def.NodosPreviosIds.All(id =>
                    _estado.Nodos.TryGetValue(id, out var n) && n.Nivel > 0);

                if (!ok) continue;

                est.Desbloqueado = true;
                EventBus.Publicar(new EventoNodoDesbloqueado(def.Id));
            }
        }

        public DefinicionNodo[] ObtenerPorEra(int era) =>
            _definiciones.Where(d => d.EraRequerida == era).ToArray();

        public DefinicionNodo BuscarDefinicion(string id) =>
            _definiciones.FirstOrDefault(d => d.Id == id);
    }

    // ══════════════════════════════════════════════════════════════════════
    // SISTEMA LOGROS
    // ══════════════════════════════════════════════════════════════════════
    public class SistemaLogros : ISistema
    {
        private readonly DefinicionLogro[] _definiciones;
        private EstadoJuego _estado;

        private float _timerComprobacion;
        private const float INTERVALO_COMPROBACION = 5f;  // comprobar cada 5s

        public SistemaLogros(DefinicionLogro[] defs) => _definiciones = defs;

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;
            foreach (var def in _definiciones)
                if (!_estado.Logros.ContainsKey(def.Id))
                    _estado.Logros[def.Id] = new EstadoLogro(def.Id);
        }

        public void Inicializar() { }

        public void Actualizar(float delta)
        {
            _timerComprobacion -= delta;
            if (_timerComprobacion > 0) return;
            _timerComprobacion = INTERVALO_COMPROBACION;

            ComprobarTodos();
        }

        private void ComprobarTodos()
        {
            var snapshot = CrearSnapshot();

            foreach (var def in _definiciones)
            {
                var est = _estado.Logros[def.Id];
                if (est.Completado) continue;

                try
                {
                    if (!def.Condicion(snapshot)) continue;
                }
                catch { continue; }

                est.Completado = true;
                est.FechaCompletado = DateTime.UtcNow;
                EventBus.Publicar(new EventoLogroDesbloqueado(def.Id));
            }
        }

        private EstadoSnapshot CrearSnapshot() => new EstadoSnapshot
        {
            EV              = _estado.EnergiaVital,
            EVPorSegundo    = _estado.EVPorSegundo,
            Era             = _estado.EraActual,
            TiempoJugado    = _estado.TiempoJugadoTotal,
            VecesPrestige   = _estado.Prestige.VecesTotales,
            Fosiles         = _estado.Prestige.Fosiles,
            Genes           = _estado.Prestige.Genes,
            Quarks          = _estado.Prestige.Quarks,
            SinergiasActivas = _estado.Sinergias.Values.Count(s => s.Activa)
        };

        public int ContarCompletados() => _estado.Logros.Values.Count(l => l.Completado);
    }

    // ══════════════════════════════════════════════════════════════════════
    // SISTEMA OFFLINE
    // ══════════════════════════════════════════════════════════════════════
    public class SistemaOffline : ISistema
    {
        private readonly CalculadorProduccion _calculador;
        private EstadoJuego _estado;

        private const double MAX_HORAS_OFFLINE = 12.0;
        private const string KEY_TIMESTAMP = "terra_timestamp";

        public SistemaOffline(CalculadorProduccion calculador) =>
            _calculador = calculador;

        public void AsignarEstado(EstadoJuego estado) => _estado = estado;
        public void Inicializar() { }
        public void Actualizar(float delta) { }

        public void CalcularGananciaOffline()
        {
            if (!PlayerPrefs.HasKey(KEY_TIMESTAMP)) return;

            long bin = long.Parse(PlayerPrefs.GetString(KEY_TIMESTAMP));
            DateTime ultimaVez = DateTime.FromBinary(bin);
            double segundosOffline = (DateTime.UtcNow - ultimaVez).TotalSeconds;

            if (segundosOffline < 10) return;  // ignorar sesiones muy cortas

            double capSegundos = MAX_HORAS_OFFLINE * 3600;
            double tiempoEfectivo = Math.Min(segundosOffline, capSegundos);

            // Calcular con la producción actual (sin eventos temporales)
            double evPorSegundo = _calculador.Calcular(_estado);
            double evGanada = evPorSegundo * tiempoEfectivo;

            _estado.EnergiaVital += evGanada;
            _estado.TiempoJugadoTotal += tiempoEfectivo;

            EventBus.Publicar(new EventoOfflineCalculado(evGanada, tiempoEfectivo));
        }

        public void GuardarTimestamp() =>
            PlayerPrefs.SetString(KEY_TIMESTAMP, DateTime.UtcNow.ToBinary().ToString());
    }

    // ══════════════════════════════════════════════════════════════════════
    // SISTEMA RACHA DIARIA
    // ══════════════════════════════════════════════════════════════════════
    public class SistemaRacha : ISistema
    {
        private EstadoJuego _estado;

        public void AsignarEstado(EstadoJuego estado) => _estado = estado;
        public void Inicializar() { }
        public void Actualizar(float delta) { }

        public void ComprobarConexionDiaria()
        {
            var racha = _estado.Racha;
            var ahora = DateTime.UtcNow;

            if (racha.UltimaConexion == default)
            {
                // Primera vez
                racha.DiasConsecutivos = 1;
                racha.UltimaConexion = ahora;
                racha.BonusDiarioReclamado = false;
                EventBus.Publicar(new EventoRachaActualizada(racha.DiasConsecutivos));
                return;
            }

            if (racha.RachaRota(ahora))
            {
                racha.DiasConsecutivos = 1;
            }
            else if (racha.EsConexionNueva(ahora))
            {
                racha.DiasConsecutivos++;
                racha.BonusDiarioReclamado = false;
            }

            racha.UltimaConexion = ahora;
            EventBus.Publicar(new EventoRachaActualizada(racha.DiasConsecutivos));
        }

        public bool ReclamarBonusDiario()
        {
            if (_estado.Racha.BonusDiarioReclamado) return false;

            double bonus = _estado.EVPorSegundo * 60 * _estado.Racha.DiasConsecutivos;
            _estado.EnergiaVital += bonus;
            _estado.Racha.BonusDiarioReclamado = true;
            return true;
        }

        public double BonusDiarioPendiente() =>
            _estado.Racha.BonusDiarioReclamado
            ? 0
            : _estado.EVPorSegundo * 60 * _estado.Racha.DiasConsecutivos;
    }
}
