using SautinSoft;
using System.IO;

namespace PdfToWordConverter.Services
{
    public class PdfToWordService
    {
        public byte[] ConvertPdfToWord(byte[] pdfBytes)
        {
            // Initialize PDF Focus object
            PdfFocus pdfFocus = new PdfFocus();

            // Set the input PDF byte array
            pdfFocus.OpenPdf(pdfBytes);

            if (pdfFocus.PageCount > 0)
            {
                // Convert PDF to Word in DOCX format
                using (MemoryStream wordStream = new MemoryStream())
                {
                    pdfFocus.WordOptions.Format = PdfFocus.CWordOptions.eWordDocument.Docx;
                    pdfFocus.ToWord(wordStream);
                    return wordStream.ToArray();
                }
            }

            return null;
        }
    }
}
