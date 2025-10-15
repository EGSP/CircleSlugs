using System.Collections.Generic;

public static class EnumerableExtensions
{
    public static IEnumerable<T> EnumerateTogether<T>(this IEnumerable<T> first, params IEnumerable<T>[] others)
    {
        foreach (var item in first)
            yield return item;
        foreach (var collection in others)
            foreach (var item in collection)
                yield return item;
    }
}