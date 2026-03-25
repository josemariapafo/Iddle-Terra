using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cerebro del sistema idle de TERRA.
/// Gestiona: Energía Vital, 4 pilares, mejoras, sinergias, prestige.
/// </summary>
public class SistemaIdle : MonoBehaviour
{
    public static SistemaIdle Instance { get; private set; }

    // ── ENUMS ─────────────────────────────────────────────────────────────

    public enum Pilar { Atmosfera, Oceanos, Tierra, Vida }

    // ── DATOS DE MEJORA ───────────────────────────────────────────────────

    [Serializable]
    public class DatosMejora
    {
        public string id;
        public string nombre;
        public string descripcion;
        public Pilar pilar;
        public int zonaIndex;          // 0,1,2 zonas por pilar
        public int nivelActual;
        public int nivelMaximo = 500;
        public double produccionBase;  // EV/s que genera en nivel 1
        public double costeBase;       // coste en nivel 1
        public float exponente = 1.15f;// coste se multiplica x1.15 por nivel

        // Requisitos para desbloquear
        public int eraRequerida = 1;
        public Pilar pilarRequisito;
        public int nivelRequisitoPilar = 0; // 0 = sin requisito

        // Calculados
        public double ProduccionActual => produccionBase * nivelActual;
        public double CosteProximoNivel => Math.Floor(costeBase * Math.Pow(exponente, nivelActual));
        public bool Desbloqueada => nivelRequisitoPilar == 0 ||
            SistemaIdle.Instance.NivelPilar(pilarRequisito) >= nivelRequisitoPilar;
    }

    // ── DATOS DE SINERGIA ─────────────────────────────────────────────────

    [Serializable]
    public class DatosSinergia
    {
        public string nombre;
        public Pilar pilarA;
        public Pilar pilarB;
        public int nivelRequeridoA;
        public int nivelRequeridoB;
        public float multiplicador;    // ej: 1.4 = +40%
        public bool activa;
    }

    // ── DATOS DE NODO EVOLUCIÓN ───────────────────────────────────────────

    [Serializable]
    public class DatosNodoEvolucion
    {
        public string id;
        public string nombre;
        public string descripcion;
        public int eraRequerida;
        public Pilar pilarRequerido;
        public int nivelPilarRequerido;
        public string idNodoPrevio;    // null si es raíz
        public int nivelActual;
        public int nivelMaximo = 10;
        public double costeBase;
        public float multiplicadorPorNivel = 0.1f; // +10% por nivel
        public Pilar pilarAfectado;
        public float bonusBase;        // multiplicador base al Lv1

        public bool Desbloqueado =>
            string.IsNullOrEmpty(idNodoPrevio) ||
            SistemaIdle.Instance.NodoCompletado(idNodoPrevio);

        public float MultiplicadorActual => 1f + (bonusBase - 1f) + (nivelActual * multiplicadorPorNivel);
        public double CosteProximoNivel => Math.Floor(costeBase * Math.Pow(1.2, nivelActual));
    }

    // ── ESTADO DEL JUEGO ──────────────────────────────────────────────────

    [Header("Estado actual")]
    public double energiaVital = 0;
    public double energiaVitalPorSegundo = 0;
    public int eraActual = 1;
    public int cicloPrestige = 0;      // cuántas veces se ha hecho prestige

    [Header("Multiplicadores de prestige (permanentes)")]
    public double multiplicadorFosiles = 1.0;   // Extinción Lv1
    public double multiplicadorGenes = 1.0;   // Glaciación Lv2
    public double multiplicadorQuarks = 1.0;   // Big Bang Lv3
    public int cantidadFosiles = 0;
    public int cantidadGenes = 0;
    public int cantidadQuarks = 0;

    [Header("Datos de mejoras")]
    public List<DatosMejora> mejoras = new List<DatosMejora>();

    [Header("Sinergias")]
    public List<DatosSinergia> sinergias = new List<DatosSinergia>();

    [Header("Árbol de evolución")]
    public List<DatosNodoEvolucion> nodosEvolucion = new List<DatosNodoEvolucion>();

    [Header("Debug — acelerar tiempo")]
    public float velocidadDebug = 1f;  // 1=normal, 60=un minuto por segundo

    // ── CONSTANTES ────────────────────────────────────────────────────────

    // Producción base por pilar (suma de sus mejoras × nivel)
    // La EV/s total = suma de todas las mejoras × sinergias × prestige

    // ── EVENTOS ───────────────────────────────────────────────────────────

    public static event Action<double> OnEnergiaChanged;
    public static event Action<double> OnProduccionChanged;
    public static event Action<DatosMejora> OnMejoraComprada;
    public static event Action<DatosSinergia> OnSinergiaActivada;
    public static event Action<int> OnEraCompletada;
    public static event Action OnPrestigeDisponible;

    // ── UNITY ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InicializarMejoras();
        InicializarSinergias();
        InicializarNodosEvolucion();
        RecalcularProduccion();
    }

    void Update()
    {
        float dt = Time.deltaTime * velocidadDebug;

        // Producir energía
        energiaVital += energiaVitalPorSegundo * dt;
        OnEnergiaChanged?.Invoke(energiaVital);

        // Comprobar sinergias
        ComprobarSinergias();

        // Comprobar prestige disponible
        ComprobarPrestigeDisponible();
    }

    // ── API PÚBLICA ───────────────────────────────────────────────────────

    /// <summary>Intenta comprar un nivel de una mejora. Devuelve true si éxito.</summary>
    public bool ComprarMejora(string idMejora)
    {
        DatosMejora mejora = mejoras.Find(m => m.id == idMejora);
        if (mejora == null) return false;
        if (!mejora.Desbloqueada) return false;
        if (mejora.nivelActual >= mejora.nivelMaximo) return false;

        double coste = mejora.CosteProximoNivel;
        if (energiaVital < coste) return false;

        energiaVital -= coste;
        mejora.nivelActual++;

        RecalcularProduccion();
        ComprobarSinergias();
        OnMejoraComprada?.Invoke(mejora);

        Debug.Log($"[Idle] {mejora.nombre} → Lv{mejora.nivelActual} | EV/s: {energiaVitalPorSegundo:F1}");
        return true;
    }

    /// <summary>Compra la máxima cantidad de niveles posible de una mejora.</summary>
    public bool ComprarMaxMejora(string idMejora)
    {
        DatosMejora mejora = mejoras.Find(m => m.id == idMejora);
        if (mejora == null || !mejora.Desbloqueada) return false;

        bool comprado = false;
        while (mejora.nivelActual < mejora.nivelMaximo &&
               energiaVital >= mejora.CosteProximoNivel)
        {
            energiaVital -= mejora.CosteProximoNivel;
            mejora.nivelActual++;
            comprado = true;
        }

        if (comprado)
        {
            RecalcularProduccion();
            ComprobarSinergias();
            OnMejoraComprada?.Invoke(mejora);
        }
        return comprado;
    }

    /// <summary>Intenta investigar (subir nivel) un nodo de evolución.</summary>
    public bool InvestigarNodo(string idNodo)
    {
        DatosNodoEvolucion nodo = nodosEvolucion.Find(n => n.id == idNodo);
        if (nodo == null || !nodo.Desbloqueado) return false;
        if (nodo.nivelActual >= nodo.nivelMaximo) return false;
        if (nodo.eraRequerida > eraActual) return false;
        if (NivelPilar(nodo.pilarRequerido) < nodo.nivelPilarRequerido) return false;

        double coste = nodo.CosteProximoNivel;
        if (energiaVital < coste) return false;

        energiaVital -= coste;
        nodo.nivelActual++;

        RecalcularProduccion();
        Debug.Log($"[Idle] Nodo {nodo.nombre} → Lv{nodo.nivelActual}");
        return true;
    }

    /// <summary>Devuelve el nivel total de un pilar (suma niveles de sus mejoras).</summary>
    public int NivelPilar(Pilar pilar)
    {
        int total = 0;
        foreach (var m in mejoras)
            if (m.pilar == pilar) total += m.nivelActual;
        return total;
    }

    /// <summary>Devuelve si un nodo está al menos en Lv1.</summary>
    public bool NodoCompletado(string idNodo)
    {
        DatosNodoEvolucion nodo = nodosEvolucion.Find(n => n.id == idNodo);
        return nodo != null && nodo.nivelActual > 0;
    }

    /// <summary>Devuelve el multiplicador total de sinergias activas.</summary>
    public float MultiplicadorSinergias()
    {
        float total = 1f;
        foreach (var s in sinergias)
            if (s.activa) total *= s.multiplicador;
        return total;
    }

    /// <summary>Devuelve el multiplicador total del árbol de evolución para un pilar.</summary>
    public float MultiplicadorArbol(Pilar pilar)
    {
        float total = 1f;
        foreach (var n in nodosEvolucion)
            if (n.pilarAfectado == pilar && n.nivelActual > 0)
                total *= n.MultiplicadorActual;
        return total;
    }

    // ── PRESTIGE ──────────────────────────────────────────────────────────

    /// <summary>Calcula cuántos fósiles ganaría el jugador si hace Extinción ahora.</summary>
    public int FosilesGanados()
    {
        // Basado en EV total producida en esta partida
        double evTotal = energiaVital + energiaVitalPorSegundo * 3600; // aproximación
        return Mathf.FloorToInt((float)Math.Pow(evTotal / 1e6, 0.4));
    }

    public double MultiplicadorFosilesNuevo(int fosiles)
    {
        return 1.0 + fosiles * 0.1; // cada fósil = +10%
    }

    /// <summary>Ejecuta Extinción (prestige Lv1) — reset eras 1-4, conserva árbol y eras 5+.</summary>
    public void EjecutarExtincion()
    {
        int fosiles = FosilesGanados();
        cantidadFosiles += fosiles;
        multiplicadorFosiles = MultiplicadorFosilesNuevo(cantidadFosiles);
        cicloPrestige++;

        // Reset mejoras de eras 1-4
        foreach (var m in mejoras)
            if (m.eraRequerida <= 4) m.nivelActual = 0;

        energiaVital = 0;
        if (eraActual > 4) eraActual = 5; // conservar progreso de eras altas
        else eraActual = 1;

        RecalcularProduccion();
        Debug.Log($"[Prestige] Extinción #{cicloPrestige} | Fósiles: {cantidadFosiles} | Mult: ×{multiplicadorFosiles:F2}");
    }

    // ── PRODUCCIÓN ────────────────────────────────────────────────────────

    void RecalcularProduccion()
    {
        double total = 0;

        float multSinergias = MultiplicadorSinergias();
        double multPrestige = multiplicadorFosiles * multiplicadorGenes * multiplicadorQuarks;

        foreach (var m in mejoras)
        {
            if (m.nivelActual == 0) continue;
            float multArbol = MultiplicadorArbol(m.pilar);
            total += m.ProduccionActual * multArbol;
        }

        energiaVitalPorSegundo = total * multSinergias * multPrestige;
        OnProduccionChanged?.Invoke(energiaVitalPorSegundo);
    }

    // ── SINERGIAS ─────────────────────────────────────────────────────────

    void ComprobarSinergias()
    {
        foreach (var s in sinergias)
        {
            bool estabaActiva = s.activa;
            s.activa = NivelPilar(s.pilarA) >= s.nivelRequeridoA &&
                       NivelPilar(s.pilarB) >= s.nivelRequeridoB;

            if (!estabaActiva && s.activa)
            {
                RecalcularProduccion();
                OnSinergiaActivada?.Invoke(s);
                Debug.Log($"[Sinergia] {s.nombre} activada × {s.multiplicador}");
            }
        }
    }

    // ── PRESTIGE DISPONIBLE ───────────────────────────────────────────────

    private float _tiempoUltimaMejora = 0;
    private float _mediaEntreUltimasMejoras = 60f;
    private float _tiempoSinMejora = 0;

    void ComprobarPrestigeDisponible()
    {
        _tiempoSinMejora += Time.deltaTime;

        // Si llevan 3× la media sin poder comprar nada → notificar
        if (_tiempoSinMejora > _mediaEntreUltimasMejoras * 3f && eraActual >= 5)
        {
            OnPrestigeDisponible?.Invoke();
            _tiempoSinMejora = 0; // no notificar cada frame
        }
    }

    public void RegistrarCompra()
    {
        float ahora = Time.time;
        float intervalo = ahora - _tiempoUltimaMejora;
        _mediaEntreUltimasMejoras = Mathf.Lerp(_mediaEntreUltimasMejoras, intervalo, 0.2f);
        _tiempoUltimaMejora = ahora;
        _tiempoSinMejora = 0;
    }

    // ── INICIALIZACIÓN ────────────────────────────────────────────────────

    void InicializarMejoras()
    {
        mejoras.Clear();

        // ── ATMÓSFERA — 7 mejoras (3 zonas) ──────────────────────────────

        // Zona 0: Ionosfera
        Agregar("atm_0_1", "Rayos Cósmicos", "Ionización primaria", Pilar.Atmosfera, 0, 1, 0.1, 1, 0);
        Agregar("atm_0_2", "Escudo Magnético", "Protege la atmósfera", Pilar.Atmosfera, 0, 1, 0.3, 1, 0);

        // Zona 1: Estratosfera
        Agregar("atm_1_1", "Ozono Primitivo", "Primera capa de ozono", Pilar.Atmosfera, 1, 2, 1.0, 2, 3);
        Agregar("atm_1_2", "Capa de Ozono", "Bloquea radiación UV", Pilar.Atmosfera, 1, 2, 3.0, 2, 5);

        // Zona 2: Troposfera
        Agregar("atm_2_1", "Nubes Primitivas", "Primeras nubes de agua", Pilar.Atmosfera, 2, 3, 8.0, 3, 8);
        Agregar("atm_2_2", "Ciclo del Nitrógeno", "N2 estabiliza el aire", Pilar.Atmosfera, 2, 3, 20.0, 3, 10);
        Agregar("atm_2_3", "Gases de Invernadero", "Mantienen el calor", Pilar.Atmosfera, 2, 4, 60.0, 4, 15);

        // ── OCÉANOS — 7 mejoras ───────────────────────────────────────────

        // Zona 0: Superficie
        Agregar("oce_0_1", "Mares Poco Profundos", "Aguas cálidas soleadas", Pilar.Oceanos, 0, 1, 0.2, 1, 0);
        Agregar("oce_0_2", "Corrientes Marinas", "Distribuyen calor", Pilar.Oceanos, 0, 1, 0.5, 1, 0);

        // Zona 1: Zona Media
        Agregar("oce_1_1", "Corrientes Oceánicas", "Mezclan nutrientes", Pilar.Oceanos, 1, 2, 2.0, 2, 4);
        Agregar("oce_1_2", "Termoclina", "Separa capas de agua", Pilar.Oceanos, 1, 2, 5.0, 2, 6);

        // Zona 2: Abismo
        Agregar("oce_2_1", "Fuentes Hidrotermales", "Calor del interior", Pilar.Oceanos, 2, 3, 15.0, 3, 9);
        Agregar("oce_2_2", "Fosas Oceánicas", "Subducción de placas", Pilar.Oceanos, 2, 3, 40.0, 3, 12);
        Agregar("oce_2_3", "Ciclo del Agua", "Evaporación y lluvia", Pilar.Oceanos, 2, 4, 100.0, 4, 18);

        // ── TIERRA — 7 mejoras ────────────────────────────────────────────

        // Zona 0: Superficie
        Agregar("tie_0_1", "Roca Basáltica", "Base del suelo", Pilar.Tierra, 0, 1, 0.2, 1, 0);
        Agregar("tie_0_2", "Erosión", "Crea suelo fértil", Pilar.Tierra, 0, 1, 0.5, 1, 0);

        // Zona 1: Corteza
        Agregar("tie_1_1", "Tectónica de Placas", "Mueve los continentes", Pilar.Tierra, 1, 2, 2.5, 2, 4);
        Agregar("tie_1_2", "Volcanes", "Liberan minerales", Pilar.Tierra, 1, 2, 6.0, 2, 6);

        // Zona 2: Interior
        Agregar("tie_2_1", "Núcleo de Hierro", "Campo magnético", Pilar.Tierra, 2, 3, 18.0, 3, 9);
        Agregar("tie_2_2", "Manto Convectivo", "Mueve las placas", Pilar.Tierra, 2, 3, 45.0, 3, 12);
        Agregar("tie_2_3", "Ciclo de las Rocas", "Renueva la corteza", Pilar.Tierra, 2, 4, 120.0, 4, 18);

        // ── VIDA — 7 mejoras ──────────────────────────────────────────────

        // Zona 0: Microbios
        Agregar("vid_0_1", "Bacteria Primordial", "El primer ser vivo", Pilar.Vida, 0, 1, 0.3, 1, 0);
        Agregar("vid_0_2", "ARN Primitivo", "Información genética", Pilar.Vida, 0, 1, 0.8, 1, 0);

        // Zona 1: Células
        Agregar("vid_1_1", "Células Eucariotas", "Núcleo diferenciado", Pilar.Vida, 1, 2, 3.0, 2, 5);
        Agregar("vid_1_2", "Reproducción Sexual", "Diversidad genética", Pilar.Vida, 1, 2, 8.0, 2, 7);

        // Zona 2: Organismos
        Agregar("vid_2_1", "Fotosíntesis", "Convierte luz en energía", Pilar.Vida, 2, 3, 20.0, 3, 10);
        Agregar("vid_2_2", "Multicelularidad", "Organismos complejos", Pilar.Vida, 2, 3, 55.0, 3, 13);
        Agregar("vid_2_3", "Sistema Nervioso", "Respuesta al entorno", Pilar.Vida, 2, 4, 150.0, 4, 20);
    }

    void Agregar(string id, string nombre, string desc, Pilar pilar, int zona,
                 int eraReq, double prodBase, double costeBase, int nivelReqPilar)
    {
        mejoras.Add(new DatosMejora
        {
            id = id,
            nombre = nombre,
            descripcion = desc,
            pilar = pilar,
            zonaIndex = zona,
            eraRequerida = eraReq,
            produccionBase = prodBase,
            costeBase = costeBase,
            nivelRequisitoPilar = nivelReqPilar,
            pilarRequisito = pilar
        });
    }

    void InicializarSinergias()
    {
        sinergias.Clear();
        sinergias.Add(new DatosSinergia { nombre = "Ciclo del Agua", pilarA = Pilar.Atmosfera, pilarB = Pilar.Oceanos, nivelRequeridoA = 5, nivelRequeridoB = 5, multiplicador = 1.4f });
        sinergias.Add(new DatosSinergia { nombre = "Ecosistema", pilarA = Pilar.Tierra, pilarB = Pilar.Vida, nivelRequeridoA = 5, nivelRequeridoB = 5, multiplicador = 1.6f });
        sinergias.Add(new DatosSinergia { nombre = "Fotosíntesis Marina", pilarA = Pilar.Oceanos, pilarB = Pilar.Vida, nivelRequeridoA = 8, nivelRequeridoB = 8, multiplicador = 1.8f });
        sinergias.Add(new DatosSinergia { nombre = "Clima Estable", pilarA = Pilar.Atmosfera, pilarB = Pilar.Tierra, nivelRequeridoA = 8, nivelRequeridoB = 8, multiplicador = 1.5f });
        sinergias.Add(new DatosSinergia { nombre = "Gaia", pilarA = Pilar.Atmosfera, pilarB = Pilar.Vida, nivelRequeridoA = 15, nivelRequeridoB = 15, multiplicador = 2.0f });
    }

    void InicializarNodosEvolucion()
    {
        nodosEvolucion.Clear();

        // Raíz
        NodoEvo("bac_primordial", "Bacteria Primordial", "El origen de todo",
                1, Pilar.Vida, 0, null, 5.0, Pilar.Vida, 1.3f);

        // Nivel 1
        NodoEvo("fotosintesis", "Fotosíntesis", "Luz → Energía",
                2, Pilar.Vida, 3, "bac_primordial", 50.0, Pilar.Oceanos, 1.3f);
        NodoEvo("respiracion", "Respiración", "O2 → Energía",
                2, Pilar.Atmosfera, 4, "bac_primordial", 50.0, Pilar.Atmosfera, 1.3f);

        // Nivel 2
        NodoEvo("algas", "Algas", "Primera vida compleja",
                2, Pilar.Oceanos, 5, "fotosintesis", 200.0, Pilar.Oceanos, 1.5f);
        NodoEvo("hongos", "Hongos", "Descomponedores",
                3, Pilar.Tierra, 6, "respiracion", 200.0, Pilar.Tierra, 1.5f);
        NodoEvo("plantas_vasc", "Plantas Vasculares", "Conquista la tierra",
                4, Pilar.Tierra, 12, "hongos", 800.0, Pilar.Tierra, 1.8f);
        NodoEvo("animales_simples", "Animales Simples", "Primeros animales",
                3, Pilar.Vida, 10, "algas", 600.0, Pilar.Vida, 1.6f);
    }

    void NodoEvo(string id, string nombre, string desc, int eraReq,
                 Pilar pilarReq, int nivelReq, string previo,
                 double coste, Pilar pilarAfectado, float bonus)
    {
        nodosEvolucion.Add(new DatosNodoEvolucion
        {
            id = id,
            nombre = nombre,
            descripcion = desc,
            eraRequerida = eraReq,
            pilarRequerido = pilarReq,
            nivelPilarRequerido = nivelReq,
            idNodoPrevio = previo,
            costeBase = coste,
            pilarAfectado = pilarAfectado,
            bonusBase = bonus
        });
    }
}
