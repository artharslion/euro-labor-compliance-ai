# 03 — Complete Code Lists (Enumeration Dictionaries)

These are the standardized codes used across all SETU messages. For AI OCR mapping, this is the "translation dictionary" — extracted field values must be mapped to these codes.

## AllowanceCode (78 codes)

Allowances are the most complex code domain. Each `EAxxx` and `HTxxx` code represents a specific type of allowance or surcharge.

### EA-Series: Expenses & Compensations

| Code | Description (Dutch) | Description (English) |
|------|---------------------|-----------------------|
| EA100 | Reiskosten woon-werkverkeer | Commuting costs |
| EA101 | Reiskosten woon-werk OV | Public transport commuting |
| EA102 | Reiskosten woon-werk fiets | Bicycle commuting |
| EA103 | Reiskosten woon-werk auto | Car commuting (per km) |
| EA105 | Reiskosten dienstreizen | Business travel expenses |
| EA107 | Verblijfkosten | Accommodation costs |
| EA108 | Verhuiskosten | Relocation costs |
| EA109 | Parkeerkosten | Parking costs |
| EA110 | Tolgelden | Toll fees |
| EA111 | Reiskosten overig | Other travel expenses |
| EA112 | Representatiekosten | Representation expenses |
| EA113 | Congreskosten | Conference costs |
| EA201 | Vaste onkostenvergoeding | Fixed expense allowance |
| EA202 | Vaste reiskostenvergoeding | Fixed travel allowance |
| EA203 | Vaste thuiswerkvergoeding | Fixed home office allowance |
| EA204 | Kostenvergoeding overig | Other fixed expense allowance |
| EA300 | Verhuiskostenvergoeding | Relocation allowance |
| EA301 | Dubbele huisvesting | Dual housing costs |
| EA302 | Buitenlandvergoeding | Foreign posting allowance |
| EA600 | Consignatievergoeding | On-call / standby allowance |
| EA602 | Beschikbaarheidsvergoeding | Availability allowance |
| EA603 | Bereikbaarheidsvergoeding | Reachability allowance |
| EA604 | Aanwezigheidsvergoeding | Presence allowance |
| EA605 | Telefoonvergoeding | Phone allowance |
| EA606 | Internetvergoeding | Internet allowance |
| EA607 | Vergoeding arbodienst | Occupational health service |
| EA608 | Vergoeding bedrijfskleding | Work clothing allowance |
| EA609 | Vergoeding gereedschap | Tool allowance |
| EA610 | Vergoeding overig | Other compensation |
| EA801 | Thuiswerkvergoeding | Home office allowance |
| EA802 | Isolatievergoeding | Isolation allowance (COVID-era) |
| EA803 | Coronatoeslag | COVID surcharge |
| EA903 | Dienstjubileum | Service anniversary bonus |
| EA910 | Vergoeding cao | CLA-based compensation |

### HT-Series: Working Hours & Surcharges

| Code | Description (Dutch) | Description (English) |
|------|---------------------|-----------------------|
| HT100 | Overwerktoeslag | Overtime surcharge |
| HT101 | Overwerktoeslag ma-za | Overtime Mon-Sat |
| HT102 | Overwerktoeslag zon-ft | Overtime Sunday/holiday |
| HT200 | Toeslag onregelmatige uren | Irregular hours surcharge |
| HT201 | Toeslag onregelmatig ma-za | Irregular hours Mon-Sat |
| HT202 | Toeslag onregelmatig zon-ft | Irregular hours Sun/holiday |
| HT210 | Toeslag bijzondere uren | Special hours surcharge |
| HT211 | Toeslag bijzondere uren zon-ft | Special hours Sun/holiday |
| HT300 | Ploegentoeslag | Shift work surcharge |
| HT301 | Ploegentoeslag 2-ploegen | 2-shift surcharge |
| HT302 | Ploegentoeslag 3-ploegen | 3-shift surcharge |
| HT310 | Volcontinutoeslag | Continuous shift surcharge |
| HT311 | Volcontinutoeslag zon-ft | Continuous shift Sun/holiday |
| HT320 | Toeslag afwijkende uren | Deviating hours surcharge |
| HT321 | Weekendtoeslag | Weekend surcharge |
| HT322 | Zaterdagtoeslag | Saturday surcharge |
| HT330 | Zondagtoeslag | Sunday surcharge |
| HT331 | Feestdagtoeslag | Holiday surcharge |
| HT340 | Nachtdiensttoeslag | Night shift surcharge |
| HT400 | Consignatietoeslag | On-call surcharge |
| HT500 | Prestatietoeslag | Performance bonus |
| HT501 | Stukloontoeslag | Piecework surcharge |
| HT600 | Gevarentoeslag | Hazard pay |
| HT601 | Vuilwerktoeslag | Dirty work surcharge |
| HT602 | Onaangenaam werk toeslag | Unpleasant work surcharge |
| HT700 | Projecttoeslag | Project bonus |
| HT701 | Afbouwtoeslag | Phase-out surcharge |
| HT702 | Garantietoeslag | Guaranteed pay surcharge |
| HT703 | Waarnemingstoeslag | Acting/stand-in surcharge |
| HT800 | Inconveniëntentoeslag | Inconvenience surcharge |

## SupplementaryArrangementCode

| Code | Description |
|------|-------------|
| ERA | Early Retirement Arrangement |
| Generationpact | Generation pact (older workers reduce hours) |
| AccidentBenefit | Accident/occupational injury benefit |
| PAWW | Private aanvulling WW (private unemployment supplement) |
| PAZW | Private aanvulling ZW (private sickness supplement) |
| RVU | Regeling Vervroegde Uittreding (early retirement scheme) |
| WGA | Work Resumption Partial Disability scheme |

## SustainabilityCode (SustainableEmployability)

| Code | Description |
|------|-------------|
| ArrangementForFinancialHealth | Financial wellness programs |
| ArrangementForHealth | Health programs |
| ArrangementForMentalHealth | Mental health programs |
| CareerCoaching | Career coaching |
| Education | Education/training budget |
| InformationProvision | Information/knowledge provisions |
| Other | Other sustainability arrangements |
| OutplacementPrograms | Outplacement/redeployment programs |
| SocialSupport | Social support programs |
| SustainableSociety | Sustainability/CSR-related |
| VitalityBudget | Vitality/wellness budget |

## AmountUnitCode

| Code | Usage |
|------|-------|
| Hour | Hourly rate values |
| Percentage | Percentage values (e.g., 8% holiday allowance) |
| Euro | Fixed euro amounts |
| SalaryStep | Salary step references |
| Day | Daily rate values |

## BaseUnitCode

| Code | Description |
|------|-------------|
| HourlyRate | Per hour |
| DailyRate | Per day |
| FourWeeklyRate | Per 4-week period |
| MonthlyRate | Per month |
| YearlyRate | Per year |
| Expense | Reimbursable expense |
| Fixed | Fixed amount (not rate-based) |

## IntervalCode (31 codes)

| Code | Description |
|------|-------------|
| Day | Per day |
| DayPart | Per part of day (morning/afternoon) |
| Hour | Per hour |
| Item | Per item (piecework) |
| Kilometer | Per kilometer |
| Month | Per month |
| Night | Per night |
| Once | One-time |
| OnExpenseBasis | On actual expense basis |
| Quarter | Per quarter |
| Relocation | Per relocation event |
| Route | Per route |
| Shift | Per shift |
| Trip | Per trip |
| Week | Per week |
| WeeklyTravelDay | Per weekly travel day |
| WorkedDay | Per worked day |
| WorkFromHomeDay | Per WFH day |
| Year | Per year |

## BaseDefinitionCode

| Code | Description |
|------|-------------|
| GrossSalary | Gross/before-tax salary |
| UsualWage | Usual/lohn (SV-loon) wage |
| BaseWage | Base/foundation wage |
| SocialInsuranceWage | Social insurance wage (SV-loon) |
| HolidayAllowance | Holiday allowance base |
| Pension | Pension calculation base |
| SickPay | Sick pay calculation base |
| 13thMonth | 13th month / year-end bonus base |

## LabourAgreementReferenceType

| Type | Description |
|------|-------------|
| CollectiveLabourAgreement | Standard CAO |
| CollectiveLabourAgreementExtended | CAO with AVP (algemeen verbindend verklaard) |
| CustomLabourAgreement | Organization-specific agreement |
| Unknown | Source unknown/undetermined |

## IKbReferenceRelationType

| Type | Description |
|------|-------------|
| Included | Component is included in IKB |
| OnTop | Component is additional to IKB |
| None | No relationship to IKB |

## AllowanceReferenceExtended.relationType

| Type | Description |
|------|-------------|
| Compounding | Allowance A calculated first, used to calculate B |
| Cumulative | Allowances A + B both applied |
| Subtractive | Allowance B reduces A |

## ContributionSourceType

| Type |
|------|
| Employee |
| Employer |

## LegalSchemeAgencyId

| Code | Description |
|------|-------------|
| KvK | Kamer van Koophandel (Chamber of Commerce number) |
| OIN | Overheidsidentificatienummer (Government ID number) |
| RSIN | Rechtspersonen Samenwerkingsverbanden Informatienummer (Legal entity ID) |

## WeekdayCode

```
Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, Holiday
```

## EmploymentDurationReferenceDateType

```
SeniorityDate, HireDate, ContractStartDate, AssignmentStartDate
```

## Occurrence Events

```
HireDate, ContractStart, ContractEnd, SeniorityStart, BirthDate,
EvaluationDate, SalaryPeriodStart, SalaryPeriodEnd, SickLeave, OtherEvent
```

## Email Use Codes

```
Business, Private
```

## Operators (for Conditions)

```
eq, neq, gt, gte, lt, lte, in
```
