using System;
using System.Collections.Generic;

namespace SlimDapperMapper
{
    public static class Configuration
    {
        public delegate string LookupFieldIdentification(Type t);

        public static void RegisterLookupConvention<T>(LookupFieldIdentification lookupResolver)
        {
            LookupConventions[typeof(T)] = lookupResolver;
        }

        internal static readonly Dictionary<Type, LookupFieldIdentification> LookupConventions = new Dictionary<Type, LookupFieldIdentification>();
    }
}
