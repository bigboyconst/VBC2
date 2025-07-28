using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.VBC2.Utils;
using Celeste.Mod.DBBHelper;
using Celeste.Mod.DBBHelper.Entities;

namespace Celeste.Mod.VBC2.Entities
{
    /// <summary>
    /// Represents a light source that moves along a path defined by nodes.
    /// </summary>
    [CustomEntity("VBC2/PathLightSource")]
    public class PathLightSource : InvisibleLight
    {
        /// <inheritdoc cref="PathSpinner.ClosePath"/>
        public bool ClosePath { get; set; }

        /// <inheritdoc cref="PathSpinner.StopAtNodes">
        public bool StopAtNodes { get; set; }

        /// <inheritdoc cref="PathSpinner.EaseMode"/>
        public EaseMode EaseMode { get; set; }

        /// <inheritdoc cref="PathSpinner.Speed"/>
        public float Speed { get; set; }

        /// <inheritdoc cref="PathSpinner.SmoothPath"/>
        public bool SmoothPath { get; set; }

        /// <inheritdoc cref="PathSpinner.Percent"/>
        public float Percent { get; private set; }

        /// <inheritdoc cref="PathSpinner.NodeCount"/>
        public int NodeCount => Nodes.Length;

        public Vector2[] Nodes { get; private set; }

        public bool IsMovingForward { get; private set; }

        public bool IsMoving { get; private set; }

        readonly Ease.Easer easer;

        Spline spline;

        int StartIndex;
        int EndIndex;

        public PathLightSource(EntityData data, Vector2 offset) : base(data, offset)
        {
            Nodes = data.NodesWithPosition(offset);
            Speed = data.Float("movementSpeed");
            Percent = 0f;
            IsMovingForward = true;
            IsMoving = true;
            StopAtNodes = data.Bool("stopAtNodes");
            EaseMode = data.Enum("easeMode", EaseMode.Both);
            ClosePath = data.Bool("closePath");
            SmoothPath = data.Bool("smoothPath");

            if (SmoothPath)
                spline = SplineInterpolation.Interpolate(Nodes, ClosePath ? SplineType.Closed : SplineType.Natural);

            if (!StopAtNodes)
                easer = Ease.Linear;
            else
            {
                easer = EaseMode switch
                {
                    EaseMode.In => Ease.QuadIn,
                    EaseMode.Out => Ease.QuadOut,
                    EaseMode.Both => Ease.QuadInOut,
                    _ => Ease.QuadInOut
                };
            }

            StartIndex = 0;
            EndIndex = 1;
        }

        #region Spline Logic

        void UpdateEndpoints()
        {
            /*
            [MOVING FORWARD LOGIC]
            - If MovingForwards and Percent - StartIndex >= 1 (we've passed the current end node)
                - StartIndex++
                - If EndIndex == NodeCount - 1 (we are at the end of the curve)
                    - If ClosePath
                        - EndIndex = 0 (go back to the start)
                    - Else
                        - EndIndex = NodeCount - 2 (start moving backwards)
                        - MovingForwards = false
                - Else
                    - EndIndex++
            [MOVING BACKWARDS LOGIC]
            - If not MovingForwards and StartIndex - Percent >= 1 (we've passed the end point)
                - StartIndex--
                - If EndIndex == 0
                    - EndIndex = 1
                    - MovingForwards = true
                - Else
                    - EndIndex--
            */
            if (IsMovingForward && Percent - StartIndex >= 1f)
            {
                StartIndex++;
                if (EndIndex == NodeCount - 1)
                {
                    if (ClosePath)
                        EndIndex = 0;
                    else
                    {
                        EndIndex--;
                        IsMovingForward = false;
                    }
                }
                else
                    EndIndex++;
            }
            else if (!IsMovingForward && StartIndex - Percent >= 1f)
            {
                StartIndex--;
                if (EndIndex == 0)
                {
                    EndIndex = 1;
                    IsMovingForward = true;
                }
                else
                    EndIndex--;
            }
        }

        float EasedPercent(float percent) => (NodeCount - 1) * easer(percent / (NodeCount - 1));

        Vector2 GetNextPosition()
        {
            Vector2 res;
            Vector2 start = Nodes[StartIndex];
            Vector2 end = Nodes[EndIndex];
            float t = IsMovingForward ? Percent - StartIndex : StartIndex - Percent;
            float easedPercent = EasedPercent(Percent);
            float t2 = IsMovingForward ? StartIndex + easer(t) : StartIndex - easer(t);
            if (!SmoothPath)
            {
                res = Vector2.Lerp(start, end, easer(t));
            }
            else
            {
                Vector2 v = spline.Evaluate(easedPercent);
                if (ClosePath)
                    v = spline.Evaluate(NodeCount * easer(Percent / NodeCount));
                res = StopAtNodes ? spline.Evaluate(t2) : v;
            }
            return res;
        }

        public void UpdatePosition()
        {
            UpdateEndpoints();
            Position = GetNextPosition();
        }

        public virtual void OnPathStart()
        {
            if (SmoothPath && (spline.xSegments.Length == 0 || spline.ySegments.Length == 0))
            {
                spline = SplineInterpolation.Interpolate(Nodes, ClosePath ? SplineType.Closed : SplineType.Natural);
            }
        }

        #endregion

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            OnPathStart();
        }

        public override void Update()
        {
            base.Update();

            if (!IsMoving)
                return;
            Percent += (IsMovingForward ? 1f : -1f) * Engine.DeltaTime * Speed;
            if (Percent >= NodeCount)
            {
                StartIndex = 0;
                EndIndex = 1;
            }
            Percent = MathF.Max(Percent, 0f);
            Percent %= NodeCount;
            UpdatePosition();
            if (!ClosePath && IsAtEndOfPath())
            {
                IsMovingForward = !IsMovingForward;
            }
        }

        public bool IsAtEndOfPath() => (IsMovingForward && VBC2Math.Approximately(Percent, NodeCount - 1)) || (!IsMovingForward && VBC2Math.Approximately(Percent, 0f));

        public override void Render()
        {
            base.Render();
        }
    }
}
