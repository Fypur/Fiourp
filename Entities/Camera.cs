using Microsoft.Xna.Framework;
using System;

namespace Fiourp
{
    public class Camera : Actor
    {
        public new Rectangle Bounds = Rectangle.Empty;
        public Rectangle StrictFollowBounds => new Rectangle(new Vector2(-Engine.RenderTarget.Width / (float)2, -Engine.RenderTarget.Height / (float)4).ToPoint(), new Vector2(Engine.RenderTarget.Width, Engine.RenderTarget.Height / 2).ToPoint());

        private bool hasChanged;
        public bool FollowsPlayer;
        public bool Locked;

        private Timer moveTimer;

        private Vector2 inBoundsOffset;
        public Vector2 InBoundsOffset
        {
            get => inBoundsOffset;
            set
            {
                if (value != inBoundsOffset)
                    hasChanged = true;
                inBoundsOffset = value;
            }
        }

        private Vector2 trueOffset;
        public Vector2 TrueOffset
        {
            get => trueOffset;
            set
            {
                if (value != trueOffset)
                    hasChanged = true;
                trueOffset = value;
            }
        }

        public bool RenderTargetMode { get => Size == new Vector2(Engine.RenderTarget.Width, Engine.RenderTarget.Height);
            set
            {
                if (value) Size = new Vector2(Engine.RenderTarget.Width, Engine.RenderTarget.Height);
                else Size = Engine.ScreenSize;
            }
        }

        public new Vector2 Pos
        {
            get => base.Pos;
            set
            {
                base.Pos = InBoundsPos(value + Size / 2, out bool changed) - Size / 2;
                if (changed)
                    hasChanged = true;
            }
        }

        public Vector2 CenteredPos
        {
            get => base.Pos + Size / 2;

            set
            {
                base.Pos = InBoundsPos(value, out bool changed) - Size / 2;
                if(changed)
                    hasChanged = true;
            }
        }

        public Vector2 NoBoundsPos
        {
            get => base.Pos + Size / 2;
            set
            {
                if(base.Pos - Size / 2 == value) return;
                base.Pos = value - Size / 2;
                hasChanged = true;
            }
        }

        public override Vector2 ExactPos { get => base.Pos + Size / 2; set => CenteredPos = value; }

        public Vector2 WholePos => VectorHelper.Round(Pos);

        private float rot;
        public float Rotation
        {
            get => rot;
            set
            {
                hasChanged = true;
                rot = value % 360 + (value < 0 ? 360 : 0);
            }
        }

        private float zoom;
        public float ZoomLevel
        {
            get => zoom;
            set
            {
                hasChanged = true;
                zoom = value;
            }
        }

        private Matrix view;
        public Matrix ViewMatrix
        {
            get
            {
                if (hasChanged)
                {
                    hasChanged = false;
                    return view = Matrix.CreateTranslation(new Vector3(-VectorHelper.Round(WholePos + trueOffset), 0.0f)) *
                           Matrix.CreateScale(ZoomLevel) *
                           Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation));
                }
                else
                    return view;
            }
        }

        public Matrix InverseViewMatrix
        {
            get => Matrix.Invert(ViewMatrix);
        }

        public float ScreenSizeCoef { get => Engine.ScreenSize.X / Width; }

        public Camera(Vector2 position, float rotation, float zoomLevel, Rectangle? bounds = null)
            : base(position, (int)Engine.ScreenSize.X, (int)Engine.ScreenSize.Y, null)
        {
            Engine.Cam = this;

            Pos = position;
            Rotation = rotation;
            ZoomLevel = zoomLevel;

            if (bounds != null)
                SetBoundaries((Rectangle)bounds);
        }
        
        public override void Update()
        {
            base.Update();

            if (Engine.Player != null && FollowsPlayer && !Locked && (moveTimer == null || moveTimer.Value <= 0))
            {
                Follow(Engine.Player, 4, 4, StrictFollowBounds);
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            PreviousPos = WholePos;
        }

        public void Refresh()
            => hasChanged = true;

        public void Follow(Entity actor, float xSmooth, float ySmooth, Rectangle strictFollowBounds)
        {
            Vector2 amount = FollowedPos(actor, xSmooth, ySmooth, strictFollowBounds, Bounds) - CenteredPos;
            Vector2 previous = ExactPos;

            //Debug.LogUpdate(FollowedPos(actor, xSmooth, ySmooth, strictFollowBounds, Bounds));
            //if(Math.Abs(amount.X) >= 0.1f)
                MoveX(amount.X, new System.Collections.Generic.List<Entity>(Engine.CurrentMap.Data.CameraSolids), null);
            //if (Math.Abs(amount.Y) >= 0.1f)
                MoveY(amount.Y, new System.Collections.Generic.List<Entity>(Engine.CurrentMap.Data.CameraSolids), null);

            /*if (HasComponent<Shaker>())
            {
                //ExactPos = previous;
                shakerInitPos += ExactPos - previous;
            }*/

            if (!Bounds.Contains(WholePos + Vector2.One) || !Bounds.Contains(WholePos + Size - Vector2.One)) {
                CenteredPos = FollowedPos(actor, xSmooth, ySmooth, strictFollowBounds, Bounds);
                hasChanged = true;
            }

            if (amount != Vector2.Zero)
                hasChanged = true;
        }

        public Vector2 FollowedPos(Entity followed, float xSmooth, float ySmooth, Rectangle strictFollowBounds, Rectangle bounds)
        {
            strictFollowBounds.Location += CenteredPos.ToPoint();
            Vector2 inBoundsActorPos = InBoundsPos(InBoundsPos(followed.MiddlePos, bounds) + inBoundsOffset, bounds);

            return new Vector2(
                MathHelper.Lerp(CenteredPos.X, inBoundsActorPos.X, Engine.Deltatime * xSmooth),
                MathHelper.Lerp(CenteredPos.Y, inBoundsActorPos.Y, 
                    Engine.Deltatime * ySmooth * (strictFollowBounds.Contains(followed.MiddlePos) ? 1 : 2.5f)));
        }

        public void Move(Vector2 offset, float time, Func<float, float> easingFunction = null, Func<bool> stop = null)
        {
            Vector2 initPos = CenteredPos;
            Vector2 newPos = CenteredPos + offset;
            moveTimer = (Timer)AddComponent(new Timer(time, true, (t) =>
            {
                /*Vector2 amount = Vector2.Lerp(initPos, newPos, (easingFunction ?? Ease.None).Invoke(Ease.Reverse(t.Value / t.MaxValue))) - CenteredPos;
                Debug.LogUpdate(amount);
                W
                Pos += amount;*/
                if (stop != null && stop.Invoke())
                {
                    RemoveComponent(t);
                    return;
                }
                CenteredPos = Vector2.Lerp(initPos, newPos, (easingFunction ?? Ease.None).Invoke(Ease.Reverse(t.Value / t.MaxValue)));

            },

                () =>
                {
                    CenteredPos = newPos;
                    moveTimer = null;
                }));
        }

        public void LightShake()
            => Shake(0.2f, 1);

        public void Shake(float time, float intensity)
        {
            Shaker shaker = GetComponent<Shaker>();
            if(shaker == null || time > shaker.Time ||  intensity > shaker.Intensity)
            {
                RemoveComponent(shaker);
                AddComponent(new Shaker(time, intensity, () => CenteredPos));
            }
        }

        public Vector2 InBoundsPos(Vector2 position, out bool changed)
        {
            changed = false;

            if (Bounds == Rectangle.Empty)
            {
                if(CenteredPos != position)
                    changed = true;
                return position;
            }

            if ((Bounds.Contains(position - HalfSize) && Bounds.Contains(position + HalfSize)) || Bounds == Rectangle.Empty)
            {
                if(position != CenteredPos)
                    changed = true;

                return position;
            }

            Vector2 correctedPos = InBoundsPos(position);

            if (CenteredPos != correctedPos)
                changed = true;

            return correctedPos;
        }

        public Vector2 InBoundsPos(Vector2 position)
        {
            if ((Bounds.Contains(position - HalfSize) && Bounds.Contains(position + HalfSize)) || Bounds == Rectangle.Empty)
                return position;

            Vector2 inBounds = new Vector2(InBoundsPosX(position.X), InBoundsPosY(position.Y));

            return inBounds;
        }

        public Vector2 InBoundsPos(Vector2 position, Rectangle bounds)
        {
            Rectangle oldBounds = this.Bounds;
            this.Bounds = bounds;

            Vector2 inBounds = InBoundsPos(position);

            this.Bounds = oldBounds;

            return inBounds;
        }

        private float InBoundsPosX(float x)
        {
            if (x - HalfSize.X > Bounds.X && x + HalfSize.X < Bounds.X + Bounds.Width)
                return x;
            else
            {
                float correctedX = x - HalfSize.X;

                if (correctedX < Bounds.X)
                    correctedX = Bounds.X;
                else if (correctedX + Size.X > Bounds.X + Bounds.Width)
                    correctedX = Bounds.X + Bounds.Width - Size.X;

                correctedX += HalfSize.X;

                return correctedX;
            }
        }

        private float InBoundsPosY(float y)
        {
            //float halfSizeY = Engine.ScreenSize.Y / 2 / RenderTargetScreenSizeCoef;
            if (y - HalfSize.Y > Bounds.Y && y + HalfSize.Y < Bounds.Y + Bounds.Height)
                return y;
            else
            {
                float correctedY = y - HalfSize.Y;

                if (correctedY < Bounds.Y)
                    correctedY = Bounds.Y;
                else if (correctedY + Size.Y > Bounds.Y + Bounds.Height)
                    correctedY = Bounds.Y + Bounds.Height - Size.Y;

                correctedY += HalfSize.Y;

                return correctedY;
            }
        }

        public void MoveTo(Vector2 position, float time, Func<float, float> easingFunction = null)
            => Move(position - CenteredPos, time, easingFunction);

        public void SetBoundaries(Rectangle bounds)
        {
            this.Bounds = bounds;
            CenteredPos = CenteredPos;
        }

        public void SetBoundaries(Vector2 position, Vector2 size)
        {
            Bounds = new Rectangle(position.ToPoint(), size.ToPoint());
            CenteredPos = CenteredPos;
        }

        public Vector2 WorldToScreenPosition(Vector2 position)
            => Vector2.Transform(position, ViewMatrix);

        public Vector2 ScreenToWorldPosition(Vector2 position)
            => Vector2.Transform(position, InverseViewMatrix);

        public Vector2 RenderTargetToScreenPosition(Vector2 position)
            => position * ScreenSizeCoef;

        public Vector2 ScreenToRenderTargetPosition(Vector2 position)
            => position / ScreenSizeCoef;

        public Vector2 ScreenToCamPosition(Vector2 position)
            => position * (float)Engine.Cam.Width / Engine.RenderTarget.Width / ScreenSizeCoef;

        public Vector2 RenderTargetToWorldPosition(Vector2 position)
            => position + Engine.Cam.WorldToScreenPosition(position) * (Engine.Cam.ScreenSizeCoef - 1);

        public override bool CollidingConditions(Collider other)
        {
            if (other.ParentEntity is Solid s && !Engine.CurrentMap.Data.CameraSolids.Contains(s))
                return false;

            return base.CollidingConditions(other);
        }

        public override void Squish()
        { }
    }
}