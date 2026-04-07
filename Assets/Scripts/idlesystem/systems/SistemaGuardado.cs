using System;
using System.Collections.Generic;
using System.Globalization;
using Terra.Core;
using Terra.State;
using UnityEngine;

namespace Terra.Systems
{
    /// <summary>
    /// Gestiona guardado y carga. Serializa EstadoJuego a PlayerPrefs con JSON.
    /// En producción se puede cambiar el backend (archivo, nube) sin tocar el resto.
    /// </summary>
    public class SistemaGuardado
    {
        private const string KEY_GUARDADO = "terra_save_v1";
        private const float INTERVALO_AUTOSAVE = 30f;

        private float _timerAutoguardado;

        [Serializable]
        private class DatosSerializables
        {
            public double energiaVital;
            public int eraActual;
            public double tiempoJugado;
            public double fosiles, genes, quarks;
            public int vecesExtincion, vecesGlaciacion, vecesBigBang;
            public int diasRacha;
            public string ultimaConexion;

            // Mejoras: id:nivel separados por |
            public string mejoras;
            // Nodos: id:nivel separados por |
            public string nodos;
            // Logros completados: ids separados por |
            public string logros;
            // Cadenas: id:nivel separados por |
            public string cadenas;
            // Misiones activas: id:progreso separados por |
            public string misionesActivas;
            // Misiones completadas: ids separados por |
            public string misionesCompletadas;
            // Nodos Códice Fósil: id:nivel separados por |
            public string nodosCodice;
            // Revelación progresiva
            public double evMaximoAlcanzado;
            public int eraMaximaAlcanzada;
        }

        public void Guardar(EstadoJuego estado)
        {
            var datos = new DatosSerializables
            {
                energiaVital     = estado.EnergiaVital,
                eraActual        = estado.EraActual,
                tiempoJugado     = estado.TiempoJugadoTotal,
                fosiles          = estado.Prestige.Fosiles,
                genes            = estado.Prestige.Genes,
                quarks           = estado.Prestige.Quarks,
                vecesExtincion   = estado.Prestige.VecesExtincion,
                vecesGlaciacion  = estado.Prestige.VecesGlaciacion,
                vecesBigBang     = estado.Prestige.VecesBigBang,
                diasRacha        = estado.Racha.DiasConsecutivos,
                ultimaConexion   = estado.Racha.UltimaConexion.ToBinary().ToString(),
            };

            // Serializar mejoras
            var partesMejoras = new System.Text.StringBuilder();
            foreach (var kv in estado.Mejoras)
                if (kv.Value.Nivel > 0)
                    partesMejoras.Append($"{kv.Key}:{kv.Value.Nivel}|");
            datos.mejoras = partesMejoras.ToString();

            // Serializar nodos
            var partesNodos = new System.Text.StringBuilder();
            foreach (var kv in estado.Nodos)
                if (kv.Value.Nivel > 0)
                    partesNodos.Append($"{kv.Key}:{kv.Value.Nivel}|");
            datos.nodos = partesNodos.ToString();

            // Serializar logros
            var partesLogros = new System.Text.StringBuilder();
            foreach (var kv in estado.Logros)
                if (kv.Value.Completado)
                    partesLogros.Append($"{kv.Key}|");
            datos.logros = partesLogros.ToString();

            // Serializar cadenas
            var partesCadenas = new System.Text.StringBuilder();
            foreach (var kv in estado.Cadenas)
                if (kv.Value.Nivel > 0)
                    partesCadenas.Append($"{kv.Key}:{kv.Value.Nivel}|");
            datos.cadenas = partesCadenas.ToString();

            // Serializar misiones activas (id:progreso)
            var partesMisiones = new System.Text.StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                var m = estado.MisionesActivas[i];
                if (m != null && !string.IsNullOrEmpty(m.Id))
                    partesMisiones.Append($"{m.Id}:{m.ProgresoActual.ToString(CultureInfo.InvariantCulture)}:{(m.Completada ? 1 : 0)}|");
            }
            datos.misionesActivas = partesMisiones.ToString();

            // Serializar misiones completadas (id:recogida)
            var partesCompletadas = new System.Text.StringBuilder();
            foreach (var mc in estado.MisionesCompletadas)
                partesCompletadas.Append($"{mc.Id}:{(mc.RecompensaRecogida ? 1 : 0)}|");
            datos.misionesCompletadas = partesCompletadas.ToString();

            // Serializar nodos Códice Fósil
            var partesCodice = new System.Text.StringBuilder();
            foreach (var kv in estado.NodosCodice)
                if (kv.Value.Nivel > 0)
                    partesCodice.Append($"{kv.Key}:{kv.Value.Nivel}|");
            datos.nodosCodice = partesCodice.ToString();

            // Revelación progresiva
            datos.evMaximoAlcanzado = estado.EVMaximoAlcanzado;
            datos.eraMaximaAlcanzada = estado.EraMaximaAlcanzada;

            string json = JsonUtility.ToJson(datos);
            PlayerPrefs.SetString(KEY_GUARDADO, json);
            PlayerPrefs.Save();
        }

        public bool Cargar(EstadoJuego estado)
        {
            string json = PlayerPrefs.GetString(KEY_GUARDADO, "");
            if (string.IsNullOrEmpty(json)) return false;

            try
            {
                var datos = JsonUtility.FromJson<DatosSerializables>(json);

                estado.EnergiaVital          = datos.energiaVital;
                estado.EraActual             = Mathf.Clamp(datos.eraActual, 1, 8);
                estado.TiempoJugadoTotal     = datos.tiempoJugado;
                estado.Prestige.Fosiles      = datos.fosiles;
                estado.Prestige.Genes        = datos.genes;
                estado.Prestige.Quarks       = datos.quarks;
                estado.Prestige.VecesExtincion  = datos.vecesExtincion;
                estado.Prestige.VecesGlaciacion = datos.vecesGlaciacion;
                estado.Prestige.VecesBigBang    = datos.vecesBigBang;
                estado.Racha.DiasConsecutivos   = datos.diasRacha;

                if (long.TryParse(datos.ultimaConexion, out long bin))
                    estado.Racha.UltimaConexion = DateTime.FromBinary(bin);

                // Revelación progresiva
                estado.EVMaximoAlcanzado = datos.evMaximoAlcanzado;
                estado.EraMaximaAlcanzada = System.Math.Max(datos.eraMaximaAlcanzada, 1);

                // Cargar mejoras
                if (!string.IsNullOrEmpty(datos.mejoras))
                    foreach (var parte in datos.mejoras.Split('|'))
                    {
                        if (string.IsNullOrEmpty(parte)) continue;
                        var kv = parte.Split(':');
                        if (kv.Length != 2) continue;
                        string id = kv[0];
                        if (int.TryParse(kv[1], out int nivel) && estado.Mejoras.ContainsKey(id))
                            estado.Mejoras[id].Nivel = nivel;
                    }

                // Cargar nodos
                if (!string.IsNullOrEmpty(datos.nodos))
                    foreach (var parte in datos.nodos.Split('|'))
                    {
                        if (string.IsNullOrEmpty(parte)) continue;
                        var kv = parte.Split(':');
                        if (kv.Length != 2) continue;
                        string id = kv[0];
                        if (int.TryParse(kv[1], out int nivel) && estado.Nodos.ContainsKey(id))
                            estado.Nodos[id].Nivel = nivel;
                    }

                // Cargar logros
                if (!string.IsNullOrEmpty(datos.logros))
                    foreach (var id in datos.logros.Split('|'))
                    {
                        if (string.IsNullOrEmpty(id)) continue;
                        if (estado.Logros.ContainsKey(id))
                        {
                            estado.Logros[id].Completado = true;
                            estado.Logros[id].FechaCompletado = DateTime.UtcNow;
                        }
                    }

                // Cargar cadenas
                if (!string.IsNullOrEmpty(datos.cadenas))
                    foreach (var parte in datos.cadenas.Split('|'))
                    {
                        if (string.IsNullOrEmpty(parte)) continue;
                        var kv = parte.Split(':');
                        if (kv.Length != 2) continue;
                        string id = kv[0];
                        if (int.TryParse(kv[1], out int nivel) && estado.Cadenas.ContainsKey(id))
                            estado.Cadenas[id].Nivel = nivel;
                    }

                // Cargar nodos Códice Fósil
                if (!string.IsNullOrEmpty(datos.nodosCodice))
                    foreach (var parte in datos.nodosCodice.Split('|'))
                    {
                        if (string.IsNullOrEmpty(parte)) continue;
                        var kv = parte.Split(':');
                        if (kv.Length != 2) continue;
                        string id = kv[0];
                        if (int.TryParse(kv[1], out int nivel) && estado.NodosCodice.ContainsKey(id))
                            estado.NodosCodice[id].Nivel = nivel;
                    }

                // Cargar misiones completadas (id:recogida o solo id para compat)
                if (!string.IsNullOrEmpty(datos.misionesCompletadas))
                    foreach (var parte in datos.misionesCompletadas.Split('|'))
                    {
                        if (string.IsNullOrEmpty(parte)) continue;
                        var kv = parte.Split(':');
                        string id = kv[0];
                        bool recogida = kv.Length >= 2 && kv[1] == "1";
                        estado.MisionesCompletadas.Add(new MisionCompletada(id) { RecompensaRecogida = recogida });
                    }

                // Cargar misiones activas (id:progreso:completada)
                if (!string.IsNullOrEmpty(datos.misionesActivas))
                {
                    int slot = 0;
                    foreach (var parte in datos.misionesActivas.Split('|'))
                    {
                        if (string.IsNullOrEmpty(parte) || slot >= 3) continue;
                        var kv = parte.Split(':');
                        if (kv.Length < 2) continue;
                        string id = kv[0];
                        double.TryParse(kv[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double progreso);
                        bool completada = kv.Length >= 3 && kv[2] == "1";
                        estado.MisionesActivas[slot] = new EstadoMision(id)
                        {
                            ProgresoActual = progreso,
                            Completada = completada
                        };
                        slot++;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[TERRA] Error al cargar partida: {e.Message}");
                return false;
            }
        }

        public void BorrarGuardado() => PlayerPrefs.DeleteKey(KEY_GUARDADO);

        public void Tick(float delta, EstadoJuego estado)
        {
            _timerAutoguardado -= delta;
            if (_timerAutoguardado <= 0)
            {
                Guardar(estado);
                _timerAutoguardado = INTERVALO_AUTOSAVE;
            }
        }
    }
}
