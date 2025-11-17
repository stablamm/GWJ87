using Godot;
using static GWJ87.Scenes.Entities.Cats.Cat;

public partial class SignalManager : Node
{
    public static SignalManager Instance { get; private set; }

    [Signal] public delegate void CatDestinationReachedEventHandler();
    [Signal] public delegate void DayChangedEventHandler(int newDay);
    [Signal] public delegate void TimeOfDayChangedEventHandler(string newTimeOfDay);
    [Signal] public delegate void HealthChangedEventHandler(int newHealth);
    [Signal] public delegate void HungerChangedEventHandler(int newHunger);
    [Signal] public delegate void HappinessChangedEventHandler(int newHappiness);
    [Signal] public delegate void EnergyChangedEventHandler(int newEnergy);
    [Signal] public delegate void CleanlinessChangedEventHandler(int newCleanliness);
    [Signal] public delegate void DecayChangedEventHandler(int newDecay);
    [Signal] public delegate void StartFeedRoutineEventHandler();
    [Signal] public delegate void StartSleepRoutineEventHandler();
    [Signal] public delegate void StartCleanRoutineEventHandler();
    [Signal] public delegate void RequestRandomPositionEventHandler(); // Cat will request a random position after being idle for a certain amount of time.
    [Signal] public delegate void AttemptCleanEventHandler();
    [Signal] public delegate void ManuallySetCatAnimationEventHandler(CAT_ANIMATIONS newAnimation);
    [Signal] public delegate void ManuallySetCatStateEventHandler(CAT_STATE newState);
    
    public override void _Ready()
    {
        Instance = this;
    }

    public void EmitCatDestinationReached() => EmitSignal(SignalName.CatDestinationReached);
    public void EmitDayChanged(int newDay) => EmitSignal(SignalName.DayChanged, newDay);
    public void EmitTimeOfDayChanged(string newTimeOfDay) => EmitSignal(SignalName.TimeOfDayChanged, newTimeOfDay);
    public void EmitHealthChanged(int newHealth) => EmitSignal(SignalName.HealthChanged, newHealth);
    public void EmitHungerChanged(int newHunger) => EmitSignal(SignalName.HungerChanged, newHunger);
    public void EmitHappinessChanged(int newHappiness) => EmitSignal(SignalName.HappinessChanged, newHappiness);
    public void EmitEnergyChanged(int newEnergy) => EmitSignal(SignalName.EnergyChanged, newEnergy);
    public void EmitCleanlinessChanged(int newCleanliness) => EmitSignal(SignalName.CleanlinessChanged, newCleanliness);
    public void EmitDecayChanged(int newDecay) => EmitSignal(SignalName.DecayChanged, newDecay);
    public void EmitStartFeedRoutine() => EmitSignal(SignalName.StartFeedRoutine);
    public void EmitStartSleepRoutine() => EmitSignal(SignalName.StartSleepRoutine);
    public void EmitStartCleanRoutine() => EmitSignal(SignalName.StartCleanRoutine);
    public void EmitRequestRandomPosition() => EmitSignal(SignalName.RequestRandomPosition);
    public void EmitAttemptClean() => EmitSignal(SignalName.AttemptClean);
    public void EmitManuallySetCatAnimation(CAT_ANIMATIONS newAnimation)
        => EmitSignal(SignalName.ManuallySetCatAnimation, (int)newAnimation);

    public void EmitManuallySetCatState(CAT_STATE newState)
        => EmitSignal(SignalName.ManuallySetCatState, (int)newState);
}
