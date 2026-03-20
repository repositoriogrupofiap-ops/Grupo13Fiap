using System.Numerics;

namespace Grupo13Fiap.Utils.Extensions;

public static class NumericExtensions
{
    public static bool IsZeroOrNegative<T>(this T value)
        where T : struct, INumber<T>
        => value <= T.Zero;

    public static bool IsZero<T>(this T value)
        where T : struct, INumber<T>
        => value == T.Zero;
}
