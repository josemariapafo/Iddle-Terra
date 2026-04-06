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
        AutoTap                 // auto-tap (N taps por activación)
    }

    public enum TipoEslabon { Generacion, Procesamiento, Distribucion }

    public enum TipoMision { Compra, Produccion, Sinergia, Era, Prestige, Combo, CuelloBotella }

    public enum TipoRecompensa { EVInstante, MultiplicadorTemporal, NivelNodoGratis, FosilesExtra }

    public enum TipoCodice { Abundancia, Eficiencia, Dominio }
}
