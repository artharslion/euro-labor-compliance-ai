# 01 — SETU Overview

## What is SETU?

**SETU** = **Stichting Elektronische Transacties Uitzendbranche**
(Foundation for Electronic Transactions in the Staffing Industry)

- **Jurisdiction**: Netherlands (expanding internationally via Peppol)
- **Founded by**: ABU (Algemene Bond Uitzendondernemingen — Dutch Association of Temporary Work Agencies)
- **Purpose**: Develop and maintain open ICT standards for electronic data exchange in the flexible staffing industry
- **Legal Status**: Listed on the Dutch "Pas-toe-of-leg-uit" (comply-or-explain) mandatory standards list — obligatory for all (semi-)public institutions

## Official Resources

| Resource | URL |
|----------|-----|
| Official Site (Dutch) | https://setu.nl |
| Standards Portal | https://standard.setu.nl |
| Semantic Treehouse | https://setu.semantic-treehouse.nl |
| GitHub Organization | https://github.com/setu-standards |
| API Specifications | https://standard.setu.nl/docs/api/ |

## SETU Standards Portfolio

The standards cover the complete flexible staffing lifecycle:

| Standard | Version | Purpose |
|----------|---------|---------|
| **Staffing Order** | v2.0 (Dec 2024) | Staffing customer requests/orders employees for a position |
| **Human Resource** | v2.0 | Staffing supplier matches a human resource to a position |
| **Assignment** | v2.0 | Confirms placement of a worker at the staffing customer |
| **Timecard** | v2.0 | Reports time recording and expense info to staffing supplier |
| **Invoice** | v2.0 (UBL-based) | Standardized e-invoicing for the staffing industry |
| **Planning & Scheduling** | v1.0 | Exchange planning constraints, requests, and assignments |
| **Inquiry Pay Equity** | v2.0 (Mar 2026) | Ensures flex workers earn equal pay — **our primary focus** |
| **Vacancy** | TBD (2025) | Job vacancy posting |

## SETU 2.0 Modernization (December 2024)

The v2.0 standards represent a major modernization:

- Based on a standardized **SETU domain language** (ontology)
- Built on **HR Open v4.3** international standards
- Support both **XML and JSON** message formats
- **REST APIs** with OpenAPI Specifications (OAS)
- Deployed on the **Peppol network** for international e-procurement
- XSD Schemas + Schematron validation via Logius/DigiPoort (Dutch government gateway)

## Regulatory Context

### Key EU/Dutch Regulations

1. **WAADI** (Wet Allocatie Arbeidskrachten door Intermediairs) — Labor leasing governance
2. **CAO System** — Collective Labor Agreements (ABU CAO covers ~85% of temp workers; NBBU CAO covers smaller agencies)
3. **EU Pay Transparency Directive (2023/970)** — Requires objective, gender-neutral criteria for pay variance
4. **EU AI Act (Regulation 2024/1689)** — Recruitment AI classified as high-risk (Annex III, point 4a)
5. **EU Posted Workers Directive** — Cross-border worker posting compliance
6. **ViDA** (VAT in the Digital Age) — Upcoming EU e-invoicing and digital reporting mandate

### Key Industry Bodies

| Body | Role |
|------|------|
| **ABU** | Dutch Association of Temporary Work Agencies (founded SETU) |
| **NBBU** | Dutch Association of Mediation and Temp Agencies |
| **Peppol Authority (NPa)** | Dutch Peppol network authority — integrated SETU standards |
| **Logius** | Dutch government digital service — enforces SETU via DigiPoort |
| **Forum Standaardisatie** | Dutch standards forum — lists SETU as mandatory |
| **ELA** (European Labour Authority) | EU-level enforcement of labor mobility rules |

## AVP & MAVP — Key Concepts

### AVP (Algemeen Verbindend Verklaard)

When a CAO is declared "algemeen verbindend verklaard" (universally binding) by the Dutch government, it becomes legally enforceable across the entire industry — even for companies that are not members of the negotiating union or employer association.

**Implication for AI compliance**: The system must maintain a dynamic AVP rule registry, tracking which CAO clauses have been declared universally binding and when. This directly affects which fields are mandatory vs. optional.

### MAVP (Meldingsplicht / Minimumbepalingen)

In the context of cross-border worker posting (EU Posted Workers Directive), MAVP refers to mandatory minimum standards that must be reported and complied with regardless of the worker's home country contract. This includes minimum wage, maximum working hours, minimum paid leave, and health & safety requirements.

**Implication for AI compliance**: For multi-jurisdiction scenarios, the system must automatically identify which SETU fields are subject to MAVP (mandatory minimums) vs. optional supplements. This requires multi-jurisdiction comparison logic — e.g., "Polish worker posted to Netherlands → Dutch AVP-applied CAO minimum standards → compare with Polish contract."

## Why This Project Matters

1. **Regulatory pressure**: EU Pay Transparency Directive + Dutch mandatory SETU standards create strong compliance demand
2. **Data fragmentation**: Thousands of Dutch staffing agencies and their clients have compliance data scattered across heterogeneous documents
3. **Manual process cost**: Manually filling SETU forms (especially Inquiry Pay Equity) takes hours per worker placement and is highly error-prone
4. **Penalty risk**: Non-compliance with equal pay rules carries significant fines from Dutch labor authorities and the ELA
5. **Internationalization**: Peppol network integration means SETU standards will expand beyond the Netherlands
