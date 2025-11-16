using Godot;
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


    [Export] public CAT_COLORS CurrentColor { get; private set; } = CAT_COLORS.DEFAULT;
    [Export] public float Speed { get; set; } = 100f;

    public CAT_ANIMATIONS CurrentAnimation { get; private set; } = CAT_ANIMATIONS.IDLE;
    public CAT_DIRECTION CurrentDirection { get; private set; } = CAT_DIRECTION.RIGHT;

    private Dictionary<CAT_COLORS, Texture2D> ColorToSprite = new()
    {
        { CAT_COLORS.DEFAULT, GD.Load<Texture2D>("res://Assets/Cats/AllCats.png") },
        { CAT_COLORS.BLACK, GD.Load<Texture2D>("res://Assets/Cats/AllCatsBlack.png") },
        { CAT_COLORS.GREY, GD.Load<Texture2D>("res://Assets/Cats/AllCatsGrey.png") },
        { CAT_COLORS.GREYWHITE, GD.Load<Texture2D>("res://Assets/Cats/AllCatsGreyWhite.png") },
        { CAT_COLORS.ORANGE, GD.Load<Texture2D>("res://Assets/Cats/AllCatsOrange.png") },
        { CAT_COLORS.WHITE, GD.Load<Texture2D>("res://Assets/Cats/AllCatsWhite.png") }
    };

    private Sprite2D Sprite;
    private AnimationPlayer AnimPlayer;
    private NavigationAgent2D NavAgent;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite2D>("%Sprite");
        AnimPlayer = GetNode<AnimationPlayer>("%AnimPlayer");
        NavAgent = GetNode<NavigationAgent2D>("%NavAgent");
        SetColor(CurrentColor);
    }

    public override void _PhysicsProcess(double delta)
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

            GD.Print($"CurrentDirection: {CurrentDirection}");
            FlipSprite();
        }
        else
        {
            Velocity = Vector2.Zero;
            SetAnimation(CAT_ANIMATIONS.IDLE);

            SignalManager.Instance.EmitCatDestinationReached();
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

    public void SetTargetPosition(Vector2 targetPos)
    {
        NavAgent.TargetPosition = targetPos;
        SetAnimation(CAT_ANIMATIONS.RUNNING);
    }

    // Will only flip sprite if the animation needs it, but call this everytime you change animation or direction
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
}
