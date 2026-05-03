using Microsoft.Xna.Framework;

namespace Fiourp
{
    public class Camera : Entity
    {
        public int Width;
        public int Height;

        private bool needToRecalculateMatrix;
        private bool needToRecalculateInverseMatrix;
        public override Vector2 Pos
        {
            get => base.Pos;
            set { if (value != base.Pos) { needToRecalculateMatrix = true; base.Pos = value; } }
        }

        public override float Rotation
        {
            get => base.Rotation;
            set { if (value != base.Rotation) { needToRecalculateMatrix = true; base.Rotation = value; } }
        }

        private float zoom;
        public float ZoomLevel
        {
            get => zoom;
            set { if (value != zoom) { needToRecalculateMatrix = true; zoom = value; } }
        }

        private Matrix view;
        public Matrix ViewMatrix
        {
            get
            {
                if (needToRecalculateMatrix)
                {
                    needToRecalculateMatrix = false;
                    needToRecalculateInverseMatrix = true;
                    Vector2 wholePos = Pos;
                    wholePos.Round();
                    view = Matrix.CreateTranslation(new Vector3(-VectorHelper.Round(wholePos), 0.0f)) *
                           Matrix.CreateScale(ZoomLevel) *
                           Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation));
                }

                return view;
            }
        }

        private Matrix inverseMatrix;
        public Matrix InverseViewMatrix
        {
            get
            {
                if (needToRecalculateInverseMatrix)
                {
                    inverseMatrix = Matrix.Invert(ViewMatrix);
                    needToRecalculateInverseMatrix = false;
                }

                return inverseMatrix;
            }
        }


        public Camera(Vector2 position, float rotation, float zoomLevel)
        {
            Engine.Cam = this;

            Pos = position;
            Rotation = rotation;
            ZoomLevel = zoomLevel;

            if (bounds != null)
                SetBoundaries((Rectangle)bounds);
        }

        public void Refresh()
            => needToRecalculateMatrix = true;

        public void LightShake()
            => Shake(0.2f, 1);

        public void Shake(float time, float intensity)
        {
            Shaker shaker = GetComponent<Shaker>();
            if (shaker == null || time > shaker.Time || intensity > shaker.Intensity)
            {
                RemoveComponent(shaker);
                AddComponent(new Shaker(time, intensity, () => CenteredPos));
            }
        }

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
    }
}