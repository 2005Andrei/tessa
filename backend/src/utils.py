import io
import re
from docx import Document
from docx.shared import Inches
from bs4 import BeautifulSoup


def create_doc(texts):
    doc = Document()

    for text in texts:
        doc.add_paragraph(text)

    return doc

