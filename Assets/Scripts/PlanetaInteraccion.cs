using UnityEngine;

/// <summary>
/// PlanetaInteraccion — rota el planeta al arrastrar.
/// Usa InputManager si existe, funciona solo si no.
/// </summary>
public class PlanetaInteraccion : MonoBehaviour
{
    [Header("Rotacion automatica")]
    public float velocidadAutoRotacion = 5f;

    [Header("Control tactil")]
    public float sensibilidadTouch = 0.3f;
    public float inercia = 0.95f;

    private Vector3 _velocidadRotacion;
    private bool _arrastrando = false;
    private Vector3 _posicionAnterior;

    void Update()
    {
        bool esArrastre = InputManager.Instance != null
            ? InputManager.Instance.EsArrastre
            : true; // si no hay InputManager, todo se trata como arrastre

        // ── Mouse ─────────────────────────────────────────────────────────
        if (Input.GetMouseButtonDown(0))
        {
            _arrastrando = true;
            _posicionAnterior = Input.mousePosition;
            _velocidadRotacion = Vector3.zero;
        }
        if (Input.GetMouseButtonUp(0))
            _arrastrando = false;

        if (_arrastrando && Input.GetMouseButton(0) && esArrastre)
        {
            Vector3 delta = Input.mousePosition - _posicionAnterior;
            _velocidadRotacion = new Vector3(delta.y, -delta.x, 0) * sensibilidadTouch;
            transform.Rotate(_velocidadRotacion, Space.World);
            _posicionAnterior = Input.mousePosition;
        }

        // ── Touch ─────────────────────────────────────────────────────────
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                _arrastrando = true;
                _velocidadRotacion = Vector3.zero;
            }
            if (t.phase == TouchPhase.Moved && esArrastre)
            {
                Vector3 delta = new Vector3(t.deltaPosition.y, -t.deltaPosition.x, 0) * sensibilidadTouch;
                _velocidadRotacion = delta;
                transform.Rotate(_velocidadRotacion, Space.World);
            }
            if (t.phase == TouchPhase.Ended)
                _arrastrando = false;
        }

        // ── Inercia ────────────────────────────────────────────────────────
        if (!_arrastrando)
        {
            if (_velocidadRotacion.magnitude > 0.001f)
            {
                transform.Rotate(_velocidadRotacion, Space.World);
                _velocidadRotacion *= inercia;
            }
            else
            {
                transform.Rotate(0, velocidadAutoRotacion * Time.deltaTime, 0, Space.World);
            }
        }
    }
}
