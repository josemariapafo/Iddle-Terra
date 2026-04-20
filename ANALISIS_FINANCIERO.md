# Análisis Financiero: Top 5 Ideas

> **Objetivo:** Determinar inversión inicial, costos operativos, estrategia de free tier y break-even para cada uno de los 5 productos del documento de investigación
> **Fecha:** 2026-04-09
> **Documento relacionado:** `INVESTIGACION_NEGOCIO.md`

---

## Índice

1. [Resumen ejecutivo](#1-resumen-ejecutivo)
2. [Metodología y supuestos](#2-metodología-y-supuestos)
3. [Categorías de costos explicadas](#3-categorías-de-costos-explicadas)
4. [Producto #1 — GEO (AI Search Visibility)](#4-producto-1--geo-ai-search-visibility-tracker)
5. [Producto #2 — Loom OSS](#5-producto-2--loom-open-source-alternative)
6. [Producto #3 — LLM Observability](#6-producto-3--llm-observability)
7. [Producto #4 — Cloud Cost Prediction](#7-producto-4--cloud-cost-prediction)
8. [Producto #5 — AI PDF Extraction](#8-producto-5--ai-pdf-extraction)
9. [Ranking financiero final](#9-ranking-financiero-final)
10. [Plan de inversión por fases](#10-plan-de-inversión-por-fases)
11. [Riesgos financieros ocultos](#11-riesgos-financieros-ocultos)
12. [Recomendación final](#12-recomendación-final)

---

## 1. Resumen ejecutivo

### Tabla comparativa rápida

| # | Producto | Inversión inicial | Costo fijo/mes | Costo variable/usuario | Break-even (users) | Tiempo al break-even |
|---|---|---|---|---|---|---|
| 1 | **GEO** | $300-500 | $30 | $14-72 | 2-3 paying | 3-4 meses |
| 2 | **Loom OSS** | $500-1,000 | $80 | $0.15 | 10-15 paying | 5-6 meses |
| 3 | **LLM Observability** | $200-300 | $30 | $0.02 | 2 paying | 2-3 meses |
| 4 | **Cloud Cost Prediction** | $400-600 | $80 | $3 | 2-3 paying | 4-5 meses |
| 5 | **AI PDF Extraction** | $300-500 | $60 | $8-40 | 3 paying | 3-4 meses |

### Hallazgos clave

1. **Ninguno requiere capital significativo.** El más caro (Loom OSS) necesita ~$1000 máximo para estar en producción estable.
2. **LLM Observability es el más capital-eficiente** ($200-300) pero tiene el ceiling de revenue más bajo.
3. **GEO tiene la mejor relación costo/potencial** — inversión media, ceiling alto, break-even rápido.
4. **Loom OSS requiere 2-3x más capital** que los demás por la complejidad del MVP (4 meses vs 2-3 meses).
5. **PDF Extraction puede tener márgenes finos en planes altos** si no optimizas el modelo usado.

### Inversión mínima absoluta (cualquiera de los 5): **$200**
### Inversión recomendada con margen de seguridad: **$500-1,000**

---

## 2. Metodología y supuestos

### Cómo calculo los números

**Costos fijos:** infraestructura mínima para estar en producción (domain, hosting, DB, email, monitoring).

**Costos variables:** gastos que escalan con el número de usuarios activos (llamadas a APIs externas, storage, bandwidth, compute).

**Break-even operativo:** número de clientes pagando necesario para cubrir costos fijos + variables + free tier. **No incluye valorar tu tiempo**.

**Inversión inicial:** dinero necesario antes de tener clientes pagando, incluyendo 3 meses de hosting + desarrollo + testing.

### Supuestos clave

- **Stack elegido para estimaciones:** Next.js + Supabase/Postgres + Cloudflare (infra barata optimizada)
- **Tus horas (30/semana) NO tienen costo en cash** pero sí costo de oportunidad (ver sección 11)
- **Free tier estratégico:** captación, no gratis total. Siempre hay límites que forzan upgrade
- **Payment processor:** Stripe (2.9% + $0.30 por transacción). Esto reduce el margen neto ~3-4% adicional
- **Precios de APIs:** actualizado a Q1 2026 (estimados, verificar antes de lanzar)
- **Marketing:** asumo estrategia orgánica (contenido, comunidades, SEO). Si tienes que pagar ads, los números cambian dramáticamente
- **Infra scaling:** los primeros 100-500 usuarios caben en tiers cheap. Escalar después requiere refactor, no presupuesto mayor inmediato

### Nivel de confianza

| Número | Confianza | Por qué |
|---|---|---|
| Costos fijos | 🟢 Alta | Precios públicos y estables |
| Costos variables (APIs LLM) | 🟡 Media | Pueden cambiar, dependen de uso real |
| Customer acquisition rate | 🔴 Baja | Depende 100% de ejecución y calidad del producto |
| Revenue projections | 🟡 Media | Basadas en benchmarks de indie SaaS comparables |

**Regla de oro:** multiplica los costos estimados por 1.5x y divide las revenue projections por 1.5x para obtener un escenario más realista.

---

## 3. Categorías de costos explicadas

### Costos fijos (pago todo el mes tengas 0 o 1000 usuarios)

- **Domain:** $10-15/año (~$1/mes)
- **Hosting backend:** $0-50/mes (depende de stack)
  - Vercel Free tier: $0 (suficiente para <5k visitors/mes)
  - Railway/Render: $5-20/mes
  - Hetzner VPS: $5-15/mes (más barato, más setup)
- **Database:** $0-25/mes
  - Supabase/Neon free tier: $0 hasta 500MB
  - Postgres hosted: $15-25/mes
- **Object storage:** $0-10/mes
  - Cloudflare R2: $0 hasta 10GB (⚠️ **egress gratis, crítico para video**)
  - AWS S3: más caro, egress cuesta
- **Email transaccional:** $0-15/mes
  - Resend Free: 100 emails/día → $0
  - Postmark: $15/mes
- **Error tracking:** $0
  - Sentry Free tier suficiente al principio
- **Analytics:** $0
  - Plausible self-hosted o Umami

### Costos variables (escalan con uso)

- **LLM API calls:** depende del producto (crítico)
- **Compute (transcoding, batch jobs):** GPU cloud $0.50-2/hora
- **Bandwidth:** problema solo con video (#2)
- **Stripe fees:** 2.9% + $0.30 por transacción

### Costos one-time (pagas una vez)

- **Logo/branding:** $0-100 (usa Canva gratis o encarga en Fiverr)
- **Templates/UI kits:** $0-50 (muchos gratis, Tailwind UI es bueno si pagas)
- **Legal (ToS, Privacy Policy):** $0 (usa generadores gratis como Termly al principio)

### Costos ocultos (fáciles de olvidar)

- **Testing durante desarrollo:** tú mismo llamas las APIs mientras construyes ($20-100 de APIs)
- **Beta users gratis:** si les das 10 queries/día gratis, pagas tú esos tokens
- **Refunds y chargebacks:** ~1-2% del revenue en refunds estimado
- **Tu tiempo:** 30h × 4 semanas × $50/h de costo de oportunidad = $6000/mes ⚠️

---

## 4. Producto #1 — GEO (AI Search Visibility Tracker)

### Stack técnico asumido
- Next.js en Vercel (free tier inicialmente)
- Supabase Postgres (free tier → Pro cuando escalas)
- Cloudflare Workers para cron scheduler
- APIs: OpenAI, Anthropic, Perplexity, Google Gemini

### Costos fijos mensuales

| Item | Costo | Notas |
|---|---|---|
| Domain | $1 | `.com` estándar |
| Vercel | $0 → $20 | Free tier hasta 100GB bandwidth |
| Supabase | $0 → $25 | Free hasta 500MB, Pro después |
| Resend (email) | $0 → $20 | Free hasta 100/día |
| Sentry | $0 | Free tier OK |
| **Total inicial** | **$1-30/mes** | Escala a $65-100 con tracción |

### Costos variables (los importantes)

**Precio aproximado por query (respuesta de ~500 tokens):**

| Modelo | Costo por query | Notas |
|---|---|---|
| GPT-4o | $0.006-0.010 | Razonablemente preciso |
| Claude Sonnet | $0.005-0.009 | Similar a GPT-4o |
| Perplexity Sonar | $0.005 | Mejor para search-context |
| Gemini 1.5 Pro | $0.002-0.004 | El más barato |
| **Promedio por query** | **~$0.006** | 4 modelos promediados |

### Costo por plan (usuario pagando)

| Plan | Queries/día | Modelos | Total/mes | Costo API/mes | Revenue | Margen bruto |
|---|---|---|---|---|---|---|
| **Starter $49** | 20 | 4 | 2,400 calls | $14 | $49 | $35 (71%) |
| **Growth $149** | 100 | 4 | 12,000 calls | $72 | $149 | $77 (52%) |
| **Pro $399** | 300 | 4 | 36,000 calls | $216 | $399 | $183 (46%) |

⚠️ **Importante:** el plan Pro tiene margen más bajo. Considerar subir a $499 o limitar a 200 queries.

### Estrategia de free tier

**Free tier propuesto:**
- 5 queries únicas
- 2 modelos (GPT + Claude)
- Refresh semanal (no diario)
- Sin alertas ni exports

**Costo por usuario free:**
- 5 queries × 2 modelos × 4 semanas = **40 calls/mes**
- 40 × $0.006 = **$0.24/mes por free user**

**Cuántos free users puedes permitirte:**
- Con $30/mes de margen de un paying user → subsidia 125 free users
- Con 10 paying users → subsidias 1,250 free users

**Free tier es sostenible**, pero conviene limitar para evitar abuse. Cap en 500-1000 free users con waitlist después.

### Inversión inicial necesaria

| Item | Costo | Notas |
|---|---|---|
| Domain (2 años) | $25 | |
| Hosting 3 meses (pre-revenue) | $0-45 | Free tiers iniciales |
| API testing durante desarrollo | $100-150 | Testing constante llamando APIs |
| Beta users gratis (5 users × 2 meses) | $5-10 | Subsidio de free tier beta |
| Landing page + marketing tools | $0 | Orgánico |
| Buffer de imprevistos | $100-200 | |
| **TOTAL INVERSIÓN INICIAL** | **$230-430** | |

**Redondeo realista: $300-500.**

### Break-even analysis

**Escenario de costos mensuales con 50 usuarios pagando (mix típico):**
- 35 Starter ($49) + 12 Growth ($149) + 3 Pro ($399) = **$4,712 MRR**
- Costos API: 35×$14 + 12×$72 + 3×$216 = **$1,982**
- Costos fijos: $70
- **Beneficio neto: $2,660/mes** (margen 56%)

**Break-even operativo (cubrir costos fijos):**
- Necesitas cubrir $30-70/mes de fijos
- 1-2 Starter users es suficiente = **2 usuarios pagando**

**Break-even de inversión inicial ($300-500):**
- Con margen promedio de $40/user/mes
- $400 ÷ $40 = **10 usuarios × 1 mes** = recuperas la inversión en el primer mes con ≥10 users

### Escenarios (12 meses)

#### 🔴 Pesimista (1 de 5)
- Mes 3: 0 paying
- Mes 6: 3 paying → $147 MRR
- Mes 12: 15 paying → $735 MRR
- **Burn total: ~$500** (inversión perdida o casi)

#### 🟡 Realista (mediana)
- Mes 3: 2 paying → $98 MRR (break-even operativo)
- Mes 6: 15 paying → $735 MRR
- Mes 12: 60 paying → $2,940 MRR
- **Revenue total año 1: ~$12,000**

#### 🟢 Optimista (si ejecutas muy bien)
- Mes 3: 10 paying → $490 MRR
- Mes 6: 50 paying → $2,450 MRR
- Mes 12: 180 paying → $8,820 MRR
- **Revenue total año 1: ~$40,000**

### Veredicto #1
✅ **Viable con $300-500.** Break-even operativo rápido (2 usuarios). El riesgo principal NO es financiero sino de ejecución y competencia con Profound.

---

## 5. Producto #2 — Loom Open Source Alternative

### Stack técnico asumido
- Next.js frontend en Vercel
- Node.js backend en Hetzner VPS (dedicado para transcoding)
- Cloudflare R2 para storage (⚠️ **elección crítica, egress gratis**)
- PostgreSQL en VPS o Supabase
- ffmpeg para transcoding (self-hosted, no AWS MediaConvert)
- BullMQ + Redis para queue

### Costos fijos mensuales

| Item | Costo | Notas |
|---|---|---|
| Domain | $1 | |
| Hetzner VPS (backend + transcoding) | $15-40 | CX31 o CPX31 recomendado |
| Vercel (frontend) | $0 → $20 | |
| Cloudflare R2 storage | $0-15 | 10GB free, después $0.015/GB |
| Email | $15 | |
| **Total inicial** | **$31-90/mes** | Puede llegar a $150 con tracción |

### Costos variables

**Este es el punto crítico de Loom OSS.** La magia está en usar R2 con egress gratis.

**Asumiendo video promedio:**
- Resolución: 720p
- Tamaño: ~10MB/minuto de video
- Video promedio: 3 minutos = 30MB

**Costos por usuario (10 videos/mes, 50 views/video):**

| Recurso | Cantidad | Costo | Por usuario/mes |
|---|---|---|---|
| Storage | 300MB | $0.015/GB | $0.0045 |
| Egress (views) | 15GB | **$0 con R2** | $0 |
| Transcoding CPU | ~5 min compute | VPS compartido | ~$0.10 |
| **Total variable** | | | **~$0.15** |

⚠️ **Si usaras AWS S3 + CloudFront:** $2-5/usuario/mes. Diferencia brutal. **R2 es indispensable para este producto.**

### Costo por plan

| Plan | Revenue | Costo variable | Margen bruto |
|---|---|---|---|
| **Free** (25 videos) | $0 | $0.05 | -$0.05 |
| **Pro $8** | $8 | $0.15 | $7.85 (98%) |
| **Team $15** | $15 | $0.30 | $14.70 (98%) |

**Márgenes espectaculares gracias a R2.**

### Estrategia de free tier

**Free tier propuesto:**
- 25 videos totales
- 5 min máximo por video
- Retención 90 días
- Sin workspace colaborativo
- Watermark "Made with [tu producto]" (opcional)

**Costo por usuario free:**
- Máximo ~$0.05/mes si usa mucho
- Prácticamente regalado

### Inversión inicial necesaria

| Item | Costo | Notas |
|---|---|---|
| Domain (2 años) | $25 | |
| Hetzner VPS 4 meses (pre-revenue) | $60-160 | MVP toma 4 meses |
| R2 storage | $10 | |
| Email | $60 | 4 meses |
| Testing + beta users | $20 | |
| SSL cert | $0 | Let's Encrypt |
| Buffer imprevistos | $150-200 | Mayor por complejidad |
| **TOTAL INVERSIÓN INICIAL** | **$325-475** | |

**Redondeo realista: $500-1,000** (margen de seguridad mayor por complejidad técnica).

### Break-even analysis

**Costos fijos:** $80/mes aprox.

**Break-even operativo:**
- Contribución por Pro user: $7.85/mes
- Break-even = $80 / $7.85 = **11 usuarios pagando**

**Break-even de inversión inicial ($1000):**
- Con contribución promedio de $10/user/mes
- $1000 ÷ $10 = **100 user-meses** (ej: 20 users × 5 meses, o 10 users × 10 meses)

### Escenarios (12 meses)

#### 🔴 Pesimista
- Mes 4: 0 paying
- Mes 6: 5 paying → $40 MRR
- Mes 12: 25 paying → $200 MRR (por debajo de break-even)
- **Conclusión: necesitas tracción real o mueres**

#### 🟡 Realista
- Mes 4: 3 paying → $24 MRR
- Mes 6: 15 paying → $120 MRR (cerca de break-even)
- Mes 12: 80 paying → $640 MRR
- **Revenue total año 1: ~$3,500**

#### 🟢 Optimista
- Mes 4: 10 paying
- Mes 6: 50 paying → $400 MRR
- Mes 12: 250 paying → $2,000 MRR
- **Revenue total año 1: ~$11,000**

### Veredicto #2
⚠️ **Viable con $500-1000 y R2**, pero **break-even más lento** (11 users vs 2 de GEO). El ceiling de ingresos es alto pero el camino más largo. Requiere más disciplina en costos desde día 1.

---

## 6. Producto #3 — LLM Observability

### Stack técnico asumido
- Next.js en Vercel
- Supabase Postgres (ClickHouse eventualmente para traces a escala)
- No depende de APIs externas caras (TÚ no llamas a LLMs)

### Costos fijos mensuales

| Item | Costo | Notas |
|---|---|---|
| Domain | $1 | |
| Vercel | $0 → $20 | Free tier OK al principio |
| Supabase | $0 → $25 | Traces caben en Postgres hasta ~10k usuarios |
| Email | $0 → $15 | |
| **Total inicial** | **$1-30/mes** | Escala a $60-80 con tracción |

### Costos variables

**Casi inexistentes.** Los traces son JSON en una base de datos.

Por usuario:
- Solo Dev ($19/mo, 100k traces): ~100MB storage = **$0.02/mes**
- Team ($79/mo, 1M traces): ~1GB storage = **$0.02/mes**

**Margen bruto: >99% en todos los planes.**

### Estrategia de free tier

**Free tier propuesto:**
- 10k traces/mes
- 1 proyecto
- 7 días de retención
- Sin features de equipo

**Costo por usuario free:** prácticamente $0. Puedes tener 10,000 free users sin sudar.

### Inversión inicial necesaria

| Item | Costo | Notas |
|---|---|---|
| Domain (2 años) | $25 | |
| Hosting 3 meses | $0-30 | Free tiers |
| Sin APIs externas caras | $0 | |
| Testing | $20 | |
| Buffer | $100-150 | |
| **TOTAL INVERSIÓN INICIAL** | **$145-225** | |

**Redondeo realista: $200-300.** El más barato de todos.

### Break-even analysis

**Costos fijos:** $30/mes

**Break-even operativo:**
- Contribución por Solo Dev user: $19/mes (margen casi puro)
- Break-even = $30 / $19 = **2 usuarios pagando**

**Break-even de inversión inicial ($300):**
- $300 ÷ $19 = **16 user-meses** (ej: 8 users × 2 meses)

### Escenarios (12 meses)

#### 🔴 Pesimista (Langfuse te come)
- Mes 3: 2 paying → $38 MRR
- Mes 6: 8 paying → $152 MRR
- Mes 12: 25 paying → $475 MRR
- **Revenue total año 1: ~$2,500**

#### 🟡 Realista
- Mes 3: 5 paying → $95 MRR
- Mes 6: 20 paying → $380 MRR
- Mes 12: 60 paying → $1,140 MRR
- **Revenue total año 1: ~$5,500**

#### 🟢 Optimista
- Mes 3: 15 paying → $285 MRR
- Mes 6: 60 paying → $1,140 MRR
- Mes 12: 200 paying → $3,800 MRR
- **Revenue total año 1: ~$15,000**

### Veredicto #3
✅ **El más capital-eficiente** con $200-300. Break-even inmediato (2 usuarios). **PERO:** ceiling más bajo de revenue por competencia con Langfuse y riesgo de commoditización. Ideal si quieres minimizar riesgo financiero.

---

## 7. Producto #4 — Cloud Cost Prediction

### Stack técnico asumido
- Next.js en Vercel
- Postgres con timeseries (historical cost data)
- Worker en VPS para pulling APIs de AWS/GCP cada hora

### Costos fijos mensuales

| Item | Costo | Notas |
|---|---|---|
| Domain | $1 | |
| Vercel | $0 → $20 | |
| Postgres | $25 | Necesita más que free tier por historical data |
| VPS para workers | $15 | Hetzner básico |
| Email | $15 | |
| **Total inicial** | **$56-76/mes** | Escala a $100-120 |

### Costos variables

**AWS Cost Explorer API:**
- Primera request del día: gratis
- Adicionales: $0.01 cada una
- **Estrategia:** cachear agresivamente, actualizar cada 6h = 4 calls/día = gratis las primeras, $0.03/día adicional = $1/mes

**Por usuario:**
- Pulling cost data: 4 calls/día × 30 = 120 calls/mes
- $1.20/usuario/mes en API calls

**GCP Billing API:** gratuita básicamente.
**Azure:** similar, APIs gratis con limits.

**Costo promedio: $1-3/usuario/mes**

### Costo por plan

| Plan | Revenue | Costo variable | Margen bruto |
|---|---|---|---|
| **Free** (1 account, $10k spend) | $0 | $1 | -$1 |
| **Pro $49** | $49 | $2 | $47 (96%) |
| **Scale $199** | $199 | $3 | $196 (98%) |

### Estrategia de free tier

**Free tier propuesto:**
- 1 cuenta cloud
- Hasta $10k/mes de spend tracked
- Refresh diario (no horario)
- 30 días de historial (no anual)
- Sin alertas

**Costo por usuario free: ~$1/mes**

### Inversión inicial necesaria

| Item | Costo | Notas |
|---|---|---|
| Domain | $25 | |
| Hosting 4 meses | $120-200 | MVP más lento (4 meses por complejidad APIs) |
| AWS/GCP account propia para testing | $20-50 | Necesitas spend real para probar |
| Testing APIs | $20 | |
| Buffer | $150-200 | |
| **TOTAL INVERSIÓN INICIAL** | **$335-495** | |

**Redondeo realista: $400-600.**

### Break-even analysis

**Costos fijos:** $80/mes

**Break-even operativo:**
- Contribución por Pro user: $47
- Break-even = $80 / $47 = **2 usuarios pagando**

### Escenarios (12 meses)

#### 🔴 Pesimista
- Mes 4: 1 paying → $49
- Mes 6: 5 paying → $245
- Mes 12: 20 paying → $980 MRR
- **Revenue total año 1: ~$5,000**

#### 🟡 Realista
- Mes 4: 3 paying → $147
- Mes 6: 15 paying → $735
- Mes 12: 50 paying → $2,450 MRR
- **Revenue total año 1: ~$12,000**

#### 🟢 Optimista
- Mes 4: 10 paying → $490
- Mes 6: 40 paying → $1,960
- Mes 12: 150 paying → $7,350 MRR
- **Revenue total año 1: ~$35,000**

### Veredicto #4
✅ **Viable con $400-600.** Break-even rápido. **Ventaja:** márgenes altísimos (96-98%) porque los cloud APIs son mayormente gratis. **Desventaja:** MVP más lento + complejidad técnica.

---

## 8. Producto #5 — AI PDF Extraction

### Stack técnico asumido
- Next.js en Vercel
- Supabase Postgres
- Cloudflare R2 para PDFs almacenados
- APIs: GPT-4o Vision o Claude Sonnet Vision

### Costos fijos mensuales

| Item | Costo | Notas |
|---|---|---|
| Domain | $1 | |
| Vercel | $0 → $20 | |
| Supabase | $0 → $25 | |
| R2 storage | $5-10 | PDFs uploaded |
| Email | $15 | |
| **Total inicial** | **$21-71/mes** | Escala a $90-120 |

### Costos variables (CRÍTICOS)

**Vision API pricing (abril 2026 estimado):**
- GPT-4o Vision: ~$0.015-0.03 por página
- Claude Sonnet Vision: ~$0.010-0.025 por página
- Gemini 1.5 Pro Vision: ~$0.005-0.010 por página (más barato)

**Asumiendo promedio $0.015/página, factura promedio de 2 páginas:**
- Costo por PDF: $0.03

### Costo por plan

| Plan | PDFs/mes | Costo API | Revenue | Margen bruto |
|---|---|---|---|---|
| **Free** | 20 | $0.60 | $0 | -$0.60 |
| **Starter $29** | 200 | $6.00 | $29 | $23 (79%) |
| **Pro $99** | 1,000 | $30.00 | $99 | $69 (70%) |
| **Firm $299** | 5,000 | $150.00 | $299 | $149 (50%) |

⚠️ **Plan Firm tiene margen fino.** Opciones para mejorarlo:
1. **Subir a $499/mes** → margen 70%
2. **Usar Gemini Flash Vision** → costo $0.005/página, margen mejor
3. **Cache hashes** de PDFs ya procesados (cliente ya subió esta factura el mes pasado)
4. **Modelo propio** self-hosted en Modal/Replicate con A10 ($0.50/hr, ~100 pages/hr = $0.005/page)

### Estrategia de free tier

**Free tier propuesto:**
- 20 PDFs/mes
- 1 tipo de documento (invoices o statements)
- Sin export directo a QB/Xero
- Sin batch processing

**Costo por usuario free: $0.60/mes**

**Red flag:** free tier es más caro aquí. Si tienes 1000 free users, son $600/mes de burn. Necesitas cap más temprano.

### Inversión inicial necesaria

| Item | Costo | Notas |
|---|---|---|
| Domain | $25 | |
| Hosting 2 meses | $0-40 | MVP rápido |
| API testing durante desarrollo | $80-120 | Muchos PDFs de prueba |
| Beta users gratis (3 meses) | $30-60 | Subsidio alto |
| Buffer | $150-200 | |
| **TOTAL INVERSIÓN INICIAL** | **$285-445** | |

**Redondeo realista: $300-500.**

### Break-even analysis

**Costos fijos:** $60/mes

**Break-even operativo:**
- Contribución por Starter user: $23
- Break-even = $60 / $23 = **3 usuarios pagando**

### Escenarios (12 meses)

#### 🔴 Pesimista
- Mes 2: 0 paying
- Mes 6: 8 paying → $232 MRR
- Mes 12: 30 paying → $870 MRR
- **Revenue total año 1: ~$4,000**

#### 🟡 Realista
- Mes 2: 2 paying → $58
- Mes 6: 20 paying → $580 MRR
- Mes 12: 80 paying → $2,320 MRR
- **Revenue total año 1: ~$12,000**

#### 🟢 Optimista
- Mes 2: 8 paying → $232
- Mes 6: 60 paying → $1,740
- Mes 12: 200 paying → $5,800 MRR
- **Revenue total año 1: ~$28,000**

### Veredicto #5
✅ **Viable con $300-500.** Break-even rápido. **Ventaja:** MVP el más rápido (2 meses). **Riesgo:** margen se come rápido si los clientes procesan muchos PDFs. **Crítico:** usar Gemini Flash Vision o modelo propio para mejorar márgenes.

---

## 9. Ranking financiero final

### Ranking por capital-eficiencia pura

1. **🥇 #3 LLM Observability** — $200 inversión, 2 users break-even, 99% margen
2. **🥈 #1 GEO** — $300-500, 2-3 users break-even, 52-71% margen
3. **🥉 #4 Cloud Cost Prediction** — $400-600, 2-3 users break-even, 96% margen
4. **#5 PDF Extraction** — $300-500, 3 users break-even, 70-79% margen
5. **#2 Loom OSS** — $500-1000, 11 users break-even, 98% margen (gracias a R2)

### Ranking por ROI esperado (revenue potential vs inversión)

1. **🥇 #1 GEO** — Mejor balance inversión/ceiling
2. **🥈 #4 Cloud Cost Prediction** — Altos márgenes, buen ceiling
3. **🥉 #5 PDF Extraction** — MVP rápido, mercado grande
4. **#3 LLM Observability** — Menor riesgo pero menor ceiling
5. **#2 Loom OSS** — Mayor capital, mayor tiempo, mayor upside

### Tabla final consolidada

| Criterio | #1 GEO | #2 Loom | #3 LLM Obs | #4 Cloud | #5 PDF |
|---|---|---|---|---|---|
| Inversión inicial | $300-500 | $500-1000 | $200-300 | $400-600 | $300-500 |
| Break-even users | 2-3 | 11-15 | 2 | 2-3 | 3 |
| Margen bruto típico | 60% | 98% | 99% | 96% | 70% |
| Tiempo a MVP | 3 meses | 4 meses | 2-3 meses | 3-4 meses | 2 meses |
| Revenue año 1 (realista) | $12,000 | $3,500 | $5,500 | $12,000 | $12,000 |
| Revenue año 1 (optimista) | $40,000 | $11,000 | $15,000 | $35,000 | $28,000 |
| Riesgo financiero | Bajo | Medio | Muy bajo | Bajo | Medio |

---

## 10. Plan de inversión por fases

### Fase 0 — Pre-inversión (GRATIS, semana 1)

Antes de gastar ni $1:
- [ ] Validación con 5 entrevistas (objetivo: 5 "yes, pagaría por esto")
- [ ] Post en LinkedIn/Twitter/Reddit para medir interés
- [ ] Research de competidores (feature gaps, pricing)
- [ ] Landing page simple con waitlist (Carrd gratis o Vercel gratis)

**Si no consigues al menos 10 personas interesadas → NO INVIERTAS. Reevalúa.**

### Fase 1 — Mínimo viable (Semanas 2-4, inversión: $50-100)

- [ ] Comprar domain: $12/año
- [ ] Setup Vercel + Supabase gratis
- [ ] Primer código: $0
- [ ] API tokens para testing: $30-50
- [ ] Stripe cuenta creada: $0
- [ ] Beta users gratis (3-5 personas): $10-30 en API costs
- [ ] **NO gastes en ads, logos caros, diseño profesional, tools SaaS innecesarios**

### Fase 2 — MVP en producción (Meses 2-3, inversión adicional: $100-300)

- [ ] Hosting pagado si free tier se queda corto: $20/mes
- [ ] Database Pro tier si es necesario: $25/mes
- [ ] Email transaccional: $15/mes
- [ ] APIs en producción: $50-100/mes
- [ ] Primeras conversiones a paying users: empiezan a cubrir costos

### Fase 3 — Crecimiento validado (Meses 4-12, inversión: revenue propio)

Una vez con ≥10 paying users, **reinviertes el revenue** en:
- Hosting escalable
- Más features pedidas por usuarios
- Posiblemente un diseñador freelance ($200-500 one-time)
- Opcionalmente: ads de validación ($50-100 para probar canales)

### Total máximo a gastar antes de tener ingresos

| Fase | Inversión acumulada |
|---|---|
| Fase 0 | $0 |
| Fase 1 | $100 |
| Fase 2 | $300-600 |

**Nunca inviertas más de $600-1000 sin haber validado demanda real.**

---

## 11. Riesgos financieros ocultos

### 1. Free tier abuse

**Riesgo:** usuarios malos crean múltiples cuentas free para no pagar. En productos con APIs caras (GEO, PDF Extract), esto puede costar $100-500/mes si no se controla.

**Mitigación:**
- Rate limiting por IP/device
- Email verification obligatoria
- Cap global de free users con waitlist
- Monitoreo de uso anómalo

### 2. Stripe fees

**Impacto:** 2.9% + $0.30 por transacción. En un plan de $49, son $1.72 de fee = 3.5% real.

**No tan grave** pero considéralo en márgenes:
- Plan $49 → recibes $47.28
- Plan $149 → recibes $144.39
- Plan $399 → recibes $387.13

### 3. Chargebacks y refunds

**Riesgo:** ~1-2% de revenue en refunds esperados. Chargebacks cuestan $15-25 adicionales por caso.

**Mitigación:** 
- Trial period claro (7-14 días)
- Política de refund generosa al principio (reduce chargebacks)
- Cancelación fácil self-service

### 4. API pricing changes

**Riesgo crítico en GEO y PDF Extract:** si OpenAI/Anthropic suben precios 50%, tu margen se come.

**Mitigación:**
- Multi-modelo desde día 1 (poder switchear a más barato)
- Caching agresivo
- Usar modelos open source self-hosted cuando sea viable

### 5. Costo de oportunidad de tu tiempo

**El costo más grande que nadie calcula:**
- 30h/semana × 4 semanas = 120h/mes
- Si podrías facturar $40/h como freelance → **$4,800/mes de costo de oportunidad**
- En 12 meses: **$57,600 "invertidos" en tiempo**

Esto no es cash, pero es real. Si al mes 12 tu producto genera $2,000 MRR, financieramente estás peor que haciendo freelance. 

**La apuesta del bootstrapping es:**
1. Escapar del freelance y tener asset que vale 10x ARR al vender ($240k a 10x de $2k MRR)
2. Romper el techo de ingresos por hora
3. Libertad geográfica y de agenda

Esta apuesta es válida pero **debes entrar con los ojos abiertos**.

### 6. Tax y burocracia

**Fácil de olvidar:**
- Stripe te reporta al fisco
- Si eres autónomo en España: cuota RETA ~$300/mes
- IVA en clientes europeos (MOSS simplifica)
- Contador: $50-100/mes

**Mitigación:** los primeros 6 meses opera como persona física + factura ocasional. Cuando llegues a $1,500 MRR, formaliza con gestor (~$500 setup + $80/mes).

### 7. Scaling infrastructure

**Trampa:** los primeros 100 usuarios caben en tiers baratos ($50/mes). El usuario 101 puede requerir upgrade que doble el costo ($100-200/mes). Esto es bueno (tienes tracción) pero no lo tienes presupuestado.

**Mitigación:** tener siempre 2-3 meses de runway operativo guardado.

---

## 12. Recomendación final

### 🏆 Apuesta financiera recomendada: **#1 GEO**

**Por qué es la mejor apuesta inversión/retorno:**

1. **Inversión moderada** ($300-500) comparado con el upside
2. **Break-even rápido** (2-3 usuarios, mes 3-4)
3. **Mayor ceiling realista** ($12-40k año 1)
4. **Costos variables manejables** y optimizables
5. **Free tier barato** ($0.24/user) permite captación agresiva

### Plan B si quieres máxima seguridad: **#3 LLM Observability**

Si tu prioridad absoluta es **no perder dinero**, LLM Observability es la apuesta más segura:
- Solo $200-300 de inversión
- Break-even en 2 usuarios
- 99% margen
- Riesgo casi cero de quemar capital

**Trade-off:** ceiling más bajo ($5-15k año 1).

### Plan C si quieres mayor ceiling con tolerancia a más tiempo: **#2 Loom OSS**

Solo si tienes $1000 disponibles Y tolerancia a 5-6 meses sin ingresos. Mayor upside a largo plazo pero más arriesgado.

### Presupuesto total recomendado para lanzar GEO

| Concepto | Monto |
|---|---|
| Inversión inicial (antes de revenue) | $400 |
| Buffer de imprevistos | $200 |
| Runway operativo 3 meses (por si tarda) | $150 |
| **TOTAL A TENER DISPONIBLE** | **$750** |

Con $750 puedes lanzar GEO sin estrés y llegar al mes 6 aunque la tracción sea lenta.

### Si solo tienes $200-300

No pasa nada. Elige **#3 LLM Observability**. Es financieramente la más viable con poco capital. Ceiling más bajo pero seguro.

### Si no tienes ni $200

**Primera prioridad:** junta $200 haciendo algún freelance de 1-2 semanas. Lanzar un SaaS con $0 es posible pero multiplica la fricción x10 (no puedes pagar hosting, no puedes testear APIs, no puedes aceptar pagos bien).

---

## Notas finales

- **Todos los números son estimaciones.** Verifica precios actuales antes de lanzar (APIs cambian).
- **La ejecución importa más que el capital.** Postiz llegó a $14k MRR con menos de $500 de inversión. Lo crítico es encontrar una sub-comunidad que pague.
- **No sobre-inviertas al principio.** Resiste la tentación de comprar "ese tool SaaS que parece útil", "ese curso", "ese logo profesional". Cada $50 ahorrado es 2 meses más de runway.
- **Reinvierte agresivamente después del break-even.** Una vez en $500+ MRR, cada dólar de beneficio puede ir a features, marketing test, o acelerar desarrollo.

**Pregunta pendiente:** ¿Confirmas que vas con GEO? Si sí, en la siguiente vuelta puedo hacerte el plan técnico detallado del MVP (arquitectura, stack específico, diseño de DB, primeros endpoints) o el plan de go-to-market (posts de validación, templates de entrevistas, landing page wireframe).
