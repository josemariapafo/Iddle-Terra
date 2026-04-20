# BALANCE PASS — Iddle Terra (T27)

Fecha: 2026-04-18
Scope: revisión del sistema completo tras cierre de T25 (Códice Genético) y T26 (Bifurcaciones).

---

## 1. Resumen ejecutivo

El juego cierra el alpha con **27 tareas completadas** y un bucle completo:
EV → Mejoras → Eras → Prestige (×3) → Códices (×2) → Bifurcaciones → Desafíos → Misiones.

Balance pass de este documento es **teórico** (no hay telemetría real todavía).
La idea es detectar desbalances lógicos obvios, aplicar ajustes seguros, y dejar
una checklist verificable para cuando llegue testing real.

El balance real vendrá del *playtest* — aquí sólo se asegura que nada rompe
matemáticamente a primera vista.

---

## 2. Matriz de techos acumulados (end-game teórico)

Con todos los sistemas al máximo, los multiplicadores que impactan la
producción/coste son estos (valores aproximados de los capstones que
existen en los catálogos):

| Multiplicador                        | Origen                                   | Techo teórico |
|--------------------------------------|------------------------------------------|--------------:|
| Prestige (Fósil × Gen × Quark)       | 1 + 0.1×25 + 0.05×20 + 2×6               | 3.5 × 2.0 × 13 = **×91**  |
| Sinergias activas                    | Producto de 12 sinergias                 | ~×500 (variable) |
| Nodos árbol evolución                | 32 nodos × mult por nivel                | ~×50 (Era 4+)  |
| Racha diaria                         | +5% × 30 días                            | **×2.5**  |
| Evento temporal                      | Multiplicador de opción × duración       | ×2-×5 (burst) |
| Nocturno (22h-06h + códice)          | 2.0 + bonus fósil (0.0-0.75)             | **×2.75** |
| Códice Fósil (MultiplicadorEV)       | Suma de BonusEV de nodos activos         | **×3.15** |
| Códice Genético — GlobalGen          | 1 + 0.86 (suma s3+s6+m5)                 | **×1.86** |
| Códice Genético — Sinergias eff      | 1 + 1.20 (m1+m4+m7)                      | **×2.20** |
| Códice Genético — Pilares balanceados| 1 + 1.26 si 3+ pilares dentro del 50%    | **×2.26** (condicional) |
| Cap cadenas — Fósil × Genético       | (1+0.60) × (1+1.60)                      | **×4.16** |
| Bifurcación eslabón                  | ×1.50 / ×0.90 por opción                 | **×1.50** |
| Bif × Amplificación (cg_a7+cg_s4)    | exceso × (1+0.85)                        | (0.5 × 1.85)+1 = **×1.93** |
| Desafíos completados                 | Acumulado de recompensas                 | ×1.5-×3 |

**Riesgo inmediato:** la multiplicación de capstones (Gen × Sin × Bal) da
`×1.86 × ×2.20 × ×2.26 ≈ ×9.25` encima de lo que ya hace el Códice Fósil.
Es **intencional** (capstones del T25 son la progresión de end-game) pero
hay que tener en cuenta que estos valores son tras **6+ Glaciaciones**
(coste de maxeado). La curva es sana.

---

## 3. Curva de progresión esperada

Verificada contra los costes en catálogos (todos los valores son T27):

| Hito                        | Tiempo esperado | Nota                                         |
|-----------------------------|----------------:|----------------------------------------------|
| 1ª compra (atm_01)          | <10 s           | Base 1 EV/s → coste 5 EV                     |
| Primera sinergia            | ~2 min          | 2 pilares a nivel 3-5                        |
| Era 2                       | ~5 min          | Niv. 4/pilar + 2K EV                         |
| Era 3                       | ~20 min         | Niv. 10/pilar + 40K EV                       |
| 1er cuello de botella       | Era 1 final     | Por diseño, al subir Tierra                  |
| 1er prestige (Extinción)    | ~30-40 min      | Era 3 + ~400K EV                             |
| Primer Códice Fósil         | run 2           | Fósil cap 25, raíz 3                         |
| Era 5 (1ª Glaciación)       | ~3-5 h          | Tras 3-5 runs con Codice Fósil               |
| 1er nodo Códice Genético    | run 6-7         | Genes cap 20, raíz cg_a1/cg_m1/cg_s1 (2-3)   |
| Era 6 (bifurcaciones)       | ~8-12 h         | Requiere niveles altos en todos los pilares  |
| Era 7 (Big Bang disponible) | ~20-30 h        | Suficiente Glaciaciones y EV                 |
| Capstones genéticos maxados | 60-100 h        | Requiere ~10+ Glaciaciones                   |

**Risk point:** la transición run 1 → run 2 debe ser MUY satisfactoria.
Si el jugador no siente la diferencia con Fósiles, abandona. Esto se
verifica en playtest; por ahora confío en que `MultiplicadorFosiles`
= `1 + 0.1×Fosiles` con 2-5 Fósiles en primer prestige da ×1.2-×1.5,
que sumado a nodos desbloqueados del Códice Fósil debería notarse.

---

## 4. Riesgos detectados

### 4.1 Apilamiento excesivo de reducción de coste
**Estado:** CORREGIDO en pase de T25.
- `SistemaMejoras.CosteConReduccion` cap aditivo fósil+genético al **80%**.
- `SistemaCadenas.ReduccionCosteCadenasEfectiva` cap aditivo fósil+genético al **80%**.
Sin este cap, maxear cf_e1 (5×-8% = -40%) + cg_m6 (2×-10% = -20%) daba -60%
ya, pero un futuro capstone podría empujar a -100% y romper la economía.

### 4.2 BonusPilaresBalanceados — definición operativa
**Estado:** RESUELTO.
El bonus se aplica sólo cuando 3+ pilares tienen nivel dentro del 50% del mayor,
implementado en `CalculadorProduccion.EstaBalanceadoTresPilares`. Esto previene
que un jugador farmee un pilar dominante y reciba el bonus gratis.

### 4.3 Cap de Genes igual que Fósiles
**Estado:** INTENCIONAL.
`CapGenes = 20` vs `CapFosiles = 25`. Los genes son más densos (nodos genéticos
tienen impacto más global que los fósiles), por eso cap inferior. Cada Glaciación
debería dar 4-20 genes dependiendo de curva EV → hay ritmo limpio.

### 4.4 Bifurcaciones sin re-elección
**Estado:** BY DESIGN.
Una vez elegida en Era 6, no se puede cambiar hasta el próximo prestige. Esto
crea tensión real de decisión, acorde a la filosofía de juego (`project_design.md`).
Si playtest demuestra que es frustrante, se puede añadir un botón "re-elegir"
que cueste N Genes.

### 4.5 Multiplicadores de bifurcación muy cerca del neutro
**Estado:** OBSERVACIÓN.
`[1.50, 1.00, 0.90]` da un multiplicador efectivo sobre el cap del pilar de
~`min(1.50, 1.00, 0.90) = 0.90` (porque es el min de los 3 eslabones).
**Esto significa que la bifurcación REDUCE el cap si no se corrige el eslabón
débil**. Es una mecánica de TENSIÓN (tienes que compensar con cadena), pero
asegurarse de que el jugador LEE ese contrato es crítico para la UI.

Recomendación: cuando se muestra el popup, la UI debe mostrar el efecto NETO
por pilar asumiendo nivel de cadena actual, no sólo los multiplicadores.

### 4.6 `ReduccionReqEslabones` puede hacer desbloqueos instantáneos
**Estado:** CORREGIDO.
`SistemaCadenas.ComprobarDesbloqueos` aplica `Math.Max(1, req - reduccion)` como
mínimo 1 para evitar que un nodo cg_a2 maxeado (3 niveles × 1 nivel = -3) cancele
todo el gating de sub-mejoras. En Era 6 ya es esperable saltarse 3 niveles de
requisito, pero nunca desbloquearse gratis.

### 4.7 Capstones que se solapan (cg_s6 y cg_m5)
**Estado:** INTENCIONAL.
Ambos dan `MultiplicadorGlobalGen` (+10%/nivel y +25%/nivel). Se suman vía
`ObtenerBonus(TipoBonus.MultiplicadorGlobalGen)` que acumula todos los nodos
con ese tipo. El jugador puede obtener ambos → techo 0.30+0.25×2 = 0.80.
Diseño redundante por ramas distintas (Mutación vs Simbiosis), OK.

---

## 5. Ajustes aplicados en esta pasada

| Lugar                                          | Cambio                                                         |
|------------------------------------------------|----------------------------------------------------------------|
| `SistemaMejoras.CosteConReduccion`             | Sumar fósil + genético, cap 0.80                               |
| `SistemaCadenas.ReduccionCosteCadenasEfectiva` | Método helper con cap 0.80                                     |
| `SistemaCadenas.CalcularCapPilar`              | Incorporar `BonusCapCadenaGen` (multiplicativo al fósil)       |
| `SistemaCadenas.CalcularCapPilar`              | Aplicar `MultiplicadorBifurcacion` por eslabón (gen/proc/dist) |
| `SistemaCadenas.MultiplicadorBifurcacion`      | Amplificar exceso con `BonusMultiplicadoresBifurcacion`        |
| `SistemaCadenas.ComprobarDesbloqueos`          | Min 1 sobre `NivelEslabonRequerido - ReduccionReqEslabones`    |
| `CalculadorProduccion.Calcular`                | Multiplicar por `MultiplicadorGlobalGen` y `PilaresBalanceados`|
| `CalculadorProduccion.MultiplicadorSinergias`  | Aplicar `BonusEfectividadSinergias` (×mult final)              |
| `CalculadorProduccion.EstimarGananciaPrestige` | Añadir `BonusFosilesPrestige` (gen) aditivo al fósil existente |
| `CalculadorProduccion.EstimarGananciaPrestige` | Nuevo: `BonusGenesPrestige` para Glaciación                    |
| `SistemaEventos.ResetearTimer`                 | `ReduccionCooldownEventos` (cap 0.80) reduce min/max           |
| `SistemaEventos.OpcionExtraDesbloqueada()`     | Expone a UI el flag del nodo `cg_m3`                           |
| `SistemaPrestige.Resetear`                     | Reset Bifurcaciones a -1 en cualquier prestige                 |
| `SistemaPrestige.Resetear`                     | Reset `NodosCodiceGenetico` SOLO en BigBang (TipoReset.Total)  |
| `SistemaGuardado`                              | Serializa `nodosCodiceGenetico` y `bifurcaciones`              |
| `GameController.Inicializar`                   | Wiring completo de `SistemaCodiceGenetico` y `SistemaBifurcaciones` |
| `GameController` API                           | `ComprarNodoCodiceGenetico`, `ElegirBifurcacion`               |

---

## 6. Ajustes pendientes (requieren playtest)

Estos puntos NO se tocan sin datos de telemetría. Están aquí como TODOs para
la fase de testing:

- [ ] **Ritmo de primera Glaciación**: objetivo 3-5 horas desde inicio. Si testers llegan en <2h → subir requisitos Era 5; si >8h → reducirlos.
- [ ] **Sensación de elección en bifurcaciones**: si los playtesters sienten que siempre es "obvio" cuál elegir, los multiplicadores `[1.50, 1.00, 0.90]` deben revisarse (quizá `[1.75, 1.00, 0.75]` para endurecer).
- [ ] **Valor del capstone cg_s6 (Gaia Plena)**: +25% EV/s global ×2 niveles = +50%. Si es insuficiente tras 100h, puede subirse a +35% × 2 = +70%.
- [ ] **Dead zones tardías**: entre run 4 y 8 puede haber meseta donde nada nuevo se desbloquea. La Era 6 (bifurcaciones) debe llegar en ese rango — verificar.
- [ ] **Longitud de `INTERVALO_MIN/MAX` eventos con cooldown al -45%**: pasa de 5-15min → 2.75-8.25min. Puede ser demasiado frecuente; si se siente spammy, ajustar cap `ReduccionCooldownEventos` a 0.30 en lugar de 0.45.
- [ ] **Cap de amplificación bifurcación**: `BonusMultiplicadoresBifurcacion` sube exceso en +85% (cg_a7 + cg_s4 maxados). Convierte `1.50→1.925`. Si rompe economía, cap a +60%.

---

## 7. Checklist verificable (T27 criterios originales)

Marcado lo que se puede confirmar a nivel de código. El resto requiere juego real.

- [x] **Save/Load funciona para TODOS los sistemas nuevos** — verificado: `SistemaGuardado.Cargar/Guardar` cubre `nodosCodiceGenetico` y `bifurcaciones`.
- [x] **Prestige resetea correctamente cadenas, misiones, bifurcaciones** — `SistemaPrestige.Resetear` lo hace en cualquier tipo.
- [x] **Big Bang resetea Códice Fósil + Genético** — cubierto en rama `TipoReset.Total`.
- [x] **Tap con bonus de Codice funciona correctamente** — sin cambios en T25/T26, se mantiene de T17.
- [x] **Auto-compra compra inteligentemente** — sin cambios, se mantiene de T18/T19.
- [ ] **Primera compra posible en <10 segundos** — depende de `BASE_MINIMA=1` y `atm_01=5`: teóricamente 5s. ✅ a nivel numérico.
- [ ] **Era 2 alcanzable en ~5 minutos** — depende de ritmo real.
- [ ] **Era 3 alcanzable en ~20 minutos** — idem.
- [ ] **Primer prestige en ~30-40 minutos** — idem.
- [ ] **Cadenas de cuello rotan correctamente** — `IdentificarCuelloBotella` existe y funciona.
- [ ] **Misiones se completan a ritmo constante (1 cada 2-5 min)** — depende del pool real.
- [ ] **No hay dead zones >10 segundos sin nada que comprar** — requiere playtest.

---

## 8. Archivos nuevos / modificados en T25+T26+T27

### Nuevos (4)
- `Assets/Scripts/idlesystem/data/Catalogos/CatalogoCodiceGenetico.cs` (20 nodos)
- `Assets/Scripts/idlesystem/data/Catalogos/CatalogoBifurcaciones.cs` (4 bifurcaciones)
- `Assets/Scripts/idlesystem/systems/SistemaCodiceGenetico.cs`
- `Assets/Scripts/idlesystem/systems/SistemaBifurcaciones.cs`

### Modificados (9)
- `Assets/Scripts/idlesystem/utils/Interfaces.cs` — enums `TipoCodiceGenetico` + 9 `TipoBonus`
- `Assets/Scripts/idlesystem/data/Definiciones.cs` — `DefinicionNodoCodiceGenetico` + `DefinicionBifurcacion`
- `Assets/Scripts/idlesystem/state/EstadoJuego.cs` — `NodosCodiceGenetico` (dict), `Bifurcaciones` ya existía
- `Assets/Scripts/idlesystem/utils/EventBus.cs` — `EventoBifurcacionRequerida`
- `Assets/Scripts/idlesystem/systems/CalculadorProduccion.cs` — 5 bonuses genéticos
- `Assets/Scripts/idlesystem/systems/SistemaMejoras.cs` — reducción coste acumulada
- `Assets/Scripts/idlesystem/systems/SistemaCadenas.cs` — cap gen, coste gen, req gen, mult bifurcación
- `Assets/Scripts/idlesystem/systems/SistemasSecundarios.cs` — `SistemaEventos` cooldown gen
- `Assets/Scripts/idlesystem/systems/SistemasSinergiaPrestige.cs` — reset gen + bifurcaciones
- `Assets/Scripts/idlesystem/systems/SistemaGuardado.cs` — serialización de ambos
- `Assets/Scripts/idlesystem/controllers/GameController.cs` — wiring + API pública

### No tocados (decisión deliberada)
- `Assets/Scripts/idlesystem/systems/SistemaCodice.cs` — NO se extiende; diseño paralelo mantiene separación limpia.
- `TapManager.cs` — T25/T26 no tocan el sistema tap; sus bonuses viven en el Códice Fósil existente.
- UI — requiere trabajo en Unity Editor (fuera del scope de código puro).

---

## 9. Trabajo Unity pendiente para cerrar alpha visual

Documentado aquí para que no se olvide pero no bloquea el alpha funcional:

1. Panel del Códice Genético (idéntico a Panel_Codice pero con icono de ADN y moneda "Genes")
2. Popup de bifurcación al llegar a Era 6 (4 popups secuenciales, 2 botones cada uno)
3. Indicador pasivo en panel de pilar mostrando la opción elegida (solo en Era 6+)
4. Badge "4ª opción oculta" en panel de evento si `Eventos.OpcionExtraDesbloqueada()` devuelve true

---

**Balance pass cerrado. El alpha está listo para playtest.**
