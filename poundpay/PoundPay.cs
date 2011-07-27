using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace poundpay
{
	public class PoundPay
	{
		public readonly Payment Payment;
		private Client client;

		public PoundPay(string developerSid, string auth)
		{
			client = new Client(developerSid, auth);

			Payment = new Payment(client);
		}
	}
}
