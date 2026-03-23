using UnityEngine;

/// <summary>
/// Gestiona las 8 eras geológicas de TERRA.
/// Controla texturas del planeta y atmósfera por era.
/// </summary>
public class EraManager : MonoBehaviour
{
    // ── Datos de cada era ─────────────────────────────────────────────────

    [System.Serializable]
    public class EraData
    {
        public string nombre;
        [TextArea(1, 2)]
        public string descripcion;

        [Header("Texturas")]
        public Texture2D texturaDia;
        public Texture2D texturaNoche;   // null para eras 1-5

        [Header("Atmósfera")]
        [Range(0f, 3f)]
        public float atmosferaIntensidad = 1.5f;
        public Color atmosferaColor = new Color(0.4f, 0.6f, 1.0f, 1.0f);
    }

    // ── Inspector ─────────────────────────────────────────────────────────

    [Header("Eras (arrastra las texturas en orden Era1 → Era8)")]
    public EraData[] eras = new EraData[8];

    [Header("Referencias")]
    public Renderer planetaRenderer;
    public Renderer atmosferaRenderer;

    [Header("Estado actual (solo lectura)")]
    [SerializeField] private int _eraActual = 0;

    // ── Propiedad pública ─────────────────────────────────────────────────

    public int EraActual => _eraActual + 1;

    // ── Unity ─────────────────────────────────────────────────────────────

    void Reset()
    {
        ConfigurarValoresPorDefecto();
    }

    void Start()
    {
        AplicarEra(_eraActual, animado: false);
    }

    // ── API pública ───────────────────────────────────────────────────────

    public void AvanzarEra()
    {
        if (_eraActual >= eras.Length - 1)
        {
            Debug.Log("[EraManager] Ya estamos en la Era final.");
            return;
        }
        _eraActual++;
        AplicarEra(_eraActual, animado: true);
    }

    public void IrAEra(int numeroEra)
    {
        int index = Mathf.Clamp(numeroEra - 1, 0, eras.Length - 1);
        _eraActual = index;
        AplicarEra(_eraActual, animado: false);
    }

    // ── Lógica interna ────────────────────────────────────────────────────

    void AplicarEra(int index, bool animado)
    {
        if (planetaRenderer == null)
        {
            Debug.LogError("[EraManager] No hay Renderer del planeta asignado.");
            return;
        }

        EraData era = eras[index];

        if (era.texturaDia == null)
        {
            Debug.LogWarning($"[EraManager] Era {index + 1} no tiene textura de día asignada.");
            return;
        }

        // ── Planeta ───────────────────────────────────────────────────────
        Material matPlaneta = planetaRenderer.material;
        matPlaneta.SetTexture("_DayTex", era.texturaDia);
        matPlaneta.SetTexture("_NightTex", era.texturaNoche);

        // ── Atmósfera ─────────────────────────────────────────────────────
        if (atmosferaRenderer != null)
        {
            Material matAtmos = atmosferaRenderer.material;
            matAtmos.SetFloat("_Intensidad", era.atmosferaIntensidad);
            matAtmos.SetColor("_Color", era.atmosferaColor);
        }

        Debug.Log($"[EraManager] Era {index + 1} aplicada: {era.nombre} | Atmósfera: {era.atmosferaIntensidad}");

        // TODO: llamar al sistema de transición
        // if (animado) TransicionManager.Instance.ReproducirTransicion(index);
    }

    // ── Valores por defecto científicamente correctos ─────────────────────

    public void ConfigurarValoresPorDefecto()
    {
        if (eras == null || eras.Length != 8) eras = new EraData[8];
        for (int i = 0; i < 8; i++)
            if (eras[i] == null) eras[i] = new EraData();

        // Era 1 — Roca Primordial — sin atmósfera
        eras[0].nombre = "Roca Primordial";
        eras[0].atmosferaIntensidad = 0f;
        eras[0].atmosferaColor = new Color(0.4f, 0.6f, 1.0f, 1.0f);

        // Era 2 — Primeros Océanos — atmósfera tóxica, naranja tenue
        eras[1].nombre = "Primeros Océanos";
        eras[1].atmosferaIntensidad = 0.15f;
        eras[1].atmosferaColor = new Color(0.8f, 0.5f, 0.2f, 1.0f);

        // Era 3 — Vida Marina — CO2 + O2 formándose, azul-naranja
        eras[2].nombre = "Vida Marina";
        eras[2].atmosferaIntensidad = 0.4f;
        eras[2].atmosferaColor = new Color(0.5f, 0.55f, 0.9f, 1.0f);

        // Era 4 — Pangea — atmósfera establecida
        eras[3].nombre = "Pangea";
        eras[3].atmosferaIntensidad = 0.8f;
        eras[3].atmosferaColor = new Color(0.4f, 0.6f, 1.0f, 1.0f);

        // Era 5 — Jurásico — O2 rico, azul intenso
        eras[4].nombre = "Jurásico";
        eras[4].atmosferaIntensidad = 1.2f;
        eras[4].atmosferaColor = new Color(0.35f, 0.6f, 1.0f, 1.0f);

        // Era 6 — Civilización Primitiva — moderna completa
        eras[5].nombre = "Civilización Primitiva";
        eras[5].atmosferaIntensidad = 1.5f;
        eras[5].atmosferaColor = new Color(0.4f, 0.6f, 1.0f, 1.0f);

        // Era 7 — Civilización Avanzada — ligero haze contaminación
        eras[6].nombre = "Civilización Avanzada";
        eras[6].atmosferaIntensidad = 1.5f;
        eras[6].atmosferaColor = new Color(0.45f, 0.58f, 0.9f, 1.0f);

        // Era 8 — Era Espacial — atmósfera limpia y fina tecnológica
        eras[7].nombre = "Era Espacial";
        eras[7].atmosferaIntensidad = 1.3f;
        eras[7].atmosferaColor = new Color(0.4f, 0.65f, 1.0f, 1.0f);
    }

    // ── Testing en Editor ─────────────────────────────────────────────────

#if UNITY_EDITOR
    [ContextMenu("TEST → Era Anterior")]
    void TestEraAnterior()
    {
        if (_eraActual > 0) { _eraActual--; AplicarEra(_eraActual, false); }
    }

    [ContextMenu("TEST → Era Siguiente")]
    void TestEraSiguiente()
    {
        if (_eraActual < eras.Length - 1) { _eraActual++; AplicarEra(_eraActual, false); }
    }

    [ContextMenu("TEST → Aplicar valores por defecto de atmósfera")]
    void TestValoresPorDefecto()
    {
        ConfigurarValoresPorDefecto();
    }
#endif
}
