using GRP.Web.Helpers;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GRP.Web.Controllers
{
    [Authorize]
    public class ReportingController : BaseController
    {
        public async Task<ActionResult> PDF(string prm)
        {
            if (string.IsNullOrEmpty(prm))
                return Content("Invalid Parameters Sent");

            WebClient webClient = new WebClient();
            Stream stream = await webClient.OpenReadTaskAsync(string.Format(ReportingUrl, prm));
            Response.AppendHeader("Content-Disposition", "inline;filename=Print.pdf");
            return new FileStreamResult(stream, "application/pdf");
        }

        public async Task<ActionResult> IntDocs(string prm)
        {
            if (string.IsNullOrEmpty(prm))
                return Content("Invalid Parameters Sent");

            string reportParamContent = "report={0}|no_prev";
            string[] allReports = prm.Split('-');
            string decodedParams;
            List<string> finalPrintingUrls = new List<string>();
            
            foreach (string repUrl in allReports)
            {
                decodedParams = GRPEncoding.DecodeString(repUrl);
                if (decodedParams.StartsWith("hrIntidabCard"))
                {
                    ProcessEmpIntCards(reportParamContent, decodedParams, finalPrintingUrls);
                }
                else
                {
                    finalPrintingUrls.Add(GRPEncoding.EncodeString(string.Format(reportParamContent, decodedParams)));
                }
            }

            using (PdfDocument finalPrint = new PdfDocument())
            {
                PdfDocument report = new PdfDocument();

                foreach (string printPrm in finalPrintingUrls)
                {
                    WebClient webClient = new WebClient();
                    Stream stream = await webClient.OpenReadTaskAsync(string.Format(ReportingUrl, printPrm));
                    MemoryStream ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    report = PdfReader.Open(ms, PdfDocumentOpenMode.Import);
                    CopyPDF(report, finalPrint);
                    report.Dispose();
                    stream.Dispose();
                    ms.Dispose();
                }

                //byte[] fileContents = null;
                MemoryStream finalReturnStream = new MemoryStream();

                finalPrint.Save(finalReturnStream, false);
                //fileContents = stream.ToArray();

                Response.AppendHeader("Content-Disposition", "inline;filename=Print.pdf");
                return new FileStreamResult(finalReturnStream, "application/pdf");
            }
        }

        public async Task<ActionResult> OvrDocs(string prm)
        {
            if (string.IsNullOrEmpty(prm))
                return Content("Invalid Parameters Sent");

            string reportParamContent = "report={0}|no_prev";
            string[] allReports = prm.Split('-');
            string decodedParams;
            List<string> finalPrintingUrls = new List<string>();

            foreach (string repUrl in allReports)
            {
                decodedParams = GRPEncoding.DecodeString(repUrl);
                if (decodedParams.StartsWith("hrOvertimeCard"))
                {
                    ProcessEmpIntCards(reportParamContent, decodedParams, finalPrintingUrls);
                }
                else
                {
                    finalPrintingUrls.Add(GRPEncoding.EncodeString(string.Format(reportParamContent, decodedParams)));
                }
            }

            using (PdfDocument finalPrint = new PdfDocument())
            {
                PdfDocument report = new PdfDocument();

                foreach (string printPrm in finalPrintingUrls)
                {
                    WebClient webClient = new WebClient();
                    Stream stream = await webClient.OpenReadTaskAsync(string.Format(ReportingUrl, printPrm));
                    MemoryStream ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    report = PdfReader.Open(ms, PdfDocumentOpenMode.Import);
                    CopyPDF(report, finalPrint);
                    report.Dispose();
                    stream.Dispose();
                    ms.Dispose();
                }

                //byte[] fileContents = null;
                MemoryStream finalReturnStream = new MemoryStream();

                finalPrint.Save(finalReturnStream, false);
                //fileContents = stream.ToArray();

                Response.AppendHeader("Content-Disposition", "inline;filename=Print.pdf");
                return new FileStreamResult(finalReturnStream, "application/pdf");
            }
        }

        /*MemoryStream GetFileStream(Stream stream)
        {
            byte[] buffer = new byte[4096];

            MemoryStream memoryStream = new MemoryStream();

            int count = 0;
            do
            {
                count = stream.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, count);

            } while (count != 0);

            return memoryStream;
        }*/

        void ProcessEmpIntCards(string reportingPrintUrl,string intCardParams, List<string> UrlPrintUrls)
        {
            string empList = intCardParams.Substring(intCardParams.IndexOf('|') + 5).Substring(0, intCardParams.Substring(intCardParams.IndexOf('|') + 5).IndexOf('|'));
            string reportPrintParams = intCardParams.Replace(empList, "xxxx");

            string[] empIds = empList.Split(',');
            foreach (string eId in empIds)
            {
                UrlPrintUrls.Add(GRPEncoding.EncodeString(string.Format(reportingPrintUrl, reportPrintParams.Replace("xxxx",eId))));
            }            
        }

        void CopyPDF(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
    }

    //public async Task<ActionResult> Index(string prm)
    //{
    //    if (string.IsNullOrEmpty(prm))
    //        return Content("Invalid Parameters Sent");

    //    WebClient webClient = new WebClient();
    //    Stream stream = await webClient.OpenReadTaskAsync("http://localhost:14811/ViewReport?rp=" + prm);
    //    StreamReader reader = new StreamReader(stream);
    //    String respone = reader.ReadToEnd();
    //    return Content(respone);
    //}

    //public class CustomFileResult : FileContentResult
    //{
    //    public CustomFileResult(byte[] fileContents, string contentType) : base(fileContents, contentType)
    //    {
    //    }

    //    public bool Inline { get; set; }

    //    public override void ExecuteResult(ControllerContext context)
    //    {
    //        if (context == null)
    //        {
    //            throw new ArgumentNullException("context");
    //        }
    //        HttpResponseBase response = context.HttpContext.Response;
    //        response.ContentType = ContentType;
    //        if (!string.IsNullOrEmpty(FileDownloadName))
    //        {
    //            string str = new System.Net.Mime.ContentDisposition { FileName = this.FileDownloadName, Inline = Inline }.ToString();
    //            context.HttpContext.Response.AddHeader("Content-Disposition", str);
    //        }
    //        WriteFile(response);
    //    }
    //}
}