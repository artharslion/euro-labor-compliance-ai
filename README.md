# Euro Labor Compliance AI

AI-driven compliance middleware for European (Dutch) staffing industry labor standards.

## Problem

SETU (Stichting Elektronische Transacties Uitzendbranche) defines open ICT standards for electronic data exchange in the Dutch flexible staffing industry. The **Inquiry Pay Equity v2.0** standard requires enterprises to submit massive amounts of structured remuneration data — salary scales, allowances, holiday pay, sick pay, leave, pension, IKB, and more — to prove compliance with equal pay regulations.

The problem: this data is scattered across enterprise documents in wildly varying formats (PDF contracts, Excel salary tables, CAO texts spanning hundreds of pages). Manual entry into SETU-compliant formats is error-prone, time-consuming, and a compliance liability.

## Solution

An AI middleware layer that:

1. **Ingests** enterprise documents in any format (PDF, scanned images, DOCX, Excel)
2. **Extracts** structured data via AI OCR + Layout Understanding
3. **Maps** extracted fields to SETU standard fields via LLM semantic reasoning
4. **Identifies gaps** between available data and SETU required fields
5. **Generates smart forms** with only missing fields, contextual cues, and compliance hints

## Key Terms

| Term | Definition |
|------|------------|
| **SETU** | Stichting Elektronische Transacties Uitzendbranche — Dutch staffing industry data exchange standards body |
| **AVP** | Algemeen Verbindend Verklaard — CAO clauses declared universally binding by Dutch government |
| **MAVP** | Meldingsplicht / Minimumbepalingen — mandatory minimum standards for cross-border worker posting |
| **CAO** | Collectieve Arbeidsovereenkomst — Collective Labor Agreement |
| **IKB** | Individueel Keuze Budget — Individual Choice Budget (Dutch flexible benefits) |
| **CLA** | Collective Labour Agreement (English equivalent of CAO) |
| **Peppol** | Pan-European Public Procurement Online network for e-invoicing |
| **ABU / NBBU** | Dutch associations of temporary work agencies |

## Documentation

| Document | Content |
|----------|---------|
| [01 — SETU Overview](docs/01-setu-overview.md) | SETU organization, standards, regulatory context |
| [02 — Data Model](docs/02-data-model.md) | Complete Inquiry Pay Equity v2.0 JSON schema (105+ definitions) |
| [03 — Code Lists](docs/03-code-lists.md) | Full enumeration dictionaries (78 allowance codes, 31 interval codes, etc.) |
| [04 — Benchmark](docs/04-wijzerbelonen-benchmark.md) | Official wijzerbelonen.nl gold standard + 37-field mapping table |
| [05 — OCR Challenges](docs/05-ocr-challenges.md) | Core AI OCR technical challenges for SETU compliance |
| [06 — Pipeline Architecture](docs/06-pipeline-architecture.md) | 5-layer AI pipeline architecture design |
| [07 — Market Landscape](docs/07-market-landscape.md) | Competitive analysis and market opportunities |

## Key References

- SETU Official Site: https://setu.nl
- SETU Standards Portal: https://standard.setu.nl
- SETU GitHub: https://github.com/setu-standards
- Semantic Treehouse (Data Models): https://setu.semantic-treehouse.nl
- wijzerbelonen.nl (Gold Standard): https://wijzerbelonen.nl
- Peppol + SETU Integration: https://www.peppol.nl/en/news/setu-standards-now-available-peppol-network

## Technology Stack (Target)

```
OCR Layer:     LayoutLM / Azure Document Intelligence / Tesseract
Mapping Layer: LLM (GPT-4/Claude) + RAG for SETU ontology
Validation:    JSON Schema validation against SETU OAS spec
Backend:       Python (FastAPI) / TypeScript (Node.js)
Frontend:      React/Vue for Smart Form rendering
```
