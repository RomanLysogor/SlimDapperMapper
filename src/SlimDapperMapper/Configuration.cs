using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseMapper
{
    public static class Configuration
    {
        public delegate string LookupFieldIdentification(Type t);

        public static void RegisterLookupConvention<T>(LookupFieldIdentification lookupResolver)
        {
            if (!LookupConventions.ContainsKey(typeof(T)))
            {
                LookupConventions.Add(typeof(T), lookupResolver);
            }
            else
            {
                LookupConventions[typeof(T)] = lookupResolver;
            }
        }

        internal static readonly Dictionary<Type, LookupFieldIdentification> LookupConventions = new Dictionary<Type, LookupFieldIdentification>();
    }
}
