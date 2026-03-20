using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public float velocidadRotacion = 10f;

    void Update()
    {
        transform.Rotate(0f, velocidadRotacion * Time.deltaTime, 0f);
    }
}