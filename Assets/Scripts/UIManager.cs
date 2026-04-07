using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Terra.Controllers;
using Terra.Core;
using Terra.Data;
using Terra.State;
using Terra.Systems;

/// <summary>
/// UIManager v2 — conecta toda la UI con el sistema idle.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // ── Referencias externas ─────────────────────────────────────────────
    [Header("Referencias")]
    public EraManager eraManager;

    // ── Pantallas ─────────────────────────────────────────────────────────
    [Header("Pantallas")]
    public GameObject Panel_Principal;
    public GameObject Panel_Categoria;
    public GameObject Panel_Prestige;
    public GameObject Panel_Evento;
    public GameObject Panel_EraDesbloqueada;

    // ── HUD Principal ─────────────────────────────────────────────────────
    [Header("HUD Principal")]
    public TextMeshProUGUI Text_EVTotal;
    public TextMeshProUGUI Text_EVPorSegundo;
    public TextMeshProUGUI Text_Era;
    public Slider Slider_ProgresoEra;
    public TextMeshProUGUI Text_ProgresoEra;
    public Button Btn_Evolucionar;
    public TextMeshProUGUI Text_BtnEvolucionar;

    // ── Botones categorías ────────────────────────────────────────────────
    [Header("Botones categorias")]
    public Button Btn_Atmosfera;
    public Button Btn_Oceanos;
    public Button Btn_Tierra;
    public Button Btn_Vida;

    // ── Panel Categoría ───────────────────────────────────────────────────
    [Header("Panel Categoria")]
    public TextMeshProUGUI Text_NombreCategoria;
    public TextMeshProUGUI Text_ProduccionCategoria;
    public Transform Contenedor_Zona0;
    public Transform Contenedor_Zona1;
    public Transform Contenedor_Zona2;
    public TextMeshProUGUI Text_TituloZona0;
    public TextMeshProUGUI Text_TituloZona1;
    public TextMeshProUGUI Text_TituloZona2;
    public Button Btn_CerrarCategoria;
    public GameObject Prefab_TarjetaMejora;

    // ── Cadenas (pestañas en Panel Categoria) ─────────────────────────────
    [Header("Cadenas")]
    public Button Btn_TabProduccion;
    public Button Btn_TabInfraestructura;
    public TextMeshProUGUI Text_ResumenCadenas;

    // ── Panel Prestige ────────────────────────────────────────────────────
    [Header("Panel Prestige")]
    public Button Btn_AbrirPrestige;
    public TextMeshProUGUI Text_GananciaExtincion;
    public TextMeshProUGUI Text_GananciaGlaciacion;
    public TextMeshProUGUI Text_GananciaBigBang;
    public Button Btn_Extincion;
    public Button Btn_Glaciacion;
    public Button Btn_BigBang;
    public TextMeshProUGUI Text_FosilesActuales;
    public TextMeshProUGUI Text_GenesActuales;
    public TextMeshProUGUI Text_QuarksActuales;
    public Button Btn_CerrarPrestige;

    // ── Panel Evento ──────────────────────────────────────────────────────
    [Header("Panel Evento")]
    public TextMeshProUGUI Text_NombreEvento;
    public TextMeshProUGUI Text_DescripcionEvento;
    public Button Btn_AceptarEvento;
    public Button Btn_RechazarEvento;

    // ── Panel Era Desbloqueada ────────────────────────────────────────────
    [Header("Panel Era Desbloqueada")]
    public TextMeshProUGUI Text_NombreEra;
    public TextMeshProUGUI Text_DescripcionEra;
    public Button Btn_ContinuarEra;

    // ── Panel Evolución ─────────────────────────────────────────────────
    [Header("Panel Evolucion")]
    public GameObject Panel_Evolucion;
    public Transform Contenedor_Nodos;
    public GameObject Prefab_NodoArbol;
    public Button Btn_AbrirEvolucion;
    public Button Btn_CerrarEvolucion;

    [Header("Popup Nodo")]
    public GameObject Panel_PopupNodo;
    public TextMeshProUGUI Text_PopupNombre;
    public TextMeshProUGUI Text_PopupDescripcion;
    public TextMeshProUGUI Text_PopupEfecto;
    public TextMeshProUGUI Text_PopupCoste;
    public Button Btn_ComprarNodo;
    public Button Btn_CerrarPopup;

    // ── Panel Códice Fósil ──────────────────────────────────────────────
    [Header("Codice Fosil")]
    public GameObject Panel_Codice;
    public Button Btn_AbrirCodice;
    public Button Btn_CerrarCodice;
    public Transform Contenedor_NodosCodice;
    public GameObject Prefab_NodoCodice;
    public TextMeshProUGUI Text_FosilesCodice;

    [Header("Popup Nodo Codice")]
    public GameObject Panel_PopupCodice;
    public TextMeshProUGUI Text_PopupCodiceNombre;
    public TextMeshProUGUI Text_PopupCodiceDescripcion;
    public TextMeshProUGUI Text_PopupCodiceEfecto;
    public TextMeshProUGUI Text_PopupCodiceCoste;
    public Button Btn_ComprarNodoCodice;
    public Button Btn_CerrarPopupCodice;

    // ── Panel Misiones ──────────────────────────────────────────────────
    [Header("Misiones")]
    public GameObject Panel_Misiones;
    public Button Btn_AbrirMisiones;

    [Header("Banner Mision Completada")]
    public GameObject Panel_BannerMision;
    public TextMeshProUGUI Text_BannerMision;

    // ── Indicador estancamiento ───────────────────────────────────────────
    [Header("Indicador estancamiento")]
    public GameObject Indicador_Estancamiento;

    // ── Racha diaria ──────────────────────────────────────────────────────
    [Header("Racha diaria")]
    public GameObject Panel_BonusDiario;
    public TextMeshProUGUI Text_BonusDiario;
    public Button Btn_ReclamarBonus;

    // ── Meta proxima ───────────────────────────────────────────────────────
    [Header("Meta proxima visible")]
    public GameObject Panel_MetaProxima;
    public TextMeshProUGUI Text_MetaNombre;
    public TextMeshProUGUI Text_MetaProgreso;
    public Slider Slider_MetaProgreso;
    public Button Btn_CerrarMeta;

    private float _timerSinComprar = 0f;
    private float _timerReaparicion = 0f;
    private const float TIEMPO_SIN_COMPRAR = 30f;
    private const float TIEMPO_REAPARICION = 30f;
    private const float UMBRAL_PROGRESO = 0.8f;  // mejora al 80% alcanzada

    // ── Internas ──────────────────────────────────────────────────────────
    private TipoPilar _categoriaActual = TipoPilar.Atmosfera;
    private bool _tabInfraestructura = false;
    private float _timerUI = 0f;
    private const float INTERVALO_UI = 0.2f;
    private readonly List<(GameObject tarjeta, DefinicionMejora def)> _tarjetasActivas
        = new List<(GameObject, DefinicionMejora)>();
    private readonly List<(GameObject tarjeta, DefinicionSubMejoraCadena def)> _tarjetasCadenaActivas
        = new List<(GameObject, DefinicionSubMejoraCadena)>();
    private readonly List<(GameObject nodo, DefinicionNodo def)> _nodosActivos
        = new List<(GameObject, DefinicionNodo)>();
    private string _nodoSeleccionadoId;
    private string _nodoCodiceSeleccionadoId;
    private readonly List<(GameObject nodo, DefinicionNodoCodice def)> _nodosCodiceActivos
        = new List<(GameObject, DefinicionNodoCodice)>();
    private float _timerBannerMision = 0f;
    private const float DURACION_BANNER_MISION = 3f;

    // ── Revelación progresiva ─────────────────────────────────────────
    private int _eraMaxVisualAnterior;

    // ── Misiones: UI dinámica ─────────────────────────────────────────
    private bool _misionesTabCompletadas = false;
    private Transform _contenedorMisionesActivas;
    private Transform _contenedorMisionesCompletadas;
    private Button _btnTabPorHacer;
    private Button _btnTabCompletadas;
    private TextMeshProUGUI _txtTabPorHacer;
    private TextMeshProUGUI _txtTabCompletadas;
    private TextMeshProUGUI _txtBadgeMisiones;

    // ── Auto-compra (T19): toggles dinámicos en Panel_Categoria ──────
    private Button _btnAutoCompra;
    private TextMeshProUGUI _txtAutoCompra;
    private Button _btnAutoCompraSmart;
    private TextMeshProUGUI _txtAutoCompraSmart;

    static readonly string[][] _zonasNombres =
    {
        new[]{ "Baja atmosfera",  "Nubes",         "Alta atmosfera"  },
        new[]{ "Superficie",      "Zona media",     "Profundidades"   },
        new[]{ "Superficie",      "Corteza",        "Interior"        },
        new[]{ "Microvida",       "Vida compleja",  "Vida inteligente"},
    };

    static readonly string[] _nombresPilar = { "ATMOSFERA", "OCEANOS", "TIERRA", "VIDA" };
    static readonly string[] _eslabonNombres = { "Generación", "Procesamiento", "Distribución" };

    // ─── DEBUG/TESTEO ────────────────────────────────────────────────────
    // TODO: Poner a false antes del release.
    // Activa hotkeys de testeo: F2 = +100 fosiles, F3 = +1000 EV.
    private const bool DEBUG_HOTKEYS_TESTEO = true;

    // ══════════════════════════════════════════════════════════════════════
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    void Start()
    {
        ConstruirUICodiceProgramatico();
        ConfigurarBotones();
        SuscribirEventos();
        InicializarPanelMisiones();
        InicializarRevelacionProgresiva();
        ArreglarLayoutPopupNodo();
        MostrarPantalla(Panel_Principal);
        ComprobarBonusDiario();
    }

    // Reorganiza los textos del Panel_PopupNodo en vertical. La escena tiene
    // los 4 textos en fila con cajas de 200x50 y fuente 36 — no cabe casi nada,
    // sobre todo la descripción. Se ajusta en runtime para no tocar la escena.
    void ArreglarLayoutPopupNodo()
    {
        ReposicionarTextoPopup(Text_PopupNombre,      new Vector2(0,  140), new Vector2(700, 60), 30);
        ReposicionarTextoPopup(Text_PopupDescripcion, new Vector2(0,   70), new Vector2(700, 60), 22);
        ReposicionarTextoPopup(Text_PopupEfecto,      new Vector2(0,    0), new Vector2(700, 50), 22);
        ReposicionarTextoPopup(Text_PopupCoste,       new Vector2(0,  -60), new Vector2(700, 50), 22);
    }

    void ReposicionarTextoPopup(TextMeshProUGUI txt, Vector2 pos, Vector2 size, float fontSize)
    {
        if (txt == null) return;
        var rt = txt.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        txt.fontSize = fontSize;
        txt.alignment = TextAlignmentOptions.Center;
        txt.enableWordWrapping = true;
    }

    void Update()
    {
#pragma warning disable CS0162
        // ─── DEBUG: F2 = +100 fosiles | F3 = +10M EV ─────────────────────
        if (DEBUG_HOTKEYS_TESTEO)
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var gcDbg = GameController.Instance;
                if (gcDbg != null)
                {
                    gcDbg.Estado.Prestige.Fosiles += 100;
                    Debug.Log("[DEBUG] +100 Fosiles. Total: " + gcDbg.Estado.Prestige.Fosiles);
                }
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                var gcDbg = GameController.Instance;
                if (gcDbg != null)
                {
                    gcDbg.Estado.EnergiaVital += 10_000_000;
                    Debug.Log("[DEBUG] +10M EV. Total: " + gcDbg.Estado.EnergiaVital);
                }
            }
        }
#pragma warning restore CS0162

        _timerUI -= Time.deltaTime;
        if (_timerUI > 0) return;
        _timerUI = INTERVALO_UI;
        ActualizarHUD();
        if (Panel_Categoria != null && Panel_Categoria.activeSelf)
        {
            if (_tabInfraestructura) ActualizarTarjetasCadena();
            else ActualizarTarjetas();
        }
        if (Panel_Prestige != null && Panel_Prestige.activeSelf)
            ActualizarPrestige();
        if (Panel_Codice != null && Panel_Codice.activeSelf)
            ActualizarNodosCodice();
        if (Panel_Evolucion != null && Panel_Evolucion.activeSelf)
            ActualizarNodosArbol();
        if (Panel_Misiones != null && Panel_Misiones.activeSelf)
            ActualizarWidgetMisiones();
    }

    void OnDestroy()
    {
        DesuscribirEventos();
    }

    // ══════════════════════════════════════════════════════════════════════
    // SETUP
    // ══════════════════════════════════════════════════════════════════════

    void ConfigurarBotones()
    {
        Btn_Atmosfera?.onClick.AddListener(() => AbrirCategoria(TipoPilar.Atmosfera));
        Btn_Oceanos?.onClick.AddListener(() => AbrirCategoria(TipoPilar.Oceanos));
        Btn_Tierra?.onClick.AddListener(() => AbrirCategoria(TipoPilar.Tierra));
        Btn_Vida?.onClick.AddListener(() => AbrirCategoria(TipoPilar.Vida));

        Btn_Evolucionar?.onClick.AddListener(OnClickEvolucionar);
        Btn_CerrarCategoria?.onClick.AddListener(() => MostrarPantalla(Panel_Principal));
        Btn_AbrirMisiones?.onClick.AddListener(() => { CambiarTabMisiones(false); MostrarPantalla(Panel_Misiones); });
        // Btn_CerrarMisiones se crea dinámicamente en InicializarPanelMisiones
        Btn_AbrirEvolucion?.onClick.AddListener(() => AbrirEvolucion());
        Btn_CerrarEvolucion?.onClick.AddListener(() => MostrarPantalla(Panel_Principal));
        Btn_ComprarNodo?.onClick.AddListener(OnClickComprarNodo);
        Btn_CerrarPopup?.onClick.AddListener(() => Panel_PopupNodo?.SetActive(false));
        Btn_TabProduccion?.onClick.AddListener(() => CambiarTab(false));
        Btn_TabInfraestructura?.onClick.AddListener(() => CambiarTab(true));
        Btn_CerrarMeta?.onClick.AddListener(CerrarMetaManual);
        Btn_AbrirPrestige?.onClick.AddListener(() => MostrarPantalla(Panel_Prestige));
        Btn_CerrarPrestige?.onClick.AddListener(() => MostrarPantalla(Panel_Principal));
        Btn_AbrirCodice?.onClick.AddListener(() => AbrirCodice());
        Btn_CerrarCodice?.onClick.AddListener(() => MostrarPantalla(Panel_Principal));
        Btn_ComprarNodoCodice?.onClick.AddListener(OnClickComprarNodoCodice);
        Btn_CerrarPopupCodice?.onClick.AddListener(() => Panel_PopupCodice?.SetActive(false));

        Btn_Extincion?.onClick.AddListener(() => OnClickPrestige(TipoPrestige.Extincion));
        Btn_Glaciacion?.onClick.AddListener(() => OnClickPrestige(TipoPrestige.Glaciacion));
        Btn_BigBang?.onClick.AddListener(() => OnClickPrestige(TipoPrestige.BigBang));

        Btn_AceptarEvento?.onClick.AddListener(OnClickAceptarEvento);
        Btn_RechazarEvento?.onClick.AddListener(OnClickRechazarEvento);
        Btn_ContinuarEra?.onClick.AddListener(() =>
        {
            int era = GameController.Instance?.Estado.EraActual ?? 0;
            Debug.Log($"[UIManager] ContinuarEra pulsado. Era={era} eraManager={eraManager != null}");
            if (eraManager != null)
                eraManager.AplicarEraVisual(era);
            else
                Debug.LogError("[UIManager] eraManager no asignado en el Inspector");
            Panel_EraDesbloqueada?.SetActive(false);
            MostrarPantalla(Panel_Principal);
        });
        Btn_ReclamarBonus?.onClick.AddListener(OnClickReclamarBonus);
    }

    void SuscribirEventos()
    {
        EventBus.Suscribir<EventoEraAvanzada>(OnEraAvanzada);
        EventBus.Suscribir<EventoEventoActivado>(OnEventoActivado);
        EventBus.Suscribir<EventoEstancamientoDetectado>(OnEstancamiento);
        EventBus.Suscribir<EventoSinergiaActivada>(OnSinergiaActivada);
        EventBus.Suscribir<EventoLogroDesbloqueado>(OnLogroDesbloqueado);
        EventBus.Suscribir<EventoMejoraDesbloqueada>(OnMejoraDesbloqueada);
        EventBus.Suscribir<EventoMisionCompletada>(OnMisionCompletada);
        EventBus.Suscribir<EventoPrestigeRealizado>(OnPrestigeRealizado);
    }

    void DesuscribirEventos()
    {
        EventBus.Desuscribir<EventoEraAvanzada>(OnEraAvanzada);
        EventBus.Desuscribir<EventoEventoActivado>(OnEventoActivado);
        EventBus.Desuscribir<EventoEstancamientoDetectado>(OnEstancamiento);
        EventBus.Desuscribir<EventoSinergiaActivada>(OnSinergiaActivada);
        EventBus.Desuscribir<EventoLogroDesbloqueado>(OnLogroDesbloqueado);
        EventBus.Desuscribir<EventoMejoraDesbloqueada>(OnMejoraDesbloqueada);
        EventBus.Desuscribir<EventoMisionCompletada>(OnMisionCompletada);
        EventBus.Desuscribir<EventoPrestigeRealizado>(OnPrestigeRealizado);
    }

    // ══════════════════════════════════════════════════════════════════════
    // HUD PRINCIPAL
    // ══════════════════════════════════════════════════════════════════════

    void ActualizarHUD()
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        var estado = gc.Estado;

        if (Text_EVTotal != null)
            Text_EVTotal.text = Formateador.Numero(estado.EnergiaVital) + " EV";

        if (Text_EVPorSegundo != null)
            Text_EVPorSegundo.text = "+" + Formateador.Numero(estado.EVPorSegundo) + "/seg";

        if (Text_Era != null)
            Text_Era.text = "Era " + estado.EraActual + ": " + gc.Eras.ObtenerDefinicionActual().Nombre;

        double progreso = gc.ProgresoEra();
        if (Slider_ProgresoEra != null)
            Slider_ProgresoEra.value = (float)progreso;
        if (Text_ProgresoEra != null)
            Text_ProgresoEra.text = Formateador.Porcentaje(progreso);

        bool puedeEvolucionar = gc.PuedeAvanzarEra();
        if (Btn_Evolucionar != null)
            Btn_Evolucionar.interactable = puedeEvolucionar;
        if (Text_BtnEvolucionar != null)
        {
            if (puedeEvolucionar)
            {
                Text_BtnEvolucionar.text = "EVOLUCIONAR";
            }
            else
            {
                var cond = gc.Eras.ObtenerCondicionSiguiente();
                if (cond != null && progreso >= 1.0 && estado.EnergiaVital < cond.EVRequerida)
                    Text_BtnEvolucionar.text = "EVOLUCIONAR\nFaltan "
                        + Formateador.Numero(cond.EVRequerida - estado.EnergiaVital) + " EV";
                else
                    Text_BtnEvolucionar.text = "EVOLUCIONAR\n" + Formateador.Porcentaje(progreso);
            }
        }

        if (Indicador_Estancamiento != null)
            Indicador_Estancamiento.SetActive(gc.Estancamiento.EstaEstancado());

        // ── Revelación progresiva de botones ──
        ActualizarRevelacionProgresiva(gc, estado);

        ActualizarResumenCadenas();
        ActualizarMetaProxima();
        ActualizarBannerMision();
    }

    // ══════════════════════════════════════════════════════════════════════
    // REVELACIÓN PROGRESIVA
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Al arrancar, oculta todo lo que el jugador aún no ha descubierto.
    /// Usa EraMaximaAlcanzada (permanente, sobrevive prestige).
    /// </summary>
    void InicializarRevelacionProgresiva()
    {
        var gc = GameController.Instance;
        int eraMax = gc != null ? gc.Estado.EraMaximaAlcanzada : 1;
        _eraMaxVisualAnterior = eraMax;

        // Pilares: Tierra(1), Oceanos(2), Atmosfera(3), Vida(4)
        SetVisible(Btn_Tierra, true);                // siempre visible
        SetVisible(Btn_Oceanos, eraMax >= 2);
        SetVisible(Btn_Atmosfera, eraMax >= 3);
        SetVisible(Btn_Vida, eraMax >= 4);

        // Funcionalidades
        SetVisible(Btn_AbrirEvolucion, eraMax >= 2);
        SetVisible(Btn_AbrirMisiones, eraMax >= 2);
        SetVisible(Btn_AbrirPrestige, eraMax >= 3);
        // Códice visible tras primer prestige (tiene fósiles)
        var gc2 = GameController.Instance;
        bool tienePrestige = gc2 != null && gc2.Estado.Prestige.VecesTotales > 0;
        SetVisible(Btn_AbrirCodice, tienePrestige);
    }

    void ActualizarRevelacionProgresiva(GameController gc, EstadoJuego estado)
    {
        int eraMax = estado.EraMaximaAlcanzada;

        // Solo comprobar si la era máxima subió desde el último check
        if (eraMax > _eraMaxVisualAnterior)
        {
            // Desbloquear lo nuevo de esta era
            if (eraMax >= 2 && _eraMaxVisualAnterior < 2)
            {
                Revelar(Btn_Oceanos, "Oceanos");
                Revelar(Btn_AbrirEvolucion, "Evolucion");
                Revelar(Btn_AbrirMisiones, "Misiones");
            }
            if (eraMax >= 3 && _eraMaxVisualAnterior < 3)
            {
                Revelar(Btn_Atmosfera, "Atmosfera");
                Revelar(Btn_AbrirPrestige, "Prestige");
            }
            if (eraMax >= 4 && _eraMaxVisualAnterior < 4)
            {
                Revelar(Btn_Vida, "Vida");
            }

            _eraMaxVisualAnterior = eraMax;
        }

        // Codice Fosil: revelar cuando el jugador hace su primer prestige
        if (Btn_AbrirCodice != null && !Btn_AbrirCodice.gameObject.activeSelf
            && estado.Prestige.VecesTotales > 0)
        {
            Revelar(Btn_AbrirCodice, "Codice Fosil");
        }

        // Interactividad de pilares visibles (gris si cadena no desbloqueada aún en esta run)
        if (Btn_Tierra != null) Btn_Tierra.interactable = gc.Cadenas.CadenaPilarDesbloqueada(TipoPilar.Tierra);
        if (Btn_Oceanos != null && Btn_Oceanos.gameObject.activeSelf)
            Btn_Oceanos.interactable = gc.Cadenas.CadenaPilarDesbloqueada(TipoPilar.Oceanos);
        if (Btn_Atmosfera != null && Btn_Atmosfera.gameObject.activeSelf)
            Btn_Atmosfera.interactable = gc.Cadenas.CadenaPilarDesbloqueada(TipoPilar.Atmosfera);
        if (Btn_Vida != null && Btn_Vida.gameObject.activeSelf)
            Btn_Vida.interactable = gc.Cadenas.CadenaPilarDesbloqueada(TipoPilar.Vida);
    }

    void Revelar(Button boton, string nombre)
    {
        if (boton == null || boton.gameObject.activeSelf) return;
        boton.gameObject.SetActive(true);
        EventBus.Publicar(new EventoUIDesbloqueado(nombre));
        MostrarBannerMision("Nuevo: " + nombre + "!");
    }

    void SetVisible(Button boton, bool visible)
    {
        if (boton != null) boton.gameObject.SetActive(visible);
    }

    // ══════════════════════════════════════════════════════════════════════
    // PANEL CATEGORÍA
    // ══════════════════════════════════════════════════════════════════════

    void AbrirCategoria(TipoPilar pilar)
    {
        _categoriaActual = pilar;
        _tabInfraestructura = false;

        if (Text_NombreCategoria != null)
            Text_NombreCategoria.text = _nombresPilar[(int)pilar];

        // Mostrar/ocultar pestaña infraestructura según si la cadena está desbloqueada
        var gc = GameController.Instance;
        bool cadenaActiva = gc != null && gc.Cadenas.CadenaPilarDesbloqueada(pilar);
        if (Btn_TabInfraestructura != null)
            Btn_TabInfraestructura.gameObject.SetActive(cadenaActiva);
        if (Btn_TabProduccion != null)
            Btn_TabProduccion.gameObject.SetActive(cadenaActiva);

        ActualizarTogglesAutomatizacion(pilar);

        AplicarTab();
        MostrarPantalla(Panel_Categoria);
    }

    // ── Auto-compra (T19) ────────────────────────────────────────────────

    private static readonly TipoAutomatizacion[] _automatPorPilar =
    {
        TipoAutomatizacion.Viento,           // Atmosfera
        TipoAutomatizacion.Corrientes,       // Oceanos
        TipoAutomatizacion.Gravedad,         // Tierra
        TipoAutomatizacion.EvolucionNatural  // Vida
    };

    void ActualizarTogglesAutomatizacion(TipoPilar pilar)
    {
        var gc = GameController.Instance;
        if (gc?.Automatizacion == null || Panel_Categoria == null) return;

        // Crear toggles si no existen
        if (_btnAutoCompra == null)
        {
            var go = CrearBotonUI(Panel_Categoria.transform, "Btn_AutoCompra_Dyn",
                "AUTO", 0, 0, 200, 70);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-20, -20);
            _btnAutoCompra = go.GetComponent<Button>();
            _txtAutoCompra = go.GetComponentInChildren<TextMeshProUGUI>();
            _txtAutoCompra.fontSize = 26;
            _txtAutoCompra.fontStyle = FontStyles.Bold;
            _btnAutoCompra.onClick.AddListener(OnClickToggleAutoCompra);
        }
        if (_btnAutoCompraSmart == null)
        {
            var go = CrearBotonUI(Panel_Categoria.transform, "Btn_AutoCompraSmart_Dyn",
                "SMART", 0, 0, 200, 70);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-20, -100);
            _btnAutoCompraSmart = go.GetComponent<Button>();
            _txtAutoCompraSmart = go.GetComponentInChildren<TextMeshProUGUI>();
            _txtAutoCompraSmart.fontSize = 26;
            _txtAutoCompraSmart.fontStyle = FontStyles.Bold;
            _btnAutoCompraSmart.onClick.AddListener(OnClickToggleAutoCompraSmart);
        }

        // Forzar que se dibujen por encima del resto (el panel puede añadir hijos después)
        _btnAutoCompra.transform.SetAsLastSibling();
        _btnAutoCompraSmart.transform.SetAsLastSibling();

        // Toggle del pilar
        var tipoPilarAuto = _automatPorPilar[(int)pilar];
        bool desbloqueadoPilar = gc.Automatizacion.EstaDesbloqueada(tipoPilarAuto);
        _btnAutoCompra.gameObject.SetActive(desbloqueadoPilar);
        if (desbloqueadoPilar)
            PintarToggleAutomat(_btnAutoCompra, _txtAutoCompra,
                gc.Automatizacion.EstaActivada(tipoPilarAuto));

        // Toggle smart (solo si nd_37 está comprado)
        bool desbloqueadoSmart = gc.Automatizacion.EstaDesbloqueada(TipoAutomatizacion.SeleccionNatural);
        _btnAutoCompraSmart.gameObject.SetActive(desbloqueadoSmart);
        if (desbloqueadoSmart)
            PintarToggleAutomat(_btnAutoCompraSmart, _txtAutoCompraSmart,
                gc.Automatizacion.EstaActivada(TipoAutomatizacion.SeleccionNatural));
    }

    void PintarToggleAutomat(Button btn, TextMeshProUGUI txt, bool activa)
    {
        var img = btn.GetComponent<Image>();
        if (img != null)
            img.color = activa
                ? new Color(0.30f, 0.85f, 0.40f, 1f)   // verde brillante = ON
                : new Color(0.80f, 0.25f, 0.25f, 1f);  // rojo = OFF
        if (txt != null)
        {
            string label = btn == _btnAutoCompraSmart ? "SMART" : "AUTO";
            txt.text = activa ? label + ": ON" : label + ": OFF";
            txt.color = Color.white;
        }
    }

    void OnClickToggleAutoCompra()
    {
        var gc = GameController.Instance;
        if (gc?.Automatizacion == null) return;
        var tipo = _automatPorPilar[(int)_categoriaActual];
        gc.AlternarAutomatizacion(tipo);
        PintarToggleAutomat(_btnAutoCompra, _txtAutoCompra,
            gc.Automatizacion.EstaActivada(tipo));
    }

    void OnClickToggleAutoCompraSmart()
    {
        var gc = GameController.Instance;
        if (gc?.Automatizacion == null) return;
        gc.AlternarAutomatizacion(TipoAutomatizacion.SeleccionNatural);
        PintarToggleAutomat(_btnAutoCompraSmart, _txtAutoCompraSmart,
            gc.Automatizacion.EstaActivada(TipoAutomatizacion.SeleccionNatural));
    }

    void CambiarTab(bool infraestructura)
    {
        _tabInfraestructura = infraestructura;
        AplicarTab();
    }

    void AplicarTab()
    {
        // Resaltar pestaña activa
        if (Btn_TabProduccion != null)
            Btn_TabProduccion.interactable = _tabInfraestructura;
        if (Btn_TabInfraestructura != null)
            Btn_TabInfraestructura.interactable = !_tabInfraestructura;

        if (_tabInfraestructura)
            GenerarTarjetasCadena(_categoriaActual);
        else
            GenerarTarjetas(_categoriaActual);
    }

    void GenerarTarjetas(TipoPilar pilar)
    {
        _tarjetasActivas.Clear();

        // Restaurar títulos de zona (pueden haber sido cambiados por la pestaña cadenas)
        if (Text_TituloZona0 != null) Text_TituloZona0.text = _zonasNombres[(int)pilar][0];
        if (Text_TituloZona1 != null) Text_TituloZona1.text = _zonasNombres[(int)pilar][1];
        if (Text_TituloZona2 != null) Text_TituloZona2.text = _zonasNombres[(int)pilar][2];

        // Limpiar contenedores
        Transform[] contenedores = { Contenedor_Zona0, Contenedor_Zona1, Contenedor_Zona2 };
        foreach (var c in contenedores)
        {
            if (c == null) continue;
            foreach (Transform hijo in c) Destroy(hijo.gameObject);
        }

        if (Prefab_TarjetaMejora == null)
        {
            Debug.LogError("[UIManager] Prefab_TarjetaMejora no asignado.");
            return;
        }

        var gc = GameController.Instance;
        if (gc == null) return;

        double prodTotal = 0;
        for (int zona = 0; zona < 3; zona++)
        {
            if (contenedores[zona] == null) continue;
            foreach (var def in gc.Mejoras.ObtenerPorPilarYZona(pilar, zona))
            {
                var tarjeta = Instantiate(Prefab_TarjetaMejora, contenedores[zona]);
                RellenarTarjeta(tarjeta, def);
                _tarjetasActivas.Add((tarjeta, def));

                int nivel = gc.Estado.NivelMejora(def.Id);
                prodTotal += def.ProduccionEnNivel(nivel) * def.MultiplicadorHito(nivel);
            }
        }

        // Mostrar si hay cuello de botella
        if (Text_ProduccionCategoria != null)
        {
            double cap = gc.Cadenas.CalcularCapPilar(pilar);
            bool limitado = cap < prodTotal && cap < double.MaxValue;
            if (limitado)
                Text_ProduccionCategoria.text = Formateador.Numero(cap) + " / "
                    + Formateador.Numero(prodTotal) + " EV/s  <color=#FF6644>[CUELLO DE BOTELLA — mejora Infraestructura]</color>";
            else
                Text_ProduccionCategoria.text = Formateador.Numero(prodTotal) + " EV/s";
        }
    }

    void RellenarTarjeta(GameObject tarjeta, DefinicionMejora def)
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        bool desbloqueada = gc.Estado.MejoraDesbloqueada(def.Id);
        int nivel = gc.Estado.NivelMejora(def.Id);
        double ev = gc.Estado.EnergiaVital;

        // ── Textos por nombre ────────────────────────────────────────────
        var tNombre = tarjeta.transform.Find("Text_Nombre")?.GetComponent<TextMeshProUGUI>();
        var tProd = tarjeta.transform.Find("Text_Produccion")?.GetComponent<TextMeshProUGUI>();
        var tCoste = tarjeta.transform.Find("Text_Coste")?.GetComponent<TextMeshProUGUI>();

        if (tNombre != null)
            tNombre.text = desbloqueada
                ? def.Nombre + "  <size=70%>Nv " + nivel + "</size>"
                : def.Nombre + "  <size=70%>[Era " + def.EraRequerida + "]</size>";

        if (tProd != null)
            tProd.text = nivel > 0
                ? "+" + Formateador.Numero(def.ProduccionEnNivel(nivel) * def.MultiplicadorHito(nivel)) + " EV/s"
                : def.Descripcion;

        if (tCoste != null)
            tCoste.text = desbloqueada
                ? Formateador.Numero(def.CosteEnNivel(nivel)) + " EV"
                : "Bloqueada";

        // ── Slider hito ───────────────────────────────────────────────────
        var slider = tarjeta.transform.Find("Slider_Hito")?.GetComponent<Slider>();
        if (slider != null && desbloqueada)
        {
            int[] hitos = { 10, 25, 50, 100, 200, 500 };
            int ant = 0, sig = def.NivelMax;
            foreach (var h in hitos) { if (nivel < h) { sig = h; break; } ant = h; }
            slider.value = sig > ant ? (float)(nivel - ant) / (sig - ant) : 1f;
        }

        // ── Botones por nombre ────────────────────────────────────────────
        ConfigurarBtn(tarjeta, "Btn_Comprar", def, nivel, 1, ev, desbloqueada);
        ConfigurarBtn(tarjeta, "Btn_Comprar10", def, nivel, 10, ev, desbloqueada);
        ConfigurarBtn(tarjeta, "Btn_Comprar100", def, nivel, 100, ev, desbloqueada);
        ConfigurarBtnMax(tarjeta, "Btn_ComprarMax", def, nivel, ev, desbloqueada);

        tarjeta.SetActive(desbloqueada || def.EraRequerida <= gc.Estado.EraActual + 1);
    }

    void ConfigurarBtn(GameObject tarjeta, string nombre, DefinicionMejora def,
                       int nivel, int cantidad, double ev, bool desbloqueada)
    {
        var t = tarjeta.transform.Find(nombre);
        if (t == null) return;
        var btn = t.GetComponent<Button>();
        if (btn == null) return;

        // Coste total de N niveles
        double total = 0;
        int nTemp = nivel;
        int comprablesReales = 0;
        for (int k = 0; k < cantidad && nTemp < def.NivelMax; k++, nTemp++)
        {
            total += def.CosteEnNivel(nTemp);
            comprablesReales++;
        }

        bool puede = desbloqueada && comprablesReales > 0 && ev >= total;
        btn.interactable = puede;
        btn.onClick.RemoveAllListeners();

        var defC = def; var tarC = tarjeta; int cantC = cantidad;
        btn.onClick.AddListener(() =>
        {
            if (cantC == 1) GameController.Instance?.ComprarMejora(defC.Id);
            else GameController.Instance?.ComprarMejoraN(defC.Id, cantC);
            RellenarTarjeta(tarC, defC);
        });

        // Texto del boton
        var txt = t.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            string label = cantidad == 1 ? "x1" : "x" + cantidad;
            txt.text = puede
                ? label + "\n" + Formateador.Numero(total)
                : label;
        }
    }

    void ConfigurarBtnMax(GameObject tarjeta, string nombre, DefinicionMejora def,
                          int nivel, double ev, bool desbloqueada)
    {
        var t = tarjeta.transform.Find(nombre);
        if (t == null) return;
        var btn = t.GetComponent<Button>();
        if (btn == null) return;

        double coste1 = def.CosteEnNivel(nivel);
        bool puede = desbloqueada && nivel < def.NivelMax && ev >= coste1;
        btn.interactable = puede;
        btn.onClick.RemoveAllListeners();

        var defC = def; var tarC = tarjeta;
        btn.onClick.AddListener(() =>
        {
            GameController.Instance?.ComprarMejoraMax(defC.Id);
            RellenarTarjeta(tarC, defC);
        });

        var txt = t.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null) txt.text = "MAX";
    }

    void ActualizarTarjetas()
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        double prodTotal = 0;
        foreach (var (tarjeta, def) in _tarjetasActivas)
        {
            if (tarjeta == null) continue;
            RellenarTarjeta(tarjeta, def);
            int nivel = gc.Estado.NivelMejora(def.Id);
            prodTotal += def.ProduccionEnNivel(nivel) * def.MultiplicadorHito(nivel);
        }

        if (Text_ProduccionCategoria != null)
        {
            double cap = gc.Cadenas.CalcularCapPilar(_categoriaActual);
            bool limitado = cap < prodTotal && cap < double.MaxValue;
            if (limitado)
                Text_ProduccionCategoria.text = Formateador.Numero(cap) + " / "
                    + Formateador.Numero(prodTotal) + " EV/s  <color=#FF6644>[CUELLO DE BOTELLA]</color>";
            else
                Text_ProduccionCategoria.text = Formateador.Numero(prodTotal) + " EV/s";
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // CADENAS (INFRAESTRUCTURA)
    // ══════════════════════════════════════════════════════════════════════

    void GenerarTarjetasCadena(TipoPilar pilar)
    {
        _tarjetasCadenaActivas.Clear();

        Transform[] contenedores = { Contenedor_Zona0, Contenedor_Zona1, Contenedor_Zona2 };
        foreach (var c in contenedores)
        {
            if (c == null) continue;
            foreach (Transform hijo in c) Destroy(hijo.gameObject);
        }

        if (Prefab_TarjetaMejora == null) return;
        var gc = GameController.Instance;
        if (gc == null) return;

        var eslabones = new[] { TipoEslabon.Generacion, TipoEslabon.Procesamiento, TipoEslabon.Distribucion };
        TipoEslabon cuelloBotella = gc.Cadenas.IdentificarCuelloBotella(pilar);

        for (int e = 0; e < 3; e++)
        {
            if (contenedores[e] == null) continue;

            double capEslabon = gc.Cadenas.CalcularCapEslabon(pilar, eslabones[e]);
            bool esCuello = eslabones[e] == cuelloBotella;

            var tituloZona = e switch { 0 => Text_TituloZona0, 1 => Text_TituloZona1, _ => Text_TituloZona2 };
            if (tituloZona != null)
            {
                string color = esCuello ? "<color=#FF6644>" : "<color=#88FF88>";
                string marca = esCuello ? " ◄ LIMITA" : "";
                tituloZona.text = _eslabonNombres[e] + "  " + color
                    + Formateador.Numero(capEslabon) + "/s" + marca + "</color>";
            }

            foreach (var def in gc.Cadenas.ObtenerSubMejorasPorEslabon(pilar, eslabones[e]))
            {
                var tarjeta = Instantiate(Prefab_TarjetaMejora, contenedores[e]);
                RellenarTarjetaCadena(tarjeta, def);
                _tarjetasCadenaActivas.Add((tarjeta, def));
            }
        }

        ActualizarTextoCadenaPilar(pilar);
    }

    void RellenarTarjetaCadena(GameObject tarjeta, DefinicionSubMejoraCadena def)
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        var est = gc.Estado.Cadenas.TryGetValue(def.Id, out var e) ? e : null;
        bool desbloqueada = est != null && est.Desbloqueada;
        int nivel = est?.Nivel ?? 0;
        double ev = gc.Estado.EnergiaVital;

        var tNombre = tarjeta.transform.Find("Text_Nombre")?.GetComponent<TextMeshProUGUI>();
        var tProd = tarjeta.transform.Find("Text_Produccion")?.GetComponent<TextMeshProUGUI>();
        var tCoste = tarjeta.transform.Find("Text_Coste")?.GetComponent<TextMeshProUGUI>();

        if (tNombre != null)
            tNombre.text = desbloqueada
                ? def.Nombre + "  <size=70%>Nv " + nivel + "/" + def.NivelMax + "</size>"
                : def.Nombre + "  <size=70%>[Eslabón Nv " + def.NivelEslabonRequerido + "]</size>";

        if (tProd != null)
            tProd.text = desbloqueada && nivel > 0
                ? "Cap +" + Formateador.Numero(def.CapPorNivel * nivel) + "/s"
                : def.Descripcion;

        if (tCoste != null)
            tCoste.text = desbloqueada
                ? Formateador.Numero(def.CosteEnNivel(nivel)) + " EV"
                : "Bloqueada";

        // Ocultar slider de hito (no aplica a cadenas)
        var slider = tarjeta.transform.Find("Slider_Hito")?.GetComponent<Slider>();
        if (slider != null) slider.gameObject.SetActive(false);

        // Botones de compra
        ConfigurarBtnCadena(tarjeta, "Btn_Comprar", def, nivel, 1, ev, desbloqueada);
        ConfigurarBtnCadena(tarjeta, "Btn_Comprar10", def, nivel, 10, ev, desbloqueada);
        ConfigurarBtnCadena(tarjeta, "Btn_Comprar100", def, nivel, 100, ev, desbloqueada);
        ConfigurarBtnMaxCadena(tarjeta, "Btn_ComprarMax", def, nivel, ev, desbloqueada);

        tarjeta.SetActive(desbloqueada || def.EraRequerida <= gc.Estado.EraActual);
    }

    void ConfigurarBtnCadena(GameObject tarjeta, string nombre, DefinicionSubMejoraCadena def,
                              int nivel, int cantidad, double ev, bool desbloqueada)
    {
        var t = tarjeta.transform.Find(nombre);
        if (t == null) return;
        var btn = t.GetComponent<Button>();
        if (btn == null) return;

        double total = 0;
        int nTemp = nivel;
        int comprables = 0;
        for (int k = 0; k < cantidad && nTemp < def.NivelMax; k++, nTemp++)
        {
            total += def.CosteEnNivel(nTemp);
            comprables++;
        }

        bool puede = desbloqueada && comprables > 0 && ev >= total;
        btn.interactable = puede;
        btn.onClick.RemoveAllListeners();

        var defC = def; var tarC = tarjeta; int cantC = cantidad;
        btn.onClick.AddListener(() =>
        {
            for (int i = 0; i < cantC; i++)
                if (!GameController.Instance.ComprarSubMejoraCadena(defC.Id)) break;
            RellenarTarjetaCadena(tarC, defC);
        });

        var txt = t.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            string label = cantidad == 1 ? "x1" : "x" + cantidad;
            txt.text = puede ? label + "\n" + Formateador.Numero(total) : label;
        }
    }

    void ConfigurarBtnMaxCadena(GameObject tarjeta, string nombre, DefinicionSubMejoraCadena def,
                                 int nivel, double ev, bool desbloqueada)
    {
        var t = tarjeta.transform.Find(nombre);
        if (t == null) return;
        var btn = t.GetComponent<Button>();
        if (btn == null) return;

        bool puede = desbloqueada && nivel < def.NivelMax && ev >= def.CosteEnNivel(nivel);
        btn.interactable = puede;
        btn.onClick.RemoveAllListeners();

        var defC = def; var tarC = tarjeta;
        btn.onClick.AddListener(() =>
        {
            GameController.Instance?.ComprarSubMejoraCadenaMax(defC.Id);
            RellenarTarjetaCadena(tarC, defC);
        });

        var txt = t.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null) txt.text = "MAX";
    }

    void ActualizarTarjetasCadena()
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        foreach (var (tarjeta, def) in _tarjetasCadenaActivas)
        {
            if (tarjeta == null) continue;
            RellenarTarjetaCadena(tarjeta, def);
        }

        // Refrescar títulos de eslabones con indicador de cuello de botella
        var eslabones = new[] { TipoEslabon.Generacion, TipoEslabon.Procesamiento, TipoEslabon.Distribucion };
        TipoEslabon cuello = gc.Cadenas.IdentificarCuelloBotella(_categoriaActual);
        for (int e = 0; e < 3; e++)
        {
            double capEslabon = gc.Cadenas.CalcularCapEslabon(_categoriaActual, eslabones[e]);
            bool esCuello = eslabones[e] == cuello;
            var tituloZona = e switch { 0 => Text_TituloZona0, 1 => Text_TituloZona1, _ => Text_TituloZona2 };
            if (tituloZona != null)
            {
                string color = esCuello ? "<color=#FF6644>" : "<color=#88FF88>";
                string marca = esCuello ? " ◄ LIMITA" : "";
                tituloZona.text = _eslabonNombres[e] + "  " + color
                    + Formateador.Numero(capEslabon) + "/s" + marca + "</color>";
            }
        }

        ActualizarTextoCadenaPilar(_categoriaActual);
    }

    void ActualizarTextoCadenaPilar(TipoPilar pilar)
    {
        var gc = GameController.Instance;
        if (gc == null || Text_ProduccionCategoria == null) return;

        double capPilar = gc.Cadenas.CalcularCapPilar(pilar);
        double potencial = 0;
        foreach (var def in gc.Mejoras.ObtenerPorPilar(pilar))
        {
            int nivel = gc.Estado.NivelMejora(def.Id);
            potencial += def.ProduccionEnNivel(nivel) * def.MultiplicadorHito(nivel);
        }

        bool limitado = capPilar < potencial && capPilar < double.MaxValue;
        TipoEslabon cuello = gc.Cadenas.IdentificarCuelloBotella(pilar);

        Text_ProduccionCategoria.text = limitado
            ? "Produce " + Formateador.Numero(potencial) + "/s → Cap "
                + Formateador.Numero(capPilar) + "/s  <color=#FF6644>[Mejora "
                + _eslabonNombres[(int)cuello] + "]</color>"
            : Formateador.Numero(potencial) + " EV/s  <color=#88FF88>[Sin límite]</color>";
    }

    void ActualizarResumenCadenas()
    {
        if (Text_ResumenCadenas == null) return;
        var gc = GameController.Instance;
        if (gc == null) { Text_ResumenCadenas.text = ""; return; }

        var partes = new System.Text.StringBuilder();
        for (int p = 0; p < 4; p++)
        {
            var pilar = (TipoPilar)p;
            if (!gc.Cadenas.CadenaPilarDesbloqueada(pilar)) continue;

            double cap = gc.Cadenas.CalcularCapPilar(pilar);
            if (cap >= double.MaxValue) continue; // no hay niveles comprados

            double potencial = 0;
            foreach (var def in gc.Mejoras.ObtenerPorPilar(pilar))
            {
                int nivel = gc.Estado.NivelMejora(def.Id);
                potencial += def.ProduccionEnNivel(nivel) * def.MultiplicadorHito(nivel);
            }

            bool limitado = cap < potencial;
            if (partes.Length > 0) partes.Append("  |  ");
            partes.Append(_nombresPilar[p].Substring(0, 3));
            partes.Append(": " + Formateador.Numero(System.Math.Min(cap, potencial)));
            partes.Append("/" + Formateador.Numero(potencial));
            partes.Append(limitado ? " [!]" : " [OK]");
        }

        Text_ResumenCadenas.text = partes.ToString();
    }

    // ══════════════════════════════════════════════════════════════════════
    // WIDGET MISIONES (HUD)
    // ══════════════════════════════════════════════════════════════════════

    // ── Inicializar panel de misiones (generación dinámica) ──────────────

    void InicializarPanelMisiones()
    {
        if (Panel_Misiones == null) return;

        var panelRT = Panel_Misiones.GetComponent<RectTransform>();

        // 1. Destruir el VerticalLayoutGroup de la escena (sobreescribe posiciones)
        var vlgViejo = Panel_Misiones.GetComponent<VerticalLayoutGroup>();
        if (vlgViejo != null) DestroyImmediate(vlgViejo);

        // 2. Forzar panel a pantalla completa
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // 3. Destruir TODOS los hijos viejos de la escena
        for (int i = panelRT.childCount - 1; i >= 0; i--)
            DestroyImmediate(panelRT.GetChild(i).gameObject);

        // 4. Boton cerrar (arriba-derecha)
        var goCerrar = CrearBotonUI(panelRT, "Btn_CerrarMisiones_Dyn", "X", 0, 0, 60, 50);
        var rtCerrar = goCerrar.GetComponent<RectTransform>();
        rtCerrar.anchorMin = new Vector2(1, 1);
        rtCerrar.anchorMax = new Vector2(1, 1);
        rtCerrar.anchoredPosition = new Vector2(-40, -30);
        goCerrar.GetComponent<Button>().onClick.AddListener(() => MostrarPantalla(Panel_Principal));

        // 5. Pestañas (arriba-centro)
        var goPorHacer = CrearBotonUI(panelRT, "Btn_TabPorHacer", "POR HACER", 0, 0, 300, 50);
        var rtPH = goPorHacer.GetComponent<RectTransform>();
        rtPH.anchorMin = new Vector2(0.5f, 1);
        rtPH.anchorMax = new Vector2(0.5f, 1);
        rtPH.anchoredPosition = new Vector2(-160, -30);
        _btnTabPorHacer = goPorHacer.GetComponent<Button>();
        _txtTabPorHacer = goPorHacer.GetComponentInChildren<TextMeshProUGUI>();

        var goCompletadas = CrearBotonUI(panelRT, "Btn_TabCompletadas", "COMPLETADAS", 0, 0, 300, 50);
        var rtComp2 = goCompletadas.GetComponent<RectTransform>();
        rtComp2.anchorMin = new Vector2(0.5f, 1);
        rtComp2.anchorMax = new Vector2(0.5f, 1);
        rtComp2.anchoredPosition = new Vector2(160, -30);
        _btnTabCompletadas = goCompletadas.GetComponent<Button>();
        _txtTabCompletadas = goCompletadas.GetComponentInChildren<TextMeshProUGUI>();

        _btnTabPorHacer.onClick.AddListener(() => CambiarTabMisiones(false));
        _btnTabCompletadas.onClick.AddListener(() => CambiarTabMisiones(true));

        // 6. Contenedor activas (stretch, debajo de tabs)
        var goActivas = new GameObject("Contenedor_MisionesActivas", typeof(RectTransform), typeof(VerticalLayoutGroup));
        goActivas.transform.SetParent(panelRT, false);
        var rtActivas = goActivas.GetComponent<RectTransform>();
        rtActivas.anchorMin = Vector2.zero;
        rtActivas.anchorMax = Vector2.one;
        rtActivas.offsetMin = new Vector2(15, 15);
        rtActivas.offsetMax = new Vector2(-15, -65);
        var vlgActivas = goActivas.GetComponent<VerticalLayoutGroup>();
        vlgActivas.spacing = 12;
        vlgActivas.padding = new RectOffset(0, 0, 5, 5);
        vlgActivas.childForceExpandWidth = true;
        vlgActivas.childForceExpandHeight = false;
        vlgActivas.childControlWidth = true;
        vlgActivas.childControlHeight = false;
        _contenedorMisionesActivas = goActivas.transform;

        // 7. Contenedor completadas (mismo layout)
        var goComp = new GameObject("Contenedor_MisionesCompletadas", typeof(RectTransform), typeof(VerticalLayoutGroup));
        goComp.transform.SetParent(panelRT, false);
        var rtComp = goComp.GetComponent<RectTransform>();
        rtComp.anchorMin = Vector2.zero;
        rtComp.anchorMax = Vector2.one;
        rtComp.offsetMin = new Vector2(15, 15);
        rtComp.offsetMax = new Vector2(-15, -65);
        var vlgComp = goComp.GetComponent<VerticalLayoutGroup>();
        vlgComp.spacing = 10;
        vlgComp.padding = new RectOffset(0, 0, 5, 5);
        vlgComp.childForceExpandWidth = true;
        vlgComp.childForceExpandHeight = false;
        vlgComp.childControlWidth = true;
        vlgComp.childControlHeight = false;
        _contenedorMisionesCompletadas = goComp.transform;

        _misionesTabCompletadas = false;
        CambiarTabMisiones(false);
    }

    void CambiarTabMisiones(bool completadas)
    {
        _misionesTabCompletadas = completadas;
        if (_contenedorMisionesActivas != null)
            _contenedorMisionesActivas.gameObject.SetActive(!completadas);
        if (_contenedorMisionesCompletadas != null)
            _contenedorMisionesCompletadas.gameObject.SetActive(completadas);

        // Colores de pestañas
        if (_txtTabPorHacer != null)
            _txtTabPorHacer.color = completadas ? Color.gray : Color.white;
        if (_txtTabCompletadas != null)
            _txtTabCompletadas.color = completadas ? Color.white : Color.gray;
    }

    // ── Actualizar UI misiones (llamado en Update) ────────────────────

    void ActualizarWidgetMisiones()
    {
        if (Panel_Misiones == null || !Panel_Misiones.activeSelf) return;
        var gc = GameController.Instance;
        if (gc == null) return;

        if (_misionesTabCompletadas)
            RegenerarMisionesCompletadas(gc);
        else
            RegenerarMisionesActivas(gc);

        ActualizarBadgeMisiones(gc);
    }

    void RegenerarMisionesActivas(GameController gc)
    {
        if (_contenedorMisionesActivas == null) return;

        // Limpiar
        for (int i = _contenedorMisionesActivas.childCount - 1; i >= 0; i--)
            Destroy(_contenedorMisionesActivas.GetChild(i).gameObject);

        bool hayAlguna = false;
        for (int i = 0; i < 3; i++)
        {
            var def = gc.Misiones.ObtenerDefinicionSlot(i);
            var est = gc.Estado.MisionesActivas[i];
            if (def == null || est == null || string.IsNullOrEmpty(est.Id)) continue;

            hayAlguna = true;
            CrearFilaMisionActiva(def, est);
        }

        if (!hayAlguna)
        {
            var goVacio = CrearTextoUI(_contenedorMisionesActivas, "Text_SinMisiones",
                "No hay misiones disponibles por ahora.", 20);
            var rt = goVacio.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, 40);
        }
    }

    void CrearFilaMisionActiva(DefinicionMision def, Terra.State.EstadoMision est)
    {
        // Contenedor de la fila
        var fila = new GameObject("Fila_" + def.Id, typeof(RectTransform), typeof(VerticalLayoutGroup),
            typeof(CanvasRenderer), typeof(Image));
        fila.transform.SetParent(_contenedorMisionesActivas, false);
        var rtFila = fila.GetComponent<RectTransform>();
        rtFila.sizeDelta = new Vector2(0, 155);
        var vlg = fila.GetComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(20, 20, 10, 10);
        vlg.spacing = 6;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        var img = fila.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

        // Nombre
        var goNombre = CrearTextoUI(fila.transform, "Nombre", "<b>" + def.Nombre + "</b>", 24);
        goNombre.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 30);

        // Descripción (más alta para que quepa el texto completo)
        var goDesc = CrearTextoUI(fila.transform, "Desc", def.Descripcion, 19);
        goDesc.GetComponent<TextMeshProUGUI>().color = new Color(0.85f, 0.85f, 0.85f);
        goDesc.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);

        // Barra de progreso
        float progreso = def.ValorObjetivo > 0
            ? (float)System.Math.Min(est.ProgresoActual / def.ValorObjetivo, 1.0)
            : 0f;

        var goBarraContainer = new GameObject("BarraContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        goBarraContainer.transform.SetParent(fila.transform, false);
        goBarraContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 28);
        var hlg = goBarraContainer.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 15;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;

        // Slider
        var goSlider = CrearSliderUI(goBarraContainer.transform, "Slider", progreso);
        var slLE = goSlider.AddComponent<LayoutElement>();
        slLE.flexibleWidth = 1;

        // Texto progreso
        string txtProg = Formateador.Numero(est.ProgresoActual) + " / " + Formateador.Numero(def.ValorObjetivo);
        var goProgreso = CrearTextoUI(goBarraContainer.transform, "Progreso", txtProg, 18);
        var progLE = goProgreso.AddComponent<LayoutElement>();
        progLE.minWidth = 140;
        progLE.preferredWidth = 140;

        // Recompensa
        string recompensaTxt = "<color=#FFD700>Recompensa: +" + Formateador.Numero(def.ValorRecompensa) + "s de EV</color>";
        var goRec = CrearTextoUI(fila.transform, "Recompensa", recompensaTxt, 17);
        goRec.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 24);
    }

    void RegenerarMisionesCompletadas(GameController gc)
    {
        if (_contenedorMisionesCompletadas == null) return;

        // Limpiar
        for (int i = _contenedorMisionesCompletadas.childCount - 1; i >= 0; i--)
            Destroy(_contenedorMisionesCompletadas.GetChild(i).gameObject);

        var completadas = gc.Estado.MisionesCompletadas;
        if (completadas.Count == 0)
        {
            var goVacio = CrearTextoUI(_contenedorMisionesCompletadas, "Text_SinCompletadas",
                "Aun no has completado ninguna mision.", 20);
            goVacio.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);
            return;
        }

        // Mostrar de más reciente a más antigua
        for (int i = completadas.Count - 1; i >= 0; i--)
        {
            var mc = completadas[i];
            var def = gc.Misiones.BuscarDefinicion(mc.Id);
            if (def == null) continue;

            CrearFilaMisionCompletada(def, mc, gc);
        }
    }

    void CrearFilaMisionCompletada(DefinicionMision def, Terra.State.MisionCompletada mc, GameController gc)
    {
        var fila = new GameObject("FilaComp_" + def.Id, typeof(RectTransform), typeof(HorizontalLayoutGroup),
            typeof(CanvasRenderer), typeof(Image));
        fila.transform.SetParent(_contenedorMisionesCompletadas, false);
        var rtFila = fila.GetComponent<RectTransform>();
        rtFila.sizeDelta = new Vector2(0, 80);
        var hlg = fila.GetComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(20, 20, 10, 10);
        hlg.spacing = 15;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        var img = fila.GetComponent<Image>();
        img.color = mc.RecompensaRecogida
            ? new Color(0.1f, 0.15f, 0.1f, 0.8f)
            : new Color(0.15f, 0.2f, 0.1f, 0.9f);

        // Check verde + nombre
        string checkColor = mc.RecompensaRecogida ? "#66AA66" : "#88FF88";
        string nombre = "<color=" + checkColor + ">V</color>  " + def.Nombre;
        var goNombre = CrearTextoUI(fila.transform, "Nombre", nombre, 21);
        var nomLE = goNombre.AddComponent<LayoutElement>();
        nomLE.flexibleWidth = 1;

        if (mc.RecompensaRecogida)
        {
            // Ya recogida
            var goRecogida = CrearTextoUI(fila.transform, "Estado", "<color=#66AA66>Recogida</color>", 18);
            var recLE = goRecogida.AddComponent<LayoutElement>();
            recLE.minWidth = 160;
            recLE.preferredWidth = 160;
        }
        else
        {
            // Botón recoger
            string textoBoton = "Recoger\n<size=80%>+" + Formateador.Numero(def.ValorRecompensa) + "s EV</size>";
            var goBtn = CrearBotonUI(fila.transform, "Btn_Recoger_" + def.Id, textoBoton, 0, 0, 180, 55);
            var btnLE = goBtn.AddComponent<LayoutElement>();
            btnLE.minWidth = 180;
            btnLE.preferredWidth = 180;
            var btn = goBtn.GetComponent<Button>();
            var colores = btn.colors;
            colores.normalColor = new Color(0.2f, 0.6f, 0.2f);
            colores.highlightedColor = new Color(0.3f, 0.7f, 0.3f);
            colores.pressedColor = new Color(0.1f, 0.5f, 0.1f);
            btn.colors = colores;

            string idCaptura = def.Id;
            btn.onClick.AddListener(() =>
            {
                var gcInner = GameController.Instance;
                if (gcInner != null && gcInner.Misiones.RecogerRecompensa(idCaptura))
                {
                    MostrarBannerMision("RECOMPENSA RECOGIDA\n+" + Formateador.Numero(def.ValorRecompensa) + "s de EV");
                }
            });
        }
    }

    // ── Badge de misiones pendientes en HUD ───────────────────────────

    void ActualizarBadgeMisiones(GameController gc)
    {
        if (Btn_AbrirMisiones == null) return;

        bool hayPendiente = gc.Misiones.HayRecompensaPendiente();

        // Crear badge si no existe
        if (_txtBadgeMisiones == null)
        {
            var goBadge = CrearTextoUI(Btn_AbrirMisiones.transform, "Badge_Misiones", "", 18);
            _txtBadgeMisiones = goBadge.GetComponent<TextMeshProUGUI>();
            _txtBadgeMisiones.alignment = TextAlignmentOptions.TopRight;
            var rtBadge = goBadge.GetComponent<RectTransform>();
            rtBadge.anchorMin = new Vector2(1, 1);
            rtBadge.anchorMax = new Vector2(1, 1);
            rtBadge.anchoredPosition = new Vector2(-5, -5);
            rtBadge.sizeDelta = new Vector2(30, 30);
        }

        _txtBadgeMisiones.text = hayPendiente ? "<color=#FF4444>!</color>" : "";
    }

    // ── Banner ────────────────────────────────────────────────────────

    void ActualizarBannerMision()
    {
        if (Panel_BannerMision == null || !Panel_BannerMision.activeSelf) return;

        _timerBannerMision -= INTERVALO_UI;
        if (_timerBannerMision <= 0)
            Panel_BannerMision.SetActive(false);
    }

    void MostrarBannerMision(string texto)
    {
        if (Panel_BannerMision == null) return;
        if (Text_BannerMision != null) Text_BannerMision.text = texto;
        Panel_BannerMision.SetActive(true);
        _timerBannerMision = DURACION_BANNER_MISION;
    }

    // ── Helpers para crear UI dinámica ─────────────────────────────────

    GameObject CrearTextoUI(Transform parent, string nombre, string texto, int fontSize)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.richText = true;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        return go;
    }

    GameObject CrearBotonUI(Transform parent, string nombre, string texto, float x, float y, float w, float h)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(w, h);
        var img = go.GetComponent<Image>();
        img.color = new Color(0.25f, 0.25f, 0.3f);

        // Texto del botón
        var goTxt = CrearTextoUI(go.transform, "Text", texto, 16);
        var rtTxt = goTxt.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero;
        rtTxt.anchorMax = Vector2.one;
        rtTxt.offsetMin = Vector2.zero;
        rtTxt.offsetMax = Vector2.zero;
        goTxt.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        go.GetComponent<Button>().targetGraphic = img;
        return go;
    }

    GameObject CrearSliderUI(Transform parent, string nombre, float valor)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(Slider));
        go.transform.SetParent(parent, false);
        var slider = go.GetComponent<Slider>();

        // Background
        var goBg = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        goBg.transform.SetParent(go.transform, false);
        var rtBg = goBg.GetComponent<RectTransform>();
        rtBg.anchorMin = Vector2.zero;
        rtBg.anchorMax = Vector2.one;
        rtBg.offsetMin = Vector2.zero;
        rtBg.offsetMax = Vector2.zero;
        goBg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f);

        // Fill area
        var goFillArea = new GameObject("Fill Area", typeof(RectTransform));
        goFillArea.transform.SetParent(go.transform, false);
        var rtFillArea = goFillArea.GetComponent<RectTransform>();
        rtFillArea.anchorMin = Vector2.zero;
        rtFillArea.anchorMax = Vector2.one;
        rtFillArea.offsetMin = Vector2.zero;
        rtFillArea.offsetMax = Vector2.zero;

        var goFill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        goFill.transform.SetParent(goFillArea.transform, false);
        var rtFill = goFill.GetComponent<RectTransform>();
        rtFill.anchorMin = Vector2.zero;
        rtFill.anchorMax = new Vector2(0, 1);
        rtFill.offsetMin = Vector2.zero;
        rtFill.offsetMax = Vector2.zero;
        goFill.GetComponent<Image>().color = new Color(0.3f, 0.7f, 0.3f);

        slider.fillRect = rtFill;
        slider.interactable = false;
        slider.value = valor;

        return go;
    }

    Transform CrearContenedor(Transform parent, string nombre, float x, float y, float w, float h)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(w, h);
        return go.transform;
    }

    // ══════════════════════════════════════════════════════════════════════
    // PANEL EVOLUCIÓN (ÁRBOL)
    // ══════════════════════════════════════════════════════════════════════

    static readonly Color _colorBloqueado = new Color(0.4f, 0.4f, 0.4f);
    static readonly Color _colorComprable = new Color(1f, 0.85f, 0.2f);
    static readonly Color _colorComprado  = new Color(0.3f, 0.9f, 0.3f);

    void AbrirEvolucion()
    {
        GenerarNodosArbol();
        MostrarPantalla(Panel_Evolucion);
    }

    void GenerarNodosArbol()
    {
        _nodosActivos.Clear();
        if (Contenedor_Nodos == null || Prefab_NodoArbol == null) return;

        foreach (Transform hijo in Contenedor_Nodos)
            Destroy(hijo.gameObject);

        var gc = GameController.Instance;
        if (gc == null) return;

        for (int era = 1; era <= 8; era++)
        {
            var nodosDeLaEra = gc.Arbol.ObtenerPorEra(era);
            foreach (var def in nodosDeLaEra)
            {
                var nodoGO = Instantiate(Prefab_NodoArbol, Contenedor_Nodos);
                RellenarNodoArbol(nodoGO, def);
                _nodosActivos.Add((nodoGO, def));
            }
        }
    }

    void RellenarNodoArbol(GameObject nodoGO, DefinicionNodo def)
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        var est = gc.Estado.Nodos.TryGetValue(def.Id, out var n) ? n : null;
        bool desbloqueado = est != null && est.Desbloqueado;
        int nivel = est?.Nivel ?? 0;
        bool comprado = nivel > 0;
        bool nivelMax = nivel >= def.NivelMax;

        // Color de fondo
        Color color;
        if (comprado) color = nivelMax ? new Color(0.2f, 0.7f, 0.2f) : _colorComprado;
        else if (desbloqueado) color = _colorComprable;
        else color = _colorBloqueado;

        var img = nodoGO.GetComponent<Image>();
        if (img != null) img.color = color;

        // Texto
        var txt = nodoGO.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            if (!desbloqueado)
                txt.text = "?\n<size=60%>Era " + def.EraRequerida + "</size>";
            else if (nivelMax)
                txt.text = def.Nombre + "\n<size=60%>MAX</size>";
            else
                txt.text = def.Nombre + "\n<size=60%>Nv " + nivel + "/" + def.NivelMax + "</size>";
        }

        // Visibilidad: mostrar si la era está desbloqueada o es la siguiente
        nodoGO.SetActive(def.EraRequerida <= gc.Estado.EraActual + 1);

        // Click → abrir popup
        var btn = nodoGO.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            var defC = def;
            btn.onClick.AddListener(() => MostrarPopupNodo(defC));
            btn.interactable = desbloqueado;
        }
    }

    void MostrarPopupNodo(DefinicionNodo def)
    {
        var gc = GameController.Instance;
        if (gc == null || Panel_PopupNodo == null) return;

        _nodoSeleccionadoId = def.Id;
        var est = gc.Estado.Nodos.TryGetValue(def.Id, out var n) ? n : null;
        int nivel = est?.Nivel ?? 0;
        bool nivelMax = nivel >= def.NivelMax;

        if (Text_PopupNombre != null)
            Text_PopupNombre.text = def.Nombre + (nivelMax ? "  [MAX]" : "  Nv " + nivel + "/" + def.NivelMax);

        if (Text_PopupDescripcion != null)
            Text_PopupDescripcion.text = def.Descripcion;

        if (Text_PopupEfecto != null)
        {
            double multActual = def.MultiplicadorTotal(nivel);
            double multSiguiente = nivelMax ? multActual : def.MultiplicadorTotal(nivel + 1);
            Text_PopupEfecto.text = nivelMax
                ? "Efecto: x" + multActual.ToString("F2")
                : "Efecto: x" + multActual.ToString("F2") + " → x" + multSiguiente.ToString("F2");
        }

        if (Text_PopupCoste != null)
            Text_PopupCoste.text = nivelMax
                ? "Completado"
                : "Coste: " + Formateador.Numero(def.CosteEnNivel(nivel)) + " EV";

        if (Btn_ComprarNodo != null)
        {
            bool puede = !nivelMax && gc.Estado.EnergiaVital >= def.CosteEnNivel(nivel);
            Btn_ComprarNodo.interactable = puede;
            var txtBtn = Btn_ComprarNodo.GetComponentInChildren<TextMeshProUGUI>();
            if (txtBtn != null) txtBtn.text = nivelMax ? "MAX" : "COMPRAR";
        }

        Panel_PopupNodo.SetActive(true);
    }

    void OnClickComprarNodo()
    {
        if (string.IsNullOrEmpty(_nodoSeleccionadoId)) return;
        var gc = GameController.Instance;
        if (gc == null) return;

        gc.ComprarNodoArbol(_nodoSeleccionadoId);

        var def = gc.Arbol.BuscarDefinicion(_nodoSeleccionadoId);
        if (def != null) MostrarPopupNodo(def);
        ActualizarNodosArbol();
    }

    void ActualizarNodosArbol()
    {
        foreach (var (nodoGO, def) in _nodosActivos)
        {
            if (nodoGO == null) continue;
            RellenarNodoArbol(nodoGO, def);
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // PANEL CÓDICE FÓSIL
    // ══════════════════════════════════════════════════════════════════════

    static readonly string[] _ramaNombres = { "ABUNDANCIA", "EFICIENCIA", "DOMINIO" };
    static readonly Color _colorCodiceAbundancia = new Color(0.3f, 0.8f, 0.3f);
    static readonly Color _colorCodiceEficiencia = new Color(0.3f, 0.6f, 0.9f);
    static readonly Color _colorCodiceDominio    = new Color(0.9f, 0.5f, 0.2f);

    /// <summary>
    /// Construye toda la UI del Códice Fósil programáticamente.
    /// Si Panel_Codice ya está asignado en Inspector, no hace nada (modo manual).
    /// Pensado para testear sin tener que cablear todo en Unity.
    /// </summary>
    void ConstruirUICodiceProgramatico()
    {
        if (Panel_Codice != null) return;

        // Buscar canvas raíz a partir de un panel ya wireado en Inspector
        Transform canvasRoot = null;
        if (Panel_Prestige != null) canvasRoot = Panel_Prestige.transform.parent;
        else if (Panel_Principal != null) canvasRoot = Panel_Principal.transform.parent;
        if (canvasRoot == null)
        {
            var canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[UIManager] No se encontro Canvas para construir UI Codice");
                return;
            }
            canvasRoot = canvas.transform;
        }

        // ───── PANEL CODICE (pantalla completa) ─────
        Panel_Codice = new GameObject("Panel_Codice_Dyn",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Panel_Codice.transform.SetParent(canvasRoot, false);
        var rtCodice = Panel_Codice.GetComponent<RectTransform>();
        rtCodice.anchorMin = Vector2.zero;
        rtCodice.anchorMax = Vector2.one;
        rtCodice.offsetMin = Vector2.zero;
        rtCodice.offsetMax = Vector2.zero;
        Panel_Codice.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.95f);

        // Título
        var goTitulo = CrearTextoUI(Panel_Codice.transform, "Text_TituloCodice", "<b>CODICE FOSIL</b>", 36);
        var rtTit = goTitulo.GetComponent<RectTransform>();
        rtTit.anchorMin = new Vector2(0.5f, 1);
        rtTit.anchorMax = new Vector2(0.5f, 1);
        rtTit.anchoredPosition = new Vector2(0, -40);
        rtTit.sizeDelta = new Vector2(500, 50);
        goTitulo.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        // Contador de fósiles
        var goFosiles = CrearTextoUI(Panel_Codice.transform, "Text_FosilesCodice", "Fosiles: 0", 24);
        var rtFos = goFosiles.GetComponent<RectTransform>();
        rtFos.anchorMin = new Vector2(0.5f, 1);
        rtFos.anchorMax = new Vector2(0.5f, 1);
        rtFos.anchoredPosition = new Vector2(0, -85);
        rtFos.sizeDelta = new Vector2(400, 35);
        var tmpFos = goFosiles.GetComponent<TextMeshProUGUI>();
        tmpFos.alignment = TextAlignmentOptions.Center;
        tmpFos.color = new Color(1f, 0.85f, 0.4f);
        Text_FosilesCodice = tmpFos;

        // Botón cerrar (esquina arriba-derecha)
        var goCerrar = CrearBotonUI(Panel_Codice.transform, "Btn_CerrarCodice_Dyn", "X", 0, 0, 60, 50);
        var rtCer = goCerrar.GetComponent<RectTransform>();
        rtCer.anchorMin = new Vector2(1, 1);
        rtCer.anchorMax = new Vector2(1, 1);
        rtCer.anchoredPosition = new Vector2(-40, -30);
        Btn_CerrarCodice = goCerrar.GetComponent<Button>();

        // Contenedor de nodos (Grid: 5 columnas x 3 ramas)
        var goGrid = new GameObject("Contenedor_NodosCodice", typeof(RectTransform), typeof(GridLayoutGroup));
        goGrid.transform.SetParent(Panel_Codice.transform, false);
        var rtGrid = goGrid.GetComponent<RectTransform>();
        rtGrid.anchorMin = new Vector2(0, 0);
        rtGrid.anchorMax = new Vector2(1, 1);
        rtGrid.offsetMin = new Vector2(60, 60);
        rtGrid.offsetMax = new Vector2(-60, -130);
        var grid = goGrid.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(180, 100);
        grid.spacing = new Vector2(15, 15);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;
        Contenedor_NodosCodice = goGrid.transform;

        // Template del nodo (inactivo, usado como prefab por GenerarNodosCodice)
        var goTemplate = new GameObject("NodoCodice_Template",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        goTemplate.transform.SetParent(Panel_Codice.transform, false);
        goTemplate.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 100);
        goTemplate.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
        var goTplTxt = CrearTextoUI(goTemplate.transform, "Text", "Nodo", 14);
        var rtTplTxt = goTplTxt.GetComponent<RectTransform>();
        rtTplTxt.anchorMin = Vector2.zero;
        rtTplTxt.anchorMax = Vector2.one;
        rtTplTxt.offsetMin = new Vector2(4, 4);
        rtTplTxt.offsetMax = new Vector2(-4, -4);
        goTplTxt.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        goTemplate.GetComponent<Button>().targetGraphic = goTemplate.GetComponent<Image>();
        goTemplate.SetActive(false);
        Prefab_NodoCodice = goTemplate;

        // ───── PANEL POPUP CODICE (modal) ─────
        Panel_PopupCodice = new GameObject("Panel_PopupCodice_Dyn",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup));
        Panel_PopupCodice.transform.SetParent(canvasRoot, false);
        var rtPopup = Panel_PopupCodice.GetComponent<RectTransform>();
        rtPopup.anchorMin = new Vector2(0.5f, 0.5f);
        rtPopup.anchorMax = new Vector2(0.5f, 0.5f);
        rtPopup.anchoredPosition = Vector2.zero;
        rtPopup.sizeDelta = new Vector2(550, 420);
        Panel_PopupCodice.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.18f, 0.98f);
        var vlgPopup = Panel_PopupCodice.GetComponent<VerticalLayoutGroup>();
        vlgPopup.padding = new RectOffset(25, 25, 25, 25);
        vlgPopup.spacing = 12;
        vlgPopup.childForceExpandWidth = true;
        vlgPopup.childForceExpandHeight = false;
        vlgPopup.childControlWidth = true;
        vlgPopup.childControlHeight = false;

        var goPopNombre = CrearTextoUI(Panel_PopupCodice.transform, "Text_PopupCodiceNombre",
            "Nombre del nodo", 26);
        goPopNombre.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);
        var tmpPopNom = goPopNombre.GetComponent<TextMeshProUGUI>();
        tmpPopNom.alignment = TextAlignmentOptions.Center;
        tmpPopNom.fontStyle = FontStyles.Bold;
        Text_PopupCodiceNombre = tmpPopNom;

        var goPopDesc = CrearTextoUI(Panel_PopupCodice.transform, "Text_PopupCodiceDescripcion",
            "Descripcion", 18);
        goPopDesc.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 80);
        var tmpPopDesc = goPopDesc.GetComponent<TextMeshProUGUI>();
        tmpPopDesc.alignment = TextAlignmentOptions.Center;
        tmpPopDesc.color = new Color(0.85f, 0.85f, 0.85f);
        Text_PopupCodiceDescripcion = tmpPopDesc;

        var goPopEf = CrearTextoUI(Panel_PopupCodice.transform, "Text_PopupCodiceEfecto",
            "Efecto: -", 20);
        goPopEf.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);
        var tmpPopEf = goPopEf.GetComponent<TextMeshProUGUI>();
        tmpPopEf.alignment = TextAlignmentOptions.Center;
        tmpPopEf.color = new Color(0.4f, 1f, 0.4f);
        Text_PopupCodiceEfecto = tmpPopEf;

        var goPopCo = CrearTextoUI(Panel_PopupCodice.transform, "Text_PopupCodiceCoste",
            "Coste: -", 18);
        goPopCo.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 32);
        var tmpPopCo = goPopCo.GetComponent<TextMeshProUGUI>();
        tmpPopCo.alignment = TextAlignmentOptions.Center;
        tmpPopCo.color = new Color(1f, 0.85f, 0.4f);
        Text_PopupCodiceCoste = tmpPopCo;

        // Fila de botones (comprar / cerrar)
        var goBotones = new GameObject("ContenedorBotonesPopup",
            typeof(RectTransform), typeof(HorizontalLayoutGroup));
        goBotones.transform.SetParent(Panel_PopupCodice.transform, false);
        goBotones.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 65);
        var hlgBot = goBotones.GetComponent<HorizontalLayoutGroup>();
        hlgBot.spacing = 20;
        hlgBot.childForceExpandWidth = true;
        hlgBot.childForceExpandHeight = true;
        hlgBot.childControlWidth = true;
        hlgBot.childControlHeight = true;
        hlgBot.padding = new RectOffset(20, 20, 5, 5);

        var goCompr = CrearBotonUI(goBotones.transform, "Btn_ComprarNodoCodice_Dyn", "COMPRAR", 0, 0, 0, 0);
        goCompr.GetComponent<Image>().color = new Color(0.2f, 0.55f, 0.2f);
        Btn_ComprarNodoCodice = goCompr.GetComponent<Button>();

        var goCerrarP = CrearBotonUI(goBotones.transform, "Btn_CerrarPopupCodice_Dyn", "CERRAR", 0, 0, 0, 0);
        goCerrarP.GetComponent<Image>().color = new Color(0.45f, 0.25f, 0.25f);
        Btn_CerrarPopupCodice = goCerrarP.GetComponent<Button>();

        Panel_PopupCodice.SetActive(false);

        // ───── BOTÓN ABRIR CODICE ─────
        // Va en el HUD principal, junto a los botones Evolucion/Misiones.
        // Replica el layout de Btn_AbrirEvolucion (mismo padre, mismos anchors)
        // y se posiciona con un offset vertical para quedar debajo.
        if (Btn_AbrirCodice == null && Btn_AbrirEvolucion != null)
        {
            var rtRef = Btn_AbrirEvolucion.GetComponent<RectTransform>();
            var goAbrir = CrearBotonUI(Btn_AbrirEvolucion.transform.parent,
                "Btn_AbrirCodice_Dyn", "CODICE FOSIL", 0, 0, rtRef.sizeDelta.x, rtRef.sizeDelta.y);
            var rtAbrir = goAbrir.GetComponent<RectTransform>();
            rtAbrir.anchorMin = rtRef.anchorMin;
            rtAbrir.anchorMax = rtRef.anchorMax;
            rtAbrir.pivot     = rtRef.pivot;
            // Pegar debajo del de Evolucion (o de Misiones si está más abajo)
            float yMasBajo = rtRef.anchoredPosition.y;
            if (Btn_AbrirMisiones != null)
            {
                var rtMis = Btn_AbrirMisiones.GetComponent<RectTransform>();
                if (rtMis.anchoredPosition.y < yMasBajo) yMasBajo = rtMis.anchoredPosition.y;
            }
            rtAbrir.anchoredPosition = new Vector2(rtRef.anchoredPosition.x,
                yMasBajo - rtRef.sizeDelta.y - 10);
            goAbrir.GetComponent<Image>().color = new Color(0.5f, 0.3f, 0.6f);
            Btn_AbrirCodice = goAbrir.GetComponent<Button>();
        }

        Panel_Codice.SetActive(false);

        Debug.Log("[UIManager] UI Codice construida programaticamente");
    }

    void AbrirCodice()
    {
        GenerarNodosCodice();
        MostrarPantalla(Panel_Codice);
    }

    void GenerarNodosCodice()
    {
        _nodosCodiceActivos.Clear();
        if (Contenedor_NodosCodice == null || Prefab_NodoCodice == null) return;

        foreach (Transform hijo in Contenedor_NodosCodice)
            Destroy(hijo.gameObject);

        var gc = GameController.Instance;
        if (gc?.Codice == null) return;

        foreach (var def in gc.Codice.ObtenerDefiniciones())
        {
            var nodoGO = Instantiate(Prefab_NodoCodice, Contenedor_NodosCodice);
            nodoGO.SetActive(true);  // El template está inactivo, los clones deben activarse
            RellenarNodoCodice(nodoGO, def);
            _nodosCodiceActivos.Add((nodoGO, def));
        }
    }

    void RellenarNodoCodice(GameObject nodoGO, DefinicionNodoCodice def)
    {
        var gc = GameController.Instance;
        if (gc?.Codice == null) return;

        int nivel = gc.Codice.NivelNodo(def.Id);
        bool nivelMax = nivel >= def.NivelMax;
        bool desbloqueado = gc.Codice.PrerequisitoCumplido(def);
        bool comprado = nivel > 0;

        // Color de rama + estado
        Color colorRama = def.Rama switch
        {
            TipoCodice.Abundancia => _colorCodiceAbundancia,
            TipoCodice.Eficiencia => _colorCodiceEficiencia,
            TipoCodice.Dominio    => _colorCodiceDominio,
            _ => Color.white
        };

        Color color;
        if (nivelMax) color = colorRama;
        else if (comprado) color = Color.Lerp(colorRama, Color.white, 0.3f);
        else if (desbloqueado) color = Color.Lerp(colorRama, Color.white, 0.6f);
        else color = _colorBloqueado;

        var img = nodoGO.GetComponent<Image>();
        if (img != null) img.color = color;

        // Texto
        var txt = nodoGO.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            if (!desbloqueado)
                txt.text = "?\n<size=60%>Bloqueado</size>";
            else if (nivelMax)
                txt.text = def.Nombre + "\n<size=60%>MAX</size>";
            else
                txt.text = def.Nombre + "\n<size=60%>Nv " + nivel + "/" + def.NivelMax + "</size>";
        }

        // Click → popup
        var btn = nodoGO.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            var defC = def;
            btn.onClick.AddListener(() => MostrarPopupCodice(defC));
            btn.interactable = desbloqueado;
        }
    }

    void MostrarPopupCodice(DefinicionNodoCodice def)
    {
        var gc = GameController.Instance;
        if (gc?.Codice == null || Panel_PopupCodice == null) return;

        _nodoCodiceSeleccionadoId = def.Id;
        int nivel = gc.Codice.NivelNodo(def.Id);
        bool nivelMax = nivel >= def.NivelMax;

        if (Text_PopupCodiceNombre != null)
            Text_PopupCodiceNombre.text = def.Nombre
                + (nivelMax ? "  [MAX]" : "  Nv " + nivel + "/" + def.NivelMax);

        if (Text_PopupCodiceDescripcion != null)
            Text_PopupCodiceDescripcion.text = def.Descripcion;

        if (Text_PopupCodiceEfecto != null)
        {
            double valorActual = nivel * def.ValorBonusPorNivel;
            double valorSiguiente = (nivel + 1) * def.ValorBonusPorNivel;
            string formato = FormatearBonusCodice(def, valorActual);
            string formatoSig = FormatearBonusCodice(def, valorSiguiente);
            Text_PopupCodiceEfecto.text = nivelMax
                ? "Efecto: " + formato
                : "Efecto: " + formato + " → " + formatoSig;
        }

        if (Text_PopupCodiceCoste != null)
            Text_PopupCodiceCoste.text = nivelMax
                ? "Completado"
                : "Coste: " + Formateador.Numero(def.CosteEnNivel(nivel)) + " Fosiles";

        if (Btn_ComprarNodoCodice != null)
        {
            Btn_ComprarNodoCodice.interactable = !nivelMax && gc.Codice.PuedeComprar(def.Id);
            var txtBtn = Btn_ComprarNodoCodice.GetComponentInChildren<TextMeshProUGUI>();
            if (txtBtn != null) txtBtn.text = nivelMax ? "MAX" : "COMPRAR";
        }

        Panel_PopupCodice.SetActive(true);
    }

    string FormatearBonusCodice(DefinicionNodoCodice def, double valor)
    {
        return def.TipoBonus switch
        {
            TipoBonus.MultiplicadorEV     => "+" + (valor * 100).ToString("F0") + "% EV/s",
            TipoBonus.BonusNocturno       => "+" + (valor * 100).ToString("F0") + "% nocturno",
            TipoBonus.BonusSinergias      => "+" + (valor * 100).ToString("F0") + "% sinergias",
            TipoBonus.ReduccionCosteMejoras => "-" + (valor * 100).ToString("F0") + "% coste mejoras",
            TipoBonus.ReduccionCosteCadenas => "-" + (valor * 100).ToString("F0") + "% coste cadenas",
            TipoBonus.NivelesGratisInicio  => "+" + valor.ToString("F0") + " niveles gratis",
            TipoBonus.BonusFosilesPrestige => "+" + (valor * 100).ToString("F0") + "% fosiles",
            TipoBonus.BonusCapCadena       => "+" + (valor * 100).ToString("F0") + "% cap cadenas",
            TipoBonus.BonusTap             => "+" + (valor * 100).ToString("F0") + "% tap",
            TipoBonus.ReduccionTapsCombo   => "-" + valor.ToString("F0") + " taps combo",
            TipoBonus.DuracionCombo        => "+" + valor.ToString("F0") + "s combo",
            TipoBonus.MultiplicadorCombo   => "+" + valor.ToString("F2") + "x combo",
            TipoBonus.AutoTap              => valor.ToString("F0") + " auto-taps",
            _ => "+" + valor.ToString("F2")
        };
    }

    void OnClickComprarNodoCodice()
    {
        if (string.IsNullOrEmpty(_nodoCodiceSeleccionadoId)) return;
        var gc = GameController.Instance;
        if (gc?.Codice == null) return;

        gc.ComprarNodoCodice(_nodoCodiceSeleccionadoId);

        var def = gc.Codice.BuscarDefinicion(_nodoCodiceSeleccionadoId);
        if (def != null) MostrarPopupCodice(def);
        ActualizarNodosCodice();
    }

    void ActualizarNodosCodice()
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        // Actualizar contador de fósiles
        if (Text_FosilesCodice != null)
            Text_FosilesCodice.text = "Fosiles: " + Formateador.Numero(gc.Estado.Prestige.Fosiles);

        foreach (var (nodoGO, def) in _nodosCodiceActivos)
        {
            if (nodoGO == null) continue;
            RellenarNodoCodice(nodoGO, def);
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // PANEL PRESTIGE
    // ══════════════════════════════════════════════════════════════════════

    void ActualizarPrestige()
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        var p = gc.Estado.Prestige;

        if (Text_FosilesActuales != null)
            Text_FosilesActuales.text = "Fosiles: " + Formateador.Numero(p.Fosiles)
                + "/" + Formateador.Numero(EstadoPrestige.CapFosiles)
                + "  (x" + p.MultiplicadorFosiles.ToString("F2") + ")";
        if (Text_GenesActuales != null)
            Text_GenesActuales.text = "Genes: " + Formateador.Numero(p.Genes)
                + "/" + Formateador.Numero(EstadoPrestige.CapGenes)
                + "  (x" + p.MultiplicadorGenes.ToString("F2") + ")";
        if (Text_QuarksActuales != null)
            Text_QuarksActuales.text = "Quarks: " + Formateador.Numero(p.Quarks)
                + "/" + Formateador.Numero(EstadoPrestige.CapQuarks)
                + "  (x" + p.MultiplicadorQuarks.ToString("F2") + ")";

        ActualizarBotonPrestige(Btn_Extincion, Text_GananciaExtincion,
            TipoPrestige.Extincion, "Extincion", "Fosiles", "Era 3+",
            p.Fosiles, 0.1, p.FosilesAlMax);
        ActualizarBotonPrestige(Btn_Glaciacion, Text_GananciaGlaciacion,
            TipoPrestige.Glaciacion, "Glaciacion", "Genes", "Era 5+",
            p.Genes, 0.05, p.GenesAlMax);
        ActualizarBotonPrestige(Btn_BigBang, Text_GananciaBigBang,
            TipoPrestige.BigBang, "Big Bang", "Quarks", "Era 7+",
            p.Quarks, 2.0, p.QuarksAlMax);
    }

    void ActualizarBotonPrestige(Button btn, TextMeshProUGUI texto,
        TipoPrestige tipo, string nombre, string moneda, string req,
        double recursoActual, double multPorUnidad, bool alMax)
    {
        var gc = GameController.Instance;
        bool puede = gc.Prestige.PuedeHacer(tipo);

        if (texto == null) return;

        if (!puede)
        {
            if (btn != null) btn.interactable = false;
            texto.text = nombre + "\nRequiere " + req;
            return;
        }

        if (alMax)
        {
            if (btn != null) btn.interactable = false;
            texto.text = nombre + "\n<color=#FFD700>" + moneda + " AL MAXIMO</color>"
                + "\nx" + (1.0 + recursoActual * multPorUnidad).ToString("F1");
            return;
        }

        // Ganancia efectiva (respeta el cap restante)
        double ahora = gc.Prestige.GananciaEfectiva(tipo);
        if (btn != null) btn.interactable = ahora > 0;

        double en5m  = gc.Prestige.GananciaProyectada(tipo, 300f);
        double en30m = gc.Prestige.GananciaProyectada(tipo, 1800f);

        // Clampear proyecciones al cap restante
        double restante = tipo switch
        {
            TipoPrestige.Extincion  => EstadoPrestige.CapFosiles - recursoActual,
            TipoPrestige.Glaciacion => EstadoPrestige.CapGenes   - recursoActual,
            TipoPrestige.BigBang    => EstadoPrestige.CapQuarks  - recursoActual,
            _ => 0
        };
        en5m  = System.Math.Min(en5m, restante);
        en30m = System.Math.Min(en30m, restante);

        // Multiplicador actual → después de prestige
        double multActual = 1.0 + recursoActual * multPorUnidad;
        double multNuevo  = 1.0 + (recursoActual + ahora) * multPorUnidad;

        var sb = new System.Text.StringBuilder();
        sb.Append(nombre);
        sb.Append("\nAhora: +" + Formateador.Numero(ahora) + " " + moneda);
        sb.Append("  (x" + multActual.ToString("F1") + " -> x" + multNuevo.ToString("F1") + ")");
        if (en5m > ahora)
            sb.Append("\nEn 5 min: +" + Formateador.Numero(en5m) + " " + moneda);
        if (en30m > en5m)
            sb.Append("\nEn 30 min: +" + Formateador.Numero(en30m) + " " + moneda);

        texto.text = sb.ToString();
    }

    // ══════════════════════════════════════════════════════════════════════
    // HANDLERS
    // ══════════════════════════════════════════════════════════════════════

    void OnClickEvolucionar()
    {
        if (GameController.Instance == null || !GameController.Instance.PuedeAvanzarEra()) return;
        GameController.Instance.AvanzarEra();
    }

    void OnClickPrestige(TipoPrestige tipo)
    {
        GameController.Instance?.HacerPrestige(tipo);
        // La navegación y reset visual los maneja OnPrestigeRealizado via EventBus
    }

    void OnClickAceptarEvento()
    {
        var gc = GameController.Instance;
        if (gc?.Estado.EventoActivoId != null)
            gc.AceptarEvento(gc.Estado.EventoActivoId);
        Panel_Evento?.SetActive(false);
    }

    void OnClickRechazarEvento()
    {
        GameController.Instance?.RechazarEvento();
        Panel_Evento?.SetActive(false);
    }

    void OnClickReclamarBonus()
    {
        GameController.Instance?.ReclamarBonusDiario();
        Panel_BonusDiario?.SetActive(false);
    }

    void ComprobarBonusDiario()
    {
        if (GameController.Instance == null) return;
        double bonus = GameController.Instance.Racha.BonusDiarioPendiente();
        if (bonus > 0 && Panel_BonusDiario != null)
        {
            Panel_BonusDiario.SetActive(true);
            if (Text_BonusDiario != null)
                Text_BonusDiario.text = "Bonus diario: +" + Formateador.Numero(bonus) + " EV";
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // EVENTOS DEL BUS
    // ══════════════════════════════════════════════════════════════════════

    void OnEraAvanzada(EventoEraAvanzada evt)
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        var def = gc.Eras.ObtenerDefinicionActual();
        if (Text_NombreEra != null)
            Text_NombreEra.text = "ERA " + evt.NuevaEra + ": " + def.Nombre.ToUpper();
        if (Text_DescripcionEra != null)
            Text_DescripcionEra.text = def.Descripcion;

        Panel_EraDesbloqueada?.SetActive(true);
    }

    void OnEventoActivado(EventoEventoActivado evt)
    {
        var evDef = GameController.Instance?.Eventos.ObtenerEventoActivo();
        if (evDef == null) return;

        if (Text_NombreEvento != null) Text_NombreEvento.text = evDef.Nombre;
        if (Text_DescripcionEvento != null) Text_DescripcionEvento.text = evDef.Descripcion;

        Btn_AceptarEvento?.gameObject.SetActive(evDef.RequiereAccion);
        Btn_RechazarEvento?.gameObject.SetActive(evDef.RequiereAccion);
        Panel_Evento?.SetActive(true);
    }

    void OnEstancamiento(EventoEstancamientoDetectado evt)
    {
        Indicador_Estancamiento?.SetActive(true);
    }

    void OnSinergiaActivada(EventoSinergiaActivada evt)
    {
        Debug.Log("[UI] Sinergia activada: " + evt.IdSinergia);
    }

    void OnLogroDesbloqueado(EventoLogroDesbloqueado evt)
    {
        Debug.Log("[UI] Logro desbloqueado: " + evt.IdLogro);
    }

    void OnMejoraDesbloqueada(EventoMejoraDesbloqueada evt)
    {
        // Refrescar tarjetas si el panel está abierto
        if (Panel_Categoria != null && Panel_Categoria.activeSelf)
            ActualizarTarjetas();
    }

    void OnMisionCompletada(EventoMisionCompletada evt)
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        var def = gc.Misiones.BuscarDefinicion(evt.IdMision);
        string nombre = def != null ? def.Nombre : evt.IdMision;

        MostrarBannerMision("MISION COMPLETADA\n" + nombre
            + "\nRecoge tu recompensa en Misiones > Completadas");
    }

    void OnPrestigeRealizado(EventoPrestigeRealizado evt)
    {
        // Volver a pantalla principal
        MostrarPantalla(Panel_Principal);

        // Resetear visual del planeta a Era 1
        var gc = GameController.Instance;
        if (gc != null && eraManager != null)
            eraManager.AplicarEraVisual(gc.Estado.EraActual);

        // Notificar al jugador
        string tipo = evt.Tipo switch
        {
            TipoPrestige.Extincion  => "Extincion",
            TipoPrestige.Glaciacion => "Glaciacion",
            TipoPrestige.BigBang    => "Big Bang",
            _                       => "Prestige"
        };
        MostrarBannerMision(tipo + " completado\n+" + Formateador.Numero(evt.Ganancia) + " recursos");
    }

    // ══════════════════════════════════════════════════════════════════════
    // UTILIDADES
    // ══════════════════════════════════════════════════════════════════════


    // ══════════════════════════════════════════════════════════════════════


    // ══════════════════════════════════════════════════════════════════════
    // META PROXIMA VISIBLE
    // ══════════════════════════════════════════════════════════════════════

    void ActualizarMetaProxima()
    {
        if (Panel_MetaProxima == null) return;
        var gc = GameController.Instance;
        if (gc == null) return;

        // Si ya esta visible solo refrescar texto
        if (Panel_MetaProxima.activeSelf)
        {
            RefrescarTextoMeta();
            return;
        }

        // Esperar reaparicion si fue cerrado manualmente
        if (_timerReaparicion > 0) { _timerReaparicion -= INTERVALO_UI; return; }

        // Acumular tiempo sin comprar
        _timerSinComprar += INTERVALO_UI;
        if (_timerSinComprar < TIEMPO_SIN_COMPRAR) return;

        // Buscar la mejora mas cara que SE PUEDE comprar ahora
        var meta = BuscarMetaProxima();
        if (meta == null) return;

        Panel_MetaProxima.SetActive(true);
        RefrescarTextoMeta();
    }

    Terra.Data.DefinicionMejora BuscarMetaProxima()
    {
        var gc = GameController.Instance;
        if (gc == null) return null;

        var estado = gc.Estado;
        double evActual = estado.EnergiaVital;

        // La mejora mas cara que SE PUEDE comprar ahora mismo
        Terra.Data.DefinicionMejora mejoraCara = null;
        double mayorCoste = -1;

        foreach (TipoPilar pilar in System.Enum.GetValues(typeof(TipoPilar)))
        {
            foreach (var def in gc.Mejoras.ObtenerPorPilar(pilar))
            {
                if (!estado.MejoraDesbloqueada(def.Id)) continue;
                int nivel = estado.NivelMejora(def.Id);
                if (nivel >= def.NivelMax) continue;

                double coste = def.CosteEnNivel(nivel);
                if (coste > evActual) continue; // no se puede comprar

                if (coste > mayorCoste)
                {
                    mayorCoste = coste;
                    mejoraCara = def;
                }
            }
        }

        return mejoraCara;
    }

    void RefrescarTextoMeta()
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        var meta = BuscarMetaProxima();
        if (meta == null) { Panel_MetaProxima?.SetActive(false); return; }

        var estado = gc.Estado;
        double ev = estado.EnergiaVital;
        int nivel = estado.NivelMejora(meta.Id);
        double coste = meta.CosteEnNivel(nivel);

        if (Text_MetaNombre != null)
            Text_MetaNombre.text = "Puedes mejorar: " + meta.Nombre + " Lv" + (nivel + 1);

        if (Text_MetaProgreso != null)
            Text_MetaProgreso.text = "Coste: " + Formateador.Numero(coste) + " EV";

        if (Slider_MetaProgreso != null)
            Slider_MetaProgreso.value = coste > 0 ? (float)System.Math.Min(ev / coste, 1.0) : 1f;
    }

    void CerrarMetaManual()
    {
        Panel_MetaProxima?.SetActive(false);
        _timerSinComprar = 0f;
        _timerReaparicion = TIEMPO_REAPARICION;
    }

    void OnMejoraCompradaResetMeta(EventoMejoraComprada _)
    {
        NotificarActividad();
    }

    public void NotificarActividad()
    {
        _timerSinComprar = 0f;
        _timerReaparicion = 0f;
        Panel_MetaProxima?.SetActive(false);
    }

    void MostrarPantalla(GameObject pantalla)
    {
        Panel_Principal?.SetActive(pantalla == Panel_Principal);
        Panel_Categoria?.SetActive(pantalla == Panel_Categoria);
        Panel_Prestige?.SetActive(pantalla == Panel_Prestige);
        Panel_Evolucion?.SetActive(pantalla == Panel_Evolucion);
        Panel_Misiones?.SetActive(pantalla == Panel_Misiones);
        Panel_Codice?.SetActive(pantalla == Panel_Codice);
        Panel_PopupNodo?.SetActive(false);
        Panel_PopupCodice?.SetActive(false);
        // Evento y EraDesbloqueada se manejan como overlays, no cierran el resto
    }
}
