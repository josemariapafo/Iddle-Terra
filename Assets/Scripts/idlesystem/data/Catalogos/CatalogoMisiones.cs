using Terra.Core;

namespace Terra.Data.Catalogos
{
    /// <summary>
    /// ~30 misiones progresivas. Cada cadena se desbloquea al completar la anterior.
    /// 3 misiones activas simultáneas, elegidas del pool disponible.
    /// Recompensas: todas EVInstante (ValorRecompensa = segundos de producción actual).
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
                    "Compra 5 mejoras de cualquier pilar. Pulsa un pilar para ver sus mejoras.",
                    TipoMision.Compra, 5,
                    TipoRecompensa.EVInstante, 30, 1, "mis_comp_02"),

                new DefinicionMision("mis_comp_02", "Inversor novato",
                    "Compra 15 mejoras de cualquier pilar. Sube de nivel las que ya tienes.",
                    TipoMision.Compra, 15,
                    TipoRecompensa.EVInstante, 60, 1, "mis_comp_03"),

                new DefinicionMision("mis_comp_03", "Comprador compulsivo",
                    "Compra 50 mejoras en total. Usa los botones x10 o MAX para ir mas rapido.",
                    TipoMision.Compra, 50,
                    TipoRecompensa.EVInstante, 120, 1, "mis_comp_04"),

                new DefinicionMision("mis_comp_04", "Magnate planetario",
                    "Compra 150 mejoras en total. Reparte entre todos los pilares disponibles.",
                    TipoMision.Compra, 150,
                    TipoRecompensa.EVInstante, 180, 2, "mis_comp_05"),

                new DefinicionMision("mis_comp_05", "Arquitecto del mundo",
                    "Compra 500 mejoras en total. Cada pilar tiene 3 zonas con mejoras distintas.",
                    TipoMision.Compra, 500,
                    TipoRecompensa.EVInstante, 300, 3),

                // ══════════════════════════════════════════════════════════════
                // COMPRA — cadena Tierra
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_ctie_01", "Geologo novato",
                    "Compra 3 mejoras del pilar TIERRA. Pulsa el boton Tierra en el menu principal.",
                    TipoMision.Compra, 3,
                    TipoRecompensa.EVInstante, 20, 1, "mis_ctie_02"),

                new DefinicionMision("mis_ctie_02", "Maestro tectonico",
                    "Compra 10 mejoras del pilar TIERRA. Mejora las 3 zonas: superficie, corteza e interior.",
                    TipoMision.Compra, 10,
                    TipoRecompensa.EVInstante, 60, 1),

                // ══════════════════════════════════════════════════════════════
                // COMPRA — cadena Océanos
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_coce_01", "Marinero",
                    "Compra 3 mejoras del pilar OCEANOS. Se desbloquea en Era 2.",
                    TipoMision.Compra, 3,
                    TipoRecompensa.EVInstante, 20, 2, "mis_coce_02"),

                new DefinicionMision("mis_coce_02", "Senor de los mares",
                    "Compra 10 mejoras del pilar OCEANOS. Mejora superficie, zona media y profundidades.",
                    TipoMision.Compra, 10,
                    TipoRecompensa.EVInstante, 60, 2),

                // ══════════════════════════════════════════════════════════════
                // PRODUCCIÓN
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_prod_01", "Chispa inicial",
                    "Tu produccion debe llegar a 10 EV/s. Compra mejoras para aumentarla.",
                    TipoMision.Produccion, 10,
                    TipoRecompensa.EVInstante, 30, 1, "mis_prod_02"),

                new DefinicionMision("mis_prod_02", "Motor encendido",
                    "Alcanza 100 EV/s. Sube varias mejoras de nivel y revisa la infraestructura.",
                    TipoMision.Produccion, 100,
                    TipoRecompensa.EVInstante, 60, 1, "mis_prod_03"),

                new DefinicionMision("mis_prod_03", "Energia desbordante",
                    "Alcanza 1.000 EV/s. Si tu produccion esta capada, mejora la infraestructura del pilar.",
                    TipoMision.Produccion, 1_000,
                    TipoRecompensa.EVInstante, 120, 2, "mis_prod_04"),

                new DefinicionMision("mis_prod_04", "Central planetaria",
                    "Alcanza 10.000 EV/s. Desbloquea sinergias entre pilares para multiplicar tu EV.",
                    TipoMision.Produccion, 10_000,
                    TipoRecompensa.EVInstante, 180, 3, "mis_prod_05"),

                new DefinicionMision("mis_prod_05", "Supernova",
                    "Alcanza 100.000 EV/s. Usa el arbol de evolucion y prestiges para potenciarte.",
                    TipoMision.Produccion, 100_000,
                    TipoRecompensa.EVInstante, 300, 4, "mis_prod_06"),

                new DefinicionMision("mis_prod_06", "Dios de la energia",
                    "Alcanza 1.000.000 EV/s. Necesitaras varios prestiges y todos los pilares al maximo.",
                    TipoMision.Produccion, 1_000_000,
                    TipoRecompensa.EVInstante, 600, 5),

                // ══════════════════════════════════════════════════════════════
                // SINERGIA
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_sin_01", "Primera conexion",
                    "Las sinergias se activan al subir dos pilares a cierto nivel. Sube Tierra y otro pilar.",
                    TipoMision.Sinergia, 1,
                    TipoRecompensa.EVInstante, 60, 2, "mis_sin_02"),

                new DefinicionMision("mis_sin_02", "Red emergente",
                    "Desbloquea 3 sinergias. Cada par de pilares tiene una sinergia distinta.",
                    TipoMision.Sinergia, 3,
                    TipoRecompensa.EVInstante, 120, 3, "mis_sin_03"),

                new DefinicionMision("mis_sin_03", "Armonia planetaria",
                    "Desbloquea 6 sinergias. Sube todos los pilares para activar mas conexiones.",
                    TipoMision.Sinergia, 6,
                    TipoRecompensa.EVInstante, 240, 4),

                // ══════════════════════════════════════════════════════════════
                // ERA
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_era_01", "Evolucion",
                    "Avanza a Era 2. Sube Tierra a nivel 4 y acumula 2.000 EV.",
                    TipoMision.Era, 2,
                    TipoRecompensa.EVInstante, 60, 1, "mis_era_02"),

                new DefinicionMision("mis_era_02", "Explosion cambrica",
                    "Avanza a Era 3. Necesitas nivel 10 en cada pilar y la sinergia Erosion fertil.",
                    TipoMision.Era, 3,
                    TipoRecompensa.EVInstante, 120, 2, "mis_era_03"),

                new DefinicionMision("mis_era_03", "Pangea",
                    "Avanza a Era 4. Necesitas nivel 25 en cada pilar y la sinergia Ciclo del agua.",
                    TipoMision.Era, 4,
                    TipoRecompensa.EVInstante, 240, 3),

                // ══════════════════════════════════════════════════════════════
                // PRESTIGE
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_pre_01", "Superviviente",
                    "Haz tu primera Extincion. Abre el panel Prestige cuando estes en Era 3+.",
                    TipoMision.Prestige, 1,
                    TipoRecompensa.EVInstante, 120, 3, "mis_pre_02"),

                new DefinicionMision("mis_pre_02", "Coleccionista fosil",
                    "Acumula 10 Fosiles. Haz mas Extinciones — cada una da Fosiles segun tu progreso.",
                    TipoMision.Prestige, 10,
                    TipoRecompensa.EVInstante, 180, 3, "mis_pre_03"),

                new DefinicionMision("mis_pre_03", "Ciclo eterno",
                    "Haz 5 prestiges en total. Cada prestige te hace mas fuerte permanentemente.",
                    TipoMision.Prestige, 5,
                    TipoRecompensa.EVInstante, 300, 4),

                // ══════════════════════════════════════════════════════════════
                // COMBO (TAP)
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_cmb_01", "Primer pulso",
                    "Toca el planeta 5 veces seguidas para activar un combo. El combo multiplica tu EV.",
                    TipoMision.Combo, 1,
                    TipoRecompensa.EVInstante, 20, 1, "mis_cmb_02"),

                new DefinicionMision("mis_cmb_02", "Dedos agiles",
                    "Activa 3 combos. Toca el planeta rapido — 5 toques en menos de 3 segundos.",
                    TipoMision.Combo, 3,
                    TipoRecompensa.EVInstante, 60, 1, "mis_cmb_03"),

                new DefinicionMision("mis_cmb_03", "Furia sismica",
                    "Activa 10 combos. Cada combo dura 10 segundos de produccion multiplicada.",
                    TipoMision.Combo, 10,
                    TipoRecompensa.EVInstante, 120, 2),

                // ══════════════════════════════════════════════════════════════
                // CUELLO DE BOTELLA
                // ══════════════════════════════════════════════════════════════
                new DefinicionMision("mis_bot_01", "Optimizador",
                    "Compra 5 mejoras de infraestructura. Abre un pilar y pulsa la pestana Infraestructura.",
                    TipoMision.CuelloBotella, 5,
                    TipoRecompensa.EVInstante, 30, 1, "mis_bot_02"),

                new DefinicionMision("mis_bot_02", "Ingeniero jefe",
                    "Compra 20 mejoras de infraestructura. Mejora el eslabon que dice LIMITA en rojo.",
                    TipoMision.CuelloBotella, 20,
                    TipoRecompensa.EVInstante, 90, 2, "mis_bot_03"),

                new DefinicionMision("mis_bot_03", "Maestro logistico",
                    "Compra 50 mejoras de infraestructura. Equilibra los 3 eslabones de cada pilar.",
                    TipoMision.CuelloBotella, 50,
                    TipoRecompensa.EVInstante, 180, 3),
            };
        }
    }
}
