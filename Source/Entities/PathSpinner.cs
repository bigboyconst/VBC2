using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.VBC2.Utils;

namespace Celeste.Mod.VBC2.Entities
{
    /// <summary>
    /// Represents a spinner whose path is determined by a certain number of nodes.
    /// </summary>
    [CustomEntity("VBC2/PathSpinner")]
    public class PathSpinner : Entity
    {
        /// <summary>
        /// Whether or not the path should be closed.  If this is false, the 
        /// spinner will move back and forth along its path, but if this is 
        /// true, the spinner's path will be extended to connect the last 
        /// node to the first node.
        /// </summary>
        public bool ClosePath { get; set; }

        /// <summary>
        /// Whether the spinner should stop temporarily at each of the nodes on its path. If this 
        /// is set to false, the <see cref="EaseMode"/> option will be ignored.
        /// </summary>
        public bool StopAtNodes { get; set; }

        /// <summary>
        /// The easing mode to be used
        /// </summary>
        public EaseMode EaseMode { get; set; }

        /// <summary>
        /// The speed at which the spinner travels along its path.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Whether or not to smooth the path that the spinner takes.
        /// </summary>
        public bool SmoothPath { get; set; }

        /// <summary>
        /// The value of the parameter for this track spinner.
        /// </summary>
        public float Percent { get; private set; }

        /// <summary>
        /// The number of nodes in the spinner's path.
        /// </summary>
        public int NodeCount => Nodes.Length;

        public Vector2[] Nodes { get; private set; }

        public bool IsMovingForward { get; private set; }

        public bool IsMoving { get; private set; }

        readonly Ease.Easer easer;

        Spline spline;

        int StartIndex;
        int EndIndex;

        public PathSpinner(EntityData data, Vector2 offset) : base(data.Position + offset)
        {

            Collider = new ColliderList(new Circle(6f), new Hitbox(16f, 4f, -8f, -3f));
            Add(new PlayerCollider(OnPlayer));
            Add(VBC2Module.Instance.PandaSpriteBank.Create("templeBlade"));
            Nodes = data.NodesWithPosition(offset);
            Speed = data.Float("speed");
            Percent = 0f;
            IsMovingForward = true;
            IsMoving = true;
            StopAtNodes = data.Bool("stopAtNodes");
            EaseMode = data.Enum("easeMode", EaseMode.Both);
            ClosePath = data.Bool("closePath");
            SmoothPath = data.Bool("smoothPath");

            if (SmoothPath)
            {
                spline = SplineInterpolation.Interpolate(Nodes, ClosePath ? SplineType.Closed : SplineType.Natural);
            }

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


        public virtual void OnPlayer(Player player)
        {
            if (player.Die((player.Position - Position).SafeNormalize()) != null)
                IsMoving = false;
        }

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
            if (!ClosePath && ((IsMovingForward && VBC2Math.Approximately(Percent, NodeCount - 1)) || (!IsMovingForward && VBC2Math.Approximately(Percent, 0f))))
            {
                IsMovingForward = !IsMovingForward;
                OnPathEnd();
            }
        }

        public virtual void OnPathStart()
        {
            if (SmoothPath && (spline.xSegments.Length == 0 || spline.ySegments.Length == 0))
            {
                spline = SplineInterpolation.Interpolate(Nodes, ClosePath ? SplineType.Closed : SplineType.Natural);
            }
        }

        public virtual void OnPathEnd()
        {

        }

        public override void Render()
        {
            base.Render();
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
        }
    }
}
