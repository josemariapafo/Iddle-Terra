using Terra.Core;
using Terra.Data;
using Terra.State;

namespace Terra.Systems
{
    /// <summary>
    /// Auto-compra de mejoras. Cada automatización tiene su propio timer interno.
    /// Las 4 de pilar compran la mejora más barata desbloqueada de su pilar.
    /// SeleccionNatural compra la de mejor ratio (incremento prod / coste) global.
    ///
    /// El desbloqueo se infiere del nivel del nodo correspondiente del árbol:
    ///   Gravedad         → nd_33
    ///   Corrientes       → nd_34
    ///   Viento           → nd_35
    ///   EvolucionNatural → nd_36
    ///   SeleccionNatural → nd_37
    /// </summary>
    public class SistemaAutomatizacion : ISistema
    {
        private const float INTERVALO = 2.0f;
        private const int CANTIDAD    = 5;

        private static readonly string[] _nodosDesbloqueo =
        {
            "nd_33", "nd_34", "nd_35", "nd_36", "nd_37"
        };

        private static readonly TipoPilar[] _pilarPorTipo =
        {
            TipoPilar.Tierra,    // Gravedad
            TipoPilar.Oceanos,   // Corrientes
            TipoPilar.Atmosfera, // Viento
            TipoPilar.Vida       // EvolucionNatural
            // SeleccionNatural no tiene pilar fijo
        };

        private readonly SistemaMejoras _mejoras;
        private EstadoJuego _estado;
        private readonly float[] _timers = new float[CANTIDAD];

        public SistemaAutomatizacion(SistemaMejoras mejoras) => _mejoras = mejoras;

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;
            if (_estado.AutomatizacionesActivas == null
                || _estado.AutomatizacionesActivas.Length < CANTIDAD)
                _estado.AutomatizacionesActivas = new bool[CANTIDAD];
        }

        public void Inicializar() { }

        public void Actualizar(float delta)
        {
            if (_estado == null || _mejoras == null) return;

            for (int i = 0; i < CANTIDAD; i++)
            {
                if (!_estado.AutomatizacionesActivas[i]) continue;
                if (!EstaDesbloqueada((TipoAutomatizacion)i)) continue;

                _timers[i] += delta;
                if (_timers[i] < INTERVALO) continue;
                _timers[i] = 0f;

                EjecutarTick((TipoAutomatizacion)i);
            }
        }

        // ── API pública ───────────────────────────────────────────────────

        public bool EstaDesbloqueada(TipoAutomatizacion tipo)
        {
            string idNodo = _nodosDesbloqueo[(int)tipo];
            return _estado.NivelNodo(idNodo) > 0;
        }

        public bool EstaActivada(TipoAutomatizacion tipo) =>
            _estado != null && _estado.AutomatizacionesActivas[(int)tipo];

        public void Alternar(TipoAutomatizacion tipo)
        {
            if (_estado == null) return;
            if (!EstaDesbloqueada(tipo)) return;
            int i = (int)tipo;
            _estado.AutomatizacionesActivas[i] = !_estado.AutomatizacionesActivas[i];
            _timers[i] = 0f;
        }

        public void Establecer(TipoAutomatizacion tipo, bool activa)
        {
            if (_estado == null) return;
            if (activa && !EstaDesbloqueada(tipo)) return;
            _estado.AutomatizacionesActivas[(int)tipo] = activa;
            _timers[(int)tipo] = 0f;
        }

        // ── Lógica de tick ────────────────────────────────────────────────

        private void EjecutarTick(TipoAutomatizacion tipo)
        {
            if (tipo == TipoAutomatizacion.SeleccionNatural)
            {
                EjecutarSmart();
                return;
            }

            // Modo pilar: compra la mejora más barata desbloqueada del pilar
            TipoPilar pilar = _pilarPorTipo[(int)tipo];
            DefinicionMejora elegida = null;
            double costeMin = double.MaxValue;

            foreach (var def in _mejoras.ObtenerPorPilar(pilar))
            {
                var est = _estado.Mejoras[def.Id];
                if (!est.Desbloqueada || est.Nivel >= def.NivelMax) continue;
                double coste = def.CosteEnNivel(est.Nivel);
                if (coste < costeMin && coste <= _estado.EnergiaVital)
                {
                    costeMin = coste;
                    elegida = def;
                }
            }

            if (elegida != null)
                _mejoras.ComprarUno(elegida.Id);
        }

        private void EjecutarSmart()
        {
            // Recorre TODAS las mejoras desbloqueadas y elige la de mejor ratio
            //   incrementoProduccion(nivel+1) / coste(nivel)
            DefinicionMejora elegida = null;
            double mejorRatio = 0;

            foreach (var def in _mejoras.ObtenerPorPilar(TipoPilar.Atmosfera))
                EvaluarSmart(def, ref elegida, ref mejorRatio);
            foreach (var def in _mejoras.ObtenerPorPilar(TipoPilar.Oceanos))
                EvaluarSmart(def, ref elegida, ref mejorRatio);
            foreach (var def in _mejoras.ObtenerPorPilar(TipoPilar.Tierra))
                EvaluarSmart(def, ref elegida, ref mejorRatio);
            foreach (var def in _mejoras.ObtenerPorPilar(TipoPilar.Vida))
                EvaluarSmart(def, ref elegida, ref mejorRatio);

            if (elegida != null)
                _mejoras.ComprarUno(elegida.Id);
        }

        private void EvaluarSmart(DefinicionMejora def, ref DefinicionMejora elegida, ref double mejorRatio)
        {
            var est = _estado.Mejoras[def.Id];
            if (!est.Desbloqueada || est.Nivel >= def.NivelMax) return;

            double coste = def.CosteEnNivel(est.Nivel);
            if (coste > _estado.EnergiaVital || coste <= 0) return;

            double incremento = def.ProduccionEnNivel(est.Nivel + 1) - def.ProduccionEnNivel(est.Nivel);
            double ratio = incremento / coste;

            if (ratio > mejorRatio)
            {
                mejorRatio = ratio;
                elegida = def;
            }
        }
    }
}
