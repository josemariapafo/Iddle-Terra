using Terra.Core;

namespace Terra.Data.Catalogos
{
    /// <summary>
    /// ~30 misiones progresivas. Cada cadena se desbloquea al completar la anterior.
    /// 3 misiones activas simultáneas, elegidas del pool disponible.
    /// </summary>
    public static class CatalogoMisiones
    {
        public static DefinicionMision[] Crear()
        {
            return new[]
            {
                // ══════════════════════════════════════════════════════════════
                // COMPRA — cadena general
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_comp_01", "Primeros pasos",
                    "Compra 5 mejoras",
                    TipoMision.Compra, 5,
                    TipoRecompensa.EVInstante, 30, 1, "mis_comp_02"),

                new DefinicionMision("mis_comp_02", "Inversor novato",
                    "Compra 15 mejoras",
                    TipoMision.Compra, 15,
                    TipoRecompensa.MultiplicadorTemporal, 2, 1, "mis_comp_03"),

                new DefinicionMision("mis_comp_03", "Comprador compulsivo",
                    "Compra 50 mejoras",
                    TipoMision.Compra, 50,
                    TipoRecompensa.FosilesExtra, 2, 1, "mis_comp_04"),

                new DefinicionMision("mis_comp_04", "Magnate planetario",
                    "Compra 150 mejoras",
                    TipoMision.Compra, 150,
                    TipoRecompensa.EVInstante, 60, 2, "mis_comp_05"),

                new DefinicionMision("mis_comp_05", "Arquitecto del mundo",
                    "Compra 500 mejoras",
                    TipoMision.Compra, 500,
                    TipoRecompensa.NivelNodoGratis, 1, 3),

                // ══════════════════════════════════════════════════════════════
                // COMPRA — cadena Tierra
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_ctie_01", "Geólogo novato",
                    "Compra 3 mejoras de Tierra",
                    TipoMision.Compra, 3,
                    TipoRecompensa.EVInstante, 20, 1, "mis_ctie_02"),

                new DefinicionMision("mis_ctie_02", "Maestro tectónico",
                    "Compra 10 mejoras de Tierra",
                    TipoMision.Compra, 10,
                    TipoRecompensa.MultiplicadorTemporal, 2, 1),

                // ══════════════════════════════════════════════════════════════
                // COMPRA — cadena Océanos
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_coce_01", "Marinero",
                    "Compra 3 mejoras de Océanos",
                    TipoMision.Compra, 3,
                    TipoRecompensa.EVInstante, 20, 1, "mis_coce_02"),

                new DefinicionMision("mis_coce_02", "Señor de los mares",
                    "Compra 10 mejoras de Océanos",
                    TipoMision.Compra, 10,
                    TipoRecompensa.MultiplicadorTemporal, 2, 2),

                // ══════════════════════════════════════════════════════════════
                // PRODUCCIÓN
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_prod_01", "Chispa inicial",
                    "Alcanza 10 EV/s",
                    TipoMision.Produccion, 10,
                    TipoRecompensa.EVInstante, 30, 1, "mis_prod_02"),

                new DefinicionMision("mis_prod_02", "Motor encendido",
                    "Alcanza 100 EV/s",
                    TipoMision.Produccion, 100,
                    TipoRecompensa.EVInstante, 30, 1, "mis_prod_03"),

                new DefinicionMision("mis_prod_03", "Energía desbordante",
                    "Alcanza 1.000 EV/s",
                    TipoMision.Produccion, 1_000,
                    TipoRecompensa.MultiplicadorTemporal, 2, 2, "mis_prod_04"),

                new DefinicionMision("mis_prod_04", "Central planetaria",
                    "Alcanza 10.000 EV/s",
                    TipoMision.Produccion, 10_000,
                    TipoRecompensa.FosilesExtra, 3, 3, "mis_prod_05"),

                new DefinicionMision("mis_prod_05", "Supernova",
                    "Alcanza 100.000 EV/s",
                    TipoMision.Produccion, 100_000,
                    TipoRecompensa.NivelNodoGratis, 1, 4, "mis_prod_06"),

                new DefinicionMision("mis_prod_06", "Dios de la energía",
                    "Alcanza 1.000.000 EV/s",
                    TipoMision.Produccion, 1_000_000,
                    TipoRecompensa.FosilesExtra, 5, 5),

                // ══════════════════════════════════════════════════════════════
                // SINERGIA
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_sin_01", "Primera conexión",
                    "Desbloquea tu primera sinergia",
                    TipoMision.Sinergia, 1,
                    TipoRecompensa.EVInstante, 60, 1, "mis_sin_02"),

                new DefinicionMision("mis_sin_02", "Red emergente",
                    "Desbloquea 3 sinergias",
                    TipoMision.Sinergia, 3,
                    TipoRecompensa.MultiplicadorTemporal, 3, 2, "mis_sin_03"),

                new DefinicionMision("mis_sin_03", "Armonía planetaria",
                    "Desbloquea 6 sinergias",
                    TipoMision.Sinergia, 6,
                    TipoRecompensa.FosilesExtra, 5, 3),

                // ══════════════════════════════════════════════════════════════
                // ERA
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_era_01", "Evolución",
                    "Avanza a Era 2",
                    TipoMision.Era, 2,
                    TipoRecompensa.EVInstante, 60, 1, "mis_era_02"),

                new DefinicionMision("mis_era_02", "Explosión cámbrica",
                    "Avanza a Era 3",
                    TipoMision.Era, 3,
                    TipoRecompensa.MultiplicadorTemporal, 3, 2, "mis_era_03"),

                new DefinicionMision("mis_era_03", "Pangea",
                    "Avanza a Era 4",
                    TipoMision.Era, 4,
                    TipoRecompensa.NivelNodoGratis, 1, 3),

                // ══════════════════════════════════════════════════════════════
                // PRESTIGE
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_pre_01", "Superviviente",
                    "Haz tu primera Extinción",
                    TipoMision.Prestige, 1,
                    TipoRecompensa.FosilesExtra, 3, 3, "mis_pre_02"),

                new DefinicionMision("mis_pre_02", "Coleccionista fósil",
                    "Acumula 10 Fósiles",
                    TipoMision.Prestige, 10,
                    TipoRecompensa.MultiplicadorTemporal, 3, 3, "mis_pre_03"),

                new DefinicionMision("mis_pre_03", "Ciclo eterno",
                    "Haz 5 prestiges en total",
                    TipoMision.Prestige, 5,
                    TipoRecompensa.FosilesExtra, 10, 4),

                // ══════════════════════════════════════════════════════════════
                // COMBO (TAP)
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_cmb_01", "Primer pulso",
                    "Activa 1 combo",
                    TipoMision.Combo, 1,
                    TipoRecompensa.EVInstante, 20, 1, "mis_cmb_02"),

                new DefinicionMision("mis_cmb_02", "Dedos ágiles",
                    "Activa 3 combos",
                    TipoMision.Combo, 3,
                    TipoRecompensa.MultiplicadorTemporal, 2, 1, "mis_cmb_03"),

                new DefinicionMision("mis_cmb_03", "Furia sísmica",
                    "Activa 10 combos",
                    TipoMision.Combo, 10,
                    TipoRecompensa.FosilesExtra, 2, 2),

                // ══════════════════════════════════════════════════════════════
                // CUELLO DE BOTELLA
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_bot_01", "Optimizador",
                    "Compra 5 mejoras de infraestructura",
                    TipoMision.CuelloBotella, 5,
                    TipoRecompensa.EVInstante, 30, 1, "mis_bot_02"),

                new DefinicionMision("mis_bot_02", "Ingeniero jefe",
                    "Compra 20 mejoras de infraestructura",
                    TipoMision.CuelloBotella, 20,
                    TipoRecompensa.MultiplicadorTemporal, 2, 2, "mis_bot_03"),

                new DefinicionMision("mis_bot_03", "Maestro logístico",
                    "Compra 50 mejoras de infraestructura",
                    TipoMision.CuelloBotella, 50,
                    TipoRecompensa.FosilesExtra, 5, 3),
            };
        }
    }
}
