# Test Data for AI OCR → SETU Compliance Pipeline

## Dataset Overview

This directory contains synthetic test data constructed from publicly available Dutch staffing industry CAO (Collective Labour Agreement) provisions. All monetary values and rates reflect actual ABU CAO 2026-2028 data.

## Data Sources

| Source | URL |
|--------|-----|
| ABU CAO 2026-2028 (official PDF) | https://www.abu.nl/app/uploads/2026/01/CAO-voor-Uitzendkrachten-2026-2028.pdf |
| ABU Kostprijselementen 2026 | https://www.abu.nl/kennisbank/cao-voor-uitzendkrachten/kostprijselementen/ |
| Dutch Minimum Wage 2026 | https://www.rijksoverheid.nl/onderwerpen/minimumloon/bedragen-minimumloon/bedragen-minimumloon-2026 |
| StiPP Pension 2026 | https://www.stippensioen.nl/werkgever/nieuws/definitieve-cijfers-en-bedragen-2026-zijn-bekend/ |
| SETU Inquiry Pay Equity v2.0 | https://standard.setu.nl/docs/gelijkwaardige-beloning/ |

## Key Facts (ABU CAO 2026-2028)

| Parameter | Value |
|-----------|-------|
| Minimum wage (21+) Jan 2026 | €14.71/hour |
| Minimum wage (21+) Jul 2026 | €14.99/hour |
| Function group 4 (starting) | €15.31/hour |
| Function group 5 (starting) | €16.01/hour |
| Function group 6 (starting) | €16.79/hour |
| Periodic increase | 2.25%/year |
| Vacation days | >=25 days/year (per hirer's CAO from 2026) |
| Holiday allowance | 8.33% of gross salary |
| Pension total premium | 23.4% |
| Employer pension share | 15.9% |
| Employee pension share | 7.5% |
| Pension franchise (2026) | €9.24/hour |
| Max pensionable wage (2026) | €42.42/hour |
| Public holidays | 6-7 days/year |
| Working days/year (CAO) | 260 |

## Test Cases

| ID | Document Type | Language | Complexity | Description |
|----|---------------|----------|------------|-------------|
| TC-001 | Employment Contract | nl-NL | Low | Basic Phase A contract, single salary, standard benefits |
| TC-002 | Salary Sheet + Contract | nl-NL | Medium | Multi-scale salary table with age conditions, allowances |
| TC-003 | Cross-Border | en/nl/pl | High | Polish worker posted to Netherlands, multi-CAO comparison |

## Directory Structure

```
test/data/
├── README.md
├── documents/           # Synthetic input documents (what AI OCR reads)
│   ├── tc-001-contract.txt
│   ├── tc-002-salary-sheet.txt
│   └── tc-003-cross-border.txt
├── ground-truth/        # Expected SETU JSON outputs
│   ├── tc-001-setu-output.json
│   ├── tc-002-setu-output.json
│   └── tc-003-setu-output.json
└── schemas/             # JSON Schema for validation
    └── inquiry-pay-equity-v2.schema.json
```
