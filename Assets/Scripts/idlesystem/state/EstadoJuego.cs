using System;
using System.Collections.Generic;
using Terra.Core;

namespace Terra.State
{
    // ── Estado de una mejora en runtime ──────────────────────────────────
    [Serializable]
    public class EstadoMejora
    {
        public string Id;
        public int Nivel;
        public bool Desbloqueada;

        public EstadoMejora(string id, bool desbloqueadaInicial = false)
        {
            Id = id;
            Nivel = 0;
            Desbloqueada = desbloqueadaInicial;
        }
    }

    // ── Estado de una sinergia en runtime ────────────────────────────────
    [Serializable]
    public class EstadoSinergia
    {
        public string Id;
        public bool Activa;
        public int VecesActivada;

        public EstadoSinergia(string id)
        {
            Id = id;
            Activa = false;
            VecesActivada = 0;
        }
    }

    // ── Estado de un nodo del árbol en runtime ───────────────────────────
    [Serializable]
    public class EstadoNodo
    {
        public string Id;
        public int Nivel;
        public bool Desbloqueado;

        public EstadoNodo(string id)
        {
            Id = id;
            Nivel = 0;
            Desbloqueado = false;
        }
    }

    // ── Estado del prestige ───────────────────────────────────────────────
    [Serializable]
    public class EstadoPrestige
    {
        public double Fosiles;
        public double Genes;
        public double Quarks;
        public int VecesExtincion;
        public int VecesGlaciacion;
        public int VecesBigBang;

        public double MultiplicadorFosiles  => 1.0 + Fosiles  * 0.1;
        public double MultiplicadorGenes    => 1.0 + Genes    * 0.05;
        public double MultiplicadorQuarks   => 1.0 + Quarks   * 2.0;
        public double MultiplicadorTotal    =>
            MultiplicadorFosiles * MultiplicadorGenes * MultiplicadorQuarks;

        public int VecesTotales => VecesExtincion + VecesGlaciacion + VecesBigBang;
    }

    // ── Estado de un recurso secundario ──────────────────────────────────
    [Serializable]
    public class EstadoRecurso
    {
        public string Id;
        public double Cantidad;
        public bool Desbloqueado;

        public EstadoRecurso(string id) { Id = id; Cantidad = 0; Desbloqueado = false; }
    }

    // ── Estado de la racha diaria ─────────────────────────────────────────
    [Serializable]
    public class EstadoRacha
    {
        public int DiasConsecutivos;
        public DateTime UltimaConexion;
        public bool BonusDiarioReclamado;

        public bool EsConexionNueva(DateTime ahora) =>
            (ahora.Date - UltimaConexion.Date).Days >= 1;

        public bool RachaRota(DateTime ahora) =>
            (ahora.Date - UltimaConexion.Date).Days > 1;

        public double MultiplicadorRacha =>
            1.0 + Math.Min(DiasConsecutivos, 30) * 0.05;  // max +150% a los 30 días
    }

    // ── Estado de un logro ────────────────────────────────────────────────
    [Serializable]
    public class EstadoLogro
    {
        public string Id;
        public bool Completado;
        public DateTime FechaCompletado;

        public EstadoLogro(string id) { Id = id; Completado = false; }
    }

    // ── Estado de un desafío ──────────────────────────────────────────────
    [Serializable]
    public class EstadoDesafio
    {
        public string Id;
        public bool Activo;
        public bool Completado;
        public DateTime InicioDesafio;

        public EstadoDesafio(string id) { Id = id; Activo = false; Completado = false; }
    }

    // ── Estado de sub-mejora de cadena ────────────────────────────────────
    [Serializable]
    public class EstadoSubMejoraCadena
    {
        public string Id;
        public int Nivel;
        public bool Desbloqueada;

        public EstadoSubMejoraCadena(string id)
        {
            Id = id; Nivel = 0; Desbloqueada = false;
        }
    }

    // ── Estado de misión activa ───────────────────────────────────────────
    [Serializable]
    public class EstadoMision
    {
        public string Id;
        public double ProgresoActual;
        public bool Completada;

        public EstadoMision(string id)
        {
            Id = id; ProgresoActual = 0; Completada = false;
        }
    }

    // ── Estado de nodo del Códice ─────────────────────────────────────────
    [Serializable]
    public class EstadoNodoCodice
    {
        public string Id;
        public int Nivel;

        public EstadoNodoCodice(string id) { Id = id; Nivel = 0; }
    }

    // ── Estado central del juego ──────────────────────────────────────────
    [Serializable]
    public class EstadoJuego
    {
        // Recursos principales
        public double EnergiaVital;
        public double EVPorSegundo;
        public int EraActual = 1;
        public double TiempoJugadoTotal;

        // Recursos secundarios
        public Dictionary<string, EstadoRecurso> Recursos
            = new Dictionary<string, EstadoRecurso>();

        // Mejoras (clave = id mejora)
        public Dictionary<string, EstadoMejora> Mejoras
            = new Dictionary<string, EstadoMejora>();

        // Sinergias
        public Dictionary<string, EstadoSinergia> Sinergias
            = new Dictionary<string, EstadoSinergia>();

        // Árbol de evolución
        public Dictionary<string, EstadoNodo> Nodos
            = new Dictionary<string, EstadoNodo>();

        // Prestige
        public EstadoPrestige Prestige = new EstadoPrestige();

        // Racha
        public EstadoRacha Racha = new EstadoRacha();

        // Logros
        public Dictionary<string, EstadoLogro> Logros
            = new Dictionary<string, EstadoLogro>();

        // Desafíos
        public Dictionary<string, EstadoDesafio> Desafios
            = new Dictionary<string, EstadoDesafio>();

        // Cadenas (cuello de botella per-pilar)
        public Dictionary<string, EstadoSubMejoraCadena> Cadenas
            = new Dictionary<string, EstadoSubMejoraCadena>();

        // Misiones
        public EstadoMision[] MisionesActivas = new EstadoMision[3];
        public HashSet<string> MisionesCompletadas = new HashSet<string>();

        // Códice Fósil
        public Dictionary<string, EstadoNodoCodice> NodosCodice
            = new Dictionary<string, EstadoNodoCodice>();

        // Bifurcaciones evolutivas (pilar -> opción 0 o 1, -1 = no elegida)
        public Dictionary<TipoPilar, int> Bifurcaciones
            = new Dictionary<TipoPilar, int>();

        // UI - desbloqueos progresivos
        public double EVMaximoAlcanzado;
        public bool EvolucionUIDesbloqueada;
        public bool PrestigeUIDesbloqueado;
        public bool CodiceUIDesbloqueado;

        // Multiplicadores temporales (eventos)
        public double MultiplicadorEvento = 1.0;
        public float TiempoRestanteEvento = 0f;
        public string EventoActivoId = null;

        // Stats de sesión (no persistidos)
        [NonSerialized] public double EVGanadaEnSesion;
        [NonSerialized] public int MejorasCompradasEnSesion;

        // Helpers
        public int NivelTotalPilar(TipoPilar pilar, Data.DefinicionMejora[] defsDelPilar)
        {
            int total = 0;
            foreach (var def in defsDelPilar)
                if (Mejoras.TryGetValue(def.Id, out var est))
                    total += est.Nivel;
            return total;
        }

        public bool SinergiaActiva(string id) =>
            Sinergias.TryGetValue(id, out var s) && s.Activa;

        public int NivelMejora(string id) =>
            Mejoras.TryGetValue(id, out var m) ? m.Nivel : 0;

        public bool MejoraDesbloqueada(string id) =>
            Mejoras.TryGetValue(id, out var m) && m.Desbloqueada;

        public int NivelNodo(string id) =>
            Nodos.TryGetValue(id, out var n) ? n.Nivel : 0;

        public bool LogroCompletado(string id) =>
            Logros.TryGetValue(id, out var l) && l.Completado;
    }
}
