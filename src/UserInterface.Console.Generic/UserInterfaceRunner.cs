using Functional;
using UserInterface.Console.Generic.Scenarios;
using static Functional.F;

namespace UserInterface.Console.Generic;

public static class UserInterfaceRunner
{
    public static async Task RunAsync(InteractionScenario entryPointScenario)
        => await new ConsoleInterface()
                    .Pipe(@interface
                            => Async(new Context(new UserInterface(@interface), entryPointScenario))
                                .IterateUntilAsync(
                                    async ctx => (await TryAsync(() => ctx.CurrentScenario.Execute(ctx)).RunAsync().ConfigureAwait(false))
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
