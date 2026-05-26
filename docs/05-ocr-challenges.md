# 05 — AI OCR Technical Challenges for SETU Compliance

Mapping enterprise documents to SETU standards is not a generic OCR problem. The specific challenges are:

## Challenge 1: Borderless Table Parsing

Enterprise salary tables and CAO documents rarely use clean HTML/CSS tables. They appear as:

```
| 职级    | <18岁  | 18-20岁 | 21岁+  |
| Step 0  | €6.50  | €8.20   | €11.50 |
| Step 5  | €8.00  | €10.50  | €14.20 |
```

The AI must:

- **Detect implicit table structure** without visible cell borders (LayoutLM / Table Transformer)
- **Extract row headers** (Step numbers) and **column headers** (age ranges) as separate structured fields
- **Infer conditions** from column headers: `<18岁` → `{conditionType: "Age", operator: "lt", age: 18}`
- **Produce correct SETU JSON**: each cell → a `SalaryScaleStep` with value + conditions

### Solution Stack

- LayoutLMv3 / Microsoft Table Transformer for table detection
- Custom post-processing to convert implicit headers → explicit Condition objects
- Confidence scoring: flag cells where table structure detection is ambiguous for human review

## Challenge 2: Unit Code Inference

Enterprise documents use natural language, not SETU codes:

| Document Text | Must Infer |
|---------------|------------|
| "交通补助每公里 0.23 欧元" | `amount.unitCode: "Euro"`, `interval.unitCode: "Kilometer"` |
| "加班费为时薪的 150%" | `amount.unitCode: "Percentage"`, `baseAmount.unitCode: "HourlyRate"` |
| "每月固定津贴 €50" | `amount.unitCode: "Euro"`, `interval.unitCode: "Month"`, `baseAmount.unitCode: "Fixed"` |
| "每年 25 天带薪假" | `amount.value: 25`, `amount.unitCode: "Day"`, `interval.unitCode: "Year"` |

### Solution Stack

- LLM-based reasoning (GPT-4o / Claude) — provide the full enum lists as context
- Few-shot prompting with examples of document text → SETU code mapping
- Fallback: if confidence < threshold, flag for human review with top-3 suggestions

## Challenge 3: Condition Logic Extraction

Conditions in documents are expressed narratively:

| Document Text | Must Generate |
|---------------|---------------|
| "21 岁以上且担任 Team Lead 才适用" | `AllOf(Age >= 21, PositionProfile in ["TeamLead"])` |
| "病假第一年全薪，之后 70%" | Two `sickPay.line[]` entries with `Occurrence(Relative, SickLeave, P0D)` and `Occurrence(Relative, SickLeave, P1Y)` |
| "年满 55 岁且工龄超过 10 年可申请" | `AllOf(Age >= 55, EmploymentDuration >= P10Y)` |
| "周日或夜间工作适用额外津贴" | `AnyOf(weekday=Sunday, timePeriod={night hours})` |

### Solution Stack

- LLM chain-of-thought prompting: "Parse the following condition text. Identify the condition type(s), operators, and values. Output as structured JSON."
- Supported condition types in prompt: Age, EmploymentDuration, Occurrence, PositionProfile, SalaryScale, Text (fallback)
- For complex nested conditions, use recursive parsing: identify AND/OR keywords → build AllOf/AnyOf tree

## Challenge 4: Allowance TypeCode Classification

The hardest mapping problem. There are 78 AllowanceCodes across EA and HT series. Enterprise documents use vernacular names:

| Document Calls It | Must Map To |
|-------------------|-------------|
| "周末加班费" (weekend OT pay) | HT321 (Weekend surcharge) or HT101 (OT surcharge Sat) |
| "夜班补助" (night shift allowance) | HT340 (Night shift surcharge) |
| "交通补贴" (transport allowance) | EA103 (Car commuting) or EA101 (Public transport) |
| "居家办公津贴" (WFH allowance) | EA801 (Home office allowance) |
| "轮班津贴" (shift allowance) | HT300 (Shift work surcharge) |

### Solution Stack

- RAG (Retrieval Augmented Generation): index SETU code descriptions + Dutch synonyms
- LLM classification with full AllowanceCode list as system prompt
- Confidence scoring: return top-3 matches with scores, let user confirm
- Build a continuously learning mapping table from user corrections

## Challenge 5: Origin Attribution

Documents don't explicitly state whether each clause comes from a CAO or is a custom company policy:

```yaml
origin:
  type: CollectiveLabourAgreement | CollectiveLabourAgreementExtended 
      | CustomLabourAgreement | Unknown
```

### Solution

- If a clause appears identically in the referenced CAO document → `CollectiveLabourAgreement`
- If the CAO has been AVP-declared → `CollectiveLabourAgreementExtended`
- If the clause deviates from or supplements the CAO → `CustomLabourAgreement`
- If uncertain → `Unknown` (safe default, flag for review)

## Challenge 6: Multi-Document Aggregation

A single SETU message may require data from 5+ enterprise documents:

1. Employment contract (work hours, salary, position)
2. CAO document (allowance rates, leave entitlements, pension rules)
3. Salary administration sheet (actual salary scales and steps)
4. Expense policy (commuting, home office reimbursements)
5. Pension plan document (contribution rates, franchise rules)

The AI must:
- **Cross-reference** values across documents (does contract salary match CAO scale?)
- **Detect conflicts** (contract says 25 days leave, CAO says 23 — which applies?)
- **Merge** data from all sources into a single coherent SETU message

## Challenge 7: ISO 8601 Date/Duration Parsing

SETU uses ISO 8601 extensively for:
- Dates: `2026-01-15`
- Date-times: `2026-01-15T10:30:00Z`
- Durations: `P6M` (6 months), `P1Y` (1 year), `P0D` (0 days from event)
- Recurring intervals: `R/2026-5/P1Y` (every year in May)

Enterprise documents use various date formats: `15-01-2026`, `January 15, 2026`, `15 Jan 2026`, `15/1/26`.

### Solution

- Normalize all dates to ISO 8601 at extraction time
- Infer durations from narrative text: "after 6 months" → `P6M`
- Use LLM to convert recurring patterns: "每年5月" → `R/2026-5/P1Y`

## Recommended AI Tech Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| **OCR** | Azure Document Intelligence / LayoutLMv3 | Table detection, field extraction |
| **Semantic Mapping** | GPT-4o / Claude 3.5 Sonnet | Natural language → SETU code mapping |
| **RAG** | Vector DB (Pinecone/Weaviate) + SETU docs | Code lookup, AllowanceCode classification |
| **Validation** | JSON Schema (ajv) + SETU OAS spec | Structural validation of generated JSON |
| **Orchestration** | LangChain / custom pipeline | Multi-step document processing |
| **Review UI** | React/Vue | Human-in-the-loop for low-confidence fields |
