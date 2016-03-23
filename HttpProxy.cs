using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace OwinHttpProxy
{
    public class HttpProxy : OwinMiddleware
    {
        private readonly HttpProxydOptions _options;
        private readonly IRequestBodyBuilder _bodyBuilder;
        private readonly IRequestHeaderBuilder _headerBuilder;

        public HttpProxy(OwinMiddleware next, HttpProxydOptions options, IRequestBodyBuilder bodyBuilder, IRequestHeaderBuilder headerBuilder) : base(next)
        {
            _options = options;
            _bodyBuilder = bodyBuilder;
            _headerBuilder = headerBuilder;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.StartsWithSegments(_options.ProxyPathEndpoint))
            {
                if (!context.Request.Headers.ContainsKey(_options.TargetUrlHeaderName))
                {
                    throw new InvalidOperationException($"'{_options.TargetUrlHeaderName}' header not found");
                }

                string targeturl = context.Request.Headers.FirstOrDefault(x => String.Compare(x.Key, _options.TargetUrlHeaderName, StringComparison.OrdinalIgnoreCase) == 0).Value.FirstOrDefault();

                WebRequest request = WebRequest.Create(targeturl);
                request.Method = context.Request.Method;
                request.ContentType = context.Request.Headers.FirstOrDefault(x => String.Compare(x.Key, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0).Value.FirstOrDefault();

                _headerBuilder.Build(context, request);
                await _bodyBuilder.Build(context, request);

                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        context.Response.StatusCode = (int)response.StatusCode;
                        if (response.ContentLength != 0)
                        {
                            await response.GetResponseStream().CopyToAsync(context.Response.Body);
                        }
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        context.Response.StatusCode = (int)(ex.Response as HttpWebResponse).StatusCode;
                        await ex.Response.GetResponseStream().CopyToAsync(context.Response.Body);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                await Next.Invoke(context);
            }
        }
        
    }
}
