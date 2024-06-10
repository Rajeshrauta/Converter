using SautinSoft;
using System.IO;

namespace PdfToWordConverter.Services
{
    public class PdfToWordService
    {
        public byte[] ConvertPdfToWord(byte[] pdfBytes)
        {
            PdfFocus pdfFocus = new PdfFocus();
            pdfFocus.OpenPdf(pdfBytes);

            if (pdfFocus.PageCount > 0)
            {
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
