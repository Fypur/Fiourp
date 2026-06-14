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

        private float rotation;
        public float Rotation
        {
            get => rotation;
            set { if (value != rotation) { needToRecalculateMatrix = true; rotation = value; } }
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
                    Vector2 wholePos = pos;
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
        }

        public void Refresh()
            => needToRecalculateMatrix = true;

        public Vector2 WorldToScreenPosition(Vector2 position)
            => Vector2.Transform(position, ViewMatrix);

        public Vector2 ScreenToWorldPosition(Vector2 position)
            => Vector2.Transform(position, InverseViewMatrix);
    }
}