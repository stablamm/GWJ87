using Godot;

namespace GWJ87.Globals;

public partial class GameStats : Node
{
    public static GameStats Instance { get; private set; }

    public int DayCount { get; private set; } = 0;
    public string CurrentTime { get; private set; } = string.Empty;
    
    public int Health { get; private set; } = 100;
    public int Hunger { get; private set; } = 50;
    public int Happiness { get; private set; } = 50;
    public int Energy { get; private set; } = 80;
    public int Cleanliness { get; private set; } = 80;
    public int Decay { get; private set; } = 0;

    private int ticksBeforeDecay = 0;
    private const int ticksPerDecay = 5;

    public override void _Ready()
    {
        Instance = this;

        SignalManager.Instance.TimeOfDayChanged += OnTimeOfDayChanged;
    }

    public void OnDayChanged(int newDay)
        => DayCount = newDay;

    public void OnTimeOfDayChanged(string newTimeOfDay)
    {
        CurrentTime = newTimeOfDay;
        DecayStats();
    }

    public void OnHealthChanged(int newHealth)
        => Health = newHealth;
    
    public void OnHungerChanged(int newHunger)
        => Hunger = newHunger;
    
    public void OnHappinessChanged(int newHappiness)
        => Happiness = newHappiness;
    
    public void OnEnergyChanged(int newEnergy)
        => Energy = newEnergy;
    
    public void OnCleanlinessChanged(int newCleanliness)
        => Cleanliness = newCleanliness;

    public void DecayStats()
    {
        if (!CanDecay())
            return;

        Hunger = Mathf.Clamp(Hunger += 3, 0, 100);
        Happiness = Mathf.Clamp(Happiness -= 2, 0, 100);
        Energy = Mathf.Clamp(Energy -= 2, 0, 100);
        Cleanliness = Mathf.Clamp(Cleanliness -= 1, 0, 100);

        // Conditional Decay Based On Other Stats
        if (Hunger >= 70)
            Decay = Mathf.Clamp(Decay += 2, 0, 100);
        if (Happiness <= 30)
            Decay = Mathf.Clamp(Decay += 2, 0, 100);
        if (Energy <= 30)
            Decay = Mathf.Clamp(Decay += 2, 0, 100);
        if (Cleanliness <= 30)
            Decay = Mathf.Clamp(Decay += 2, 0, 100);

    }

    private bool CanDecay()
    {
        ticksBeforeDecay++;
        if (ticksBeforeDecay >= ticksPerDecay)
        {
            ticksBeforeDecay = 0;
            return true;
        }
        else 
            return false;
    }
}
