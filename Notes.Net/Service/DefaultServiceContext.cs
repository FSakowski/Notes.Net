using Notes.Net.Models;
using System;
using System.Collections.Generic;

namespace Notes.Net.Service
{
    public class DefaultServiceContext : IServiceContext
    {
        private static readonly Dictionary<Type, int> _numbers = new Dictionary<Type, int>();

        public Tenant CurrentTenant { get; private set; }

        public User CurrentUser => new User { UserId = 1, Name = "Florian", Admin = true };

        public DefaultServiceContext(Tenant tenant)
        {
            CurrentTenant = tenant;
        }

        public int GetNextFreeNumber<T>()
        {
            int no = 1;

            if (_numbers.ContainsKey(typeof(T)))
                no = _numbers[typeof(T)]++;

            _numbers[typeof(T)] = no;

            return no;
        }
    }
}
