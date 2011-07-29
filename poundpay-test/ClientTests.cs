using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using poundpay;
using System.Collections.Specialized;

namespace poundpay_test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class ClientTests:TestBase
	{
		/// <summary>
		/// PoundPay p = new PoundPay(DEVELOPER, AUTH);
		/// 
		/// var jsonData = p.Payments.Find(params);
		/// 
		/// </summary>
		public ClientTests()
		{
            base.Init();
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void TestClientValidation()
		{
			Client cr;

			bool hasException = false;
			try
			{
				cr = new Client(string.Empty, string.Empty);
			}
			catch (ArgumentException) { hasException = true; }

			Assert.IsTrue(hasException);

			hasException = false;
			try
			{
				cr = new Client(developerSid, string.Empty);
			}
			catch (ArgumentException) { hasException = true; }

			Assert.IsTrue(hasException);

			hasException = false;
			try
			{
				cr = new Client(string.Empty, authToken);
			}
			catch (ArgumentException) { hasException = true; }

			Assert.IsTrue(hasException);

			hasException = false;

			hasException = false;

			cr = new Client(developerSid, authToken);

		}

		[TestMethod]
		public void TestDeveloperSidValidation()
		{
			bool hasException = false;
			try
			{
				var cr = new Client("AB123123123", authToken);
			}
			catch (ArgumentException) { hasException = true; }

			Assert.IsTrue(hasException);
		}

		[TestMethod]
		public void TestDefaultApiVersionWhenExplicitlySetToNone()
		{
			apiVersion = null;

			var client = new Client(developerSid, authToken, apiUrl, apiVersion);

			Assert.IsTrue(client.baseUrl.EndsWith(Client.API_VERSION + "/"),
				client.baseUrl + " does not end with " + Client.API_VERSION + "/");
		}
		[TestMethod]
		public void TestDefaultApiUrlWhenSetToNull()
		{
			apiUrl = null;

			var client = new Client(developerSid, authToken, apiUrl, apiVersion);

			Assert.IsFalse(String.IsNullOrEmpty(client.baseUrl));
		}
		[TestMethod]
		public void TestDefaultUrlAndVersion()
		{
			var client = new Client(developerSid, authToken, apiUrl, apiVersion);
			Assert.AreEqual(client.baseUrl,
						 apiUrl + "/" + apiVersion + "/");
		}

		[TestMethod]
		public void TestAuthorization()
		{
			var client = new Client(developerSid, authToken, apiUrl, apiVersion);

			string authString = Encoding.UTF8.GetString(Convert.FromBase64String(client.authString));

			string[] authParts = authString.Split(new char[] { ':' });

			Assert.AreEqual(authParts[0], developerSid);
			Assert.AreEqual(authParts[1], authToken);

		}

		[TestMethod]
		public void TestDifferentUrlAndVersionS()
		{
			apiUrl = "https://www.reddit.com";
			apiVersion = "gold";

			var client = new Client(developerSid, authToken, apiUrl, apiVersion);

			Assert.AreEqual(apiUrl + "/" + apiVersion + "/", client.baseUrl);
		}

		[TestMethod]
		public void TestJsonSerializing()
		{
			var dict = new Dictionary<string, string>() { { "foo", "bar" } };

			var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

			var serialized = serializer.Serialize(dict);

			Assert.AreEqual("{\"foo\":\"bar\"}", serialized, serialized);

			var deserialized = serializer.Deserialize<Dictionary<string, string>>(serialized);

			Assert.AreEqual(deserialized["foo"], "bar");
		}
	}
}
