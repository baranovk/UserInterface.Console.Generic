namespace UserInterface.Console.Generic.Arguments;
using Functional;
using static Functional.F;

public abstract class ArgumentsParser(ArgumentsParser? next = null)
{
    private readonly ArgumentsParser? _next = next;

    public virtual IDictionary<string, string> ParseArguments(ReadOnlyMemory<string> argsToParse, IDictionary<string, string>? parsedArgs = null)
        => (parsedArgs ?? new Dictionary<string, string>())
                .Pipe(parsedArgsInitialized => ParseArgumentsInternal(argsToParse, parsedArgsInitialized))
                .Pipe(parseResult => 0 < parseResult.ArgsToParseNext.Length
                        ? _next?.ParseArguments(parseResult.ArgsToParseNext, parseResult.ParsedArgsEnriched) ?? parseResult.ParsedArgsEnriched
                        : parseResult.ParsedArgsEnriched);

    protected abstract (ReadOnlyMemory<string> ArgsToParseNext, IDictionary<string, string> ParsedArgsEnriched) ParseArgumentsInternal(
        ReadOnlyMemory<string> argsToParse, IDictionary<string, string> parsedArgs);
}
