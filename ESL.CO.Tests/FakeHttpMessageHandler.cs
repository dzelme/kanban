using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ESL.CO.Tests
{
    class FakeHttpMessageHandler : HttpClientHandler
    {
        public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return null;
        }
    }
}
