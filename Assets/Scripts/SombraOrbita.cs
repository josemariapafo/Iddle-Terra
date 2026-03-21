using UnityEngine;

public class SombraOrbita : MonoBehaviour
{
    public Renderer planetaRenderer;
    public float velocidad = 0.01f; // 1 = vuelta completa por segundo

    private float _angulo = 0f;

    void Update()
    {
        _angulo += velocidad * Time.deltaTime;
        if (_angulo > 1f) _angulo -= 1f;

        if (planetaRenderer != null)
        {
            planetaRenderer.material.SetFloat("_ShadowAngle", _angulo);
        }
    }
}