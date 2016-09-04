using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SlimDapperMapper
{
    internal static class HelperFunctions
    {
        public static FieldInfo[] GetAllPublicNonGenericFields(Type type)
        {
            return type.GetFields().Where(t => !t.FieldType.IsGenericParameter).ToArray();
        }

        public static void SetFieldValue(FieldInfo fieldInfo, object fieldValue, object targetObject)
        {
            if (targetObject == null)
            {
                throw new ArgumentNullException($"target object was null during SetField for [{fieldInfo.FieldType}] fieldName: [{fieldInfo.Name}]");
            }

            fieldInfo.SetValue(targetObject, fieldValue);
        }

        public static TResult CreateInstanceAndMatchFields<TResult>(Type type, FieldInfo[] fields, IDictionary<string, object> row, Configuration.LookupFieldIdentification lookupIdentifier)
        {
            var instance = CreateInstance<TResult>(type);
            var identificationField = lookupIdentifier(type);

            if (row[identificationField] == null)
            {
                return default(TResult);
            }

            foreach (var field in fields)
            {
                if (row.ContainsKey(field.Name) && row[field.Name] != null)
                {
                    var value = ConvertToTargetType(field, row[field.Name]);
                    SetFieldValue(field, value, instance);
                }
            }

            return instance;
        }

        private static bool HasDefaultValue(Type type, object value)
        {
            var defaultValue = Activator.CreateInstance(type);
            return defaultValue.Equals(value);
        }

        private static object ConvertToTargetType(FieldInfo fi, object value)
        {
            object converted = value;

            // Add support for more cases if necessary

            if (fi.FieldType.Name.Equals("Int32", StringComparison.OrdinalIgnoreCase))
            {
                converted = Convert.ToInt32(value);
            }

            return converted;
        }

        private static T CreateInstance<T>(Type type)
        {
            return (T)Activator.CreateInstance(type);
        }
    }
}
