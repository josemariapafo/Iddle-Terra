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
            // Nodos Códice Genético (T25): id:nivel separados por |
            public string nodosCodiceGenetico;
            // Bifurcaciones (T26): pilar:opcion separados por |  (pilar = 0..3, opcion = 0|1)
            public string bifurcaciones;
            // Automatizaciones activas: "1|0|1|0|1" (5 flags)
            public string automatizacionesActivas;
            // Revelación progresiva
            public double evMaximoAlcanzado;
            public int eraMaximaAlcanzada;

            // Desafíos (T24)
            public string desafiosCompletados;          // ids separados por |
            public string desafioActivoId;              // id del desafío en curso (o "")
            public float  tiempoRestanteDesafio;        // segundos restantes si tenía límite
            public int    comprasEnDesafio;             // contador para restricción Minimalista
            public double bonusDesafiosCompletados;     // multiplicador acumulado
            public string pilaresBloqueadosDesafio;     // "1|0|0|1" (4 flags)
            public bool   cadenasBloqueadasDesafio;
            public bool   prestigeBloqueadoDesafio;
            public bool   soloZona0Desafio;
            public int    maxComprasDesafio;
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

            // Serializar nodos Códice Genético
            var partesCodiceGen = new System.Text.StringBuilder();
            foreach (var kv in estado.NodosCodiceGenetico)
                if (kv.Value.Nivel > 0)
                    partesCodiceGen.Append($"{kv.Key}:{kv.Value.Nivel}|");
            datos.nodosCodiceGenetico = partesCodiceGen.ToString();

            // Serializar bifurcaciones (solo las que tienen opción elegida ≠ -1)
            var partesBif = new System.Text.StringBuilder();
            foreach (var kv in estado.Bifurcaciones)
                if (kv.Value >= 0)
                    partesBif.Append($"{(int)kv.Key}:{kv.Value}|");
            datos.bifurcaciones = partesBif.ToString();

            // Serializar automatizaciones activas
            if (estado.AutomatizacionesActivas != null)
            {
                var partesAuto = new System.Text.StringBuilder();
                for (int i = 0; i < estado.AutomatizacionesActivas.Length; i++)
                {
                    if (i > 0) partesAuto.Append('|');
                    partesAuto.Append(estado.AutomatizacionesActivas[i] ? '1' : '0');
                }
                datos.automatizacionesActivas = partesAuto.ToString();
            }

            // Revelación progresiva
            datos.evMaximoAlcanzado = estado.EVMaximoAlcanzado;
            datos.eraMaximaAlcanzada = estado.EraMaximaAlcanzada;

            // ── Desafíos (T24) ────────────────────────────────────────
            var partesDesafios = new System.Text.StringBuilder();
            foreach (var kv in estado.Desafios)
                if (kv.Value.Completado)
                    partesDesafios.Append($"{kv.Key}|");
            datos.desafiosCompletados = partesDesafios.ToString();

            datos.desafioActivoId          = estado.DesafioActivoId ?? "";
            datos.tiempoRestanteDesafio    = estado.TiempoRestanteDesafio;
            datos.comprasEnDesafio         = estado.ComprasEnDesafio;
            datos.bonusDesafiosCompletados = estado.BonusDesafiosCompletados;

            var partesPilaresBloq = new System.Text.StringBuilder();
            for (int i = 0; i < estado.PilaresBloqueadosDesafio.Length; i++)
            {
                if (i > 0) partesPilaresBloq.Append('|');
                partesPilaresBloq.Append(estado.PilaresBloqueadosDesafio[i] ? '1' : '0');
            }
            datos.pilaresBloqueadosDesafio = partesPilaresBloq.ToString();
            datos.cadenasBloqueadasDesafio = estado.CadenasBloqueadasDesafio;
            datos.prestigeBloqueadoDesafio = estado.PrestigeBloqueadoDesafio;
            datos.soloZona0Desafio         = estado.SoloZona0Desafio;
            datos.maxComprasDesafio        = estado.MaxComprasDesafio;

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

                // Cargar nodos Códice Genético
                if (!string.IsNullOrEmpty(datos.nodosCodiceGenetico))
                    foreach (var parte in datos.nodosCodiceGenetico.Split('|'))
                    {
                        if (string.IsNullOrEmpty(parte)) continue;
                        var kv = parte.Split(':');
                        if (kv.Length != 2) continue;
                        string id = kv[0];
                        if (int.TryParse(kv[1], out int nivel) && estado.NodosCodiceGenetico.ContainsKey(id))
                            estado.NodosCodiceGenetico[id].Nivel = nivel;
                    }

                // Cargar bifurcaciones
                if (!string.IsNullOrEmpty(datos.bifurcaciones))
                    foreach (var parte in datos.bifurcaciones.Split('|'))
                    {
                        if (string.IsNullOrEmpty(parte)) continue;
                        var kv = parte.Split(':');
                        if (kv.Length != 2) continue;
                        if (int.TryParse(kv[0], out int pilar)
                            && int.TryParse(kv[1], out int opcion)
                            && pilar >= 0 && pilar <= 3
                            && (opcion == 0 || opcion == 1))
                        {
                            estado.Bifurcaciones[(TipoPilar)pilar] = opcion;
                        }
                    }

                // Cargar automatizaciones activas
                if (!string.IsNullOrEmpty(datos.automatizacionesActivas))
                {
                    var partes = datos.automatizacionesActivas.Split('|');
                    if (estado.AutomatizacionesActivas == null
                        || estado.AutomatizacionesActivas.Length < partes.Length)
                        estado.AutomatizacionesActivas = new bool[partes.Length];
                    for (int i = 0; i < partes.Length && i < estado.AutomatizacionesActivas.Length; i++)
                        estado.AutomatizacionesActivas[i] = partes[i] == "1";
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

                // ── Cargar desafíos (T24) ─────────────────────────────
                if (!string.IsNullOrEmpty(datos.desafiosCompletados))
                    foreach (var id in datos.desafiosCompletados.Split('|'))
                    {
                        if (string.IsNullOrEmpty(id)) continue;
                        if (estado.Desafios.TryGetValue(id, out var est))
                            est.Completado = true;
                    }

                estado.DesafioActivoId       = string.IsNullOrEmpty(datos.desafioActivoId) ? null : datos.desafioActivoId;
                estado.TiempoRestanteDesafio = datos.tiempoRestanteDesafio;
                estado.ComprasEnDesafio      = datos.comprasEnDesafio;

                // Si había un desafío activo, marcar su EstadoDesafio como Activo
                if (!string.IsNullOrEmpty(estado.DesafioActivoId)
                    && estado.Desafios.TryGetValue(estado.DesafioActivoId, out var estActivo))
                {
                    estActivo.Activo = true;
                }

                // El bono acumulado se recalcula desde RecalcularBonusAcumulado tras cargar,
                // pero guardamos el valor por si acaso
                if (datos.bonusDesafiosCompletados > 0)
                    estado.BonusDesafiosCompletados = datos.bonusDesafiosCompletados;

                if (!string.IsNullOrEmpty(datos.pilaresBloqueadosDesafio))
                {
                    var partes = datos.pilaresBloqueadosDesafio.Split('|');
                    for (int i = 0; i < partes.Length && i < estado.PilaresBloqueadosDesafio.Length; i++)
                        estado.PilaresBloqueadosDesafio[i] = partes[i] == "1";
                }
                estado.CadenasBloqueadasDesafio = datos.cadenasBloqueadasDesafio;
                estado.PrestigeBloqueadoDesafio = datos.prestigeBloqueadoDesafio;
                estado.SoloZona0Desafio         = datos.soloZona0Desafio;
                estado.MaxComprasDesafio        = datos.maxComprasDesafio;

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
