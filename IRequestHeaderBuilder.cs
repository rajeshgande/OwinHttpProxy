using System.Net;
using Microsoft.Owin;

namespace OwinHttpProxy
{
    public interface IRequestHeaderBuilder
    {
        void Build(IOwinContext context, WebRequest request);
    }
}