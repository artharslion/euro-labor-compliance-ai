namespace EuroLaborCompliance.Pipeline.Models;

public enum SchemeAgencyId { Customer, Supplier }

public enum LabourAgreementType { CollectiveLabourAgreement, CollectiveLabourAgreementExtended, CustomLabourAgreement, Unknown }

public enum AllowanceCode
{
    HT100, HT101, HT102, HT103, HT104, HT105, HT106, HT107, HT108, HT109,
    HT110, HT111, HT112, HT113, HT114, HT115, HT116, HT117, HT118, HT119,
    HT120, HT121, HT122, HT123, HT124, HT125, HT126, HT127, HT128, HT129,
    HT130, HT131, HT132, HT133, HT134, HT135, HT136, HT137, HT138, HT139,
    HT140, HT141, HT142, HT143, HT144, HT145S, HT145, HT149, HT150, HT151,
    HT152, HT153, HT154, HT155, HT156, HT157, HT158, HT159, HT160, HT810,
    HT811, HT812, HT813, HT814, HT815, HT816, HT817, HT818, HT819, HT820,
    HT821, HT822, HT823, HT910, EA100, EA110, EA120, EA130, EA140, EA150,
    EA200, EA210, EA220, EA300, EA301, EA310, EA311, EA312, EA320, EA330,
    EA400, EA410, EA500, EA510, EA520, EA600, EA610, EA620, EA630, EA700,
    EA710, EA800, EA810, EA820, EA830, EA840, EA850, EA860, EA900, EA901,
    EA902, EA903, EA904, EA905, EA906, EA907, EA908, EA909, EA910
}

public enum AmountUnitCode { Hour, Percentage, Euro, SalaryStep, Day }

public enum BaseUnitCode { HourlyRate, DailyRate, FourWeeklyRate, MonthlyRate, YearlyRate, Expense, Fixed }

public enum IntervalCode
{
    Day, DayPart, Hour, Item, Kilometer, Month, Night, Once, OnExpenseBasis,
    Quarter, Relocation, Route, Shift, Trip, Week, WeeklyTravelDay, WorkedDay, WorkFromHomeDay, Year
}

public enum BaseDefinitionCode { GrossSalary, UsualWage, BaseWage, SocialInsuranceWage, HolidayAllowance, Pension, SickPay, ThirteenthMonth }

public enum WeekdayCode { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, Holiday }

public enum ContributionSourceType { Employee, Employer }

public enum IkbRelationType { Included, OnTop, None }

public enum AllowanceRelationType { Compounding, Cumulative, Subtractive }

public enum LegalSchemeAgencyId { KvK, OIN, RSIN }

public enum SupplementaryArrangementCode { ERA, Generationpact, AccidentBenefit, PAWW, PAZW, RVU, WGA }

public enum SustainabilityCode
{
    ArrangementForFinancialHealth, ArrangementForHealth, CareerCoaching, Education,
    InformationProvision, Other, OutplacementPrograms, SocialSupport, SustainableSociety, VitalityBudget
}

public enum OrderType { Order, RFQ }

public enum PositionResponsibilityCode { Placeholder }
