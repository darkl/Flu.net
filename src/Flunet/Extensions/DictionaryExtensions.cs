using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Flunet.TypeAnalyzer;

namespace Flunet.Extensions
{
    internal static class DictionaryExtensions
    {
        #region Double Key Extensions

        public static IDictionary<Tuple<TKey1, TKey2>, TValue> ToDictionary<TSource, TKey1, TKey2, TValue>
            (this IEnumerable<TSource> source,
             Func<TSource, TKey1> key1Selector,
             Func<TSource, TKey2> key2Selector,
             Func<TSource, TValue> valueSelector)
        {
            return source.ToDictionary(x => Tuple.Create(key1Selector(x), key2Selector(x)),
                                       valueSelector);
        }

        public static TValue Get<TKey, TValue>(this IDictionary<Tuple<TKey, TKey>, TValue> dictionary, TKey first,
                                               TKey second)
        {
            return dictionary[Tuple.Create(first, second)];
        }

        public static void Add<TKey1, TKey2>(this HashSet<Tuple<TKey1, TKey2>> set, TKey1 key1, TKey2 key2)
        {
            set.Add(Tuple.Create(key1, key2));
        }

        public static bool Contains<TKey1, TKey2>(this HashSet<Tuple<TKey1, TKey2>> set, TKey1 key1, TKey2 key2)
        {
            return set.Contains(Tuple.Create(key1, key2));
        }

        #endregion

        #region Lookup Extensions

        public static void Add<TKey, TValue>(this IDictionary<TKey, ICollection<TValue>> dictionary, TKey key, TValue value)
        {
            ICollection<TValue> current;

            if (!dictionary.ContainsKey(key))
            {
                current = new HashSet<TValue>();
                dictionary[key] = current;
            }
            else
            {
                current = dictionary[key];
            }

            current.Add(value);
        }

        public static void Add<TKey>(this IDictionary<TKey, ICollection<MethodInfo>> dictionary, TKey key, MethodInfo value)
        {
            ICollection<MethodInfo> current;

            if (!dictionary.ContainsKey(key))
            {
                current = new HashSet<MethodInfo>(new ToStringComparer<MethodInfo>());
                dictionary[key] = current;
            }
            else
            {
                current = dictionary[key];
            }

            current.Add(value);
        }


        #endregion
    }
}
