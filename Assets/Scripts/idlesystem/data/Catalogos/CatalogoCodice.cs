using Terra.Core;

namespace Terra.Data.Catalogos
{
    /// <summary>
    /// 15 nodos del Códice Fósil: 5 por rama, cadena lineal.
    /// Diseño conservador: bonuses mayoritariamente aditivos,
    /// fácil de balancear y ajustar en T27.
    ///
    /// Costes pensados para la curva de fósiles de Extinción:
    ///   ~2 fósiles a 40K EV, ~4 a 200K, ~8 a 800K.
    ///   Raíz (3 fósiles) = accesible tras 1-2 prestiges.
    ///   Capstone (~30 fósiles) = requiere ~6+ prestiges.
    ///   MultCoste x2 por nivel: nivel 1 = coste, nivel 2 = x2, nivel 3 = x4...
    /// </summary>
    public static class CatalogoCodice
    {
        public static DefinicionNodoCodice[] Crear()
        {
            return new[]
            {
                // ══════════════════════════════════════════════════════════════
                // ABUNDANCIA — producción pasiva
                // ══════════════════════════════════════════════════════════════

                new DefinicionNodoCodice(
                    "cf_a1", "Raíces Profundas",
                    "+10% EV/s por nivel",
                    TipoCodice.Abundancia, 3,
                    TipoBonus.MultiplicadorEV, 0.10,
                    nivelMax: 5),

                new DefinicionNodoCodice(
                    "cf_a2", "Erupción Perpetua",
                    "+25% producción nocturna por nivel",
                    TipoCodice.Abundancia, 6,
                    TipoBonus.BonusNocturno, 0.25,
                    nivelMax: 3, nodoPrevio: "cf_a1"),

                new DefinicionNodoCodice(
                    "cf_a3", "Mareas Ancestrales",
                    "+8% bonus sinergias por nivel",
                    TipoCodice.Abundancia, 10,
                    TipoBonus.BonusSinergias, 0.08,
                    nivelMax: 5, nodoPrevio: "cf_a2"),

                new DefinicionNodoCodice(
                    "cf_a4", "Pulso Vital",
                    "+15% EV/s por nivel",
                    TipoCodice.Abundancia, 20,
                    TipoBonus.MultiplicadorEV, 0.15,
                    nivelMax: 3, nodoPrevio: "cf_a3"),

                new DefinicionNodoCodice(
                    "cf_a5", "Gaia Menor",
                    "+20% EV/s por nivel",
                    TipoCodice.Abundancia, 35,
                    TipoBonus.MultiplicadorEV, 0.20,
                    nivelMax: 2, nodoPrevio: "cf_a4"),

                // ══════════════════════════════════════════════════════════════
                // EFICIENCIA — economía y aceleración
                // ══════════════════════════════════════════════════════════════

                new DefinicionNodoCodice(
                    "cf_e1", "Memoria Geológica",
                    "-8% coste mejoras por nivel",
                    TipoCodice.Eficiencia, 3,
                    TipoBonus.ReduccionCosteMejoras, 0.08,
                    nivelMax: 5),

                new DefinicionNodoCodice(
                    "cf_e2", "Tectónica Acelerada",
                    "-10% coste cadenas por nivel",
                    TipoCodice.Eficiencia, 6,
                    TipoBonus.ReduccionCosteCadenas, 0.10,
                    nivelMax: 3, nodoPrevio: "cf_e1"),

                new DefinicionNodoCodice(
                    "cf_e3", "Erosión Rápida",
                    "+1 nivel gratis en mejoras Era 1 tras prestige",
                    TipoCodice.Eficiencia, 12,
                    TipoBonus.NivelesGratisInicio, 1.0,
                    nivelMax: 3, nodoPrevio: "cf_e2"),

                new DefinicionNodoCodice(
                    "cf_e4", "Sedimentación",
                    "+15% fósiles ganados en prestige por nivel",
                    TipoCodice.Eficiencia, 18,
                    TipoBonus.BonusFosilesPrestige, 0.15,
                    nivelMax: 3, nodoPrevio: "cf_e3"),

                new DefinicionNodoCodice(
                    "cf_e5", "Estratificación",
                    "+15% cap de cadenas por nivel",
                    TipoCodice.Eficiencia, 30,
                    TipoBonus.BonusCapCadena, 0.15,
                    nivelMax: 3, nodoPrevio: "cf_e4"),

                // ══════════════════════════════════════════════════════════════
                // DOMINIO — tap y juego activo
                // ══════════════════════════════════════════════════════════════

                new DefinicionNodoCodice(
                    "cf_d1", "Impacto Cósmico",
                    "+30% poder de tap por nivel",
                    TipoCodice.Dominio, 3,
                    TipoBonus.BonusTap, 0.30,
                    nivelMax: 5),

                new DefinicionNodoCodice(
                    "cf_d2", "Combo Rápido",
                    "-1 tap para activar combo por nivel",
                    TipoCodice.Dominio, 8,
                    TipoBonus.ReduccionTapsCombo, 1.0,
                    nivelMax: 2, nodoPrevio: "cf_d1"),

                new DefinicionNodoCodice(
                    "cf_d3", "Pulso Prolongado",
                    "+3s duración de combo por nivel",
                    TipoCodice.Dominio, 6,
                    TipoBonus.DuracionCombo, 3.0,
                    nivelMax: 3, nodoPrevio: "cf_d2"),

                new DefinicionNodoCodice(
                    "cf_d4", "Resonancia",
                    "+0.25x multiplicador de combo por nivel",
                    TipoCodice.Dominio, 20,
                    TipoBonus.MultiplicadorCombo, 0.25,
                    nivelMax: 2, nodoPrevio: "cf_d3"),

                new DefinicionNodoCodice(
                    "cf_d5", "Auto-Impulso",
                    "1 tap automático por nivel (cada 10s/6s/3s)",
                    TipoCodice.Dominio, 25,
                    TipoBonus.AutoTap, 1.0,
                    nivelMax: 3, nodoPrevio: "cf_d4"),
            };
        }
    }
}
