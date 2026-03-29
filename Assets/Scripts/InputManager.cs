using UnityEngine;

/// <summary>
/// InputManager — gestor central de input.
/// Añadir al GameManager. No destruye nada.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public float umbralArrastre = 8f;

    public bool EsArrastre { get; private set; }
    public bool TapEsteFrame { get; private set; }
    public Vector2 PosicionToque { get; private set; }

    private Vector2 _posicionInicio;
    private bool _presionando;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    void Update()
    {
        TapEsteFrame = false;

        if (Input.GetMouseButtonDown(0))
        {
            _presionando = true;
            EsArrastre = false;
            _posicionInicio = Input.mousePosition;
            PosicionToque = Input.mousePosition;
        }

        if (_presionando && Input.GetMouseButton(0))
        {
            PosicionToque = Input.mousePosition;
            if (!EsArrastre && ((Vector2)Input.mousePosition - _posicionInicio).magnitude > umbralArrastre)
                EsArrastre = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!EsArrastre) TapEsteFrame = true;
            _presionando = false;
            EsArrastre = false;
        }

        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                _presionando = true;
                EsArrastre = false;
                _posicionInicio = t.position;
                PosicionToque = t.position;
            }
            if (t.phase == TouchPhase.Moved)
            {
                PosicionToque = t.position;
                if (!EsArrastre && (t.position - _posicionInicio).magnitude > umbralArrastre)
                    EsArrastre = true;
            }
            if (t.phase == TouchPhase.Ended)
            {
                if (!EsArrastre) TapEsteFrame = true;
                _presionando = false;
                EsArrastre = false;
            }
        }
    }
}