using Godot;
using GWJ87.Scenes.Entities.Cats;

public partial class Home : Node2D
{
    [Export] public Cat CatObj { get; private set; }

    private NavigationRegion2D NavRegion;

    public override void _Ready()
    {
        NavRegion = GetNode<NavigationRegion2D>("%NavRegion");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("move_up"))
        {
            CatObj.SetColor(Cat.CAT_COLORS.BLACK);
            CatObj.SetAnimation(Cat.CAT_ANIMATIONS.PLAYFUL);

            var randomPoint = GetRandomNavMeshPoint();
            CatObj.SetTargetPosition(randomPoint);
            GD.Print($"Random Point: {randomPoint}");
        }
        else if (@event.IsActionPressed("move_down"))
        {
            CatObj.SetColor(Cat.CAT_COLORS.ORANGE);
            CatObj.SetAnimation(Cat.CAT_ANIMATIONS.EXCITED);
        }
        else if (@event.IsActionPressed("move_left"))
        {
            CatObj.SetColor(Cat.CAT_COLORS.GREY);
            CatObj.SetAnimation(Cat.CAT_ANIMATIONS.IDLE);
        }
        else if (@event.IsActionPressed("move_right"))
        {
            CatObj.SetColor(Cat.CAT_COLORS.GREYWHITE);
            CatObj.SetAnimation(Cat.CAT_ANIMATIONS.SLEEPING);
        }
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
