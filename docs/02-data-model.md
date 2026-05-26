# 02 — SETU Inquiry Pay Equity v2.0 — Complete Data Model

> Published: March 11, 2026 | Source: https://standard.setu.nl/docs/gelijkwaardige-beloning/

## Top-Level Message Structure

```yaml
InquiryPayEquity:
  required: [documentId, effectivePeriod, customer, remuneration]
  properties:
    documentId              # Identifier {value, schemeAgencyId: Customer|Supplier}
    versionId               # {value} — customer-specific version tracking
    issued                  # datetime — document issue timestamp
    effectivePeriod         # {validFrom, validTo} — regulation validity period
    customer                # Party — staffing customer/hirer info
    baseDefinition          # BaseDefinition[] — what's included in base salary calcs
    labourAgreements        # LabourAgreements — sector/CLA info
    positionProfile         # PositionProfile[] — job function profiles
    remuneration *          # RemunerationPackage[] (min 1) — salary packages
    allowance               # AllowanceArrangement[] — overtime/shift/commute etc.
    holidayAllowance        # HolidayAllowanceArrangement[] — holiday pay
    sickPay                 # SickPayArrangement[] — sick leave pay
    leave                   # LeaveArrangement[] — vacation/holidays/parental
    individualChoiceBudget  # IndividualChoiceBudgetArrangement[] — IKB
    pension                 # PensionArrangement[] — pension contributions
    sustainableEmployability # SustainableEmployabilityArrangement[] — training
    supplementaryArrangement # SupplementaryArrangement[] — additional schemes
    otherArrangement        # OtherArrangement[] — catch-all
```

\* = required

## The "Pay Equity Structure" (Shared Pattern)

Nearly all components (allowance, sickPay, leave, holidayAllowance, pension, IKB, etc.) share this common structure:

```json
{
  "id": { "value": "uuid", "schemeAgencyId": "Customer" },
  "origin": {
    "type": "CollectiveLabourAgreement"  // | CollectiveLabourAgreementExtended 
                                         // | CustomLabourAgreement | Unknown
  },
  "name": "Overtime Compensation",
  "typeCode": "HT320",                   // SETU standardized code
  "effectivePeriod": {
    "validFrom": "2026-01-01",
    "validTo": "2027-12-31"              // end-inclusive
  },
  "period": [{                           // (Allowance only) when applicable
    "datePeriod": [{"start": "2026-07-26", "end": "2026-08-11"}],
    "timePeriod": {"start": "18:00:00", "end": "23:59:59"},
    "weekday": ["Saturday", "Sunday"]
  }],
  "line": [{                             // calculation rules (can be multiple)
    "lineId": {"value": "OT-1"},
    "amount": {
      "value": 150,                      // decimal number
      "minValue": null,                  // optional floor
      "maxValue": null,                  // optional ceiling
      "unitCode": "Percentage",          // Percentage|Euro|Hour|Day|SalaryStep
      "baseAmount": {
        "unitCode": "HourlyRate",        // HourlyRate|MonthlyRate|DailyRate|
                                         // FourWeeklyRate|YearlyRate|Fixed|Expense
        "baseType": "GrossSalary",       // from BaseDefinitionCode
        "value": 3201,                   // optional — e.g., actual monthly rate
        "minValue": null,
        "maxValue": null
      },
      "proportional": {
        "partTimePercentage": true,      // prorate by part-time %
        "employmentDuration": false,     // prorate by tenure
        "description": null
      }
    },
    "interval": {
      "value": 1,                        // 0.25 = 15 min, 1 = 1 hour
      "unitCode": "Hour"
    },
    "conditions": [...],                 // Condition[] — eligibility logic
    "contributionSource": "Employer",    // Employee|Employer
    "ikbReference": [...]                // link to Individual Choice Budget
  }],
  "payDate": {...},                      // Occurrence — when benefit is paid
  "reference": [...],                    // allowance-to-allowance relations
  "description": "..."                   // free-text clarifications
}
```

## Component-Specific Structures

### RemunerationPackage (Required, min 1)

```yaml
RemunerationPackage:
  required: [origin, workDuration]
  properties:
    origin                  # LabourAgreementReference — CAO/custom source
    effectivePeriod         # validity window
    workDuration:           # standard work hours
      amount: {value: 38, unitCode: "Hour"}
      interval: {value: 1, unitCode: "Week"}
      valuePerWeek: 38
    interval:               # salary period (e.g., Monthly)
      value: 1, unitCode: "Month"
    hourlyWageConversion:   # salary → hourly rate conversion
      hourlyWageFactor: 173.33        # monthly / factor = hourly
      hourlyWagePercentage: 0.607     # or percentage method
    salaryScale[]:          # pay scale tables
      - name: "Scale A"
        minValue: 2200
        maxValue: 4200
        currency: "EUR"
        salaryStep[]:
          - name: "Step 0"
            value: 2200.00
            minimumWage: false
            conditions:                 # e.g., Age=18, EmploymentDuration=0-6mo
              - {conditionType: "Age", operator: "gte", age: 18}
              - {conditionType: "Age", operator: "lt", age: 19}
        careerLevel: {indicator: true, description: "..."}
        positionProfileReference[]:
          - positionId: "TeamLead"
            startSalaryStep: "Step 3"
            description: "..."
    individualSalaryIncrease[]: # performance-based raises
    generalSalaryIncrease[]:    # across-the-board raises
    conditions[]                # overall applicability
```

### LeaveArrangement

```yaml
LeaveArrangement:
  required: [name, origin]
  properties:
    paidLeave[]:              # ArrangementLineLeave — vacation days
    workingHoursReduction[]:  # reduced work hours schemes
    holidays[]:               # public holidays
    specialLeave[]:           # bereavement, marriage, relocation
    additionalParentalLeave[]:# employer supplement beyond statutory
    mandatoryLeaveAllocation: # forced leave usage policy
```

ArrangementLineLeave extends the base pattern with `leaveDayValue` — the financial equivalent of one leave day:
```json
{ "value": 133.0, "unitCode": "Euro", "baseAmount": { "unitCode": "Fixed" } }
```

### SickPayArrangement

```yaml
SickPayArrangement:
  required: [name, origin]
  properties:
    waitingDays:             # waiting period before eligibility
      value: 1, unitCode: "Day"
    line[]:                  # tiered rates (e.g., 100% first year, 70% thereafter)
      - conditions:
          - {conditionType: "Occurrence", occurrence: {
              occurrenceType: "Relative", event: "SickLeave", offset: "P0D"}}
        amount: {value: 100, unitCode: "Percentage", ...}
      - conditions:
          - {conditionType: "Occurrence", occurrence: {
              occurrenceType: "Relative", event: "SickLeave", offset: "P1Y"}}
        amount: {value: 70, unitCode: "Percentage", ...}
```

## Condition System (9 Types)

```yaml
Condition (discriminator: conditionType):
  types:
    Age:
      conditionType: "Age"
      operator: eq|neq|gt|gte|lt|lte|in
      age: number                    # age in years
    
    EmploymentDuration:
      conditionType: "EmploymentDuration"
      operator: eq|neq|gt|gte|lt|lte
      duration: "P6M"               # ISO 8601 duration
      referenceDateType: SeniorityDate|HireDate|ContractStartDate|AssignmentStartDate
    
    Occurrence:
      conditionType: "Occurrence"
      occurrence: (oneOf)
        - Single: {occurrenceType: "Single", date: "2026-12-15"}
        - Recurring: {occurrenceType: "Recurring", recurringInterval: "R/2026-5/P1Y"}
        - Relative: {occurrenceType: "Relative", event: "SickLeave"|"HireDate"|..., offset: "P1Y"}
    
    PositionProfile:
      conditionType: "PositionProfile"
      operator: "in"                # only "in" supported
      positionProfileIds: ["TeamLead", "Manager"]
    
    SalaryScale:
      conditionType: "SalaryScale"
      operator: eq|neq|gt|gte|lt|lte
      salaryScale: "Scale A"
      step: "Step 5"                # optional
    
    Text:
      conditionType: "Text"
      description: "Based on individual performance review results."
    
    AllOf:
      conditionType: "AllOf"
      conditions: [Condition, ...]  # min 1 — all must be met
    
    AnyOf:
      conditionType: "AnyOf"
      conditions: [Condition, ...]  # min 1 — any must be met
    
    Not:
      conditionType: "Not"
      condition: Condition          # negation
```

## Party Structure

```yaml
Party:
  required: [legalId, personContacts]
  properties:
    id[]:                     # Identifier[] — up to 2 identifiers
      - {value: "12345678", schemeAgencyId: "Customer"}
    name: "Acme Corporation"
    legalId[]:                # government-assigned IDs
      - value: "12345678"
        schemeAgencyId: KvK|OIN|RSIN
    personContacts[]:         # min 1 — authorized contacts
      - name: {formattedName: "Jan de Vries"}
        communication:
          phone: [{...}]
          email: [{address: "j.devries@acme.nl", useCode: "Business"}]
        roleCode: "HR Manager"
        positionTitle: "Head of HR Operations"
```

## Labour Agreements

```yaml
LabourAgreements:
  properties:
    industryIdentifier[]:     # CBS - SBI 2008 industry codes
      - {value: "78"}         # e.g., 78 = Employment activities
    collectiveLabourAgreement:
      name: "CAO voor Uitzendkrachten"
      id: {value: "ABU-CAO-2025"}
      effectivePeriod: {validFrom: "2025-01-01", validTo: "2027-12-31"}
      basedOn: "membership"   # membership|general_binding|contract
    customLabourAgreement: false
```

## BaseDefinition

```yaml
BaseDefinition:
  required: [baseType, remunerationIndicator, holidayAllowanceIndicator, 
             paidLeaveDayIndicator, allAllowancesIndicator]
  properties:
    baseType: GrossSalary|UsualWage|BaseWage|SocialInsuranceWage|
              HolidayAllowance|Pension|SickPay|13thMonth
    remunerationIndicator: boolean
    holidayAllowanceIndicator: boolean
    paidLeaveDayIndicator: boolean
    allAllowancesIndicator: boolean
    allowances[]:             # specific allowance refs (if not all)
      - {typeCode: "HT320"}
    referenceDate: Occurrence # optional reference date for calculation
```

## API Endpoints (REST)

```
PUT    /inquiry-pay-equity/{id}     # Submit/update (full replacement)
GET    /inquiry-pay-equity/{id}     # Retrieve
DELETE /inquiry-pay-equity/{id}     # Remove

Headers: setuVersionId: "2.0", Content-Type: application/json
```

**Data exchange rules:**
- Must be complete for one or more job functions, or all job functions
- Partial delivery is possible but not considered "complete" per SETU standard
- Two scenarios: (1) Customer → Supplier direct, (2) Via CAO service provider
- PUT is idempotent — repeated submission fully replaces previous data

## Full Schema Source

The complete OAS 3.1.0 YAML specification is available at:
`https://standard.setu.nl/docs/redocusaurus/gelijkwaardige-beloning-api.yaml`

GitHub repository with schemas and examples:
`https://github.com/setu-standards/xml-specifications/tree/main/setu-gelijkwaardige-beloning/v2.0/`
