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
	/// The response returned from any :class:`~poundpay.Client` HTTP method.
	/// :class:`~poundpay.ClientResponse` has a ``response`` instance variable,
	/// representing the file-like object returned by `urllib2.urlopen <http://
	/// docs.python.org/library/urllib2.html#urllib2.urlopen>`_ and a ``data``
	/// instance variable, representing the raw data ``read()`` from the
	/// ``response``.
	/// 
	/// Sample usage::
	/// 
	///		response = urllib2.urlopen('http://google.com')
	///    client_response = ClientResponse(response, response.read())
	/// 
	///    # access the internal response object for its API
	///    assert client_response.response.getcode() == 200
	///    assert client_response.response.geturl() == 'http://google.com'
	/// 
	///    # access the internal data object for the read contents.
	///    assert 'html' in client_response.data.lower()
	/// 
	/// </summary>
	public class ClientResponse
	{
		private string data;
		private WebResponse fileLikeObject;

		public ClientResponse(WebResponse file_like_object)
		{
			this.fileLikeObject = file_like_object;

			this.data = LoadResponseData();

		}

		private string LoadResponseData()
		{
			string responseData = string.Empty;

			using (var responseReader = new StreamReader(fileLikeObject.GetResponseStream()))
			{
				responseData = responseReader.ReadToEnd();

				responseReader.Close();
			}

			return responseData;
		}

		public object Json
		{
			get
			{
				JavaScriptSerializer serializer = new JavaScriptSerializer();

				return serializer.DeserializeObject(this.data);
			}
		}
		/*

    def __init__(self, file_like_object, data):
        self.response = file_like_object
        self.data = data

    @property
    def json(self):
        """A property which decodes a JSON payload.
        Equivalent to::

           return json.loads(self.data)

        """

        return json.loads(self.data)
	*/
	}

	public class Client
	{

		/// <summary>
		/// The API URL to use for all HTTP requests
		/// </summary>
		public static string API_URL = "https://api.poundpay.com";

		/// <summary>
		/// The API version to use for all HTTP requests. PoundPay's current
		/// version can always be found at the `Developer website
		/// https://dev.poundpay.com/
		/// </summary>
		public static string API_VERSION = "silver";

		public string developerSid, authToken, apiUrl, apiVersion, authString, baseUrl;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="developerSid"></param>
		/// <param name="authToken"></param>
		/// <param name="apiUrl"></param>
		/// <param name="apiVersion"></param>
		/// <param name="openerHandlers"></param>
		public Client(string developerSid, string authToken, string apiUrl = null,
			string apiVersion = null, string openerHandlers = null)
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

		/* def __init__(self, developer_sid, auth_token, api_url=API_URL,

			 opener_handlers = opener_handlers if opener_handlers else []
			 self.opener = urllib2.build_opener(*opener_handlers)
			 authstring = base64.b64encode('%s:%s' % (developer_sid, auth_token))
			 self.opener.addheaders.append(('Authorization', 'Basic ' + authstring))
		 */

		/// <summary>
		/// Issue a ``GET /path/``. If the ``/path/`` has a resource-sid
		/// associated with it, this will return the representation of the
		/// resource located at ``/path/`` that has that associated resource-sid.
		/// </summary>
		/// <param name="path">The resource location</param>
		/// <param name="parameters">Optional parameters to urlencode and append to  ``path`` prefixed with a '?'.</param>
		/// <returns></returns>
		public ClientResponse Get(string path, NameValueCollection parameters = null)
		{
			string url = baseUrl + path;

			if (parameters != null)
			{
				url += "?" + ConstructQueryString(parameters);
			}

			var webRequest = CreateRequest(url);

			var response = new ClientResponse(webRequest.GetResponse());

			return response;
		}

		/*def get(self, path, **params):
			"""Issue a ``GET /path/``. If the ``/path/`` has a resource-sid
			associated with it, this will return the representation of the
			resource located at ``/path/`` that has that associated resource-sid.

			:param path: The resource location
			:param params: Optional parameters to `urllib.urlencode <http://docs.
			   python.org/library/urllib.html#urllib.urlencode>`_ and append to
			   ``path`` prefixed with a '?'.
			:rtype: A :class:`~poundpay.ClientResponse`.

			::

			   # issue an index on all our payments
			   client = Client('YOUR_DEVELOPER_SID', 'YOUR_AUTH_TOKEN')
			   client_response = client.get('/silver/payments/')
			   assert client_response.response.getcode() == 200
			   # gives us back a paginated response
			   payload = client_response.json
			   assert 'num_pages' in payload
			   assert 'page_size' in payload
			   assert 'payments' in payload   # will be the resource name

			   # show a resource with resource-sid PY...
			   client_response = client.get('/silver/payments/PY...')
			   assert client_response.response.getcode() == 200
			   assert isinstance(client_response.json, dict)
			   assert client_response.json['sid'] == 'PY...'

			"""
			if params:
				params = _url_encode(params)
				path = path.rstrip('/') + '/?' + params
			req = urllib2.Request(self.base_url + path)
			resp = self.opener.open(req)
			return ClientResponse(resp, resp.read())
	*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="?"></param>
		/// <returns></returns>
		public ClientResponse Post(string path, NameValueCollection parameters = null)
		{
			string url = baseUrl + path;

			var webRequest = CreateRequest(url);

			webRequest.Method = "POST";
			webRequest.ContentType = "application/x-www-form-urlencoded";

			if (parameters != null)
			{
				SendPostData(webRequest, parameters);
			}

			var response = new ClientResponse(webRequest.GetResponse());

			return response;
		}

		/*
    def post(self, path, params):
        """Issue a ``POST /path/``.

        :param path: The resource location
        :param params: The parameters to create the resource with.
        :rtype: A :class:`~poundpay.ClientResponse`.

        ::

           client = Client('YOUR_DEVELOPER_SID', 'YOUR_AUTH_TOKEN')
           data = {
              'amount': 4000,
              'payer_email_address': 'x@y.org',
              'recipient_email_address': 'bl@x.com',
              'payer_fee_amount': 100,
              'recipient_fee_amount': 0,
           }
           client_response = client.post('/silver/payments', data)
           assert client_response.response.getcode() == 201
           assert isinstance(client_response.json, dict)
           for key, value in data.iteritems():
               assert client_response.json[key] == value

        """
        data = _url_encode(params)
        req = urllib2.Request(self.base_url + path, data)
        resp = self.opener.open(req)
        return ClientResponse(resp, resp.read())
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="?"></param>
		/// <returns></returns>
		public ClientResponse Put(string path, NameValueCollection parameters = null)
		{
			string url = baseUrl + path;

			var webRequest = CreateRequest(url);

			webRequest.Method = "PUT";
			webRequest.ContentType = "application/x-www-form-urlencoded";

			if (parameters != null)
			{
				SendPostData(webRequest, parameters);
			}

			var response = new ClientResponse(webRequest.GetResponse());

			return response;
		}

		/*
			def put(self, path, params):
				"""Issue a ``PUT /path/resource-sid``.

				:param path: The resource location + the resource's sid
				:param params: The parameters to update the resource with.
				:rtype: A :class:`~poundpay.ClientResponse`.

				::

				   client = Client('YOUR_DEVELOPER_SID', 'YOUR_AUTH_TOKEN')
				   data = {'status': 'CANCELED'}
				   client_response = client.put('/silver/payments/PY...', data)
				   assert client_response.response.getcode() == 201
				   assert isinstance(client_response.json, dict)
				   assert client_response.json['status'] == 'CANCELED'

				"""

				data = _url_encode(params)
				req = urllib2.Request(self.base_url + path, data)
				req.get_method = lambda: 'PUT'
				resp = self.opener.open(req)
				return ClientResponse(resp, resp.read())
				*/
		/// <summary>
		/// Issue a ``DELETE /path/resource-sid``.
		/// </summary>
		/// <param name="path">The resource location + the resource's sid</param>
		/// <returns></returns>
		public ClientResponse Delete(string path)
		{
			string url = baseUrl + path;

			var webRequest = CreateRequest(url);

			webRequest.Method = "DELETE";
			
			var response = new ClientResponse(webRequest.GetResponse());

			return response;
		}
		/*
    def delete(self, path):
        """Issue a ``DELETE /path/resource-sid``.

        :param path: The resource location + the resource's sid
        :rtype: A :class:`~poundpay.ClientResponse`.

        ::

           client = Client('YOUR_DEVELOPER_SID', 'YOUR_AUTH_TOKEN')
           client_response = client.delete('/silver/payments/PY...')
           assert client_response.response.getcode() == 204
           assert client_response.json == {}

        """

        req = urllib2.Request(self.base_url + path)
        req.get_method = lambda: 'DELETE'
        resp = self.opener.open(req)
        return ClientResponse(resp, resp.read())

		*/

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
		/// Consider this method to be the opposite of "System.Web.HttpUtility.ParseQueryString"
		/// </summary>
		/// <param name="parameters">NameValueCollection of parameters (unencoded)</param>
		/// <returns>string like val1=%20fx&val2=abc&</returns>
		public static string ConstructQueryString(NameValueCollection parameters)
		{
			List<string> items = new List<string>();

			foreach (string name in parameters)
			{
				items.Add(string.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name])));
			}

			return string.Join("&", items.ToArray());
		}

		private static void SendPostData(HttpWebRequest webRequest, NameValueCollection parameters)
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
