using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace OwinHttpProxy
{
    public class RequestBodyBuilder : IRequestBodyBuilder
    {
        public async Task Build(IOwinContext context, WebRequest request)
        {
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                using (var stream = new MemoryStream())
                {
                    await context.Request.Body.CopyToAsync(stream);
                    var bytearray = stream.GetBuffer();
                    request.ContentLength = bytearray.Length;
                    using (Stream dataStream = request.GetRequestStream())
                    {
                        await dataStream.WriteAsync(bytearray, 0, bytearray.Length);
                    }
                }
            }
        }
    }
}