using Functional;
using static Functional.F;

namespace UserInterface.Console.Generic;

public static class UserInterfaceRunner
{
    public static async Task RunAsync(InteractionScenario entryPointScenario)
        => await new ConsoleInterface()
                    .Pipe(@interface
                            => Async(new Context(new UserInterface(@interface), entryPointScenario))
                                .IterateUntilAsync(
                                    async ctx => await ctx.CurrentScenario.Execute(ctx).ConfigureAwait(false),
                                    ctx => ctx.Finished
                                )
                    ).ConfigureAwait(false);
}
