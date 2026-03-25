using System;

namespace Terra.Core
{
    public static class Formateador
    {
        private static readonly (double umbral, string sufijo)[] _notacion =
        {
            (1e18, "Qi"),
            (1e15, "Q"),
            (1e12, "T"),
            (1e9,  "G"),
            (1e6,  "M"),
            (1e3,  "K"),
        };

        public static string Numero(double n, int decimales = 2)
        {
            if (double.IsNaN(n) || double.IsInfinity(n)) return "∞";
            if (n < 0) return "-" + Numero(-n, decimales);

            foreach (var (umbral, sufijo) in _notacion)
                if (n >= umbral)
                    return $"{Math.Round(n / umbral, decimales)}{sufijo}";

            return n < 1000 ? $"{n:F0}" : $"{n:N0}";
        }

        public static string Tiempo(double segundos)
        {
            if (segundos < 60)   return $"{segundos:F0}s";
            if (segundos < 3600) return $"{segundos / 60:F0}m";
            return $"{segundos / 3600:F1}h";
        }

        public static string Porcentaje(double valor, int decimales = 1) =>
            $"{Math.Round(valor * 100, decimales)}%";

        public static string Multiplicador(double mult) =>
            mult >= 1 ? $"×{mult:F2}" : $"÷{(1.0 / mult):F2}";
    }
}
