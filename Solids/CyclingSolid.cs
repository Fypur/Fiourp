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
        protected Timer MovingTimer;

        public CyclingSolid(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite) { }
        public CyclingSolid(Vector2 position, int width, int height, Color color) : base(position, width, height, color) { }

        public CyclingSolid(int width, int height, Sprite sprite, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction)
            : base(positions[0], width, height, sprite)
        {
            if (timesBetweenPositions.Length != positions.Length - 1) 
                throw new Exception("Times between positions and positions amounts are not synced");
            Contract.EndContractBlock();

            moving = true;

            Positions = positions;
            Times = timesBetweenPositions;
            EasingFunction = easingfunction;

            StartTimer();
        }

        public CyclingSolid(Vector2 position, int width, int height, Sprite sprite, bool goingForwards, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction)
            : base(InitPos(position, positions, timesBetweenPositions, width, height, goingForwards, out int currentIndex, out float currentTime, out bool direction), width, height, sprite)
        {
            if (timesBetweenPositions.Length != positions.Length - 1)
                throw new Exception("Times between positions and positions amounts are not synced");

            moving = true;
            increment = direction;
            nextIndex = currentIndex + (increment ? 1 : -1);

            Positions = positions;
            Times = timesBetweenPositions;
            EasingFunction = easingfunction;

            MovingTimer = new Timer(Times[currentIndex + (increment ? 0 : -1)] - currentTime, true, (timer) =>
            {
                if (!moving)
                    timer.PauseUntil(() => moving);

                float reversedTimer = timer.MaxValue - timer.Value;
                float time = (reversedTimer + currentTime) / timesBetweenPositions[currentIndex + (increment ? 0 : -1)];
                MoveTo(Vector2.Lerp(Positions[currentIndex], Positions[nextIndex],
                    EasingFunction.Invoke(
                        time
                        )));

            }, () =>
            {
                MoveTo(Positions[nextIndex]);

                if (nextIndex == 0 || nextIndex == Positions.Length - 1)
                    increment = !increment;

                if (increment)
                    nextIndex++;
                else
                    nextIndex--;

                StartTimer();
            });

            AddComponent(MovingTimer);
    }

        private static Vector2 InitPos(Vector2 position, Vector2[] positions, float[] timesBetweenPositions, int width, int height, bool goingForwards, out int currentIndex, out float currentTime, out bool direction)
        {
            if (timesBetweenPositions.Length  != positions.Length - 1)
                throw new Exception("Times between positions and positions amounts are not synced");

            position += new Vector2(width / 2, height / 2);
            currentIndex = -1;
            float minDistance = float.PositiveInfinity;
            Vector2 final = position;

            for (int i = 0; i < positions.Length - 1; i++)
            {
                Vector2 possiblePos = VectorHelper.ClosestOnSegment(position, positions[i], positions[i + 1]);
                float distance = Vector2.Distance(possiblePos, position);

                if (distance < minDistance)
                {
                    final = possiblePos;
                    if(goingForwards)
                        currentIndex = i;
                    else
                        currentIndex = i + 1;
                    minDistance = distance;
                }
            }

            int nextIndex = currentIndex + (goingForwards ? 1 : -1);

            Vector2 beginPos = positions[currentIndex];
            Vector2 nextPos = positions[nextIndex];

            currentTime = Vector2.Distance(beginPos, final) / Vector2.Distance(beginPos, nextPos) * timesBetweenPositions[currentIndex + (goingForwards ? 0 : -1)];

            direction = goingForwards;
            if(final == nextPos)
            {
                currentTime = 0;
                currentIndex = nextIndex;
                direction = !goingForwards;
            }

            //final -= new Vector2(width / 2, height / 2);
            return final;
        }

        private void StartTimer()
        {
            MovingTimer = new Timer(Times[nextIndex + (increment ? -1 : 0)], true, (timer) =>
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

            AddComponent(MovingTimer);
        }
    }
}
