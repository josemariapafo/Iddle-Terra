using System.Collections.Generic;
using Terra.Core;
using Terra.Data;
using Terra.State;

namespace Terra.Systems
{
    /// <summary>
    /// Gestor de bifurcaciones evolutivas (T26). Era 6+.
    ///
    /// Cada pilar tiene UNA bifurcación con dos opciones mutuamente excluyentes.
    /// El jugador la elige una vez llegada la Era 6; se resetea con cada prestige.
    ///
    /// Responsabilidades:
    ///   1. Detectar al avanzar a Era 6 qué pilares requieren bifurcación
    ///      y publicar EventoBifurcacionRequerida para que la UI la presente.
    ///   2. Exponer API para que la UI registre la elección.
    ///   3. Resolver el multiplicador por (pilar, eslabón) que SistemaCadenas
    ///      consulta al calcular el cap.
    /// </summary>
    public class SistemaBifurcaciones : ISistema
    {
        private readonly DefinicionBifurcacion[] _definiciones;
        private readonly Dictionary<TipoPilar, DefinicionBifurcacion> _porPilar;
        private EstadoJuego _estado;

        // Rastrea qué pilares ya tienen un prompt UI pendiente — evita spam
        // de EventoBifurcacionRequerida en cada tick. Se resetea al elegir
        // y al entrar prestige (via AsignarEstado + cambios a -1).
        private readonly HashSet<TipoPilar> _notificados = new HashSet<TipoPilar>();

        // Era mínima para que se active la bifurcación.
        // Coherente con el diseño: Era 6 introduce el Códice Genético,
        // y las bifurcaciones son decisiones "ya que estás en Era 6".
        private const int ERA_BIFURCACION = 6;

        public SistemaBifurcaciones(DefinicionBifurcacion[] definiciones)
        {
            _definiciones = definiciones;
            _porPilar = new Dictionary<TipoPilar, DefinicionBifurcacion>(definiciones.Length);
            foreach (var def in definiciones)
                _porPilar[def.Pilar] = def;
        }

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;

            // Asegurar entrada -1 (no elegida) para cada pilar si no hay nada
            foreach (TipoPilar p in System.Enum.GetValues(typeof(TipoPilar)))
                if (!_estado.Bifurcaciones.ContainsKey(p))
                    _estado.Bifurcaciones[p] = -1;
        }

        public void Inicializar() { }

        public void Actualizar(float delta)
        {
            // Detección pasiva: si estamos en Era 6+ y algún pilar todavía
            // no ha elegido, publicar evento UNA sola vez por pilar (hasta
            // que se elija o se haga prestige — entonces se re-notifica).
            if (_estado.EraActual < ERA_BIFURCACION)
            {
                if (_notificados.Count > 0) _notificados.Clear();
                return;
            }

            foreach (var def in _definiciones)
            {
                int opcion = _estado.Bifurcaciones[def.Pilar];
                if (opcion < 0 && !_notificados.Contains(def.Pilar))
                {
                    EventBus.Publicar(new EventoBifurcacionRequerida(def.Pilar, IdBifurcacion(def.Pilar)));
                    _notificados.Add(def.Pilar);
                }
                else if (opcion >= 0 && _notificados.Contains(def.Pilar))
                {
                    _notificados.Remove(def.Pilar);
                }
            }
        }

        /// <summary>
        /// Registra la elección del jugador. Solo válida una vez por run
        /// (los valores ≠ -1 no pueden modificarse hasta el próximo prestige).
        /// </summary>
        public bool Elegir(TipoPilar pilar, int opcion)
        {
            if (_estado.EraActual < ERA_BIFURCACION) return false;
            if (opcion != 0 && opcion != 1) return false;
            if (!_porPilar.ContainsKey(pilar)) return false;
            if (_estado.Bifurcaciones[pilar] >= 0) return false;  // ya elegida

            _estado.Bifurcaciones[pilar] = opcion;
            EventBus.Publicar(new EventoBifurcacionElegida(pilar, opcion));
            return true;
        }

        /// <summary>
        /// Multiplicador para SistemaCadenas — si no hay elección, 1.0.
        /// </summary>
        public double MultiplicadorEslabon(TipoPilar pilar, TipoEslabon eslabon)
        {
            if (!_porPilar.TryGetValue(pilar, out var def)) return 1.0;
            int opcion = _estado.Bifurcaciones[pilar];
            return def.MultiplicadorEslabon(opcion, eslabon);
        }

        // ── Consultas para UI ─────────────────────────────────────────────

        public DefinicionBifurcacion[] ObtenerDefiniciones() => _definiciones;

        public DefinicionBifurcacion BuscarPorPilar(TipoPilar pilar) =>
            _porPilar.TryGetValue(pilar, out var def) ? def : null;

        public int OpcionElegida(TipoPilar pilar) =>
            _estado.Bifurcaciones.TryGetValue(pilar, out var o) ? o : -1;

        public bool RequiereDecision(TipoPilar pilar) =>
            _estado.EraActual >= ERA_BIFURCACION
            && _porPilar.ContainsKey(pilar)
            && _estado.Bifurcaciones[pilar] < 0;

        public bool HayBifurcacionPendiente()
        {
            if (_estado.EraActual < ERA_BIFURCACION) return false;
            foreach (var def in _definiciones)
                if (_estado.Bifurcaciones[def.Pilar] < 0) return true;
            return false;
        }

        private static string IdBifurcacion(TipoPilar p) => $"bif_{p.ToString().ToLower()}";
    }
}
