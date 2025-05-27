using System.Globalization;
using System.Text;

namespace UserInterface.Console.Generic;

public static class Extensions
{
    public static string Format(this string template, params object?[] args)
        => string.Format(CultureInfo.InvariantCulture, template, args);

    public static string Format(this string template, IFormatProvider formatProvider, params object?[] args)
        => string.Format(CultureInfo.InvariantCulture, template, args);

    public static bool IsQuitKey(this TextInput input) => input.Value == Resources.QuitKey;

    public static IDictionary<string, string> ParseArguments(this string[] args)
    {
        if (0 == args.Length) { return new Dictionary<string, string>(); }

        string sep;
        var sb = new StringBuilder();
        var argsDict = new Dictionary<string, string>();

        for (var i = 0; i < args.Length;)
        {
            if ('-' != args[i][0]) { i++; continue; }

            var key = args[i][1..];
            sep = string.Empty;

            while (++i < args.Length && '-' != args[i][0])
            {
                sb.Append(sep).Append(args[i]);
                sep = " ";
            }

            argsDict.Add(key, sb.ToString());
            sb.Clear();
        }

        return argsDict;
    }
}
