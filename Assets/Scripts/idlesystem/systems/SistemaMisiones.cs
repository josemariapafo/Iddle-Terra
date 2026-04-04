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
    /// Al completar objetivo → pasa a Completadas (sin recompensa automática).
    /// El jugador recoge la recompensa manualmente desde la UI.
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

        private void OnSinergia(EventoSinergiaActivada evt) { }
        private void OnEra(EventoEraAvanzada evt) { }
        private void OnPrestige(EventoPrestigeRealizado evt) { }

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
                    if (def.Id.Contains("pre_01"))
                        est.ProgresoActual = _estado.Prestige.VecesExtincion;
                    else if (def.Id.Contains("pre_02"))
                        est.ProgresoActual = _estado.Prestige.Fosiles;
                    else
                        est.ProgresoActual = _estado.Prestige.VecesTotales;
                    break;
            }
        }

        // ── Completar (sin recompensa — el jugador la recoge) ─────────────

        private void CompletarMision(int slot, DefinicionMision def)
        {
            _estado.MisionesActivas[slot].Completada = true;

            // Mover a completadas (recompensa pendiente)
            _estado.MisionesCompletadas.Add(new MisionCompletada(def.Id));

            EventBus.Publicar(new EventoMisionCompletada(def.Id));
            Debug.Log($"[Misiones] Completada: {def.Nombre} — recompensa pendiente de recoger");

            // Asignar siguiente misión al slot
            AsignarMisionAlSlot(slot);
        }

        // ── Recoger recompensa (llamado desde UI) ─────────────────────────

        public bool RecogerRecompensa(string misionId)
        {
            var completada = _estado.MisionesCompletadas.Find(m => m.Id == misionId);
            if (completada == null || completada.RecompensaRecogida) return false;

            var def = BuscarDefinicion(misionId);
            if (def == null) return false;

            // Todas las recompensas son EV por ahora
            _estado.EnergiaVital += _estado.EVPorSegundo * def.ValorRecompensa;

            completada.RecompensaRecogida = true;
            Debug.Log($"[Misiones] Recompensa recogida: {def.Nombre} (+{def.ValorRecompensa}s de EV)");
            return true;
        }

        /// <summary>Devuelve true si hay al menos una misión completada con recompensa pendiente.</summary>
        public bool HayRecompensaPendiente()
        {
            foreach (var m in _estado.MisionesCompletadas)
                if (!m.RecompensaRecogida) return true;
            return false;
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
            var activos = new HashSet<string>();
            for (int i = 0; i < 3; i++)
                if (_estado.MisionesActivas[i] != null && !string.IsNullOrEmpty(_estado.MisionesActivas[i].Id))
                    activos.Add(_estado.MisionesActivas[i].Id);

            var completadaIds = new HashSet<string>();
            foreach (var m in _estado.MisionesCompletadas)
                completadaIds.Add(m.Id);

            foreach (var def in _definiciones)
            {
                if (completadaIds.Contains(def.Id)) continue;
                if (activos.Contains(def.Id)) continue;
                if (def.EraMinima > _estado.EraActual) continue;
                if (!EsDisponible(def, completadaIds)) continue;

                pool.Add(def);
            }

            return pool;
        }

        private bool EsDisponible(DefinicionMision def, HashSet<string> completadaIds)
        {
            foreach (var otra in _definiciones)
            {
                if (otra.MisionSiguienteId == def.Id)
                    return completadaIds.Contains(otra.Id);
            }
            return true;
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

        public DefinicionMision[] ObtenerTodasDefiniciones() => _definiciones;
    }
}
