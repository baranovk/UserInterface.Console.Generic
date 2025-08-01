using Functional;
using static Functional.F;

namespace UserInterface.Console.Generic.Scenarios;

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
        Func<UserInput, CancellationToken, Task<Validation<T>>> reaction,
        CancellationToken cancellationToken = default,
        params string[] prompt)
        => (await reaction(ui.GetInput(prompt), cancellationToken)
                .IterateUntilAsync(
                    async (validation) =>
                    {
                        validation
                            .Match(
                                errors => _ = errors.Aggregate(ui.WriteEmpty(), (ui, e) => ui.WriteMessage(e.Message)),
                                _ => { }
                            );

                        cancellationToken.ThrowIfCancellationRequested();
                        return await reaction(ui.GetInput(prompt), cancellationToken).ConfigureAwait(false);
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

    #endregion

    #region Public Methods

    public virtual async Task<Context> Execute(Context context, CancellationToken cancellationToken = default)
        => await DisplayInteractions(context.UI)
            .Pipe(
                _ => AwaitInput(context.UI, (input, _) => SelectValidInteractionScenario(input), cancellationToken, Resources.SelectInteraction)
                        .Bind(nextScenario => Async(context with { CurrentScenario = nextScenario, Finished = nextScenario is QuitScenario }))
            ).ConfigureAwait(false);

    #endregion

    #region Private Methods

    private Validation<InteractionScenario> FindScenarioByKey(string key)
        => _interactions.Find(i => i.Key.Equals(key, StringComparison.Ordinal))
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
    public override Task<Context> Execute(Context context, CancellationToken cancellationToken = default) => Async(context);
}
