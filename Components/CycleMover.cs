using Microsoft.Xna.Framework;
using System;

namespace Fiourp
{
    public class CycleMover : Component
    {
        public bool Cycling;

        public bool Moving;
        public Vector2[] Positions;
        public float[] Times;
        public Func<float, float> EasingFunction;

        private int nextIndex = 1;
        private bool increment = true;
        protected Timer MovingTimer;

        public CycleMover(Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction)
        {
            if (timesBetweenPositions.Length != positions.Length - 1)
                throw new Exception("Times between positions and positions amounts are not synced");

            Moving = true;

            Positions = positions;
            Times = timesBetweenPositions;
            EasingFunction = easingfunction;
        }

        public CycleMover(Vector2 position, int width, int height, bool goingForwards, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction, out Vector2 initPos)
        {
            initPos = InitPos(position, positions, timesBetweenPositions, width, height, goingForwards, out int currentIndex, out float currentTime, out bool direction);

            if (timesBetweenPositions.Length != positions.Length - 1)
                throw new Exception("Times between positions and positions amounts are not synced");

            Cycling = true;
            increment = direction;
            nextIndex = currentIndex + (increment ? 1 : -1);

            Positions = positions;
            Times = timesBetweenPositions;
            EasingFunction = easingfunction;

            MovingTimer = new Timer(Times[currentIndex + (increment ? 0 : -1)] - currentTime, true, (timer) =>
            {
                if (!Cycling)
                    timer.PauseUntil(() => Cycling);

                float reversedTimer = timer.MaxValue - timer.Value;
                float time = (reversedTimer + currentTime) / timesBetweenPositions[currentIndex + (increment ? 0 : -1)];

                Vector2 destination = Vector2.Lerp(Positions[currentIndex], Positions[nextIndex], EasingFunction.Invoke(time));

                MoveParentTo(destination);


            }, () =>
            {
                MoveParentTo(Positions[nextIndex]);

                if (nextIndex == 0 || nextIndex == Positions.Length - 1)
                    increment = !increment;

                if (increment)
                    nextIndex++;
                else
                    nextIndex--;

                StartTimer();
            });
        }

        public override void Added()
        {
            base.Added();

            if (MovingTimer == null)
                StartTimer();
            else
                ParentEntity.AddComponent(MovingTimer);
        }

        private static Vector2 InitPos(Vector2 position, Vector2[] positions, float[] timesBetweenPositions, int width, int height, bool goingForwards, out int currentIndex, out float currentTime, out bool direction)
        {
            if (timesBetweenPositions.Length != positions.Length - 1)
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
                    if (goingForwards)
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
            if (final == nextPos)
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
                if (!Cycling)
                    timer.PauseUntil(() => Cycling);

                Vector2 destination = Vector2.Lerp(Positions[nextIndex + (increment ? -1 : 1)], Positions[nextIndex], EasingFunction.Invoke(Ease.Reverse(timer.Value / timer.MaxValue)));

                MoveParentTo(destination);

            }, () =>
            {
                MoveParentTo(Positions[nextIndex]);

                if (nextIndex == 0 || nextIndex == Positions.Length - 1)
                    increment = !increment;

                if (increment)
                    nextIndex++;
                else
                    nextIndex--;

                StartTimer();
            });

            ParentEntity.AddComponent(MovingTimer);
        }

        void MoveParentTo(Vector2 position)
        {
            if (ParentEntity is Actor actor) actor.MoveTo(position);
            else if (ParentEntity is MovingSolid solid) solid.MoveTo(position);
            else throw new Exception("No moving function found for parentEntity");
        }

        public override void Removed()
        {
            base.Removed();
            ParentEntity.RemoveComponent(MovingTimer);
        }
    }
}
