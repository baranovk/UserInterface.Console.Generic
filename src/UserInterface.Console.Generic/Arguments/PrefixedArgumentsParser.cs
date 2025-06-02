using System.Text;

namespace UserInterface.Console.Generic.Arguments;

public class PrefixedArgumentsParser(string argKeyPrefix, ArgumentsParser? next = null) : ArgumentsParser(next)
{
    private const string DefaultArgKeyPrefix = "-";
    private readonly string _argKeyPrefix = argKeyPrefix;

    public PrefixedArgumentsParser(ArgumentsParser? next = null) : this(DefaultArgKeyPrefix, next)
    {
    }

    protected override (ReadOnlyMemory<string> ArgsToParseNext, IDictionary<string, string> ParsedArgsEnriched) ParseArgumentsInternal(
        ReadOnlyMemory<string> argsToParse, IDictionary<string, string> parsedArgs)
    {
        if (0 == argsToParse.Length) { return (argsToParse, parsedArgs); }

        string sep;
        int keysCount = default;
        var sb = new StringBuilder();

        for (var i = 0; i < argsToParse.Length;)
        {
            if (argsToParse.Span[i].Length <= _argKeyPrefix.Length
                || !argsToParse.Span[i].StartsWith(_argKeyPrefix, StringComparison.CurrentCulture))
            {
                i++;
                continue;
            }

            var key = argsToParse.Span[i][_argKeyPrefix.Length..];
            sep = string.Empty;

            while (++i < argsToParse.Length && !argsToParse.Span[i].StartsWith(_argKeyPrefix, StringComparison.CurrentCulture))
            {
                sb.Append(sep).Append(argsToParse.Span[i]);
                sep = " ";
            }

            keysCount++;
            parsedArgs.Add(key, sb.ToString());
            sb.Clear();
        }

        return (0 < keysCount ? argsToParse[^1..] : argsToParse, parsedArgs);
    }
}
