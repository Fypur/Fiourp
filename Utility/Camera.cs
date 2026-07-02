using Microsoft.Xna.Framework;

namespace Fiourp
{
    public class Camera
    {
        public int ViewportWidth;
        public int ViewportHeight;

        public int Width;
        public int Height;
        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set { Width = (int)value.X; Height = (int)value.Y; }
        }

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
                    //math below is potentially wrong
                    view = Matrix.CreateTranslation(new Vector3(-VectorHelper.Round(pos), 0.0f)) *
                           Matrix.CreateScale(new Vector3(ViewportWidth / Width, ViewportHeight / Height, 1f)) *
                           Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation)) *
                           Matrix.CreateTranslation(new Vector3(ViewportWidth / 2f, ViewportHeight / 2f, 0f));
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


        public Camera(Vector2 position, float rotation, int viewportWidth, int viewportHeight)
        {
            Engine.Cam = this;

            Pos = position;
            Rotation = rotation;

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