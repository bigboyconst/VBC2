using System;
using Monocle;
using Microsoft.Xna.Framework;
using System.Linq;
using FMOD;
using System.Collections.Generic;
using System.Text;


namespace Celeste.Mod.VBC2.Utils
{
    /// <summary>
    /// Represents one of the cubic polynomials of the form <c>a + b(t - x) + c(t - x)^2 + d(t - x)^3</c> that are part of a spline.
    /// </summary>
    public struct SplineSegment
    {
        public float a, b, c, d;
        public float x;

        public readonly float Evaluate(float t) => a + b * (t - x) + c * MathF.Pow(t - x, 2f) + d * MathF.Pow(t - x, 3f);

        public readonly override string ToString()
        {
            return $"f(t) = {a} + {b}(t - {x}) + {c}(t - {x})^2 + {d}(t - {x})^3";
        }
    }

    /// <summary>
    /// Represents a cubic spline.
    /// </summary>
    public struct Spline
    {
        public SplineSegment[] xSegments;
        public SplineSegment[] ySegments;
        public SplineType type;

        public readonly Vector2 Evaluate(float x)
        {
            int index = (int)MathF.Floor(x);
            if (index >= xSegments.Length || index >= ySegments.Length)
                index = (int)MathF.Min(xSegments.Length, ySegments.Length) - 1;
            return new(xSegments[index].Evaluate(x), ySegments[index].Evaluate(x));
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            for (int i = 0; i < xSegments.Length; i++)
                sb.AppendLine($"p{i + 1}(t) = ({xSegments[i].ToString()[7..]}, {ySegments[i].ToString()[7..]})");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents a type of spline, either closed or natural
    /// </summary>
    public enum SplineType
    {
        Closed,
        Natural
    }

    /// <summary>
    /// Class allowing interpolation with splines given control points
    /// </summary>
    public static class SplineInterpolation
    {
        public static Vector2[] ToVectorArray(float[] x, float[] y)
        {
            Vector2[] arr = new Vector2[x.Length];
            for (int i = 0; i < x.Length; i++)
                arr[i] = new(x[i], y[i]);
            return arr;
        }
        /// <summary>
        /// Interpolates the given points with a cubic spline of type <paramref name="type"/>.
        /// </summary>
        public static Spline Interpolate(Vector2[] points, SplineType type = SplineType.Natural)
        {
            var x = points.Select(p => p.X).ToArray();
            var y = points.Select(p => p.Y).ToArray();
            var t = Enumerable.Range(0, points.Length).Select(i => (float)i).ToArray();

            var p1 = ToVectorArray(t, x);
            var p2 = ToVectorArray(t, y);

            if (type == SplineType.Closed)
            {
                var xSegments = ComputeClosed(p1);
                var ySegments = ComputeClosed(p2);
                return new Spline { type = type, xSegments = xSegments, ySegments = ySegments };
            }
            else
            {
                var xSegments = ComputeNatural(p1);
                var ySegments = ComputeNatural(p2);
                return new Spline { type = type, xSegments = xSegments, ySegments = ySegments };
            }
        }

        static SplineSegment[] ComputeClosed(Vector2[] points)
        {
            float[] x = points.Select(p => p.X).ToArray();
            float[] y = points.Select(p => p.Y).ToArray();
            int n = points.Length;
            float[] h = new float[n];
            for (int i = 0; i < n - 1; i++)
                h[i] = x[i + 1] - x[i];
            h[n - 1] = x[0] + (x[n - 1] - x[n - 2]) - x[n - 1]; // wrap

            float[] alpha = new float[n];
            for (int i = 1; i < n - 1; i++)
            {
                alpha[i] = (3f / h[i]) * (y[i + 1] - y[i]) - (3f / h[i - 1]) * (y[i] - y[i - 1]);
            }
            alpha[0] = (3f / h[0]) * (y[1] - y[0]) - (3f / h[n - 1]) * (y[0] - y[n - 1]);
            alpha[n - 1] = (3f / h[n - 1]) * (y[0] - y[n - 1]) - (3f / h[n - 2]) * (y[n - 1] - y[n - 2]);

            float[] aDiag = new float[n];
            float[] bDiag = new float[n];
            float[] cDiag = new float[n];
            for (int i = 0; i < n; i++)
            {
                aDiag[i] = i == 0 ? h[n - 1] : h[i - 1];
                bDiag[i] = 2f * (h[i % n] + h[i == 0 ? n - 1 : i - 1]);
                cDiag[i] = h[i % n];
            }

            float[] u = new float[n];
            float[] v = new float[n];
            u[0] = 1; u[n - 1] = 1;
            v[0] = 1; v[n - 1] = -1;

            float[] m = SolveCyclicTridiagonal(aDiag, bDiag, cDiag, alpha, u, v);

            var segments = new SplineSegment[n];
            for (int i = 0; i < n; i++)
            {
                int ip1 = (i + 1) % n;
                float hi = h[i];
                float a = y[i];
                float c0 = m[i];
                float c1 = m[ip1];
                float d = (c1 - c0) / (3f * hi);
                float b = ((y[ip1] - y[i]) / hi) - (hi * (2f * c0 + c1)) / 3f;

                segments[i] = new SplineSegment
                {
                    a = a,
                    b = b,
                    c = c0,
                    d = d,
                    x = x[i]
                };
            }
            return segments;
        }

        static float[] SolveCyclicTridiagonal(float[] a, float[] b, float[] c, float[] d, float[] u, float[] v)
        {
            int n = b.Length;
            float[] x = SolveTridiagonal(a, b, c, d);
            float[] z = SolveTridiagonal(a, b, c, u);

            float vTz = 0, vTx = 0;
            for (int i = 0; i < n; i++)
            {
                vTz += v[i] * z[i];
                vTx += v[i] * x[i];
            }

            float factor = vTx / (1f + vTz);
            for (int i = 0; i < n; i++)
                x[i] -= factor * z[i];

            return x;
        }

        static float[] SolveTridiagonal(float[] a, float[] b, float[] c, float[] d)
        {
            int n = b.Length;
            float[] cp = new float[n];
            float[] dp = new float[n];
            float[] x = new float[n];

            cp[0] = c[0] / b[0];
            dp[0] = d[0] / b[0];
            for (int i = 1; i < n; i++)
            {
                float denom = b[i] - a[i] * cp[i - 1];
                cp[i] = i < n - 1 ? c[i] / denom : 0;
                dp[i] = (d[i] - a[i] * dp[i - 1]) / denom;
            }

            x[n - 1] = dp[n - 1];
            for (int i = n - 2; i >= 0; i--)
                x[i] = dp[i] - cp[i] * x[i + 1];

            return x;
        }

        static SplineSegment[] ComputeNatural(Vector2[] points)
        {
            float[] x = points.Select(p => p.X).ToArray();
            float[] y = points.Select(p => p.Y).ToArray();
            int n = points.Length;
            float[] h = new float[n - 1];
            for (int i = 0; i < n - 1; i++)
                h[i] = x[i + 1] - x[i];

            float[] alpha = new float[n - 1];
            for (int i = 1; i < n - 1; i++)
                alpha[i] = (3f / h[i]) * (y[i + 1] - y[i]) - (3f / h[i - 1]) * (y[i] - y[i - 1]);

            float[] l = new float[n], mu = new float[n], z = new float[n];
            l[0] = 1;
            z[0] = mu[0] = 0;

            for (int i = 1; i < n - 1; i++)
            {
                l[i] = 2 * (x[i + 1] - x[i - 1]) - h[i - 1] * mu[i - 1];
                mu[i] = h[i] / l[i];
                z[i] = (alpha[i] - h[i - 1] * z[i - 1]) / l[i];
            }

            l[n - 1] = 1;
            z[n - 1] = 0;

            float[] c = new float[n];
            float[] b = new float[n - 1];
            float[] d = new float[n - 1];

            for (int j = n - 2; j >= 0; j--)
            {
                c[j] = z[j] - mu[j] * c[j + 1];
                b[j] = (y[j + 1] - y[j]) / h[j] - h[j] * (c[j + 1] + 2 * c[j]) / 3f;
                d[j] = (c[j + 1] - c[j]) / (3 * h[j]);
            }

            var segments = new SplineSegment[n - 1];
            for (int i = 0; i < n - 1; i++)
                segments[i] = new SplineSegment
                {
                    a = y[i],
                    b = b[i],
                    c = c[i],
                    d = d[i],
                    x = x[i]
                };
            return segments;
        }
    }
}
