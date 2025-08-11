using System;
using Monocle;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System.Reflection;
using MonoMod.Utils;

namespace Celeste.Mod.VBC2.Entities;

[CustomEntity("VBC2/SeekerSpring", "VBC2/SeekerSpringRight", "VBC2/SeekerSpringLeft", "VBC2/SeekerSpringDown")]
public class SeekerSpring : Spring
{
    private static FieldInfo spriteInfo = typeof(Spring).GetField("sprite", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
    private static FieldInfo playerCanUseInfo = typeof(Spring).GetField("playerCanUse", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

    const float PushForce = 200f;
    const float VelocityTolerance = 0f;

    public SeekerSpring(Vector2 position, Orientations orientation, bool playerCanUse)
        : base(position, orientation == (Orientations)3 ? Orientations.Floor : orientation, playerCanUse)
    {
        Orientation = orientation;

        DynData<Spring> selfData = new(this);

        Remove(Get<PlayerCollider>());
        Add(new PlayerCollider(OnCollide));
        SeekerCollider seekerCollider = new(OnSeeker);
        Add(seekerCollider);

        Sprite sprite = (Sprite)spriteInfo.GetValue(this);
        sprite.Reset(GFX.Game, "objects/spring/");
        sprite.Add("idle", "", 0f, default(int));
        sprite.Add("bounce", "", 0.07f, "idle", 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 4, 5);
        sprite.Add("disabled", "white", 0.07f);
        sprite.Origin.X = sprite.Width / 2;
        sprite.Origin.Y = sprite.Height;
        switch (orientation)
        {
            case Orientations.Floor:
                break;
            case Orientations.WallLeft:
                sprite.Rotation = (float)Math.PI / 2f;
                break;
            case Orientations.WallRight:
                sprite.Rotation = -(float)Math.PI / 2f;
                break;
            case (Orientations)3:
                selfData.Get<StaticMover>("staticMover").SolidChecker = s => CollideCheck(s, Position - Vector2.UnitY);
                selfData.Get<StaticMover>("staticMover").JumpThruChecker = jt => CollideCheck(jt, Position - Vector2.UnitY);
                Collider = new Hitbox(16, 6, -8, 0);
                Get<PufferCollider>().Collider = new Hitbox(16, 10, -8, 0);
                sprite.Rotation = (float)Math.PI;
                break;
            default:
                break;
        }
    }

    public SeekerSpring(EntityData data, Vector2 offset)
        : this(data.Position + offset, GetOrientationFromName(data.Name), data.Bool("playerCanUse", true))
    {

    }

    static Orientations GetOrientationFromName(string name)
    {
        return name switch
        {
            "up" or "VBC2/SeekerSpring" => Orientations.Floor,
            "right" or "VBC2/SeekerSpringRight" => Orientations.WallRight,
            "left" or "VBC2/SeekerSpringLeft" => Orientations.WallLeft,
            "down" or "VBC2/SeekerSpringDown" => (Orientations)3,
            _ => Orientations.Floor
        };
    }

    public override void Render()
    {
        base.Render();
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
    }

    public override void Update()
    {
        base.Update();
    }

    protected new void OnCollide(Player player)
    {
        if (player.StateMachine.State == 9 || !(bool)playerCanUseInfo.GetValue(this))
            return;

        if (Orientation == Orientations.Floor)
        {
            if (player.Speed.Y >= 0)
            {
                BounceAnimate();
                player.SuperBounce(Top);
            }
            return;
        }
        if (Orientation == Orientations.WallLeft)
        {
            if (player.SideBounce(1, Right, CenterY))
                BounceAnimate();
            return;
        }
        if (Orientation == Orientations.WallRight)
        {
            if (player.SideBounce(-1, Left, CenterY))
                BounceAnimate();
            return;
        }
        if (Orientation == (Orientations)3)
        {
            if (player.Speed.Y <= 0)
            {
                BounceAnimate();
                player.SuperBounce(Bottom + player.Height);
                DynData<Player> playerData = new(player);
                playerData["varJumpSpeed"] = player.Speed.Y = 185f;
                SceneAs<Level>().DirectionalShake(Vector2.UnitY, 0.1f);
            }
            return;
        }
    }

    public new void OnSeeker(Seeker seeker)
    {
        switch (Orientation)
        {
            case Orientations.Floor:
                if (seeker.Speed.Y >= -VelocityTolerance)
                {
                    BounceAnimate();
                    seeker.Speed.Y = -PushForce;
                }
                break;
            case Orientations.WallLeft:
                if (seeker.Speed.X <= VelocityTolerance)
                {
                    BounceAnimate();
                    seeker.Speed.X = PushForce;
                }
                break;
            case Orientations.WallRight:
                if (seeker.Speed.X >= -VelocityTolerance)
                {
                    BounceAnimate();
                    seeker.Speed.X = -PushForce;
                }
                break;
            case (Orientations)3:
                if (seeker.Speed.Y <= VelocityTolerance)
                {
                    BounceAnimate();
                    seeker.Speed.Y = PushForce;
                }
                break;
            default:
                throw new Exception("Orientation not supported");
        }
    }
}
