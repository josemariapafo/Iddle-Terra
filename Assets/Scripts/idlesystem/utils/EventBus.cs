using System;
using System.Collections.Generic;

namespace Terra.Core
{
    /// <summary>
    /// Bus de eventos tipado. Desacopla completamente los sistemas entre sí
    /// y de la UI. Nadie se referencia directamente — todos hablan a través del bus.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _suscriptores
            = new Dictionary<Type, List<Delegate>>();

        public static void Suscribir<T>(Action<T> callback)
        {
            var tipo = typeof(T);
            if (!_suscriptores.ContainsKey(tipo))
                _suscriptores[tipo] = new List<Delegate>();
            _suscriptores[tipo].Add(callback);
        }

        public static void Desuscribir<T>(Action<T> callback)
        {
            var tipo = typeof(T);
            if (_suscriptores.ContainsKey(tipo))
                _suscriptores[tipo].Remove(callback);
        }

        public static void Publicar<T>(T evento)
        {
            var tipo = typeof(T);
            if (!_suscriptores.ContainsKey(tipo)) return;

            // Copia para evitar modificaciones durante iteración
            var lista = new List<Delegate>(_suscriptores[tipo]);
            foreach (var suscriptor in lista)
                (suscriptor as Action<T>)?.Invoke(evento);
        }

        public static void LimpiarTodo() => _suscriptores.Clear();
    }

    // ── Eventos del juego ─────────────────────────────────────────────────

    public readonly struct EventoEVCambia      { public readonly double Cantidad; public EventoEVCambia(double v) => Cantidad = v; }
    public readonly struct EventoMejoraComprada { public readonly string IdMejora; public readonly int NuevoNivel; public EventoMejoraComprada(string id, int n) { IdMejora = id; NuevoNivel = n; } }
    public readonly struct EventoMejoraDesbloqueada { public readonly string IdMejora; public EventoMejoraDesbloqueada(string id) => IdMejora = id; }
    public readonly struct EventoSinergiaActivada  { public readonly string IdSinergia; public EventoSinergiaActivada(string id) => IdSinergia = id; }
    public readonly struct EventoEraAvanzada       { public readonly int NuevaEra; public EventoEraAvanzada(int e) => NuevaEra = e; }
    public readonly struct EventoPrestigeRealizado { public readonly TipoPrestige Tipo; public readonly double Ganancia; public EventoPrestigeRealizado(TipoPrestige t, double g) { Tipo = t; Ganancia = g; } }
    public readonly struct EventoNodoDesbloqueado  { public readonly string IdNodo; public EventoNodoDesbloqueado(string id) => IdNodo = id; }
    public readonly struct EventoLogroDesbloqueado { public readonly string IdLogro; public EventoLogroDesbloqueado(string id) => IdLogro = id; }
    public readonly struct EventoEstancamientoDetectado { public readonly TipoPrestige PrestigeSugerido; public readonly double GananciaEstimada; public EventoEstancamientoDetectado(TipoPrestige t, double g) { PrestigeSugerido = t; GananciaEstimada = g; } }
    public readonly struct EventoEventoActivado    { public readonly string IdEvento; public EventoEventoActivado(string id) => IdEvento = id; }
    public readonly struct EventoRachaActualizada  { public readonly int DiasConsecutivos; public EventoRachaActualizada(int d) => DiasConsecutivos = d; }
    public readonly struct EventoOfflineCalculado  { public readonly double EVGanada; public readonly double SegundosOffline; public EventoOfflineCalculado(double ev, double s) { EVGanada = ev; SegundosOffline = s; } }
    public readonly struct EventoRecursoDesbloqueado { public readonly string IdRecurso; public EventoRecursoDesbloqueado(string id) => IdRecurso = id; }
    public readonly struct EventoDesafioCompletado { public readonly string IdDesafio; public EventoDesafioCompletado(string id) => IdDesafio = id; }
    public readonly struct EventoHitoAlcanzado     { public readonly string IdMejora; public readonly int Nivel; public EventoHitoAlcanzado(string id, int n) { IdMejora = id; Nivel = n; } }
    public readonly struct EventoCadenaComprada    { public readonly string IdSubMejora; public readonly int NuevoNivel; public EventoCadenaComprada(string id, int n) { IdSubMejora = id; NuevoNivel = n; } }
    public readonly struct EventoMisionCompletada  { public readonly string IdMision; public EventoMisionCompletada(string id) => IdMision = id; }
    public readonly struct EventoNodoCodiceComprado { public readonly string IdNodo; public readonly int NuevoNivel; public EventoNodoCodiceComprado(string id, int n) { IdNodo = id; NuevoNivel = n; } }
    public readonly struct EventoBifurcacionElegida   { public readonly TipoPilar Pilar; public readonly int Opcion; public EventoBifurcacionElegida(TipoPilar p, int o) { Pilar = p; Opcion = o; } }
    public readonly struct EventoBifurcacionRequerida { public readonly TipoPilar Pilar; public readonly string IdBifurcacion; public EventoBifurcacionRequerida(TipoPilar p, string id) { Pilar = p; IdBifurcacion = id; } }
    public readonly struct EventoComboActivado     { }
    public readonly struct EventoUIDesbloqueado    { public readonly string Elemento; public EventoUIDesbloqueado(string e) => Elemento = e; }
}
