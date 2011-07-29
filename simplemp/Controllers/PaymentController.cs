using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace simplemp.Controllers
{
    public class PaymentController : Controller
    {
        static string developerSid, authToken, apiUrl, apiVersion, iframeRootUri;

        private static poundpay.PoundPay _pp;
        private static poundpay.PoundPay PP
        {
            get
            {
                if (_pp == null)
                {
                    developerSid = System.Configuration.ConfigurationManager.AppSettings["sid"];
                    authToken = System.Configuration.ConfigurationManager.AppSettings["token"];
                    apiUrl = System.Configuration.ConfigurationManager.AppSettings["api_url"];
                    apiVersion = System.Configuration.ConfigurationManager.AppSettings["version"];
                    iframeRootUri = System.Configuration.ConfigurationManager.AppSettings["www_url"];

                    _pp = new poundpay.PoundPay(developerSid, authToken, apiUrl, apiVersion);

                }

                return _pp;
            }
        }

        public ActionResult Index()
        {
            var pp = PP;

            ViewData["iframe_root_uri"] = iframeRootUri;
            ViewData["amount"] = "12300";
            ViewData["payer_fee_amount"] = "125";
            ViewData["recipient_fee_amount"] = "126";
            ViewData["payer_email_address"] = "testuser@poundpay.com";
            ViewData["recipient_email_address"] = "joebloggs@poundpay.com";
            ViewData["description"] = "Test Poundpay Payment";
            ViewData["developer_identifier"] = "";

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create()
        {

            var payment = PP.Payment.Create();

            foreach (string val in Request.Form.Keys)
            {
                payment[val] = Request.Form[val];
            }

            payment.Save();

            return Json(new { sid = payment["sid"] });

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Escrow()
        {
            var payment = PP.Payment.Find(Request.Form["sid"]);

            payment.Escrow();

            return Json(payment);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Release()
        {

            var payment = PP.Payment.Find(Request.Form["sid"]);

            payment.Release();

            return Json(payment);
        }
    }
}
