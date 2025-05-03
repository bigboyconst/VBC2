using Celeste.Mod.Entities;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.VBC2.Entities
{
    [CustomEntity("VBC2/SampleEntity")]
    public class SampleEntity : Entity 
    {
        public float Time { get; private set; }

        public Vector2 OriginalPos { get; private set; }

        public float Amplitude { get; private set; }

        public float Frequency { get; private set; }

        public SampleEntity(EntityData data, Vector2 offset)
            : base(data.Position + offset) 
        {
            Add(VBC2Module.Instance.SampleSpriteBank.Create("sampleEntity"));
            Collider = new Hitbox(16, 16, -8, -8);
            Time = 0f;
            OriginalPos = data.Position + offset;
            Amplitude = data.Float("oscillationAmplitude");
            Frequency = data.Float("oscillationSpeed");
        }

        //void OnPlayerTouch(Player player)
        //{
        //    // If the player is in fact colliding with this object, resolve the collision
        //    if (player.CollideCheck(this))
        //    {
        //        Vector2 resolve = Utils.Collision.Resolve(player, this);
        //        if (resolve != Vector2.Zero)
        //            player.Position += resolve;
        //    }
        //}

        public override void Render()
        {
            base.Render();
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            scene.Add(this);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            scene.Remove(this);
        }

        public override void Update()
        {
            base.Update();
            Time += Engine.DeltaTime;
            Position.Y = OriginalPos.Y + Amplitude * MathF.Sin(Frequency * Time);
        }
    }
}