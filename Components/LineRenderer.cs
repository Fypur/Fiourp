using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class LineRenderer : Renderer
    {
        public List<Vector2> Positions = new List<Vector2>();
        public int Thickness;
        public Color Color;

        private Action<LineRenderer> updateAction;
        private Action<LineRenderer> renderAction;

        public LineRenderer(Vector2 start, Vector2 end, int width, Color lineColor,
            Action<LineRenderer> UpdateAction = null, Action<LineRenderer> RenderAction = null)
        {
            Positions.Add(start);
            Positions.Add(end);
            Thickness = width;
            Color = lineColor;
            updateAction = UpdateAction;
            renderAction = RenderAction;
        }

        public LineRenderer(List<Vector2> positions, int thickness, Color lineColor,
            Action<LineRenderer> UpdateAction = null, Action<LineRenderer> RenderAction = null)
        {
            Positions.AddRange(positions);
            Thickness = thickness;
            Color = lineColor;
            updateAction = UpdateAction;
            renderAction = RenderAction;
        }

        public override void Update()
        {
            updateAction?.Invoke(this);
        }

        public override void Render()
        {
            renderAction?.Invoke(this);

            for(int i = 0; i < Positions.Count - 1; i++)
                Drawing.DrawLine(Positions[i], Positions[i + 1], Color, Thickness);
        }
    }
}
