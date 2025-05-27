namespace UserInterface.Console.Generic.Scenarios;

public abstract class ArgumentsInputInteractionScenario(IDictionary<string, string> arguments)
{
    protected IDictionary<string, string> Arguments { get; private set; } = arguments;

    public abstract Task<Context> Execute(Context context);
}
