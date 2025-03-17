using Functional;

namespace UserInterface.Console.Generic;

public interface IUserInterface
{
    UserInput GetInput(params string[] prompt);

    IUserInterface WriteMessage(params string[] prompt);

    IUserInterface WriteEmpty();
}

internal sealed class UserInterface(ConsoleInterface console) : IUserInterface
{
    private readonly ConsoleInterface _console = console;

    public UserInput GetInput(params string[] prompt)
        => _console
            .WriteLine(string.Empty)
            .WritePrompt(prompt)
            .ReadLine()
            .Match(
                ex => new UserInputError(ex),
                option => option.Match<UserInput>(
                    () => new EmptyInput(),
                    value => value switch
                    {
                        string strInt when int.TryParse(strInt, out _) => new IntegerInput(int.Parse(strInt)),
                        _ => new TextInput(value)
                    }
                )
            );

    public IUserInterface WriteMessage(params string[] prompt) => _console.WriteLine(prompt).Pipe(_ => this);

    public IUserInterface WriteEmpty() => _console.WriteLine().Pipe(_ => this);
}
