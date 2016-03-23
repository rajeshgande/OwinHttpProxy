using System.Linq;
using System.Net;
using Microsoft.Owin;

namespace OwinHttpProxy
{
    public class RequestHeaderBuilder : IRequestHeaderBuilder
    {
        public void Build(IOwinContext context, WebRequest request)
        {
            string[] headerfilterlist = new[] { "x-targeturl", "Content-Type", "Connection", "Accept", "Host", "User-Agent", "Content-Length" };
            foreach (var header in context.Request.Headers)
            {
                if (headerfilterlist.Contains(header.Key))
                    continue;
                foreach (var value in header.Value)
                    request.Headers.Add(header.Key, value);
            }
        }

    }
}