using System.Linq;
using Terra.Core;
using Terra.Data;
using Terra.State;

namespace Terra.Systems
{
    /// <summary>
    /// Gestiona las cadenas per-pilar (cuello de botella).
    /// Producción efectiva de un pilar = min(generación, procesamiento, distribución).
    /// Patrón ISistema + AsignarEstado como el resto de sistemas.
    /// </summary>
    public class SistemaCadenas : ISistema
    {
        private readonly DefinicionSubMejoraCadena[] _definiciones;
        private EstadoJuego _estado;

        // Era mínima por pilar (derivada de las definiciones)
        private readonly int[] _eraDesbloqueo = new int[4]; // indexado por TipoPilar

        public SistemaCadenas(DefinicionSubMejoraCadena[] definiciones)
        {
            _definiciones = definiciones;

            // Calcular era de desbloqueo por pilar (la mínima de sus sub-mejoras)
            for (int p = 0; p < 4; p++)
            {
                var pilar = (TipoPilar)p;
                var defsDelPilar = _definiciones.Where(d => d.Pilar == pilar).ToArray();
                _eraDesbloqueo[p] = defsDelPilar.Length > 0
                    ? defsDelPilar.Min(d => d.EraRequerida)
                    : 99;
            }
        }

        public void Inicializar() { }

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;

            foreach (var def in _definiciones)
                if (!_estado.Cadenas.ContainsKey(def.Id))
                    _estado.Cadenas[def.Id] = new EstadoSubMejoraCadena(def.Id);

            ComprobarDesbloqueos();
        }

        public void Actualizar(float delta) { }

        // ── Compra ────────────────────────────────────────────────────────

        public bool ComprarNivel(string idSubMejora)
        {
            var def = BuscarDefinicion(idSubMejora);
            if (def == null) return false;

            var est = _estado.Cadenas[idSubMejora];
            if (!est.Desbloqueada || est.Nivel >= def.NivelMax) return false;

            double coste = def.CosteEnNivel(est.Nivel);
            if (_estado.EnergiaVital < coste) return false;

            _estado.EnergiaVital -= coste;
            est.Nivel++;

            EventBus.Publicar(new EventoCadenaComprada(idSubMejora, est.Nivel));
            ComprobarDesbloqueos();
            return true;
        }

        public int ComprarMax(string idSubMejora)
        {
            var def = BuscarDefinicion(idSubMejora);
            if (def == null) return 0;

            var est = _estado.Cadenas[idSubMejora];
            if (!est.Desbloqueada) return 0;

            int comprados = 0;
            while (est.Nivel < def.NivelMax)
            {
                double coste = def.CosteEnNivel(est.Nivel);
                if (_estado.EnergiaVital < coste) break;

                _estado.EnergiaVital -= coste;
                est.Nivel++;
                comprados++;
            }

            if (comprados > 0)
            {
                EventBus.Publicar(new EventoCadenaComprada(idSubMejora, est.Nivel));
                ComprobarDesbloqueos();
            }

            return comprados;
        }

        // ── Desbloqueos ───────────────────────────────────────────────────

        public void ComprobarDesbloqueos()
        {
            foreach (var def in _definiciones)
            {
                var est = _estado.Cadenas[def.Id];
                if (est.Desbloqueada) continue;

                // Era requerida
                if (def.EraRequerida > _estado.EraActual) continue;

                // Nivel acumulado del eslabón requerido
                if (def.NivelEslabonRequerido > 0)
                {
                    int nivelEslabon = NivelAcumuladoEslabon(def.Pilar, def.Eslabon);
                    if (nivelEslabon < def.NivelEslabonRequerido) continue;
                }

                est.Desbloqueada = true;
            }
        }

        // ── Cálculos de cap ───────────────────────────────────────────────

        /// <summary>
        /// Cap de un eslabón = suma de (capPorNivel × nivel) de sus sub-mejoras.
        /// </summary>
        public double CalcularCapEslabon(TipoPilar pilar, TipoEslabon eslabon)
        {
            double cap = 0;
            foreach (var def in _definiciones)
            {
                if (def.Pilar != pilar || def.Eslabon != eslabon) continue;
                var est = _estado.Cadenas[def.Id];
                cap += def.CapPorNivel * est.Nivel;
            }
            return cap;
        }

        /// <summary>
        /// Cap efectivo del pilar = min(generación, procesamiento, distribución).
        /// Retorna double.MaxValue si la cadena no está desbloqueada (sin límite).
        /// </summary>
        public double CalcularCapPilar(TipoPilar pilar)
        {
            if (!CadenaPilarDesbloqueada(pilar)) return double.MaxValue;

            double gen  = CalcularCapEslabon(pilar, TipoEslabon.Generacion);
            double proc = CalcularCapEslabon(pilar, TipoEslabon.Procesamiento);
            double dist = CalcularCapEslabon(pilar, TipoEslabon.Distribucion);

            return System.Math.Min(gen, System.Math.Min(proc, dist));
        }

        /// <summary>
        /// Devuelve el eslabón que más limita la producción del pilar.
        /// </summary>
        public TipoEslabon IdentificarCuelloBotella(TipoPilar pilar)
        {
            double gen  = CalcularCapEslabon(pilar, TipoEslabon.Generacion);
            double proc = CalcularCapEslabon(pilar, TipoEslabon.Procesamiento);
            double dist = CalcularCapEslabon(pilar, TipoEslabon.Distribucion);

            if (gen <= proc && gen <= dist) return TipoEslabon.Generacion;
            if (proc <= dist) return TipoEslabon.Procesamiento;
            return TipoEslabon.Distribucion;
        }

        /// <summary>
        /// True si la era actual permite usar cadenas de este pilar.
        /// </summary>
        public bool CadenaPilarDesbloqueada(TipoPilar pilar) =>
            _estado.EraActual >= _eraDesbloqueo[(int)pilar];

        // ── Consultas para UI ─────────────────────────────────────────────

        public DefinicionSubMejoraCadena[] ObtenerSubMejorasPorEslabon(TipoPilar pilar, TipoEslabon eslabon) =>
            _definiciones.Where(d => d.Pilar == pilar && d.Eslabon == eslabon).ToArray();

        public DefinicionSubMejoraCadena[] ObtenerPorPilar(TipoPilar pilar) =>
            _definiciones.Where(d => d.Pilar == pilar).ToArray();

        public DefinicionSubMejoraCadena BuscarDefinicion(string id)
        {
            foreach (var d in _definiciones)
                if (d.Id == id) return d;
            return null;
        }

        /// <summary>
        /// Nivel acumulado de todas las sub-mejoras de un eslabón.
        /// Usado para desbloqueos progresivos dentro del eslabón.
        /// </summary>
        public int NivelAcumuladoEslabon(TipoPilar pilar, TipoEslabon eslabon)
        {
            int total = 0;
            foreach (var def in _definiciones)
            {
                if (def.Pilar != pilar || def.Eslabon != eslabon) continue;
                if (_estado.Cadenas.TryGetValue(def.Id, out var est))
                    total += est.Nivel;
            }
            return total;
        }
    }
}
