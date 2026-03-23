namespace Grupo13Fiap.Utils.Extensions;

public static class CollectionExtensions
{
    public static List<T> FilterNullsAndDefaults<T>(this List<T?> values) where T : struct, IComparable<T>
    {
        if(((ICollection<T?>)values).IsNullOrEmpty())
            return [];

        var result = new List<T>(values.Count);

        result.AddRange(from value in values
                        where !value.IsNullOrDefault()
                        select value.Value);
        return result;
    }

    public static List<T> FilterNullsAndZeros<T>(this T?[] values) where T : struct, IComparable<T>
    {
        return values.IsNullOrEmpty()
            ? []
            : FilterNullsAndZeros(values.AsSpan());
    }

    private static List<T> FilterNullsAndZeros<T>(this Span<T?> values) where T : struct, IComparable<T>
    {
        var result = new List<T>(values.Length);

        foreach(var value in values)
        {
            if(!value.IsNullOrDefault())
                result.Add(value!.Value);
        }

        return result;
    }

    public static bool IsNullOrEmpty<T>(this T[]? array)
        => array is null or { Length: 0 };

    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        if(source is null)
            return true;

        if(source is ICollection<T> collection)
            return collection is { Count: 0 };

        if(source is IReadOnlyCollection<T> readOnlyCollection)
            return readOnlyCollection is { Count: 0 };

        return !source.Any();
    }


    public static bool IsNullOrDefault<T>(this T? value) where T : struct
        => !value.HasValue || value.Value.Equals(default(T));

    public static bool IsDefault<T>(this T value) where T : struct
        => value.Equals(default(T));
}
