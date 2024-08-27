using DocumentManager.Models;
using DocumentManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        IDocumentManagerService _documentManager;

        public DocumentController(IDocumentManagerService documentManager)
        {
            _documentManager = documentManager;
        }

        [HttpPost(Name = "GenerateDocuments")]
        public async Task<FileResult> Post(Agreement document)
        {
            string html = _documentManager.ConvertObjectToHtml(document);
            byte[] pdfDocs = await _documentManager.ConvertHtmlToPdfAsync(html);
            string file_type = "application/pdf";
            string file_name = "agreement.pdf";
            return File(pdfDocs, file_type, file_name);
        }
    }
}