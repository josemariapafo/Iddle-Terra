using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Terra.Controllers;
using Terra.Core;
using Terra.Data;
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

    // ── Indicador estancamiento ───────────────────────────────────────────
    [Header("Indicador estancamiento")]
    public GameObject Indicador_Estancamiento;
    public Button Btn_AbrirPrestige;

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
    }

    void DesuscribirEventos()
    {
        EventBus.Desuscribir<EventoEraAvanzada>(OnEraAvanzada);
        EventBus.Desuscribir<EventoEventoActivado>(OnEventoActivado);
        EventBus.Desuscribir<EventoEstancamientoDetectado>(OnEstancamiento);
        EventBus.Desuscribir<EventoSinergiaActivada>(OnSinergiaActivada);
        EventBus.Desuscribir<EventoLogroDesbloqueado>(OnLogroDesbloqueado);
        EventBus.Desuscribir<EventoMejoraDesbloqueada>(OnMejoraDesbloqueada);
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
            Text_BtnEvolucionar.text = puedeEvolucionar
                ? "EVOLUCIONAR"
                : "EVOLUCIONAR\n" + Formateador.Porcentaje(progreso);

        if (Indicador_Estancamiento != null)
            Indicador_Estancamiento.SetActive(gc.Estancamiento.EstaEstancado());

        ActualizarResumenCadenas();
        ActualizarMetaProxima();
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
    // PANEL PRESTIGE
    // ══════════════════════════════════════════════════════════════════════

    void ActualizarPrestige()
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        var p = gc.Estado.Prestige;

        if (Text_FosilesActuales != null)
            Text_FosilesActuales.text = "Fosiles: " + Formateador.Numero(p.Fosiles)
                + "  (x" + p.MultiplicadorFosiles.ToString("F2") + ")";
        if (Text_GenesActuales != null)
            Text_GenesActuales.text = "Genes: " + Formateador.Numero(p.Genes)
                + "  (x" + p.MultiplicadorGenes.ToString("F2") + ")";
        if (Text_QuarksActuales != null)
            Text_QuarksActuales.text = "Quarks: " + Formateador.Numero(p.Quarks)
                + "  (x" + p.MultiplicadorQuarks.ToString("F2") + ")";

        ActualizarBotonPrestige(Btn_Extincion, Text_GananciaExtincion,
            TipoPrestige.Extincion, "Extincion", "Fosiles", "Era 5+");
        ActualizarBotonPrestige(Btn_Glaciacion, Text_GananciaGlaciacion,
            TipoPrestige.Glaciacion, "Glaciacion", "Genes", "Era 7+");
        ActualizarBotonPrestige(Btn_BigBang, Text_GananciaBigBang,
            TipoPrestige.BigBang, "Big Bang", "Quarks", "Era 8");
    }

    void ActualizarBotonPrestige(Button btn, TextMeshProUGUI texto,
        TipoPrestige tipo, string nombre, string moneda, string req)
    {
        var gc = GameController.Instance;
        bool puede = gc.Prestige.PuedeHacer(tipo);
        if (btn != null) btn.interactable = puede;
        if (texto != null)
            texto.text = puede
                ? nombre + "\n+" + Formateador.Numero(gc.Prestige.GananciaEstimada(tipo)) + " " + moneda
                : nombre + "\nRequiere " + req;
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
        MostrarPantalla(Panel_Principal);
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
        // Evento y EraDesbloqueada se manejan como overlays, no cierran el resto
    }
}
