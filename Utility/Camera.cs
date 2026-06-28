using Microsoft.Xna.Framework;

namespace Fiourp
{
    public class Camera
    {
        public int Width;
        public int Height;
        public int ViewportWidth;
        public int ViewportHeight;

        private bool needToRecalculateMatrix;
        private bool needToRecalculateInverseMatrix;

        private Vector2 pos;
        public Vector2 Pos
        {
            get => pos;
            set { if (value != pos) { needToRecalculateMatrix = true; pos = value; } }
        }

        private float rotation;
        public float Rotation
        {
            get => rotation;
            set { if (value != rotation) { needToRecalculateMatrix = true; rotation = value; } }
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
                    Vector2 wholePos = pos;
                    wholePos.Round();
                    view = Matrix.CreateTranslation(new Vector3(-VectorHelper.Round(wholePos), 0.0f)) *
                           Matrix.CreateScale(new Vector3(ViewportWidth / Width, ViewportHeight / Height, 1f)) *
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
                if (needToRecalculateMatrix || needToRecalculateInverseMatrix)
                {
                    inverseMatrix = Matrix.Invert(ViewMatrix);
                    needToRecalculateInverseMatrix = false;
                }

                return inverseMatrix;
            }
        }


        public Camera(Vector2 position, float rotation, float viewportWidth, float viewportHeight)
        {
            Engine.Cam = this;

            Pos = position;
            Rotation = rotation;
            ZoomLevel = zoomLevel;

            Width = viewportWidth;
            Height = viewportHeight;
            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;
        }

        public void Refresh()
            => needToRecalculateMatrix = true;

        public Vector2 WorldToScreenPosition(Vector2 position)
            => Vector2.Transform(position, ViewMatrix);

        public Vector2 ScreenToWorldPosition(Vector2 position)
            => Vector2.Transform(position, InverseViewMatrix);
    }
}