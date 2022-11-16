using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GrpSample.Web.Helpers
{
    //Custom Instance for converting file in byte[] to HttpPostedFileBase object
    public partial class CustomPostedFile : HttpPostedFileBase
    {
        private readonly byte[] fileBytes;
        MemoryStream stream;

        public CustomPostedFile(byte[] fileBytes, string filename = null, string fileType = null)
        {
            this.fileBytes = fileBytes;
            this.FileName = filename;
            this.ContentType = fileType;//"application/pdf";//"application/octet-stream";
            this.stream = new MemoryStream(fileBytes);
        }

        public override int ContentLength => fileBytes.Length;
        public override string FileName { get; }
        public override Stream InputStream
        {
            get { return stream; }
        }
        public override string ContentType { get; }
    }
}