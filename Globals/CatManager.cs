using Godot;
using System;

namespace GWJ87.Globals;

public partial class CatManager : Node
{
    public static CatManager Instance;

    public override void _Ready()
    {
        Instance = this;
    }

    public bool UseNavAgent { get; private set; } = true;

    public bool IsMoving { get; private set; } = false;
    public Vector2 TargetPosition { get; private set; } = Vector2.Zero;

    public void SetUseNavAgent(bool useNavAgent) => UseNavAgent = useNavAgent;
    public void SetIsMoving(bool isMoving) => IsMoving = isMoving;
    public void SetTargetPosition(Vector2 targetPosition) => TargetPosition = targetPosition;
}
