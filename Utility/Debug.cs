using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public static class Debug
    {
        public static bool DebugMode;

        public static void LogUpdate(params object[] log)
        {
            foreach (object l in log)
            {
                if(l != null)
                    Drawing.DebugUpdate.Add(l.ToString());
                else
                    Drawing.DebugUpdate.Add("null");
            }
        }

        public static void Log(params object[] log)
        {
            foreach (object l in log)
            {
                if(l != null)
                    Drawing.DebugForever.Add(l.ToString());
                else
                    Drawing.DebugForever.Add("null");
            }
        }

        public static void Point(params Vector2[] log)
            => Point(Color.DarkRed, log);

        public static void PointUpdate(params Vector2[] log)
            => PointUpdate(Color.DarkRed, log);

        public static void Point(Color color, params Vector2[] log)
        {
            foreach (Vector2 v in log)
                Drawing.DebugPos.Add(new Tuple<Vector2, Color>(v, color));
        }

        public static void PointUpdate(Color color, params Vector2[] log)
        {
            foreach (Vector2 v in log)
                Drawing.DebugPosUpdate.Add(new Tuple<Vector2, Color>(v, color));
        }

        public static void Line(Vector2 begin, Vector2 end, Color color, int thickness = 1)
            => Drawing.DebugEvent += () => Drawing.DrawLine(begin, end, color, thickness);

        public static void Vector(Vector2 debugged, Vector2 position, float scale, float arrowScale)
            => Vector(debugged, position, Color.Red, scale, arrowScale);

        public static void Vector(Vector2 debugged, Vector2 position, Color color, float scale, float arrowScale)
        {
            Drawing.DebugEvent += () =>
            {
                Vector2 dir = debugged.Normalized();
                Drawing.DrawLine(position, position + debugged * scale, Color.Red, 1);
                Drawing.DrawLine(position + debugged * scale, position + debugged * scale + VectorHelper.Rotate(dir, 2 * (float)Math.PI / 3) * arrowScale, color, 1);
                Drawing.DrawLine(position + debugged * scale, position + debugged * scale + VectorHelper.Rotate(dir, -2 * (float)Math.PI / 3) * arrowScale, color, 1);
            };
        }
        public static void Event(params Action[] actions)
        {
            foreach (Action action in actions)
                Drawing.DebugEvent += action;
        }

        public static void Clear()
        {
            Drawing.DebugForever.Clear();
            Drawing.DebugPos.Clear();
        }
    }
}