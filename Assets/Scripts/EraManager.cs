using UnityEngine;

public class EraManager : MonoBehaviour
{
    [System.Serializable]
    public class EraData
    {
        public string nombre;
        [TextArea(1, 2)]
        public string descripcion;

        [Header("Texturas")]
        public Texture2D texturaDia;
        public Texture2D texturaNoche;

        [Header("Atmosfera")]
        [Range(0f, 3f)]
        public float atmosferaIntensidad = 1.5f;
        public Color atmosferaColor = new Color(0.4f, 0.6f, 1.0f, 1.0f);
    }

    [Header("Eras (Era1 a Era8)")]
    public EraData[] eras = new EraData[8];

    [Header("Referencias")]
    public Renderer planetaRenderer;
    public Renderer atmosferaRenderer;
    public TransicionEra transicion;

    [SerializeField] private int _eraActual = 0;

    public int EraActual => _eraActual + 1;

    void Start()
    {
        AplicarEraDesdeTransicion(_eraActual);
    }

    /// <summary>
    /// Llamado desde UIManager al pulsar "Continuar Evolucionando".
    /// Aplica la textura y la transicion visual de la era.
    /// </summary>
    public void AplicarEraVisual(int numeroEra)
    {
        int index = Mathf.Clamp(numeroEra - 1, 0, eras.Length - 1);
        _eraActual = index;

        Debug.Log($"[EraManager] AplicarEraVisual llamado. Era={numeroEra} index={index} transicion={transicion != null}");

        if (transicion != null && Application.isPlaying)
            transicion.Reproducir(index);
        else
            AplicarEraDesdeTransicion(index);
    }

    public void AvanzarEra()
    {
        if (_eraActual >= eras.Length - 1)
        {
            Debug.Log("[EraManager] Era final alcanzada.");
            return;
        }

        int siguiente = _eraActual + 1;
        _eraActual = siguiente;

        if (transicion != null && Application.isPlaying)
            transicion.Reproducir(siguiente);
        else
            AplicarEraDesdeTransicion(siguiente);
    }

    public void IrAEra(int numeroEra)
    {
        int index = Mathf.Clamp(numeroEra - 1, 0, eras.Length - 1);
        _eraActual = index;
        AplicarEraDesdeTransicion(index);
    }

    public void AplicarEraDesdeTransicion(int index)
    {
        if (planetaRenderer == null) return;

        EraData era = eras[index];
        if (era == null || era.texturaDia == null)
        {
            Debug.LogWarning($"[EraManager] Era {index + 1} sin textura de dia.");
            return;
        }

        Material matPlaneta = planetaRenderer.material;
        matPlaneta.SetTexture("_DayTex", era.texturaDia);
        matPlaneta.SetTexture("_NightTex", era.texturaNoche);

        if (atmosferaRenderer != null)
        {
            Material matAtmos = atmosferaRenderer.material;
            matAtmos.SetFloat("_Intensidad", era.atmosferaIntensidad);
            matAtmos.SetColor("_Color", era.atmosferaColor);
        }

        Debug.Log($"[EraManager] Era {index + 1}: {era.nombre}");
    }

    public void ConfigurarValoresPorDefecto()
    {
        if (eras == null || eras.Length != 8) eras = new EraData[8];
        for (int i = 0; i < 8; i++)
            if (eras[i] == null) eras[i] = new EraData();

        eras[0].nombre = "Roca Primordial";
        eras[0].atmosferaIntensidad = 0f;
        eras[0].atmosferaColor = new Color(0.4f, 0.6f, 1.0f, 1.0f);

        eras[1].nombre = "Primeros Oceanos";
        eras[1].atmosferaIntensidad = 0.005f;
        eras[1].atmosferaColor = new Color(0.6f, 0.349f, 0.149f, 1.0f);

        eras[2].nombre = "Vida Marina";
        eras[2].atmosferaIntensidad = 0.008f;
        eras[2].atmosferaColor = new Color(0.4f, 0.6f, 1.0f, 1.0f);

        eras[3].nombre = "Pangea";
        eras[3].atmosferaIntensidad = 0.008f;
        eras[3].atmosferaColor = new Color(0.4f, 0.6f, 1.0f, 1.0f);

        eras[4].nombre = "Jurasico";
        eras[4].atmosferaIntensidad = 0.009f;
        eras[4].atmosferaColor = new Color(0.35f, 0.6f, 1.0f, 1.0f);

        eras[5].nombre = "Civilizacion Primitiva";
        eras[5].atmosferaIntensidad = 0.01f;
        eras[5].atmosferaColor = new Color(0.4f, 0.6f, 1.0f, 1.0f);

        eras[6].nombre = "Civilizacion Avanzada";
        eras[6].atmosferaIntensidad = 0.01f;
        eras[6].atmosferaColor = new Color(0.45f, 0.58f, 0.9f, 1.0f);

        eras[7].nombre = "Era Espacial";
        eras[7].atmosferaIntensidad = 0.01f;
        eras[7].atmosferaColor = new Color(0.4f, 0.65f, 1.0f, 1.0f);
    }

#if UNITY_EDITOR
    [ContextMenu("TEST -> Era Anterior")]
    void TestAnterior() { if (_eraActual > 0) { _eraActual--; AplicarEraDesdeTransicion(_eraActual); } }

    [ContextMenu("TEST -> Era Siguiente")]
    void TestSiguiente() { if (_eraActual < eras.Length - 1) { _eraActual++; AplicarEraDesdeTransicion(_eraActual); } }

    [ContextMenu("TEST -> Aplicar valores por defecto")]
    void TestDefecto() { ConfigurarValoresPorDefecto(); }
#endif
}
