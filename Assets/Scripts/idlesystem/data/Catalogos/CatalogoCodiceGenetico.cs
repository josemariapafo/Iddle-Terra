using Terra.Core;

namespace Terra.Data.Catalogos
{
    /// <summary>
    /// 20 nodos del Códice Genético (Era 6+): 7 en cada rama principal + 6 simbiosis.
    /// Moneda: Genes (ganados en Glaciación).
    /// Se resetea SOLO con Big Bang (sobrevive a Extinción y Glaciación).
    ///
    /// Curva de costes:
    ///   Raíz (2-3 genes) = accesible tras 1 Glaciación con Codice Fósil base.
    ///   Tier medio (8-15 genes) = requiere 3-4 Glaciaciones.
    ///   Capstone (~30-40 genes) = requiere ~6+ Glaciaciones.
    ///   MultCoste x2 por nivel (como Fósil).
    ///
    /// Filosofía de diseño:
    ///   ADAPTACIÓN → acelerar la construcción del mundo (cadenas, prestige fósil)
    ///   MUTACIÓN   → profundizar el sistema (sinergias, eventos con opciones extra)
    ///   SIMBIOSIS  → recompensar juego balanceado y combinar las bifurcaciones
    /// </summary>
    public static class CatalogoCodiceGenetico
    {
        public static DefinicionNodoCodiceGenetico[] Crear()
        {
            return new[]
            {
                // ══════════════════════════════════════════════════════════════
                // ADAPTACIÓN — cadenas y construcción acelerada (7 nodos)
                // ══════════════════════════════════════════════════════════════

                new DefinicionNodoCodiceGenetico(
                    "cg_a1", "Plasticidad Genética",
                    "+12% cap de cadenas por nivel (multiplicativo al Fósil)",
                    TipoCodiceGenetico.Adaptacion, 2,
                    TipoBonus.BonusCapCadenaGen, 0.12,
                    nivelMax: 5),

                new DefinicionNodoCodiceGenetico(
                    "cg_a2", "Herencia Rápida",
                    "-1 nivel requisito para desbloquear sub-mejoras de cadena (por nivel)",
                    TipoCodiceGenetico.Adaptacion, 6,
                    TipoBonus.ReduccionReqEslabones, 1.0,
                    nivelMax: 3, nodoPrevio: "cg_a1"),

                new DefinicionNodoCodiceGenetico(
                    "cg_a3", "Selección Estable",
                    "+10% fósiles ganados en Extinción por nivel",
                    TipoCodiceGenetico.Adaptacion, 10,
                    TipoBonus.BonusFosilesPrestige, 0.10,
                    nivelMax: 3, nodoPrevio: "cg_a2"),

                new DefinicionNodoCodiceGenetico(
                    "cg_a4", "Adaptación Continua",
                    "-6% coste cadenas por nivel (multiplicativo al Fósil)",
                    TipoCodiceGenetico.Adaptacion, 14,
                    TipoBonus.ReduccionCosteCadenas, 0.06,
                    nivelMax: 3, nodoPrevio: "cg_a3"),

                new DefinicionNodoCodiceGenetico(
                    "cg_a5", "Codón Prístino",
                    "+18% cap de cadenas por nivel (refuerzo multiplicativo)",
                    TipoCodiceGenetico.Adaptacion, 22,
                    TipoBonus.BonusCapCadenaGen, 0.18,
                    nivelMax: 2, nodoPrevio: "cg_a4"),

                new DefinicionNodoCodiceGenetico(
                    "cg_a6", "Genoma Optimizado",
                    "+15% genes ganados en Glaciación por nivel",
                    TipoCodiceGenetico.Adaptacion, 30,
                    TipoBonus.BonusGenesPrestige, 0.15,
                    nivelMax: 2, nodoPrevio: "cg_a5"),

                new DefinicionNodoCodiceGenetico(
                    "cg_a7", "Evolución Dirigida",
                    "+20% a multiplicadores de Bifurcación",
                    TipoCodiceGenetico.Adaptacion, 40,
                    TipoBonus.BonusMultiplicadoresBifurcacion, 0.20,
                    nivelMax: 2, nodoPrevio: "cg_a6"),

                // ══════════════════════════════════════════════════════════════
                // MUTACIÓN — sinergias y eventos profundos (7 nodos)
                // ══════════════════════════════════════════════════════════════

                new DefinicionNodoCodiceGenetico(
                    "cg_m1", "Divergencia Útil",
                    "+8% efectividad de sinergias por nivel (multiplicativo)",
                    TipoCodiceGenetico.Mutacion, 3,
                    TipoBonus.BonusEfectividadSinergias, 0.08,
                    nivelMax: 5),

                new DefinicionNodoCodiceGenetico(
                    "cg_m2", "Pulso Mutagénico",
                    "-15% cooldown entre eventos por nivel",
                    TipoCodiceGenetico.Mutacion, 8,
                    TipoBonus.ReduccionCooldownEventos, 0.15,
                    nivelMax: 3, nodoPrevio: "cg_m1"),

                new DefinicionNodoCodiceGenetico(
                    "cg_m3", "Opción Arcana",
                    "Desbloquea una 4ª opción oculta en eventos con múltiples opciones",
                    TipoCodiceGenetico.Mutacion, 12,
                    TipoBonus.OpcionEventoExtra, 1.0,
                    nivelMax: 1, nodoPrevio: "cg_m2"),

                new DefinicionNodoCodiceGenetico(
                    "cg_m4", "Cascada Epigenética",
                    "+12% efectividad de sinergias por nivel (refuerzo)",
                    TipoCodiceGenetico.Mutacion, 16,
                    TipoBonus.BonusEfectividadSinergias, 0.12,
                    nivelMax: 3, nodoPrevio: "cg_m3"),

                new DefinicionNodoCodiceGenetico(
                    "cg_m5", "Marcaje Memético",
                    "+10% EV/s global por nivel",
                    TipoCodiceGenetico.Mutacion, 20,
                    TipoBonus.MultiplicadorGlobalGen, 0.10,
                    nivelMax: 3, nodoPrevio: "cg_m4"),

                new DefinicionNodoCodiceGenetico(
                    "cg_m6", "Recombinación Rápida",
                    "-10% coste mejoras por nivel (multiplicativo al Fósil)",
                    TipoCodiceGenetico.Mutacion, 28,
                    TipoBonus.ReduccionCosteMejoras, 0.10,
                    nivelMax: 2, nodoPrevio: "cg_m5"),

                new DefinicionNodoCodiceGenetico(
                    "cg_m7", "Diversidad Radical",
                    "+25% efectividad de sinergias por nivel (capstone)",
                    TipoCodiceGenetico.Mutacion, 40,
                    TipoBonus.BonusEfectividadSinergias, 0.25,
                    nivelMax: 2, nodoPrevio: "cg_m6"),

                // ══════════════════════════════════════════════════════════════
                // SIMBIOSIS — balance de pilares y capstones (6 nodos)
                // ══════════════════════════════════════════════════════════════

                new DefinicionNodoCodiceGenetico(
                    "cg_s1", "Red Trófica",
                    "+8% EV/s cuando 3+ pilares están balanceados (nivel dentro del 50%)",
                    TipoCodiceGenetico.Simbiosis, 3,
                    TipoBonus.BonusPilaresBalanceados, 0.08,
                    nivelMax: 5),

                new DefinicionNodoCodiceGenetico(
                    "cg_s2", "Equilibrio Gaia",
                    "+12% EV/s cuando 4 pilares balanceados (refuerzo al cg_s1)",
                    TipoCodiceGenetico.Simbiosis, 8,
                    TipoBonus.BonusPilaresBalanceados, 0.12,
                    nivelMax: 3, nodoPrevio: "cg_s1"),

                new DefinicionNodoCodiceGenetico(
                    "cg_s3", "Coevolución",
                    "+12% EV/s global por nivel",
                    TipoCodiceGenetico.Simbiosis, 14,
                    TipoBonus.MultiplicadorGlobalGen, 0.12,
                    nivelMax: 3, nodoPrevio: "cg_s2"),

                new DefinicionNodoCodiceGenetico(
                    "cg_s4", "Redundancia Vital",
                    "+15% a multiplicadores de Bifurcación por nivel",
                    TipoCodiceGenetico.Simbiosis, 20,
                    TipoBonus.BonusMultiplicadoresBifurcacion, 0.15,
                    nivelMax: 3, nodoPrevio: "cg_s3"),

                new DefinicionNodoCodiceGenetico(
                    "cg_s5", "Holobionte",
                    "+20% cap cadenas por nivel (multiplicativo refuerzo)",
                    TipoCodiceGenetico.Simbiosis, 28,
                    TipoBonus.BonusCapCadenaGen, 0.20,
                    nivelMax: 2, nodoPrevio: "cg_s4"),

                new DefinicionNodoCodiceGenetico(
                    "cg_s6", "Gaia Plena",
                    "+25% EV/s global (capstone absoluto)",
                    TipoCodiceGenetico.Simbiosis, 45,
                    TipoBonus.MultiplicadorGlobalGen, 0.25,
                    nivelMax: 2, nodoPrevio: "cg_s5"),
            };
        }
    }
}
