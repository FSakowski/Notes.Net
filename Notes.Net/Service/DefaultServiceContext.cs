using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Notes.Net.Models;
using System.Threading.Tasks;

namespace Notes.Net.Service
{
    public class DefaultServiceContext : IServiceContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<User> userManager;

        public DefaultServiceContext(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<User> GetCurrentUser()
        {
            return await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        }
    }
}
