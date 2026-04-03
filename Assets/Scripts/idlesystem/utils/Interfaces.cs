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

    public enum TipoBonus { MultiplicadorEV, MultiplicadorPilar, RecursoExtra, DesbloqueoInstante }

    public enum TipoEslabon { Generacion, Procesamiento, Distribucion }

    public enum TipoMision { Compra, Produccion, Sinergia, Era, Prestige, Combo, CuelloBotella }

    public enum TipoRecompensa { EVInstante, MultiplicadorTemporal, NivelNodoGratis, FosilesExtra }

    public enum TipoCodice { Abundancia, Eficiencia, Dominio }
}
