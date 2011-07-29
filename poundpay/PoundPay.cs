using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace poundpay
{
	/// <summary>
	/// Pound pay mega object
	/// </summary>
	/// <example>
	/// var pp = new PoundPay("DEVELOPER_SID", "AUTH_TOKEN");
	/// 
	/// var payment = pp.Payment.Create();
	/// payment["amount"] = 4400;
	/// payment["recipient_email"] = "a@b.com";
	/// 
	/// payment.save()
	/// 
	/// var payments = pp.Payment.All();
	/// 
	/// payment = payments[0];
	/// 
	/// payment.Escrow();
	/// 
	/// payment.Release();
	/// </example>
	public class PoundPay
	{
		public readonly Payment Payment;
        public readonly Developer Developer;

        private readonly Client client;

        /// <summary>
        /// 
        /// </summary>
        public PoundPay(string developerSid, string auth, string apiUrl = null, string apiVersion = null)
        {
            client = new Client(developerSid, auth, apiUrl, apiVersion);

            Payment = new Payment(client);
            Developer = new Developer(client);
        }
	}
}
