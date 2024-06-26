﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SautinSoft;

namespace PdfToWordConverter.Services
{
    public class PdfToWordService
    {
        //SautinSoft
        public byte[] ConvertPdfToWord(byte[] pdfBytes)
        {
            byte[] wordBytes;
            PdfFocus pdfFocus = new PdfFocus();
            pdfFocus.OpenPdf(pdfBytes);

            if (pdfFocus.PageCount > 0)
            {
                using (MemoryStream wordStream = new MemoryStream())
                {
                    pdfFocus.WordOptions.Format = PdfFocus.CWordOptions.eWordDocument.Docx;
                    pdfFocus.ToWord(wordStream);
                    wordBytes = wordStream.ToArray();
                }
            }
            else
            {
                return null;
            }
            return RemoveLastPageFromWord(wordBytes);
        }


        
        private byte[] RemoveLastPageFromWord(byte[] wordBytes)
        {
            using (var wordStream = new MemoryStream())
            {
                wordStream.Write(wordBytes, 0, wordBytes.Length);
                using (var wordDoc = WordprocessingDocument.Open(wordStream, true))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;
                    var paragraphs = body.Elements<Paragraph>().ToList();
                    var tables = body.Elements<DocumentFormat.OpenXml.InkML.Table>().ToList();

                    // Find the last element (either paragraph or table) and remove it
                    if (paragraphs.Any())
                    {
                        var lastParagraph = paragraphs.Last();
                        lastParagraph.Remove();
                    }
                    else if (tables.Any())
                    {
                        var lastTable = tables.Last();
                        lastTable.Remove();
                    }

                    wordDoc.MainDocumentPart.Document.Save();
                }

                return wordStream.ToArray();
            }
        }
    }
}
