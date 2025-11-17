using Godot;
using GWJ87.Globals;

public partial class CatTerminal : Node2D
{
    private Label CatNameLabel;
    private Label DayLabel;
    private Label GameTimeLabel;
    private Label MoodLabel;
    private Label StatusLabel;
    private ProgressBar Health;
    private ProgressBar Hunger;
    private ProgressBar Happiness;
    private ProgressBar Energy;
    private ProgressBar Cleanliness;
    private ProgressBar Decay;
    private RichTextLabel Status;

    public override void _Ready()
    {
        CatNameLabel = GetNode<Label>("%CatNameLabel");
        DayLabel = GetNode<Label>("%DayLabel");
        GameTimeLabel = GetNode<Label>("%GameTimeLabel");
        MoodLabel = GetNode<Label>("%MoodLabel");
        StatusLabel = GetNode<Label>("%StatusLabel");
        Health = GetNode<ProgressBar>("%Health");
        Hunger = GetNode<ProgressBar>("%Hunger");
        Happiness = GetNode<ProgressBar>("%Happiness");
        Energy = GetNode<ProgressBar>("%Energy");
        Cleanliness = GetNode<ProgressBar>("%Cleanliness");
        Decay = GetNode<ProgressBar>("%Decay");
        Status = GetNode<RichTextLabel>("%Status");

        SignalManager.Instance.TimeOfDayChanged += OnTimeOfDayChanged;
        SignalManager.Instance.DayChanged += OnDayChanged;
    }

    public override void _Process(double delta)
    {
        var gameStats = GameStats.Instance;

        Health.Value = gameStats.Health;
        Hunger.Value = gameStats.Hunger;
        Happiness.Value = gameStats.Happiness;
        Energy.Value = gameStats.Energy;
        Cleanliness.Value = gameStats.Cleanliness;
        Decay.Value = gameStats.Decay;
    }

    private void OnFeedButtonPressed()
    {
        GD.Print("Feed Button Pressed");
        SignalManager.Instance.EmitStartFeedRoutine();
    }

    private void OnCleanButtonPressed()
    {
        GD.Print("Clean Button Pressed");
        SignalManager.Instance.EmitStartCleanRoutine();
    }

    private void OnPlayButtonPressed()
    {
        GD.Print("Play Button Pressed");
    }

    private void OnSleepButtonPressed()
    {
        GD.Print("Sleep Button Pressed");
        SignalManager.Instance.EmitStartSleepRoutine();
    }

    private void OnPurifyButtonPressed()
    {
        GD.Print("Purify Button Pressed");
    }

    private void OnTimeOfDayChanged(string timeOfDay)
        => GameTimeLabel.Text = timeOfDay;

    private void OnDayChanged(int newDay)
        => DayLabel.Text = newDay.ToString();
}
