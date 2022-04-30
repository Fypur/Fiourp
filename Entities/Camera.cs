using Microsoft.Xna.Framework;
using System;

namespace Fiourp
{
    public class Camera : Entity
    {
        public new Rectangle Bounds = Rectangle.Empty;
        private bool hasChanged;
        public bool FollowsPlayer;
        public bool Locked;

        private Vector2 pos;
        public new Vector2 Pos
        {
            get => pos;

            set
            {
                pos = InBoundsPos(value, out bool changed);
                if(changed) hasChanged = true;
            }
        }

        public Vector2 NoBoundsPos
        {
            get => pos;
            set
            {
                if(pos == value) return;
                pos = value;
                hasChanged = true;
            }
        }

        public override Vector2 ExactPos { get => Pos; set => Pos = value; }

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
                    return view = Matrix.CreateTranslation(new Vector3(-pos, 0.0f)) *
                           Matrix.CreateScale(ZoomLevel) *
                           Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation)) *
                           Matrix.CreateTranslation(new Vector3(new Vector2(Engine.RenderTarget.Width / 2, Engine.RenderTarget.Height / 2), 0.0f));
                }
                else
                    return view;
            }
        }

        public Matrix InverseViewMatrix
        {
            get => Matrix.Invert(ViewMatrix);
        }

        public float RenderTargetScreenSizeCoef { get => Engine.ScreenSize.X / Engine.RenderTarget.Width; }

        public Camera(Vector2 position, float rotation, float zoomLevel, Rectangle? bounds = null)
            : base(position)
        {
            //TODO: Auto Scroll for Chase Sequence
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

            if (Engine.Player != null && FollowsPlayer && !Locked && (!GetComponent(out Timer timer) || timer.Value <= 0) && !GetComponent<Shaker>())
                Follow(Engine.Player, 3, 3, new Rectangle(new Vector2(-Engine.ScreenSize.X / 6, -Engine.ScreenSize.Y / 12).ToPoint(),
                    new Vector2(Engine.ScreenSize.X / 3, Engine.ScreenSize.Y / 6).ToPoint()));
        }

        public Vector2 Follow(Entity actor, float xSmooth, float ySmooth, Rectangle strictFollowBounds)
            => Pos = FollowedPos(actor, xSmooth, ySmooth, strictFollowBounds, Bounds);

        public Vector2 FollowedPos(Entity followed, float xSmooth, float ySmooth, Rectangle strictFollowBounds, Rectangle bounds)
        {
            strictFollowBounds.Location += Pos.ToPoint();
            Vector2 inBoundsActorPos = InBoundsPos(followed.Pos, bounds);

            if (strictFollowBounds.Contains(followed.Pos))
                return new Vector2(MathHelper.Lerp(Pos.X, inBoundsActorPos.X, Engine.Deltatime * xSmooth),
                    MathHelper.Lerp(Pos.Y, inBoundsActorPos.Y, Engine.Deltatime * ySmooth));
            else
                return new Vector2(MathHelper.Lerp(Pos.X, inBoundsActorPos.X, Engine.Deltatime * xSmooth),
                    MathHelper.Lerp(Pos.Y, inBoundsActorPos.Y, Engine.Deltatime * ySmooth * 2.5f));
        }

        public void Move(Vector2 offset, float time, Func<float, float> easingFunction = null)
        {
            Vector2 initPos = Pos;
            Vector2 newPos = Pos + offset;
            AddComponent(new Timer(time, true, (t) =>

                Pos = Vector2.Lerp(initPos, newPos,
                     (easingFunction ?? Ease.None).Invoke(Ease.Reverse(t.Value / t.MaxValue))),

                () => Pos = newPos));
        }

        public void Shake(float time, float intensity)
        {
            AddComponent(new Shaker(time, intensity, () =>
                FollowedPos(Engine.Player, 3, 3,
                new Rectangle(new Vector2(-Engine.ScreenSize.X / 6 + intensity, -Engine.ScreenSize.Y / 12).ToPoint(),
                        new Vector2(Engine.ScreenSize.X / 3 - intensity, Engine.ScreenSize.Y / 6 + 20).ToPoint()),
                Bounds), false));
        }

        public Vector2 InBoundsPos(Vector2 position, out bool changed)
        {
            changed = false;

            if (Bounds == Rectangle.Empty)
            {
                if(Pos != position)
                    changed = true;
                return position;
            }

            if ((Bounds.Contains(position - Engine.ScreenSize / 2 / RenderTargetScreenSizeCoef) && Bounds.Contains(position + Engine.ScreenSize / 2 / RenderTargetScreenSizeCoef)) || Bounds == Rectangle.Empty)
            {
                if(position != Pos)
                    changed = true;

                return position;
            }

            Vector2 correctedPos = InBoundsPos(position);

            if (Pos != correctedPos)
                changed = true;

            return correctedPos;
        }

        public Vector2 InBoundsPos(Vector2 position)
        {
            if ((Bounds.Contains(position - Engine.ScreenSize / 2 / RenderTargetScreenSizeCoef) && Bounds.Contains(position + Engine.ScreenSize / 2 / RenderTargetScreenSizeCoef)) || Bounds == Rectangle.Empty)
                return position;

            return new Vector2(InBoundsPosX(position.X), InBoundsPosY(position.Y));
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
            if (x - Engine.ScreenSize.X / 2 / RenderTargetScreenSizeCoef > Bounds.X && x - Engine.ScreenSize.X / 2 / RenderTargetScreenSizeCoef < Bounds.X + Bounds.Width &&
                x + Engine.ScreenSize.X / 2 / RenderTargetScreenSizeCoef > Bounds.X && x + Engine.ScreenSize.X / 2 / RenderTargetScreenSizeCoef < Bounds.X + Bounds.Width)
                return x;
            else
            {
                float correctedX = x - Engine.ScreenSize.X / 2 / RenderTargetScreenSizeCoef;

                if (correctedX < Bounds.X)
                    correctedX = Bounds.X;
                else if (correctedX + Engine.ScreenSize.X / RenderTargetScreenSizeCoef > Bounds.X + Bounds.Width)
                    correctedX = Bounds.X + Bounds.Width - Engine.ScreenSize.X / RenderTargetScreenSizeCoef;

                correctedX += Engine.ScreenSize.X / 2 / RenderTargetScreenSizeCoef;

                return correctedX;
            }
        }

        private float InBoundsPosY(float y)
        {
            if (y - Engine.ScreenSize.Y / 2 / RenderTargetScreenSizeCoef > Bounds.Y && y - Engine.ScreenSize.Y / 2 / RenderTargetScreenSizeCoef < Bounds.Y + Bounds.Height &&
                y + Engine.ScreenSize.Y / 2 / RenderTargetScreenSizeCoef > Bounds.Y && y + Engine.ScreenSize.Y / 2 / RenderTargetScreenSizeCoef < Bounds.Y + Bounds.Height)
                return y;
            else
            {
                float correctedY = y - Engine.ScreenSize.Y / 2 / RenderTargetScreenSizeCoef;

                if (correctedY < Bounds.Y)
                    correctedY = Bounds.Y;
                else if (correctedY + Engine.ScreenSize.Y / RenderTargetScreenSizeCoef > Bounds.Y + Bounds.Height)
                    correctedY = Bounds.Y + Bounds.Height - Engine.ScreenSize.Y / RenderTargetScreenSizeCoef;

                correctedY += Engine.ScreenSize.Y / 2 / RenderTargetScreenSizeCoef;

                return correctedY;
            }
        }

        public void MoveTo(Vector2 position, float time, Func<float, float> easingFunction = null)
            => Move(position - Pos, time, easingFunction);

        public void SetBoundaries(Rectangle bounds)
        {
            this.Bounds = bounds;
            Pos = Pos;
        }

        public void SetBoundaries(Vector2 position, Vector2 size)
        {
            Bounds = new Rectangle(position.ToPoint(), size.ToPoint());
            Pos = Pos;
        }

        public Vector2 WorldToScreenPosition(Vector2 position)
            => Vector2.Transform(position, ViewMatrix);

        public Vector2 ScreenToWorldPosition(Vector2 position)
            => Vector2.Transform(position, InverseViewMatrix);

        public Vector2 RenderTargetToScreenPosition(Vector2 position)
            => position * RenderTargetScreenSizeCoef;

        public Vector2 ScreenToRenderTargetPosition(Vector2 position)
            => position / RenderTargetScreenSizeCoef;

        public Vector2 RenderTargetToWorldPosition(Vector2 position)
            => position + Engine.Cam.WorldToScreenPosition(position) * (Engine.Cam.RenderTargetScreenSizeCoef - 1);
    }
}