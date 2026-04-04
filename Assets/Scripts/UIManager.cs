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

    static readonly string[][] _zonasNombres =
    {
        new[]{ "Baja atmosfera",  "Nubes",         "Alta atmosfera"  },
        new[]{ "Superficie",      "Zona media",     "Profundidades"   },
        new[]{ "Superficie",      "Corteza",        "Interior"        },
        new[]{ "Microvida",       "Vida compleja",  "Vida inteligente"},
    };

    static readonly string[] _nombresPilar = { "ATMOSFERA", "OCEANOS", "TIERRA", "VIDA" };
    static readonly string[] _eslabonNombres = { "Generación", "Procesamiento", "Distribución" };

    // ══════════════════════════════════════════════════════════════════════
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    void Start()
    {
        ConfigurarBotones();
        SuscribirEventos();
        InicializarPanelMisiones();
        InicializarRevelacionProgresiva();
        MostrarPantalla(Panel_Principal);
        ComprobarBonusDiario();
    }

    void Update()
    {
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

        AplicarTab();
        MostrarPantalla(Panel_Categoria);
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
        Panel_PopupNodo?.SetActive(false);
        // Evento y EraDesbloqueada se manejan como overlays, no cierran el resto
    }
}
