using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Terra.Controllers;
using Terra.Core;

/// <summary>
/// TapManager v3 — completamente autocontenido, sin InputManager.
/// Distingue tap de arrastre internamente.
/// Los numeros flotantes usan TextMeshProUGUI en Canvas.
/// </summary>
public class TapManager : MonoBehaviour
{
    [Header("Referencias")]
    public Collider coliderPlaneta;
    public Camera camaraPrincipal;
    public Canvas canvasUI;                  // Canvas donde se muestran los numeros

    [Header("Prefab numero flotante (UI)")]
    public GameObject Prefab_NumeroFlotante;    // Prefab con TextMeshProUGUI

    [Header("Tap")]
    public float segundosPorTap = 0.5f;
    public float umbralArrastre = 12f;         // pixeles de movimiento para considerar arrastre

    [Header("Combo")]
    public int tapsParaCombo = 10;
    public float duracionCombo = 3f;
    public float multiplicadorCombo = 2f;
    public float tiempoResetCombo = 1.5f;

    [Header("UI Combo")]
    public Slider Slider_Combo;
    public TextMeshProUGUI Text_ComboContador;
    public TextMeshProUGUI Text_ComboActivo;
    public GameObject Panel_Combo;

    [Header("Colores")]
    public Color colorTapNormal = new Color(1f, 0.85f, 0.2f);
    public Color colorComboActivo = new Color(1f, 0.4f, 0f);
    public Color colorMeteoro = new Color(0.4f, 1f, 0.4f);

    // ── Estado tap ────────────────────────────────────────────────────────
    private Vector2 _posicionInicio;
    private bool _presionando = false;
    private bool _esArrastre = false;

    // ── Estado combo ──────────────────────────────────────────────────────
    private int _tapsCombo = 0;
    private float _timerResetCombo = 0f;
    private bool _comboActivo = false;
    private float _tiempoRestanteCombo = 0f;

    private MeteoroManager _meteoroManager;

    // ══════════════════════════════════════════════════════════════════════
    void Start()
    {
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
        if (canvasUI == null) canvasUI = FindObjectOfType<Canvas>();
        _meteoroManager = FindObjectOfType<MeteoroManager>();

        if (Panel_Combo != null) Panel_Combo.SetActive(false);
        if (Text_ComboActivo != null) Text_ComboActivo.gameObject.SetActive(false);
        ActualizarUICombo();
    }

    void Update()
    {
        ActualizarComboTimers();
        ProcesarInput();
    }

    // ══════════════════════════════════════════════════════════════════════
    // INPUT — autocontenido, sin InputManager
    // ══════════════════════════════════════════════════════════════════════

    void ProcesarInput()
    {
        // ── Mouse ─────────────────────────────────────────────────────────
        if (Input.GetMouseButtonDown(0))
        {
            _presionando = true;
            _esArrastre = false;
            _posicionInicio = Input.mousePosition;
        }

        if (_presionando && Input.GetMouseButton(0))
        {
            float dist = Vector2.Distance(Input.mousePosition, _posicionInicio);
            if (dist > umbralArrastre)
                _esArrastre = true;
        }

        if (Input.GetMouseButtonUp(0) && _presionando)
        {
            if (!_esArrastre)
                EjecutarTap(Input.mousePosition);

            _presionando = false;
            _esArrastre = false;
        }

        // ── Touch ─────────────────────────────────────────────────────────
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                _presionando = true;
                _esArrastre = false;
                _posicionInicio = t.position;
            }

            if (t.phase == TouchPhase.Moved)
            {
                float dist = Vector2.Distance(t.position, _posicionInicio);
                if (dist > umbralArrastre) _esArrastre = true;
            }

            if (t.phase == TouchPhase.Ended && _presionando)
            {
                if (!_esArrastre)
                    EjecutarTap(t.position);

                _presionando = false;
                _esArrastre = false;
            }
        }
    }

    void EjecutarTap(Vector2 posicionPantalla)
    {
        Ray rayo = camaraPrincipal.ScreenPointToRay(posicionPantalla);

        // 1. Intentar golpear meteorito
        if (_meteoroManager != null && _meteoroManager.IntentarGolpearMeteoro(rayo))
        {
            double evMeteoro = GameController.Instance?.Estado.EVPorSegundo * 2.0 ?? 0;
            MostrarNumero(posicionPantalla, evMeteoro, colorMeteoro);
            return;
        }

        // 2. Intentar tocar el planeta
        if (coliderPlaneta == null) return;
        if (!coliderPlaneta.Raycast(rayo, out RaycastHit hit, 20f)) return;

        // Convertir punto 3D a posicion en pantalla
        Vector3 puntoEnPantalla = camaraPrincipal.WorldToScreenPoint(hit.point);
        TapearPlaneta(new Vector2(puntoEnPantalla.x, puntoEnPantalla.y));
    }

    void TapearPlaneta(Vector2 posicionPantalla)
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        double mult = _comboActivo ? multiplicadorCombo : 1.0;
        double evExtra = gc.Estado.EVPorSegundo * segundosPorTap * mult;
        gc.Estado.EnergiaVital += evExtra;

        Color color = _comboActivo ? colorComboActivo : colorTapNormal;
        string prefijo = _comboActivo ? "COMBO +" : "+";
        MostrarNumero(posicionPantalla, evExtra, color, prefijo);

        if (!_comboActivo)
        {
            _tapsCombo++;
            _timerResetCombo = tiempoResetCombo;
            if (Panel_Combo != null) Panel_Combo.SetActive(true);
            ActualizarUICombo();
            if (_tapsCombo >= tapsParaCombo) ActivarCombo();
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // NUMEROS FLOTANTES — usa TextMeshProUGUI en Canvas
    // ══════════════════════════════════════════════════════════════════════

    void MostrarNumero(Vector2 posicionPantalla, double cantidad, Color color, string prefijo = "+")
    {
        if (Prefab_NumeroFlotante == null)
        {
            Debug.LogWarning("[TapManager] Prefab_NumeroFlotante no asignado.");
            return;
        }
        if (canvasUI == null) return;

        // Instanciar en el canvas
        GameObject obj = Instantiate(Prefab_NumeroFlotante, canvasUI.transform);

        // Posicionar en espacio del canvas
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasUI.GetComponent<RectTransform>(),
                posicionPantalla,
                canvasUI.renderMode == RenderMode.ScreenSpaceOverlay ? null : camaraPrincipal,
                out Vector2 localPos);

            // Offset aleatorio pequeno
            localPos += new Vector2(Random.Range(-25f, 25f), Random.Range(10f, 40f));
            rect.anchoredPosition = localPos;
        }

        // Configurar texto y color
        var nf = obj.GetComponent<NumeroFlotante>();
        if (nf != null)
            nf.Configurar(prefijo + Formateador.Numero(cantidad), color);
        else
        {
            // Fallback: buscar TextMeshProUGUI directamente
            var tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = prefijo + Formateador.Numero(cantidad);
                tmp.color = color;
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // COMBO
    // ══════════════════════════════════════════════════════════════════════

    void ActualizarComboTimers()
    {
        if (_tapsCombo > 0 && !_comboActivo)
        {
            _timerResetCombo -= Time.deltaTime;
            if (_timerResetCombo <= 0)
            {
                _tapsCombo = 0;
                ActualizarUICombo();
                if (Panel_Combo != null) Panel_Combo.SetActive(false);
            }
        }

        if (_comboActivo)
        {
            _tiempoRestanteCombo -= Time.deltaTime;
            ActualizarUIComboActivo();
            if (_tiempoRestanteCombo <= 0) TerminarCombo();
        }
    }

    void ActivarCombo()
    {
        _comboActivo = true;
        _tiempoRestanteCombo = duracionCombo;
        _tapsCombo = 0;
        if (Panel_Combo != null) Panel_Combo.SetActive(false);
        if (Text_ComboActivo != null) Text_ComboActivo.gameObject.SetActive(true);
        ActualizarUIComboActivo();
    }

    void TerminarCombo()
    {
        _comboActivo = false;
        if (Text_ComboActivo != null) Text_ComboActivo.gameObject.SetActive(false);
    }

    void ActualizarUICombo()
    {
        if (Slider_Combo != null)
            Slider_Combo.value = tapsParaCombo > 0 ? (float)_tapsCombo / tapsParaCombo : 0;
        if (Text_ComboContador != null)
            Text_ComboContador.text = _tapsCombo + " / " + tapsParaCombo;
    }

    void ActualizarUIComboActivo()
    {
        if (Text_ComboActivo != null)
            Text_ComboActivo.text = "COMBO x" + multiplicadorCombo.ToString("F0")
                + "  " + Mathf.CeilToInt(_tiempoRestanteCombo) + "s";
    }
}
