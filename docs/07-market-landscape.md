# 07 — Market Landscape & Competitive Analysis

## Target Market

### Primary: Dutch Staffing Industry

- **Thousands** of uitzendbureaus (staffing agencies) in the Netherlands
- **All** must comply with SETU standards for government (semi-)public contracts
- **Equal pay compliance** (Inquiry Pay Equity) is mandatory from Jan 1, 2026
- ABU CAO covers ~85% of temp workers (Randstad, Adecco, Manpower, Tempo-Team)
- NBBU CAO covers smaller/medium agencies

### Secondary: European Cross-Border Staffing

- EU Posted Workers Directive requires compliance with host country minimum standards
- Cross-border staffing is growing rapidly (especially Poland→Netherlands, Romania→Germany)
- Each country has its own labor standards; SETU-like compliance will spread

### Tertiary: Enterprise Vendor Compliance

- Large enterprises managing multiple staffing suppliers (AVP/MAVP programs)
- Need to collect, validate, and monitor supplier compliance data at scale
- Current process: manual spreadsheets, email, PDF exchange

## Regulatory Tailwinds

| Regulation | Impact | Timeline |
|------------|--------|----------|
| SETU "Pas-toe-of-leg-uit" | Mandatory for Dutch public sector | Active |
| EU Pay Transparency Directive (2023/970) | Requires objective pay criteria | Transposition by June 2026 |
| EU AI Act (2024/1689) | Recruitment AI = high-risk | Aug 2026 enforcement |
| Equal Pay Mandate (NL) | Flex workers = equal pay to permanent | Jan 1, 2026 |
| ViDA (VAT in Digital Age) | E-invoicing mandate | 2028-2030 |
| Wet meer zekerheid flexwerkers | Strengthened flex worker rights | 2026 |

## Competitive Landscape

### Direct Competitors

| Company | Product | Focus | Differentiator |
|---------|---------|-------|----------------|
| **wijzerbelonen.nl** | Free web form | SETU Pay Equity manual entry | Official ABU/NBBU/SETU platform; free but manual |
| **RGF Staffing + Klippa/Doxis** | AI OCR onboarding | Employee document verification | 81% reduction in onboarding time; limited to identity docs |
| **Assembly Industries** | Vendor compliance automation | US staffing compliance | AI-driven document collection + credential tracking |
| **DynaTech** | Vendor onboarding agent | Microsoft Dynamics 365 ecosystem | Azure AI + Teams integration |

### Adjacent Competitors (potential entrants)

| Company | Product | Overlap |
|---------|---------|---------|
| **MOVA Lab** | AI business process contracts | OCR + compliance audit trails; EU AI Act ready |
| **Novantix** | Pay transparency platform | ESCO taxonomy mapping; job architecture calibration |
| **Nexgile** | Cross-border compliance | RAG-powered regulatory analysis; multi-jurisdiction |
| **Beeline** | Enterprise VMS | Vendor compliance management; global scale |
| **Solid Online / zvoove** | SETU integration software | Direct SETU API implementation; staffing ERP |

### White Space / Competitive Advantage

| Gap in Market | Our Opportunity |
|---------------|-----------------|
| No end-to-end AI OCR → SETU mapping solution exists | First-mover in automated compliance generation |
| wijzerbelonen.nl is manual-only | AI reduces hours of manual entry to minutes |
| Existing OCR tools don't understand SETU semantics | Domain-specific mapping engine is the moat |
| Cross-border compliance is fragmented | Multi-jurisdiction MAVP logic is defensible IP |
| Vendor compliance is spreadsheet-driven | Automated document ingestion → compliance scoring |

## Business Model Options

| Model | Description | Revenue Driver |
|-------|-------------|----------------|
| **SaaS** | Per-document or per-worker pricing | Volume: # of worker placements processed |
| **Enterprise** | Annual license for staffing agencies | Contract value: integration + support |
| **API-first** | White-label OCR→SETU mapping API | Usage-based: per API call |
| **Marketplace** | Compliance document exchange platform | Transaction fee: per compliance packet |

## Go-to-Market Strategy

### Phase 1: Dutch Staffing (0-12 months)
- Target: Top 20 Dutch staffing agencies (ABU members)
- Channel: SETU/ABU partnership, staffing industry events
- MVP: AI OCR → SETU Pay Equity JSON generation
- Pricing: Per-document processing

### Phase 2: Enterprise Vendor Compliance (12-24 months)
- Target: Large enterprises with AVP/MAVP programs
- Channel: Procurement/HR technology partners
- Feature: Multi-supplier dashboard, compliance scoring, audit trails

### Phase 3: European Expansion (24-36 months)
- Target: Cross-border staffing providers (Poland→Netherlands, Romania→Germany corridors)
- Channel: EU Posted Worker compliance networks
- Feature: Multi-jurisdiction MAVP rules engine, multi-language document processing

## Key Risks

| Risk | Mitigation |
|------|------------|
| SETU standards change rapidly (v2.0 just released) | Build modular schema adapter; track SETU change log |
| LLM hallucination in legal/financial context | Human-in-the-loop for all monetary values; confidence thresholds |
| EU AI Act classifies this as high-risk | Build audit trails, human oversight, transparency from day one |
| Dutch-language OCR accuracy | Fine-tune LayoutLM on Dutch staffing documents |
| wijzerbelonen.nl is free — hard to compete on price | Compete on speed (AI) and integration (API), not price |
