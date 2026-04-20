using System.Collections.Generic;
using Terra.Core;
using Terra.Data;
using Terra.State;

namespace Terra.Systems
{
    /// <summary>
    /// Gestiona el Códice Genético (T25): árbol de gasto de Genes (Era 6+).
    /// 20 nodos en 3 ramas (Adaptación, Mutación, Simbiosis).
    /// Se resetea SOLO con Big Bang (sobrevive a Extinción y Glaciación).
    /// Patrón paralelo a SistemaCodice para mantener separación limpia.
    /// </summary>
    public class SistemaCodiceGenetico : ISistema
    {
        private readonly DefinicionNodoCodiceGenetico[] _definiciones;
        private readonly Dictionary<string, DefinicionNodoCodiceGenetico> _porId;
        private EstadoJuego _estado;

        public SistemaCodiceGenetico(DefinicionNodoCodiceGenetico[] definiciones)
        {
            _definiciones = definiciones;
            _porId = new Dictionary<string, DefinicionNodoCodiceGenetico>(definiciones.Length);
            foreach (var def in definiciones)
                _porId[def.Id] = def;
        }

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;
            foreach (var def in _definiciones)
                if (!_estado.NodosCodiceGenetico.ContainsKey(def.Id))
                    _estado.NodosCodiceGenetico[def.Id] = new EstadoNodoCodice(def.Id);
        }

        public void Inicializar() { }
        public void Actualizar(float delta) { }

        // ── Compra ────────────────────────────────────────────────────────

        public bool ComprarNodo(string id)
        {
            if (!_porId.TryGetValue(id, out var def)) return false;
            var est = _estado.NodosCodiceGenetico[id];

            if (est.Nivel >= def.NivelMax) return false;
            if (!PrerequisitoCumplido(def)) return false;

            double coste = def.CosteEnNivel(est.Nivel);
            if (_estado.Prestige.Genes < coste) return false;

            _estado.Prestige.Genes -= coste;
            est.Nivel++;

            EventBus.Publicar(new EventoNodoCodiceComprado(id, est.Nivel));
            return true;
        }

        public bool PrerequisitoCumplido(DefinicionNodoCodiceGenetico def)
        {
            if (string.IsNullOrEmpty(def.NodoPrevioId)) return true;
            return _estado.NodosCodiceGenetico.TryGetValue(def.NodoPrevioId, out var previo)
                && previo.Nivel > 0;
        }

        public bool PuedeComprar(string id)
        {
            if (!_porId.TryGetValue(id, out var def)) return false;
            var est = _estado.NodosCodiceGenetico[id];
            if (est.Nivel >= def.NivelMax) return false;
            if (!PrerequisitoCumplido(def)) return false;
            return _estado.Prestige.Genes >= def.CosteEnNivel(est.Nivel);
        }

        // ── Consultas de bonus ────────────────────────────────────────────

        /// <summary>
        /// Suma el valor total de un tipo de bonus específico
        /// (nivel × valorPorNivel) de todos los nodos del Códice Genético.
        /// </summary>
        public double ObtenerBonus(TipoBonus tipo)
        {
            // Guardia defensiva — ver comentario análogo en SistemaCodice.
            if (_estado == null) return 0;
            double total = 0;
            foreach (var def in _definiciones)
            {
                if (def.TipoBonus != tipo) continue;
                var est = _estado.NodosCodiceGenetico[def.Id];
                if (est.Nivel > 0)
                    total += est.Nivel * def.ValorBonusPorNivel;
            }
            return total;
        }

        // ── Adaptación ──

        /// <summary>Extra % cap cadenas Gen: aditivo, multiplicativo al Fósil. 0.0 a ~1.60.</summary>
        public double BonusCapCadenaGen() => ObtenerBonus(TipoBonus.BonusCapCadenaGen);

        /// <summary>Reducción nivel requisito sub-mejoras (0-3).</summary>
        public int ReduccionReqEslabones() => (int)ObtenerBonus(TipoBonus.ReduccionReqEslabones);

        /// <summary>Extra % fósiles en prestige (multiplicativo al del Fósil). 0.0 a 0.30.</summary>
        public double BonusFosilesPrestige() => ObtenerBonus(TipoBonus.BonusFosilesPrestige);

        /// <summary>Extra % genes en Glaciación. 0.0 a 0.30.</summary>
        public double BonusGenesPrestige() => ObtenerBonus(TipoBonus.BonusGenesPrestige);

        /// <summary>Reducción coste cadenas Gen (aditivo al Fósil). 0.0 a 0.18.</summary>
        public double ReduccionCosteCadenas() => ObtenerBonus(TipoBonus.ReduccionCosteCadenas);

        // ── Mutación ──

        /// <summary>Efectividad sinergias (multiplicador extra). 0.0 a ~1.20.</summary>
        public double BonusEfectividadSinergias() => ObtenerBonus(TipoBonus.BonusEfectividadSinergias);

        /// <summary>Reducción cooldown eventos. 0.0 a 0.45.</summary>
        public double ReduccionCooldownEventos() => ObtenerBonus(TipoBonus.ReduccionCooldownEventos);

        /// <summary>Si es >0, eventos con opciones exponen una 4ª opción extra.</summary>
        public bool OpcionEventoExtraDesbloqueada() =>
            ObtenerBonus(TipoBonus.OpcionEventoExtra) > 0;

        /// <summary>Reducción coste mejoras Gen (multiplicativo al Fósil). 0.0 a 0.20.</summary>
        public double ReduccionCosteMejoras() => ObtenerBonus(TipoBonus.ReduccionCosteMejoras);

        // ── Simbiosis ──

        /// <summary>
        /// Extra % cuando 3+ pilares están balanceados (nivel dentro del 50% del mayor).
        /// El caller (CalculadorProduccion) decide si aplicar según el estado.
        /// 0.0 a ~1.26.
        /// </summary>
        public double BonusPilaresBalanceados() => ObtenerBonus(TipoBonus.BonusPilaresBalanceados);

        /// <summary>Multiplicador global EV/s del Códice Genético. 0.0 a ~0.86.</summary>
        public double MultiplicadorGlobalGen() => ObtenerBonus(TipoBonus.MultiplicadorGlobalGen);

        /// <summary>Extra % a los multiplicadores de bifurcación. 0.0 a 0.85.</summary>
        public double BonusMultiplicadoresBifurcacion() =>
            ObtenerBonus(TipoBonus.BonusMultiplicadoresBifurcacion);

        // ── Consultas para UI ─────────────────────────────────────────────

        public DefinicionNodoCodiceGenetico[] ObtenerDefiniciones() => _definiciones;

        public DefinicionNodoCodiceGenetico BuscarDefinicion(string id) =>
            _porId.TryGetValue(id, out var def) ? def : null;

        public int NivelNodo(string id) =>
            _estado != null && _estado.NodosCodiceGenetico.TryGetValue(id, out var est) ? est.Nivel : 0;
    }
}
