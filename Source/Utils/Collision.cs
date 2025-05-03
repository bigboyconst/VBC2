using System;
using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.VBC2.Utils
{
    public static class Collision
    {
        public static Vector2 Resolve(Player player, Entity obstacle)
        {
            Vector2 res = Vector2.Zero;
            bool foundSomething = false;

            while (player.CollideCheck(obstacle))
            {
                if (player.Speed.X > 0)
                {
                    res.X -= 1;
                    foundSomething = true;
                }
                else if (player.Speed.X < 0)
                {
                    res.X += 1;
                    foundSomething = true;
                }
                
                if (player.Speed.Y > 0)
                {
                    res.Y -= 1;
                    foundSomething = true;
                }
                else if (player.Speed.Y < 0)
                {
                    res.Y += 1;
                    foundSomething = true;
                }

                if (!foundSomething)
                    break;
            }
            return res;
        }
    }
}
