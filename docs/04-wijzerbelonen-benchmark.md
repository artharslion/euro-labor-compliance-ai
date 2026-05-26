# 04 — wijzerbelonen.nl Gold Standard Benchmark

## Platform Overview

**wijzerbelonen.nl** is the official reference implementation jointly developed by **ABU, NBBU, and SETU**. It serves as the practical UI for generating SETU-compliant Pay Equity messages.

- **Launched**: May 2025
- **Purpose**: Help staffing customers (inleners) generate SETU-compliant remuneration data for equal pay compliance (mandatory from Jan 1, 2026)
- **Output**: SETU-compliant JSON (per OAS schema) + PDF export
- **Target audience**: Smaller organizations without direct API integration capability
- **Tight coupling**: Form fields and SETU standard are 1:1 linked — form cannot be modified without updating the SETU standard itself

## Platform URL

- https://wijzerbelonen.nl/ — Main platform
- https://wijzerbelonen.nl/stap-1-uitvraag-arbeidsvoorwaarden/ — Standard inquiry form

## Complete Form Field → SETU JSON Mapping Table

This is the mapping that any AI OCR pipeline must reproduce. The left column represents what enterprise documents contain; the right column is the target SETU JSON output.

| # | Form Section | Input Field | SETU JSON Path | OCR Target |
|---|-------------|-------------|----------------|------------|
| 1 | Party Info | Company name | `customer.name` | Company header from contract/CAO |
| 2 | Party Info | KvK number | `customer.legalId[].value` (schemeAgencyId=KvK) | Chamber of Commerce registration |
| 3 | Party Info | OIN/RSIN | `customer.legalId[].value` (schemeAgencyId=OIN/RSIN) | Government entity ID |
| 4 | Party Info | Contact person name | `customer.personContacts[].name.formattedName` | Signatory from contract |
| 5 | Party Info | Contact email | `customer.personContacts[].communication.email[].address` | Contact info |
| 6 | Party Info | Contact phone | `customer.personContacts[].communication.phone[]` | Contact info |
| 7 | Labour | Industry (CBS SBI) | `labourAgreements.industryIdentifier[].value` | Industry classification |
| 8 | Labour | CLA name | `labourAgreements.collectiveLabourAgreement.name` | CAO document title |
| 9 | Labour | CLA ID | `labourAgreements.collectiveLabourAgreement.id.value` | CAO registration number |
| 10 | Labour | CLA effective period | `labourAgreements.collectiveLabourAgreement.effectivePeriod` | CAO validity dates |
| 11 | Labour | CLA basis | `labourAgreements.collectiveLabourAgreement.basedOn` | membership/general_binding/contract |
| 12 | Labour | Custom agreement active | `labourAgreements.customLabourAgreement` | boolean flag |
| 13 | Position | Position ID | `positionProfile[].positionId.value` | Internal job code |
| 14 | Position | Position title | `positionProfile[].positionTitle` | Job title from contract |
| 15 | Position | CLA reference title | `positionProfile[].referenceTitle` | Function name in CAO |
| 16 | Position | Work description | `positionProfile[].workDescription` | Job description |
| 17 | Remuneration | Work hours/week | `remuneration[].workDuration.valuePerWeek` | 36/38/40 from contract |
| 18 | Remuneration | Period unit | `remuneration[].interval.unitCode` | Monthly/Weekly |
| 19 | Remuneration | Hourly factor | `remuneration[].hourlyWageConversion.hourlyWageFactor` | 173.33 for 38h or derived |
| 20 | Salary Scale | Scale name | `remuneration[].salaryScale[].name` | "Scale A", "FWG 10" |
| 21 | Salary Scale | Min value | `remuneration[].salaryScale[].minValue` | Lowest salary in scale |
| 22 | Salary Scale | Max value | `remuneration[].salaryScale[].maxValue` | Highest salary in scale |
| 23 | Salary Step | Step label | `salaryScale[].salaryStep[].name` | "Step 0", "Trede 5" |
| 24 | Salary Step | Amount (EUR) | `salaryScale[].salaryStep[].value` | Exact euro amount |
| 25 | Salary Step | Is minimum wage | `salaryScale[].salaryStep[].minimumWage` | true/false |
| 26 | Salary Step | Conditions | `salaryScale[].salaryStep[].conditions[]` | Age/tenure/job thresholds |
| 27 | Position-Scale | Linked positions | `salaryScale[].positionProfileReference[]` | Which jobs use this scale |
| 28 | Increase | Performance raise | `remuneration[].individualSalaryIncrease[]` | % increase / fixed amount |
| 29 | Increase | General raise | `remuneration[].generalSalaryIncrease[]` | CAO-mandated % increase |
| 30 | Allowances | Overtime rate | `allowance[].line[].amount.value` | 150% or 200% from CAO |
| 31 | Allowances | Shift surcharge | `allowance[].line[].amount.value` | Fixed or % per hour |
| 32 | Allowances | Allowance type | `allowance[].typeCode` | HTxxx or EAxxx code |
| 33 | Allowances | Applicable period | `allowance[].period[]` | Days/hours when active |
| 34 | Holiday Pay | Holiday allowance % | `holidayAllowance[].line[].amount.value` | Typically 8% of annual salary |
| 35 | Holiday Pay | Base for calc | `holidayAllowance[].line[].amount.baseAmount` | Which salary components |
| 36 | Holiday Pay | Pay date | `holidayAllowance[].payDate` | May or monthly |
| 37 | Sick Pay | Waiting days | `sickPay[].waitingDays` | 0/1/2 from CAO |
| 38 | Sick Pay | Rate year 1 | `sickPay[].line[0].amount.value` | Usually 100% |
| 39 | Sick Pay | Rate year 2+ | `sickPay[].line[1].amount.value` | Usually 70% |
| 40 | Leave | Vacation days/year | `leave[].paidLeave[].amount.value` | 25 from CAO (min 20 statutory) |
| 41 | Leave | Public holidays count | `leave[].holidays[].amount.value` | 7 recognized holidays |
| 42 | Leave | Special leave | `leave[].specialLeave[].amount` | Bereavement/marriage days |
| 43 | Leave | Parental leave supplement | `leave[].additionalParentalLeave[]` | Employer extras |
| 44 | Leave | Leave day value (EUR) | `leave[].leaveDayValue` or per-line `leaveDayValue` | Daily rate for leave |
| 45 | IKB | IKB budget % or EUR | `individualChoiceBudget[].line[].amount` | % of salary or fixed |
| 46 | IKB | Included components | `individualChoiceBudget[].line[].ikbReference` | What can be exchanged |
| 47 | Pension | Employer contribution % | `pension[].line[].amount` | % with Employee/Employer source |
| 48 | Pension | Franchise indicator | `pension[].franchise` | Whether franchise applies |
| 49 | Pension | Employee contribution % | `pension[].line[].amount` | separate line with source=Employee |
| 50 | Training | Training budget | `sustainableEmployability[].line[].amount` | Annual EUR amount |
| 51 | Training | Budget type | `sustainableEmployability[].typeCode` | Education/CareerCoaching/etc. |
| 52 | Supplementary | Commuting | `supplementaryArrangement[]` | EA103 allowance |
| 53 | Supplementary | Home office | `supplementaryArrangement[]` | EA801 allowance |
| 54 | Supplementary | Internet | `supplementaryArrangement[]` | EA606 allowance |
| 55 | Base Def | Base salary includes | `baseDefinition[]` | Which components in base |
| 56 | Conditions | Age thresholds | `conditions[]` | ConditionType=Age |
| 57 | Conditions | Tenure thresholds | `conditions[]` | ConditionType=EmploymentDuration |
| 58 | Conditions | Job function gates | `conditions[]` | ConditionType=PositionProfile |

## Business Rules (from wijzerbelonen)

1. **Completeness**: Data must be complete for 1+ job functions. Partial submission is technically possible but not SETU-compliant.
2. **Multi-entity**: Different KvK/OIN with different CLA → separate forms required.
3. **Per doelgroep**: Different target groups (youth, experienced, leadership) may have different remuneration packages.
4. **Proportionality**: Part-time % and employment duration flags affect all calculations.
5. **IKB interactions**: Each component can be `Included`, `OnTop`, or `None` relative to IKB.
6. **Compounding**: Allowances can compound (A calculated first, used in B) or accumulate (A+B both applied).

## Benchmark Metrics for AI OCR Pipeline

| Metric | Definition | Target |
|--------|------------|--------|
| **Field Recall** | OCR fields extracted / SETU required fields | >95% |
| **Unit Code Accuracy** | Correct unitCode inference / total unitCodes | >90% |
| **Condition Accuracy** | Correct condition logic / total conditions | >85% |
| **TypeCode Match** | Correct AllowanceCode/SupplementaryCode mapping | >90% |
| **Numeric Precision** | Exact monetary value extraction | >99% |
| **Structure Fidelity** | Correct JSON nesting / total components | >95% |
