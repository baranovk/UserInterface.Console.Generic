using static Functional.F;

namespace UserInterface.Console.Generic.Scenarios;

public class HandleIntegerInputInteractionScenario(
    string prompt,
    string invalidInputMessage,
    Func<InteractionScenario> onCancelScenario,
    Func<IntegerInput, Context, CancellationToken, Task<Context>> inputHandler,
    string? cancelKey = null) : InteractionScenario
{
    private readonly string _prompt = prompt;
    private readonly string _cancelKey = cancelKey ?? Resources.QuitKey;
    private readonly string _invalidInputMessage = invalidInputMessage;
    private readonly Func<InteractionScenario> _onCancelScenario = onCancelScenario;
    private readonly Func<IntegerInput, Context, CancellationToken, Task<Context>> _inputHandler = inputHandler;

    public override async Task<Context> Execute(Context context, CancellationToken cancellationToken = default)
        => await (await AwaitInput(
                context.UI,
                (input, _) => input switch
                {
                    IntegerInput i => Async(Valid(i as UserInput)),
                    TextInput ti => Async(_cancelKey == ti.Value ? Valid(ti as UserInput) : Invalid<UserInput>(Error(_invalidInputMessage))),
                    _ => Async(Invalid<UserInput>(Error(_invalidInputMessage)))
                },
                cancellationToken,
                _prompt
            )
            .ConfigureAwait(false)
        )
        .Pipe(input => input switch
            {
                IntegerInput i => _inputHandler(i, context, cancellationToken),
                _ => Async(context with { CurrentScenario = _onCancelScenario() })
            }
        ).ConfigureAwait(false);
}
