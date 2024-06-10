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

        [HttpPost("pdf-to-word")]
        public IActionResult ConvertPdfToWord(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
                return BadRequest("No file uploaded.");

            var originalFileName = Path.GetFileNameWithoutExtension(pdfFile.FileName);
            // Set the new file name with .docx extension
            var newFileName = $"{originalFileName}.docx";

            using (var memoryStream = new MemoryStream())
            {
                pdfFile.CopyTo(memoryStream);
                var pdfBytes = memoryStream.ToArray();
                var wordBytes = _pdfToWordService.ConvertPdfToWord(pdfBytes);
                if (wordBytes == null)
                    return StatusCode(500, "Conversion failed.");

                return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", newFileName);
            }
        }
    }
}
