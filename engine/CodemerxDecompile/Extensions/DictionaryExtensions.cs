using System.Collections.Generic;

namespace CodemerxDecompile.Extensions;

public static class DictionaryExtensions
{
    public static void AddRange<T, K>(this Dictionary<T, K> to, Dictionary<T, K> from)
        where T: notnull
    {
        foreach (KeyValuePair<T, K> pair in from)
        {
            to[pair.Key] = pair.Value;
        }
    }
}
