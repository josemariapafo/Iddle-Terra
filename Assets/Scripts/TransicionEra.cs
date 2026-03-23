using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona la animación de transición entre eras:
/// Flash blanco → Zoom in → Swap textura → Zoom out
/// 
/// SETUP en Unity:
/// 1. Añadir este script al GameManager (mismo objeto que EraManager)
/// 2. Crear Canvas → Image (panel negro full-screen) → asignar a flashPanel
/// 3. EraManager llamará a TransicionEra.Reproducir() automáticamente
/// </summary>
public class TransicionEra : MonoBehaviour
{
    // ── Referencias ───────────────────────────────────────────────────────

    [Header("Referencias")]
    public EraManager eraManager;
    public Camera camaraPrincipal;

    [Header("UI Flash")]
    public Image flashPanel;          // Image full-screen en un Canvas overlay

    // ── Parámetros de animación ───────────────────────────────────────────

    [Header("Tiempos (segundos)")]
    public float duracionFlashEntrada = 0.3f;   // Negro → Blanco
    public float duracionFlashSalida = 0.5f;   // Blanco → Transparente
    public float duracionZoomIn = 0.6f;   // Cámara se acerca
    public float duracionZoomOut = 0.8f;   // Cámara vuelve

    [Header("Zoom")]
    public float zoomInZ = -2.8f;  // Posición Z al acercarse
    private float _zoomOriginalZ = -4.55f; // Se lee automáticamente en Start
    private float _fovOriginal = 60f;

    [Header("Bloqueo de input")]
    public PlanetaInteraccion planetaInteraccion; // Se desactiva durante transición

    // ── Estado ────────────────────────────────────────────────────────────

    private bool _enTransicion = false;
    public bool EnTransicion => _enTransicion;

    // ── Unity ─────────────────────────────────────────────────────────────

    void Start()
    {
        if (camaraPrincipal == null)
            camaraPrincipal = Camera.main;

        _zoomOriginalZ = camaraPrincipal.transform.localPosition.z;
        _fovOriginal = camaraPrincipal.fieldOfView;

        // Flash panel empieza invisible
        if (flashPanel != null)
            flashPanel.color = new Color(1, 1, 1, 0);
    }

    // ── API pública ───────────────────────────────────────────────────────

    /// <summary>
    /// Llama esto para iniciar la transición hacia la era siguiente.
    /// EraManager lo llama internamente al avanzar de era.
    /// </summary>
    public void Reproducir(int eraIndexDestino)
    {
        if (_enTransicion) return;
        StartCoroutine(SecuenciaTransicion(eraIndexDestino));
    }

    // ── Secuencia principal ───────────────────────────────────────────────

    IEnumerator SecuenciaTransicion(int eraIndexDestino)
    {
        _enTransicion = true;

        // Desactivar interacción con el planeta
        if (planetaInteraccion != null)
            planetaInteraccion.enabled = false;

        // ── 1. FLASH BLANCO ───────────────────────────────────────────────
        yield return StartCoroutine(FadeFlash(0f, 1f, duracionFlashEntrada));

        // ── 2. ZOOM IN (durante el flash, la cámara ya se mueve) ─────────
        yield return StartCoroutine(ZoomCamara(
            camaraPrincipal.transform.localPosition.z,
            zoomInZ,
            duracionZoomIn
        ));

        // ── 3. SWAP DE TEXTURA (invisible bajo el flash) ──────────────────
        eraManager.AplicarEraDesdeTransicion(eraIndexDestino);

        // Pausa mínima para que Unity procese el swap
        yield return null;

        // ── 4. FLASH DESAPARECE ───────────────────────────────────────────
        yield return StartCoroutine(FadeFlash(1f, 0f, duracionFlashSalida));

        // ── 5. ZOOM OUT ───────────────────────────────────────────────────
        yield return StartCoroutine(ZoomCamara(
            camaraPrincipal.transform.localPosition.z,
            _zoomOriginalZ,
            duracionZoomOut
        ));

        // Reactivar interacción
        if (planetaInteraccion != null)
            planetaInteraccion.enabled = true;

        _enTransicion = false;
    }

    // ── Coroutines de animación ───────────────────────────────────────────

    IEnumerator FadeFlash(float desde, float hasta, float duracion)
    {
        if (flashPanel == null) yield break;

        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(desde, hasta, t / duracion);
            flashPanel.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        flashPanel.color = new Color(1f, 1f, 1f, hasta);
    }

    IEnumerator ZoomCamara(float desdeZ, float hastaZ, float duracion)
    {
        float t = 0f;
        Vector3 posicion = camaraPrincipal.transform.localPosition;

        while (t < duracion)
        {
            t += Time.deltaTime;
            float progreso = EasInOut(t / duracion);
            posicion.z = Mathf.Lerp(desdeZ, hastaZ, progreso);
            camaraPrincipal.transform.localPosition = posicion;
            yield return null;
        }

        posicion.z = hastaZ;
        camaraPrincipal.transform.localPosition = posicion;
    }

    // Curva de suavizado — acelera al inicio, frena al final
    float EasInOut(float t)
    {
        return t * t * (3f - 2f * t);
    }

    // ── Testing en Editor ─────────────────────────────────────────────────

#if UNITY_EDITOR
    [ContextMenu("TEST → Disparar transición a era siguiente")]
    void TestTransicion()
    {
        if (eraManager == null) { Debug.LogError("Asigna EraManager"); return; }
        eraManager.AvanzarEra();
    }
#endif
}
