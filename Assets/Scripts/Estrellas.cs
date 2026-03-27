using UnityEngine;

public class Estrellas : MonoBehaviour
{
    public int cantidadEstrellas = 2000;
    public float radioEsfera = 50f;
    public float tamañoMinimo = 0.02f;
    public float tamañoMaximo = 0.06f;

    void Start()
    {
        GenerarEstrellas();
    }

    void GenerarEstrellas()
    {
        for (int i = 0; i < cantidadEstrellas; i++)
        {
            // Posición aleatoria en una esfera
            Vector3 posicion = Random.onUnitSphere * radioEsfera;

            // Crear objeto estrella
            GameObject estrella = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            estrella.transform.position = posicion;
            estrella.transform.parent = transform;

            // Tamaño aleatorio
            float tamaño = Random.Range(tamañoMinimo, tamañoMaximo);
            estrella.transform.localScale = Vector3.one * tamaño;

            // Quitar el collider — no lo necesitamos
            Destroy(estrella.GetComponent<Collider>());

            // Material blanco brillante
            Renderer r = estrella.GetComponent<Renderer>();
            r.material = new Material(Shader.Find("Legacy Shaders/Self-Illumin/Diffuse"));

            // Color blanco con brillo aleatorio
            float brillo = Random.Range(0.6f, 1.0f);
            r.material.color = new Color(brillo, brillo, brillo, 1f);
        }
    }
}