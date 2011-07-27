using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace poundpay_test
{
	[TestClass]
	public class PaymentTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			poundpay.PoundPay pp = new poundpay.PoundPay(string.Empty, string.Empty);

			var payment = pp.Payment.Create();

			payment["something"] = "something else";

			payment.Save();

		}
	}
}
