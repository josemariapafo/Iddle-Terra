using UnityEngine;

public class PlanetaInteraccion : MonoBehaviour
{
    [Header("Rotación automática")]
    public float velocidadAutoRotacion = 5f;

    [Header("Control táctil")]
    public float sensibilidadTouch = 0.3f;
    public float inercia = 0.95f;

    private Vector3 _velocidadRotacion;
    private bool _arrastrando = false;
    private Vector3 _posicionAnterior;

    void Update()
    {
        // ── PC: Mouse ─────────────────────────────────────────────
        if (Input.GetMouseButtonDown(0))
        {
            _arrastrando = true;
            _posicionAnterior = Input.mousePosition;
            _velocidadRotacion = Vector3.zero;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _arrastrando = false;
        }

        if (_arrastrando && Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - _posicionAnterior;
            _velocidadRotacion = new Vector3(delta.y, -delta.x, 0) * sensibilidadTouch;
            transform.Rotate(_velocidadRotacion, Space.World);
            _posicionAnterior = Input.mousePosition;
        }

        // ── Mobile: Touch ──────────────────────────────────────────
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _arrastrando = true;
                _velocidadRotacion = Vector3.zero;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 delta = new Vector3(
                    touch.deltaPosition.y,
                   -touch.deltaPosition.x,
                    0
                ) * sensibilidadTouch;

                _velocidadRotacion = delta;
                transform.Rotate(_velocidadRotacion, Space.World);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _arrastrando = false;
            }
        }

        // ── Inercia — el planeta sigue girando al soltar ───────────
        if (!_arrastrando)
        {
            if (_velocidadRotacion.magnitude > 0.001f)
            {
                transform.Rotate(_velocidadRotacion, Space.World);
                _velocidadRotacion *= inercia;
            }
            else
            {
                // Retomar rotación automática suavemente
                transform.Rotate(0, velocidadAutoRotacion * Time.deltaTime, 0, Space.World);
            }
        }
    }
}