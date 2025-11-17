using Godot;

namespace GWJ87.Globals;

public partial class RoutineManager : Node
{
    public enum CURRENT_ROUTINE
    {
        NONE,
        FEEDING,
        SLEEPING,
        CLEANING
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

    #region Cleaning Routine Variables
    private bool hasCleaningStarted = false;
    private float cleaningTimer = 1f;
    private int maxCleaningTicks = 5;
    private int cleaningTicks = 0;
    #endregion

    private Timer timer;
    private Timer cTimer;

    public override void _Ready()
    {
        Instance = this;

        timer = new Timer();
        timer.WaitTime = 1.0f;
        timer.Autostart = false;
        timer.OneShot = false;
        timer.Timeout += OnTimerTimeout;
        AddChild(timer);

        cTimer = new Timer();
        cTimer.WaitTime = 1.0f;
        cTimer.Autostart = false;
        cTimer.OneShot = false;
        cTimer.Timeout += OnCleaningTimerTimeout;
        AddChild(cTimer);
    }

    private void OnTimerTimeout()
    {
        StartRoutine(CurrentRoutine);
    }

    private void OnCleaningTimerTimeout()
    {
        cleaningTicks++;

        if (cleaningTicks >= maxCleaningTicks)
        {
            // need to stop cleaning
            StopRoutine();

            SignalManager.Instance.EmitManuallySetCatAnimation(Scenes.Entities.Cats.Cat.CAT_ANIMATIONS.IDLE);
            SignalManager.Instance.EmitManuallySetCatState(Scenes.Entities.Cats.Cat.CAT_STATE.IDLING);
        }
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
        else if (CurrentRoutine == CURRENT_ROUTINE.CLEANING)
        {
            CleaningRoutine();
        }
    }

    public void StopRoutine()
    {
        CurrentRoutine = CURRENT_ROUTINE.NONE;

        // Reset any flags back to default
        hasFeedingStarted = false;
        hasSleepingStarted = false;
        hasCleaningStarted = false;
        cleaningTicks = 0;

        timer.Stop();
        cTimer.Stop();
    }

    public void FeedingRoutine()
    {
        if (!hasFeedingStarted)
        {
            hasFeedingStarted = true;

            timer.Stop();
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
            hasSleepingStarted = true;

            timer.Stop();
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

    public void CleaningRoutine()
    {
        if (!hasCleaningStarted)
        {
            hasCleaningStarted = true;
            cleaningTicks = 0;

            timer.Stop();
            timer.WaitTime = cleaningTimer;
            timer.Start();

            cTimer.Stop();
            cTimer.Start();

            return;
        }

        SignalManager.Instance.EmitAttemptClean();
    }

    public void Clean()
    {
        int newEnergy = Mathf.Clamp(GameStats.Instance.Energy + 3, 0, 100);
        int newHappiness = Mathf.Clamp(GameStats.Instance.Happiness + 2, 0, 100);
        int newCleanliness = Mathf.Clamp(GameStats.Instance.Cleanliness + 10, 0, 100);
        int newDecay = Mathf.Clamp(GameStats.Instance.Decay - 3, 0, 100);
        SignalManager.Instance.EmitHappinessChanged(newHappiness);
        SignalManager.Instance.EmitEnergyChanged(newEnergy);
        SignalManager.Instance.EmitCleanlinessChanged(newCleanliness);
        SignalManager.Instance.EmitDecayChanged(newDecay);
    }
}
