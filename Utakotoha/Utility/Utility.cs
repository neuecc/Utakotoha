using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace Utakotoha
{
    internal static class Utility
    {
        public static TR Pipe<T, TR>(this T self, Func<T, TR> func)
        {
            return func(self);
        }

        public static string Wrap(this string input, string wrapper)
        {
            return wrapper + input + wrapper;
        }

        public static string Join<T>(this IEnumerable<T> source, string separator)
        {
            var index = 0;
            return source.Aggregate(new StringBuilder(),
                    (sb, o) => (index++ == 0) ? sb.Append(o) : sb.AppendFormat("{0}{1}", separator, o))
                .ToString();
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source) action(item);
        }
    }
}