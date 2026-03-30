using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Terra.Controllers;
using Terra.Core;
using Terra.Data;

/// <summary>
/// UIManager v2 — conecta toda la UI con el sistema idle.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

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
    private float _timerUI = 0f;
    private const float INTERVALO_UI = 0.2f;
    private readonly List<(GameObject tarjeta, DefinicionMejora def)> _tarjetasActivas
        = new List<(GameObject, DefinicionMejora)>();

    static readonly string[][] _zonasNombres =
    {
        new[]{ "Baja atmosfera",  "Nubes",         "Alta atmosfera"  },
        new[]{ "Superficie",      "Zona media",     "Profundidades"   },
        new[]{ "Superficie",      "Corteza",        "Interior"        },
        new[]{ "Microvida",       "Vida compleja",  "Vida inteligente"},
    };

    static readonly string[] _nombresPilar = { "ATMOSFERA", "OCEANOS", "TIERRA", "VIDA" };

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
            ActualizarTarjetas();
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

        ActualizarMetaProxima();
    }

    // ══════════════════════════════════════════════════════════════════════
    // PANEL CATEGORÍA
    // ══════════════════════════════════════════════════════════════════════

    void AbrirCategoria(TipoPilar pilar)
    {
        _categoriaActual = pilar;

        if (Text_NombreCategoria != null)
            Text_NombreCategoria.text = _nombresPilar[(int)pilar];
        if (Text_TituloZona0 != null) Text_TituloZona0.text = _zonasNombres[(int)pilar][0];
        if (Text_TituloZona1 != null) Text_TituloZona1.text = _zonasNombres[(int)pilar][1];
        if (Text_TituloZona2 != null) Text_TituloZona2.text = _zonasNombres[(int)pilar][2];

        GenerarTarjetas(pilar);
        MostrarPantalla(Panel_Categoria);
    }

    void GenerarTarjetas(TipoPilar pilar)
    {
        _tarjetasActivas.Clear();

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

        if (Text_ProduccionCategoria != null)
            Text_ProduccionCategoria.text = Formateador.Numero(prodTotal) + " EV/s";
    }

    void RellenarTarjeta(GameObject tarjeta, DefinicionMejora def)
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        bool desbloqueada = gc.Estado.MejoraDesbloqueada(def.Id);
        int nivel = gc.Estado.NivelMejora(def.Id);
        double coste = def.CosteEnNivel(nivel);
        bool puedePagar = gc.Estado.EnergiaVital >= coste;

        var textos = tarjeta.GetComponentsInChildren<TextMeshProUGUI>(true);
        var botones = tarjeta.GetComponentsInChildren<Button>(true);
        var sliders = tarjeta.GetComponentsInChildren<Slider>(true);

        // Texto 0 — nombre y nivel
        if (textos.Length > 0)
            textos[0].text = desbloqueada
                ? def.Nombre + "\nNv " + nivel + "/" + def.NivelMax
                : def.Nombre + "\n[Era " + def.EraRequerida + "]";

        // Texto 1 — producción o descripción
        if (textos.Length > 1)
            textos[1].text = nivel > 0
                ? "+" + Formateador.Numero(def.ProduccionEnNivel(nivel) * def.MultiplicadorHito(nivel)) + " EV/s"
                : def.Descripcion;

        // Texto 2 — coste del botón
        if (textos.Length > 2)
            textos[2].text = desbloqueada
                ? Formateador.Numero(coste) + " EV"
                : "Bloqueada";

        // Slider de progreso hacia próximo hito
        if (sliders.Length > 0 && desbloqueada)
        {
            int[] hitos = { 10, 25, 50, 100, 200, 500 };
            int anterior = 0, siguiente = def.NivelMax;
            foreach (var h in hitos)
            {
                if (nivel < h) { siguiente = h; break; }
                anterior = h;
            }
            sliders[0].value = siguiente > anterior
                ? (float)(nivel - anterior) / (siguiente - anterior)
                : 1f;
        }

        // Botón Comprar ×1
        if (botones.Length > 0)
        {
            botones[0].interactable = desbloqueada && nivel < def.NivelMax && puedePagar;
            botones[0].onClick.RemoveAllListeners();
            var defCopia = def;
            var tarjetaCopia = tarjeta;
            botones[0].onClick.AddListener(() =>
            {
                if (GameController.Instance.ComprarMejora(defCopia.Id))
                    RellenarTarjeta(tarjetaCopia, defCopia);
            });
        }

        // Botón Comprar Max
        if (botones.Length > 1)
        {
            botones[1].interactable = desbloqueada && nivel < def.NivelMax && puedePagar;
            botones[1].onClick.RemoveAllListeners();
            var defCopia = def;
            var tarjetaCopia = tarjeta;
            botones[1].onClick.AddListener(() =>
            {
                if (GameController.Instance.ComprarMejoraMax(defCopia.Id) > 0)
                    RellenarTarjeta(tarjetaCopia, defCopia);
            });
        }

        // Ocultar tarjetas de eras futuras
        tarjeta.SetActive(desbloqueada || def.EraRequerida <= (gc.Estado.EraActual + 1));
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
            Text_ProduccionCategoria.text = Formateador.Numero(prodTotal) + " EV/s";
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
        // Al comprar cualquier mejora resetear el timer y ocultar el panel
        _timerSinComprar = 0f;
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
