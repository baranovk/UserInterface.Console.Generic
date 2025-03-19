using Functional;
using static Functional.F;

namespace UserInterface.Console.Generic;

public abstract class InteractionScenario
{
    #region Fields

    private static readonly Interaction _quitInteraction = new(Resources.QuitKey, Resources.QuitPrompt, new QuitScenario());
    private readonly IEnumerable<Interaction> _interactions;

    #endregion

    #region Constructors

    protected InteractionScenario(params Interaction[] interactions)
        => _interactions = interactions.Append(_quitInteraction);

    protected InteractionScenario(IEnumerable<Interaction>? interactions = null)
        => _interactions = (interactions ?? Enumerable.Empty<Interaction>()).Append(_quitInteraction);

    #endregion

    #region Protected Methods

    protected InteractionScenario DisplayInteractions(IUserInterface ui)
        => ui.WriteEmpty()
             .Pipe(
                _ => _interactions
                        .Aggregate(this, (scenario, interaction) =>
                        {
                            ui.WriteMessage($"[{interaction.Key}] {interaction.Description}");
                            return scenario;
                        })
                );

    protected static async Task<T> AwaitInput<T>(
        IUserInterface ui,
        Func<UserInput, Task<Validation<T>>> reaction,
        params string[] prompt)
        => (await reaction(ui.GetInput(prompt))
                .IterateUntilAsync(
                    async (validation) =>
                    {
                        validation
                            .Match(errors => _ = errors.Aggregate(ui.WriteEmpty(), (ui, e) => ui.WriteMessage(e.Message)),
                            _ => { }
                        );

                        return await reaction(ui.GetInput(prompt)).ConfigureAwait(false);
                    },
                    _ => _.IsValid
                ).ConfigureAwait(false)
            )
            .AsEnumerable()
            .First();

    protected Task<Validation<InteractionScenario>> SelectValidInteractionScenario(UserInput input)
        => input switch
        {
            IntegerInput iInt => Async(FindScenarioByKey(iInt.Value.ToString())),
            TextInput iText => Async(FindScenarioByKey(iText.Value)),
            EmptyInput => Async(Validation<InteractionScenario>.Fail(new ValidationError(Resources.SelectInteraction))),
            _ => Async(Validation<InteractionScenario>.Fail(new ValidationError(Resources.MissingAction)))
        };

    protected static string FormatPrompt(string prompt) => $"{prompt} --> ";

    #endregion

    #region Public Methods

    public virtual async Task<Context> Execute(Context context)
        => await DisplayInteractions(context.UI)
            .Pipe(
                _ => AwaitInput(context.UI, input => SelectValidInteractionScenario(input), FormatPrompt(Resources.SelectInteraction))
                        .Bind(nextScenario => Async(context with { CurrentScenario = nextScenario, Finished = nextScenario is QuitScenario }))
            ).ConfigureAwait(false);

    #endregion

    #region Private Methods

    private Validation<InteractionScenario> FindScenarioByKey(string key)
        => EnumerableExt
            .Find(_interactions, i => i.Key.Equals(key, StringComparison.Ordinal))
            .Match(
                () => Invalid(new ValidationError(Resources.MissingAction)),
                interaction => Valid(interaction.Scenario)
            );

    #endregion
}

public record Interaction(string Key, string Description, InteractionScenario Scenario);

internal sealed class QuitScenario : NoopScenario { }

internal class NoopScenario : InteractionScenario
{
    public override Task<Context> Execute(Context context) => Async(context);
}
