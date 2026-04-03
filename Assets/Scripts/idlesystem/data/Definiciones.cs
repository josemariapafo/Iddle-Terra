using System;
using Terra.Core;

namespace Terra.Data
{
    // ── Definición de mejora (inmutable) ──────────────────────────────────
    [Serializable]
    public class DefinicionMejora
    {
        public string Id;
        public string Nombre;
        public string Descripcion;
        public TipoPilar Pilar;
        public int Zona;                        // 0,1,2 — zona visual dentro del pilar
        public double CosteBase;
        public double ProduccionBase;           // EV/s en nivel 1
        public double MultiplicadorCoste;       // default 1.15
        public int NivelMax;                    // default 500
        public int EraRequerida;
        public string[] RequisitosIds;          // mejoras previas requeridas
        public TipoPilar PilarRequisito;
        public int NivelPilarRequisito;         // nivel mínimo del pilar requisito

        public DefinicionMejora(
            string id, string nombre, string desc,
            TipoPilar pilar, int zona,
            double costeBase, double prodBase,
            int eraReq = 1,
            string[] requisitos = null,
            TipoPilar pilarReq = TipoPilar.Atmosfera,
            int nivelPilarReq = 0,
            double multCoste = 1.15,
            int nivelMax = 500)
        {
            Id = id; Nombre = nombre; Descripcion = desc;
            Pilar = pilar; Zona = zona;
            CosteBase = costeBase; ProduccionBase = prodBase;
            EraRequerida = eraReq;
            RequisitosIds = requisitos ?? Array.Empty<string>();
            PilarRequisito = pilarReq;
            NivelPilarRequisito = nivelPilarReq;
            MultiplicadorCoste = multCoste;
            NivelMax = nivelMax;
        }

        public double CosteEnNivel(int nivel) =>
            Math.Floor(CosteBase * Math.Pow(MultiplicadorCoste, nivel));

        public double ProduccionEnNivel(int nivel) =>
            nivel <= 0 ? 0 : ProduccionBase * nivel * Math.Pow(1.08, nivel);

        public double MultiplicadorHito(int nivel)
        {
            if (nivel >= 500) return 10.0;
            if (nivel >= 200) return 6.0;
            if (nivel >= 100) return 4.0;
            if (nivel >= 50)  return 2.5;
            if (nivel >= 25)  return 1.8;
            if (nivel >= 10)  return 1.3;
            return 1.0;
        }
    }

    // ── Definición de sinergia (inmutable) ────────────────────────────────
    [Serializable]
    public class DefinicionSinergia
    {
        public string Id;
        public string Nombre;
        public string Descripcion;
        public TipoPilar PilarA;
        public TipoPilar PilarB;
        public int NivelRequeridoA;
        public int NivelRequeridoB;
        public double Multiplicador;
        public bool RequiereCuatroPilares;      // para GAIA y similares

        public DefinicionSinergia(
            string id, string nombre, string desc,
            TipoPilar a, TipoPilar b,
            int nivelA, int nivelB,
            double mult,
            bool cuatroPilares = false)
        {
            Id = id; Nombre = nombre; Descripcion = desc;
            PilarA = a; PilarB = b;
            NivelRequeridoA = nivelA; NivelRequeridoB = nivelB;
            Multiplicador = mult;
            RequiereCuatroPilares = cuatroPilares;
        }
    }

    // ── Condición de era (inmutable) ──────────────────────────────────────
    [Serializable]
    public class CondicionEra
    {
        public int NivelTotalPorPilar;          // nivel mínimo de cada pilar
        public double EVRequerida;
        public string[] SinergiasRequeridas;    // ids de sinergias que deben estar activas
        public int VecesGaiaActivada;

        public CondicionEra(
            int nivelPilar, double ev,
            string[] sinergias = null,
            int gaia = 0)
        {
            NivelTotalPorPilar = nivelPilar;
            EVRequerida = ev;
            SinergiasRequeridas = sinergias ?? Array.Empty<string>();
            VecesGaiaActivada = gaia;
        }
    }

    [Serializable]
    public class DefinicionEra
    {
        public int Numero;
        public string Nombre;
        public string Descripcion;
        public CondicionEra Condicion;
        public string TexturaDiaId;
        public string TexturaNocheId;

        public DefinicionEra(int num, string nombre, string desc,
            CondicionEra condicion,
            string texDia = "", string texNoche = "")
        {
            Numero = num; Nombre = nombre; Descripcion = desc;
            Condicion = condicion;
            TexturaDiaId = texDia; TexturaNocheId = texNoche;
        }
    }

    // ── Definición de nodo del árbol de evolución (inmutable) ─────────────
    [Serializable]
    public class DefinicionNodo
    {
        public string Id;
        public string Nombre;
        public string Descripcion;
        public int EraRequerida;
        public string[] NodosPreviosIds;
        public double CosteBase;
        public int NivelMax;                    // default 10
        public TipoBonus TipoBonus;
        public TipoPilar PilarBonus;            // qué pilar afecta
        public double BonusPorNivel;            // multiplicador adicional por nivel

        public DefinicionNodo(
            string id, string nombre, string desc,
            int eraReq, string[] previos,
            double coste, TipoBonus tipoBonus,
            TipoPilar pilarBonus, double bonusPorNivel,
            int nivelMax = 10)
        {
            Id = id; Nombre = nombre; Descripcion = desc;
            EraRequerida = eraReq;
            NodosPreviosIds = previos ?? Array.Empty<string>();
            CosteBase = coste; TipoBonus = tipoBonus;
            PilarBonus = pilarBonus; BonusPorNivel = bonusPorNivel;
            NivelMax = nivelMax;
        }

        public double CosteEnNivel(int nivel) =>
            Math.Floor(CosteBase * Math.Pow(1.8, nivel));

        public double MultiplicadorTotal(int nivel) =>
            1.0 + BonusPorNivel * nivel;
    }

    // ── Definición de evento aleatorio (inmutable) ────────────────────────
    [Serializable]
    public class DefinicionEvento
    {
        public string Id;
        public string Nombre;
        public string Descripcion;
        public TipoEvento Tipo;
        public int EraMinima;
        public double Multiplicador;            // qué aplica al aceptar
        public float DuracionSegundos;
        public int CooldownMinutos;
        public TipoPilar PilarAfectado;
        public bool RequiereAccion;             // si false, se aplica automático

        public DefinicionEvento(
            string id, string nombre, string desc,
            TipoEvento tipo, int eraMin,
            double mult, float duracion,
            int cooldown, TipoPilar pilar,
            bool requiereAccion = true)
        {
            Id = id; Nombre = nombre; Descripcion = desc;
            Tipo = tipo; EraMinima = eraMin;
            Multiplicador = mult; DuracionSegundos = duracion;
            CooldownMinutos = cooldown; PilarAfectado = pilar;
            RequiereAccion = requiereAccion;
        }
    }

    // ── Definición de logro (inmutable) ───────────────────────────────────
    [Serializable]
    public class DefinicionLogro
    {
        public string Id;
        public string Nombre;
        public string Descripcion;
        public TipoLogro Tipo;
        public TipoBonus TipoBonus;
        public double ValorBonus;
        public TipoPilar PilarBonus;
        public Func<EstadoSnapshot, bool> Condicion;   // evaluada en runtime

        public DefinicionLogro(
            string id, string nombre, string desc,
            TipoLogro tipo,
            TipoBonus tipoBonus, double valorBonus,
            TipoPilar pilarBonus,
            Func<EstadoSnapshot, bool> condicion)
        {
            Id = id; Nombre = nombre; Descripcion = desc;
            Tipo = tipo; TipoBonus = tipoBonus;
            ValorBonus = valorBonus; PilarBonus = pilarBonus;
            Condicion = condicion;
        }
    }

    // ── Definición de desafío (inmutable) ─────────────────────────────────
    [Serializable]
    public class DefinicionDesafio
    {
        public string Id;
        public string Nombre;
        public string Descripcion;
        public int EraRequerida;
        public string RestriccionDescripcion;
        public string ObjetivoDescripcion;
        public double BonusMultiplicador;
        public Func<EstadoSnapshot, bool> CondicionVictoria;
        public Action<EstadoSnapshot> AplicarRestriccion;

        public DefinicionDesafio(
            string id, string nombre, string desc,
            int era, string restriccion, string objetivo,
            double bonus,
            Func<EstadoSnapshot, bool> condVictoria,
            Action<EstadoSnapshot> aplicarRestriccion)
        {
            Id = id; Nombre = nombre; Descripcion = desc;
            EraRequerida = era;
            RestriccionDescripcion = restriccion;
            ObjetivoDescripcion = objetivo;
            BonusMultiplicador = bonus;
            CondicionVictoria = condVictoria;
            AplicarRestriccion = aplicarRestriccion;
        }
    }

    // ── Snapshot de estado para condiciones ───────────────────────────────
    // Estructura de solo lectura que se pasa a condiciones de logros/desafíos
    public struct EstadoSnapshot
    {
        public double EV;
        public double EVPorSegundo;
        public int Era;
        public int[] NivelesPilares;            // nivel total de cada pilar
        public double TiempoJugado;
        public int VecesPrestige;
        public double Fosiles;
        public double Genes;
        public double Quarks;
        public int SinergiasActivas;
    }
}
