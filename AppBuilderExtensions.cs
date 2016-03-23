using Microsoft.Owin;
using Owin;

namespace OwinHttpProxy
{
    public static class AppBuilderExtensions
    {
        public static void UseHttpProxy(
            this IAppBuilder app, HttpProxydOptions options = null)
        {
            options = options ?? new HttpProxydOptions();

            if (!options.ProxyPathEndpoint.HasValue)
                options.ProxyPathEndpoint = new PathString("/proxy");

            if (string.IsNullOrEmpty(options.TargetUrlHeaderName))
                options.TargetUrlHeaderName = "x-targeturl";

            app.Use<HttpProxy>(options, new RequestBodyBuilder(), new RequestHeaderBuilder());
        }
    }
}