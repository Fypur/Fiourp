using Microsoft.Xna.Framework;

namespace Fiourp
{
    public class Camera : Entity
    {
        private bool needToRecalculateMatrix;
        private bool needToRecalculateInverseMatrix;

        public new Vector2 Pos
        {
            get => base.Pos;
            set { if (value != base.Pos) { needToRecalculateMatrix = true; base.Pos = value; } }
        }

        public new float Rotation
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


        public Camera(Vector2 position, float rotation, float zoomLevel) : base(position)
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