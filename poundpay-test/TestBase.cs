using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace poundpay_test
{
    public abstract class TestBase
    {
		protected string developerSid, authToken, apiUrl, apiVersion;

		public void Init()
		{
			developerSid = System.Configuration.ConfigurationManager.AppSettings["sid"];
			authToken = System.Configuration.ConfigurationManager.AppSettings["token"];
			apiUrl = System.Configuration.ConfigurationManager.AppSettings["api_url"];
			apiVersion = System.Configuration.ConfigurationManager.AppSettings["version"];
		}
    }
}
