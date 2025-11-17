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
        SignalManager.Instance.StartCleanRoutine += OnCleanRoutineStarted;
    }

    public override void _Process(double delta)
    {
        if (CatObj.CurrentState != Cat.CAT_STATE.CLEANING && unpackedRain != null)
        {
            RemoveChild(unpackedRain);
            unpackedRain.Dispose();
            unpackedRain = null;
        }

        if (unpackedRain != null)
        {
            unpackedRain.GlobalPosition = GetGlobalMousePosition();
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

    private void OnCleanRoutineStarted()
    {
        if (CatObj.CurrentState == Cat.CAT_STATE.CLEANING)
            return;
        if (!CatObj.CanChangeRoutine())
            return;

        RoutineManager.Instance.StopRoutine();
        CatObj.StartCleanRoutine();
        
        if (unpackedRain == null)
        {
            unpackedRain = packedRainEffect.Instantiate<RainParticleEffects>();
            AddChild(unpackedRain);
        }
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
