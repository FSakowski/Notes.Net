using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Notes.Net.Infrastructure
{
    public class SecurityPolicyMiddleware
    {
        private readonly RequestDelegate next;

        public string Policy {
            get;
            private set;
        }

        public SecurityPolicyMiddleware(RequestDelegate next, string policy)
        {
            this.next = next;
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add(
                "Content-Security-Policy",
                Policy);

            await next.Invoke(context);
        }
    }

    public static class SecurityPolicy
    {
        public static void UseCSP(this IApplicationBuilder app, string policy)
        {
            app.UseMiddleware<SecurityPolicyMiddleware>(policy);
        }
    }
}
