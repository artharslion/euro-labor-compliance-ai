"""Dual-extraction: PyMuPDF full text + Camelot tables → merged markdown."""
import sys, fitz, camelot, io, json, os

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

pdf_path = sys.argv[1]
output_path = sys.argv[2]
max_pages = int(sys.argv[3]) if len(sys.argv) > 3 else 0
max_chars = int(sys.argv[4]) if len(sys.argv) > 4 else 150000

doc = fitz.open(pdf_path)
total_pages = doc.page_count
pages_to_process = min(max_pages, total_pages) if max_pages > 0 else total_pages

# ── Pass 1: PyMuPDF full text ──
sections = []
char_count = 0
for i in range(pages_to_process):
    page = doc[i]
    text = page.get_text()
    sections.append(f"## Page {i+1}\n\n{text}\n")
    char_count += len(text)
    if char_count > max_chars:
        sections.append(f"\n[TRUNCATED — {total_pages - i - 1} pages remaining]\n")
        break

full_text = "\n".join(sections)

# ── Pass 2: Camelot table extraction ──
tables_md = []
try:
    page_range = f"1-{pages_to_process}"
    
    # Try lattice first (for bordered tables)
    lattice_tables = camelot.read_pdf(pdf_path, pages=page_range, flavor='lattice')
    for i, t in enumerate(lattice_tables):
        acc = t.parsing_report.get('accuracy', 0)
        if acc > 50 and t.shape[1] > 1 and t.shape[0] > 1:
            tables_md.append(f"### Lattice Table {i+1} (acc={acc:.0f}%, {t.shape[0]}r x {t.shape[1]}c)\n\n{t.df.to_markdown(index=False)}\n")
    
    # Try stream for borderless tables  
    stream_tables = camelot.read_pdf(pdf_path, pages=page_range, flavor='stream')
    for i, t in enumerate(stream_tables):
        if t.shape[1] > 1 and t.shape[0] > 1:
            acc = t.parsing_report.get('accuracy', 0)
            if acc > 30:
                tables_md.append(f"### Stream Table {i+1} (acc={acc:.0f}%, {t.shape[0]}r x {t.shape[1]}c)\n\n{t.df.to_markdown(index=False)}\n")
except Exception as e:
    tables_md.append(f"\n[Camelot error: {e}]\n")

# ── Merge ──
if tables_md:
    full_text += "\n\n## EXTRACTED TABLES\n\n" + "\n".join(tables_md)

with open(output_path, 'w', encoding='utf-8') as f:
    f.write(full_text)

print(json.dumps({
    "pages_processed": pages_to_process,
    "total_pages": total_pages,
    "chars_extracted": char_count,
    "tables_extracted": len(tables_md),
    "truncated": char_count > max_chars
}))
