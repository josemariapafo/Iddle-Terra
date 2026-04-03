using Terra.Core;

namespace Terra.Data.Catalogos
{
    /// <summary>
    /// 48 sub-mejoras de cadena: 4 pilares × 3 eslabones × 4 niveles.
    /// Cada pilar se desbloquea en una era distinta.
    /// Producción efectiva = min(generación, procesamiento, distribución).
    /// </summary>
    public static class CatalogoCadenas
    {
        public static DefinicionSubMejoraCadena[] Crear()
        {
            return new[]
            {
                // ══════════════════════════════════════════════════════════════
                // TIERRA (Era 1)
                // ══════════════════════════════════════════════════════════════

                // ── Extracción Mineral (Generación) ──────────────────────────
                new DefinicionSubMejoraCadena("tc_tie_g1", "Recogida Manual",
                    "Recoges minerales a mano de la superficie",
                    TipoPilar.Tierra, TipoEslabon.Generacion,
                    3, 15, 100, 0, 1),
                new DefinicionSubMejoraCadena("tc_tie_g2", "Excavación Profunda",
                    "Picos y túneles alcanzan capas más ricas",
                    TipoPilar.Tierra, TipoEslabon.Generacion,
                    15, 200, 80, 10, 1),
                new DefinicionSubMejoraCadena("tc_tie_g3", "Perforación Sísmica",
                    "Ondas sísmicas revelan depósitos ocultos",
                    TipoPilar.Tierra, TipoEslabon.Generacion,
                    80, 3_000, 60, 30, 1),
                new DefinicionSubMejoraCadena("tc_tie_g4", "Extractor de Manto",
                    "Máquinas perforan hasta el manto terrestre",
                    TipoPilar.Tierra, TipoEslabon.Generacion,
                    500, 50_000, 40, 60, 1),

                // ── Presión Tectónica (Procesamiento) ────────────────────────
                new DefinicionSubMejoraCadena("tc_tie_p1", "Plumas Térmicas",
                    "El calor interno empuja material hacia arriba",
                    TipoPilar.Tierra, TipoEslabon.Procesamiento,
                    2.5, 12, 100, 0, 1),
                new DefinicionSubMejoraCadena("tc_tie_p2", "Subducción Activa",
                    "Una placa se hunde bajo otra reciclando roca",
                    TipoPilar.Tierra, TipoEslabon.Procesamiento,
                    12, 150, 80, 10, 1),
                new DefinicionSubMejoraCadena("tc_tie_p3", "Compresión Continental",
                    "Las placas colisionan transformando minerales",
                    TipoPilar.Tierra, TipoEslabon.Procesamiento,
                    70, 2_500, 60, 30, 1),
                new DefinicionSubMejoraCadena("tc_tie_p4", "Motor Tectónico",
                    "Control total del ciclo de convección del manto",
                    TipoPilar.Tierra, TipoEslabon.Procesamiento,
                    450, 40_000, 40, 60, 1),

                // ── Erosión y Sedimentación (Distribución) ───────────────────
                new DefinicionSubMejoraCadena("tc_tie_d1", "Desgaste Básico",
                    "Viento y agua descomponen la roca lentamente",
                    TipoPilar.Tierra, TipoEslabon.Distribucion,
                    4, 20, 100, 0, 1),
                new DefinicionSubMejoraCadena("tc_tie_d2", "Ríos Primordiales",
                    "Cauces de agua transportan sedimentos",
                    TipoPilar.Tierra, TipoEslabon.Distribucion,
                    18, 250, 80, 10, 1),
                new DefinicionSubMejoraCadena("tc_tie_d3", "Delta Fértil",
                    "Los deltas acumulan nutrientes en la costa",
                    TipoPilar.Tierra, TipoEslabon.Distribucion,
                    90, 4_000, 60, 30, 1),
                new DefinicionSubMejoraCadena("tc_tie_d4", "Ciclo Sedimentario",
                    "Ciclo completo de erosión-transporte-deposición",
                    TipoPilar.Tierra, TipoEslabon.Distribucion,
                    550, 60_000, 40, 60, 1),

                // ══════════════════════════════════════════════════════════════
                // OCÉANOS (Era 2)
                // ══════════════════════════════════════════════════════════════

                // ── Volumen (Generación) ─────────────────────────────────────
                new DefinicionSubMejoraCadena("tc_oce_g1", "Charcos",
                    "Pequeñas acumulaciones de agua en la superficie",
                    TipoPilar.Oceanos, TipoEslabon.Generacion,
                    3.5, 25, 100, 0, 2),
                new DefinicionSubMejoraCadena("tc_oce_g2", "Mares Interiores",
                    "Grandes cuencas se llenan de agua salada",
                    TipoPilar.Oceanos, TipoEslabon.Generacion,
                    18, 350, 80, 10, 2),
                new DefinicionSubMejoraCadena("tc_oce_g3", "Océano Global",
                    "Un océano continuo cubre la mayor parte del planeta",
                    TipoPilar.Oceanos, TipoEslabon.Generacion,
                    95, 5_000, 60, 30, 2),
                new DefinicionSubMejoraCadena("tc_oce_g4", "Abismo",
                    "Las fosas más profundas alcanzan presiones extremas",
                    TipoPilar.Oceanos, TipoEslabon.Generacion,
                    600, 80_000, 40, 60, 2),

                // ── Corrientes (Procesamiento) ───────────────────────────────
                new DefinicionSubMejoraCadena("tc_oce_p1", "Mareas Básicas",
                    "La gravedad lunar mueve masas de agua",
                    TipoPilar.Oceanos, TipoEslabon.Procesamiento,
                    3, 20, 100, 0, 2),
                new DefinicionSubMejoraCadena("tc_oce_p2", "Giros Oceánicos",
                    "Corrientes circulares redistribuyen calor",
                    TipoPilar.Oceanos, TipoEslabon.Procesamiento,
                    14, 280, 80, 10, 2),
                new DefinicionSubMejoraCadena("tc_oce_p3", "Termohalina",
                    "Diferencias de sal y temperatura mueven el océano profundo",
                    TipoPilar.Oceanos, TipoEslabon.Procesamiento,
                    75, 4_200, 60, 30, 2),
                new DefinicionSubMejoraCadena("tc_oce_p4", "Circulación Abisal",
                    "Corriente global que conecta todos los océanos",
                    TipoPilar.Oceanos, TipoEslabon.Procesamiento,
                    480, 65_000, 40, 60, 2),

                // ── Profundidad (Distribución) ───────────────────────────────
                new DefinicionSubMejoraCadena("tc_oce_d1", "Zona Fótica",
                    "La luz penetra los primeros metros del agua",
                    TipoPilar.Oceanos, TipoEslabon.Distribucion,
                    4.5, 30, 100, 0, 2),
                new DefinicionSubMejoraCadena("tc_oce_d2", "Mesopelágica",
                    "La zona crepuscular donde la luz se desvanece",
                    TipoPilar.Oceanos, TipoEslabon.Distribucion,
                    20, 400, 80, 10, 2),
                new DefinicionSubMejoraCadena("tc_oce_d3", "Batial",
                    "Profundidades oscuras con vida extremófila",
                    TipoPilar.Oceanos, TipoEslabon.Distribucion,
                    100, 6_000, 60, 30, 2),
                new DefinicionSubMejoraCadena("tc_oce_d4", "Abisal",
                    "El fondo marino más profundo y misterioso",
                    TipoPilar.Oceanos, TipoEslabon.Distribucion,
                    650, 90_000, 40, 60, 2),

                // ══════════════════════════════════════════════════════════════
                // ATMÓSFERA (Era 3)
                // ══════════════════════════════════════════════════════════════

                // ── Composición (Generación) ─────────────────────────────────
                new DefinicionSubMejoraCadena("tc_atm_g1", "Desgasificación",
                    "Gases escapan del interior del planeta",
                    TipoPilar.Atmosfera, TipoEslabon.Generacion,
                    4, 40, 100, 0, 3),
                new DefinicionSubMejoraCadena("tc_atm_g2", "Volcanismo",
                    "Erupciones liberan CO2, SO2 y vapor masivamente",
                    TipoPilar.Atmosfera, TipoEslabon.Generacion,
                    20, 550, 80, 10, 3),
                new DefinicionSubMejoraCadena("tc_atm_g3", "Fotólisis",
                    "La radiación UV descompone moléculas generando O2",
                    TipoPilar.Atmosfera, TipoEslabon.Generacion,
                    110, 8_000, 60, 30, 3),
                new DefinicionSubMejoraCadena("tc_atm_g4", "Regulación",
                    "Ciclos de retroalimentación estabilizan los gases",
                    TipoPilar.Atmosfera, TipoEslabon.Generacion,
                    700, 120_000, 40, 60, 3),

                // ── Circulación (Procesamiento) ──────────────────────────────
                new DefinicionSubMejoraCadena("tc_atm_p1", "Brisas",
                    "Movimientos de aire locales por diferencia de temperatura",
                    TipoPilar.Atmosfera, TipoEslabon.Procesamiento,
                    3.5, 35, 100, 0, 3),
                new DefinicionSubMejoraCadena("tc_atm_p2", "Celdas de Hadley",
                    "Circulación convectiva entre ecuador y trópicos",
                    TipoPilar.Atmosfera, TipoEslabon.Procesamiento,
                    16, 450, 80, 10, 3),
                new DefinicionSubMejoraCadena("tc_atm_p3", "Jet Stream",
                    "Corrientes de alta velocidad en la tropopausa",
                    TipoPilar.Atmosfera, TipoEslabon.Procesamiento,
                    85, 6_500, 60, 30, 3),
                new DefinicionSubMejoraCadena("tc_atm_p4", "Circulación Global",
                    "Sistema completo de redistribución de calor atmosférico",
                    TipoPilar.Atmosfera, TipoEslabon.Procesamiento,
                    550, 100_000, 40, 60, 3),

                // ── Densidad (Distribución) ──────────────────────────────────
                new DefinicionSubMejoraCadena("tc_atm_d1", "Capa Fina",
                    "Una delgada capa de gas rodea el planeta",
                    TipoPilar.Atmosfera, TipoEslabon.Distribucion,
                    5, 50, 100, 0, 3),
                new DefinicionSubMejoraCadena("tc_atm_d2", "Troposfera",
                    "Capa densa donde ocurre el clima",
                    TipoPilar.Atmosfera, TipoEslabon.Distribucion,
                    22, 650, 80, 10, 3),
                new DefinicionSubMejoraCadena("tc_atm_d3", "Estratosfera",
                    "Capa protectora con la capa de ozono",
                    TipoPilar.Atmosfera, TipoEslabon.Distribucion,
                    120, 9_500, 60, 30, 3),
                new DefinicionSubMejoraCadena("tc_atm_d4", "Magnetosfera",
                    "Escudo magnético que protege toda la atmósfera",
                    TipoPilar.Atmosfera, TipoEslabon.Distribucion,
                    750, 140_000, 40, 60, 3),

                // ══════════════════════════════════════════════════════════════
                // VIDA (Era 4)
                // ══════════════════════════════════════════════════════════════

                // ── Biomasa (Generación) ─────────────────────────────────────
                new DefinicionSubMejoraCadena("tc_vid_g1", "Caldo Primordial",
                    "Moléculas orgánicas se forman en las aguas cálidas",
                    TipoPilar.Vida, TipoEslabon.Generacion,
                    4.5, 60, 100, 0, 4),
                new DefinicionSubMejoraCadena("tc_vid_g2", "Estromatolitos",
                    "Capas de cianobacterias producen oxígeno y biomasa",
                    TipoPilar.Vida, TipoEslabon.Generacion,
                    22, 800, 80, 10, 4),
                new DefinicionSubMejoraCadena("tc_vid_g3", "Bosques",
                    "Ecosistemas terrestres masivos generan biomasa",
                    TipoPilar.Vida, TipoEslabon.Generacion,
                    120, 12_000, 60, 30, 4),
                new DefinicionSubMejoraCadena("tc_vid_g4", "Megafauna",
                    "Animales gigantes dominan todos los ecosistemas",
                    TipoPilar.Vida, TipoEslabon.Generacion,
                    750, 180_000, 40, 60, 4),

                // ── Cadena Trófica (Procesamiento) ───────────────────────────
                new DefinicionSubMejoraCadena("tc_vid_p1", "Autótrofos",
                    "Organismos que producen su propio alimento",
                    TipoPilar.Vida, TipoEslabon.Procesamiento,
                    4, 50, 100, 0, 4),
                new DefinicionSubMejoraCadena("tc_vid_p2", "Herbívoros",
                    "Consumidores primarios que procesan plantas",
                    TipoPilar.Vida, TipoEslabon.Procesamiento,
                    18, 700, 80, 10, 4),
                new DefinicionSubMejoraCadena("tc_vid_p3", "Depredadores",
                    "Cazadores que regulan las poblaciones",
                    TipoPilar.Vida, TipoEslabon.Procesamiento,
                    95, 10_000, 60, 30, 4),
                new DefinicionSubMejoraCadena("tc_vid_p4", "Apex",
                    "Depredadores supremos sin rivales naturales",
                    TipoPilar.Vida, TipoEslabon.Procesamiento,
                    600, 150_000, 40, 60, 4),

                // ── Diversidad (Distribución) ────────────────────────────────
                new DefinicionSubMejoraCadena("tc_vid_d1", "Monocultivo",
                    "Una sola especie domina cada nicho",
                    TipoPilar.Vida, TipoEslabon.Distribucion,
                    5, 70, 100, 0, 4),
                new DefinicionSubMejoraCadena("tc_vid_d2", "Nicho Doble",
                    "Dos especies coexisten compartiendo recursos",
                    TipoPilar.Vida, TipoEslabon.Distribucion,
                    24, 900, 80, 10, 4),
                new DefinicionSubMejoraCadena("tc_vid_d3", "Red Ecológica",
                    "Múltiples especies interconectadas en redes complejas",
                    TipoPilar.Vida, TipoEslabon.Distribucion,
                    130, 13_000, 60, 30, 4),
                new DefinicionSubMejoraCadena("tc_vid_d4", "Simbiosis Global",
                    "Todas las especies cooperan en un ecosistema planetario",
                    TipoPilar.Vida, TipoEslabon.Distribucion,
                    800, 200_000, 40, 60, 4),
            };
        }
    }
}
