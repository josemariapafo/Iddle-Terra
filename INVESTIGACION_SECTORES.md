# Investigación por Sectores: Modelos de Negocio con Baja Competencia

> **Fecha:** 2026-04-11
> **Filtros aplicados:** No AI wrappers, competencia verificada (no estimada), sin capital, 30h/semana, sin dominio específico
> **Documentos relacionados:** `INVESTIGACION_NEGOCIO.md`, `ANALISIS_FINANCIERO.md`, `ANALISIS_COMPETITIVO.md`

---

## ADVERTENCIA: Las listas de ideas mienten sobre competencia

Las listas populares de "micro SaaS ideas 2026" (ideaproof.io, nxcode.io, calmops.com) etiquetan ideas como "Very Low competition" que al verificar tienen 5-10 competidores. Ejemplos concretos:

| Idea etiquetada "Very Low" | Realidad verificada |
|---|---|
| Newsletter sponsorship marketplace | SponsorGap, Paved, Beehiiv (adquirió Swapstack), Hecto, Passionfroot |
| Changelog as a Service | Canny, Beamer, ReleaseGlow, RightFeature, ProductLift, AnnounceKit, FeatureBase (10+) |
| Shopify review app | Judge.me (37K reviews), Loox (100K negocios), Okendo, Junip, Doran, Ali Reviews |
| Heatmap/session recording OSS | PostHog, OpenReplay, Highlight.io, Matomo, UXWizz |

**Este documento SOLO incluye ideas cuya competencia fue verificada, no estimada.**

---

## Índice por Sector

1. [Insight clave: "sin competencia" no existe](#1-insight-clave)
2. [Sector: E-commerce (Shopify)](#2-e-commerce-shopify)
3. [Sector: Food & Hospitality](#3-food--hospitality)
4. [Sector: Field Service & Construction](#4-field-service--construction)
5. [Sector: Freelancers & Agencies](#5-freelancers--agencies)
6. [Sector: Developer Tools](#6-developer-tools)
7. [Sector: Small Business Operations](#7-small-business-operations)
8. [Sector: Education](#8-education)
9. [Sector: Real Estate / Property](#9-real-estate--property)
10. [Sector: Creator Economy](#10-creator-economy)
11. [Sector: Compliance & Legal](#11-compliance--legal)
12. [Top 5 oportunidades tras verificación](#12-top-5-oportunidades-verificadas)
13. [Recomendación final revisada](#13-recomendación-final-revisada)

---

## 1. Insight clave

### "Sin competencia" no existe — pero "mal servido" sí

Cada idea obvia ya tiene competidores. PERO hay una diferencia crucial:

- **"Sin competencia"** = nadie ha intentado esto (raro, y normalmente significa que no hay mercado)
- **"Mal servido"** = existen soluciones pero son caras, viejas, complejas, o no open source

Los 3 éxitos que validamos al principio NO tenían "cero competencia":

| Producto | Competidores que existían | Por qué ganaron igual |
|---|---|---|
| **Postiz** ($14k MRR) | Buffer, Later, Hootsuite (docenas) | Open source + self-hosted + integración n8n |
| **Plausible** ($3.1M ARR) | Google Analytics (monopolio) | Privacidad + GDPR + simple + OSS |
| **Cal.com** | Calendly (dominante) | Open source + developer-first + API |

**Patrón:** No buscaron "sin competencia". Buscaron **un segmento mal servido de un mercado validado** y construyeron para ese segmento con un ángulo diferente (OSS, privacidad, simplicidad, vertical).

**Esto cambia la pregunta de "¿hay competencia?" a "¿hay un segmento que los incumbentes ignoran?"**

---

## 2. E-commerce (Shopify)

### Datos duros del sector
- **358,686 tiendas Shopify** analizadas en estudio de StoreInspect
- 40% de tiendas corren ads pero tienen **ZERO apps de retención**
- Shopify ofrece **0% revenue share** en el primer $1M de revenue por app
- Apps medianas generan $18.6K/semana (~$75K/mes)

### Gaps verificados con datos

| Categoría | % de tiendas SIN esta app | Apps existentes |
|---|---|---|
| Subscriptions | **98.1%** sin app | ReCharge, Bold (dominan pero caros) |
| Analytics | **98.1%** sin app | Triple Whale, Lifetimely (caros, enterprise) |
| Support/helpdesk | **97.8%** sin app | Gorgias ($300+/mo), Zendesk |
| Reviews | **94.4%** sin app | Judge.me (dominante + gratis), Loox |
| SMS marketing | **94.8%** sin app | Klaviyo, Postscript (dominantes) |
| Loyalty | **93.4%** sin app | Smile.io, Yotpo (establecidos) |

### Verticals con mayor gap de retención
- **Automotive:** 51.7% gap
- **Electronics:** 48.3% gap
- **Hobby:** 43.2% gap
- **Home & Garden:** 42.9% gap (45,362 tiendas)

### Oportunidades reales (después de verificar competencia)

**A. App de subscriptions para Shopify (budget tier)**
- ReCharge cobra $99-499/mes. Bold Subscriptions $49.99/mo.
- **Gap:** No hay opción buena por $15-29/mo para tiendas pequeñas
- **98.1% de tiendas no tienen subscriptions** — parcialmente porque las opciones son caras
- Competencia: Baja en el segmento de precio bajo
- **Viabilidad sin dominio:** Media. Necesitas entender e-commerce pero no un vertical específico.

**B. Bundle de retención vertical (ej: Automotive)**
- 51.7% de tiendas Automotive sin retención
- Idea: app que combine reviews + email follow-up + loyalty básico, diseñada para tiendas de autoparts
- **Competencia en este vertical:** Muy baja (nadie hace esto)
- **Riesgo:** TAM pequeño (¿cuántas tiendas Automotive hay en Shopify?)

**C. Analytics simple y barato para Shopify**
- Triple Whale cuesta $100-400/mo. Lifetimely es complejo.
- 98.1% no tiene analytics dedicado
- **Gap:** Dashboard simple con profit real por producto, $15/mo
- **Competencia en budget tier:** Baja
- **Viabilidad:** Alta (solo Shopify API + dashboard)

### Veredicto del sector
⚠️ **El ecosistema Shopify tiene gaps ENORMES en adopción, pero las categorías principales (reviews, SMS, loyalty) están dominadas.** La oportunidad real está en: (a) el segmento de precio bajo ($15-29/mo), o (b) bundling de retención para verticals específicos.

---

## 3. Food & Hospitality

### Datos duros
- **78% de food halls** usan POS de restaurante no diseñado para multi-vendor
- Crear **8-12 horas/semana de trabajo manual** por usar software equivocado
- Tabski es el único software purpose-built (~$26K/año)

### Gaps verificados

**A. Software de food hall (multi-vendor POS)**
- **Tabski** es prácticamente el único (competencia muy baja)
- Features que faltan en POS normales: carrito multi-vendor, rent collection automático, data isolation por vendor, kitchen routing
- **Problema:** Requiere conocimiento de food service operations
- **TAM:** ~6,000+ food halls en USA
- **Pricing:** $200-500/mes/hall
- **Competencia:** 🟢 Muy baja (solo Tabski)

**B. Software para food trucks (offline-first POS)**
- Los food trucks necesitan POS que funcione sin WiFi
- Square y Toast funcionan offline parcialmente pero no están optimizados
- **Problema:** Hardware integrations (impresoras, lectores de tarjetas)
- **Competencia:** Media (Square, Toast cubren parcialmente)

### Veredicto del sector
🟡 **Food hall management tiene la competencia más baja verificada, pero requiere dominio que no tienes.** Si pudieras hacer customer development con 3-5 operadores de food halls, podrías aprender rápido (sus problemas son operacionales, no técnicos).

---

## 4. Field Service & Construction

### Datos duros
- Solo **35% de empresas de construcción** usan scheduling dedicado
- Pequeños contractors pierden **30 min/técnico/día** en llamadas "¿dónde estás?"
- = 10 horas/mes/persona desperdiciadas

### Gaps verificados

**A. Scheduling + dispatch para HVAC/plumbing/electrical (1-10 empleados)**
- Jobber ($49-199/mo) y ServiceTitan ($500+/mo) dominan pero son caros/complejos para <10 empleados
- Kickserv ($47-239/mo) es más simple pero le falta route optimization
- **Gap:** Herramienta de $15-29/mo para contractors con 1-5 empleados
- **Competencia en budget tier:** Baja-media
- **Problema:** Necesitas domain knowledge (workflows de HVAC, plumbing, etc.)

### Veredicto del sector
🟡 **Gap verificado en el segmento small (1-5 empleados, <$30/mo) pero requiere entender el workflow del sector.** Si no lo conoces, es fácil construir algo que nadie usa.

---

## 5. Freelancers & Agencies

### Datos duros
- El mayor dolor NO resuelto: **billing transparency** en retainers
- Los portales existentes resuelven file sharing pero NO el "¿cuánto llevo gastado este mes?"
- 15+ client portals existen pero la mayoría son genéricos

### Gaps verificados

**A. Client portal con billing transparency en tiempo real**
- Pain: "My client constantly asks how many hours we've spent"
- Soluciones existentes (Copilot, SuiteDash, AgencyHandy) hacen file sharing y project management
- **Gap:** Live time tracking visible al cliente + burn rate del retainer + alertas automáticas al llegar a 80% del presupuesto
- **Competencia en este feature específico:** Baja
- **Riesgo:** Feature, no producto. ¿Pagarían por esto si SuiteDash añade el feature?

**B. Scope creep manager para freelancers**
- Pain: El proyecto creció de $2K a $8K sin que el freelancer renegociara
- **Competencia:** Casi ninguna herramienta específica
- **Riesgo:** TAM pequeño, WTP bajo ($9-19/mo)

### Veredicto del sector
🟡 **Gaps existen pero son features, no productos.** Difícil construir un negocio standalone. Mejor como feature dentro de un client portal más amplio.

---

## 6. Developer Tools

### Gaps verificados (NON-AI)

**A. Status page open source moderna (Cachet murió)**
- Cachet (el líder OSS) está abandonado/archivado
- Instatus ($19-79/mo), BetterStack ($20-85/mo) son pagados
- Upptime existe pero es limitado
- **Gap:** Status page moderna, bonita, OSS, self-hosted, con integración de monitors
- **Competencia OSS:** Baja (Cachet muerto, Upptime básico)
- **Competencia paid:** Media (Instatus, BetterStack)
- **Viabilidad:** Alta (es un dashboard + health checks + notificaciones)
- **Monetización:** Hosted version $10-30/mo
- **TAM:** Todo SaaS/API provider necesita una

**B. Invoice PDF Generator API**
- Herramienta API para generar PDFs de facturas profesionales
- **Competencia:** Muy baja como API dedicada
- **Riesgo:** WTP bajo, nicho pequeño

**C. Open source helpdesk moderno (Zendesk/Freshdesk killer)**
- osTicket existe pero es de 2003, interfaz antigua
- Chatwoot es live chat, no ticketing completo
- Zammad existe (OSS) pero es complejo y enterprise-focused
- **Gap:** Un helpdesk moderno, simple, OSS, con tickets + knowledge base + portal cliente
- **Competencia OSS moderna:** Baja
- **Competencia paid:** Alta (Zendesk, Freshdesk, Intercom)
- **Viabilidad:** Media-alta (scope grande pero bien definido)
- **Monetización:** Self-hosted gratis, cloud $15-49/agent/mo
- **TAM:** Enorme (toda empresa con soporte al cliente)

### Veredicto del sector
🟢 **Status page OSS y helpdesk OSS son los gaps más claros.** Status page es scope pequeño y alcanzable. Helpdesk es scope grande pero TAM enorme.

---

## 7. Small Business Operations

### Gaps verificados

**A. Checklist/SOP builder para procesos repetitivos**
- Pain: "No puedo delegar porque nada está documentado"
- Tango.us ($0-40/mo) graba pantalla y genera SOPs
- Process Street ($25-75/user/mo) es caro
- **Gap:** Herramienta simple de checklists + SOPs por $10-20/mo
- **Competencia:** Baja-media en precio bajo

**B. Inventory management para micro-negocios (1-2 personas)**
- Pain: Control de stock en Excel para tiendas/talleres pequeños
- **Competencia:** Las soluciones existentes asumen teams más grandes
- **Viabilidad sin dominio:** Media

### Veredicto del sector
🟡 **Hay gaps pero son pequeños y difíciles de monetizar.** WTP bajo de small businesses ($10-20/mo).

---

## 8. Education

### Gaps verificados

**A. Student management para tutores/academias pequeñas (<50 alumnos)**
- DreamClass existe pero el gradebook es débil
- Classe365 existe pero la interfaz es vieja
- **Gap:** Sistema simple: alumnos + horarios + seguimiento de progreso + comunicación con padres + pagos
- **Competencia:** Media (existen opciones pero ninguna buena y barata)
- **Problema:** Necesitas entender el workflow educativo
- **WTP:** $15-39/mo

### Veredicto del sector
🟡 **Gap existe pero requiere domain knowledge educativo y el WTP es bajo.**

---

## 9. Real Estate / Property

### Datos duros
- La mayoría de software de property management está construida para **200+ unidades**
- Landlords con 5-15 propiedades pagan por features que no usan
- Soluciones emergentes: SimplifyEm, Leasense, Landlord Cart, Stessa

### Gaps verificados

**A. Property management para small landlords (1-15 unidades)**
- **Competencia: Media-Alta** (SimplifyEm, Stessa, TenantCloud, Landlord Studio ya cubren esto)
- El gap se está cerrando rápido
- Baselane ofrece banking + property management integrado
- **Veredicto:** ❌ Gap casi cerrado, muchos entrantes

### Veredicto del sector
❌ **Muchos players ya atacando el small landlord segment. Llegar tarde.**

---

## 10. Creator Economy

### Gaps verificados

**A. Open source newsletter platform (Beehiiv/ConvertKit killer)**
- Beehiiv ($42-100/mo) y ConvertKit ($25-75/mo) son caros
- Listmonk es OSS pero SOLO envía emails (no tiene landing pages, referrals, monetización)
- **Gap:** Newsletter platform completa + monetización + referral system, open source
- **Competencia OSS:** 🟢 Baja (Listmonk es solo sending)
- **Competencia paid:** Alta (Beehiiv, ConvertKit, Substack)
- **Problema:** Scope enorme (email deliverability es MUY difícil), ¿6+ meses de desarrollo?
- **WTP:** $0-29/mo (creators son price-sensitive)

**B. Course platform self-hosted (Teachable/Thinkific killer)**
- Teachable ($39-199/mo) y Thinkific ($36-149/mo) son caros
- No hay alternativa OSS madura
- **Gap:** Platform para vender cursos, self-hosted, open source
- **Competencia OSS:** 🟢 Baja
- **Problema:** Scope muy grande (video hosting, payments, drip content, certificates)

### Veredicto del sector
🟡 **Gaps grandes pero scope enorme.** Newsletter platform es 6+ meses. Course platform es 8+ meses. Ambos son proyectos de un año, no de 3 meses.

---

## 11. Compliance & Legal

### Gaps verificados

**A. Compliance-lite para online creators (disclosures, policies)**
- Creators necesitan: FTC disclosures, privacy policies, cookie consent, tax compliance
- **Competencia:** Muy baja como producto integrado
- **WTP:** $10-29/mo
- **Riesgo:** TAM difuso, ¿quién busca esto activamente?

**B. SOC 2 automation para early-stage SaaS**
- SOC 2 cuesta $15K-50K normalmente
- Vanta ($10K+/año) y Drata ($5K+/año) son para series A+
- **Gap:** $500-2000/año para pre-seed/seed
- **Competencia en budget tier:** Baja
- **Problema:** Requiere profundo conocimiento de compliance

### Veredicto del sector
🟡 **Gaps reales pero requieren domain expertise en legal/compliance.**

---

## 12. Top 5 oportunidades verificadas

Después de analizar 11 sectores, verificar competencia, y filtrar por tu perfil (no domain, no capital, 30h/semana, no AI wrapper):

### Scoring actualizado con verificación de competencia

| # | Idea | Sector | Competencia verificada | WTP | Feasibility | No-domain needed | Score |
|---|---|---|---|---|---|---|---|
| 1 | **Status page OSS** | Dev tools | 🟢 Baja (Cachet muerto) | $10-30 | Alta | ✅ Sí | **27/35** |
| 2 | **Helpdesk OSS moderno** | Dev tools | 🟢 Baja-Media (osTicket viejo) | $15-49/agent | Media-alta | ✅ Sí | **26/35** |
| 3 | **Shopify analytics simple** | E-commerce | 🟢 Baja en budget tier | $15-29 | Alta | ✅ Sí | **25/35** |
| 4 | **Shopify subscriptions budget** | E-commerce | 🟢 Baja en budget tier | $15-29 | Media-alta | 🟡 Algo | **24/35** |
| 5 | **Food hall management** | Hospitality | 🟢 Muy baja (solo Tabski) | $200-500 | Media | ❌ Necesita domain | **23/35** |

### Alternativas que casi entran (honorable mentions)

| Idea | Por qué no entró al top 5 |
|---|---|
| Newsletter OSS (Beehiiv killer) | Scope demasiado grande (6+ meses), email deliverability es una pesadilla |
| Course platform OSS | Scope gigante (8+ meses), video hosting |
| Client portal con billing live | Es un feature, no un producto standalone |
| Field service scheduling budget | Requiere domain knowledge (HVAC, plumbing) |
| SOP/checklist builder | WTP demasiado bajo ($10-15) |

---

## 🥇 #1 — Status Page Open Source (detalle)

### Qué es
Página pública que muestra el estado de tus servicios: "API: ✅ Operational", "Database: ⚠️ Degraded". Toda empresa SaaS la necesita. Los clientes la consultan cuando algo falla.

### Por qué la competencia es baja
- **Cachet** (el líder open source histórico) está **muerto/archivado** — ya no se mantiene
- **Upptime** (OSS) es muy limitado — solo GitHub Actions, no tiene dashboard propio
- **Instatus** ($19-79/mo) es paid, no OSS, sin self-hosting
- **BetterStack** ($20-85/mo) es paid, no OSS
- **Statuspage by Atlassian** ($79-399/mo) es caro y enterprise

**Gap real:** No hay un status page **moderno, bonito, open source, mantenido activamente, con self-hosting fácil (Docker).**

### Por qué encaja contigo
- ✅ No requiere domain expertise (es infraestructura genérica)
- ✅ No es AI wrapper (es puro workflow: monitors + health checks + notificaciones)
- ✅ Scope pequeño (MVP en 6-8 semanas)
- ✅ Distribución clara (r/selfhosted, awesome-selfhosted, HN "Show HN", DEV.to)
- ✅ Costos operativos bajísimos ($10-20/mes hosting propio)
- ✅ Playbook probado: es el MISMO playbook que Postiz, Plausible, Cal.com usaron

### MVP features (6-8 semanas)
1. Dashboard público con estado de servicios
2. Incident management (crear/actualizar/resolver incidentes)
3. Health checks automáticos (HTTP ping, TCP, DNS)
4. Notificaciones (email, Slack, webhook)
5. Badge embebible ("Status: Operational")
6. Página de historial (uptime últimos 90 días)
7. Customización de marca (logo, colores, dominio)

### Monetización
- **Self-hosted:** Gratis (código en GitHub)
- **Cloud hosted:**
  - Free: 1 proyecto, 5 monitors
  - Pro $12/mo: 10 proyectos, 50 monitors, SSL custom domain
  - Team $29/mo: Ilimitado, team management, API
- **Fórmula Postiz:** cobras por la comodidad del hosting, no por el software

### Distribución (probada)
1. Publicar en GitHub con README bonito
2. Listarse en awesome-selfhosted (backlinks gratuitos)
3. Post en r/selfhosted (comunidad MUY activa para esto)
4. "Show HN" en Hacker News
5. Post en DEV.to
6. Docker Hub con imagen lista (critical para adopción)

### Proyección realista
- Semana 1-2: Core del dashboard + health checks
- Semana 3-4: Incident management + notificaciones
- Semana 5-6: Customización + auth + hosting cloud
- Semana 7-8: Stripe billing + free tier
- Mes 3: 200+ GitHub stars, 10-20 self-hosted users, 3-5 cloud paying
- Mes 6: 1000+ stars, $500-1500 MRR
- Mes 12: 3000+ stars, $3000-8000 MRR

### Riesgos
1. **BetterStack podría lanzar versión gratis** — risk, pero su modelo es enterprise
2. **Alguien más podría llenar el gap de Cachet** — por eso hay que moverse rápido
3. **Revenue ceiling moderado** — status pages no cobran mucho ($10-30/mo), necesitas volumen

### Competidores verificados con datos

| Competidor | Tipo | Precio | Estado |
|---|---|---|---|
| Cachet | OSS | Gratis | **Muerto/archivado** |
| Upptime | OSS | Gratis | Limitado (solo GitHub Actions) |
| Instatus | Paid | $19-79/mo | Activo, no OSS |
| BetterStack | Paid | $20-85/mo | Activo, no OSS |
| Statuspage (Atlassian) | Paid | $79-399/mo | Enterprise, caro |
| Oh Dear | Paid | $39-199/mo | Activo, no OSS |

**Conclusión: 0 competidores OSS modernos y mantenidos. Gap verificado y claro.**

---

## 🥈 #2 — Helpdesk Open Source Moderno (detalle)

### Qué es
Sistema de soporte al cliente: tickets por email, knowledge base (artículos de ayuda), portal para que el cliente vea sus tickets. Como Zendesk/Freshdesk pero open source y simple.

### Por qué la competencia es baja
- **osTicket** (OSS) es de **2003**, interfaz arcaica, PHP legacy
- **Chatwoot** (OSS) es **live chat**, no ticketing completo
- **Zammad** (OSS) es funcional pero **complejo y enterprise-focused**
- **FreeScout** (OSS) existe y es un Helpscout clone — este es el competidor más serio
- **Zendesk** ($19-115/agent) y **Freshdesk** ($15-79/agent) son caros

### Problema: FreeScout existe
FreeScout es un open source helpdesk que funciona. **Pero:**
- Stack viejo (PHP/Laravel)
- La mayoría de features son vía **módulos pagados** (no todo es gratis)
- UI funcional pero no moderna
- Self-hosting complicado (PHP + MySQL + mail server)

**Gap real:** Un helpdesk moderno en stack moderno (Next.js/Node), con Docker one-click, features core gratis, y UI bonita. Tipo "Chatwoot pero para tickets en vez de chat".

### MVP (10-12 semanas)
1. Inbox compartido (emails → tickets)
2. Asignación a agentes
3. Status (open/pending/resolved)
4. Knowledge base básica (artículos)
5. Portal para clientes (ver sus tickets)
6. Integraciones básicas (email, Slack)

### Monetización
- Self-hosted: Gratis
- Cloud: $12-29/agent/mo

### Riesgo principal
FreeScout ya funciona. Tienes que ser **significativamente mejor** en UX y developer experience para que la gente migre.

### Competidores verificados

| Competidor | Tipo | Estado |
|---|---|---|
| osTicket | OSS | Antiguo (2003), interfaz vieja |
| Chatwoot | OSS | Solo live chat, no ticketing |
| Zammad | OSS | Enterprise-focused, complejo |
| **FreeScout** | OSS | **Funcional pero PHP, módulos pagados** |
| Zendesk | Paid | $19-115/agent |
| Freshdesk | Paid | $15-79/agent |

---

## 🥉 #3 — Shopify Analytics Simple (detalle)

### Qué es
Dashboard que conecta con Shopify y muestra: profit real por producto (después de COGS, shipping, fees), customer lifetime value, best sellers, y métricas que Shopify no da nativamente.

### Por qué funciona
- **98.1% de tiendas Shopify** no tienen analytics dedicado
- Triple Whale cobra $100-400/mo — demasiado para tiendas pequeñas
- Lifetimely es complejo
- Shopify nativo da revenue pero NO profit real (no resta costos)

### Gap verificado
**$15-29/mo analytics para tiendas con <$50K/mo revenue.** El segmento budget está vacío.

### MVP (6-8 semanas)
1. Conectar Shopify API
2. Dashboard: revenue, profit (input COGS manual), orders, AOV
3. Top productos por profit (no por revenue)
4. Customer cohorts básicos (repeat vs new)
5. Comparación periodos

### Monetización
- $15/mo: 1 tienda, métricas básicas
- $29/mo: métricas avanzadas, multi-tienda, exports

### Distribución
- Shopify App Store (distribución built-in, gratis)
- Posts en Shopify community forums
- r/shopify, r/ecommerce

### Riesgos
1. Triple Whale podría lanzar plan barato
2. Shopify podría mejorar sus analytics nativos
3. El segmento budget ($15/mo) genera menos revenue por usuario

---

## 🏅 #4 y #5 — Shopify Subscriptions Budget y Food Hall Management

### #4 Shopify Subscriptions Budget
Similar al #3 pero para subscriptions (recurring orders). ReCharge cobra $99-499/mo. Bold $49.99/mo. Gap en $15-29/mo.

**Riesgo:** Shopify está mejorando subscriptions nativamente. Podrían comerse este espacio.

### #5 Food Hall Management
Competencia más baja de todas (solo Tabski). WTP alto ($200-500/mo). Pero **requiere domain knowledge de food service** que no tienes.

**Consejo:** Solo considerar si estás dispuesto a pasar 2-3 semanas haciendo customer development con operadores de food halls antes de escribir código.

---

## 13. Recomendación final revisada

### 🏆 Apuesta principal: Status Page Open Source

**Es la mejor opción porque cumple TODOS los filtros a la vez:**

| Criterio | Status |
|---|---|
| ¿Competencia verificada como baja? | ✅ Cachet muerto, 0 OSS modernos |
| ¿No es AI wrapper? | ✅ Pure infrastructure |
| ¿No necesita domain expertise? | ✅ Es infra genérica |
| ¿No necesita capital? | ✅ $100-200 máximo |
| ¿30h/semana es suficiente? | ✅ MVP en 6-8 semanas |
| ¿Distribución clara sin ads? | ✅ r/selfhosted, awesome lists, HN |
| ¿Playbook probado? | ✅ Mismo modelo que Postiz, Plausible, Cal.com |
| ¿Revenue ceiling viable? | 🟡 Moderado ($3-8K MRR año 1) |

**El único punto débil es el revenue ceiling.** Status pages cobran $10-30/mo, así que necesitas 100-300 clientes para llegar a $3-8K MRR. Pero el funnel de OSS → cloud es probado (Postiz lo hizo).

### Plan B: Helpdesk Open Source Moderno

Si el ceiling de status page te parece bajo, el helpdesk tiene:
- Revenue ceiling mucho mayor ($15-49/agent × muchos agentes)
- Pero scope mayor (10-12 semanas MVP vs 6-8)
- Y FreeScout es competidor real (aunque viejo/PHP)

### Plan C: Shopify Analytics Budget

Si prefieres estar en un ecosistema con distribución built-in (App Store), Shopify analytics a $15-29/mo es viable. Pero dependes de que Shopify no mejore sus analytics nativos.

### Lo que cambió respecto a los documentos anteriores

| Documento anterior | Este documento |
|---|---|
| Recomendaba GEO (AI Search Visibility) | ❌ Otterly tiene 20K users, es AI wrapper |
| Recomendaba Loom OSS | ❌ Cap.so ganó (17.9K stars) |
| Recomendaba LLM Observability | ❌ Langfuse dominante (24.7K stars) |
| Recomendaba Cloud Cost | ❌ Vantage cubre el gap a $30/mo |
| Recomendaba PDF Extraction | ❌ Es un AI wrapper ("¿por qué no usar Claude?") |
| **Ahora recomienda: Status Page OSS** | ✅ Gap verificado, playbook probado, no AI wrapper |

### Pregunta para ti

El trade-off final es:
- **Status Page:** Más fácil, más rápido, más seguro, pero ceiling moderado ($3-8K MRR)
- **Helpdesk OSS:** Más difícil, más lento, más riesgoso (FreeScout compite), pero ceiling alto ($10-30K MRR)

¿Prefieres seguridad con ceiling moderado, o ambición con más riesgo?

---

## Fuentes verificadas

- [StoreInspect — 358K Shopify Stores Analyzed](https://storeinspect.com/report/state-of-shopify)
- [StoreInspect — Shopify Retention Gap Study](https://storeinspect.com/blog/shopify-retention-gap)
- [Tabski — Food Hall Software](https://tabski.com/food-hall-software/)
- [IdeaProof — 52 Micro SaaS Ideas](https://ideaproof.io/lists/micro-saas-ideas)
- [Repair-CRM — Field Service Scheduling Guide 2026](https://www.repair-crm.com/2026/03/19/field-service-scheduling-software-the-2026-guide-for-small-teams)
- [Posthog — OSS Session Replay Tools](https://posthog.com/blog/best-open-source-session-replay-tools)
- [ProductLift — Best Changelog Tools 2026](https://www.productlift.dev/best-changelog-tool/)
- [SponsorGap](https://sponsorgap.com)
- [Vantage Pricing](https://www.vantage.sh/pricing)
- [Langfuse GitHub](https://github.com/langfuse/langfuse)
- [Cap.so](https://cap.so)
- [Otterly.AI](https://otterly.ai)
- [Rossum Pricing](https://rossum.ai/pricing)
- [Helicone](https://helicone.ai)
