using Terra.Core;

namespace Terra.Data.Catalogos
{
    /// <summary>
    /// Bifurcaciones evolutivas (T26). Una por pilar, disponibles en Era 6.
    ///
    /// Cada bifurcación enfrenta dos caminos opuestos que favorecen distintos
    /// eslabones de la cadena (Generación / Procesamiento / Distribución):
    ///
    ///   - Opción fuerte en un eslabón: ×1.50 cap
    ///   - Eslabón neutro: ×1.00
    ///   - Eslabón débil (compromiso): ×0.90
    ///
    /// El multiplicador se aplica al cap del eslabón correspondiente en
    /// SistemaCadenas.CalcularCapPilar(). El jugador debe comprometerse con
    /// UN camino por pilar — crea identidad mecánica y fuerza decisión.
    ///
    /// El Códice Genético (nodo Redundancia Vital / Evolución Dirigida) amplifica
    /// el exceso sobre 1.0, haciendo las elecciones más impactantes.
    /// </summary>
    public static class CatalogoBifurcaciones
    {
        public static DefinicionBifurcacion[] Crear()
        {
            return new[]
            {
                // ──────────────────────────────────────────────────────────
                // ATMÓSFERA — Densa (filtrado) vs Ligera (captura directa)
                // ──────────────────────────────────────────────────────────
                new DefinicionBifurcacion(
                    TipoPilar.Atmosfera,
                    "Atmósfera Densa",
                    "Capas de gases espesos filtran y catalizan: el procesamiento " +
                    "se potencia, pero la captura energética inicial se ralentiza.",
                    /* Gen  */ 0.90,
                    /* Proc */ 1.50,
                    /* Dist */ 1.00,
                    "Atmósfera Ligera",
                    "Aire transparente: la energía solar llega sin obstáculo, " +
                    "pero las reacciones de refinado son más lentas.",
                    /* Gen  */ 1.50,
                    /* Proc */ 0.90,
                    /* Dist */ 1.00),

                // ──────────────────────────────────────────────────────────
                // OCÉANOS — Abisal (distribución) vs Tropical (generación)
                // ──────────────────────────────────────────────────────────
                new DefinicionBifurcacion(
                    TipoPilar.Oceanos,
                    "Océanos Abisales",
                    "Corrientes profundas y frías: la red de distribución " +
                    "global se vuelve dominante, a costa de generación superficial.",
                    /* Gen  */ 0.90,
                    /* Proc */ 1.00,
                    /* Dist */ 1.50,
                    "Océanos Tropicales",
                    "Aguas cálidas y luminosas: fotosíntesis explosiva en superficie, " +
                    "pero las corrientes de distribución son débiles.",
                    /* Gen  */ 1.50,
                    /* Proc */ 1.00,
                    /* Dist */ 0.90),

                // ──────────────────────────────────────────────────────────
                // TIERRA — Volcánica (generación) vs Estable (procesamiento)
                // ──────────────────────────────────────────────────────────
                new DefinicionBifurcacion(
                    TipoPilar.Tierra,
                    "Tierra Volcánica",
                    "Geología activa: energía geotérmica masiva en bruto, " +
                    "pero los suelos inestables procesan con menor eficiencia.",
                    /* Gen  */ 1.50,
                    /* Proc */ 0.90,
                    /* Dist */ 1.00,
                    "Tierra Estable",
                    "Continentes calmados: suelos fértiles y ciclos minerales lentos " +
                    "que refinan todo, a costa de una generación bruta más pobre.",
                    /* Gen  */ 0.90,
                    /* Proc */ 1.50,
                    /* Dist */ 1.00),

                // ──────────────────────────────────────────────────────────
                // VIDA — Depredadora (generación) vs Cooperativa (distribución)
                // ──────────────────────────────────────────────────────────
                new DefinicionBifurcacion(
                    TipoPilar.Vida,
                    "Vida Depredadora",
                    "Cazadores agresivos: adquisición de energía rápida y brutal, " +
                    "pero la red trófica se rompe y la distribución sufre.",
                    /* Gen  */ 1.50,
                    /* Proc */ 1.00,
                    /* Dist */ 0.90,
                    "Vida Cooperativa",
                    "Simbiosis generalizadas: redes de vida que comparten recursos " +
                    "con eficiencia perfecta, a costa de generación menos agresiva.",
                    /* Gen  */ 0.90,
                    /* Proc */ 1.00,
                    /* Dist */ 1.50),
            };
        }
    }
}
