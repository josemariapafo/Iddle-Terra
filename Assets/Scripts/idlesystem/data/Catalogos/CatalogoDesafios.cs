using System;
using Terra.Core;
using Terra.Data;
using Terra.State;

namespace Terra.Data.Catalogos
{
    public static class CatalogoDesafios
    {
        public static DefinicionDesafio[] Crear() => new[]
        {
            // 1. Planeta Seco — sin Océanos, llegar a Era 3
            new DefinicionDesafio(
                "ds_01","Planeta Seco",
                "Completa la evolución sin activar el pilar Océanos",
                5, "Pilar Océanos bloqueado", "Llegar a Era 3",
                1.25,
                s => s.Era >= 3,
                e =>
                {
                    e.PilaresBloqueadosDesafio[(int)TipoPilar.Oceanos] = true;
                }),

            // 2. Sin Tectónica — sin cadenas, llegar a Era 2
            new DefinicionDesafio(
                "ds_02","Sin Tectónica",
                "Sobrevive sin el sistema de infraestructura",
                5, "Cadenas desactivadas", "Llegar a Era 2",
                1.10,
                s => s.Era >= 2,
                e =>
                {
                    e.CadenasBloqueadasDesafio = true;
                }),

            // 3. Sprint — 30 minutos para llegar a la máxima era posible
            // La "victoria" es reclamar el bonus antes de que acabe el tiempo
            new DefinicionDesafio(
                "ds_03","Sprint",
                "Carrera contrarreloj. Cuantas más eras, mayor recompensa",
                5, "30 minutos de tiempo límite", "Llegar a la era más alta posible",
                1.02, // base + 2% por era (escalado posterior)
                s => s.Era >= 3, // mínimo Era 3 para contar victoria
                e =>
                {
                    e.TiempoRestanteDesafio = 30f * 60f; // 30 min
                }),

            // 4. Minimalista — máximo 20 compras totales, llegar a Era 3
            new DefinicionDesafio(
                "ds_04","Minimalista",
                "La elegancia de lo mínimo. Cada compra cuenta",
                5, "Máximo 20 compras de mejora totales", "Llegar a Era 3",
                1.50,
                s => s.Era >= 3,
                e =>
                {
                    e.MaxComprasDesafio = 20;
                    e.ComprasEnDesafio = 0;
                }),

            // 5. Desequilibrio — solo Tierra y Atmósfera, llegar a Era 4
            new DefinicionDesafio(
                "ds_05","Desequilibrio",
                "Solo dos pilares permitidos: Tierra y Atmósfera",
                5, "Océanos y Vida bloqueados", "Llegar a Era 4",
                1.30,
                s => s.Era >= 4,
                e =>
                {
                    e.PilaresBloqueadosDesafio[(int)TipoPilar.Oceanos] = true;
                    e.PilaresBloqueadosDesafio[(int)TipoPilar.Vida] = true;
                }),

            // 6. Sin Prestige — no puedes hacer Extinción, llegar a Era 5
            new DefinicionDesafio(
                "ds_06","Sin Prestige",
                "Sin resets. Un solo intento para llegar lejos",
                5, "Extinción/Glaciación/BigBang bloqueados", "Llegar a Era 5",
                1.50,
                s => s.Era >= 5,
                e =>
                {
                    e.PrestigeBloqueadoDesafio = true;
                }),

            // 7. Velocidad — llegar a Era 3 en 10 min
            new DefinicionDesafio(
                "ds_07","Velocidad",
                "Demuestra tu dominio del ritmo de progresión",
                5, "10 minutos de tiempo límite", "Llegar a Era 3 antes del tiempo",
                1.20,
                s => s.Era >= 3,
                e =>
                {
                    e.TiempoRestanteDesafio = 10f * 60f; // 10 min
                }),

            // 8. Austeridad — solo mejoras zona 0, llegar a Era 3
            new DefinicionDesafio(
                "ds_08","Austeridad",
                "Solo las mejoras más básicas. Ni zona media ni avanzadas",
                5, "Solo mejoras de zona 0 (básicas)", "Llegar a Era 3",
                1.40,
                s => s.Era >= 3,
                e =>
                {
                    e.SoloZona0Desafio = true;
                }),
        };
    }
}
