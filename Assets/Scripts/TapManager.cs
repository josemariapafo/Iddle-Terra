using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Terra.Controllers;
using Terra.Core;

/// <summary>
/// TapManager — tap en planeta y meteoritos.
/// Requiere InputManager en la escena.
/// </summary>
public class TapManager : MonoBehaviour
{
    [Header("Referencias")]
    public Collider coliderPlaneta;
    public Camera camaraPrincipal;

    [Header("Prefab numero flotante 3D")]
    public GameObject Prefab_NumeroFlotante;

    [Header("Tap")]
    public float segundosPorTap = 0.5f;

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

    private const float RADIO_SUPERFICIE = 1.05f;
    private int _tapsCombo = 0;
    private float _timerResetCombo = 0f;
    private bool _comboActivo = false;
    private float _tiempoRestanteCombo = 0f;
    private MeteoroManager _meteoroManager;

    void Start()
    {
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
        _meteoroManager = FindObjectOfType<MeteoroManager>();
        if (Panel_Combo != null) Panel_Combo.SetActive(false);
        if (Text_ComboActivo != null) Text_ComboActivo.gameObject.SetActive(false);
        ActualizarUICombo();
    }

    void Update()
    {
        // Combo timers
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

        // Solo procesar taps confirmados por InputManager
        if (InputManager.Instance == null || !InputManager.Instance.TapEsteFrame) return;
        ProcesarTap(InputManager.Instance.PosicionToque);
    }

    void ProcesarTap(Vector2 pos)
    {
        Ray rayo = camaraPrincipal.ScreenPointToRay(pos);

        // 1. Meteorito
        if (_meteoroManager != null && _meteoroManager.IntentarGolpearMeteoro(rayo))
        {
            double ev = GameController.Instance?.Estado.EVPorSegundo * 2.0 ?? 0;
            MostrarNumeroEnPlaneta(ev, colorMeteoro);
            return;
        }

        // 2. Planeta
        if (coliderPlaneta == null) return;
        if (!coliderPlaneta.Raycast(rayo, out RaycastHit hit, 20f)) return;
        TapearPlaneta(hit.point);
    }

    void TapearPlaneta(Vector3 punto)
    {
        var gc = GameController.Instance;
        if (gc == null) return;

        double mult = _comboActivo ? multiplicadorCombo : 1.0;
        double ev = gc.Estado.EVPorSegundo * segundosPorTap * mult;
        gc.Estado.EnergiaVital += ev;

        Color color = _comboActivo ? colorComboActivo : colorTapNormal;
        string prefijo = _comboActivo ? "COMBO +" : "+";
        MostrarNumeroEn(punto, ev, color, prefijo);

        if (!_comboActivo)
        {
            _tapsCombo++;
            _timerResetCombo = tiempoResetCombo;
            if (Panel_Combo != null) Panel_Combo.SetActive(true);
            ActualizarUICombo();
            if (_tapsCombo >= tapsParaCombo) ActivarCombo();
        }
    }

    void MostrarNumeroEn(Vector3 pos, double cantidad, Color color, string prefijo = "+")
    {
        if (Prefab_NumeroFlotante == null) return;
        var obj = Instantiate(Prefab_NumeroFlotante, pos + Random.insideUnitSphere * 0.15f, Quaternion.identity);
        obj.GetComponent<NumeroFlotante>()?.Configurar(prefijo + Formateador.Numero(cantidad), color);
    }

    void MostrarNumeroEnPlaneta(double cantidad, Color color, string prefijo = "+")
    {
        Vector3 dir = Random.onUnitSphere;
        Vector3 cam = (camaraPrincipal.transform.position).normalized;
        if (Vector3.Dot(dir, cam) < 0) dir = -dir;
        MostrarNumeroEn(dir * RADIO_SUPERFICIE, cantidad, color, prefijo);
    }

    void ActivarCombo()
    {
        _comboActivo = true; _tiempoRestanteCombo = duracionCombo; _tapsCombo = 0;
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
        if (Slider_Combo != null) Slider_Combo.value = (float)_tapsCombo / tapsParaCombo;
        if (Text_ComboContador != null) Text_ComboContador.text = _tapsCombo + " / " + tapsParaCombo;
    }

    void ActualizarUIComboActivo()
    {
        if (Text_ComboActivo != null)
            Text_ComboActivo.text = "COMBO x" + multiplicadorCombo.ToString("F0") + "  " + Mathf.CeilToInt(_tiempoRestanteCombo) + "s";
    }
}
