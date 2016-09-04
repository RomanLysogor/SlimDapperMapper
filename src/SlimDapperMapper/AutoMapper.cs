using System;
using System.Collections.Generic;
using System.Linq;

namespace SlimDapperMapper
{
    public static class AutoMapper
    {
        public static List<TResult> Map<TResult, TFirst, TSecond, TThird>(IList<IDictionary<string, object>> queryRows, Action<TResult, TFirst, TSecond, TThird, IDictionary<object, TResult>> lookupResolver)
        {
            Type resultType = typeof(TResult);
            Type firstType = typeof(TFirst);
            Type secondType = typeof(TSecond);
            Type thirdType = typeof(TThird);

            if (!Configuration.LookupConventions.ContainsKey(resultType))
            {
                throw new ArgumentException($"There is no registered LookupFieldIdentification for type : {resultType}");
            }

            var lookupIndetifier = Configuration.LookupConventions[resultType];

            var resultFields = HelperFunctions.GetAllPublicNonGenericFields(resultType);
            var firstFields = HelperFunctions.GetAllPublicNonGenericFields(firstType);
            var secondFields = HelperFunctions.GetAllPublicNonGenericFields(secondType);
            var thirdFields = HelperFunctions.GetAllPublicNonGenericFields(thirdType);
            Dictionary<object, TResult> mappedResults = new Dictionary<object, TResult>();

            foreach (var row in queryRows)
            {
                var resultInstance = HelperFunctions.CreateInstanceAndMatchFields<TResult>(resultType, resultFields, row, lookupIndetifier);
                var firstInstance = HelperFunctions.CreateInstanceAndMatchFields<TFirst>(firstType, firstFields, row, lookupIndetifier);
                var secondInstance = HelperFunctions.CreateInstanceAndMatchFields<TSecond>(secondType, secondFields, row, lookupIndetifier);
                var thirdInstance = HelperFunctions.CreateInstanceAndMatchFields<TThird>(thirdType, thirdFields, row, lookupIndetifier);

                lookupResolver(resultInstance, firstInstance, secondInstance, thirdInstance, mappedResults);
            }

            return mappedResults.Values.ToList();
        }

        public static List<TResult> Map<TResult, TFirst, TSecond>(IList<IDictionary<string, object>> queryRows, Action<TResult, TFirst, TSecond, IDictionary<object, TResult>> lookupResolver)
        {
            Type resultType = typeof(TResult);
            Type firstType = typeof(TFirst);
            Type secondType = typeof(TSecond);

            if (!Configuration.LookupConventions.ContainsKey(resultType))
            {
                throw new ArgumentException($"There is no registered LookupFieldIdentification for type : {resultType}");
            }

            var lookupIndetifier = Configuration.LookupConventions[resultType];

            var resultFields = HelperFunctions.GetAllPublicNonGenericFields(resultType);
            var firstFields = HelperFunctions.GetAllPublicNonGenericFields(firstType);
            var secondFields = HelperFunctions.GetAllPublicNonGenericFields(secondType);
            Dictionary<object, TResult> mappedResults = new Dictionary<object, TResult>();

            foreach (var row in queryRows)
            {
                var resultInstance = HelperFunctions.CreateInstanceAndMatchFields<TResult>(resultType, resultFields, row, lookupIndetifier);
                var firstInstance = HelperFunctions.CreateInstanceAndMatchFields<TFirst>(firstType, firstFields, row, lookupIndetifier);
                var secondInstance = HelperFunctions.CreateInstanceAndMatchFields<TSecond>(secondType, secondFields, row, lookupIndetifier);

                lookupResolver(resultInstance, firstInstance, secondInstance, mappedResults);
            }

            return mappedResults.Values.ToList();
        }


        public static List<TResult> Map<TResult, TFirst>(IList<IDictionary<string, object>> queryRows, Action<TResult, TFirst, Dictionary<object, TResult>> lookupResolver)
        {
            Type resultType = typeof(TResult);
            Type firstType = typeof(TFirst);

            if (!Configuration.LookupConventions.ContainsKey(resultType))
            {
                throw new ArgumentException($"There is no registered LookupFieldIdentification for type : {resultType}");
            }

            var lookupIndetifier = Configuration.LookupConventions[resultType];

            var resultFields = HelperFunctions.GetAllPublicNonGenericFields(resultType);
            var firstFields = HelperFunctions.GetAllPublicNonGenericFields(firstType);
            Dictionary<object, TResult> mappedResults = new Dictionary<object, TResult>();

            foreach (var row in queryRows)
            {
                var firstInstance = HelperFunctions.CreateInstanceAndMatchFields<TFirst>(firstType, firstFields, row, lookupIndetifier);
                var resultInstance = HelperFunctions.CreateInstanceAndMatchFields<TResult>(resultType, resultFields, row, lookupIndetifier);

                lookupResolver(resultInstance, firstInstance, mappedResults);
            }

            return mappedResults.Values.ToList();
        }

        public static List<TResult> Map<TResult>(IList<IDictionary<string, object>> queryRows, Action<TResult, Dictionary<object, TResult>> lookupResolver)
        {
            Type resultType = typeof(TResult);

            if (!Configuration.LookupConventions.ContainsKey(resultType))
            {
                throw new ArgumentException($"There is no registered LookupFieldIdentification for type : {resultType}");
            }

            var lookupIndetifier = Configuration.LookupConventions[resultType];

            var resultFields = HelperFunctions.GetAllPublicNonGenericFields(resultType);
            Dictionary<object, TResult> mappedResults = new Dictionary<object, TResult>();

            foreach (var row in queryRows)
            {
                var resultInstance = HelperFunctions.CreateInstanceAndMatchFields<TResult>(resultType, resultFields, row, lookupIndetifier);

                lookupResolver(resultInstance, mappedResults);
            }

            return mappedResults.Values.ToList();
        }
    }
}
