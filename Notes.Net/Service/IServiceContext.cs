using Notes.Net.Models;
using System.Threading.Tasks;

namespace Notes.Net.Service
{
    public interface IServiceContext
    {
        public Task<User> GetCurrentUser();
    }
}
