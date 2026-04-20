# Investigación: Oportunidades de Negocio Open Source

> **Objetivo:** Proyecto paralelo a Iddle Terra para generar ingresos
> **Fecha del análisis:** 2026-04-09
> **Contexto del fundador:** Sin capital, 30h/semana disponibles, stack flexible, sin dominio específico

---

## Índice

1. [Contexto y restricciones](#1-contexto-y-restricciones)
2. [Los 3 candidatos iniciales](#2-los-3-candidatos-iniciales)
3. [Ganador de los 3](#3-ganador-de-los-3-loom-oss)
4. [Metodología del research](#4-metodología-del-research)
5. [Framework de scoring](#5-framework-de-scoring-7-criterios)
6. [Biblioteca de 60 ideas analizadas](#6-biblioteca-de-60-ideas-analizadas)
7. [TOP 5 con análisis profundo](#7-top-5-con-análisis-profundo)
8. [Recomendación final](#8-recomendación-final)
9. [Plan de acción (4 semanas)](#9-plan-de-acción-primeras-4-semanas)
10. [Fuentes consultadas](#10-fuentes-consultadas)

---

## 1. Contexto y restricciones

### Perfil del fundador
- **Capital:** Ninguno
- **Tiempo disponible:** 30 horas/semana
- **Stack técnico:** Flexible (domina todos los stacks)
- **Dominio específico:** Ninguno
- **Necesidad de ingresos:** Sí, relativamente pronto
- **Situación:** Este es un proyecto paralelo a Iddle Terra

### Camino elegido (previamente acordado)
**Camino A — Micro-SaaS con open source como distribución**

Se construye algo pequeño y específico que resuelve un dolor concreto a un nicho que ya paga. El código es abierto, pero se cobra por la versión hospedada. Objetivo: primeros $500-2000/mes en 3-6 meses.

### Filtros duros aplicados
- ❌ Ideas que requieren dominio específico
- ❌ Ideas con costos operativos altos desde día 1
- ❌ Ideas en mercados sobresaturados
- ✅ Ideas donde el stack técnico es ventaja, no requisito de dominio
- ✅ Ideas con distribución clara sin depender de anuncios pagados

### Casos reales que validan el camino

| Proyecto | Qué reemplaza | Timeline | Resultado |
|---|---|---|---|
| **Postiz** | Buffer / Later (social scheduling) | Lanzó Sept 2024, solo dev | **$14.2k MRR** en ~12 meses |
| **Papermark** | DocSend (share documents) | 2 devs, 11 productos fallidos antes | $1k → **$45k MRR** en un año, **$500k ARR** en 18 meses |
| **Plausible** | Google Analytics | Bootstrap puro | **$3.1M ARR** |

---

## 2. Los 3 candidatos iniciales

### Opción A — Loom Open Source Alternative
**Qué hace:** Herramienta para grabar y compartir videos asíncronos. Grabación de pantalla + cámara, upload automático, link compartible, comentarios con timestamp, analytics básicos.

**Casos de uso:** Devs reportando bugs, PMs explicando features, reviews asíncronos, demos personalizadas de sales, respuestas de soporte con video.

**Por qué hay gap:** Loom fue adquirido por Atlassian en 2023. Usuarios se quejan de cambios post-adquisición. No hay alternativa open source seria (Cap.so existe pero joven).

**Pricing de Loom:** $15-25/seat/mes.

### Opción B — LLM Observability / Prompt Management
**Qué hace:** "Datadog para apps con LLMs". Tracking de cada llamada al modelo, versionado de prompts, evaluaciones, debugging de producción, dashboards de costos.

**Casos de uso:** Debugging cuando un chatbot responde mal, iteración de prompts sin redeployar código, comparación A/B de versiones de prompts, monitoring de costos por feature.

**Por qué hay gap:** Langfuse lidera pero está construido para equipos grandes. Hay espacio para algo más opinionated (solo agentes, solo RAG, solo para teams <10 personas).

**Pricing de Langfuse:** $29-99/mes en cloud.

### Opción C — Gumroad Open Source Alternative
**Qué hace:** Plataforma para vender productos digitales (ebooks, cursos, templates, presets, software). Upload → página de checkout → Stripe → entrega automática.

**Casos de uso:** Creators vendiendo ebooks de $15, cursos de $99, templates de Notion, presets de Lightroom, plugins de código.

**Por qué hay gap:** En 2023-2024 Gumroad subió comisiones de 5% a 10%+. Creadores montaron escándalo público. No hay alternativa OSS madura.

**Pricing de Gumroad:** 10% comisión por venta.

---

## 3. Ganador de los 3: Loom OSS

### Por qué Loom gana entre los 3

De los 3, es el único con los 4 factores alineados a la vez:

1. **Catalizador externo claro** — Loom fue comprada por Atlassian en 2023, los usuarios están migrando y no hay alternativa OSS seria.
2. **Willingness to pay alto** — Loom cuesta $15-25/seat/mes, así que puedes cobrar $5-10/user/mes en hospedado y seguir siendo atractivo.
3. **Moat técnico** — El video (transcoding, streaming, storage) asusta a los copycats casuales. Esto protege tu inversión de 4 meses.
4. **Distribución clara** — r/selfhosted, Awesome Self-hosted, DEV.to, Hacker News. Canales probados.

### Por qué pierden los otros 2

**LLM observability:** Langfuse ejecuta rápido y está bien financiado. Si tardas 6 meses en construir algo, te come el mercado. Moat bajo.

**Gumroad OSS:** Los creators son muy price-sensitive, Stripe Connect + tax compliance internacional es una pesadilla operativa, y el ceiling de MRR es bajo (~$10k/mes). Mucha fricción, poco upside.

### Caveat importante sobre Loom OSS
El hosting de video es caro (bandwidth, transcoding). Hay que diseñar la versión hospedada con límites estrictos desde el día 1:
- Free tier = 25 videos de 5 min máx
- Retención 90 días
- Transcoding eficiente con colas async

Si no, los costos te matan antes de facturar.

### ⚠️ PERO...
Después de analizar 150+ ideas, **Loom OSS no está en el top 3 final**. Hay opciones mejores que se analizan más abajo.

---

## 4. Metodología del research

### Fuentes consultadas
- 15+ búsquedas web con queries específicas sobre micro-SaaS 2026
- 5 extracciones directas de listas curadas de ideas
- Análisis del índice `awesome-oss-alternatives` de Runa Capital
- Revisión del YC Request for Startups Spring 2026

### Fuentes más valiosas
- **nxcode.io** — 50 ideas con revenue data
- **saasniche.com** — 50 ideas validadas con quejas reales de Reddit (incluye subreddit de origen)
- **painonsocial.com** — 50+ ideas de vertical SaaS
- **calmops.com** — 28 ideas con contexto de mercado
- **runa capital awesome-oss-alternatives** — análisis de gaps en open source

### Proceso
1. Generación amplia: 150+ ideas raw de múltiples fuentes
2. Deduplicación y limpieza → ~80 ideas únicas
3. Aplicación de filtros duros (dominio, capital, saturación) → 60 ideas viables
4. Scoring contra 7 criterios → top 20 candidatos
5. Deep dive en top 5

---

## 5. Framework de scoring (7 criterios)

Cada criterio se puntúa del 1 al 5. Máximo total: **35 puntos**.

| Criterio | Qué mide | Peso mental |
|---|---|---|
| **Market demand** | ¿Cuánta gente tiene este dolor? | Alto |
| **Willingness to pay** | ¿Cuánto pagan por resolverlo? ($/mes) | Alto |
| **Competition** | ¿Qué tan vacío está el espacio? (inverso) | Medio |
| **Feasibility** | ¿Lo puedes construir solo en 2-4 meses? | Alto |
| **Distribution** | ¿Puedes llegar a usuarios sin anuncios? | Alto |
| **Moat** | ¿Qué tan difícil de copiar una vez hecho? | Medio |
| **Revenue ceiling** | MRR realista a 12 meses | Alto |

### Interpretación de scores
- **28-35:** Oportunidad excepcional, vale la pena apostar
- **23-27:** Oportunidad sólida, viable con buena ejecución
- **18-22:** Aceptable, muchas otras mejores
- **<18:** Descartar, hay opciones mucho mejores

---

## 6. Biblioteca de 60 ideas analizadas

Las ✨ son finalistas que puntuaron 23+.

### A. Developer Tools & Infrastructure

| # | Idea | Qué hace | Score |
|---|---|---|---|
| 1 | **Loom OSS** | Async video messaging self-hosted | 29 ✨ |
| 2 | **LLM observability para equipos pequeños** | Trace + eval + prompt versioning, opinionated para startups | 25 ✨ |
| 3 | **Cloud cost prediction** | Predice gasto AWS/GCP, alerta anomalías | 25 ✨ |
| 4 | Visual uptime monitoring | Screenshots periódicos + diff para detectar fallos visuales | 24 |
| 5 | SaaS metrics dashboard | MRR/churn/LTV desde Stripe, versión cheap | 22 |
| 6 | API documentation generator | De código a docs interactivas | 19 |
| 7 | Test data generator | Fake data matching schemas | 17 |
| 8 | Database migration tool | Conversión SQL/NoSQL zero-downtime | 16 |
| 9 | YAML autocomplete/validation | Smart editor para DevOps configs | 18 |
| 10 | OAuth proxy para ERP legacy | SSO para sistemas viejos | 19 |
| 11 | Secure token audit tool | Scanner para patterns inseguros | 18 |
| 12 | AWS account suspension prevention | Monitor + recovery | 17 |
| 13 | SOC 2 automation early-stage SaaS | Vanta cheap | 22 |

### B. AI/LLM-Native Products (categoría hot)

| # | Idea | Qué hace | Score |
|---|---|---|---|
| 14 | **AI search visibility tracker (GEO)** | Trackea presencia de marca en respuestas de ChatGPT/Claude/Perplexity | **30** ✨ |
| 15 | **AI-powered PDF extraction para finance** | Extrae tablas/datos de PDFs con LLMs, exporta a Excel/QB | 24 ✨ |
| 16 | AI meeting notes + action items | Otter específico para equipos técnicos | 22 |
| 17 | AI contract review | Red flags + cláusulas peligrosas | 22 |
| 18 | AI content repurposing (nicho) | Video→thread, especializado por plataforma | 21 |
| 19 | AI podcast show notes + clips | Para podcasters, incluye social clips | 21 |
| 20 | AI product description generator | Foto → copy para Shopify/Amazon | 20 |
| 21 | AI review response tool | Respuestas personalizadas Google/Yelp | 23 |
| 22 | AI customer support agent (no-code) | Agente entrenado con tus docs | 22 |
| 23 | AI content verification/provenance | Detecta contenido AI, marcas de agua | 22 |
| 24 | AI lead enrichment (Chrome ext) | Enriquece LinkedIn con firmografía | 21 |
| 25 | AI feature pricing simulator | Para SaaS añadiendo features AI | 23 |

### C. Marketing & SEO Tools

| # | Idea | Qué hace | Score |
|---|---|---|---|
| 26 | Click fraud detection Google Ads | Detecta bots comiendo budget | 23 |
| 27 | Backlink impact analyzer | Evalúa calidad real de backlinks | 22 |
| 28 | Google algorithm drop forensics | Diagnóstico estructurado de caídas | 21 |
| 29 | Search intent keyword clustering | Agrupa keywords por intención real | 20 |
| 30 | Email list cleaner | Valida, limpia bounces/spam traps | 21 |
| 31 | Influencer CRM | Gestión de relaciones con influencers | 20 |
| 32 | Cart abandonment recovery | Email + SMS + personalización | 20 |
| 33 | Automated client reporting | Reportes branded para agencias | 19 |
| 34 | Competitor price monitor | Alertas de cambios de precio | 20 |

### D. Small Business Operations

| # | Idea | Qué hace | Score |
|---|---|---|---|
| 35 | Sales tax automation SMB | TaxJar cheap para small stores | 23 |
| 36 | Business process documentation | SOPs auto-generadas al grabar pantalla | 23 |
| 37 | Customer revenue concentration monitor | Alerta si 1 cliente = >20% revenue | 22 |
| 38 | Multi-site employee hour tracking | GPS + hours para contractors multi-job | 22 |
| 39 | Invoice reminder automation | Cobra automáticamente a morosos | 19 |
| 40 | Freelancer income dashboard | Agrega ingresos, calcula tax estimate | 18 |
| 41 | Freelancer scope creep manager | Detecta cuando proyecto crece sin cobrar | 19 |

### E. E-commerce Tools

| # | Idea | Qué hace | Score |
|---|---|---|---|
| 42 | Shopify + Amazon sales tax calc | Cross-platform compliance | 24 |
| 43 | Multi-platform revenue attribution | Rentabilidad real por canal | 22 |
| 44 | Inventory alert system | Alertas antes de stockout | 20 |
| 45 | Customer photo review collector | Pide fotos con incentivos | 21 |
| 46 | Pre-order management | Para Kickstarter alumni + small brands | 19 |
| 47 | AI product photo generator | Foto básica → foto profesional | 21 |

### F. Creator Economy

| # | Idea | Qué hace | Score |
|---|---|---|---|
| 48 | Gumroad OSS alternative | Digital product sales self-hosted | 20 |
| 49 | Newsletter tool técnico | Code highlighting, gists, RSS smart | 20 |
| 50 | Waitlist builder con referrals | Lanzamientos con tracking | 18 |
| 51 | Niche link-in-bio | Músicos, podcasters, coaches | 15 |
| 52 | Creator sponsorship marketplace | Micro-influencers ↔ brands | 19 |

### G. Niche/Specialty

| # | Idea | Qué hace | Score |
|---|---|---|---|
| 53 | Wedding all-in-one planner | Vendor + budget + timeline en uno | 21 |
| 54 | Forum spam detection API | Moderación AI para comunidades | 22 |
| 55 | Focus app con bloqueos duros | Anti-circumvention | 19 |
| 56 | Sobriety app sin alcohol ads | Clean recovery tracker | 20 |
| 57 | Lightweight SIS small schools | Para escuelas privadas <100 alumnos | 18 |

### H. Vertical SaaS (descartadas por falta de dominio)

| # | Idea | Score (sin dominio) |
|---|---|---|
| 58 | CRM fitness, photo, tattoo, wedding planners | 18-21 |
| 59 | HVAC, plumbing, roofing, landscaping management | 19-22 |
| 60 | Mental health, dental, PT, veterinary | 18-20 |

---

## 7. TOP 5 con análisis profundo

### Ranking final

| Posición | Idea | Score |
|---|---|---|
| 🥇 #1 | AI Search Visibility Tracker (GEO) | 30/35 |
| 🥈 #2 | Loom Open Source Alternative | 29/35 |
| 🥉 #3 | LLM Observability para equipos pequeños | 25/35 |
| 🏅 #4 | Cloud Cost Prediction & Anomaly Detection | 25/35 |
| 🏅 #5 | AI-powered PDF Extraction para Finance | 24/35 |

---

### 🥇 #1 — AI Search Visibility Tracker / Generative Engine Optimization (GEO)

**Score: 30/35**

#### Qué hace exactamente
Cuando alguien pregunta a ChatGPT, Claude, Perplexity o Gemini *"¿cuáles son las mejores herramientas de email marketing?"*, la respuesta menciona 3-5 marcas. **Si tu marca no aparece, pierdes tráfico que antes ganabas por SEO tradicional.**

Este producto trackea:
- Qué marcas se mencionan para qué queries
- Con qué frecuencia
- Qué fuentes cita el AI al mencionarlas

Es **"Google Search Console pero para respuestas de LLM"**.

#### Caso de uso concreto
Eres marketing lead de una empresa SaaS. Configuras 50 queries relevantes (*"best CRM for small business"*, *"Salesforce alternatives cheap"*, etc.). El producto las corre cada día en GPT, Claude, Perplexity, Gemini. Ves un dashboard con:
- **Share of voice en AI:** % de queries que mencionan tu marca
- **Fuentes citadas:** qué sitios cita el AI (los nuevos backlinks que importan)
- **Competidores:** quién aparece más que tú
- **Tendencias temporales:** ¿estás mejorando?
- **Recomendaciones:** *"para aparecer en respuestas sobre X, necesitas contenido en estos topics"*

#### Scoring detallado

| Criterio | Score | Razón |
|---|---|---|
| Market demand | 5/5 | Categoría explotando 2025-2026. Cada CMO está preguntando "¿cómo medimos AI visibility?" |
| WTP | 5/5 | Marketers pagan $100-500/mes por tools SEO. GEO tendrá precios similares o más altos |
| Competition | 4/5 | Profound (YC, $3.5M raised), Otterly.AI, AthenaHQ, Peec AI — todos jóvenes, ninguno dominante |
| Feasibility | 4/5 | APIs de LLMs + scraping + dashboard. No requiere infra pesada. 2-3 meses MVP |
| Distribution | 4/5 | Marketing Twitter, SEO communities, HN. Es el topic del momento |
| Moat | 3/5 | Data quality y tracking histórico son moat parcial. Nadie puede recrear 6 meses de histórico |
| Ceiling | 5/5 | $30-50k MRR realista en 12 meses |

#### Por qué gana sobre Loom
1. **Time-to-revenue más rápido** — 2-3 meses al MVP vs 4 meses de Loom
2. **Costos operativos casi cero** — llamadas a APIs (que cobras al cliente) + DB simple
3. **Momento del mercado mejor** — categoría nueva, first-mover possible
4. **Ceiling más alto** — marketers pagan MÁS que devs

#### Cómo construir el MVP (3 meses)

**Mes 1 — Backend y data pipeline:**
- Worker que corre queries contra OpenAI, Anthropic, Perplexity, Google APIs
- DB Postgres para guardar respuestas históricas
- Parser que extrae menciones de marcas/productos de las respuestas
- Scheduler (cron) para correr queries diariamente

**Mes 2 — Dashboard y billing:**
- Frontend Next.js con:
  - Share of voice chart
  - Timeline histórico
  - Comparación con competidores
  - Lista de queries configurables
- Auth (Clerk/Supabase Auth)
- Billing con Stripe

**Mes 3 — Diferenciación:**
- Alertas cuando cambia la visibility de forma significativa
- Reportes exportables PDF/CSV
- Sugerencias de contenido (LLM analiza fuentes citadas y sugiere topics a cubrir)
- Integración con Slack para notificaciones

#### Modelo de pricing propuesto

| Plan | Precio | Qué incluye |
|---|---|---|
| **Starter** | $49/mes | 20 queries, 4 modelos, refresh diario |
| **Growth** | $149/mes | 100 queries, todos los modelos, reportes semanales |
| **Pro** | $399/mes | 500 queries, API access, alertas, white-label |

#### Playbook de distribución
- Publicación semanal en LinkedIn/Twitter sobre "AI search trends" usando tu propia data
- Guest posts en blogs SEO populares
- Product Hunt launch
- Tool lists en newsletters de marketing
- Free tier con 5 queries para captación

#### Riesgos reales (honestos)
1. **Profound.so ya está bien financiado** — ventaja de tiempo y capital
2. **APIs de LLMs pueden cambiar pricing** — cost structure sensible
3. **Respuestas de LLMs no son 100% estables** — mismo query puede dar respuestas distintas
4. **LLMs pueden empezar a dar APIs oficiales de esto** — riesgo de commoditización

#### Por qué ganas igual
Profound está yendo enterprise ($1000+/mes). Tú vas al segmento SMB/mid-market ($49-399/mes) que está desatendido. **Misma estrategia que Plausible usó contra Google Analytics.**

#### Proyección realista de ingresos

| Hito | MRR |
|---|---|
| Mes 3 | $0 (MVP listo, 10 usuarios beta gratuitos) |
| Mes 6 | $1,500 (30 usuarios pagados) |
| Mes 9 | $5,000 (90 usuarios) |
| Mes 12 | $12,000-15,000 (150-200 usuarios) |
| Mes 18 | $25-40k (si ejecución es buena) |

---

### 🥈 #2 — Loom Open Source Alternative

**Score: 29/35**

Ver análisis completo en secciones 2 y 3.

**Resumen:**
- **Fortalezas:** moat técnico alto, pain universal, catalizador (Atlassian acquisition)
- **Debilidad crítica:** costos de bandwidth/storage altos desde día 1, MVP toma 4 meses, Cap.so ya existe
- **Por qué no es #1:** time-to-revenue más lento + cost structure más pesado
- **Cuándo elegirlo:** Si tienes $500-1000 de runway para cubrir hosting los primeros meses

---

### 🥉 #3 — LLM Observability para equipos pequeños

**Score: 25/35**

#### Qué hace exactamente
"Langfuse opinionated para solo devs y equipos <10 personas":
- Setup en 5 minutos (vs 1 hora de Langfuse)
- UI simple
- Pricing plano ($29/mes ilimitado) vs usage-based
- **Opinionated para 1 caso específico** (ej: solo agentes, solo RAG, solo customer support chatbots)

#### Caso de uso (opinionated para agentes)
Eres un solo dev construyendo un agente que hace research web. El agente toma 20 pasos. A veces falla. Con tu tool:
- Ves el árbol de llamadas del agente visualmente
- Ves qué tool call falló, con qué input, qué respondió
- Comparas ejecuciones "buenas" vs "malas" del mismo prompt
- Alertas cuando el agente gasta >$5 en una sesión
- Eval suite: "corre mis 50 test cases contra esta nueva versión"

#### Por qué es #3
- Langfuse está bien financiado y ejecuta rápido (riesgo real)
- Low moat — cualquiera puede copiar una vez pruebes mercado
- Pero: el segmento "small AI team" está desatendido

#### Pricing propuesto
- Free: 10k traces/mes
- Solo Dev: $19/mes — 100k traces
- Team: $79/mes — 1M traces + colaboración
- Self-hosted: gratis

#### Distribución
- HN launch
- AI Twitter (la comunidad está en Twitter más que Reddit)
- Integración con frameworks populares (LangChain, Vercel AI SDK, Pydantic AI)
- Build in public: cada feature, post en Twitter

#### Proyección
- Mes 3: MVP
- Mes 12: $5-10k MRR (riesgo Langfuse alto)

---

### 🏅 #4 — Cloud Cost Prediction & Anomaly Detection

**Score: 25/35**

#### Qué hace exactamente
Conectas tu cuenta AWS/GCP/Azure. El producto:
1. **Predice tu factura** del próximo mes basado en tendencias
2. **Detecta anomalías** (ej: una lambda que de repente gasta 10x)
3. **Recomienda optimizaciones** (*"este EC2 lleva 30 días al 5% CPU, downsizealo → ahorra $200/mes"*)
4. **Dashboards por feature/equipo** (tag-based allocation)

#### Caso de uso
CTO de startup Serie A. Gasta $8k/mes en AWS. No entiende por qué sube. CFO pregunta. Con tu tool:
> *"AWS pasó de $7k a $8k porque RDS r5.2xlarge en staging subió 40% (abandonaron test suite), y S3 egress subió por bot scraping de tu API. Fix A ahorra $400/mes. Fix B ahorra $300/mes."*

#### Por qué es #4
- **Mercado grande, WTP alto** (CFOs + engineering managers)
- **Competidores existen** (Vantage, CloudZero, Harness) pero son caros ($500-5000/mes enterprise)
- **Gap real en SMB** — startups de <$500k MRR no pueden pagar Vantage
- **Riesgo:** requiere profundizar en AWS/GCP pricing APIs

#### Pricing
- Free: 1 account, $10k/mes de spend
- Pro: $49/mes — hasta $50k/mes de spend
- Scale: $199/mes — hasta $500k/mes
- Enterprise: custom

#### Distribución
- DevOps Twitter, r/devops, r/aws
- HN "Show HN: I analyzed my AWS bill and saved $X"
- Free tier aggressive (tus propios costs son bajos)
- Integraciones Slack (alertas)

#### Proyección
- Mes 3-4: MVP (más lento por complejidad APIs)
- Mes 12: $8-15k MRR

---

### 🏅 #5 — AI-powered PDF Extraction para Finance/Accounting

**Score: 24/35**

#### Qué hace exactamente
Los contadores pasan horas **metiendo datos de PDFs a Excel/QuickBooks**: facturas, bank statements, reportes, 1099s. Herramientas existentes (Docparser, Rossum) son caras y requieren "templates" por cada proveedor.

**Con GPT-4V/Claude Sonnet ya no necesitas templates:** el LLM extrae tablas y datos estructurados de cualquier PDF.

Tu producto:
- Upload PDF
- LLM extrae datos estructurados (JSON)
- Exporta a Excel, QuickBooks, Xero, CSV
- Aprende de correcciones manuales
- Batch processing (100 PDFs de una vez)

#### Caso de uso
Contadora freelance con 15 PYMES. Cada mes recibe ~200 PDFs de facturas. Antes: 2 días metiendo datos a QB. Con tu tool: sube los 200 PDFs, el sistema extrae, ella revisa diffs, aprueba, exporta a QB. **De 2 días a 2 horas.**

#### Por qué es #5
- Mercado enorme y doloroso
- WTP bueno ($30-150/mes, se paga en tiempo ahorrado)
- Competitors viejos son caros (Rossum enterprise, Docparser clunky)
- LLMs nuevos lo hacen FEASIBLE HOY
- **Riesgo:** low moat (fácil de copiar), muchos entrantes

#### Pricing
- Free: 20 PDFs/mes
- Starter: $29/mes — 200 PDFs
- Pro: $99/mes — 1000 PDFs + QB/Xero integration
- Firm: $299/mes — 5000 PDFs + multi-client + team

#### Distribución
- r/accounting (muy activo)
- Bookkeeping Facebook groups
- LinkedIn posts showing before/after
- QuickBooks App Store
- Integración con Zapier/Make

#### Proyección
- Mes 2: MVP (rápido, es LLM + frontend)
- Mes 12: $6-12k MRR

---

## 8. Recomendación final

### 🏆 Apuesta principal: **AI Search Visibility Tracker (GEO)**

Es la **mejor apuesta absoluta** porque:

1. **Mercado en el punto exacto de explosión** — 2026 es el año que "AI SEO" se convierte en categoría establecida. Los primeros en capturar mindshare ganan enormemente.

2. **Las restricciones del fundador lo favorecen:**
   - Cost estructure mínimo (no hosting de video, no compute pesado)
   - Time-to-MVP corto (3 meses)
   - Distribución clara (content marketing = tu propia data)

3. **Ceiling alto** — $30-50k MRR en 12-18 meses es realista con buena ejecución

4. **No requiere dominio específico** — stack técnico + pensamiento analítico suficiente

5. **El "enemigo" (Profound) está yendo enterprise** — deja el segmento SMB/mid-market libre. **Mismo playbook que Plausible usó contra Google Analytics.**

### Plan B: Cloud Cost Prediction

Si por alguna razón GEO no convence (te da miedo la competencia con Profound, o el tema no engancha), **Cloud cost prediction (#4)** es el mejor plan B. Menos "moda" pero más defensible, con un comprador claro (CTOs de startups con $10-100k/mes de AWS).

### Anti-recomendaciones (qué NO hacer)

- ❌ **No elijas una vertical sin dominio.** Todos los "CRM para X" scorean 20-24 porque requieren conocimiento del workflow que no tienes. Son trampas.
- ❌ **No elijas algo en categoría saturada.** Link-in-bio, testimonials (Senja ganó), AI content repurposing (Repurpose.io + 20 más). Moat cero.
- ❌ **No elijas algo que depende de mantener viralidad** (productos sociales). Sin budget de marketing no funciona.

---

## 9. Plan de acción (primeras 4 semanas)

### Opción elegida: AI Search Visibility Tracker (GEO)

### Semana 1 — Validación dura (ANTES de escribir código)

**Objetivo:** Confirmar que hay personas dispuestas a pagar por esto.

- [ ] Publicar en LinkedIn, Twitter y r/SEO:
  > *"Estoy construyendo una herramienta para trackear cómo ChatGPT/Claude/Perplexity mencionan tu marca. ¿Alguien pagaría $49/mes por ver esta data?"*
- [ ] Apuntar a 20 respuestas interesadas
- [ ] Hacer 5 entrevistas de 20 min con marketers que digan sí
- [ ] Preguntarles qué 10 queries ya les gustaría trackear hoy
- [ ] Documentar patrones en una nota: qué features piden más, qué precio les parece razonable

**Red flag de esta semana:** Si no consigues ni 10 respuestas interesadas, la demanda no está donde crees. Reevaluar antes de escribir código.

### Semanas 2-4 — MVP muy crudo

**Objetivo:** Producto mínimo funcional en manos de los 5 entrevistados.

**Semana 2:**
- [ ] Backend Python/Node que corre queries contra 3 APIs (GPT, Claude, Perplexity)
- [ ] DB Postgres con schema simple (queries, responses, mentions)
- [ ] Script manual para correr queries y guardar resultados

**Semana 3:**
- [ ] Frontend Next.js muy básico: lista de queries, tabla de resultados, chart simple
- [ ] Auth con Clerk o Supabase
- [ ] Primeros 5 usuarios beta del paso 1 usan la versión cruda (gratis)

**Semana 4:**
- [ ] Iterar brutalmente con feedback de los 5 beta users
- [ ] Identificar el feature más pedido → priorizarlo
- [ ] Preparar landing page pública

### Semanas 5-12 — Producto vendible + primeros ingresos

**Objetivo:** Primeros $500 MRR.

- [ ] Auth + billing Stripe completo
- [ ] Onboarding fluido (conecta primeras 10 queries en <5 min)
- [ ] Features que los beta users pidieron más
- [ ] Lanzamiento en Product Hunt
- [ ] Content marketing semanal con data propia
- [ ] Apuntar a 10 clientes pagados = primer $500 MRR

### Hitos clave

| Hito | Cuándo | Criterio de éxito |
|---|---|---|
| Validación de demanda | Final semana 1 | 10+ respuestas interesadas, 5 entrevistas hechas |
| MVP en manos de beta users | Final semana 4 | 5 personas usándolo activamente |
| Primeros ingresos | Mes 3 | $500 MRR |
| Traction real | Mes 6 | $1,500 MRR |
| Ingreso sostenible | Mes 12 | $12,000 MRR |

---

## 10. Fuentes consultadas

### Casos de éxito reales
- [Indie Hackers — Postiz $14.2k/mo case study](https://www.indiehackers.com/post/i-did-it-my-open-source-company-now-makes-14-2k-monthly-as-a-single-developer-f2fec088a4)
- [Market Clarity — Top 30 Most Profitable Indie SaaS](https://mktclarity.com/blogs/news/indie-saas-top)
- [Startups — Top 10 Solo Founder SaaS Success Stories 2025](https://startuups.com/blog/top-10-solo-founder-saas-success-stories-lessons-2025)
- [Medium — $100K MRR Micro-SaaS Founders](https://medium.com/startup-insider-edge/the-100k-mrr-illusion-5-micro-saas-founders-proving-its-possible-and-how-they-did-it-c3571dd336b3)

### Listas de ideas validadas
- [NxCode — 50 Micro SaaS Ideas 2026 with Revenue Data](https://www.nxcode.io/resources/news/micro-saas-ideas-2026)
- [SaasNiche — 50 Micro SaaS Opportunities from Reddit 2026](https://www.saasniche.com/blog/50-micro-saas-opportunities-from-reddit-in-2026)
- [PainOnSocial — 50+ Vertical SaaS Ideas 2026](https://painonsocial.com/blog/vertical-saas-ideas)
- [Calmops — 50+ Profitable Micro-SaaS Opportunities 2026](https://calmops.com/indie-hackers/micro-saas-ideas-2026/)
- [BigIdeasDB — 50 Ideas Validated from 238K Reddit Complaints](https://bigideasdb.com/micro-saas-ideas-2026)
- [Superframeworks — Best Micro SaaS Ideas for Solopreneurs 2026](https://superframeworks.com/articles/best-micro-saas-ideas-solopreneurs)
- [EntrepreneurLoop — 15 Bootstrapped SaaS Niches 2026](https://entrepreneurloop.com/bootstrapped-saas-niches-solo-founders/)

### Open source y gaps
- [Runa Capital — Awesome OSS Alternatives (GitHub)](https://github.com/RunaCapital/awesome-oss-alternatives)
- [Exit-SaaS — Best Open Source Alternatives 2026](https://exit-saas.io/blog/10-best-open-source-alternatives-popular-saas-tools-2026)
- [DreamHost — 50+ Open-Source Alternatives 2026](https://www.dreamhost.com/blog/open-source-alternatives/)

### Tendencias de mercado
- [Qubit Capital — Vertical SaaS 2026 Trends](https://qubit.capital/blog/rise-vertical-saas-sector-specific-opportunities)
- [Market Clarity — 21 Underserved Niches 2026](https://mktclarity.com/blogs/news/list-underserved-niches)
- [Y Combinator — Request for Startups 2026](https://www.ycombinator.com/rfs)
- [Superframeworks — YC RFS 2026 Indie Hacker Angle](https://superframeworks.com/articles/yc-rfs-startup-ideas-indie-hackers-2026)

### Developer tools & ecosystems
- [Dev.to — Complete Developer Productivity Tools Guide 2026](https://dev.to/_d7eb1c1703182e3ce1782/the-complete-guide-to-developer-productivity-tools-in-2026-165b)
- [Starter Story — Shopify App Profitability 2026](https://www.starterstory.com/ideas/shopify-app/profitability)

---

## Notas finales

- **Este documento es un snapshot.** El mercado cambia rápido en categorías como AI tooling. Revisar cada 2-3 meses.
- **La validación de demanda (Semana 1 del plan) es crítica.** No saltarse.
- **La competencia con Profound no es fatal.** Segmento SMB está libre.
- **La disciplina de bootstrap importa más que la idea.** Postiz llegó a $14k MRR siendo #100 en una categoría con muchos players.

**Pregunta pendiente:** ¿GEO te convence como apuesta principal, o prefieres explorar Cloud cost prediction (plan B)?
