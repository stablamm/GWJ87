using Godot;

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
}
