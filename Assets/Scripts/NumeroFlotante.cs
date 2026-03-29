using UnityEngine;
using TMPro;

/// <summary>
/// NumeroFlotante 3D — aparece en la superficie del planeta
/// y sube flotando en world space. No usa Canvas.
/// Requiere TextMeshPro en el mismo objeto.
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class NumeroFlotante : MonoBehaviour
{
    [Header("Animacion")]
    public float velocidadSubida = 0.8f;
    public float duracionVida = 1.4f;
    public float escalaInicial = 0.08f;
    public float escalaMaxima = 0.14f;
    public float tiempoEscalado = 0.12f;

    private TextMeshPro _texto;
    private float _tiempoVivo = 0f;
    private Color _colorInicial;
    private Camera _camara;
    private Vector3 _dirSubida;

    void Awake()
    {
        _texto = GetComponent<TextMeshPro>();
        _camara = Camera.main;

        // Subir en la direccion opuesta al centro del planeta (hacia arriba local)
        _dirSubida = (transform.position - Vector3.zero).normalized;

        transform.localScale = Vector3.one * escalaInicial;
    }

    public void Configurar(string texto, Color color)
    {
        if (_texto == null) _texto = GetComponent<TextMeshPro>();
        _texto.text = texto;
        _texto.color = color;
        _colorInicial = color;
    }

    void Update()
    {
        _tiempoVivo += Time.deltaTime;

        // Siempre mirar a la camara
        if (_camara != null)
            transform.LookAt(_camara.transform);

        // Subir flotando hacia afuera del planeta
        transform.position += _dirSubida * velocidadSubida * Time.deltaTime;

        // Escala: crece al inicio
        float progresoEscala = Mathf.Clamp01(_tiempoVivo / tiempoEscalado);
        float escala = Mathf.Lerp(escalaInicial, escalaMaxima, progresoEscala);
        transform.localScale = Vector3.one * escala;

        // Fade out en la segunda mitad
        float progreso = _tiempoVivo / duracionVida;
        if (progreso > 0.5f)
        {
            float alpha = Mathf.Lerp(1f, 0f, (progreso - 0.5f) * 2f);
            _texto.color = new Color(_colorInicial.r, _colorInicial.g, _colorInicial.b, alpha);
        }

        if (_tiempoVivo >= duracionVida)
            Destroy(gameObject);
    }
}
