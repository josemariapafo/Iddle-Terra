using System.Collections.Generic;
using Terra.Core;
using Terra.Data;
using Terra.State;

namespace Terra.Systems
{
    /// <summary>
    /// Gestiona el Códice Fósil: árbol de gasto de prestige (fósiles).
    /// 15 nodos en 3 ramas lineales. NO se resetea con prestige parcial.
    /// </summary>
    public class SistemaCodice : ISistema
    {
        private readonly DefinicionNodoCodice[] _definiciones;
        private readonly Dictionary<string, DefinicionNodoCodice> _porId;
        private EstadoJuego _estado;

        public SistemaCodice(DefinicionNodoCodice[] definiciones)
        {
            _definiciones = definiciones;
            _porId = new Dictionary<string, DefinicionNodoCodice>(definiciones.Length);
            foreach (var def in definiciones)
                _porId[def.Id] = def;
        }

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;
            foreach (var def in _definiciones)
                if (!_estado.NodosCodice.ContainsKey(def.Id))
                    _estado.NodosCodice[def.Id] = new EstadoNodoCodice(def.Id);
        }

        public void Inicializar() { }
        public void Actualizar(float delta) { }

        // ── Compra ────────────────────────────────────────────────────────

        public bool ComprarNodo(string id)
        {
            if (!_porId.TryGetValue(id, out var def)) return false;
            var est = _estado.NodosCodice[id];

            if (est.Nivel >= def.NivelMax) return false;
            if (!PrerequisitoCumplido(def)) return false;

            double coste = def.CosteEnNivel(est.Nivel);
            if (_estado.Prestige.Fosiles < coste) return false;

            _estado.Prestige.Fosiles -= coste;
            est.Nivel++;

            EventBus.Publicar(new EventoNodoCodiceComprado(id, est.Nivel));
            return true;
        }

        public bool PrerequisitoCumplido(DefinicionNodoCodice def)
        {
            if (string.IsNullOrEmpty(def.NodoPrevioId)) return true;
            return _estado.NodosCodice.TryGetValue(def.NodoPrevioId, out var previo)
                && previo.Nivel > 0;
        }

        public bool PuedeComprar(string id)
        {
            if (!_porId.TryGetValue(id, out var def)) return false;
            var est = _estado.NodosCodice[id];
            if (est.Nivel >= def.NivelMax) return false;
            if (!PrerequisitoCumplido(def)) return false;
            return _estado.Prestige.Fosiles >= def.CosteEnNivel(est.Nivel);
        }

        // ── Consultas de bonus ────────────────────────────────────────────

        /// <summary>
        /// Suma el valor total de un tipo de bonus específico
        /// (nivel × valorPorNivel) de todos los nodos que tengan ese tipo.
        /// </summary>
        public double ObtenerBonus(TipoBonus tipo)
        {
            double total = 0;
            foreach (var def in _definiciones)
            {
                if (def.TipoBonus != tipo) continue;
                var est = _estado.NodosCodice[def.Id];
                if (est.Nivel > 0)
                    total += est.Nivel * def.ValorBonusPorNivel;
            }
            return total;
        }

        // ── Abundancia ──

        /// <summary>Multiplicador EV/s del Códice: producto de todos los nodos MultiplicadorEV.</summary>
        public double MultiplicadorEV()
        {
            double mult = 1.0;
            foreach (var def in _definiciones)
            {
                if (def.TipoBonus != TipoBonus.MultiplicadorEV) continue;
                var est = _estado.NodosCodice[def.Id];
                if (est.Nivel > 0)
                    mult += est.Nivel * def.ValorBonusPorNivel;
            }
            return mult; // aditivo: 1.0 + 0.10*5 + 0.15*3 + 0.20*2 = max 2.35x
        }

        /// <summary>Extra % nocturno: 0.0 si nada, 0.75 al max (3 × 0.25).</summary>
        public double BonusNocturno() => ObtenerBonus(TipoBonus.BonusNocturno);

        /// <summary>Extra % sinergias: 0.0 si nada, 0.40 al max (5 × 0.08).</summary>
        public double BonusSinergias() => ObtenerBonus(TipoBonus.BonusSinergias);

        // ── Eficiencia ──

        /// <summary>Reducción de coste mejoras: 0.0 a 0.40 (5 × 0.08).</summary>
        public double ReduccionCosteMejoras() => ObtenerBonus(TipoBonus.ReduccionCosteMejoras);

        /// <summary>Reducción de coste cadenas: 0.0 a 0.30 (3 × 0.10).</summary>
        public double ReduccionCosteCadenas() => ObtenerBonus(TipoBonus.ReduccionCosteCadenas);

        /// <summary>Niveles gratis en mejoras Era 1 tras prestige: 0 a 3.</summary>
        public int NivelesGratisInicio() => (int)ObtenerBonus(TipoBonus.NivelesGratisInicio);

        /// <summary>Extra % fósiles en prestige: 0.0 a 0.45 (3 × 0.15).</summary>
        public double BonusFosilesPrestige() => ObtenerBonus(TipoBonus.BonusFosilesPrestige);

        /// <summary>Extra % cap cadenas: 0.0 a 0.45 (3 × 0.15).</summary>
        public double BonusCapCadena() => ObtenerBonus(TipoBonus.BonusCapCadena);

        // ── Dominio ──

        /// <summary>Extra % poder tap: 0.0 a 1.50 (5 × 0.30).</summary>
        public double BonusTap() => ObtenerBonus(TipoBonus.BonusTap);

        /// <summary>Reducción de taps para combo: 0 a 2.</summary>
        public int ReduccionTapsCombo() => (int)ObtenerBonus(TipoBonus.ReduccionTapsCombo);

        /// <summary>Segundos extra de combo: 0 a 9 (3 × 3s).</summary>
        public float DuracionComboExtra() => (float)ObtenerBonus(TipoBonus.DuracionCombo);

        /// <summary>Extra multiplicador combo: 0.0 a 0.50 (2 × 0.25).</summary>
        public double MultiplicadorComboExtra() => ObtenerBonus(TipoBonus.MultiplicadorCombo);

        /// <summary>Nivel de auto-tap: 0 a 3.</summary>
        public int NivelAutoTap() => (int)ObtenerBonus(TipoBonus.AutoTap);

        // ── Consultas para UI ─────────────────────────────────────────────

        public DefinicionNodoCodice[] ObtenerDefiniciones() => _definiciones;

        public DefinicionNodoCodice BuscarDefinicion(string id) =>
            _porId.TryGetValue(id, out var def) ? def : null;

        public int NivelNodo(string id) =>
            _estado.NodosCodice.TryGetValue(id, out var est) ? est.Nivel : 0;
    }
}
