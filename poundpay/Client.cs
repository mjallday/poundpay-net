using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;

namespace poundpay
{
	/// <summary>
	/// The response returned from any <see cref="Client"/> HTTP method.
	/// <see cref="ClientResponse"/> has a <see cref="Response"/> property,
	/// representing the HttpWebResponse object, a <see cref="Data"/>
	/// property, representing the raw data from the <see cref="Response"/>
	/// and a <see cref="Json"/> property which will try to parse the <see cref="Data"/>
	/// property and return it as a Dictionary&lt;string,object&gt; or null
	/// if the object cannot be deserialized into Json.
	/// </summary>
	/// <example>
	/// Sample usage:
	/// <code>
	///	var webRequest = System.Net.WebRequest.Create("http://google.com") 
	///		as HttpWebRequest;
	/// clientResponse = ClientResponse(
	///		new StreamReader(webRequest.GetResponseStream()).ReadToEnd());
	/// 
	/// // access the internal response object for its API
	/// Assert.AreEqual(System.Net.HttpStatusCode.OK, clientResponse.Response.StatusCode);
	/// Assert.AreEqual(new Uri("http://google.com", clientResponse.Response.ResponseUri);
	/// 
	/// // access the internal data object for the read contents.
	/// Assert.AreNotEqual(clientResponse.Data.ToLower().IndexOf("html"), -1);
	/// </code>
	/// </example>
	public class ClientResponse
	{
		private string data;
		private HttpWebResponse response;

		public ClientResponse(HttpWebResponse response)
		{
			this.response = response;

			this.data = LoadResponseData();
		}

		private string LoadResponseData()
		{
			string responseData = string.Empty;

			using (var responseReader = new StreamReader(response.GetResponseStream()))
			{
				responseData = responseReader.ReadToEnd();

				responseReader.Close();
			}

			return responseData;
		}

		public Dictionary<string, object> Json
		{
			get
			{
				JavaScriptSerializer serializer = new JavaScriptSerializer();

				try
				{
					return serializer.Deserialize<Dictionary<string, object>>(this.data);
				}
				catch { return null; }
			}
		}

		public string Data { get { return data; } }
		public HttpWebResponse Response { get { return response; } }

	}

	/// <summary>
	/// Client is an object that is instantiated as a class
	/// variable of <see cref="Resource"/>.
	/// Instantiating <see cref="Client"/> is as simple as
	/// passing in your developer's <see cref="developerSid"/> and
	/// <see cref="authToken"/>.
	/// </summary>
	/// <example>
	/// <code>
	/// Client client = new Client('YOUR_DEVELOPER_SID', 'YOUR_AUTH_TOKEN');
	/// 
	/// ClientResponse response = client.Get("/some/resource", parameterCollection);
	/// </code>
	/// </example>
	public class Client
	{

		/// <summary>
		/// The API URL to use for all HTTP requests
		/// </summary>
		public static readonly string API_URL = "https://api.poundpay.com";

		/// <summary>
		/// The API version to use for all HTTP requests. PoundPay's current
		/// version can always be found at the `Developer website
		/// https://dev.poundpay.com/
		/// </summary>
		public static readonly string API_VERSION = "silver";

		public string developerSid, authToken, apiUrl, apiVersion, authString, baseUrl;

		/// <summary>
		/// Creates a new <see cref="Client"/> instance which will return instances
		/// of <see cref="ClientResource"/> when issued a GET, POST, PUT or DELETE 
		/// command.
		/// </summary>
		/// <param name="developerSid"></param>
		/// <param name="authToken"></param>
		/// <param name="apiUrl"></param>
		/// <param name="apiVersion"></param>
		public Client(string developerSid, string authToken, string apiUrl = null,
			string apiVersion = null)
		{
			if (string.IsNullOrEmpty(developerSid) || string.IsNullOrEmpty(authToken))
			{
				throw new ArgumentException("developerSid and authToken required");
			}

			if (!developerSid.StartsWith("DV"))
			{
				throw new ArgumentException("developerSid must start with DV");
			}

			this.developerSid = developerSid;
			this.authToken = authToken;
			this.apiUrl = apiUrl ?? API_URL;
			this.apiVersion = apiVersion ?? API_VERSION;

			baseUrl = string.Format("{0}/{1}/", this.apiUrl, this.apiVersion);

			authString = Convert.ToBase64String(
				Encoding.UTF8.GetBytes(
					string.Format("{0}:{1}", this.developerSid, this.authToken)));


		}

		/// <summary>
		/// Issue a GET to <paramref name="path"/>. If the <paramref name="path"/> has a resource-sid
		/// associated with it, this will return the representation of the
		/// resource located at <paramref name="path"/> that has that associated resource-sid.
		/// </summary>
		/// <param name="path">The resource location</param>
		/// <param name="parameters">Optional parameters to urlencode and append to 
		/// <paramref name="path"/> prefixed with a '?'.</param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// var client = Client('YOUR_DEVELOPER_SID', 'YOUR_AUTH_TOKEN');
		/// var clientResponse = client.Get("payments/PY...");
		/// Assert.AreEqual(System.Net.HttpStatusCode.OK, clientResponse.Response.StatusCode);
		/// Assert.IsTrue(clientResponse.Json.ContainsKey("num_pages"));
		/// Assert.IsTrue(clientResponse.Json.ContainsKey("page_size"));
		/// Assert.IsTrue(clientResponse.Json.ContainsKey("payments"));
		/// Assert.IsTrue(clientResponse.Json["sid"].ToString().StartsWith("PY"));
		/// </code>
		/// </example>
		public ClientResponse Get(string path, IDictionary<string, string> parameters = null)
		{
			string url = baseUrl + path;

			if (parameters != null)
			{
				url += "?" + ConstructQueryString(parameters);
			}

			var webRequest = CreateRequest(url);

			var response = new ClientResponse(webRequest.GetResponse() as HttpWebResponse);

			return response;
		}

		/// <summary>
		/// Issue a POST to  <paramref name="path"/>
		/// </summary>
		/// <param name="path">The resource location</param>
		/// <param name="?">The parameters to create the resource with.</param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// var client = Client('YOUR_DEVELOPER_SID', 'YOUR_AUTH_TOKEN');
		/// var parameters = new Dictionary&lt;string, string&gt;() { 
		///       { "amount", "4000" }, 
		///       { "payer_email_address", "x@y.org" } 
		///       { "recipient_email_address", "bl@x.com" }, 
		///       { "payer_fee_amount", "100" } 
		///       { "recipient_fee_amount", "0" }, 
		/// };
		/// var clientResponse = client.Post('payments', parameters);
		/// Assert.AreEqual(System.Net.HttpStatusCode.CREATED, clientResponse.Response.StatusCode);
		/// </code>
		/// </example>
		public ClientResponse Post(string path, IDictionary<string, string> parameters = null)
		{
			string url = baseUrl + path;

			var webRequest = CreateRequest(url);

			webRequest.Method = "POST";
			webRequest.ContentType = "application/x-www-form-urlencoded";

			if (parameters != null)
			{
				SendPostData(webRequest, parameters);
			}

			var response = new ClientResponse(webRequest.GetResponse() as HttpWebResponse);

			return response;
		}

		/// <summary>
		/// Issue a PUT to <paramref name="path"/>
		/// </summary>
		/// <param name="path">The resource location + the resource's sid</param>
		/// <param name="parameters">The parameters to update the resource with.</param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// var client = Client('YOUR_DEVELOPER_SID', 'YOUR_AUTH_TOKEN');
		/// var parameters = new Dictionary&lt;string, string&gt;() { 
		///       { "status", "CANCELED" }, 
		/// };
		/// var clientResponse = client.Put('payments/PY...', parameters);
		/// Assert.AreEqual("CANCELLED", clientResponse.Json["status"]);
		/// Assert.AreEqual(System.Net.HttpStatusCode.Created, clientResponse.Response.StatusCode);
		/// </code>
		/// </example>
		public ClientResponse Put(string path, IDictionary<string, string> parameters = null)
		{
			string url = baseUrl + path;

			var webRequest = CreateRequest(url);

			webRequest.Method = "PUT";
			webRequest.ContentType = "application/x-www-form-urlencoded";

			if (parameters != null)
			{
				SendPostData(webRequest, parameters);
			}

			var response = new ClientResponse(webRequest.GetResponse() as HttpWebResponse);

			return response;
		}

		/// <summary>
		/// Issue a DELETE to <paramref name="path"/>
		/// </summary>
		/// <param name="path">The resource location + the resource's sid</param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// var client = Client('YOUR_DEVELOPER_SID', 'YOUR_AUTH_TOKEN');
		/// var clientResponse = client.Delete('payments/PY...');
		/// Assert.AreEqual(System.Net.HttpStatusCode.NoContent, clientResponse.Response.StatusCode);
		/// </code>
		/// </example>
		public ClientResponse Delete(string path)
		{
			string url = baseUrl + path;

			var webRequest = CreateRequest(url);

			webRequest.Method = "DELETE";

			var response = new ClientResponse(webRequest.GetResponse() as HttpWebResponse);

			return response;
		}

		private HttpWebRequest CreateRequest(string url)
		{
			var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;

			webRequest.Method = "GET";
			webRequest.Headers.Add("Authorization", "Basic " + authString);
			webRequest.UserAgent = "poundpay-net client";

			return webRequest;
		}

		/// <summary>
		/// Constructs a QueryString (string).
		/// Consider this method to be the opposite 
		/// of <see cref="System.Web.HttpUtility.ParseQueryString"/>
		/// </summary>
		/// <param name="parameters">IDictionary&lt;string, string&gt; of parameters (unencoded)</param>
		/// <returns>string like val1=%20fx&val2=abc&</returns>
		public static string ConstructQueryString(IDictionary<string, string> parameters)
		{
			List<string> items = new List<string>();

			foreach (var name in parameters)
			{
				if (!string.IsNullOrEmpty(name.Value))
				{
					items.Add(string.Concat(name.Key, "=", System.Web.HttpUtility.UrlEncode(name.Value)));
				}
			}

			return string.Join("&", items.ToArray());
		}

		private static void SendPostData(HttpWebRequest webRequest,
			IDictionary<string, string> parameters)
		{
			string postData = ConstructQueryString(parameters);

			//POST the data.
			using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
			{
				try
				{
					requestWriter.Write(postData);
				}
				catch
				{
					throw;
				}
				finally
				{
					requestWriter.Close();
				}
			}
		}
	}
}
