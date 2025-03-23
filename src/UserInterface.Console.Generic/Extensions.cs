using System.Globalization;

namespace UserInterface.Console.Generic;

public static class Extensions
{
    public static string Format(this string template, params object?[] args)
        => string.Format(CultureInfo.InvariantCulture, template, args);

    public static string Format(this string template, IFormatProvider formatProvider, params object?[] args)
        => string.Format(CultureInfo.InvariantCulture, template, args);
}
