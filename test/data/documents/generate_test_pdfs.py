#!/usr/bin/env python3
"""
Generate synthetic test PDFs with salary tables for OCR testing.
Requires: reportlab (pip install reportlab)
"""

from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import cm
from reportlab.platypus import SimpleDocTemplate, Table, TableStyle, Paragraph, Spacer
from reportlab.lib.enums import TA_LEFT, TA_CENTER


def create_salary_table_pdf(output_path: str):
    """Generate test-salary-table.pdf with salary scale table."""
    doc = SimpleDocTemplate(
        output_path,
        pagesize=A4,
        leftMargin=2*cm,
        rightMargin=2*cm,
        topMargin=2*cm,
        bottomMargin=2*cm
    )

    styles = getSampleStyleSheet()
    title_style = ParagraphStyle(
        'CustomTitle',
        parent=styles['Heading1'],
        fontSize=16,
        spaceAfter=20,
        alignment=TA_CENTER
    )
    normal_style = ParagraphStyle(
        'CustomNormal',
        parent=styles['Normal'],
        fontSize=10,
        spaceAfter=8
    )

    elements = []

    # Title
    elements.append(Paragraph("Salarisschaal Magazijnmedewerker 2026", title_style))
    elements.append(Spacer(1, 10))

    # Salary table data
    table_data = [
        ['Trede', '<18 jaar', '18-20 jaar', '21+ jaar'],
        ['0', '€6.50', '€8.20', '€11.50'],
        ['1', '€7.00', '€9.00', '€12.50'],
        ['5', '€8.00', '€10.50', '€14.20'],
        ['10', '€9.50', '€12.00', '€16.37'],
    ]

    # Create table with visible borders
    table = Table(table_data, colWidths=[3*cm, 4*cm, 4*cm, 4*cm])
    table.setStyle(TableStyle([
        # Header row
        ('BACKGROUND', (0, 0), (-1, 0), colors.Color(0.2, 0.3, 0.5)),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, 0), 'CENTER'),
        ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
        ('FONTSIZE', (0, 0), (-1, 0), 11),
        ('BOTTOMPADDING', (0, 0), (-1, 0), 10),
        ('TOPPADDING', (0, 0), (-1, 0), 10),
        # Data rows
        ('BACKGROUND', (0, 1), (-1, -1), colors.Color(0.95, 0.95, 0.95)),
        ('TEXTCOLOR', (0, 1), (-1, -1), colors.black),
        ('ALIGN', (0, 1), (-1, -1), 'CENTER'),
        ('FONTNAME', (0, 1), (-1, -1), 'Helvetica'),
        ('FONTSIZE', (0, 1), (-1, -1), 10),
        ('BOTTOMPADDING', (0, 1), (-1, -1), 8),
        ('TOPPADDING', (0, 1), (-1, -1), 8),
        # Grid with visible borders
        ('GRID', (0, 0), (-1, -1), 1, colors.Color(0.3, 0.3, 0.3)),
        ('BOX', (0, 0), (-1, -1), 1.5, colors.Color(0.2, 0.2, 0.2)),
        # Row shading for alternation
        ('BACKGROUND', (0, 2), (-1, 2), colors.white),
        ('BACKGROUND', (0, 4), (-1, 4), colors.white),
    ]))

    elements.append(table)
    elements.append(Spacer(1, 20))

    # Notes below table
    elements.append(Paragraph("Jaarlijkse periodieke verhoging: 2,25%", normal_style))
    elements.append(Paragraph(
        "Functiegroep: 4 | Normale arbeidsduur: 38 uur/week | Valuta: EUR",
        normal_style
    ))

    doc.build(elements)
    print(f"Generated: {output_path}")


def create_allowance_matrix_pdf(output_path: str):
    """Generate test-allowance-matrix.pdf with allowance rate matrix."""
    doc = SimpleDocTemplate(
        output_path,
        pagesize=A4,
        leftMargin=2*cm,
        rightMargin=2*cm,
        topMargin=2*cm,
        bottomMargin=2*cm
    )

    styles = getSampleStyleSheet()
    title_style = ParagraphStyle(
        'CustomTitle',
        parent=styles['Heading1'],
        fontSize=16,
        spaceAfter=20,
        alignment=TA_CENTER
    )
    normal_style = ParagraphStyle(
        'CustomNormal',
        parent=styles['Normal'],
        fontSize=10,
        spaceAfter=8
    )

    elements = []

    # Title
    elements.append(Paragraph("Toeslagenmatrix LogiFlex 2026", title_style))
    elements.append(Spacer(1, 10))

    # Allowance table data
    table_data = [
        ['Toeslag', 'Code', 'Tarief', 'Basis', 'Voorwaarden'],
        ['Overwerk ma-za', 'HT101', '150%', 'Uurloon', '>38 uur/week'],
        ['Overwerk zondag', 'HT102', '200%', 'Uurloon', 'Zondag'],
        ['Onregelmatig', 'HT201', '35%', 'Uurloon', '18:00-06:00'],
        ['Weekend', 'HT321', '50%', 'Uurloon', 'Za & Zo'],
        ['Nacht (23-6)', 'HT340', '40%', 'Uurloon', '23:00-06:00'],
        ['Feestdag', 'HT331', '100%', 'Uurloon', 'Feestdagen'],
        ['Gevaarlijk werk', 'HT600', '25%', 'Uurloon', 'Rangeerzone'],
    ]

    # Create table with visible borders
    table = Table(table_data, colWidths=[4.5*cm, 2*cm, 2*cm, 2.5*cm, 4*cm])
    table.setStyle(TableStyle([
        # Header row
        ('BACKGROUND', (0, 0), (-1, 0), colors.Color(0.2, 0.4, 0.3)),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, 0), 'CENTER'),
        ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
        ('FONTSIZE', (0, 0), (-1, 0), 10),
        ('BOTTOMPADDING', (0, 0), (-1, 0), 10),
        ('TOPPADDING', (0, 0), (-1, 0), 10),
        # Data rows
        ('BACKGROUND', (0, 1), (-1, -1), colors.Color(0.95, 0.95, 0.95)),
        ('TEXTCOLOR', (0, 1), (-1, -1), colors.black),
        ('ALIGN', (0, 1), (0, -1), 'LEFT'),  # Left-align first column
        ('ALIGN', (1, 1), (-1, -1), 'CENTER'),
        ('FONTNAME', (0, 1), (-1, -1), 'Helvetica'),
        ('FONTSIZE', (0, 1), (-1, -1), 9),
        ('BOTTOMPADDING', (0, 1), (-1, -1), 7),
        ('TOPPADDING', (0, 1), (-1, -1), 7),
        # Grid with visible borders
        ('GRID', (0, 0), (-1, -1), 1, colors.Color(0.3, 0.3, 0.3)),
        ('BOX', (0, 0), (-1, -1), 1.5, colors.Color(0.2, 0.2, 0.2)),
        # Row shading for alternation
        ('BACKGROUND', (0, 2), (-1, 2), colors.white),
        ('BACKGROUND', (0, 4), (-1, 4), colors.white),
        ('BACKGROUND', (0, 6), (-1, 6), colors.white),
    ]))

    elements.append(table)
    elements.append(Spacer(1, 20))

    # Notes below table
    elements.append(Paragraph("Reiskosten woon-werk: €0,23/km (EA103)", normal_style))
    elements.append(Paragraph("Thuiswerkvergoeding: €2,35/dag (EA801)", normal_style))

    doc.build(elements)
    print(f"Generated: {output_path}")


if __name__ == "__main__":
    import os

    output_dir = os.path.dirname(os.path.abspath(__file__))
    os.makedirs(output_dir, exist_ok=True)

    # Generate both PDFs
    salary_path = os.path.join(output_dir, "test-salary-table.pdf")
    allowance_path = os.path.join(output_dir, "test-allowance-matrix.pdf")

    create_salary_table_pdf(salary_path)
    create_allowance_matrix_pdf(allowance_path)

    print("\nBoth PDFs generated successfully!")
