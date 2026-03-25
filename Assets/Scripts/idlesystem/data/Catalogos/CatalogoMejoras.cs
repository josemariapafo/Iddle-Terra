using Terra.Core;
using Terra.Data;

namespace Terra.Data.Catalogos
{
    /// <summary>
    /// Catálogo de todas las mejoras del juego.
    /// 4 pilares × 3 zonas × 7 mejoras = 84 mejoras totales.
    /// Cada zona representa una sección visual dentro de la pantalla del pilar.
    /// </summary>
    public static class CatalogoMejoras
    {
        public static DefinicionMejora[] Crear()
        {
            var mejoras = new DefinicionMejora[84];
            int i = 0;

            // ── ATMÓSFERA ─────────────────────────────────────────────────
            // Zona 0 — Baja atmósfera
            mejoras[i++] = new DefinicionMejora("atm_01","Gases primordiales",    "H₂, CH₄ y NH₃ emergen del interior",        TipoPilar.Atmosfera, 0, 10,       0.1,   1);
            mejoras[i++] = new DefinicionMejora("atm_02","Vapor de agua",          "La evaporación crea las primeras nubes",     TipoPilar.Atmosfera, 0, 80,       0.5,   1, new[]{"atm_01"});
            mejoras[i++] = new DefinicionMejora("atm_03","CO₂ atmosférico",        "El efecto invernadero calienta el planeta",  TipoPilar.Atmosfera, 0, 500,      2.5,   2, new[]{"atm_02"});
            mejoras[i++] = new DefinicionMejora("atm_04","Nitrógeno estable",      "El N₂ domina la composición atmosférica",   TipoPilar.Atmosfera, 0, 3_000,    12,    2, new[]{"atm_03"});
            mejoras[i++] = new DefinicionMejora("atm_05","Capa de ozono",          "El O₃ blinda el planeta de la radiación UV",TipoPilar.Atmosfera, 0, 20_000,   60,    3, new[]{"atm_04"});
            mejoras[i++] = new DefinicionMejora("atm_06","Oxígeno libre",          "El O₂ revela el Gran Evento de Oxidación",  TipoPilar.Atmosfera, 0, 150_000,  300,   4, new[]{"atm_05"});
            mejoras[i++] = new DefinicionMejora("atm_07","Atmósfera moderna",      "Composición estabilizada al 21% O₂",        TipoPilar.Atmosfera, 0, 1_000_000,1500,  5, new[]{"atm_06"});
            // Zona 1 — Nubes
            mejoras[i++] = new DefinicionMejora("atm_08","Condensación",           "Las primeras gotas de lluvia caen",          TipoPilar.Atmosfera, 1, 200,      1,     2, new[]{"atm_02"});
            mejoras[i++] = new DefinicionMejora("atm_09","Nubes cúmulos",          "Masas de vapor cubren el cielo",             TipoPilar.Atmosfera, 1, 1_500,    5,     2, new[]{"atm_08"});
            mejoras[i++] = new DefinicionMejora("atm_10","Ciclo hidrológico",      "El agua circula entre mar, aire y tierra",   TipoPilar.Atmosfera, 1, 10_000,   25,    3, new[]{"atm_09"});
            mejoras[i++] = new DefinicionMejora("atm_11","Corrientes en chorro",   "Los jet streams redistribuyen el calor",     TipoPilar.Atmosfera, 1, 75_000,   120,   3, new[]{"atm_10"});
            mejoras[i++] = new DefinicionMejora("atm_12","Monzones",               "Lluvias estacionales fertilizan continentes",TipoPilar.Atmosfera, 1, 500_000,  600,   4, new[]{"atm_11"});
            mejoras[i++] = new DefinicionMejora("atm_13","Lluvia ácida primitiva", "Weathering químico forma los primeros suelos",TipoPilar.Atmosfera,1, 3_000_000,3000,  5, new[]{"atm_12"});
            mejoras[i++] = new DefinicionMejora("atm_14","Clima global estable",   "El ciclo climático se autorregula",          TipoPilar.Atmosfera, 1, 20_000_000,15000,6, new[]{"atm_13"});
            // Zona 2 — Alta atmósfera
            mejoras[i++] = new DefinicionMejora("atm_15","Ionosfera",              "Las partículas cargadas crean las auroras",  TipoPilar.Atmosfera, 2, 50_000,   50,    3, new[]{"atm_05"});
            mejoras[i++] = new DefinicionMejora("atm_16","Magnetosfera",           "El campo magnético deflecta el viento solar",TipoPilar.Atmosfera, 2, 400_000,  250,   4, new[]{"atm_15"});
            mejoras[i++] = new DefinicionMejora("atm_17","Escudo solar",           "Protección completa contra radiación",       TipoPilar.Atmosfera, 2, 2_500_000,1200,  5, new[]{"atm_16"});
            mejoras[i++] = new DefinicionMejora("atm_18","Viento solar captado",   "La energía solar alimenta la atmósfera",     TipoPilar.Atmosfera, 2, 15_000_000,6000, 6, new[]{"atm_17"});
            mejoras[i++] = new DefinicionMejora("atm_19","Termosfera activa",      "Radiación absorbida genera calor extremo",   TipoPilar.Atmosfera, 2, 1e8,      30_000,7, new[]{"atm_18"});
            mejoras[i++] = new DefinicionMejora("atm_20","Exosfera tecnológica",   "Satélites regulan la temperatura global",    TipoPilar.Atmosfera, 2, 1e9,      150_000,8,new[]{"atm_19"});
            mejoras[i++] = new DefinicionMejora("atm_21","Atmósfera artificial",   "Control total del clima planetario",         TipoPilar.Atmosfera, 2, 1e10,     750_000,8,new[]{"atm_20"});

            // ── OCÉANOS ───────────────────────────────────────────────────
            // Zona 0 — Superficie
            mejoras[i++] = new DefinicionMejora("oc_01","Mares poco profundos",    "Las primeras cuencas oceánicas se llenan",   TipoPilar.Oceanos, 0, 15,        0.15,  1);
            mejoras[i++] = new DefinicionMejora("oc_02","Arrecifes primitivos",    "Estructuras de carbonato crecen en el mar",  TipoPilar.Oceanos, 0, 100,       0.6,   1, new[]{"oc_01"});
            mejoras[i++] = new DefinicionMejora("oc_03","Plancton",                "Microorganismos transforman la química marina",TipoPilar.Oceanos,0, 700,       3,     2, new[]{"oc_02"});
            mejoras[i++] = new DefinicionMejora("oc_04","Corrientes superficiales","El viento arrastra masas de agua cálidas",   TipoPilar.Oceanos, 0, 5_000,     15,    2, new[]{"oc_03"});
            mejoras[i++] = new DefinicionMejora("oc_05","Zooplancton",             "Animales diminutos dominan la superficie",   TipoPilar.Oceanos, 0, 35_000,    75,    3, new[]{"oc_04"});
            mejoras[i++] = new DefinicionMejora("oc_06","Algas masivas",           "Bosques de algas cubren los mares poco prof.",TipoPilar.Oceanos,0, 250_000,   375,   4, new[]{"oc_05"});
            mejoras[i++] = new DefinicionMejora("oc_07","Gran barrera coralina",   "El mayor ecosistema marino del planeta",      TipoPilar.Oceanos,0, 2_000_000, 1875,  5, new[]{"oc_06"});
            // Zona 1 — Zona media
            mejoras[i++] = new DefinicionMejora("oc_08","Corrientes oceánicas",    "El calor se distribuye por todo el océano",  TipoPilar.Oceanos, 1, 300,       1.2,   2, new[]{"oc_02"});
            mejoras[i++] = new DefinicionMejora("oc_09","Termoclina",              "Capa que separa agua cálida de fría",         TipoPilar.Oceanos,1, 2_000,     6,     2, new[]{"oc_08"});
            mejoras[i++] = new DefinicionMejora("oc_10","Haloclina",               "Gradiente de salinidad estratifica el mar",   TipoPilar.Oceanos,1, 14_000,    30,    3, new[]{"oc_09"});
            mejoras[i++] = new DefinicionMejora("oc_11","Upwelling",               "Aguas profundas ricas en nutrientes emergen", TipoPilar.Oceanos,1, 100_000,   150,   3, new[]{"oc_10"});
            mejoras[i++] = new DefinicionMejora("oc_12","Giro oceánico",           "Corrientes circulares atrapan calor",         TipoPilar.Oceanos,1, 700_000,   750,   4, new[]{"oc_11"});
            mejoras[i++] = new DefinicionMejora("oc_13","Transportador termohalino","Cinta transportadora global de calor",        TipoPilar.Oceanos,1, 5_000_000, 3750,  5, new[]{"oc_12"});
            mejoras[i++] = new DefinicionMejora("oc_14","Océano termorregulado",   "El océano actúa como regulador climático",    TipoPilar.Oceanos,1, 35_000_000,18750, 6, new[]{"oc_13"});
            // Zona 2 — Profundidades
            mejoras[i++] = new DefinicionMejora("oc_15","Fuentes hidrotermales",   "Chimeneas submarinas crean vida sin sol",     TipoPilar.Oceanos, 2, 8_000,    20,    3, new[]{"oc_09"});
            mejoras[i++] = new DefinicionMejora("oc_16","Zona abisal",             "El fondo marino alberga vida extremófila",    TipoPilar.Oceanos, 2, 60_000,   100,   3, new[]{"oc_15"});
            mejoras[i++] = new DefinicionMejora("oc_17","Quimiosíntesis",          "Vida basada en energía química, no solar",    TipoPilar.Oceanos, 2, 400_000,  500,   4, new[]{"oc_16"});
            mejoras[i++] = new DefinicionMejora("oc_18","Metano submarino",        "Clathratos de metano almacenan energía",      TipoPilar.Oceanos, 2, 3_000_000,2500,  5, new[]{"oc_17"});
            mejoras[i++] = new DefinicionMejora("oc_19","Hidratos de gas",         "Depósitos energéticos en el fondo marino",    TipoPilar.Oceanos, 2, 20_000_000,12500,6, new[]{"oc_18"});
            mejoras[i++] = new DefinicionMejora("oc_20","Extracción submarina",    "Minería en la corteza oceánica",              TipoPilar.Oceanos, 2, 150_000_000,62500,7,new[]{"oc_19"});
            mejoras[i++] = new DefinicionMejora("oc_21","Ciudad submarina",        "Asentamientos humanos bajo el mar",           TipoPilar.Oceanos, 2, 1e9,      312500,8,new[]{"oc_20"});

            // ── TIERRA ────────────────────────────────────────────────────
            // Zona 0 — Superficie
            mejoras[i++] = new DefinicionMejora("ti_01","Roca basáltica",          "La corteza oceánica primitiva emerge",        TipoPilar.Tierra, 0, 12,        0.12,  1);
            mejoras[i++] = new DefinicionMejora("ti_02","Sedimentos",              "Capas de material se acumulan en cuencas",    TipoPilar.Tierra, 0, 90,        0.55,  1, new[]{"ti_01"});
            mejoras[i++] = new DefinicionMejora("ti_03","Suelo primitivo",         "Weathering crea las primeras tierras fértiles",TipoPilar.Tierra,0, 600,       2.8,   2, new[]{"ti_02"});
            mejoras[i++] = new DefinicionMejora("ti_04","Minerales complejos",     "Silicatos y carbonatos diversifican el suelo",TipoPilar.Tierra,0, 4_000,     14,    2, new[]{"ti_03"});
            mejoras[i++] = new DefinicionMejora("ti_05","Suelo fértil",            "Materia orgánica enriquece la tierra",        TipoPilar.Tierra, 0, 28_000,   70,    3, new[]{"ti_04"});
            mejoras[i++] = new DefinicionMejora("ti_06","Humus orgánico",          "Descomposición crea suelos ricos en carbono", TipoPilar.Tierra, 0, 200_000,  350,   4, new[]{"ti_05"});
            mejoras[i++] = new DefinicionMejora("ti_07","Tierras de cultivo",      "Humanos transforman el paisaje en campos",    TipoPilar.Tierra, 0, 1_500_000,1750,  5, new[]{"ti_06"});
            // Zona 1 — Corteza
            mejoras[i++] = new DefinicionMejora("ti_08","Placas tectónicas",       "La corteza se fragmenta en placas móviles",   TipoPilar.Tierra, 1, 250,       1.1,   2, new[]{"ti_02"});
            mejoras[i++] = new DefinicionMejora("ti_09","Cadenas montañosas",      "La colisión de placas eleva cordilleras",     TipoPilar.Tierra, 1, 1_800,     5.5,   2, new[]{"ti_08"});
            mejoras[i++] = new DefinicionMejora("ti_10","Volcanes activos",        "El magma modela la superficie constantemente",TipoPilar.Tierra,1, 12_000,    27.5,  3, new[]{"ti_09"});
            mejoras[i++] = new DefinicionMejora("ti_11","Rift continental",        "Las placas se separan formando océanos nuevos",TipoPilar.Tierra,1, 90_000,   137,   3, new[]{"ti_10"});
            mejoras[i++] = new DefinicionMejora("ti_12","Subducción",              "Una placa se hunde bajo otra, reciclando roca",TipoPilar.Tierra,1, 650_000,  687,   4, new[]{"ti_11"});
            mejoras[i++] = new DefinicionMejora("ti_13","Cordilleras globales",    "Himalaya, Andes, Rockies: la Tierra alcanza",  TipoPilar.Tierra,1, 4_500_000,3435, 5, new[]{"ti_12"});
            mejoras[i++] = new DefinicionMejora("ti_14","Deriva continental",      "Los continentes siguen moviéndose lentamente", TipoPilar.Tierra,1,32_000_000,17175,6, new[]{"ti_13"});
            // Zona 2 — Interior
            mejoras[i++] = new DefinicionMejora("ti_15","Núcleo fundido",          "Hierro y níquel líquidos en el centro",       TipoPilar.Tierra, 2, 6_000,    18,    3, new[]{"ti_09"});
            mejoras[i++] = new DefinicionMejora("ti_16","Campo magnético",         "El dinamo terrestre genera un escudo",        TipoPilar.Tierra, 2, 45_000,   90,    3, new[]{"ti_15"});
            mejoras[i++] = new DefinicionMejora("ti_17","Convección del manto",    "El manto semisólido fluye en miles de años",  TipoPilar.Tierra, 2, 320_000,  450,   4, new[]{"ti_16"});
            mejoras[i++] = new DefinicionMejora("ti_18","Puntos calientes",        "Plumas del manto perforan la corteza",        TipoPilar.Tierra, 2, 2_200_000,2250,  5, new[]{"ti_17"});
            mejoras[i++] = new DefinicionMejora("ti_19","Geotermia profunda",      "Energía del interior aprovechada",            TipoPilar.Tierra, 2, 16_000_000,11250,6, new[]{"ti_18"});
            mejoras[i++] = new DefinicionMejora("ti_20","Minería del manto",       "Extracción directa de recursos del manto",    TipoPilar.Tierra, 2, 120_000_000,56250,7,new[]{"ti_19"});
            mejoras[i++] = new DefinicionMejora("ti_21","Control tectónico",       "La humanidad controla el movimiento sísmico", TipoPilar.Tierra, 2, 1e9,      281250,8,new[]{"ti_20"});

            // ── VIDA ─────────────────────────────────────────────────────
            // Zona 0 — Microvida
            mejoras[i++] = new DefinicionMejora("vi_01","Bacteria primordial",     "Las primeras moléculas autorreplicantes",     TipoPilar.Vida, 0, 10,        0.1,   1);
            mejoras[i++] = new DefinicionMejora("vi_02","Arqueobacterias",         "Vida extremófila en condiciones inhóspitas",  TipoPilar.Vida, 0, 75,        0.5,   1, new[]{"vi_01"});
            mejoras[i++] = new DefinicionMejora("vi_03","Células eucariotas",      "Un núcleo protege el material genético",      TipoPilar.Vida, 0, 500,       2.5,   2, new[]{"vi_02"});
            mejoras[i++] = new DefinicionMejora("vi_04","Organismos multicelulares",    "Células que cooperan forman un ser mayor",    TipoPilar.Vida, 0, 3_500,     12.5,  2, new[]{"vi_03"});
            mejoras[i++] = new DefinicionMejora("vi_05","Reproducción sexual",     "La variedad genética acelera la evolución",   TipoPilar.Vida, 0, 25_000,    62.5,  3, new[]{"vi_04"});
            mejoras[i++] = new DefinicionMejora("vi_06","ADN complejo",            "Genes reguladores abren nuevas posibilidades",TipoPilar.Vida, 0, 180_000,   312.5, 4, new[]{"vi_05"});
            mejoras[i++] = new DefinicionMejora("vi_07","Epigenética",             "El entorno modifica la expresión génica",     TipoPilar.Vida, 0, 1_300_000, 1562,  5, new[]{"vi_06"});
            // Zona 1 — Vida compleja
            mejoras[i++] = new DefinicionMejora("vi_08","Algas fotosintéticas",    "La luz solar se convierte en energía vital",  TipoPilar.Vida, 1, 200,       1,     2, new[]{"vi_03"});
            mejoras[i++] = new DefinicionMejora("vi_09","Hongos",                  "Descomponedores que reciclan nutrientes",     TipoPilar.Vida, 1, 1_500,     5,     2, new[]{"vi_08"});
            mejoras[i++] = new DefinicionMejora("vi_10","Plantas vasculares",      "Raíces y tallos conquistan tierra firme",     TipoPilar.Vida, 1, 10_000,    25,    3, new[]{"vi_09"});
            mejoras[i++] = new DefinicionMejora("vi_11","Insectos",                "Polinizadores y descomponedores colonizan",   TipoPilar.Vida, 1, 75_000,    125,   3, new[]{"vi_10"});
            mejoras[i++] = new DefinicionMejora("vi_12","Reptiles",                "Vertebrados que dominan la tierra y el agua", TipoPilar.Vida, 1, 550_000,   625,   4, new[]{"vi_11"});
            mejoras[i++] = new DefinicionMejora("vi_13","Dinosaurios",             "Colosos que dominan durante 165 millones de años",TipoPilar.Vida,1,4_000_000,3125, 5, new[]{"vi_12"});
            mejoras[i++] = new DefinicionMejora("vi_14","Mamíferos dominantes",    "Tras la extinción, los mamíferos reinan",     TipoPilar.Vida, 1, 30_000_000,15625,6, new[]{"vi_13"});
            // Zona 2 — Vida inteligente
            mejoras[i++] = new DefinicionMejora("vi_15","Sistema nervioso",        "Células que procesan y transmiten señales",   TipoPilar.Vida, 2, 5_000,     15,    3, new[]{"vi_09"});
            mejoras[i++] = new DefinicionMejora("vi_16","Cerebro primitivo",       "Un órgano dedicado a procesar el entorno",    TipoPilar.Vida, 2, 40_000,    75,    3, new[]{"vi_15"});
            mejoras[i++] = new DefinicionMejora("vi_17","Conducta social",         "Grupos cooperativos superan al individuo",    TipoPilar.Vida, 2, 280_000,   375,   4, new[]{"vi_16"});
            mejoras[i++] = new DefinicionMejora("vi_18","Uso de herramientas",     "La tecnología amplifica las capacidades",     TipoPilar.Vida, 2, 2_000_000, 1875,  5, new[]{"vi_17"});
            mejoras[i++] = new DefinicionMejora("vi_19","Lenguaje",                "La comunicación simbólica transmite cultura", TipoPilar.Vida, 2, 14_000_000,9375,  6, new[]{"vi_18"});
            mejoras[i++] = new DefinicionMejora("vi_20","Consciencia colectiva",   "La humanidad piensa como un organismo único", TipoPilar.Vida, 2, 100_000_000,46875,7,new[]{"vi_19"});
            mejoras[i++] = new DefinicionMejora("vi_21","Singularidad",            "IA y biología se fusionan: fin del límite",   TipoPilar.Vida, 2, 1e9,       234375,8,new[]{"vi_20"});

            return mejoras;
        }
    }
}
