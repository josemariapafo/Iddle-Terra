using UnityEngine;

/// <summary>
/// Meteoro — objeto individual que vuela hacia el planeta.
/// Se crea y destruye dinámicamente por MeteoroManager.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class Meteoro : MonoBehaviour
{
    // ── Configuración (asignada por MeteoroManager al instanciar) ─────────
    [HideInInspector] public float velocidad      = 2.5f;
    [HideInInspector] public float radioPlaneta   = 1.1f;
    [HideInInspector] public MeteoroManager manager;

    // ── Estado ────────────────────────────────────────────────────────────
    public bool Eliminado  { get; private set; } = false;
    public bool Impactado  { get; private set; } = false;

    // ── Visual ────────────────────────────────────────────────────────────
    private Renderer _renderer;
    private Vector3 _direccion;
    private float _rotacionVelocidad;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _direccion = (Vector3.zero - transform.position).normalized;
        _rotacionVelocidad = Random.Range(60f, 180f);

        // Material gris rocoso
        if (_renderer != null)
        {
            _renderer.material = new Material(Shader.Find("Standard"));
            _renderer.material.color = new Color(
                Random.Range(0.35f, 0.50f),
                Random.Range(0.28f, 0.40f),
                Random.Range(0.20f, 0.32f)
            );
            // Rugosidad metalica para aspecto rocoso
            _renderer.material.SetFloat("_Metallic", 0f);
            _renderer.material.SetFloat("_Glossiness", 0.05f);
        }
    }

    void Update()
    {
        if (Eliminado || Impactado) return;

        // Mover hacia el planeta
        transform.position += _direccion * velocidad * Time.deltaTime;

        // Rotar para dar sensacion de caida
        transform.Rotate(Vector3.one * _rotacionVelocidad * Time.deltaTime);

        // Comprobar impacto con el planeta
        if (transform.position.magnitude <= radioPlaneta)
            OnImpacto();
    }

    // ── Golpeado por el jugador ───────────────────────────────────────────
    public void SerGolpeado()
    {
        if (Eliminado || Impactado) return;
        Eliminado = true;

        manager?.OnMeteoroEliminado(this);

        // Efecto de destruccion — escala a 0 rapido
        StartCoroutine(AnimarDestruccion());
    }

    // ── Impacto en el planeta ─────────────────────────────────────────────
    void OnImpacto()
    {
        if (Eliminado || Impactado) return;
        Impactado = true;

        manager?.OnMeteoroImpacto(this);
        Destroy(gameObject);
    }

    System.Collections.IEnumerator AnimarDestruccion()
    {
        float t = 0f;
        Vector3 escalaInicial = transform.localScale;

        while (t < 0.3f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, t / 0.3f);
            yield return null;
        }

        Destroy(gameObject);
    }
}
