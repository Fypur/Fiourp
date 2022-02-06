using Microsoft.Xna.Framework;
using System;

namespace Fiourp
{
    public class Camera
    {
        private Rectangle bounds = Rectangle.Empty;
        private bool hasChanged;
        public bool FollowsPlayer;
        private Timer timer;

        private Vector2 pos;
        public Vector2 Pos
        {
            get => pos;

            set
            {
                pos = InBoundsPos(value, out bool changed);
                if(changed) hasChanged = true;
            }
        }


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
                           Matrix.CreateTranslation(new Vector3(Engine.ScreenSize / 2, 0.0f));
                }
                else
                    return view;
            }
        }

        public Matrix InverseViewMatrix
        {
            get => Matrix.Invert(ViewMatrix);
        }

        public Camera(Vector2 position, float rotation, float zoomLevel, Rectangle? bounds = null)
        {
            Engine.Cam = this;

            Pos = position;
            Rotation = rotation;
            ZoomLevel = zoomLevel;

            if (bounds != null)
                SetBoundaries((Rectangle)bounds);
        }
        
        public void Update()
        {
            if(timer != null)
                timer.Update();

            if (FollowsPlayer)
                Follow(Engine.Player, 5, 5, new Rectangle(new Vector2(-Engine.ScreenSize.X / 6, -Engine.ScreenSize.Y / 12).ToPoint(),
                    new Vector2(Engine.ScreenSize.X / 3, Engine.ScreenSize.Y / 6).ToPoint()));
        }

        public void Follow(Entity actor, float xSmooth, float ySmooth, Rectangle strictFollowBounds)
        {
            strictFollowBounds.Location += Pos.ToPoint();

            if (strictFollowBounds.Contains(actor.Pos))
            {
                Pos = new Vector2(MathHelper.Lerp(Pos.X, InBoundsPosX(actor.Pos.X), Engine.Deltatime * xSmooth),
                    MathHelper.Lerp(Pos.Y, InBoundsPosY(actor.Pos.Y), Engine.Deltatime * ySmooth));
            }
            else
            {
                Pos = new Vector2(MathHelper.Lerp(Pos.X, InBoundsPosX(actor.Pos.X), Engine.Deltatime * xSmooth),
                    MathHelper.Lerp(Pos.Y, InBoundsPosY(actor.Pos.Y), Engine.Deltatime * ySmooth * 2.5f));
            }
        }

        public void Move(Vector2 offset, float time, Func<float, float> easingFunction = null)
        {
            Vector2 initPos = Pos;
            Vector2 newPos = Pos + offset;
            timer = new Timer(time, false, (t) =>

                Pos = Vector2.Lerp(initPos, newPos,
                     (easingFunction ?? Ease.Default).Invoke(Ease.Reverse(t.Value / t.MaxValue))),

                () => Pos = newPos);
        }

        public Vector2 InBoundsPos(Vector2 position, out bool changed)
        {
            changed = false;

            if (bounds == Rectangle.Empty)
                return position;

            if ((bounds.Contains(position - Engine.ScreenSize / 2 * ZoomLevel) && bounds.Contains(position + Engine.ScreenSize / 2 * ZoomLevel)) || bounds == Rectangle.Empty)
            {
                if(position != Pos)
                    changed = true;

                return position;
            }
            else
            {
                Vector2 correctedPos = position - Engine.ScreenSize / 2 * ZoomLevel;

                if (correctedPos.X < bounds.X)
                    correctedPos.X = bounds.X;
                else if (correctedPos.X + Engine.ScreenSize.X * ZoomLevel > bounds.X + bounds.Width)
                    correctedPos.X = bounds.X + bounds.Width - Engine.ScreenSize.X;

                if (correctedPos.Y < bounds.Y)
                    correctedPos.Y = bounds.Y;
                else if (correctedPos.Y + Engine.ScreenSize.Y * ZoomLevel > bounds.Y + bounds.Height)
                    correctedPos.Y = bounds.Y + bounds.Height - Engine.ScreenSize.Y;

                correctedPos += Engine.ScreenSize / 2 * ZoomLevel;

                if (Pos != correctedPos)
                    changed = true;

                return correctedPos;
            }
        }

        public Vector2 InBoundsPos(Vector2 position)
        {
            if ((bounds.Contains(position - Engine.ScreenSize / 2 * ZoomLevel) && bounds.Contains(position + Engine.ScreenSize / 2 * ZoomLevel)) || bounds == Rectangle.Empty)
                return position;
            else
            {
                Vector2 correctedPos = position - Engine.ScreenSize / 2 * ZoomLevel;

                if (correctedPos.X < bounds.X)
                    correctedPos.X = bounds.X;
                else if (correctedPos.X + Engine.ScreenSize.X * ZoomLevel > bounds.X + bounds.Width)
                    correctedPos.X = bounds.X + bounds.Width - Engine.ScreenSize.X;

                if (correctedPos.Y < bounds.Y)
                    correctedPos.Y = bounds.Y;
                else if (correctedPos.Y + Engine.ScreenSize.Y * ZoomLevel > bounds.Y + bounds.Height)
                    correctedPos.Y = bounds.Y + bounds.Height - Engine.ScreenSize.Y;

                correctedPos += Engine.ScreenSize / 2 * ZoomLevel;

                return correctedPos;
            }
        }

        public float InBoundsPosX(float x)
        {
            if(x - Engine.ScreenSize.X / 2 * ZoomLevel > bounds.X && x - Engine.ScreenSize.X / 2 * ZoomLevel < bounds.X + bounds.Width &&
                x + Engine.ScreenSize.X / 2 * ZoomLevel > bounds.X && x + Engine.ScreenSize.X / 2 * ZoomLevel < bounds.X + bounds.Width)
                return x;
            else
            {
                float correctedX = x - Engine.ScreenSize.X / 2 * ZoomLevel;

                if (correctedX < bounds.X)
                    correctedX = bounds.X;
                else if (correctedX + Engine.ScreenSize.X * ZoomLevel > bounds.X + bounds.Width)
                    correctedX = bounds.X + bounds.Width - Engine.ScreenSize.X;

                correctedX += Engine.ScreenSize.X / 2 * ZoomLevel;

                return correctedX;
            }
        }

        public float InBoundsPosY(float y)
        {
            if (y - Engine.ScreenSize.Y / 2 > bounds.Y && y - Engine.ScreenSize.Y / 2 < bounds.Y + bounds.Height &&
                y + Engine.ScreenSize.Y / 2 > bounds.Y && y + Engine.ScreenSize.Y / 2 < bounds.Y + bounds.Height)
                return y;
            else
            {
                float correctedY = y - Engine.ScreenSize.Y / 2;

                if (correctedY < bounds.Y)
                    correctedY = bounds.Y;
                else if (correctedY + Engine.ScreenSize.Y > bounds.Y + bounds.Height)
                    correctedY = bounds.Y + bounds.Height - Engine.ScreenSize.Y;

                correctedY += Engine.ScreenSize.Y / 2;

                return correctedY;
            }
        }

        public void MoveTo(Vector2 position, float time, Func<float, float> easingFunction = null)
            => Move(position - Pos, time, easingFunction);

        public void SetBoundaries(Rectangle bounds)
            => this.bounds = bounds;

        public void SetBoundaries(Vector2 position, Vector2 size)
            => bounds = new Rectangle(position.ToPoint(), size.ToPoint());

        public Vector2 WorldToScreenPosition(Vector2 position)
            => Vector2.Transform(position, ViewMatrix) / (Engine.ScreenSize.X / Engine.RenderTarget.Width);

        public Vector2 ScreenToWorldPosition(Vector2 position)
            => Vector2.Transform(position, InverseViewMatrix) / (Engine.ScreenSize.X / Engine.RenderTarget.Width);
    }
}