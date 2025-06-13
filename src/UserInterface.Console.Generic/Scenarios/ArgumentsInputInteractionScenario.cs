namespace UserInterface.Console.Generic.Scenarios;

public abstract class ArgumentsInputInteractionScenario(IDictionary<string, string> arguments) : InteractionScenario
{
    protected IDictionary<string, string> Arguments { get; private set; } = arguments;

    public abstract override Task<Context> Execute(Context context, CancellationToken cancellationToken = default);
}
