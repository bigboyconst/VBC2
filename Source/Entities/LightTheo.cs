using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.VBC2.Utils;
using Celeste.Mod.DBBHelper;
using Celeste.Mod.DBBHelper.Entities;

namespace Celeste.Mod.VBC2.Entities;

[CustomEntity("VBC2/CustomLightTheoCrystal")]
public class CustomLightTheoCrystal : TheoCrystal
{
    InvisibleLight light;
    Vector2 verticalShift = new(0, -10);

    public CustomLightTheoCrystal(EntityData data, Vector2 offset) : base(data, offset)
    {
        // this is so stupid
        light = new(data, offset + verticalShift);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        light.Added(scene);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        light.Awake(scene);
        light.Position = Position + verticalShift;
    }

    public override void Update()
    {
        base.Update();
        light.Update();
        light.Position = Position + verticalShift;
    }

    public override void Render()
    {
        light.Render();
        base.Render();
    }

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        light.Removed(scene);
    }
}
