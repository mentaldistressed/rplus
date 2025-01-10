using DocumentManager.Models;
using PuppeteerSharp;
using System.Text;

namespace DocumentManager.Services
{
    public class DocumentManagerService : IDocumentManagerService
    {
        private readonly IConfiguration _configuration;

        public DocumentManagerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<byte[]> ConvertHtmlToPdfAsync(string html)
        {
            var pdfOptions = new PdfOptions() { Format = PuppeteerSharp.Media.PaperFormat.A4, MarginOptions = new PuppeteerSharp.Media.MarginOptions() { Top = "25" } };
            
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[]
                {
                    "--no-sandbox"
                }
            }))
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.SetContentAsync(html);
                    return await page.PdfDataAsync(pdfOptions);
                }
            }
        }

        public string ConvertObjectToHtml(Agreement document)
        {
            // При полном заполнений данными
            if(document.typeId == 1)
            {
                string text = File.ReadAllText(_configuration["MainHtmlTemplatePath"]);

                StringBuilder mainSb = new StringBuilder(text);
                
                List<string> documentStaticData = new List<string>()
                {
                    document.Id.ToString(),
                    document.AgreementDate.ToString(),
                    document.documentDate.ToString(),
                    document.Country ?? " ",
                    document.Fio ?? " ",
                    document.NickName ?? "  ",
                    document.PassData.PassportID + " " + document.PassData.PassportSeries ?? " ",
                    document.PassData.Issued ?? " ",
                    document.PassData.DepartmentCode ?? " ",
                    document.PassData.DateOfIssue.ToString(),
                    document.PassData.Email ?? " ",
                    document.PassData.AccountNumber ?? " ",
                    document.PassData.PayeesBank ?? " ",
                    document.PassData.BIC ?? " ",
                    document.PassData.correspondentAccount + " ( Номер карты: " + document.PassData.CardNumber + ")" ?? " ",
                    document.RoyalityDatas[0].firstRow ?? " ",
                    document.RoyalityDatas[0].secondRow ?? " ",
                    document.RoyalityDatas[1].firstRow ?? " ",
                    document.RoyalityDatas[1].secondRow ?? " ",
                    document.RoyalityDatas[2].firstRow ?? " ",
                    document.RoyalityDatas[2].secondRow ?? " ",
                    document.RoyalityDatas[3].firstRow ?? " ",
                    document.RoyalityDatas[3].secondRow ?? " ",
                    document.fioTP ?? " "
                };
                
                for (int i = 0; i < documentStaticData.Count; i++)
                {
                    mainSb.Replace($"[{i}]", documentStaticData[i]);
                }
                
                // После статичных данных, заполняем таблицы
                StringBuilder tablesSb = new();
                
                string staticTableRow = "<tr><td class='tg-9wq8'>{0}</td><td class='tg-9wq8'>{1}</td><td class='tg-9wq8'>{2}</td><td class='tg-9wq8'>{3}</td><td class='tg-9wq8'>{4}</td><td class='tg-9wq8'>{5}</td><td class='tg-9wq8'>{6}</td><td class='tg-9wq8'>{7}</td></tr>";
                
                document.Musics.ForEach(m =>
                {
                    tablesSb.Append(string.Format(staticTableRow, m.Id, m.Title, m.Duration, m.InstrumentalAuthorFIO, m.WordAuthorFIO, m.Executor, m.ManufacturerOfPhonogram, m.LicenseTerm));
                });
                
                mainSb.Insert(mainSb.ToString().IndexOf("<tbody id='tgInsertDataHere'>"), tablesSb.ToString());

                return mainSb.ToString();
            }
            else
            {
                string text = File.ReadAllText(_configuration["SecondHtmlTemplatePath"]);

                StringBuilder mainSb = new StringBuilder(text);

                if(document.applId is null)
                {
                    document.applId = 1;
                }

                List<string> documentStaticData = new List<string>()
                {
                    document.applId.ToString(),
                    document.Id.ToString(),
                    document.AgreementDate.ToString(),
                    document.documentDate.ToString(),
                    document.Country ?? " ",
                    document.Fio ?? " "
                };

                for (int i = 0; i < documentStaticData.Count; i++)
                {
                    mainSb.Replace($"[{i}]", documentStaticData[i]);
                }

                // После статичных данных, заполняем таблицы
                StringBuilder tablesSb = new();

                string staticTableRow = "<tr><td class='tg-9wq8'>{0}</td><td class='tg-9wq8'>{1}</td><td class='tg-9wq8'>{2}</td><td class='tg-9wq8'>{3}</td><td class='tg-9wq8'>{4}</td><td class='tg-9wq8'>{5}</td><td class='tg-9wq8'>{6}</td><td class='tg-9wq8'>{7}</td></tr>";

                document.Musics.ForEach(m =>
                {
                    tablesSb.Append(string.Format(staticTableRow, m.Id, m.Title, m.Duration, m.InstrumentalAuthorFIO, m.WordAuthorFIO, m.Executor, m.ManufacturerOfPhonogram, m.LicenseTerm));
                });

                mainSb.Insert(mainSb.ToString().IndexOf("<tbody id='tgInsertDataHere'>"), tablesSb.ToString());

                return mainSb.ToString();
            }
            
        }
    }
}
