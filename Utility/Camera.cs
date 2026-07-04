using Microsoft.Xna.Framework;

namespace Fiourp
{
    public class Camera
    {
        private bool needToRecalculateMatrix;
        private bool needToRecalculateInverseMatrix;

        private Vector2 pos;
        public Vector2 Pos
        {
            get => pos;
            set { if (value != pos) { needToRecalculateMatrix = true; pos = value; } }
        }

        private Vector2 scale;
        public Vector2 Scale
        {
            get => scale;
            set { if (value != scale) { needToRecalculateMatrix = true; scale = value; } }
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
                           Matrix.CreateScale(new Vector3(Scale, 1f)) *
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


        public Camera(Vector2 position, float rotation, Vector2 scale)
        {
            Pos = position;
            Rotation = rotation;
            Scale = scale;
        }

        public void Refresh()
            => needToRecalculateMatrix = true;

        public Vector2 WorldToScreenPosition(Vector2 position)
            => Vector2.Transform(position, ViewMatrix);

        public Vector2 ScreenToWorldPosition(Vector2 position)
            => Vector2.Transform(position, InverseViewMatrix);
    }
}