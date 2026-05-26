# Test Data Validation Script
# Usage: python test/validate.py
# Validates that ground truth JSON files conform to expected SETU structure

import json
import os
from pathlib import Path

def validate_pay_equity(data, name):
    """Basic structural validation of a SETU Inquiry Pay Equity JSON."""
    errors = []
    
    # Required top-level fields
    required = ["documentId", "effectivePeriod", "customer", "remuneration"]
    for field in required:
        if field not in data:
            errors.append(f"Missing required field: {field}")
    
    # Validate remuneration (must be non-empty array)
    if "remuneration" in data:
        if not isinstance(data["remuneration"], list) or len(data["remuneration"]) == 0:
            errors.append("remuneration must be a non-empty array")
        else:
            for i, rem in enumerate(data["remuneration"]):
                if "workDuration" not in rem:
                    errors.append(f"remuneration[{i}] missing workDuration")
                if "salaryScale" not in rem:
                    errors.append(f"remuneration[{i}] missing salaryScale")
                else:
                    for j, scale in enumerate(rem["salaryScale"]):
                        if "salaryStep" not in scale:
                            errors.append(f"remuneration[{i}].salaryScale[{j}] missing salaryStep")
                        else:
                            for k, step in enumerate(scale["salaryStep"]):
                                if "value" not in step:
                                    errors.append(f"salaryStep[{i}.{j}.{k}] missing value")
    
    # Validate allowances (typeCode must be valid)
    valid_allowance_codes = {
        "HT100", "HT101", "HT102", "HT200", "HT201", "HT202", "HT210", "HT211",
        "HT300", "HT301", "HT302", "HT310", "HT311", "HT320", "HT321", "HT322",
        "HT330", "HT331", "HT340", "HT400", "HT500", "HT501",
        "HT600", "HT601", "HT602", "HT700", "HT701", "HT702", "HT703", "HT800",
        "EA100", "EA101", "EA102", "EA103", "EA105", "EA107", "EA108", "EA109",
        "EA110", "EA111", "EA112", "EA113", "EA201", "EA202", "EA203", "EA204",
        "EA300", "EA301", "EA302", "EA600", "EA602", "EA603", "EA604", "EA605",
        "EA606", "EA607", "EA608", "EA609", "EA610", "EA801", "EA802", "EA803",
        "EA903", "EA910"
    }
    if "allowance" in data:
        for i, allow in enumerate(data["allowance"]):
            tc = allow.get("typeCode", "")
            if tc and tc not in valid_allowance_codes:
                errors.append(f"allowance[{i}].typeCode '{tc}' not in valid AllowanceCode list")
    
    # Validate condition types
    valid_condition_types = {"Age", "EmploymentDuration", "Occurrence", "PositionProfile",
                              "SalaryScale", "Text", "AllOf", "AnyOf", "Not"}
    
    def check_conditions(obj, path):
        if isinstance(obj, dict):
            if obj.get("conditionType") and obj.get("conditionType") not in valid_condition_types:
                errors.append(f"{path}: invalid conditionType '{obj['conditionType']}'")
            for key, val in obj.items():
                check_conditions(val, f"{path}.{key}")
        elif isinstance(obj, list):
            for i, item in enumerate(obj):
                check_conditions(item, f"{path}[{i}]")
    
    check_conditions(data, name)
    
    if errors:
        print(f"[FAIL] {name}: {len(errors)} validation errors")
        for e in errors:
            print(f"   - {e}")
    else:
        print(f"[PASS] {name}: Passed structural validation")
    return len(errors) == 0

if __name__ == "__main__":
    test_dir = Path(__file__).parent
    gt_dir = test_dir / "data" / "ground-truth"
    
    all_passed = True
    for json_file in sorted(gt_dir.glob("tc-*.json")):
        with open(json_file, "r", encoding="utf-8") as f:
            data = json.load(f)
        if not validate_pay_equity(data, json_file.name):
            all_passed = False
    
    if all_passed:
        print(f"\n[OK] All ground truth files validated successfully!")
    else:
        print(f"\n[WARN] Some files have validation errors")
