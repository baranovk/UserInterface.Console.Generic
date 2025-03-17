namespace UserInterface.Console.Generic;

public abstract record UserInput();

public record IntegerInput(int Value) : UserInput;

public record TextInput(string Value) : UserInput;

public record UserInputError(Exception Error) : UserInput;

public record EmptyInput() : UserInput;
