using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace poundpay
{
    /// <summary>
    /// 
    /// </summary>
    public class Developer:Resource<Developer>
    {
        public Developer() : base(null, "developers") { }
        internal Developer(Client client) : base(client, "developers") { }

        /// <summary>
        /// Issue a GET /developers/&lt;developer_sid&gt;
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public Developer FindMe()
        {
            return base.Find(client.developerSid);
        }
    }
}
