using System;
using Terra.Core;
using Terra.Data;

namespace Terra.Data.Catalogos
{
    public static class CatalogoSinergias
    {
        public static DefinicionSinergia[] Crear() => new[]
        {
            new DefinicionSinergia("sin_01","Ciclo del agua",       "+40% EV — Atmósfera y Océanos cooperan",       TipoPilar.Atmosfera,TipoPilar.Oceanos,  5,   5,  1.40),
            new DefinicionSinergia("sin_02","Ecosistema primario",  "+60% EV — Tierra nutre a la Vida",             TipoPilar.Tierra,   TipoPilar.Vida,     5,   5,  1.60),
            new DefinicionSinergia("sin_03","Fotosíntesis marina",  "+80% EV — Océanos y Vida producen O₂",        TipoPilar.Oceanos,  TipoPilar.Vida,    12,  12,  1.80),
            new DefinicionSinergia("sin_04","Erosión fértil",       "+50% Nutrientes — Tierra y Océanos",          TipoPilar.Tierra,   TipoPilar.Oceanos, 12,  10,  1.50),
            new DefinicionSinergia("sin_05","Selva tropical",       "×2 Biomasa — Tierra y Vida avanzadas",        TipoPilar.Tierra,   TipoPilar.Vida,    25,  22,  2.00),
            new DefinicionSinergia("sin_06","Termorregulación",     "+70% EV global — Atmósfera y Tierra",         TipoPilar.Atmosfera,TipoPilar.Tierra,  18,  15,  1.70),
            new DefinicionSinergia("sin_07","Gran Oxidación",       "×2.5 EV — Atmósfera y Vida producen O₂",     TipoPilar.Atmosfera,TipoPilar.Vida,    32,  28,  2.50),
            new DefinicionSinergia("sin_08","Biosfera completa",    "×2.2 todo — Océanos y Tierra conectados",    TipoPilar.Oceanos,  TipoPilar.Tierra,  40,  40,  2.20),
            new DefinicionSinergia("sin_09","Ciclo del carbono",    "×3 EV global — Atmósfera y Océanos",         TipoPilar.Atmosfera,TipoPilar.Oceanos, 55,  48,  3.00),
            new DefinicionSinergia("sin_10","Red trófica global",   "×2.8 Vida — Vida y Océanos interconectados", TipoPilar.Vida,     TipoPilar.Oceanos, 55,  52,  2.80),
            new DefinicionSinergia("sin_11","Geodiversidad",        "×3.5 Nutrientes — Tierra y Atmósfera",       TipoPilar.Tierra,   TipoPilar.Atmosfera,70, 62, 3.50),
            new DefinicionSinergia("sin_12","GAIA",                 "×5 TODO — Los 4 pilares en armonía",         TipoPilar.Atmosfera,TipoPilar.Vida,    110, 110, 5.00, cuatroPilares: true),
        };
    }

    public static class CatalogoEras
    {
        // Pilares disponibles por era:
        //   Era 1: Tierra
        //   Era 2: Tierra + Océanos
        //   Era 3: Tierra + Océanos + Atmósfera
        //   Era 4+: Los 4 pilares
        //
        // Las sinergias requeridas solo usan pilares YA disponibles:
        //   sin_04 = Tierra+Océanos      → posible desde Era 2
        //   sin_01 = Atmósfera+Océanos    → posible desde Era 3
        //   sin_06 = Atmósfera+Tierra     → posible desde Era 3
        //   sin_02 = Tierra+Vida          → posible desde Era 4
        //   sin_03 = Océanos+Vida         → posible desde Era 4
        public static DefinicionEra[] Crear() => new[]
        {
            new DefinicionEra(1, "Roca Primordial",       "Un planeta gris y silencioso emerge del caos",
                new CondicionEra(0, 0),
                "era1_earth_day", ""),

            // Era 1→2: solo Tierra disponible
            new DefinicionEra(2, "Primeros Océanos",      "El agua líquida cubre los primeros continentes",
                new CondicionEra(4, 2_000),
                "era2_earth_day", ""),

            // Era 2→3: Tierra+Océanos disponibles. sin_04 = Erosión fértil (Tierra+Océanos)
            new DefinicionEra(3, "Vida Marina",           "El mar explota con vida en la era Cámbrica",
                new CondicionEra(10, 40_000, new[]{"sin_04"}),
                "era3_earth_day", ""),

            // Era 3→4: T+O+A disponibles. sin_01 = Ciclo del agua (Atm+Oce)
            new DefinicionEra(4, "Pangea",                "Un supercontinente único domina el planeta",
                new CondicionEra(25, 800_000, new[]{"sin_01"}),
                "era4_earth_day", ""),

            // Era 4→5: los 4 pilares. sin_02 = Ecosistema primario (Tie+Vid), sin_03 = Fotosíntesis marina (Oce+Vid)
            new DefinicionEra(5, "Jurásico",              "Los dinosaurios reinan sobre un planeta verde",
                new CondicionEra(40, 8_000_000, new[]{"sin_02","sin_03"}),
                "era5_earth_day", ""),

            new DefinicionEra(6, "Civilización Primitiva","La humanidad da sus primeros pasos",
                new CondicionEra(65, 80_000_000, new[]{"sin_07"}),
                "earth_Day",     "earth_night"),

            new DefinicionEra(7, "Civilización Avanzada", "Las luces del progreso cubren el planeta",
                new CondicionEra(95, 800_000_000, new[]{"sin_09","sin_11"}),
                "era7_earth_day","era7_earth_night"),

            new DefinicionEra(8, "Era Espacial",          "La humanidad trasciende los límites del planeta",
                new CondicionEra(130, 8_000_000_000, new[]{"sin_12"}),
                "era8_earth_day","era8_earth_night"),
        };
    }

    public static class CatalogoNodos
    {
        public static DefinicionNodo[] Crear() => new[]
        {
            // Era 1
            new DefinicionNodo("nd_01","Metabolismo básico",    "+10% EV Vida",         1, null,           100,    TipoBonus.MultiplicadorPilar, TipoPilar.Vida,    0.10),
            new DefinicionNodo("nd_02","Minerales reactivos",   "+10% EV Tierra",       1, null,           100,    TipoBonus.MultiplicadorPilar, TipoPilar.Tierra,  0.10),
            new DefinicionNodo("nd_03","Gases activos",         "+10% EV Atmósfera",    1, null,           100,    TipoBonus.MultiplicadorPilar, TipoPilar.Atmosfera,0.10),
            new DefinicionNodo("nd_04","Sales oceánicas",       "+10% EV Océanos",      1, null,           100,    TipoBonus.MultiplicadorPilar, TipoPilar.Oceanos, 0.10),
            // Era 2
            new DefinicionNodo("nd_05","Fotosíntesis",          "+15% EV Océanos+Vida", 2, new[]{"nd_01","nd_03"}, 2_000, TipoBonus.MultiplicadorEV, TipoPilar.Oceanos, 0.15),
            new DefinicionNodo("nd_06","Respiración celular",   "+15% EV global",       2, new[]{"nd_03","nd_04"}, 2_000, TipoBonus.MultiplicadorEV, TipoPilar.Atmosfera,0.15),
            new DefinicionNodo("nd_07","Algas",                 "×1.5 Océanos",         2, new[]{"nd_05"},         5_000, TipoBonus.MultiplicadorPilar, TipoPilar.Oceanos, 0.20),
            new DefinicionNodo("nd_08","Ciclo del carbono",     "Desbloquea Biomasa",   2, new[]{"nd_05","nd_06"}, 8_000, TipoBonus.RecursoExtra, TipoPilar.Vida,    0.25),
            // Era 3
            new DefinicionNodo("nd_09","Hongos",                "+40% Nutrientes",      3, new[]{"nd_07","nd_02"}, 50_000,  TipoBonus.MultiplicadorPilar, TipoPilar.Tierra,  0.25),
            new DefinicionNodo("nd_10","Esponjas",              "+50% Vida marina",     3, new[]{"nd_07"},         50_000,  TipoBonus.MultiplicadorPilar, TipoPilar.Oceanos, 0.25),
            new DefinicionNodo("nd_11","Trilobites",            "×2 Biomasa",           3, new[]{"nd_10"},         120_000, TipoBonus.MultiplicadorPilar, TipoPilar.Vida,    0.30),
            new DefinicionNodo("nd_12","Cadena trófica",        "×1.5 EV global",       3, new[]{"nd_09","nd_11"}, 200_000, TipoBonus.MultiplicadorEV, TipoPilar.Vida,    0.35),
            // Era 4
            new DefinicionNodo("nd_13","Plantas vasculares",    "×2 Tierra",            4, new[]{"nd_09"},         800_000, TipoBonus.MultiplicadorPilar, TipoPilar.Tierra,  0.40),
            new DefinicionNodo("nd_14","Insectos",              "+60% Biomasa",         4, new[]{"nd_13"},         800_000, TipoBonus.MultiplicadorPilar, TipoPilar.Vida,    0.35),
            new DefinicionNodo("nd_15","Semillas",              "×3 Nutrientes",        4, new[]{"nd_13"},         2_000_000,TipoBonus.MultiplicadorPilar,TipoPilar.Tierra,  0.50),
            new DefinicionNodo("nd_16","Anfibios",              "×2 Vida",              4, new[]{"nd_14","nd_15"}, 3_000_000,TipoBonus.MultiplicadorPilar,TipoPilar.Vida,    0.50),
            // Era 5
            new DefinicionNodo("nd_17","Reptiles",              "×2.5 Biomasa",         5, new[]{"nd_16"},         10_000_000,TipoBonus.MultiplicadorPilar,TipoPilar.Vida,   0.60),
            new DefinicionNodo("nd_18","Dinosaurios",           "×3 EV global",         5, new[]{"nd_17"},         25_000_000,TipoBonus.MultiplicadorEV,TipoPilar.Vida,     0.70),
            new DefinicionNodo("nd_19","Angiospermas",          "×4 Nutrientes",        5, new[]{"nd_15"},         15_000_000,TipoBonus.MultiplicadorPilar,TipoPilar.Tierra, 0.65),
            new DefinicionNodo("nd_20","Extinción K-Pg",        "Permite Extinción L1", 5, new[]{"nd_18"},         50_000_000,TipoBonus.DesbloqueoInstante,TipoPilar.Vida,  1.00),
            // Era 6
            new DefinicionNodo("nd_21","Mamíferos",             "×3 Vida",              6, new[]{"nd_20"},         200_000_000,TipoBonus.MultiplicadorPilar,TipoPilar.Vida,  0.80),
            new DefinicionNodo("nd_22","Primates",              "Desbloquea Conocim.",  6, new[]{"nd_21"},         500_000_000,TipoBonus.RecursoExtra,TipoPilar.Vida,       0.50),
            new DefinicionNodo("nd_23","Homo sapiens",          "×4 Conocimiento",      6, new[]{"nd_22"},         1e9,         TipoBonus.MultiplicadorPilar,TipoPilar.Vida, 1.00),
            new DefinicionNodo("nd_24","Agricultura",           "×5 Nutrientes",        6, new[]{"nd_23"},         2e9,         TipoBonus.MultiplicadorPilar,TipoPilar.Tierra,1.00),
            // Era 7
            new DefinicionNodo("nd_25","Industria",             "×6 EV global",         7, new[]{"nd_23"},         8e9,  TipoBonus.MultiplicadorEV,    TipoPilar.Vida,    1.20),
            new DefinicionNodo("nd_26","Energía nuclear",       "×8 EV global",         7, new[]{"nd_25"},         20e9, TipoBonus.MultiplicadorEV,    TipoPilar.Atmosfera,1.50),
            new DefinicionNodo("nd_27","IA global",             "×10 Tecnología",       7, new[]{"nd_26"},         50e9, TipoBonus.MultiplicadorPilar, TipoPilar.Vida,    2.00),
            new DefinicionNodo("nd_28","Fusión fría",           "×15 EV global",        7, new[]{"nd_27"},         100e9,TipoBonus.MultiplicadorEV,    TipoPilar.Atmosfera,2.50),
            // Era 8
            new DefinicionNodo("nd_29","Cohetes",               "×10 E. Espacial",      8, new[]{"nd_28"},         500e9,TipoBonus.MultiplicadorPilar, TipoPilar.Atmosfera,3.00),
            new DefinicionNodo("nd_30","Terraformación",        "×20 EV global",        8, new[]{"nd_29"},         1e12, TipoBonus.MultiplicadorEV,    TipoPilar.Tierra,  4.00),
            new DefinicionNodo("nd_31","Esfera de Dyson",       "×50 EV global",        8, new[]{"nd_30"},         5e12, TipoBonus.MultiplicadorEV,    TipoPilar.Atmosfera,5.00),
            new DefinicionNodo("nd_32","GAIA final",            "Condición fin de juego",8,new[]{"nd_31"},         10e12,TipoBonus.DesbloqueoInstante, TipoPilar.Vida,    10.00),
        };
    }

    public static class CatalogoEventos
    {
        public static DefinicionEvento[] Crear() => new[]
        {
            new DefinicionEvento("ev_01","Lluvia de meteoritos",  "Desvíalos con tu Atmósfera o pierde Vida",       TipoEvento.Negativo, 1, 3.0f, 30,  15, TipoPilar.Atmosfera, true),
            new DefinicionEvento("ev_02","Erupción volcánica",    "Lava fertilizan la Tierra pero dañan la Vida",   TipoEvento.Neutral,  1, 2.0f, 60,  20, TipoPilar.Tierra,    false),
            new DefinicionEvento("ev_03","Tormenta solar",        "Acepta: ×3 EV 30s. Rechaza: pierde Atmósfera",  TipoEvento.Neutral,  2, 3.0f, 30,  20, TipoPilar.Atmosfera, true),
            new DefinicionEvento("ev_04","Florecimiento marino",  "+200% Vida durante 60s si Océanos > Lv20",       TipoEvento.Positivo, 3, 3.0f, 60,  15, TipoPilar.Oceanos,   false),
            new DefinicionEvento("ev_05","Glaciación menor",      "Penaliza 30s pero da Fósiles extra al final",    TipoEvento.Negativo, 3, 0.5f, 30,  25, TipoPilar.Oceanos,   true),
            new DefinicionEvento("ev_06","Supervolcán",           "×5 Tierra 60s pero bloquea Vida temporalmente",  TipoEvento.Neutral,  4, 5.0f, 60,  30, TipoPilar.Tierra,    true),
            new DefinicionEvento("ev_07","Impacto asteroide",     "Gasta 50% de tu EV para ×10 durante 30s",        TipoEvento.Neutral,  4, 10.0f,30,  30, TipoPilar.Atmosfera, true),
            new DefinicionEvento("ev_08","Explosión cámbrica",    "×4 Vida durante 90s — solo Era 3+",              TipoEvento.Positivo, 3, 4.0f, 90,  20, TipoPilar.Vida,      false),
            new DefinicionEvento("ev_09","Corriente de El Niño",  "×2 Océanos + Tierra durante 60s",                TipoEvento.Positivo, 4, 2.0f, 60,  15, TipoPilar.Oceanos,   false),
            new DefinicionEvento("ev_10","Plaga",                 "Penaliza Vida 50% durante 30s",                  TipoEvento.Negativo, 5, 0.5f, 30,  20, TipoPilar.Vida,      true),
            new DefinicionEvento("ev_11","Descubrimiento fósil",  "+500 Fósiles extra si prestige activo",          TipoEvento.Positivo, 5, 1.0f, 5,   15, TipoPilar.Tierra,    false),
            new DefinicionEvento("ev_12","Revolución industrial", "×8 EV 60s — solo Era 7+",                        TipoEvento.Positivo, 7, 8.0f, 60,  30, TipoPilar.Vida,      false),
            new DefinicionEvento("ev_13","Pandemia global",       "Penaliza Vida 80% durante 60s — Era 6+",         TipoEvento.Negativo, 6, 0.2f, 60,  35, TipoPilar.Vida,      true),
            new DefinicionEvento("ev_14","Lanzamiento satélite",  "×3 Atmósfera durante 90s — Era 7+",              TipoEvento.Positivo, 7, 3.0f, 90,  20, TipoPilar.Atmosfera, false),
            new DefinicionEvento("ev_15","Primer contacto",       "×20 todo durante 30s — solo Era 8",              TipoEvento.Positivo, 8, 20.0f,30,  60, TipoPilar.Vida,      true),
        };
    }

    public static class CatalogoLogros
    {
        public static DefinicionLogro[] Crear() => new[]
        {
            // Logros de EV
            new DefinicionLogro("lg_01","Chispa de vida",     "Alcanza 1.000 EV",               TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.05, TipoPilar.Vida,      s => s.EV >= 1_000),
            new DefinicionLogro("lg_02","Llama creciente",    "Alcanza 1 Millón de EV",          TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.10, TipoPilar.Vida,      s => s.EV >= 1_000_000),
            new DefinicionLogro("lg_03","Tormenta de energía","Alcanza 1 Billón de EV",          TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.15, TipoPilar.Vida,      s => s.EV >= 1_000_000_000),
            new DefinicionLogro("lg_04","Dios cósmico",       "Alcanza 1 Trillón de EV",         TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.25, TipoPilar.Vida,      s => s.EV >= 1e12),
            // Logros de eras
            new DefinicionLogro("lg_05","Primeros pasos",     "Avanza a Era 2",                  TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.05, TipoPilar.Vida,      s => s.Era >= 2),
            new DefinicionLogro("lg_06","Oceánico",           "Avanza a Era 3",                  TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.10, TipoPilar.Oceanos,   s => s.Era >= 3),
            new DefinicionLogro("lg_07","Pangea conquistada", "Avanza a Era 4",                  TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.15, TipoPilar.Tierra,    s => s.Era >= 4),
            new DefinicionLogro("lg_08","Era del rugido",     "Avanza a Era 5",                  TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.20, TipoPilar.Vida,      s => s.Era >= 5),
            new DefinicionLogro("lg_09","Homo ludens",        "Avanza a Era 6",                  TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.25, TipoPilar.Vida,      s => s.Era >= 6),
            new DefinicionLogro("lg_10","Luces del progreso", "Avanza a Era 7",                  TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.30, TipoPilar.Vida,      s => s.Era >= 7),
            new DefinicionLogro("lg_11","Más allá del mundo", "Avanza a Era 8",                  TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.50, TipoPilar.Vida,      s => s.Era >= 8),
            // Logros de prestige
            new DefinicionLogro("lg_12","Superviviente",      "Haz tu primer prestige",          TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.20, TipoPilar.Vida,      s => s.VecesPrestige >= 1),
            new DefinicionLogro("lg_13","Ciclo eterno",       "Haz 5 prestiges",                 TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.50, TipoPilar.Vida,      s => s.VecesPrestige >= 5),
            // Logros de sinergias
            new DefinicionLogro("lg_14","Armonía",            "Activa 6 sinergias",              TipoLogro.Visible, TipoBonus.MultiplicadorEV, 1.30, TipoPilar.Atmosfera, s => s.SinergiasActivas >= 6),
            new DefinicionLogro("lg_15","GAIA despertada",    "Activa la sinergia GAIA",         TipoLogro.Visible, TipoBonus.MultiplicadorEV, 2.00, TipoPilar.Vida,      s => s.SinergiasActivas >= 12),
            // Logros ocultos
            new DefinicionLogro("lg_16","Monje digital",      "No compres nada durante 5 min",   TipoLogro.Oculto,  TipoBonus.MultiplicadorEV, 2.0,  TipoPilar.Vida,      s => s.TiempoJugado >= 300),
            new DefinicionLogro("lg_17","Coleccionista",      "Consigue 100 fósiles",            TipoLogro.Oculto,  TipoBonus.MultiplicadorEV, 1.5,  TipoPilar.Tierra,   s => s.Fosiles >= 100),
            new DefinicionLogro("lg_18","Genético",           "Consigue 50 genes",               TipoLogro.Oculto,  TipoBonus.MultiplicadorEV, 2.0,  TipoPilar.Vida,      s => s.Genes >= 50),
            new DefinicionLogro("lg_19","Cuántico",           "Consigue 10 quarks",              TipoLogro.Oculto,  TipoBonus.MultiplicadorEV, 3.0,  TipoPilar.Atmosfera, s => s.Quarks >= 10),
            new DefinicionLogro("lg_20","Explorador galáctico","Completa la Era 8 tras 3 prestiges",TipoLogro.Oculto,TipoBonus.MultiplicadorEV,5.0, TipoPilar.Vida,      s => s.Era >= 8 && s.VecesPrestige >= 3),
        };
    }
}
