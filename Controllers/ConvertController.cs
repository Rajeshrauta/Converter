using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using PdfToWordConverter.Services;

namespace PdfToWordConverter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConvertController : ControllerBase
    {
        private readonly PdfToWordService _pdfToWordService;

        public ConvertController(PdfToWordService pdfToWordService)
        {
            _pdfToWordService = pdfToWordService;
        }

        //SautinSoft
        [HttpPost("pdf-to-word")]
        public IActionResult ConvertPdfToWord(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
                return BadRequest("No file uploaded.");

            if (Path.GetExtension(pdfFile.FileName).ToLower() != ".pdf")
                return BadRequest("Please upload a PDF file.");

            try
            {

                var originalFileName = Path.GetFileNameWithoutExtension(pdfFile.FileName);
                var newFileName = $"{originalFileName}.docx";

                using (var memoryStream = new MemoryStream())
                {
                    pdfFile.CopyTo(memoryStream);
                    var pdfBytes = memoryStream.ToArray();
                    pdfBytes = AddBlankPageToPdf(pdfBytes);
                    var wordBytes = _pdfToWordService.ConvertPdfToWord(pdfBytes);
                    if (wordBytes == null)
                        return StatusCode(500, "Conversion failed.");

                    return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", newFileName);
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error converting PDF to Word: {ex.Message}");
                return StatusCode(500, "An error occurred during conversion.");
            }
        }

        private byte[] AddBlankPageToPdf(byte[] pdfBytes)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(new MemoryStream(pdfBytes));
                PdfWriter writer = new PdfWriter(outputStream);
                PdfDocument pdfDoc = new PdfDocument(reader, writer);

                // Add a blank page
                pdfDoc.AddNewPage();

                pdfDoc.Close();
                reader.Close();
                writer.Close();

                return outputStream.ToArray();
            }
        }
    }
}
