using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Fiourp
{
    public abstract class CyclingSolid : MovingSolid
    {
        public bool moving;

        public Vector2[] Positions;
        public float[] Times;
        public Func<float, float> EasingFunction;

        private int nextIndex = 1;
        private bool increment = true;
        private Timer movingTimer;

        public CyclingSolid(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite) { }
        public CyclingSolid(Vector2 position, int width, int height, Color color) : base(position, width, height, color) { }

        public CyclingSolid(int width, int height, Sprite sprite, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction)
            : base(positions[0], width, height, sprite)
        {
            if (timesBetweenPositions.Length != positions.Length - 1) 
                throw new Exception("Times between positions and positions amounts are not synced");
            Contract.EndContractBlock();

            moving = true;

            Pos = positions[0];
            Positions = positions;
            Times = timesBetweenPositions;
            EasingFunction = easingfunction;

            StartTimer();
        }

        public CyclingSolid(int width, int height, Color color, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction)
            : base(positions[0], width, height, color)
        {
            if (timesBetweenPositions.Length != positions.Length - 1)
                throw new Exception("Times between positions and positions amounts are not synced");
            Contract.EndContractBlock();

            moving = true;

            Pos = positions[0];
            Positions = positions;
            Times = timesBetweenPositions;
            EasingFunction = easingfunction;

            StartTimer();
        }

        private void StartTimer()
        {
            movingTimer = new Timer(Times[nextIndex + (increment ? -1 : 0)], true, (timer) =>
            {
                if (!moving)
                    timer.PauseUntil(() => moving);

                MoveTo(Vector2.Lerp(Positions[nextIndex + (increment ? -1 : 1)], Positions[nextIndex], EasingFunction.Invoke(Ease.Reverse(timer.Value / timer.MaxValue))));

            }, () =>
            {
                MoveTo(Positions[nextIndex]);

                if (nextIndex == 0 || nextIndex == Positions.Length - 1)
                    increment = !increment;

                if(increment)
                    nextIndex++;
                else
                    nextIndex--;

                StartTimer();
            });

            AddComponent(movingTimer);
        }
    }
}
