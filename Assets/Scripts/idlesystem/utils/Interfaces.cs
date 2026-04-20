namespace Terra.Core
{
    public interface IProductor
    {
        double ProduccionPorSegundo { get; }
    }

    public interface IComprable
    {
        double CosteActual { get; }
        bool PuedeComprar(double evDisponible);
    }

    public interface ISistema
    {
        void Inicializar();
        void Actualizar(float delta);
    }

    public interface IReseteble
    {
        void Resetear(TipoReset tipo);
    }

    public enum TipoPilar { Atmosfera = 0, Oceanos = 1, Tierra = 2, Vida = 3 }

    public enum TipoPrestige { Extincion, Glaciacion, BigBang }

    public enum TipoReset { Parcial, Total }

    public enum TipoLogro { Visible, Oculto }

    public enum TipoEvento { Positivo, Negativo, Neutral }

    public enum TipoBonus
    {
        // ── Existentes (árbol evolución, logros) ──
        MultiplicadorEV, MultiplicadorPilar, RecursoExtra, DesbloqueoInstante,

        // ── Códice Fósil: Abundancia ──
        BonusNocturno,          // +X% producción nocturna extra
        BonusSinergias,         // +X% multiplicador sinergias

        // ── Códice Fósil: Eficiencia ──
        ReduccionCosteMejoras,  // -X% coste mejoras
        ReduccionCosteCadenas,  // -X% coste cadenas
        NivelesGratisInicio,    // +N niveles gratis Era 1 tras prestige
        BonusFosilesPrestige,   // +X% fósiles ganados en prestige
        BonusCapCadena,         // +X% cap de cadenas

        // ── Códice Fósil: Dominio ──
        BonusTap,               // +X% poder de tap
        ReduccionTapsCombo,     // -N taps para activar combo
        DuracionCombo,          // +Xs duración combo
        MultiplicadorCombo,     // +X multiplicador combo
        AutoTap,                // auto-tap (N taps por activación)

        // ── Códice Genético: Adaptación ── (T25, Era 6+)
        BonusCapCadenaGen,      // +X% cap cadenas (multiplicativo al de fósil)
        ReduccionReqEslabones,  // -N niveles requisito para desbloquear sub-mejoras cadena
        BonusGenesPrestige,     // +X% genes ganados al glaciar

        // ── Códice Genético: Mutación ──
        BonusEfectividadSinergias, // +X% multiplicador de sinergias (multiplicativo)
        ReduccionCooldownEventos,  // -X% cooldown entre eventos
        OpcionEventoExtra,         // desbloquea 4ª opción en eventos

        // ── Códice Genético: Simbiosis ──
        BonusPilaresBalanceados,   // +X% EV/s cuando 3+ pilares tienen nivel cercano
        MultiplicadorGlobalGen,    // +X% EV/s global (capstone genético)
        BonusMultiplicadoresBifurcacion // +X% a los multiplicadores de bifurcación
    }

    public enum TipoEslabon { Generacion, Procesamiento, Distribucion }

    public enum TipoMision { Compra, Produccion, Sinergia, Era, Prestige, Combo, CuelloBotella }

    public enum TipoRecompensa { EVInstante, MultiplicadorTemporal, NivelNodoGratis, FosilesExtra }

    public enum TipoCodice { Abundancia, Eficiencia, Dominio }

    /// <summary>
    /// Ramas del Códice Genético (Era 6). Se gasta Genes.
    /// Se resetea solo con Big Bang (no con Glaciación).
    /// </summary>
    public enum TipoCodiceGenetico { Adaptacion, Mutacion, Simbiosis }

    /// <summary>
    /// Automatizaciones de compra. Cada una de pilar compra periódicamente la
    /// mejora más barata desbloqueada del pilar asignado. SeleccionNatural es
    /// "smart": elige globalmente la de mejor incremento_prod / coste.
    /// </summary>
    public enum TipoAutomatizacion
    {
        Gravedad         = 0, // Tierra
        Corrientes       = 1, // Oceanos
        Viento           = 2, // Atmosfera
        EvolucionNatural = 3, // Vida
        SeleccionNatural = 4  // Smart (global)
    }
}
