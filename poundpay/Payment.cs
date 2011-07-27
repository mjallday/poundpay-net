using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace poundpay
{
	public class PaymentError : Exception
	{
		private string message;
		protected const string ErrorFormat = "Payment status is {0}. Only {1} payments may be {2}";

		public PaymentError() { }
		public PaymentError(string message) { }
		public PaymentError(string first, string second, string third) {
			message = string.Format(ErrorFormat, first, second, third);
		}
	}
	public class PaymentEscrowError : PaymentError
	{
		public PaymentEscrowError(string first, string second, string third) : base(first, second, third) { }
	}
	public class PaymentReleaseError : PaymentError
	{
		public PaymentReleaseError(string first, string second, string third) : base(first, second, third) { }
	}
	public class PaymentCancelError : PaymentError
	{
		public PaymentCancelError(string first, string second, string third) : base(first, second, third) { }
	}


	/// <summary>
	/// The Payment <see cref="Resource"/>, represented by a RESTful resource located at
	/// /payments
	/// </summary>
	public class Payment : Resource
	{
		internal Payment(Client client)
			: base(client, "payments") { }

		public Payment Create()
		{
			return new Payment(this.client);
		}

		/// <summary>
		/// Retrieves the list of past versions of the payment.
		/// Each item in the list is itself a Payment object
		/// </summary>
		public List<Payment> Versions()
		{
			//	i don't think this is quite what we want to do
			throw new NotImplementedException();

			string sid;

			try
			{
				sid = properties["sid"];
			}
			catch (KeyNotFoundException)
			{
				throw new PaymentError("Sid not specified");
			}

			var jsonData = this.client.Get(GetPath(sid) + "?versions=all").Json["payments"] as List<Dictionary<string, object>>;

			List<Payment> payments = new List<Payment>();

			foreach (var item in jsonData)
			{
				Payment p = new Payment(this.client);

				foreach (var key in item.Keys)
				{
					p[key] = item[key].ToString();
				}

				payments.Add(p);
			}

			return payments;
		}
		/* def versions(self):
        """Retrieves the list of past versions of the payment.
        
        Each item in the list is itself a Payment object.
        """
        cls = self.__class__
        
        versions = cls.client.get(cls._get_path(self.sid) + '?versions=all')
        versions_json = versions.json['payments']
        
        return [cls(**version) for version in versions_json]
    */
		/// <summary>
		/// Escrows an ``AUTHORIZED`` payment by charging the authorized
		/// payment method associated with the ``AUTHORIZED`` payment
		/// </summary>
		/// <remarks>
		/// <see cref="PaymentEscrowError"/> is thrown if the
		/// <see cref="Payment"/>'s status is not ``AUTHORIZED``
		/// </remarks>
		public void Escrow()
		{
			if (this["status"] != "AUTHORIZED")
			{
				throw new PaymentEscrowError(this["status"], "AUTHORIZED", "ESCROWED");
			}
			this["status"] = "ESCROWED";
			this.Save();
		}

		/// <summary>
		/// Releases an ``ESCROWED`` payment by paying out the funds to
		/// a PoundPay account.
		/// </summary>
		/// <remarks>
		/// <see cref="PaymentReleaseError"/> is thrown if the
		/// <see cref="Payment"/>'s status is not ``ESCROWED``
		/// </remarks>
		public void Release()
		{
			if (this["status"] != "ESCROWED")
			{
				throw new PaymentReleaseError(this["status"], "ESCROWED", "RELEASED");
			}
			this["status"] = "RELEASED";
			this.Save();
		}

		/// <summary>
		/// Cancels an ``ESCROWED`` payment by refunding the payer. All
		/// PoundPay fees are refunded back to the developer, as well.
		/// </summary>
		/// <remarks>
		/// <see cref="PaymentCancelError"/> is thrown if the
		/// <see cref="Payment"/>'s status is not ``ESCROWED``
		/// </remarks>
		public void Cancel()
		{
			if (this["status"] != "ESCROWED")
			{
				throw new PaymentCancelError(this["status"], "ESCROWED", "CANCELED");
			}
			this["status"] = "CANCELED";
			this.Save();
		}
	}
}
