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
    public class Resource<T> : IDictionary<string, string> where T : Resource<T>, new()
    {
        /// <summary>
        /// client is the class variable representing a <see cref="Client"/>
        /// </summary>
        internal Client client;

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

        public T Create()
        {
            var t = new T();
            t.client = this.client;

            return t;
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

        /// <summary>
        /// Represents an index of a resource by issuing a GET /resource/
        /// </summary>
        public IList<T> All(IDictionary<string, string> parameters = null)
        {
            var all = new List<T>();

            var json = client.Get(this.name, parameters).Json;

            foreach (var item in json[name] as System.Collections.ArrayList)
            {
                var res = new T();
                res.client = client;

                foreach (var kv in item as IDictionary<string, object>)
                {
                    res[kv.Key] = kv.Value == null ? string.Empty : kv.Value.ToString();
                }

                all.Add(res);
            }

            return all;
        }

        /// <summary>
        /// Represents an show of a resource by issuing a GET /resource/sid
        /// </summary>
        public T Find(string sid, IDictionary<string, string> parameters = null)
        {
            var json = client.Get(GetPath(sid), parameters).Json;

            var t = new T();
            t.client = client;

            foreach (var item in json)
            {

                t[item.Key] = item.Value == null ? string.Empty : item.Value.ToString();
            }

            return t;
        }

        /// <summary>
        /// Issues either a POST or a PUT on a resource depending
        /// if  <paramref name="parameters"/> has a sid key.
        /// </summary>
        /// <param name="parameters"></param>
        public T Save()
        {
            var hasKey = (from k in properties.Keys
                          where k == "sid"
                          select k).Count() == 1;

            IDictionary<string, object> returnable;
            if (hasKey)
            {
                returnable = this.Update(properties["sid"]);
            }
            else
            {
                returnable = this.CreateNew();
            }

            foreach (var item in returnable)
            {
                this[item.Key] = item.Value == null ? string.Empty : item.Value.ToString();
            }

            return (T)this;
        }

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
        public void Delete(string sid = null)
        {
            if (sid == null)
            {
                sid = properties["sid"];
            }

            if (sid == null)
            {
                throw new ArgumentNullException("sid");
            }

            client.Delete(GetPath(sid));
        }

        private IDictionary<string, object> Update(string sid)
        {
            return client.Put(GetPath(sid), properties).Json;
        }

        private IDictionary<string, object> CreateNew()
        {
            return client.Post(name, properties).Json;
        }

        protected string GetPath(string sid)
        {
            return string.Format("{0}/{1}", this.name, sid);
        }

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
