using UnityEngine;
using TMPro;

/// <summary>
/// NumeroFlotante — anima el numero flotante en el Canvas.
/// Usa TextMeshProUGUI (compatible con el prefab UI existente).
/// </summary>
public class NumeroFlotante : MonoBehaviour
{
    [Header("Animacion")]
    public float velocidadSubida = 80f;     // pixels por segundo
    public float duracionVida = 1.3f;
    public float escalaInicial = 0.7f;
    public float escalaMaxima = 1.1f;
    public float tiempoEscalado = 0.12f;

    private TextMeshProUGUI _texto;
    private RectTransform _rect;
    private float _tiempoVivo = 0f;
    private Color _colorInicial;

    void Awake()
    {
        _texto = GetComponentInChildren<TextMeshProUGUI>();
        _rect = GetComponent<RectTransform>();
        if (_texto != null) _colorInicial = _texto.color;
        transform.localScale = Vector3.one * escalaInicial;
    }

    public void Configurar(string texto, Color color)
    {
        if (_texto == null) _texto = GetComponentInChildren<TextMeshProUGUI>();
        if (_texto != null)
        {
            _texto.text = texto;
            _texto.color = color;
            _colorInicial = color;
        }
    }

    void Update()
    {
        _tiempoVivo += Time.deltaTime;

        // Subir flotando
        if (_rect != null)
            _rect.anchoredPosition += Vector2.up * velocidadSubida * Time.deltaTime;

        // Escala al inicio
        float pe = Mathf.Clamp01(_tiempoVivo / tiempoEscalado);
        transform.localScale = Vector3.one * Mathf.Lerp(escalaInicial, escalaMaxima, pe);

        // Fade out segunda mitad
        float progreso = _tiempoVivo / duracionVida;
        if (progreso > 0.5f && _texto != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, (progreso - 0.5f) * 2f);
            _texto.color = new Color(_colorInicial.r, _colorInicial.g, _colorInicial.b, alpha);
        }

        if (_tiempoVivo >= duracionVida)
            Destroy(gameObject);
    }
}
