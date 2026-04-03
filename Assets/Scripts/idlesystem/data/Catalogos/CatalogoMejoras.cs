using Terra.Core;
using Terra.Data;

namespace Terra.Data.Catalogos
{
    /// <summary>
    /// Catalogo de todas las mejoras del juego.
    /// BALANCEO v2:
    /// - Produccion base Era 1 multiplicada x3 para ritmo fluido
    /// - Multiplicador coste 1.12 en eras 1-3 (antes 1.15)
    /// - Costes Era 1 rebajados para primera mejora en 8-10s
    /// </summary>
    public static class CatalogoMejoras
    {
        public static DefinicionMejora[] Crear()
        {
            var mejoras = new DefinicionMejora[84];
            int i = 0;

            // ── ATMOSFERA ─────────────────────────────────────────────────
            // Zona 0 — Baja atmosfera
            mejoras[i++] = new DefinicionMejora("atm_01","Gases primordiales",    "H2, CH4 y NH3 emergen del interior",        TipoPilar.Atmosfera, 0, 8,        0.3,   1, null, TipoPilar.Atmosfera, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("atm_02","Vapor de agua",         "La evaporacion crea las primeras nubes",     TipoPilar.Atmosfera, 0, 50,       1.5,   1, new[]{"atm_01"}, TipoPilar.Atmosfera, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("atm_03","CO2 atmosferico",       "El efecto invernadero calienta el planeta",  TipoPilar.Atmosfera, 0, 300,      7.5,   2, new[]{"atm_02"}, TipoPilar.Atmosfera, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("atm_04","Nitrogeno estable",     "El N2 domina la composicion atmosferica",    TipoPilar.Atmosfera, 0, 2_000,    36,    2, new[]{"atm_03"}, TipoPilar.Atmosfera, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("atm_05","Capa de ozono",         "El O3 blinda el planeta de la radiacion UV", TipoPilar.Atmosfera, 0, 15_000,   180,   3, new[]{"atm_04"});
            mejoras[i++] = new DefinicionMejora("atm_06","Oxigeno libre",         "El O2 revela el Gran Evento de Oxidacion",   TipoPilar.Atmosfera, 0, 100_000,  900,   4, new[]{"atm_05"});
            mejoras[i++] = new DefinicionMejora("atm_07","Atmosfera moderna",     "Composicion estabilizada al 21% O2",         TipoPilar.Atmosfera, 0, 800_000,  4500,  5, new[]{"atm_06"});
            // Zona 1 — Nubes
            mejoras[i++] = new DefinicionMejora("atm_08","Condensacion",          "Las primeras gotas de lluvia caen",           TipoPilar.Atmosfera, 1, 120,      3,     2, new[]{"atm_02"}, TipoPilar.Atmosfera, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("atm_09","Nubes cumulos",         "Masas de vapor cubren el cielo",              TipoPilar.Atmosfera, 1, 900,      15,    2, new[]{"atm_08"}, TipoPilar.Atmosfera, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("atm_10","Ciclo hidrologico",     "El agua circula entre mar, aire y tierra",    TipoPilar.Atmosfera, 1, 8_000,    75,    3, new[]{"atm_09"});
            mejoras[i++] = new DefinicionMejora("atm_11","Corrientes en chorro",  "Los jet streams redistribuyen el calor",      TipoPilar.Atmosfera, 1, 60_000,   360,   3, new[]{"atm_10"});
            mejoras[i++] = new DefinicionMejora("atm_12","Monzones",              "Lluvias estacionales fertilizan continentes", TipoPilar.Atmosfera, 1, 400_000,  1800,  4, new[]{"atm_11"});
            mejoras[i++] = new DefinicionMejora("atm_13","Lluvia acida primitiva","Weathering quimico forma los primeros suelos",TipoPilar.Atmosfera, 1, 2_500_000,9000,  5, new[]{"atm_12"});
            mejoras[i++] = new DefinicionMejora("atm_14","Clima global estable",  "El ciclo climatico se autorregula",           TipoPilar.Atmosfera, 1, 18_000_000,45000,6, new[]{"atm_13"});
            // Zona 2 — Alta atmosfera
            mejoras[i++] = new DefinicionMejora("atm_15","Ionosfera",             "Las particulas cargadas crean las auroras",   TipoPilar.Atmosfera, 2, 40_000,   150,   3, new[]{"atm_05"});
            mejoras[i++] = new DefinicionMejora("atm_16","Magnetosfera",          "El campo magnetico deflecta el viento solar", TipoPilar.Atmosfera, 2, 320_000,  750,   4, new[]{"atm_15"});
            mejoras[i++] = new DefinicionMejora("atm_17","Escudo solar",          "Proteccion completa contra radiacion",        TipoPilar.Atmosfera, 2, 2_000_000,3600,  5, new[]{"atm_16"});
            mejoras[i++] = new DefinicionMejora("atm_18","Viento solar captado",  "La energia solar alimenta la atmosfera",      TipoPilar.Atmosfera, 2, 12_000_000,18000,6, new[]{"atm_17"});
            mejoras[i++] = new DefinicionMejora("atm_19","Termosfera activa",     "Radiacion absorbida genera calor extremo",    TipoPilar.Atmosfera, 2, 9e7,      90_000,7, new[]{"atm_18"});
            mejoras[i++] = new DefinicionMejora("atm_20","Exosfera tecnologica",  "Satelites regulan la temperatura global",     TipoPilar.Atmosfera, 2, 9e8,      450_000,8,new[]{"atm_19"});
            mejoras[i++] = new DefinicionMejora("atm_21","Atmosfera artificial",  "Control total del clima planetario",          TipoPilar.Atmosfera, 2, 9e9,      2_250_000,8,new[]{"atm_20"});

            // ── OCEANOS ───────────────────────────────────────────────────
            // Zona 0 — Superficie
            mejoras[i++] = new DefinicionMejora("oc_01","Mares poco profundos",   "Las primeras cuencas oceanicas se llenan",    TipoPilar.Oceanos, 0, 10,        0.45,  1, null, TipoPilar.Oceanos, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("oc_02","Arrecifes primitivos",   "Estructuras de carbonato crecen en el mar",   TipoPilar.Oceanos, 0, 65,        1.8,   1, new[]{"oc_01"}, TipoPilar.Oceanos, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("oc_03","Plancton",               "Microorganismos transforman la quimica marina",TipoPilar.Oceanos,0, 450,       9,     2, new[]{"oc_02"}, TipoPilar.Oceanos, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("oc_04","Corrientes superficiales","El viento arrastra masas de agua calidas",   TipoPilar.Oceanos, 0, 3_200,     45,    2, new[]{"oc_03"}, TipoPilar.Oceanos, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("oc_05","Zooplancton",            "Animales diminutos dominan la superficie",    TipoPilar.Oceanos, 0, 22_000,    225,   3, new[]{"oc_04"});
            mejoras[i++] = new DefinicionMejora("oc_06","Algas masivas",          "Bosques de algas cubren los mares poco prof.",TipoPilar.Oceanos, 0, 160_000,   1125,  4, new[]{"oc_05"});
            mejoras[i++] = new DefinicionMejora("oc_07","Gran barrera coralina",  "El mayor ecosistema marino del planeta",       TipoPilar.Oceanos,0, 1_300_000, 5625,  5, new[]{"oc_06"});
            // Zona 1 — Zona media
            mejoras[i++] = new DefinicionMejora("oc_08","Corrientes oceanicas",   "El calor se distribuye por todo el oceano",   TipoPilar.Oceanos, 1, 190,       3.6,   2, new[]{"oc_02"}, TipoPilar.Oceanos, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("oc_09","Termoclina",             "Capa que separa agua calida de fria",          TipoPilar.Oceanos, 1, 1_300,     18,    2, new[]{"oc_08"}, TipoPilar.Oceanos, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("oc_10","Haloclina",              "Gradiente de salinidad estratifica el mar",    TipoPilar.Oceanos, 1, 9_000,     90,    3, new[]{"oc_09"});
            mejoras[i++] = new DefinicionMejora("oc_11","Upwelling",              "Aguas profundas ricas en nutrientes emergen",  TipoPilar.Oceanos, 1, 65_000,    450,   3, new[]{"oc_10"});
            mejoras[i++] = new DefinicionMejora("oc_12","Giro oceanico",          "Corrientes circulares atrapan calor",          TipoPilar.Oceanos, 1, 450_000,   2250,  4, new[]{"oc_11"});
            mejoras[i++] = new DefinicionMejora("oc_13","Conveyor termohalino",   "Cinta transportadora global de calor",         TipoPilar.Oceanos, 1, 3_200_000, 11250, 5, new[]{"oc_12"});
            mejoras[i++] = new DefinicionMejora("oc_14","Oceano termorregulado",  "El oceano actua como regulador climatico",     TipoPilar.Oceanos, 1, 22_000_000,56250, 6, new[]{"oc_13"});
            // Zona 2 — Profundidades
            mejoras[i++] = new DefinicionMejora("oc_15","Fuentes hidrotermales",  "Chimeneas submarinas crean vida sin sol",      TipoPilar.Oceanos, 2, 5_000,     60,    3, new[]{"oc_09"});
            mejoras[i++] = new DefinicionMejora("oc_16","Zona abisal",            "El fondo marino alberga vida extremofila",     TipoPilar.Oceanos, 2, 38_000,    300,   3, new[]{"oc_15"});
            mejoras[i++] = new DefinicionMejora("oc_17","Quimiosintesis",         "Vida basada en energia quimica, no solar",     TipoPilar.Oceanos, 2, 260_000,   1500,  4, new[]{"oc_16"});
            mejoras[i++] = new DefinicionMejora("oc_18","Metano submarino",       "Clathratos de metano almacenan energia",       TipoPilar.Oceanos, 2, 1_900_000, 7500,  5, new[]{"oc_17"});
            mejoras[i++] = new DefinicionMejora("oc_19","Hidratos de gas",        "Depositos energeticos en el fondo marino",     TipoPilar.Oceanos, 2, 13_000_000,37500, 6, new[]{"oc_18"});
            mejoras[i++] = new DefinicionMejora("oc_20","Extraccion submarina",   "Mineria en la corteza oceanica",               TipoPilar.Oceanos, 2, 9e7,       187500,7, new[]{"oc_19"});
            mejoras[i++] = new DefinicionMejora("oc_21","Ciudad submarina",       "Asentamientos humanos bajo el mar",            TipoPilar.Oceanos, 2, 8e8,       937500,8, new[]{"oc_20"});

            // ── TIERRA ────────────────────────────────────────────────────
            // Zona 0 — Superficie
            mejoras[i++] = new DefinicionMejora("ti_01","Roca basaltica",         "La corteza oceanica primitiva emerge",         TipoPilar.Tierra, 0, 9,         0.36,  1, null, TipoPilar.Tierra, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("ti_02","Sedimentos",             "Capas de material se acumulan en cuencas",     TipoPilar.Tierra, 0, 58,        1.65,  1, new[]{"ti_01"}, TipoPilar.Tierra, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("ti_03","Suelo primitivo",        "Weathering crea las primeras tierras fertiles",TipoPilar.Tierra, 0, 380,       8.4,   2, new[]{"ti_02"}, TipoPilar.Tierra, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("ti_04","Minerales complejos",    "Silicatos y carbonatos diversifican el suelo", TipoPilar.Tierra, 0, 2_600,     42,    2, new[]{"ti_03"}, TipoPilar.Tierra, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("ti_05","Suelo fertil",           "Materia organica enriquece la tierra",         TipoPilar.Tierra, 0, 18_000,    210,   3, new[]{"ti_04"});
            mejoras[i++] = new DefinicionMejora("ti_06","Humus organico",         "Descomposicion crea suelos ricos en carbono",  TipoPilar.Tierra, 0, 130_000,   1050,  4, new[]{"ti_05"});
            mejoras[i++] = new DefinicionMejora("ti_07","Tierras de cultivo",     "Humanos transforman el paisaje en campos",     TipoPilar.Tierra, 0, 950_000,   5250,  5, new[]{"ti_06"});
            // Zona 1 — Corteza
            mejoras[i++] = new DefinicionMejora("ti_08","Placas tectonicas",      "La corteza se fragmenta en placas moviles",    TipoPilar.Tierra, 1, 160,       3.3,   2, new[]{"ti_02"}, TipoPilar.Tierra, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("ti_09","Cadenas montanosas",     "La colision de placas eleva cordilleras",      TipoPilar.Tierra, 1, 1_150,     16.5,  2, new[]{"ti_08"}, TipoPilar.Tierra, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("ti_10","Volcanes activos",       "El magma modela la superficie constantemente", TipoPilar.Tierra, 1, 7_800,     82.5,  3, new[]{"ti_09"});
            mejoras[i++] = new DefinicionMejora("ti_11","Rift continental",       "Las placas se separan formando oceanos nuevos", TipoPilar.Tierra, 1, 58_000,    411,   3, new[]{"ti_10"});
            mejoras[i++] = new DefinicionMejora("ti_12","Subduccion",             "Una placa se hunde bajo otra, reciclando roca",TipoPilar.Tierra, 1, 420_000,   2061,  4, new[]{"ti_11"});
            mejoras[i++] = new DefinicionMejora("ti_13","Cordilleras globales",   "Himalaya, Andes, Rockies: la Tierra alcanza",  TipoPilar.Tierra, 1, 2_900_000, 10305, 5, new[]{"ti_12"});
            mejoras[i++] = new DefinicionMejora("ti_14","Deriva continental",     "Los continentes siguen moviendose lentamente", TipoPilar.Tierra, 1, 20_000_000,51525, 6, new[]{"ti_13"});
            // Zona 2 — Interior
            mejoras[i++] = new DefinicionMejora("ti_15","Nucleo fundido",         "Hierro y niquel liquidos en el centro",        TipoPilar.Tierra, 2, 3_900,     54,    3, new[]{"ti_09"});
            mejoras[i++] = new DefinicionMejora("ti_16","Campo magnetico",        "El dinamo terrestre genera un escudo",         TipoPilar.Tierra, 2, 29_000,    270,   3, new[]{"ti_15"});
            mejoras[i++] = new DefinicionMejora("ti_17","Conveccion del manto",   "El manto semisólido fluye en miles de anhos",  TipoPilar.Tierra, 2, 205_000,   1350,  4, new[]{"ti_16"});
            mejoras[i++] = new DefinicionMejora("ti_18","Puntos calientes",       "Plumas del manto perforan la corteza",         TipoPilar.Tierra, 2, 1_400_000, 6750,  5, new[]{"ti_17"});
            mejoras[i++] = new DefinicionMejora("ti_19","Geotermia profunda",     "Energia del interior aprovechada",             TipoPilar.Tierra, 2, 10_000_000,33750, 6, new[]{"ti_18"});
            mejoras[i++] = new DefinicionMejora("ti_20","Mineria del manto",      "Extraccion directa de recursos del manto",     TipoPilar.Tierra, 2, 7.5e7,     168750,7, new[]{"ti_19"});
            mejoras[i++] = new DefinicionMejora("ti_21","Control tectonico",      "La humanidad controla el movimiento sismico",  TipoPilar.Tierra, 2, 7.5e8,     843750,8, new[]{"ti_20"});

            // ── VIDA ─────────────────────────────────────────────────────
            // Zona 0 — Microvida
            mejoras[i++] = new DefinicionMejora("vi_01","Bacteria primordial",    "Las primeras moleculas autorreplicantes",      TipoPilar.Vida, 0, 8,          0.3,   1, null, TipoPilar.Vida, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("vi_02","Arqueobacterias",        "Vida extremofila en condiciones inhospitas",   TipoPilar.Vida, 0, 48,         1.5,   1, new[]{"vi_01"}, TipoPilar.Vida, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("vi_03","Celulas eucariotas",     "Un nucleo protege el material genetico",       TipoPilar.Vida, 0, 320,        7.5,   2, new[]{"vi_02"}, TipoPilar.Vida, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("vi_04","Organismos multicel.",   "Celulas que cooperan forman un ser mayor",     TipoPilar.Vida, 0, 2_250,      37.5,  2, new[]{"vi_03"}, TipoPilar.Vida, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("vi_05","Reproduccion sexual",    "La variedad genetica acelera la evolucion",    TipoPilar.Vida, 0, 16_000,     187.5, 3, new[]{"vi_04"});
            mejoras[i++] = new DefinicionMejora("vi_06","ADN complejo",           "Genes reguladores abren nuevas posibilidades", TipoPilar.Vida, 0, 115_000,    937.5, 4, new[]{"vi_05"});
            mejoras[i++] = new DefinicionMejora("vi_07","Epigenetica",            "El entorno modifica la expresion genica",      TipoPilar.Vida, 0, 820_000,    4686,  5, new[]{"vi_06"});
            // Zona 1 — Vida compleja
            mejoras[i++] = new DefinicionMejora("vi_08","Algas fotosinteticas",   "La luz solar se convierte en energia vital",   TipoPilar.Vida, 1, 128,        3,     2, new[]{"vi_03"}, TipoPilar.Vida, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("vi_09","Hongos",                 "Descomponedores que reciclan nutrientes",      TipoPilar.Vida, 1, 950,        15,    2, new[]{"vi_08"}, TipoPilar.Vida, 0, 1.12);
            mejoras[i++] = new DefinicionMejora("vi_10","Plantas vasculares",     "Raices y tallos conquistan tierra firme",      TipoPilar.Vida, 1, 6_500,      75,    3, new[]{"vi_09"});
            mejoras[i++] = new DefinicionMejora("vi_11","Insectos",               "Polinizadores y descomponedores colonizan",    TipoPilar.Vida, 1, 48_000,     375,   3, new[]{"vi_10"});
            mejoras[i++] = new DefinicionMejora("vi_12","Reptiles",               "Vertebrados que dominan la tierra y el agua",  TipoPilar.Vida, 1, 350_000,    1875,  4, new[]{"vi_11"});
            mejoras[i++] = new DefinicionMejora("vi_13","Dinosaurios",            "Colosos que dominan durante 165 millones",     TipoPilar.Vida, 1, 2_550_000,  9375,  5, new[]{"vi_12"});
            mejoras[i++] = new DefinicionMejora("vi_14","Mamiferos dominantes",   "Tras la extincion, los mamiferos reinan",      TipoPilar.Vida, 1, 19_000_000, 46875, 6, new[]{"vi_13"});
            // Zona 2 — Vida inteligente
            mejoras[i++] = new DefinicionMejora("vi_15","Sistema nervioso",       "Celulas que procesan y transmiten senales",    TipoPilar.Vida, 2, 3_200,      45,    3, new[]{"vi_09"});
            mejoras[i++] = new DefinicionMejora("vi_16","Cerebro primitivo",      "Un organo dedicado a procesar el entorno",     TipoPilar.Vida, 2, 25_600,     225,   3, new[]{"vi_15"});
            mejoras[i++] = new DefinicionMejora("vi_17","Conducta social",        "Grupos cooperativos superan al individuo",     TipoPilar.Vida, 2, 180_000,    1125,  4, new[]{"vi_16"});
            mejoras[i++] = new DefinicionMejora("vi_18","Uso de herramientas",    "La tecnologia amplifica las capacidades",      TipoPilar.Vida, 2, 1_280_000,  5625,  5, new[]{"vi_17"});
            mejoras[i++] = new DefinicionMejora("vi_19","Lenguaje",               "La comunicacion simbolica transmite cultura",  TipoPilar.Vida, 2, 9_000_000,  28125, 6, new[]{"vi_18"});
            mejoras[i++] = new DefinicionMejora("vi_20","Consciencia colectiva",  "La humanidad piensa como un organismo unico",  TipoPilar.Vida, 2, 6.4e7,      140625,7, new[]{"vi_19"});
            mejoras[i++] = new DefinicionMejora("vi_21","Singularidad",           "IA y biologia se fusionan: fin del limite",    TipoPilar.Vida, 2, 6.4e8,      703125,8, new[]{"vi_20"});

            return mejoras;
        }
    }
}
