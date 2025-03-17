using Functional;
using static Functional.F;

namespace UserInterface.Console.Generic;

internal sealed class ConsoleInterface
{
    public Exceptional<Option<string>> ReadLine()
        => Try(
            () => System.Console
                        .ReadLine()
                        .Pipe(input => string.IsNullOrWhiteSpace(input) ? None : Some(input))
           ).Run();

    public ConsoleInterface WriteLine(params string[] messages)
        => messages.Aggregate(this, (acc, m) => { System.Console.WriteLine(m ?? string.Empty); return acc; });

    public ConsoleInterface WritePrompt(params string[] messages)
        => (0 == messages.Length)
            ? this
            : messages.Skip(1)
                .Aggregate(
                    (@this: this, memo: messages[0]),
                    (acc, m) =>
                    {
                        System.Console.WriteLine(acc.memo ?? string.Empty);
                        acc.memo = m;
                        return acc;
                    }
                )
                .Pipe(acc => { System.Console.Write(acc.memo); return acc.@this; });
}
