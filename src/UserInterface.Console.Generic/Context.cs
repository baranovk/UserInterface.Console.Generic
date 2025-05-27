using UserInterface.Console.Generic.Scenarios;

namespace UserInterface.Console.Generic;

public record Context(IUserInterface UI, InteractionScenario CurrentScenario, bool Finished = false);
