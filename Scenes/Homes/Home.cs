using Godot;
using GWJ87.Globals;
using GWJ87.Scenes.Effects;
using GWJ87.Scenes.Entities.Cats;

public partial class Home : Node2D
{
    [Export] public Cat CatObj { get; private set; }

    private PackedScene packedRainEffect = GD.Load<PackedScene>("res://Scenes/Effects/rain_particle_effects.tscn");
    private RainParticleEffects unpackedRain = null;

    private NavigationRegion2D NavRegion;
    private Marker2D FoodBowlMarker;
    private Marker2D WaterBowlMarker;
    private Marker2D BedMarker;

    public override void _Ready()
    {
        NavRegion = GetNode<NavigationRegion2D>("%NavRegion");

        FoodBowlMarker = GetNode<Marker2D>("%FoodBowlMarker");
        WaterBowlMarker = GetNode<Marker2D>("%WaterBowlMarker");
        BedMarker = GetNode<Marker2D>("%BedMarker");

        SignalManager.Instance.StartFeedRoutine += OnFeedRoutineStarted;
        SignalManager.Instance.RequestRandomPosition += OnRequestRandomPosition;
        SignalManager.Instance.StartSleepRoutine += OnSleepRoutineStarted;
    }

    public override void _Process(double delta)
    {
        if (unpackedRain == null)
        {
            unpackedRain = packedRainEffect.Instantiate<RainParticleEffects>();
            AddChild(unpackedRain);
        }

        unpackedRain.GlobalPosition = GetGlobalMousePosition();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("move_up"))
        {
            CatObj.SetColor(Cat.CAT_COLORS.BLACK);
            CatObj.SetAnimation(Cat.CAT_ANIMATIONS.PLAYFUL);

            var randomPoint = GetRandomNavMeshPoint();
            CatObj.SetTargetPosition(randomPoint, true);
            GD.Print($"Random Point: {randomPoint}");
        }
        else if (@event.IsActionPressed("move_down"))
        {
            CatObj.SetColor(Cat.CAT_COLORS.ORANGE);
            //CatObj.SetAnimation(Cat.CAT_ANIMATIONS.EXCITED);
            CatObj.SetTargetPosition(BedMarker.GlobalPosition, false);
        }
        else if (@event.IsActionPressed("move_left"))
        {
            CatObj.SetColor(Cat.CAT_COLORS.GREY);
            CatObj.SetAnimation(Cat.CAT_ANIMATIONS.IDLE);

            var packedRainParticles = GD.Load<PackedScene>("res://Scenes/Effects/rain_particle_effects.tscn");
            RainParticleEffects unpackedRain = packedRainParticles.Instantiate<RainParticleEffects>();
            AddChild(unpackedRain);
            unpackedRain.GlobalPosition = new Vector2(200, 200);
        }
        else if (@event.IsActionPressed("move_right"))
        {
            CatObj.SetColor(Cat.CAT_COLORS.GREYWHITE);
            CatObj.SetAnimation(Cat.CAT_ANIMATIONS.SLEEPING);
        }
    }

    private void OnFeedRoutineStarted()
    {
        if (CatObj.CurrentState == Cat.CAT_STATE.FEEDING)
            return;
        if (!CatObj.CanChangeRoutine()) 
            return;

        RoutineManager.Instance.StopRoutine();
        CatObj.StartFeedRoutine();

        var r = GD.RandRange(0, 1);
        if (r == 0)
            CatObj.SetTargetPosition(FoodBowlMarker.GlobalPosition, false);
        else
            CatObj.SetTargetPosition(WaterBowlMarker.GlobalPosition, false);
    }

    private void OnSleepRoutineStarted()
    {
        if (CatObj.CurrentState == Cat.CAT_STATE.SLEEPING)
            return;
        if (!CatObj.CanChangeRoutine()) 
            return;

        RoutineManager.Instance.StopRoutine();
        CatObj.StartSleepRoutine();
        CatObj.SetTargetPosition(BedMarker.GlobalPosition, false);
    }

    private void OnRequestRandomPosition()
    {
        var randomPoint = GetRandomNavMeshPoint();
        CatObj.SetTargetPosition(randomPoint, true);
        GD.Print($"Random Point Requested: {randomPoint}");
    }

    /// <summary>
    /// Returns a random point on the NavMesh within the NavigationRegion2D
    /// </summary>
    private Vector2 GetRandomNavMeshPoint()
    {
        var regionRid = NavRegion.GetRid();
        var mapRid = NavigationServer2D.RegionGetMap(regionRid);
        uint layers = NavRegion.NavigationLayers;
        if (layers == 0)
            layers = 1;
        return NavigationServer2D.MapGetRandomPoint(mapRid, layers, true);
    }
}
