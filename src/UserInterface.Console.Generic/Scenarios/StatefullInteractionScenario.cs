using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface.Console.Generic.Scenarios;

public abstract class StatefullInteractionScenario<TState>(TState initialState) : InteractionScenario
{
    protected TState State { get; private set; } = initialState;

    protected InteractionScenario SetState(TState state)
    {
        State = state;
        return this;
    }
}
