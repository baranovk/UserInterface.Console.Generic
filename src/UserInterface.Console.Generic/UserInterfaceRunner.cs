using Functional;
using UserInterface.Console.Generic.Scenarios;
using static Functional.F;

namespace UserInterface.Console.Generic;

public static class UserInterfaceRunner
{
    public static async Task RunAsync(InteractionScenario entryPointScenario, CancellationToken cancellationToken = default)
        => await new ConsoleInterface()
                    .Pipe(@interface
                            => Async(new Context(new UserInterface(@interface), entryPointScenario))
                                .IterateUntilAsync(
                                    async ctx => (await TryAsync(ct => ctx.CurrentScenario.Execute(ctx, cancellationToken))
                                                    .RunAsync(cancellationToken).ConfigureAwait(false))
                                        .Match(
                                            ex => ctx.UI.WriteEmpty()
                                                        .WriteMessage(ex.ToString())
                                                        .WriteEmpty()
                                                        .Pipe(_ => ctx with { CurrentScenario = entryPointScenario }),
                                            @ctx => @ctx),
                                    ctx => ctx.Finished
                                )
                    ).ConfigureAwait(false);
}
