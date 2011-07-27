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
	public class ClientTests
	{
		string developerSid, authToken, apiUrl, apiVersion;
		/// <summary>
		/// PoundPay p = new PoundPay(DEVELOPER, AUTH);
		/// 
		/// var jsonData = p.Payments.find(params);
		/// 
		/// </summary>
		public ClientTests()
		{
			developerSid = System.Configuration.ConfigurationManager.AppSettings["sid"];
			authToken = System.Configuration.ConfigurationManager.AppSettings["token"];
			apiUrl = System.Configuration.ConfigurationManager.AppSettings["api_url"];
			apiVersion = System.Configuration.ConfigurationManager.AppSettings["version"];

			developerSid = "DV0383d447360511e0bbac00264a09ff3c";
			authToken = "c31155b9f944d7aed204bdb2a253fef13b4fdcc6ae15402004";
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
		public void test_default_api_url_when_explicity_set_to_None()
		{
			apiUrl = null;

			var client = new Client(developerSid, authToken, apiUrl, apiVersion);

			Assert.IsFalse(String.IsNullOrEmpty(client.baseUrl));
		}
		[TestMethod]
		public void test_default_url_and_version()
		{

			var client = new Client(developerSid, authToken, apiUrl, apiVersion);
			Assert.AreEqual(client.baseUrl.Replace("-sandbox", string.Empty),
						 "https://api.poundpay.com/silver/");
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
		public void test_different_url_and_version()
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

		[TestMethod]
		public void test_get()
		{
			//	hacks while we're testing without a poundpay dev account
			apiUrl = "http://github.com";
			apiVersion = "api/v2";

			string getFrom = "json/commits/list/poundpay/poundpay-python/master";

			var client = new Client(developerSid, authToken, apiUrl, apiVersion);

			var clientResponse = client.Get(getFrom);

			var parameters = new NameValueCollection() { { "string", "value" }, { "string2", "value2" } };

			Assert.AreEqual(System.Net.HttpStatusCode.OK, clientResponse.Response.StatusCode);
			Assert.AreEqual(new Uri(apiUrl + "/" + apiVersion + "/" + getFrom),
				clientResponse.Response.ResponseUri);


			
		}

		/*
		def test_get(self):
			client = Client(**self.production_config)
			resp_dict = {'foo': 'bar'}
			mock_open = mock.Mock()
			mock_open.return_value.read.return_value = json.dumps(resp_dict)
			with mock.patch.object(client.opener,
								   'open',
								   mock_open,
								   mocksignature=True):
				resp = client.get('payments')
			self.assertEqual(resp.json, resp_dict)
			mock_open.return_value.read.assert_called_once_with()
			*/
		[TestMethod, Ignore]
		public void test_post()
		{
			throw new NotImplementedException();
		}

		/*
		def test_post(self):
			client = Client(**self.production_config)
			resp_body_dict = {'foo': 'bar'}
			mock_open = mock.mocksignature(client.opener.open)
			mock_resp = mock.Mock()
			mock_resp.read = mock.Mock(return_value=json.dumps(resp_body_dict))
			mock_open.mock.return_value = mock_resp
			with mock.patch.object(client.opener, 'open', new=mock_open):
				resp = client.post('payments', resp_body_dict)
			self.assertEqual(resp.json, resp_body_dict)
			*/

		[TestMethod, Ignore]
		public void test_delete()
		{
			throw new NotImplementedException();
		}

		/*
		def test_delete(self):
			client = Client(**self.production_config)
			mock_open = mock.mocksignature(client.opener.open)
			mock_resp = mock.Mock()
			mock_resp.read = mock.Mock(return_value=json.dumps({}))
			mock_open.mock.return_value = mock_resp
			with mock.patch.object(client.opener, 'open', new=mock_open):
				resp = client.delete('payments/sid')
			self.assertEqual(resp.json, {})
			*/

		[TestMethod, Ignore]
		public void test_put()
		{
			throw new NotImplementedException();
		}

		/*
		def test_put(self):
			client = Client(**self.production_config)
			resp_body_dict = {'foo': 'bar'}
			mock_open = mock.mocksignature(client.opener.open)
			mock_resp = mock.Mock()
			mock_resp.read = mock.Mock(return_value=json.dumps(resp_body_dict))
			mock_open.mock.return_value = mock_resp
			with mock.patch.object(client.opener, 'open', new=mock_open):
				resp = client.put('payments/sid', resp_body_dict)
			self.assertEqual(resp.json, resp_body_dict)
			*/

	}
}
