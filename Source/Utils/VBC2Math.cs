using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.VBC2.Utils
{
    public static class VBC2Math
    {
        public static bool Approximately(float a, float b)
            => Math.Abs(b - a) < Math.Max(1E-06f * Math.Max(Math.Abs(a), Math.Abs(b)), float.Epsilon * 8f);
    }
}
