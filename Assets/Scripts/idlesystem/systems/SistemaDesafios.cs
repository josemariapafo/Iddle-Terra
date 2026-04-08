using System.Linq;
using Terra.Core;
using Terra.Data;
using Terra.State;

namespace Terra.Systems
{
    /// <summary>
    /// Gestiona los desafíos activos: iniciarlos aplicando sus restricciones,
    /// comprobar la condición de victoria cada tick, completarlos otorgando
    /// un multiplicador permanente, o abandonarlos sin recompensa.
    /// </summary>
    public class SistemaDesafios : ISistema
    {
        private readonly DefinicionDesafio[] _definiciones;
        private EstadoJuego _estado;

        public SistemaDesafios(DefinicionDesafio[] defs) => _definiciones = defs;

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;
            foreach (var def in _definiciones)
                if (!_estado.Desafios.ContainsKey(def.Id))
                    _estado.Desafios[def.Id] = new EstadoDesafio(def.Id);

            RecalcularBonusAcumulado();
        }

        public void Inicializar() { }

        public void Actualizar(float delta)
        {
            if (string.IsNullOrEmpty(_estado.DesafioActivoId)) return;

            // Contador de tiempo para desafíos con límite
            if (_estado.TiempoRestanteDesafio > 0)
            {
                _estado.TiempoRestanteDesafio -= delta;
                if (_estado.TiempoRestanteDesafio <= 0)
                {
                    // Tiempo agotado: ¿victoria o abandono?
                    if (ComprobarVictoria())
                        CompletarDesafio();
                    else
                        AbandonarDesafio();
                    return;
                }
            }

            if (ComprobarVictoria())
                CompletarDesafio();
        }

        public DefinicionDesafio BuscarDefinicion(string id) =>
            _definiciones.FirstOrDefault(d => d.Id == id);

        public DefinicionDesafio[] ObtenerTodos() => _definiciones;

        // ── IniciarDesafio ────────────────────────────────────────────────

        public bool IniciarDesafio(string id)
        {
            // No se puede iniciar otro si ya hay uno activo
            if (!string.IsNullOrEmpty(_estado.DesafioActivoId)) return false;

            var def = BuscarDefinicion(id);
            if (def == null) return false;

            // Comprobar era mínima
            if (_estado.EraActual < def.EraRequerida) return false;

            // Comprobar que no esté ya completado
            if (_estado.Desafios.TryGetValue(id, out var est) && est.Completado)
                return false;

            // Limpiar cualquier restricción residual antes de aplicar las nuevas
            LimpiarRestricciones();

            _estado.DesafioActivoId = id;
            if (est != null)
            {
                est.Activo = true;
                est.InicioDesafio = System.DateTime.UtcNow;
            }

            // Aplicar restricciones del desafío
            def.AplicarRestriccion?.Invoke(_estado);
            return true;
        }

        // ── ComprobarVictoria ─────────────────────────────────────────────

        public bool ComprobarVictoria()
        {
            if (string.IsNullOrEmpty(_estado.DesafioActivoId)) return false;

            var def = BuscarDefinicion(_estado.DesafioActivoId);
            if (def?.CondicionVictoria == null) return false;

            var snapshot = CrearSnapshot();
            try { return def.CondicionVictoria(snapshot); }
            catch { return false; }
        }

        // ── CompletarDesafio ──────────────────────────────────────────────

        public void CompletarDesafio()
        {
            if (string.IsNullOrEmpty(_estado.DesafioActivoId)) return;

            var def = BuscarDefinicion(_estado.DesafioActivoId);
            if (def == null) return;

            if (_estado.Desafios.TryGetValue(def.Id, out var est))
            {
                if (est.Completado) return; // idempotente
                est.Activo = false;
                est.Completado = true;
            }

            // Aplicar recompensa permanente
            _estado.BonusDesafiosCompletados *= def.BonusMultiplicador;

            LimpiarRestricciones();
            _estado.DesafioActivoId = null;

            EventBus.Publicar(new EventoDesafioCompletado(def.Id));
        }

        // ── AbandonarDesafio ──────────────────────────────────────────────

        public void AbandonarDesafio()
        {
            if (string.IsNullOrEmpty(_estado.DesafioActivoId)) return;

            if (_estado.Desafios.TryGetValue(_estado.DesafioActivoId, out var est))
                est.Activo = false;

            LimpiarRestricciones();
            _estado.DesafioActivoId = null;
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private void LimpiarRestricciones()
        {
            for (int i = 0; i < _estado.PilaresBloqueadosDesafio.Length; i++)
                _estado.PilaresBloqueadosDesafio[i] = false;
            _estado.CadenasBloqueadasDesafio = false;
            _estado.PrestigeBloqueadoDesafio = false;
            _estado.SoloZona0Desafio = false;
            _estado.MaxComprasDesafio = 0;
            _estado.ComprasEnDesafio = 0;
            _estado.TiempoRestanteDesafio = 0f;
        }

        /// <summary>
        /// Recalcula el multiplicador acumulado de desafíos completados desde cero.
        /// Útil tras cargar partida o tras un reset.
        /// </summary>
        public void RecalcularBonusAcumulado()
        {
            double acumulado = 1.0;
            foreach (var def in _definiciones)
                if (_estado.Desafios.TryGetValue(def.Id, out var est) && est.Completado)
                    acumulado *= def.BonusMultiplicador;
            _estado.BonusDesafiosCompletados = acumulado;
        }

        private EstadoSnapshot CrearSnapshot() => new EstadoSnapshot
        {
            EV               = _estado.EnergiaVital,
            EVPorSegundo     = _estado.EVPorSegundo,
            Era              = _estado.EraActual,
            TiempoJugado     = _estado.TiempoJugadoTotal,
            VecesPrestige    = _estado.Prestige.VecesTotales,
            Fosiles          = _estado.Prestige.Fosiles,
            Genes            = _estado.Prestige.Genes,
            Quarks           = _estado.Prestige.Quarks,
            SinergiasActivas = _estado.Sinergias.Values.Count(s => s.Activa)
        };

        // ── Consultas ─────────────────────────────────────────────────────

        public bool HayDesafioActivo() => !string.IsNullOrEmpty(_estado.DesafioActivoId);

        public DefinicionDesafio DesafioActivo() =>
            HayDesafioActivo() ? BuscarDefinicion(_estado.DesafioActivoId) : null;

        public bool EstaCompletado(string id) =>
            _estado.Desafios.TryGetValue(id, out var est) && est.Completado;

        public int ContarCompletados() =>
            _estado.Desafios.Values.Count(d => d.Completado);
    }
}
