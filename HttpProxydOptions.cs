using System.Collections.Generic;
using Microsoft.Owin;

namespace OwinHttpProxy
{
    public class HttpProxydOptions
    {
        public PathString ProxyPathEndpoint { get; set; }

        public string TargetUrlHeaderName { get; set; }

        public IEnumerable<string> HeaderExlustionList { get; set; }
    }
}