using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Terra.Controllers;
using Terra.Core;

/// <summary>
/// MeteoroManager — gestiona el spawning de meteoritos y las rondas.
/// 
/// SETUP en Unity:
/// 1. Añadir este script a un GameObject vacio llamado "MeteoroManager"
/// 2. Asignar referencias en el Inspector
/// 3. Los meteoritos se crean como primitivas — no necesitas assets
/// </summary>
public class MeteoroManager : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────

    [Header("Configuracion meteoritos")]
    public float radioSpawn      = 4.5f;   // distancia desde el planeta al spawn
    public float radioPlaneta    = 1.1f;   // radio del planeta (para detectar impacto)
    public float velocidadBase   = 1.8f;   // velocidad de vuelo
    public float tamanoMeteoro   = 0.12f;  // tamano de la esfera placeholder

    [Header("Meteorito individual (cada X minutos)")]
    public float tiempoMinIndividual = 180f;   // 3 minutos
    public float tiempoMaxIndividual = 480f;   // 8 minutos

    [Header("Ronda de meteoritos (cada X minutos)")]
    public float tiempoMinRonda  = 900f;   // 15 minutos
    public float tiempoMaxRonda  = 1500f;  // 25 minutos
    public int   meteorosPorRonda = 12;    // cuantos meteoritos en una ronda
    public float intervaloEntreMeteorosRonda = 0.8f;  // segundos entre cada uno

    [Header("Bonuses")]
    public float duracionBonusCompleto   = 30f;   // segundos bonus ronda perfecta
    public float duracionBonusParcial    = 15f;   // segundos bonus casi perfecta
    public float multiplicadorCompleto   = 1.5f;  // x1.5 ronda perfecta
    public float multiplicadorParcial    = 1.1f;  // x1.1 casi perfecta
    public int   maxImpactosParaParcial  = 2;     // impactos permitidos para bonus parcial

    [Header("UI")]
    public GameObject Panel_Meteoro;            // panel de aviso de ronda
    public TextMeshProUGUI Text_AvisoMeteoro;   // "LLUVIA DE METEORITOS"
    public TextMeshProUGUI Text_ContadorMeteoros; // "8 / 12 eliminados"
    public TextMeshProUGUI Text_BonusActivo;    // "x1.5 activo 28s"
    public GameObject Panel_ResultadoRonda;     // panel resultado
    public TextMeshProUGUI Text_ResultadoRonda;

    [Header("Era minima para meteoritos")]
    public int eraMinima = 1;

    // ── Estado interno ────────────────────────────────────────────────────

    private float _timerIndividual;
    private float _timerRonda;
    private bool  _rondaActiva   = false;

    private int   _meteorosRondaTotal;
    private int   _meteorosEliminados;
    private int   _meteorosImpactados;

    private readonly List<Meteoro> _meteorosActivos = new List<Meteoro>();

    private float _tiempoRestanteBonus = 0f;
    private float _multiplicadorActual = 1f;

    // ══════════════════════════════════════════════════════════════════════
    void Start()
    {
        ResetearTimerIndividual();
        ResetearTimerRonda();

        if (Panel_Meteoro != null)        Panel_Meteoro.SetActive(false);
        if (Panel_ResultadoRonda != null) Panel_ResultadoRonda.SetActive(false);
        if (Text_BonusActivo != null)     Text_BonusActivo.gameObject.SetActive(false);
    }

    void Update()
    {
        int era = GameController.Instance?.Estado.EraActual ?? 1;
        if (era < eraMinima) return;

        // Actualizar bonus activo
        if (_tiempoRestanteBonus > 0)
        {
            _tiempoRestanteBonus -= Time.deltaTime;
            ActualizarUIBonus();

            if (_tiempoRestanteBonus <= 0)
                TerminarBonus();
        }

        if (_rondaActiva) return;

        // Timer meteorito individual
        _timerIndividual -= Time.deltaTime;
        if (_timerIndividual <= 0)
        {
            SpawnMeteoroIndividual();
            ResetearTimerIndividual();
        }

        // Timer ronda
        _timerRonda -= Time.deltaTime;
        if (_timerRonda <= 0)
        {
            StartCoroutine(IniciarRonda());
            ResetearTimerRonda();
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // METEORITO INDIVIDUAL
    // ══════════════════════════════════════════════════════════════════════

    void SpawnMeteoroIndividual()
    {
        SpawnMeteoro(velocidadBase * 1.2f, tamanoMeteoro * 1.4f);
    }

    // ══════════════════════════════════════════════════════════════════════
    // RONDA DE METEORITOS
    // ══════════════════════════════════════════════════════════════════════

    IEnumerator IniciarRonda()
    {
        _rondaActiva       = true;
        _meteorosRondaTotal = meteorosPorRonda;
        _meteorosEliminados = 0;
        _meteorosImpactados = 0;

        // Aviso al jugador
        if (Panel_Meteoro != null)
        {
            Panel_Meteoro.SetActive(true);
            if (Text_AvisoMeteoro != null)
                Text_AvisoMeteoro.text = "LLUVIA DE METEORITOS";
            ActualizarContador();
        }

        // Spawn de meteoritos con intervalo
        for (int i = 0; i < meteorosPorRonda; i++)
        {
            SpawnMeteoro(velocidadBase, tamanoMeteoro);
            yield return new WaitForSeconds(intervaloEntreMeteorosRonda);
        }

        // Esperar a que terminen todos
        yield return new WaitUntil(() => _meteorosActivos.Count == 0);

        TerminarRonda();
    }

    void TerminarRonda()
    {
        _rondaActiva = false;

        if (Panel_Meteoro != null)
            Panel_Meteoro.SetActive(false);

        // Calcular resultado
        string resultado;
        float multiplicador = 1f;
        float duracion      = 0f;

        if (_meteorosImpactados == 0)
        {
            // Ronda perfecta
            multiplicador = multiplicadorCompleto;
            duracion      = duracionBonusCompleto;
            resultado     = "PERFECTO\nTodos eliminados\nx" + multiplicador + " durante " + duracion + "s";
        }
        else if (_meteorosImpactados <= maxImpactosParaParcial)
        {
            // Casi perfecto
            multiplicador = multiplicadorParcial;
            duracion      = duracionBonusParcial;
            resultado     = "CASI PERFECTO\n" + _meteorosImpactados + " impactos\nx" + multiplicador + " durante " + duracion + "s";
        }
        else
        {
            // Sin bonus
            resultado = "FALLIDO\n" + _meteorosImpactados + " impactos\nSin bonus";
        }

        // Mostrar resultado
        if (Panel_ResultadoRonda != null)
        {
            Panel_ResultadoRonda.SetActive(true);
            if (Text_ResultadoRonda != null)
                Text_ResultadoRonda.text = resultado;

            StartCoroutine(OcultarResultado(2.5f));
        }

        // Aplicar bonus si corresponde
        if (duracion > 0)
            AplicarBonus(multiplicador, duracion);
    }

    // ══════════════════════════════════════════════════════════════════════
    // SPAWN METEORITO
    // ══════════════════════════════════════════════════════════════════════

    void SpawnMeteoro(float velocidad, float tamano)
    {
        // Posicion aleatoria en esfera alrededor del planeta
        Vector3 direccionRandom = Random.onUnitSphere;
        Vector3 posicion = direccionRandom * radioSpawn;

        // Crear esfera primitiva
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = "Meteoro";
        obj.transform.position = posicion;
        obj.transform.localScale = Vector3.one * tamano;

        // Quitar collider estandar, añadir SphereCollider ajustado
        Destroy(obj.GetComponent<Collider>());
        var col = obj.AddComponent<SphereCollider>();
        col.radius = 1f;

        // Añadir script
        var meteoro = obj.AddComponent<Meteoro>();
        meteoro.velocidad    = velocidad;
        meteoro.radioPlaneta = radioPlaneta;
        meteoro.manager      = this;

        _meteorosActivos.Add(meteoro);
    }

    // ══════════════════════════════════════════════════════════════════════
    // CALLBACKS DE METEORITO
    // ══════════════════════════════════════════════════════════════════════

    public void OnMeteoroEliminado(Meteoro m)
    {
        _meteorosActivos.Remove(m);
        if (_rondaActiva)
        {
            _meteorosEliminados++;
            ActualizarContador();
        }

        // Dar EV extra por eliminar meteorito individual
        var gc = GameController.Instance;
        if (gc != null)
        {
            double evBonus = gc.Estado.EVPorSegundo * 2.0;  // 2 segundos de produccion
            gc.Estado.EnergiaVital += evBonus;
        }
    }

    public void OnMeteoroImpacto(Meteoro m)
    {
        _meteorosActivos.Remove(m);
        if (_rondaActiva)
        {
            _meteorosImpactados++;
            ActualizarContador();
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // TAP DETECTION (llamado desde TapManager)
    // ══════════════════════════════════════════════════════════════════════

    public bool IntentarGolpearMeteoro(Ray rayo)
    {
        if (_meteorosActivos.Count == 0) return false;

        RaycastHit hit;
        if (Physics.Raycast(rayo, out hit, 20f))
        {
            var meteoro = hit.collider.GetComponent<Meteoro>();
            if (meteoro != null && !meteoro.Eliminado && !meteoro.Impactado)
            {
                meteoro.SerGolpeado();
                return true;
            }
        }

        return false;
    }

    // ══════════════════════════════════════════════════════════════════════
    // BONUS
    // ══════════════════════════════════════════════════════════════════════

    void AplicarBonus(float multiplicador, float duracion)
    {
        _multiplicadorActual  = multiplicador;
        _tiempoRestanteBonus  = duracion;

        GameController.Instance?.AplicarEventoTemporal(multiplicador, duracion);

        if (Text_BonusActivo != null)
            Text_BonusActivo.gameObject.SetActive(true);
    }

    void TerminarBonus()
    {
        _multiplicadorActual = 1f;
        if (Text_BonusActivo != null)
            Text_BonusActivo.gameObject.SetActive(false);
    }

    void ActualizarUIBonus()
    {
        if (Text_BonusActivo == null) return;
        Text_BonusActivo.text = "x" + _multiplicadorActual.ToString("F1")
            + " activo " + Mathf.CeilToInt(_tiempoRestanteBonus) + "s";
    }

    void ActualizarContador()
    {
        if (Text_ContadorMeteoros == null) return;
        int restantes = _meteorosRondaTotal - _meteorosEliminados - _meteorosImpactados;
        Text_ContadorMeteoros.text = _meteorosEliminados + " / " + _meteorosRondaTotal
            + " eliminados (" + restantes + " en vuelo)";
    }

    IEnumerator OcultarResultado(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Panel_ResultadoRonda != null)
            Panel_ResultadoRonda.SetActive(false);
    }

    // ══════════════════════════════════════════════════════════════════════
    // UTILIDADES
    // ══════════════════════════════════════════════════════════════════════

    void ResetearTimerIndividual() =>
        _timerIndividual = Random.Range(tiempoMinIndividual, tiempoMaxIndividual);

    void ResetearTimerRonda() =>
        _timerRonda = Random.Range(tiempoMinRonda, tiempoMaxRonda);

    // Para el GameController — expone AplicarEventoTemporal
    // Añadimos este metodo al GameController para que MeteoroManager pueda llamarlo
}
