using System.Linq;
using Terra.Core;
using Terra.Data;
using Terra.State;
using UnityEngine;

namespace Terra.Systems
{
    /// <summary>
    /// Gestiona la compra y desbloqueo de mejoras.
    /// Solo responsabilidad: mutate EstadoMejora + publicar eventos.
    /// </summary>
    public class SistemaMejoras : ISistema
    {
        private readonly DefinicionMejora[] _definiciones;
        private EstadoJuego _estado;
        private SistemaCodice _codice;

        private static readonly int[] _nivelesHito = { 10, 25, 50, 100, 200, 500 };

        public SistemaMejoras(DefinicionMejora[] definiciones) =>
            _definiciones = definiciones;

        public void AsignarCodice(SistemaCodice codice) => _codice = codice;

        private double CosteConReduccion(DefinicionMejora def, int nivel)
        {
            double coste = def.CosteEnNivel(nivel);
            double reduccion = _codice?.ReduccionCosteMejoras() ?? 0.0;
            return coste * (1.0 - reduccion);
        }

        public void Inicializar()
        {
            // Estado se asigna desde GameController tras cargar
        }

        public void AsignarEstado(EstadoJuego estado)
        {
            _estado = estado;

            // Crear entrada de estado para cada mejora si no existe
            foreach (var def in _definiciones)
                if (!_estado.Mejoras.ContainsKey(def.Id))
                    _estado.Mejoras[def.Id] = new EstadoMejora(def.Id);

            // Desbloquear mejoras iniciales (era 1, sin requisitos)
            ComprobarDesbloqueos();
        }

        public void Actualizar(float delta) { }  // sin lógica de tiempo

        // ── Compra ────────────────────────────────────────────────────────

        public bool ComprarUno(string idMejora)
        {
            var def = BuscarDefinicion(idMejora);
            if (def == null) return false;

            var est = _estado.Mejoras[idMejora];
            double coste = CosteConReduccion(def, est.Nivel);

            if (!est.Desbloqueada || est.Nivel >= def.NivelMax) return false;
            if (_estado.EnergiaVital < coste) return false;

            _estado.EnergiaVital -= coste;
            est.Nivel++;
            _estado.MejorasCompradasEnSesion++;

            PublicarCompra(idMejora, est.Nivel, def);
            ComprobarDesbloqueos();
            return true;
        }

        public int ComprarN(string idMejora, int cantidad)
        {
            var def = BuscarDefinicion(idMejora);
            if (def == null) return 0;

            var est = _estado.Mejoras[idMejora];
            if (!est.Desbloqueada) return 0;

            int comprados = 0;
            while (comprados < cantidad && est.Nivel < def.NivelMax)
            {
                double coste = CosteConReduccion(def, est.Nivel);
                if (_estado.EnergiaVital < coste) break;

                _estado.EnergiaVital -= coste;
                est.Nivel++;
                comprados++;
                _estado.MejorasCompradasEnSesion++;
            }

            if (comprados > 0)
            {
                PublicarCompra(idMejora, est.Nivel, def);
                ComprobarDesbloqueos();
            }

            return comprados;
        }

        public int ComprarMax(string idMejora)
        {
            var def = BuscarDefinicion(idMejora);
            if (def == null) return 0;

            var est = _estado.Mejoras[idMejora];
            if (!est.Desbloqueada) return 0;

            int comprados = 0;
            while (est.Nivel < def.NivelMax)
            {
                double coste = CosteConReduccion(def, est.Nivel);
                if (_estado.EnergiaVital < coste) break;

                _estado.EnergiaVital -= coste;
                est.Nivel++;
                comprados++;
                _estado.MejorasCompradasEnSesion++;
            }

            if (comprados > 0)
            {
                PublicarCompra(idMejora, est.Nivel, def);
                ComprobarDesbloqueos();
            }

            return comprados;
        }

        // ── Desbloqueos ───────────────────────────────────────────────────

        public void ComprobarDesbloqueos()
        {
            foreach (var def in _definiciones)
            {
                var est = _estado.Mejoras[def.Id];
                if (est.Desbloqueada) continue;
                if (def.EraRequerida > _estado.EraActual) continue;

                if (!CumpleRequisitos(def)) continue;

                est.Desbloqueada = true;
                EventBus.Publicar(new EventoMejoraDesbloqueada(def.Id));
            }
        }

        private bool CumpleRequisitos(DefinicionMejora def)
        {
            // Comprobar nivel de pilar requerido
            if (def.NivelPilarRequisito > 0)
            {
                int nivelPilar = _definiciones
                    .Where(d => d.Pilar == def.PilarRequisito)
                    .Sum(d => _estado.NivelMejora(d.Id));

                if (nivelPilar < def.NivelPilarRequisito) return false;
            }

            // Comprobar mejoras previas
            foreach (var reqId in def.RequisitosIds)
                if (_estado.NivelMejora(reqId) <= 0) return false;

            return true;
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private void PublicarCompra(string id, int nivel, DefinicionMejora def)
        {
            EventBus.Publicar(new EventoMejoraComprada(id, nivel));

            // Comprobar hito
            if (_nivelesHito.Contains(nivel))
                EventBus.Publicar(new EventoHitoAlcanzado(id, nivel));
        }

        public DefinicionMejora BuscarDefinicion(string id)
        {
            foreach (var d in _definiciones)
                if (d.Id == id) return d;
            return null;
        }

        public DefinicionMejora[] ObtenerPorPilarYZona(TipoPilar pilar, int zona) =>
            _definiciones.Where(d => d.Pilar == pilar && d.Zona == zona).ToArray();

        public DefinicionMejora ObtenerPorId(string id)
        {
            foreach (var def in _definiciones)
                if (def.Id == id) return def;
            return null;
        }

        public DefinicionMejora[] ObtenerPorPilar(TipoPilar pilar) =>
            _definiciones.Where(d => d.Pilar == pilar).ToArray();

        public int NivelTotalPilar(TipoPilar pilar) =>
            _definiciones
                .Where(d => d.Pilar == pilar)
                .Sum(d => _estado.NivelMejora(d.Id));
    }
}
