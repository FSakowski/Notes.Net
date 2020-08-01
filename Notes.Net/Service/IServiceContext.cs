using Notes.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Service
{
    public interface IServiceContext
    {
        public User CurrentUser { get; }

        public Tenant CurrentTenant { get; }

        public int GetNextFreeNumber<T>();
    }
}
