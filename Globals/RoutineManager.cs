using Godot;

namespace GWJ87.Globals;

public partial class RoutineManager : Node
{
    public enum CURRENT_ROUTINE
    {
        NONE,
        FEEDING,
        SLEEPING
    }

    public static RoutineManager Instance { get; private set; }
    
    public CURRENT_ROUTINE CurrentRoutine { get; set; } = CURRENT_ROUTINE.NONE;

    #region Feeding Routine Variables
    private bool hasFeedingStarted = false;
    private float feedingTimer = 1f;
    #endregion

    #region Sleeping Routine Variables
    private bool hasSleepingStarted = false;
    private float sleepingTimer = 2f;
    #endregion

    private Timer timer;

    public override void _Ready()
    {
        Instance = this;

        timer = new Timer();
        timer.WaitTime = 1.0f;
        timer.Autostart = false;
        timer.OneShot = false;
        timer.Timeout += OnTimerTimeout;
        AddChild(timer);
    }

    private void OnTimerTimeout()
    {
        StartRoutine(CurrentRoutine);
    }

    public void StartRoutine(CURRENT_ROUTINE routine)
    {
        CurrentRoutine = routine;

        if (CurrentRoutine == CURRENT_ROUTINE.FEEDING)
        {
            FeedingRoutine();
        }
        else if (CurrentRoutine == CURRENT_ROUTINE.SLEEPING)
        {
            SleepingRoutine();
        }
    }

    public void StopRoutine()
    {
        CurrentRoutine = CURRENT_ROUTINE.NONE;

        // Reset any flags back to default
        hasFeedingStarted = false;
        hasSleepingStarted = false;

        timer.Stop();
    }

    public void FeedingRoutine()
    {
        if (!hasFeedingStarted)
        {
            timer.Stop();
            hasFeedingStarted = true;
            timer.WaitTime = feedingTimer;
            timer.Start();
            
            return;
        }

        int newHunger = Mathf.Clamp(GameStats.Instance.Hunger - 10, 0, 100);
        int newHappiness = Mathf.Clamp(GameStats.Instance.Happiness + 5, 0, 100);
        int newEnergy = Mathf.Clamp(GameStats.Instance.Energy + 1, 0, 100);
        int newDecay = Mathf.Clamp(GameStats.Instance.Decay - 1, 0, 100);
        SignalManager.Instance.EmitHungerChanged(newHunger);
        SignalManager.Instance.EmitHappinessChanged(newHappiness);
        SignalManager.Instance.EmitEnergyChanged(newEnergy);
        SignalManager.Instance.EmitDecayChanged(newDecay);
    }

    public void SleepingRoutine()
    {
        if (!hasSleepingStarted)
        {
            timer.Stop();
            hasSleepingStarted = true;
            timer.WaitTime = sleepingTimer;
            timer.Start();
            
            return;
        }

        int newHunger = Mathf.Clamp(GameStats.Instance.Hunger + 1, 0, 100);
        int newEnergy = Mathf.Clamp(GameStats.Instance.Energy + 10, 0, 100);
        int newHappiness = Mathf.Clamp(GameStats.Instance.Happiness -1, 0, 100);
        int newDecay = Mathf.Clamp(GameStats.Instance.Decay - 1, 0, 100);
        SignalManager.Instance.EmitHungerChanged(newHunger);
        SignalManager.Instance.EmitHappinessChanged(newHappiness);
        SignalManager.Instance.EmitEnergyChanged(newEnergy);
        SignalManager.Instance.EmitDecayChanged(newDecay);
    }
}
