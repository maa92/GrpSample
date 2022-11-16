using GrpSample.Models.System;
using Microsoft.Owin;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace GrpSample.Web.Controllers
{
    public class BaseController : Controller
    {
        private IOwinContext _owinContext;
        public IOwinContext OwinContext
        {
            get
            {
                if (_owinContext == null)
                    _owinContext = Request.GetOwinContext();
                return _owinContext;
            }
        }

        private string _reportingController;
        public string ReportingController
        {
            get
            {
                if (_reportingController == null)
                    _reportingController = ConfigurationManager.AppSettings["ReportingController"];
                return _reportingController.Split('/')[0];
            }
        }

        public string PDFReportingAction
        {
            get
            {
                if (_reportingController == null)
                    _reportingController = ConfigurationManager.AppSettings["ReportingController"];
                return _reportingController.Split('/')[1];
            }
        }

        private string _reportingUrl;
        public string ReportingUrl
        {
            get
            {
                if (_reportingUrl == null)
                    _reportingUrl = ConfigurationManager.AppSettings["ReportingUrl"];
                return _reportingUrl;
            }
        }
    }
}