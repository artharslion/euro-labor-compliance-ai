# 06 — Pipeline Architecture (5-Layer Design)

```
┌─────────────────────────────────────────────────────────────────────┐
│  L1: Multi-Format Document Ingestion                                 │
│  PDF / Scanned Images / DOCX / Excel → Preprocessing                │
├─────────────────────────────────────────────────────────────────────┤
│  L2: AI OCR + Layout Understanding                                   │
│  LayoutLM / Document AI → Borderless Table Parsing + Field Extract  │
├─────────────────────────────────────────────────────────────────────┤
│  L3: Semantic Mapping Engine                                         │
│  Extracted Fields → LLM Reasoning → SETU Standard Field Mapping     │
├─────────────────────────────────────────────────────────────────────┤
│  L4: Gap Analysis + Smart Form Generation                            │
│  Mapped Data vs SETU Required Fields → Missing Field Highlighting    │
├─────────────────────────────────────────────────────────────────────┤
│  L5: SETU-Compliant JSON Export + Validation                         │
│  Generate JSON → Schema Validation → Benchmark Scoring              │
└─────────────────────────────────────────────────────────────────────┘
```

## L1: Document Ingestion

### Input Types

| Format | Processing | Key Challenges |
|--------|-----------|----------------|
| PDF (digital) | Direct text extraction | Multi-column layouts may confuse extractors |
| PDF (scanned) | OCR via Tesseract / Azure DI | Image quality, skew, noise |
| DOCX | Parse XML structure directly | Embedded tables may lose borders |
| Excel (.xlsx) | Parse cell grid → structured data | Merged cells, multi-sheet workbooks |
| Images (PNG/JPG) | OCR pipeline | Resolution, lighting, perspective |

### Preprocessing Steps

1. **Language detection**: Identify document language (Dutch, English, German, Polish)
2. **Page classification**: Separate cover pages, tables, narrative text, appendices
3. **Deskew + denoise**: For scanned images
4. **Table region detection**: Identify candidate table areas (keyword: salary, toeslag, verlof)
5. **Document grouping**: Cluster related documents (contract + CAO + salary sheet)

### Output

```
[
  {
    "documentId": "uuid",
    "sourceFile": "contract_2026.pdf",
    "type": "employment_contract | cao_document | salary_sheet | expense_policy",
    "language": "nl",
    "pages": 12,
    "extractedRegions": [
      {"type": "table", "bbox": [x1,y1,x2,y2], "page": 3},
      {"type": "text_block", "bbox": [...], "page": 1}
    ]
  }
]
```

## L2: AI OCR + Layout Understanding

### Technology Options

| Approach | Pros | Cons |
|----------|------|------|
| **Azure Document Intelligence** | Pre-built table extraction, field detection, high accuracy for forms | Cost per page, vendor lock-in |
| **LayoutLMv3** (self-hosted) | Full control, fine-tunable on Dutch documents | Requires training data, GPU infra |
| **Tesseract + OpenCV** | Free, fast | Poor on complex layouts, no semantic understanding |
| **Amazon Textract** | Good table extraction, form key-value pairs | AWS lock-in, limited Dutch support |
| **Google Document AI** | Strong for forms, multi-language | GCP lock-in, cost |

### Recommended: Hybrid Approach

- **Azure Document Intelligence** for initial table detection and structured field extraction
- **LayoutLMv3 fine-tuned** on Dutch staffing documents for domain-specific table understanding
- **Fallback**: Tesseract for simple text blocks

### Key Extraction Targets

1. **Tables**: Salary scales, allowance matrices, leave entitlements
2. **Key-Value Pairs**: Company name, KvK number, CLA reference, contact info
3. **Numeric Values**: Salary amounts, percentages, hour counts
4. **Date Ranges**: Contract periods, CAO validity, age thresholds
5. **Conditional Text**: "applicable to...", "if...", "when..."

### Output (Structured Extraction)

```json
{
  "documentId": "uuid",
  "extracted": {
    "party": {
      "name": "Acme BV",
      "kvkNumber": "12345678",
      "contactPerson": "Jan de Vries",
      "contactEmail": "j.devries@acme.nl"
    },
    "remuneration": {
      "workHoursPerWeek": 38,
      "salaryScales": [
        {
          "name": "Scale A",
          "steps": [
            {"step": "0", "value": 2200.00, "conditions": [{"age": "18-19"}]},
            {"step": "5", "value": 2800.00, "conditions": [{"age": "21+"}]}
          ]
        }
      ]
    },
    "allowances": [
      {
        "name": "Overtime surcharge",
        "rate": "150%",
        "base": "hourly rate",
        "narrativeCondition": "for hours beyond 38 per week"
      }
    ],
    "leave": {
      "vacationDays": 25,
      "unit": "per year"
    }
  },
  "confidenceScores": {
    "party.name": 0.98,
    "remuneration.salaryScales": 0.72 // low confidence → flag for review
  }
}
```

## L3: Semantic Mapping Engine

This is where LLM reasoning converts extracted raw data → SETU-compliant structured data.

### Sub-Steps

#### 3a. Field Name Normalization

```
"交通补助" → {"candidates": ["EA103", "EA101", "EA100"], "topMatch": "EA103", "confidence": 0.85}
"加班费" → {"candidates": ["HT100", "HT101"], "topMatch": "HT100", "confidence": 0.92}
```

#### 3b. Unit Code Inference

```
"时薪的 150%" → {unitCode: "Percentage", baseUnitCode: "HourlyRate"}
"每年 25 天" → {value: 25, unitCode: "Day", intervalUnitCode: "Year"}
"固定 €50/月" → {value: 50, unitCode: "Euro", baseUnitCode: "Fixed", intervalUnitCode: "Month"}
```

#### 3c. Condition Logic Generation

```
Input: "21岁以上且担任Team Lead"
Output: {
  conditionType: "AllOf",
  conditions: [
    {conditionType: "Age", operator: "gte", age: 21},
    {conditionType: "PositionProfile", operator: "in", positionProfileIds: ["TeamLead"]}
  ]
}
```

#### 3d. Origin Attribution

```
Input: clause text + referenced CAO document
Output: {
  origin: {
    type: "CollectiveLabourAgreement" | "CustomLabourAgreement" | "Unknown"
  }
}
```

### LLM Prompt Architecture

```python
SYSTEM_PROMPT = """
You are a SETU compliance data mapper. Map extracted field names, values, and 
conditions to the SETU Inquiry Pay Equity v2.0 JSON schema.

Available AllowanceCodes: {allowance_codes}
Available Unit Codes: {unit_codes}
Condition types: Age, EmploymentDuration, Occurrence, PositionProfile, 
                 SalaryScale, Text, AllOf, AnyOf, Not

For each mapping, output:
1. The SETU JSON path
2. The mapped value with correct unit codes and condition types
3. A confidence score (0-1)
4. If confidence < 0.8, suggest alternative interpretations

Rules:
- Never guess monetary values — flag as "needs_review" if unclear
- Dutch "toeslag" → AllowanceCode starting with HT
- Dutch "vergoeding" → AllowanceCode starting with EA
- Percentages must use unitCode: "Percentage" with explicit baseAmount
- Age conditions must use conditionType: "Age" with numeric age, not text
"""
```

### Output

```json
{
  "documentId": "uuid",
  "mappedData": {
    "remuneration": [
      {
        "origin": {"type": "CollectiveLabourAgreement"},
        "workDuration": {"amount": {"value": 38, "unitCode": "Hour"}, "interval": {"value": 1, "unitCode": "Week"}, "valuePerWeek": 38},
        "salaryScale": [
          {
            "name": "Scale A",
            "currency": "EUR",
            "salaryStep": [
              {"name": "Step 0", "value": 2200.00, "conditions": [{"conditionType": "Age", "operator": "gte", "age": 18}, {"conditionType": "Age", "operator": "lt", "age": 19}]},
              {"name": "Step 5", "value": 2800.00, "conditions": [{"conditionType": "Age", "operator": "gte", "age": 21}]}
            ]
          }
        ]
      }
    ],
    "allowance": [
      {
        "name": "Overtime Compensation",
        "origin": {"type": "CollectiveLabourAgreement"},
        "typeCode": "HT100",
        "line": [{"amount": {"value": 150, "unitCode": "Percentage", "baseAmount": {"unitCode": "HourlyRate", "baseType": "GrossSalary"}}, "interval": {"value": 1, "unitCode": "Hour"}}]
      }
    ]
  },
  "mappingConfidence": {
    "overall": 0.82,
    "byField": {
      "remuneration.salaryScale": 0.72,
      "allowance.typeCode": 0.85
    }
  },
  "flags": [
    {"field": "remuneration.salaryScale", "type": "low_confidence", "alternatives": ["Scale A (document)", "Scale 1 (alternative)"]},
    {"field": "allowance.commuting", "type": "missing", "message": "No commuting allowance found in documents"}
  ]
}
```

## L4: Gap Analysis + Smart Form Generation

### Gap Detection Logic

```
SETU Required Fields - Extracted & Mapped Fields = Missing Fields
```

#### Required Field Checklist

| Required | Extracted | Gap |
|----------|-----------|-----|
| documentId | ✅ auto-generated | — |
| effectivePeriod | ❌ missing | **Prompt user** "When is this pay equity data valid from/to?" |
| customer | ✅ | — |
| customer.legalId | ✅ (KvK from doc) | — |
| remuneration[] | ✅ (min 1 package) | — |
| remuneration[].origin | ✅ | — |
| remuneration[].workDuration | ✅ | — |

#### Optional-but-Expected Field Checklist

| Expected | Extracted | Gap |
|----------|-----------|-----|
| labourAgreements | ❌ | **Prompt user** "Which CAO applies? Is it AVP?" |
| positionProfile[] | ❌ | **Prompt user** "What job functions does this cover?" |
| holidayAllowance | ✅ (8% detected) | — |
| pension | ❌ | **Prompt user** "What is the pension contribution rate?" |
| leave | ✅ (25 days detected) | — |

### Smart Form Generation

For each missing field, the form provides:

1. **SETU field name + path** (e.g., `labourAgreements.collectiveLabourAgreement.name`)
2. **Contextual hint** (e.g., "We detected references to CAO ABU. Is this correct?")
3. **Input type**: dropdown (for enums), date picker (for dates), number field (for amounts), text area (for descriptions)
4. **Validation rules**: required, min/max, enum values, ISO 8601 format
5. **AI-suggested value** (if confidence > threshold with alternatives)

### Smart Form Example

```
┌────────────────────────────────────────────────────────────┐
│  Missing Field: labourAgreements.collectiveLabourAgreement │
│                                                            │
│  We detected references to a CLA in your documents.        │
│  Please confirm or select the applicable CLA:              │
│                                                            │
│  ┌──────────────────────────────────────────────────┐     │
│  │ CAO voor Uitzendkrachten (ABU)  ← AI Suggested   │     │
│  └──────────────────────────────────────────────────┘     │
│                                                            │
│  Other options:                                            │
│  ○ CAO voor Uitzendkrachten (NBBU)                        │
│  ○ Custom employment conditions                           │
│  ○ Unknown                                                │
│                                                            │
│  Is this CLA declared AVP (algemeen verbindend verklaard)?│
│  ● Yes  ○ No  ○ Not sure                                  │
│                                                            │
│  [Skip]  [Save & Continue]                                │
└────────────────────────────────────────────────────────────┘
```

## L5: SETU-Compliant Export + Validation

### JSON Generation

Assemble all mapped + user-filled fields into the top-level `InquiryPayEquity` structure per OAS schema.

### Validation Pipeline

```python
# 1. JSON Schema validation
validate(json_output, setu_inquiry_pay_equity_schema)

# 2. Business rule validation
assert len(json_output["remuneration"]) >= 1, "At least one remuneration package required"
for allowance in json_output.get("allowance", []):
    assert allowance["typeCode"] in ALLOWANCE_CODES, f"Invalid typeCode: {allowance['typeCode']}"

# 3. Cross-reference validation
for scale in json_output["remuneration"][0]["salaryScale"]:
    for step in scale["salaryStep"]:
        if step.get("conditions"):
            validate_condition_structure(step["conditions"])

# 4. Benchmark scoring
score = benchmark.compare(json_output, wijzerbelonen_gold_standard)
```

### Export Formats

| Format | Use Case |
|--------|----------|
| JSON (SETU OAS) | Direct API submission to staffing supplier |
| PDF (SETU-compliant) | Email/manual exchange for smaller parties |
| REST API (`PUT /inquiry-pay-equity/{id}`) | Direct integration with supplier backoffice |

### Benchmark Scoring

```
┌─────────────────────────────────────────┐
│  Compliance Score: 87%                  │
│                                         │
│  Field Recall:     94%  ████████░░      │
│  Unit Code Acc:    89%  ████████░░      │
│  Condition Acc:    82%  ████████░░      │
│  TypeCode Match:   91%  █████████░      │
│  Numeric Precision: 99%  ██████████     │
│  Structure Fidelity: 95%  █████████░    │
│                                         │
│  ⚠ 3 fields flagged for review          │
│  ❌ 1 required field missing             │
│                                         │
│  [Review Flags]  [Export JSON]  [Submit]│
└─────────────────────────────────────────┘
```

## Data Flow Summary

```
Enterprise Docs (PDF/Word/Excel/Images)
        │
        ▼
    [L1: Ingestion] ──► Document metadata, regions
        │
        ▼
    [L2: AI OCR] ──► Structured extracted fields + confidence scores
        │
        ▼
    [L3: LLM Mapping] ──► SETU-mapped fields + mapping confidence
        │
        ▼
    [L4: Gap Analysis] ──► Missing field list + Smart Form
        │                         │
        │                    [Human Review]
        │                         │
        ▼                         ▼
    [L5: Export] ──► Validated SETU JSON → API / PDF
```
