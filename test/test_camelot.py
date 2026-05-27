import camelot, sys, io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

cao_pdf = 'D:/Repos/euro-labor-compliance-ai/test/data/documents/ABU-CAO-2026-2028.pdf'
test_pdf = 'D:/Repos/euro-labor-compliance-ai/test/data/documents/test-salary-table.pdf'

# Test 1: Complex CAO PDF — lattice mode
print('=== ABU CAO pages 60-75 (lattice) ===')
tables = camelot.read_pdf(cao_pdf, pages='60-75', flavor='lattice')
print(f'Found: {len(tables)} tables')
for t in tables[:3]:
    acc = t.parsing_report.get('accuracy', 0)
    print(f'  Accuracy: {acc:.0f}%, {t.shape[0]}r x {t.shape[1]}c')
    print(t.df.head(5).to_string())
    print()

# Test 2: Clean test PDF
print('=== Test Salary Table (lattice) ===')
t2 = camelot.read_pdf(test_pdf, flavor='lattice')
for t in t2:
    acc = t.parsing_report.get('accuracy', 0)
    print(f'  Accuracy: {acc:.0f}%, {t.shape[0]}r x {t.shape[1]}c')
    print(t.df.to_string())

# Test 3: CAO with stream flavor
print('\n=== ABU CAO pages 60-75 (stream) ===')
t3 = camelot.read_pdf(cao_pdf, pages='60-75', flavor='stream')
print(f'Found: {len(t3)} tables')
for t in t3[:4]:
    print(f'  {t.shape[0]}r x {t.shape[1]}c')
    print(t.df.head(3).to_string())
    print()
