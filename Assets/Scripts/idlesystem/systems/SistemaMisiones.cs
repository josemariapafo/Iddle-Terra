using System.Collections.Generic;
using System.Linq;
using Terra.Core;
using Terra.Data;
using Terra.State;
using UnityEngine;

namespace Terra.Systems
{
    /// <summary>
    /// Gestiona 3 misiones activas simultáneas.
    /// Se suscribe a eventos del bus para trackear progreso automáticamente.
    /// Al completar, otorga recompensa y rota la siguiente misión del pool.
    /// </summary>
    public class SistemaMisiones : ISistema
    {
        private readonly DefinicionMision[] _definiciones;
        private readonly CalculadorProduccion _calculador;
        private EstadoJuego _estado;

        private float _timerComprobacion;
        private const float INTERVALO = 5f;

        // Contadores de sesión (se resetean al cerrar)
        private int _comprasTotales;
        private int _combosTotales;
        private int _comprasCadena;

        public SistemaMisiones(DefinicionMision[] defs, CalculadorProduccion calculador)
        {
            _definiciones = defs;
            _calculador = calculador;
        }

        public void Inicializar() { }

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;

            // Si no hay misiones activas, rellenar los 3 slots
            for (int i = 0; i < 3; i++)
                if (_estado.MisionesActivas[i] == null || string.IsNullOrEmpty(_estado.MisionesActivas[i].Id))
                    AsignarMisionAlSlot(i);

            SuscribirEventos();
        }

        public void Actualizar(float delta)
        {
            _timerComprobacion -= delta;
            if (_timerComprobacion > 0) return;
            _timerComprobacion = INTERVALO;

            ComprobarMisiones();
        }

        // ── Suscripción a eventos ─────────────────────────────────────────

        private void SuscribirEventos()
        {
            EventBus.Suscribir<EventoMejoraComprada>(OnMejoraComprada);
            EventBus.Suscribir<EventoSinergiaActivada>(OnSinergia);
            EventBus.Suscribir<EventoEraAvanzada>(OnEra);
            EventBus.Suscribir<EventoPrestigeRealizado>(OnPrestige);
            EventBus.Suscribir<EventoComboActivado>(OnCombo);
            EventBus.Suscribir<EventoCadenaComprada>(OnCadena);
        }

        private void OnMejoraComprada(EventoMejoraComprada evt)
        {
            _comprasTotales++;
            IncrementarMisionesDeTipo(TipoMision.Compra, 1);
        }

        private void OnSinergia(EventoSinergiaActivada evt)
        {
            // Las sinergias se evalúan por cantidad total, no incremento
        }

        private void OnEra(EventoEraAvanzada evt)
        {
            // Las eras se evalúan por valor actual, no incremento
        }

        private void OnPrestige(EventoPrestigeRealizado evt)
        {
            // El prestige se evalúa por contadores totales
        }

        private void OnCombo(EventoComboActivado evt)
        {
            _combosTotales++;
            IncrementarMisionesDeTipo(TipoMision.Combo, 1);
        }

        private void OnCadena(EventoCadenaComprada evt)
        {
            _comprasCadena++;
            IncrementarMisionesDeTipo(TipoMision.CuelloBotella, 1);
        }

        private void IncrementarMisionesDeTipo(TipoMision tipo, double cantidad)
        {
            for (int i = 0; i < 3; i++)
            {
                var est = _estado.MisionesActivas[i];
                if (est == null || est.Completada) continue;

                var def = BuscarDefinicion(est.Id);
                if (def == null || def.Tipo != tipo) continue;

                est.ProgresoActual += cantidad;
            }
        }

        // ── Comprobación de misiones ──────────────────────────────────────

        private void ComprobarMisiones()
        {
            for (int i = 0; i < 3; i++)
            {
                var est = _estado.MisionesActivas[i];
                if (est == null || est.Completada) continue;

                var def = BuscarDefinicion(est.Id);
                if (def == null) continue;

                // Actualizar progreso de tipos que se evalúan por estado actual
                ActualizarProgresoEstado(est, def);

                if (est.ProgresoActual >= def.ValorObjetivo)
                    CompletarMision(i, def);
            }
        }

        private void ActualizarProgresoEstado(EstadoMision est, DefinicionMision def)
        {
            switch (def.Tipo)
            {
                case TipoMision.Produccion:
                    est.ProgresoActual = _estado.EVPorSegundo;
                    break;
                case TipoMision.Sinergia:
                    int sinActivas = 0;
                    foreach (var s in _estado.Sinergias.Values)
                        if (s.Activa) sinActivas++;
                    est.ProgresoActual = sinActivas;
                    break;
                case TipoMision.Era:
                    est.ProgresoActual = _estado.EraActual;
                    break;
                case TipoMision.Prestige:
                    // Evaluar según el id de la misión
                    if (def.Id.Contains("pre_01"))
                        est.ProgresoActual = _estado.Prestige.VecesExtincion;
                    else if (def.Id.Contains("pre_02"))
                        est.ProgresoActual = _estado.Prestige.Fosiles;
                    else
                        est.ProgresoActual = _estado.Prestige.VecesTotales;
                    break;
            }
        }

        // ── Completar y recompensar ───────────────────────────────────────

        private void CompletarMision(int slot, DefinicionMision def)
        {
            _estado.MisionesActivas[slot].Completada = true;
            _estado.MisionesCompletadas.Add(def.Id);

            OtorgarRecompensa(def);
            EventBus.Publicar(new EventoMisionCompletada(def.Id));

            Debug.Log($"[Misiones] Completada: {def.Nombre} — Recompensa: {def.TipoRecompensa}");

            // Asignar siguiente misión al slot
            AsignarMisionAlSlot(slot);
        }

        private void OtorgarRecompensa(DefinicionMision def)
        {
            switch (def.TipoRecompensa)
            {
                case TipoRecompensa.EVInstante:
                    // ValorRecompensa = segundos de producción actual
                    _estado.EnergiaVital += _estado.EVPorSegundo * def.ValorRecompensa;
                    break;

                case TipoRecompensa.MultiplicadorTemporal:
                    _estado.MultiplicadorEvento = def.ValorRecompensa;
                    _estado.TiempoRestanteEvento = 30f;
                    break;

                case TipoRecompensa.FosilesExtra:
                    _estado.Prestige.Fosiles += def.ValorRecompensa;
                    break;

                case TipoRecompensa.NivelNodoGratis:
                    // Buscar un nodo desbloqueado que no esté al max
                    foreach (var kv in _estado.Nodos)
                    {
                        if (kv.Value.Desbloqueado && kv.Value.Nivel < 10)
                        {
                            kv.Value.Nivel++;
                            break;
                        }
                    }
                    break;
            }
        }

        // ── Pool y asignación ─────────────────────────────────────────────

        private void AsignarMisionAlSlot(int slot)
        {
            var disponibles = ObtenerPool();
            if (disponibles.Count == 0)
            {
                _estado.MisionesActivas[slot] = new EstadoMision("");
                return;
            }

            int idx = Random.Range(0, disponibles.Count);
            var def = disponibles[idx];
            _estado.MisionesActivas[slot] = new EstadoMision(def.Id);
        }

        private List<DefinicionMision> ObtenerPool()
        {
            var pool = new List<DefinicionMision>();
            // IDs ya activos en los 3 slots
            var activos = new HashSet<string>();
            for (int i = 0; i < 3; i++)
                if (_estado.MisionesActivas[i] != null && !string.IsNullOrEmpty(_estado.MisionesActivas[i].Id))
                    activos.Add(_estado.MisionesActivas[i].Id);

            foreach (var def in _definiciones)
            {
                // Ya completada
                if (_estado.MisionesCompletadas.Contains(def.Id)) continue;
                // Ya activa en otro slot
                if (activos.Contains(def.Id)) continue;
                // Era no alcanzada
                if (def.EraMinima > _estado.EraActual) continue;
                // Si es progresiva, verificar que la anterior esté completada
                if (!EsDisponible(def)) continue;

                pool.Add(def);
            }

            return pool;
        }

        private bool EsDisponible(DefinicionMision def)
        {
            // Si es la primera de su cadena (no hay previa que la apunte), disponible
            // Si alguna otra misión tiene MisionSiguienteId == def.Id, esa debe estar completada
            foreach (var otra in _definiciones)
            {
                if (otra.MisionSiguienteId == def.Id)
                    return _estado.MisionesCompletadas.Contains(otra.Id);
            }
            return true; // nadie la apunta → es inicio de cadena
        }

        // ── Consultas ─────────────────────────────────────────────────────

        public DefinicionMision BuscarDefinicion(string id)
        {
            foreach (var d in _definiciones)
                if (d.Id == id) return d;
            return null;
        }

        public DefinicionMision ObtenerDefinicionSlot(int slot)
        {
            if (slot < 0 || slot >= 3) return null;
            var est = _estado.MisionesActivas[slot];
            if (est == null || string.IsNullOrEmpty(est.Id)) return null;
            return BuscarDefinicion(est.Id);
        }

        public int MisionesCompletadasTotal() => _estado.MisionesCompletadas.Count;
    }
}
