using UnityEngine;

/// <summary>
/// PlanetaInteraccion — rota el planeta al arrastrar.
/// TapManager tiene su propio umbral de arrastre — no hay conflicto.
/// </summary>
public class PlanetaInteraccion : MonoBehaviour
{
    [Header("Rotacion automatica")]
    public float velocidadAutoRotacion = 5f;

    [Header("Control tactil")]
    public float sensibilidadTouch = 0.3f;
    public float inercia = 0.95f;
    public float umbralArrastre = 12f;

    private Vector3 _velocidadRotacion;
    private bool _arrastrando = false;
    private bool _esArrastre = false;
    private Vector3 _posicionAnterior;
    private Vector2 _posicionInicio;

    void Update()
    {
        // ── Mouse ─────────────────────────────────────────────────────────
        if (Input.GetMouseButtonDown(0))
        {
            _arrastrando = true;
            _esArrastre = false;
            _posicionAnterior = Input.mousePosition;
            _posicionInicio = Input.mousePosition;
            _velocidadRotacion = Vector3.zero;
        }

        if (Input.GetMouseButtonUp(0))
            _arrastrando = false;

        if (_arrastrando && Input.GetMouseButton(0))
        {
            if (!_esArrastre)
            {
                float dist = Vector2.Distance(Input.mousePosition, _posicionInicio);
                if (dist > umbralArrastre) _esArrastre = true;
            }

            if (_esArrastre)
            {
                Vector3 delta = Input.mousePosition - _posicionAnterior;
                _velocidadRotacion = new Vector3(delta.y, -delta.x, 0) * sensibilidadTouch;
                transform.Rotate(_velocidadRotacion, Space.World);
            }

            _posicionAnterior = Input.mousePosition;
        }

        // ── Touch ─────────────────────────────────────────────────────────
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                _arrastrando = true;
                _esArrastre = false;
                _posicionInicio = t.position;
                _velocidadRotacion = Vector3.zero;
            }

            if (t.phase == TouchPhase.Moved)
            {
                if (!_esArrastre)
                {
                    float dist = Vector2.Distance(t.position, _posicionInicio);
                    if (dist > umbralArrastre) _esArrastre = true;
                }

                if (_esArrastre)
                {
                    Vector3 delta = new Vector3(t.deltaPosition.y, -t.deltaPosition.x, 0) * sensibilidadTouch;
                    _velocidadRotacion = delta;
                    transform.Rotate(_velocidadRotacion, Space.World);
                }
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
