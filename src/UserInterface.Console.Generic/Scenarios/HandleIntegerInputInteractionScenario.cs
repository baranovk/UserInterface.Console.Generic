using static Functional.F;

namespace UserInterface.Console.Generic.Scenarios;

public class HandleIntegerInputInteractionScenario(
    string prompt,
    string invalidInputMessage,
    InteractionScenario onCancelScenario,
    Func<IntegerInput, Context, Task<Context>> inputHandler,
    string? cancelKey = null) : InteractionScenario
{
    private readonly string _prompt = prompt;
    private readonly string _cancelKey = cancelKey ?? Resources.QuitKey;
    private readonly string _invalidInputMessage = invalidInputMessage;
    private readonly InteractionScenario _onCancelScenario = onCancelScenario;
    private readonly Func<IntegerInput, Context, Task<Context>> _inputHandler = inputHandler;

    public override async Task<Context> Execute(Context context)
        => await (await AwaitInput(
                context.UI,
                input => input switch
                {
                    IntegerInput i => Async(Valid(i as UserInput)),
                    TextInput ti => Async(_cancelKey == ti.Value ? Valid(ti as UserInput) : Invalid<UserInput>(Error(_invalidInputMessage))),
                    _ => Async(Invalid<UserInput>(Error(_invalidInputMessage)))
                },
                _prompt
            )
            .ConfigureAwait(false)
        )
        .Pipe(input => input switch
            {
                IntegerInput i => _inputHandler(i, context),
                _ => Async(context with { CurrentScenario = _onCancelScenario })
            }
        ).ConfigureAwait(false);
}
