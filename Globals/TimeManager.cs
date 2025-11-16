using Godot;
using System;

public partial class TimeManager : Node
{
    public static TimeManager Instance { get; private set; }
    private int currentDay = 1;
    
    /* Time Tracking */
    private int currentMinute = 50;
    private int hoursInDay = 24;
    
    private int currentHour = 23;
    private int minutesInHour = 60;

    /* Tick Tracking */
    private int tickCounter = 0;
    private int ticksPerMinute = 2; // Example: x ticks per minute

    private Timer tickTimer;

    public override void _Ready()
    {
        Instance = this;

        tickTimer = new Timer();
        tickTimer.WaitTime = 1.0;
        tickTimer.Autostart = true;
        tickTimer.Timeout += OnTickTimer_Timeout;
        
        AddChild(tickTimer);

        CallDeferred(nameof(AdvanceDay));
        CallDeferred(nameof(ChangeTimeOfDay));
    }

    private void OnTickTimer_Timeout()
    {
        tickCounter++;
        if (tickCounter >= ticksPerMinute)
        {
            tickCounter = 0;
            currentMinute++;
            if (currentMinute >= minutesInHour)
            {
                currentMinute = 0;
                currentHour++;
                if (currentHour >= hoursInDay)
                {
                    currentHour = 0;

                    currentDay++;
                }
            }
        }

        ChangeTimeOfDay();
        AdvanceDay();
    }

    public void AdvanceDay()
        => SignalManager.Instance.EmitSignal(SignalManager.SignalName.DayChanged, currentDay);
    
    public void ChangeTimeOfDay()
        => SignalManager.Instance.EmitSignal(SignalManager.SignalName.TimeOfDayChanged, GetTimeOfDay());

    public string GetTimeOfDay()
    {
        DateTime dt = new DateTime(1, 1, 1, currentHour, currentMinute, 1);
        return dt.ToString("hh:mm tt");
    }
}
