using DocumentManager.Models;

namespace DocumentManager.Services
{
    public interface IDocumentManagerService
    {
        public string ConvertObjectToHtml(Agreement document);

        public Task<byte[]> ConvertHtmlToPdfAsync(string html);
    }
}
