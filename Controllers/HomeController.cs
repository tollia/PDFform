using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDFform.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace PDFform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            byte[] pdfBytes;
            using (MemoryStream outStream = new MemoryStream())
            using (PdfReader reader = new PdfReader(@"./Data/PDFform.pdf"))
            using (PdfWriter writer = new PdfWriter(outStream))
            using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
            {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                IDictionary<String, PdfFormField> fields = form.GetFormFields();
                fields["service"].SetValue("RSK");
                fields["service"].SetReadOnly(true);
                fields["grantor"].SetValue("280467-3409");
                fields["grantor"].SetReadOnly(true);
                fields["grantee"].SetValue("700169-3759");
                fields["grantee"].SetReadOnly(true);
                fields["year"].SetValue("2021");
                fields["year"].SetReadOnly(true);
                //pdfDoc.SetCloseWriter(false);
                pdfDoc.Close();
                pdfBytes = outStream.ToArray();
            }

            return new FileContentResult(pdfBytes, MediaTypeNames.Application.Pdf);
        }

        public IActionResult Read(IFormFile file)
        {
            IDictionary<String, PdfFormField> fields = new Dictionary<String, PdfFormField>();
            if (file != null)
            {
                using (Stream inStream = file.OpenReadStream())
                using (PdfReader reader = new PdfReader(inStream))
                using (PdfDocument pdfDoc = new PdfDocument(reader))
                {
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                    fields = form.GetFormFields();
                    string service = fields["service"].GetValueAsString();
                }
            }

            return View(fields);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
