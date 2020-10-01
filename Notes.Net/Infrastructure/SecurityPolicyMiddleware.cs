using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Notes.Net.Infrastructure
{
    public class SecurityPolicyMiddleware
    {
        private readonly RequestDelegate next;

        public SecurityPolicyMiddleware(RequestDelegate next) => this.next = next;

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add(
                "Content-Security-Policy",
                "default-src 'self' *.fontawesome.com; img-src * data:; media-src *; style-src 'unsafe-inline' 'self'");

            await next.Invoke(context);
        }
    }
}
