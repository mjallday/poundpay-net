using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace poundpay_test
{
    [TestClass]
    public class DeveloperTests : TestBase
    {
        /// <summary>
        /// PoundPay p = new PoundPay(DEVELOPER, AUTH);
        /// 
        /// var jsonData = p.Payments.find(params);
        /// 
        /// </summary>
        public DeveloperTests()
        {
            base.Init();
        }

        [TestMethod]
        public void TestFindMe()
        {
            poundpay.PoundPay pp = new poundpay.PoundPay(developerSid, authToken);

            var developer = pp.Developer.FindMe();

            Assert.AreEqual(developerSid, developer["sid"]);

            var cb = developer["callback_url"] == "http://www.example.com/" ? "http://www.example2.com/" : "http://www.example.com/";

            developer["callback_url"] = cb;

            developer.Save();

            developer = pp.Developer.FindMe();

            Assert.AreEqual(cb, developer["callback_url"]);
        }
    }
}
