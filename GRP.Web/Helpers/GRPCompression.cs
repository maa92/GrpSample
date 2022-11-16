using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GrpSample.Web.Helpers
{
    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(encodingsAccepted)) return;

            encodingsAccepted = encodingsAccepted.ToLowerInvariant();
            var response = filterContext.HttpContext.Response;

            if (encodingsAccepted.Contains("deflate"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
            else if (encodingsAccepted.Contains("gzip"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
        }
    }

    //public static class GRPCompression
    //{
    //    public static byte[] Compress(byte[] data)
    //    {
    //        using (var compressedStream = new MemoryStream())
    //        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
    //        {
    //            zipStream.Write(data, 0, data.Length);
    //            zipStream.Close();
    //            return compressedStream.ToArray();
    //        }
    //    }
    //}
}