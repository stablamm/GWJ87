using Godot;
using GWJ87.Globals;
using GWJ87.Scenes.Effects;
using System.Collections.Generic;

namespace GWJ87.Scenes.Entities.Cats;

public partial class Cat : CharacterBody2D
{
    public enum CAT_COLORS
    {
        DEFAULT,
        BLACK,
        GREY,
        GREYWHITE,
        ORANGE,
        WHITE
    }

    public enum CAT_ANIMATIONS
    {
        IDLE,
        EXCITED,
        FULL,
        SLEEPING,
        PLAYFUL,
        RUNNING,
        JUMPING,
        BOX_1_UP,
        BOX_1_DOWN,
        BOX_2,
        BOX_3_UP,
        BOX_3_DOWN,
        CRYING,
        DANCING,
        CHILLING,
        SURPRISED,
        TICKLE,
        DEAD_1,
        DEAD_2,
        HURT,
        ATTACK
    }

    public enum CAT_DIRECTION
    {
        LEFT,
        RIGHT
    }

    public enum CAT_STATE
    {
        IDLING,
        MOVING,
        PLAYING,
        SLEEPING,
        FEEDING,
        FULL,
        CLEANING
    }

    [Export] public CAT_COLORS CurrentColor { get; private set; } = CAT_COLORS.DEFAULT;
    [Export] public float Speed { get; set; } = 100f;

    public CAT_ANIMATIONS CurrentAnimation { get; private set; } = CAT_ANIMATIONS.IDLE;
    public CAT_DIRECTION CurrentDirection { get; private set; } = CAT_DIRECTION.RIGHT;
    public CAT_STATE CurrentState { get; set; } = CAT_STATE.IDLING;

    private Dictionary<CAT_COLORS, Texture2D> ColorToSprite = new()
    {
        { CAT_COLORS.DEFAULT, GD.Load<Texture2D>("res://Assets/Cats/AllCats.png") },
        { CAT_COLORS.BLACK, GD.Load<Texture2D>("res://Assets/Cats/AllCatsBlack.png") },
        { CAT_COLORS.GREY, GD.Load<Texture2D>("res://Assets/Cats/AllCatsGrey.png") },
        { CAT_COLORS.GREYWHITE, GD.Load<Texture2D>("res://Assets/Cats/AllCatsGreyWhite.png") },
        { CAT_COLORS.ORANGE, GD.Load<Texture2D>("res://Assets/Cats/AllCatsOrange.png") },
        { CAT_COLORS.WHITE, GD.Load<Texture2D>("res://Assets/Cats/AllCatsWhite.png") }
    };

    private bool rainInArea = false;

    private Sprite2D Sprite;
    private AnimationPlayer AnimPlayer;
    private NavigationAgent2D NavAgent;
    private Timer IdleTimer;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite2D>("%Sprite");
        AnimPlayer = GetNode<AnimationPlayer>("%AnimPlayer");
        NavAgent = GetNode<NavigationAgent2D>("%NavAgent");
        IdleTimer = GetNode<Timer>("%IdleTimer");
        SetColor(CurrentColor);
        AnimPlayer.Play(CurrentAnimation.ToString());

        SignalManager.Instance.AttemptClean += OnAttemptClean;
        SignalManager.Instance.ManuallySetCatAnimation += SetAnimation;
        SignalManager.Instance.ManuallySetCatState += SetState;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CatManager.Instance.IsMoving)
        {
            if (CatManager.Instance.UseNavAgent)
            {
                ProcessNavAgent();
            }
            else
            {
                ProcessManualMovement();
            }
        }
        else
        {
            // If we're not idling, and the timer is stopped, start it.
            // Else if the timer is running and we're not idling, stop it.
            if (IdleTimer.TimeLeft == 0 && CurrentState == CAT_STATE.IDLING)
                IdleTimer.Start();
            else if (IdleTimer.TimeLeft > 0 && CurrentState != CAT_STATE.IDLING)
                IdleTimer.Stop();

            if (CurrentState == CAT_STATE.FEEDING)
            {
                HandleFeedingRoutine();
            }
            else if (CurrentState == CAT_STATE.FULL)
            {
                HandleFullRoutine();
            }
            else if (CurrentState == CAT_STATE.SLEEPING)
            {
                HandleSleepingRoutine();
            }
            else if (CurrentState == CAT_STATE.CLEANING)
            {
                HandleCleanRoutine();
            }
        }
    }

    private void OnIdleTimerTimeout()
    {
        SignalManager.Instance.EmitRequestRandomPosition();
    }

    private void OnAttemptClean()
    {
        if (rainInArea)
            RoutineManager.Instance.Clean();
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.GetParent().GetType() == typeof(RainParticleEffects))
        {
            rainInArea = true;
        }
    }

    private void OnAreaExited(Area2D area)
    {
        if (area.GetParent().GetType() == typeof(RainParticleEffects))
        {
            rainInArea = false;
        }
    }

    public void SetColor(CAT_COLORS newColor)
    {
        CurrentColor = newColor;
        Sprite.Texture = ColorToSprite[newColor];
    }

    public void SetAnimation(CAT_ANIMATIONS newAnimation)
    {
        if (CurrentAnimation == newAnimation) return;
        CurrentAnimation = newAnimation;
        FlipSprite();
        AnimPlayer.Play(newAnimation.ToString());
    }

    public void SetTargetPosition(Vector2 targetPos, bool useNavAgent)
    {
        CatManager.Instance.SetUseNavAgent(useNavAgent);

        if (CatManager.Instance.UseNavAgent)
        {
            NavAgent.TargetPosition = targetPos;
        }
        else
        {
            CatManager.Instance.SetTargetPosition(targetPos);
        }

        SetAnimation(CAT_ANIMATIONS.RUNNING);
        CatManager.Instance.SetIsMoving(true);
    }

    public void SetState(CAT_STATE newState)
    {
        if (CurrentState != newState)
            CurrentState = newState;
    }

    // Will only flip sprite if the animation needs it,
    // but call this everytime you change animation or direction
    public void FlipSprite()
    {
        if (CurrentAnimation == CAT_ANIMATIONS.IDLE
            || CurrentAnimation == CAT_ANIMATIONS.EXCITED
            || CurrentAnimation == CAT_ANIMATIONS.FULL
            || CurrentAnimation == CAT_ANIMATIONS.SLEEPING
            || CurrentAnimation == CAT_ANIMATIONS.PLAYFUL
            || CurrentAnimation == CAT_ANIMATIONS.BOX_1_UP
            || CurrentAnimation == CAT_ANIMATIONS.BOX_1_DOWN
            || CurrentAnimation == CAT_ANIMATIONS.BOX_2
            || CurrentAnimation == CAT_ANIMATIONS.BOX_3_UP
            || CurrentAnimation == CAT_ANIMATIONS.BOX_3_DOWN
            || CurrentAnimation == CAT_ANIMATIONS.CRYING
            || CurrentAnimation == CAT_ANIMATIONS.DANCING
            || CurrentAnimation == CAT_ANIMATIONS.CHILLING
            || CurrentAnimation == CAT_ANIMATIONS.SURPRISED
            || CurrentAnimation == CAT_ANIMATIONS.TICKLE)
        {
            if (CurrentDirection == CAT_DIRECTION.LEFT)
            {
                Sprite.FlipH = false;
            }
            else
            {
                Sprite.FlipH = true;
            }
        }
        else if (CurrentAnimation == CAT_ANIMATIONS.RUNNING
                || CurrentAnimation == CAT_ANIMATIONS.JUMPING
                || CurrentAnimation == CAT_ANIMATIONS.DEAD_1
                || CurrentAnimation == CAT_ANIMATIONS.DEAD_2
                || CurrentAnimation == CAT_ANIMATIONS.HURT
                || CurrentAnimation == CAT_ANIMATIONS.ATTACK)
        {
            if (CurrentDirection == CAT_DIRECTION.LEFT)
            {
                Sprite.FlipH = true;
            }
            else
            {
                Sprite.FlipH = false;
            }
        }
    }

    public void StartFeedRoutine() => CurrentState = CAT_STATE.FEEDING;
    public void StartSleepRoutine() => CurrentState = CAT_STATE.SLEEPING;
    public bool CanChangeRoutine() => CurrentState != CAT_STATE.FULL;
    public void StartCleanRoutine() => CurrentState = CAT_STATE.CLEANING;

    private void ProcessNavAgent()
    {
        if (!NavAgent.IsNavigationFinished())
        {
            Vector2 next = NavAgent.GetNextPathPosition();
            Vector2 dir = GlobalPosition.DirectionTo(next);
            Velocity = dir * Speed;
            MoveAndSlide();

            if (NavAgent.GetNextPathPosition().X < GlobalPosition.X)
            {
                CurrentDirection = CAT_DIRECTION.LEFT;
            }
            else if (NavAgent.GetNextPathPosition().X > GlobalPosition.X)
            {
                CurrentDirection = CAT_DIRECTION.RIGHT;
            }

            FlipSprite();
        }
        else
        {
            Velocity = Vector2.Zero;
            SetAnimation(CAT_ANIMATIONS.IDLE);
            SignalManager.Instance.EmitCatDestinationReached();
            CatManager.Instance.SetIsMoving(false);
        }
    }

    private void ProcessManualMovement()
    {
        Vector2 dir = GlobalPosition.DirectionTo(CatManager.Instance.TargetPosition);
        Velocity = dir * Speed;
        MoveAndSlide();

        if (CatManager.Instance.TargetPosition.X < GlobalPosition.X)
        {
            CurrentDirection = CAT_DIRECTION.LEFT;
        }
        else if (CatManager.Instance.TargetPosition.X > GlobalPosition.X)
        {
            CurrentDirection = CAT_DIRECTION.RIGHT;
        }

        FlipSprite();

        // If we are within 5 pixels of the target position, stop moving
        if (GlobalPosition.DistanceTo(CatManager.Instance.TargetPosition) < 5f)
        {
            CatManager.Instance.SetTargetPosition(Vector2.Zero);
            Velocity = Vector2.Zero;
            SetAnimation(CAT_ANIMATIONS.IDLE);
            SignalManager.Instance.EmitCatDestinationReached();
            CatManager.Instance.SetIsMoving(false);
        }
    }

    private void HandleFeedingRoutine()
    {
        if (RoutineManager.Instance.CurrentRoutine != RoutineManager.CURRENT_ROUTINE.FEEDING)
        {
            RoutineManager.Instance.StopRoutine();
            RoutineManager.Instance.StartRoutine(RoutineManager.CURRENT_ROUTINE.FEEDING);
        }

        if (GameStats.Instance.Hunger == 0)
        {
            CurrentState = CAT_STATE.FULL;
            SetAnimation(CAT_ANIMATIONS.FULL);
            RoutineManager.Instance.StopRoutine();
        }
    }

    private void HandleSleepingRoutine()
    {
        if (RoutineManager.Instance.CurrentRoutine != RoutineManager.CURRENT_ROUTINE.SLEEPING)
        {
            RoutineManager.Instance.StopRoutine();
            RoutineManager.Instance.StartRoutine(RoutineManager.CURRENT_ROUTINE.SLEEPING);
        }

        if (!CatManager.Instance.IsMoving && CurrentAnimation != CAT_ANIMATIONS.SLEEPING)
        {
            SetAnimation(CAT_ANIMATIONS.SLEEPING);
        }

        if (GameStats.Instance.Energy >= 100)
        {
            CurrentState = CAT_STATE.IDLING;
            SetAnimation(CAT_ANIMATIONS.IDLE);
            SignalManager.Instance.EmitRequestRandomPosition();
            RoutineManager.Instance.StopRoutine();
        }
    }

    private void HandleFullRoutine()
    {
        if (GameStats.Instance.Hunger > 5)
        {
            CurrentState = CAT_STATE.IDLING;
            SetAnimation(CAT_ANIMATIONS.IDLE);
        }
    }

    private void HandleCleanRoutine()
    {
        if (RoutineManager.Instance.CurrentRoutine != RoutineManager.CURRENT_ROUTINE.CLEANING)
        {
            RoutineManager.Instance.StopRoutine();
            RoutineManager.Instance.StartRoutine(RoutineManager.CURRENT_ROUTINE.CLEANING);
        }
    }
}
