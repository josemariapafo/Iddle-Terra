using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Terra.Controllers;
using Terra.Core;

public class TapManager : MonoBehaviour
{
    [Header("Referencias")]
    public Collider coliderPlaneta;
    public Camera camaraPrincipal;
    public Canvas canvasUI;

    [Header("Prefab numero flotante (UI)")]
    public GameObject Prefab_NumeroFlotante;

    [Header("Tap")]
    public float segundosPorTap = 5f;
    public float umbralArrastre = 12f;

    [Header("Combo")]
    public int tapsParaCombo = 5;
    public float duracionCombo = 10f;
    public float multiplicadorCombo = 2f;
    public float tiempoResetCombo = 3f;

    [Header("UI Combo")]
    public Slider Slider_Combo;
    public TextMeshProUGUI Text_ComboContador;
    public TextMeshProUGUI Text_ComboActivo;
    public GameObject Panel_Combo;

    [Header("Colores")]
    public Color colorTapNormal = new Color(1f, 0.85f, 0.2f);
    public Color colorComboActivo = new Color(1f, 0.4f, 0f);
    public Color colorMeteoro = new Color(0.4f, 1f, 0.4f);

    private Vector2 _posicionInicio;
    private bool _presionando = false;
    private bool _esArrastre = false;
    private int _tapsCombo = 0;
    private float _timerResetCombo = 0f;
    private bool _comboActivo = false;
    private float _tiempoRestanteCombo = 0f;
    private MeteoroManager _meteoroManager;

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

    void ProcesarInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _presionando = true;
            _esArrastre = false;
            _posicionInicio = Input.mousePosition;
        }

        if (_presionando && Input.GetMouseButton(0))
        {
            if (Vector2.Distance(Input.mousePosition, _posicionInicio) > umbralArrastre)
                _esArrastre = true;
        }

        if (Input.GetMouseButtonUp(0) && _presionando)
        {
            if (!_esArrastre) EjecutarTap(Input.mousePosition);
            _presionando = false;
            _esArrastre = false;
        }

        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                _presionando = true;
                _esArrastre = false;
                _posicionInicio = t.position;
            }
            if (t.phase == TouchPhase.Moved &&
                Vector2.Distance(t.position, _posicionInicio) > umbralArrastre)
                _esArrastre = true;
            if (t.phase == TouchPhase.Ended && _presionando)
            {
                if (!_esArrastre) EjecutarTap(t.position);
                _presionando = false;
                _esArrastre = false;
            }
        }
    }

    void EjecutarTap(Vector2 posicionPantalla)
    {
        Ray rayo = camaraPrincipal.ScreenPointToRay(posicionPantalla);

        if (_meteoroManager != null && _meteoroManager.IntentarGolpearMeteoro(rayo))
        {
            double evMeteoro = GameController.Instance?.Estado.EVPorSegundo * 2.0 ?? 0;
            MostrarNumero(posicionPantalla, evMeteoro, colorMeteoro);
            UIManager.Instance?.NotificarActividad();
            return;
        }

        if (coliderPlaneta == null) return;
        if (!coliderPlaneta.Raycast(rayo, out RaycastHit hit, 20f)) return;

        Vector3 puntoEnPantalla = camaraPrincipal.WorldToScreenPoint(hit.point);
        TapearPlaneta(new Vector2(puntoEnPantalla.x, puntoEnPantalla.y));
    }

    void TapearPlaneta(Vector2 posicionPantalla)
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        UIManager.Instance?.NotificarActividad();

        double bonusTap = 1.0 + (gc.Codice?.BonusTap() ?? 0.0);
        double multCombo = _comboActivo
            ? multiplicadorCombo + (float)(gc.Codice?.MultiplicadorComboExtra() ?? 0.0)
            : 1.0;
        double evExtra = gc.Estado.EVPorSegundo * segundosPorTap * multCombo * bonusTap;
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
            int tapsNecesarios = tapsParaCombo - (gc.Codice?.ReduccionTapsCombo() ?? 0);
            if (tapsNecesarios < 2) tapsNecesarios = 2; // mínimo 2 taps
            if (_tapsCombo >= tapsNecesarios) ActivarCombo();
        }
    }

    void MostrarNumero(Vector2 posicionPantalla, double cantidad, Color color, string prefijo = "+")
    {
        if (Prefab_NumeroFlotante == null)
        {
            Debug.LogError("[TapManager] Prefab_NumeroFlotante es null");
            return;
        }
        if (canvasUI == null)
        {
            Debug.LogError("[TapManager] canvasUI es null");
            return;
        }

        GameObject obj = Instantiate(Prefab_NumeroFlotante, canvasUI.transform);
        Debug.Log($"[TapManager] Numero instanciado: {obj.name} en canvas {canvasUI.name}");

        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect == null)
        {
            // El prefab no tiene RectTransform — es un objeto 3D, no UI
            // Colocarlo en world space encima del planeta
            Debug.LogWarning("[TapManager] Prefab no tiene RectTransform, usando posicion 3D");
            obj.transform.SetParent(null);
            Ray rayo = camaraPrincipal.ScreenPointToRay(posicionPantalla);
            if (coliderPlaneta != null && coliderPlaneta.Raycast(rayo, out RaycastHit hit, 20f))
                obj.transform.position = hit.point + (hit.point - Vector3.zero).normalized * 0.3f;
        }
        else
        {
            // Es UI — posicionar en canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasUI.GetComponent<RectTransform>(),
                posicionPantalla,
                canvasUI.renderMode == RenderMode.ScreenSpaceOverlay ? null : camaraPrincipal,
                out Vector2 localPos);

            localPos += new Vector2(Random.Range(-25f, 25f), Random.Range(20f, 50f));
            rect.anchoredPosition = localPos;
            Debug.Log($"[TapManager] Posicion local en canvas: {localPos}");
        }

        var nf = obj.GetComponent<NumeroFlotante>();
        if (nf != null)
            nf.Configurar(prefijo + Formateador.Numero(cantidad), color);
        else
        {
            var tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) { tmp.text = prefijo + Formateador.Numero(cantidad); tmp.color = color; }
            else Debug.LogError("[TapManager] No se encontro TextMeshProUGUI en el prefab");
        }
    }

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
        float duracionExtra = GameController.Instance?.Codice?.DuracionComboExtra() ?? 0f;
        _comboActivo = true; _tiempoRestanteCombo = duracionCombo + duracionExtra; _tapsCombo = 0;
        if (Panel_Combo != null) Panel_Combo.SetActive(false);
        if (Text_ComboActivo != null) Text_ComboActivo.gameObject.SetActive(true);
        ActualizarUIComboActivo();
        EventBus.Publicar(new EventoComboActivado());
    }

    void TerminarCombo()
    {
        _comboActivo = false;
        if (Text_ComboActivo != null) Text_ComboActivo.gameObject.SetActive(false);
    }

    void ActualizarUICombo()
    {
        int tapsNecesarios = tapsParaCombo - (GameController.Instance?.Codice?.ReduccionTapsCombo() ?? 0);
        if (tapsNecesarios < 2) tapsNecesarios = 2;
        if (Slider_Combo != null)
            Slider_Combo.value = tapsNecesarios > 0 ? (float)_tapsCombo / tapsNecesarios : 0;
        if (Text_ComboContador != null)
            Text_ComboContador.text = _tapsCombo + " / " + tapsNecesarios;
    }

    void ActualizarUIComboActivo()
    {
        if (Text_ComboActivo != null)
        {
            float multReal = multiplicadorCombo + (float)(GameController.Instance?.Codice?.MultiplicadorComboExtra() ?? 0.0);
            Text_ComboActivo.text = "COMBO x" + multReal.ToString("F1")
                + "  " + Mathf.CeilToInt(_tiempoRestanteCombo) + "s";
        }
    }
}
