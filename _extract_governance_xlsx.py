"""Extract all cell values from governance dev xlsx."""
import sys
import zipfile
import xml.etree.ElementTree as ET
from pathlib import Path

XLSX = Path(r"D:\project\GBL_NET8_2026_live\GBL_MVC\wwwroot\scripts\goveranance page dev.xlsx")
OUT = Path(r"D:\project\GBL_NET8_2026_live\_governance_xlsx_dump.txt")

NS = {
    "main": "http://schemas.openxmlformats.org/spreadsheetml/2006/main",
    "rel": "http://schemas.openxmlformats.org/officeDocument/2006/relationships",
}


def col_row(ref: str):
    col = ""
    row = ""
    for c in ref:
        if c.isalpha():
            col += c
        else:
            row += c
    return col, int(row) if row else 0


def load_shared_strings(z: zipfile.ZipFile) -> list[str]:
    try:
        data = z.read("xl/sharedStrings.xml")
    except KeyError:
        return []
    root = ET.fromstring(data)
    strings = []
    for si in root.findall("main:si", NS):
        parts = []
        for t in si.findall(".//main:t", NS):
            if t.text:
                parts.append(t.text)
            if t.tail:
                parts.append(t.tail)
        strings.append("".join(parts))
    return strings


def sheet_name_map(z: zipfile.ZipFile) -> dict[str, str]:
    wb = ET.fromstring(z.read("xl/workbook.xml"))
    rels = ET.fromstring(z.read("xl/_rels/workbook.xml.rels"))
    rid_to_target = {}
    for rel in rels:
        rid = rel.attrib.get("Id")
        target = rel.attrib.get("Target", "")
        if rid:
            rid_to_target[rid] = target
    mapping = {}
    for sh in wb.findall("main:sheets/main:sheet", NS):
        name = sh.attrib.get("name", "")
        rid = sh.attrib.get("{http://schemas.openxmlformats.org/officeDocument/2006/relationships}id")
        if rid and rid in rid_to_target:
            t = rid_to_target[rid]
            if not t.startswith("xl/"):
                t = "xl/" + t.lstrip("/")
            mapping[name] = t
    return mapping


def cell_value(c, shared: list[str]):
    t = c.attrib.get("t")
    v = c.find("main:v", NS)
    if v is None or v.text is None:
        is_el = c.find("main:is", NS)
        if is_el is not None:
            parts = []
            for t_el in is_el.findall(".//main:t", NS):
                if t_el.text:
                    parts.append(t_el.text)
            return "".join(parts)
        return None
    if t == "s":
        return shared[int(v.text)]
    return v.text


def main():
    lines = []
    with zipfile.ZipFile(XLSX, "r") as z:
        shared = load_shared_strings(z)
        sheets = sheet_name_map(z)
        lines.append(f"FILE: {XLSX}")
        lines.append(f"SHEET COUNT: {len(sheets)}")
        for sheet_name, path in sheets.items():
            lines.append("")
            lines.append(f"=== SHEET: {sheet_name} ===")
            root = ET.fromstring(z.read(path))
            sheet_data = root.find("main:sheetData", NS)
            if sheet_data is None:
                continue
            for row in sheet_data.findall("main:row", NS):
                for c in row.findall("main:c", NS):
                    ref = c.attrib.get("r", "")
                    val = cell_value(c, shared)
                    if val is not None and str(val).strip() != "":
                        col, rnum = col_row(ref) if ref else ("?", 0)
                        lines.append(f"{col}{rnum}\t{val}")
    text = "\n".join(lines)
    OUT.write_text(text, encoding="utf-8")
    print(text)


if __name__ == "__main__":
    main()
