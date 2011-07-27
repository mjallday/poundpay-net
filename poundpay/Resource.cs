using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace poundpay
{
	/// <summary>
	/// Class that represents a RESTful resource at a particular endpoint.
	/// Has a class variable, <see cref="Client"/>, that is defaulted to
	/// null. Once configured, there are standard operators that are
	/// enabled on any resource.
	/// </summary>
	public class Resource : IDictionary<string, string>
	{
		/// <summary>
		/// client is the class variable representing a <see cref="Client"/>
		/// </summary>
		protected Client client;

		/// <summary>
		/// name is the pluralized name of a resource represented by a descendant
		/// of <see cref="Resource"/>.
		/// </summary>
		protected readonly string name;

		protected readonly IDictionary<string, string> properties;

		protected Resource(Client client, string name)
		{
			this.client = client;
			this.name = name;
			this.properties = new Dictionary<string, string>();
		}

		public string this[string key]
		{
			get
			{
				return this.properties[key];
			}
			set
			{
				this.properties[key] = value;
			}
		}

		public string __repr__()
		{

			throw new NotImplementedException();
			// TODO: Reflect over this object to find all variables
			string attrs = string.Join(",", null);

			//attrs = ', '.join(['%s=%s' % (k, repr(v)) for k, v in
			//               self.__dict__.iteritems()])

			return string.Format("{0}({1})", this.GetType(), attrs);
		}

		/// <summary>
		/// Represents an index of a resource by issuing a GET /resource/
		/// </summary>
		public ClientResponse All(IDictionary<string, string> parameters = null)
		{
			return client.Get(this.name, parameters);
		}

		/*
	@classmethod
	def all(cls, **params):
		"""Represents an index of a resource by issuing a GET /resource/``

		:param params: Optional parameters to `urllib.urlencode <http://docs.
		   python.org/library/urllib.html#urllib.urlencode>`_ and append to
		   ``/resource/`` prefixed with a '?'.
		:rtype: A list of :class:`~poundpay.Resource` descendants,
		   represented by ``cls``.

		Sample Usage::

		   import poundpay
		   poundpay.configure('DEVELOPER_SID', 'AUTH_TOKEN')

		   # paginated fetch of all associated payments
		   # equivalent of GET /silver/payments/
		   poundpay.Payment.all()

		   # fetch by offset and give me 5 results
		   # equivalent of GET /silver/payments/?offset=10&limit=5
		   poundpay.Payment.all(offset=10, limit=5)

		"""
		resp = cls.client.get(cls._name, **params)
		return [cls(**attrs) for attrs in resp.json[cls._name]]
		*/


		/// <summary>
		/// Represents an index of a resource by issuing a GET /resource/sid
		/// </summary>
		public ClientResponse Find(string sid, IDictionary<string, string> parameters = null)
		{
			return client.Get(GetPath(sid), parameters);
		}

		/*
	@classmethod
	def find(cls, sid, **params):
		"""Represents an show of a resource by issuing a
		``GET /resource/sid``

		:param sid: Represents the identifier of a resource
		:param params: Optional parameters to `urllib.urlencode <http://docs.
		   python.org/library/urllib.html#urllib.urlencode>`_ and append to
		   ``/resource/sid`` prefixed with a '?'.
		:rtype: A :class:`~poundpay.Resource` descendant, represented
		   by ``cls``.

		Sample Usage::

		   import poundpay
		   poundpay.configure('DEVELOPER_SID', 'AUTH_TOKEN')

		   # paginated fetch of all associated payments
		   # equivalent of GET /silver/payments/PY...
		   poundpay.Payment.find('PY...')

		"""
		resp = cls.client.get(cls._get_path(sid), **params)
		return cls(**resp.json)
		
		*/

		/// <summary>
		/// Issues either a POST or a PU` on a resource depending
		/// if  <paramref name="parameters"/> has an sid key.
		/// </summary>
		/// <param name="parameters"></param>
		public Resource Save()
		{
			var hasKey = (from k in properties.Keys
						  where k == "sid"
						  select k).Count() == 1;

			if (hasKey)
			{
				this.Update(properties["key"]);
			}
			else
			{
				this.Create();
			}

			return this;
		}

		/*
	def save(self):
		"""Issues either a ``POST`` or a ``PUT`` on a resource depending
		if has a ``sid`` or an ``id``.

		:rtype: A :class:`~poundpay.Resource` descendant, represented
		   by ``cls``.

		Sample Usage::

		   import poundpay
		   poundpay.configure('DEVELOPER_SID', 'AUTH_TOKEN')

		   # paginated fetch of all associated payments
		   # equivalent of GET /silver/payments/
		   data = {
			  'amount': 4000,
			  'payer_email_address': 'x@y.org',
			  'recipient_email_address': 'bl@x.com',
			  'payer_fee_amount': 100,
			  'recipient_fee_amount': 0,
		   }
		   payment = poundpay.Payment(**data)
		   payment.save()   # issues POST /silver/payments
		   assert payment.sid.startswith('PY')
		   payment.status = 'CANCELED'
		   # because payment already has a sid
		   payment.save()   # issues PUT /silver/payments

		"""
		if hasattr(self, 'sid'):
			attrs = self._update(**self.__dict__)
		else:
			attrs = self._create(**self.__dict__)
		self.__dict__.update(attrs)
		return self
		
		*/

		/// <summary>
		/// Issues a DELETE on a resource.
		/// </summary>
		/// <example>
		/// <code>
		/// var pp = new PoundPay("DEVELOPER_SID", "AUTH_TOKEN");
		/// var payment = pp.Payment.find("PY...");
		/// payment.Delete();
		/// payment = pp.Payment.find("PY...");
		///	
		/// </code>
		/// </example>
		public void Delete(string sid)
		{
			client.Delete(GetPath(sid));
		}
		/*
	def delete(self):
		"""Issues a ``DELETE`` on a resource.

		:rtype: None

		Sample Usage::

		   import poundpay
		   poundpay.configure('DEVELOPER_SID', 'AUTH_TOKEN')
		   payment = poundpay.Payment.find('PY...')
		   payment.delete()   # issues a DELETE /silver/payments/PY...
		   payment = poundpay.Payment.find('PY...')
		   assert payment.response.getcode() == 404

		"""
		self.client.delete(self._get_path(self.sid))
		
		*/

		private IDictionary<string, object> Update(string sid)
		{
			return client.Put(GetPath(sid), properties).Json;
		}

		private IDictionary<string, object> Create()
		{
			return client.Post(name, properties).Json;
		}

		protected string GetPath(string sid)
		{
			return string.Format("{0}/{1}", this.name, sid);
		}

		/*
	@classmethod
	def _get_path(cls, sid):
		return '%s/%s' % (cls._name, sid)

	}
		 * */

		#region IDictionary<string,string> Members

		public void Add(string key, string value)
		{
			properties.Add(key, value);
		}

		public bool ContainsKey(string key)
		{
			return properties.ContainsKey(key);
		}

		public ICollection<string> Keys
		{
			get { return properties.Keys; }
		}

		public bool Remove(string key)
		{
			return properties.Remove(key);
		}

		public bool TryGetValue(string key, out string value)
		{
			return properties.TryGetValue(key, out value);
		}

		public ICollection<string> Values
		{
			get { return properties.Values; }
		}

		#endregion

		#region ICollection<KeyValuePair<string,string>> Members

		public void Add(KeyValuePair<string, string> item)
		{
			properties.Add(item);
		}

		public void Clear()
		{
			properties.Clear();
		}

		public bool Contains(KeyValuePair<string, string> item)
		{
			return properties.Contains(item);
		}

		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			properties.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return properties.Count;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<string, string> item)
		{
			return properties.Remove(item);
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,string>> Members

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return properties.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return properties.GetEnumerator();
		}

		#endregion
	}
}
