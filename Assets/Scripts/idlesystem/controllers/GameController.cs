using UnityEngine;
using Terra.Core;
using Terra.Data;
using Terra.Data.Catalogos;
using Terra.State;
using Terra.Systems;

namespace Terra.Controllers
{
    /// <summary>
    /// GameController — único MonoBehaviour del sistema idle.
    /// Responsabilidad: inicializar, conectar y orquestar los sistemas.
    /// No contiene lógica de juego — delega en los sistemas correspondientes.
    /// Accesible como singleton desde la UI.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        // ── Sistemas públicos (la UI los consulta) ────────────────────────
        public EstadoJuego Estado           { get; private set; }
        public SistemaMejoras Mejoras       { get; private set; }
        public SistemaSinergias Sinergias   { get; private set; }
        public SistemaPrestige Prestige     { get; private set; }
        public SistemaEras Eras             { get; private set; }
        public SistemaArbol Arbol           { get; private set; }
        public SistemaEventos Eventos       { get; private set; }
        public SistemaLogros Logros         { get; private set; }
        public SistemaEstancamiento Estancamiento { get; private set; }
        public SistemaRacha Racha           { get; private set; }

        // ── Sistemas privados ─────────────────────────────────────────────
        private CalculadorProduccion _calculador;
        private SistemaOffline _offline;
        private SistemaGuardado _guardado;

        // ── Inspector ─────────────────────────────────────────────────────
        [Header("Velocidad de simulación (1 = normal, 60 = debug rápido)")]
        [Range(1f, 3600f)]
        public float VelocidadSimulacion = 1f;

        [Header("Intervalo de actualización de EV (segundos)")]
        [Range(0.05f, 0.5f)]
        public float IntervaloActualizacion = 0.1f;

        private float _timerActualizacion;

        // ══════════════════════════════════════════════════════════════════
        // UNITY LIFECYCLE
        // ══════════════════════════════════════════════════════════════════

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Inicializar();
        }

        void Start()
        {
            CargarYCalcularOffline();
        }

        void Update()
        {
            float deltaReal = Time.deltaTime;
            float deltaSimulado = deltaReal * VelocidadSimulacion;

            _timerActualizacion += deltaSimulado;

            if (_timerActualizacion >= IntervaloActualizacion)
            {
                ActualizarSistemas(_timerActualizacion);
                _timerActualizacion = 0f;
            }

            // Auto-guardado (usa delta real, no simulado)
            _guardado.Tick(deltaReal, Estado);
        }

        void OnApplicationPause(bool pausa)
        {
            if (pausa) _offline.GuardarTimestamp();
            _guardado.Guardar(Estado);
        }

        void OnApplicationQuit()
        {
            _offline.GuardarTimestamp();
            _guardado.Guardar(Estado);
        }

        // ══════════════════════════════════════════════════════════════════
        // INICIALIZACIÓN
        // ══════════════════════════════════════════════════════════════════

        private void Inicializar()
        {
            // 1. Crear catálogos (datos inmutables)
            var defMejoras   = CatalogoMejoras.Crear();
            var defSinergias = CatalogoSinergias.Crear();
            var defEras      = CatalogoEras.Crear();
            var defNodos     = CatalogoNodos.Crear();
            var defEventos   = CatalogoEventos.Crear();
            var defLogros    = CatalogoLogros.Crear();

            // 2. Crear estado
            Estado = new EstadoJuego();

            // 3. Crear calculador (sin dependencias de Unity)
            _calculador = new CalculadorProduccion(defMejoras, defSinergias, defNodos);

            // 4. Crear sistemas
            Mejoras       = new SistemaMejoras(defMejoras);
            Sinergias     = new SistemaSinergias(defSinergias, Mejoras);
            Prestige      = new SistemaPrestige(_calculador);
            Eras          = new SistemaEras(defEras, Mejoras, Sinergias);
            Arbol         = new SistemaArbol(defNodos);
            Eventos       = new SistemaEventos(defEventos);
            Logros        = new SistemaLogros(defLogros);
            Estancamiento = new SistemaEstancamiento(Prestige);
            Racha         = new SistemaRacha();
            _offline      = new SistemaOffline(_calculador);
            _guardado     = new SistemaGuardado();

            // 5. Inyectar estado en sistemas
            Mejoras.AsignarEstado(Estado);
            Sinergias.AsignarEstado(Estado);
            Prestige.AsignarEstado(Estado);
            Eras.AsignarEstado(Estado);
            Arbol.AsignarEstado(Estado);
            Eventos.AsignarEstado(Estado);
            Logros.AsignarEstado(Estado);
            Estancamiento.AsignarEstado(Estado);
            Racha.AsignarEstado(Estado);
            _offline.AsignarEstado(Estado);
        }

        private void CargarYCalcularOffline()
        {
            bool teniaSave = _guardado.Cargar(Estado);

            // Reconectar desbloqueos tras cargar
            Mejoras.ComprobarDesbloqueos();
            Sinergias.Comprobar();
            Arbol.ComprobarDesbloqueos();

            // Comprobar racha diaria
            Racha.ComprobarConexionDiaria();

            // Calcular EV ganada offline
            if (teniaSave)
                _offline.CalcularGananciaOffline();

            // Guardar timestamp de esta sesión
            _offline.GuardarTimestamp();

            // Calcular EV/s inicial
            Estado.EVPorSegundo = _calculador.Calcular(Estado);
        }

        // ══════════════════════════════════════════════════════════════════
        // ACTUALIZACIÓN
        // ══════════════════════════════════════════════════════════════════

        private void ActualizarSistemas(float delta)
        {
            // EV
            Estado.EVPorSegundo = _calculador.Calcular(Estado);
            double ganancia = Estado.EVPorSegundo * delta;
            Estado.EnergiaVital += ganancia;
            Estado.EVGanadaEnSesion += ganancia;
            Estado.TiempoJugadoTotal += delta;

            // Eventos de tiempo
            if (Estado.TiempoRestanteEvento > 0)
                Estado.TiempoRestanteEvento -= delta;

            // Sistemas con tick
            Sinergias.Actualizar(delta);
            Eventos.Actualizar(delta);
            Logros.Actualizar(delta);
            Estancamiento.Actualizar(delta);

            // Publicar cambio de EV
            EventBus.Publicar(new EventoEVCambia(Estado.EnergiaVital));
        }

        // ══════════════════════════════════════════════════════════════════
        // API PÚBLICA (llamada desde la UI)
        // ══════════════════════════════════════════════════════════════════

        public bool ComprarMejora(string id)         => Mejoras.ComprarUno(id);
        public int  ComprarMejoraMax(string id)      => Mejoras.ComprarMax(id);
        public bool ComprarNodoArbol(string id)      => Arbol.ComprarNivel(id);
        public void HacerPrestige(TipoPrestige tipo) => Prestige.Realizar(tipo);
        public void AvanzarEra()                     => Eras.AvanzarEra();
        public bool PuedeAvanzarEra()                => Eras.PuedeAvanzar();
        public double ProgresoEra()                  => Eras.ProgresoHaciaEra();
        public void AceptarEvento(string id)         => Eventos.AceptarEvento(id);
        public void RechazarEvento()                 => Eventos.RechazarEvento();
        public bool ReclamarBonusDiario()            => Racha.ReclamarBonusDiario();

        /// <summary>Aplica un multiplicador temporal de EV — usado por MeteoroManager.</summary>
        public void AplicarEventoTemporal(float multiplicador, float duracionSegundos)
        {
            Estado.MultiplicadorEvento    = multiplicador;
            Estado.TiempoRestanteEvento   = duracionSegundos;
        }

        // ══════════════════════════════════════════════════════════════════
        // DEBUG (solo en editor)
        // ══════════════════════════════════════════════════════════════════

#if UNITY_EDITOR
        [ContextMenu("DEBUG → Añadir 1M EV")]
        void Debug_AñadirEV() => Estado.EnergiaVital += 1_000_000;

        [ContextMenu("DEBUG → Avanzar era")]
        void Debug_AvanzarEra() => Eras.AvanzarEra();

        [ContextMenu("DEBUG → Forzar prestige Extinción")]
        void Debug_Prestige() => Prestige.Realizar(TipoPrestige.Extincion);

        [ContextMenu("DEBUG → Borrar partida")]
        void Debug_BorrarPartida()
        {
            _guardado.BorrarGuardado();
            UnityEditor.EditorApplication.isPlaying = false;
        }

        void OnGUI()
        {
            if (!Application.isPlaying) return;
            GUILayout.Label($"EV: {Formateador.Numero(Estado.EnergiaVital)}");
            GUILayout.Label($"EV/s: {Formateador.Numero(Estado.EVPorSegundo)}");
            GUILayout.Label($"Era: {Estado.EraActual}");
            GUILayout.Label($"Sinergias: {Sinergias.ContarActivas()}/12");
            GUILayout.Label($"Logros: {Logros.ContarCompletados()}");
            GUILayout.Label($"Prestige: ×{Estado.Prestige.MultiplicadorTotal:F2}");
        }
#endif
    }
}
