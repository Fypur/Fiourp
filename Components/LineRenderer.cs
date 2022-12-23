using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public Texture2D Texture;

        public Action<LineRenderer> UpdateAction;
        public Action<LineRenderer> RenderAction;

        public LineRenderer(Vector2 start, Vector2 end, Texture2D texture, int width, Color lineColor,
            Action<LineRenderer> UpdateAction = null, Action<LineRenderer> RenderAction = null)
        {
            Positions.Add(start);
            Positions.Add(end);
            Thickness = width;
            Texture = texture;
            Color = lineColor;
            this.UpdateAction = UpdateAction;
            this.RenderAction = RenderAction;
        }

        public LineRenderer(List<Vector2> positions, Texture2D texture, int thickness, Color lineColor,
            Action<LineRenderer> UpdateAction = null, Action<LineRenderer> RenderAction = null)
        {
            Positions.AddRange(positions);
            Texture = texture;
            Thickness = thickness;
            Color = lineColor;
            this.UpdateAction = UpdateAction;
            this.RenderAction = RenderAction;
        }

        public override void Update()
        {
            UpdateAction?.Invoke(this);
        }

        public override void Render()
        {
            RenderAction?.Invoke(this);

            for(int i = 0; i < Positions.Count - 1; i++)
            {
                if (Texture != null)
                    Drawing.DrawLine(Texture, Positions[i], Positions[i + 1], Color, Thickness);
                else
                    Drawing.DrawLine(Positions[i], Positions[i + 1], Color, Thickness);
            }
        }
    }
}
