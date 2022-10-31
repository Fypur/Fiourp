﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Fiourp
{
    public static class Drawing
    {
        public const int maxVertices = 1000;

        private static SpriteBatch spriteBatch;
        private static GraphicsDevice graphicsDevice => Engine.Graphics.GraphicsDevice;
        public static Texture2D PointTexture;
        public static SpriteFont Font;
        
        public static List<string> DebugUpdate = new List<string>();
        public static List<string> DebugForever = new List<string>();
        public static List<Tuple<Vector2, Color>> DebugPos = new List<Tuple<Vector2, Color>>();
        public static List<Tuple<Vector2, Color>> DebugPosUpdate = new List<Tuple<Vector2, Color>>();
        public static event Action DebugEvent = delegate { };

        private static VertexPositionColor[] vertices;
        private static int[] indices;

        private static BasicEffect effect;

        private static int vertexCount;
        private static int indicesCount;
        private static int shapesCount;
        private static bool hasStartedBatching;

        public static void Init(SpriteBatch spriteBatch, SpriteFont font)
        {
            Drawing.spriteBatch = spriteBatch;
            Drawing.Font = font;
            PointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            PointTexture.Name = "Point Texture";
            PointTexture.SetData(new Color[] { Color.White });

            vertices = new VertexPositionColor[maxVertices];
            indices = new int[maxVertices * 3];
            effect = new BasicEffect(graphicsDevice);
        }

        public static void Draw(Texture2D texture, Vector2 position)
           => spriteBatch.Draw(texture, position, Color.White);

        public static void Draw(Texture2D texture, Vector2 position, Color color)
           => spriteBatch.Draw(texture, position, color);

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0)
            => spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, spriteEffects, layerDepth);

        public static void Draw(Texture2D texture, Vector2 position, Vector2 size, float rotation, float layerDepth)
            => spriteBatch.Draw(texture, new Rectangle(position.ToPoint(), size.ToPoint()), null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, layerDepth);

        public static void Draw(Texture2D texture, Vector2 position, Vector2 size, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
            => spriteBatch.Draw(texture, new Rectangle(position.ToPoint(), size.ToPoint()), null, color, rotation, origin, effects, layerDepth);

        public static void Draw(Texture2D texture, Rectangle destinationRectangle)
           => spriteBatch.Draw(texture, destinationRectangle, Color.White);

        public static void Draw(Texture2D texture, Rectangle destinationRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect = SpriteEffects.None, float layerDepth = 0)
           => spriteBatch.Draw(texture, destinationRectangle, null, color, rotation, origin, effect, layerDepth);

        public static void Draw(Rectangle rect, Color color)
            => spriteBatch.Draw(PointTexture, rect, color);

        public static void Draw(Rectangle rect, float rotation, Color color)
            => spriteBatch.Draw(PointTexture, rect, null, color, rotation, Vector2.Zero, SpriteEffects.None, 0);

        public static void DrawCircleEdge(Vector2 position, float radius, float theta, Color color, int thickness)
        {
            Vector2 previous = position + new Vector2(radius, 0);
            for (float x = theta; x < 2 * Math.PI; x += theta)
            {
                Vector2 pos = position + new Vector2((float)(Math.Cos(x) * radius), (float)(Math.Sin(x) * radius));
                DrawLine(pos, previous, color, thickness);
                previous = pos;
            }
        }

        public static void DrawCircle(Vector2 position, float radius, float theta, Color color)
            => DrawCircle(position, radius, theta, color, color);

        public static void DrawCircle(Vector2 position, float radius, float theta, Color middleColor, Color exteriorColor)
        {
            Vector2 previous = position + new Vector2(radius, 0);
            EnsureSpace(3, 3);

            int ind = vertexCount;
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(position, 0), middleColor);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(previous, 0), exteriorColor);
            

            for (float x = theta; x <= 2 * Math.PI; x += theta)
            {
                shapesCount++;

                indices[indicesCount++] = ind;
                indices[indicesCount++] = vertexCount - 1;
                indices[indicesCount++] = vertexCount;

                Vector2 pos = position + new Vector2((float)Math.Cos(x), (float)Math.Sin(x)) * radius;
                vertices[vertexCount++] = new VertexPositionColor(new Vector3(pos, 0), exteriorColor);
            }

            shapesCount++;

            indices[indicesCount++] = ind;
            indices[indicesCount++] = vertexCount - 1;
            indices[indicesCount++] = ind + 1;
        }

        public static void DrawQuad(Vector2 a, Color aColor, Vector2 b, Color bColor, Vector2 c, Color cColor, Vector2 d, Color dColor)
        {
            EnsureSpace(4, 6);

            indices[indicesCount++] = vertexCount;
            indices[indicesCount++] = vertexCount + 1;
            indices[indicesCount++] = vertexCount + 2;
            indices[indicesCount++] = vertexCount;
            indices[indicesCount++] = vertexCount + 2;
            indices[indicesCount++] = vertexCount + 3;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(a, 0), aColor);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(b, 0), bColor);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(c, 0), cColor);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(d, 0), dColor);

            shapesCount++;
        }

        public static void DrawString(string text, Vector2 position, Color color, Vector2 origin)
            => spriteBatch.DrawString(Font, text, position, color, 0, origin,
                1, SpriteEffects.None, 1);
        
        public static void DrawCenteredString(string text, Vector2 position, Color color)
            => spriteBatch.DrawString(Font, text, position, color, 0, Font.MeasureString(text) / 2,
                1, SpriteEffects.None, 0.25f);

        public static void DrawString(string text, Vector2 position, Color color, SpriteFont font)
            => spriteBatch.DrawString(font, text, position, color, 0, Vector2.Zero,
                1, SpriteEffects.None, 0.25f);

        public static void DrawString(string text, Vector2 position, Color color, SpriteFont font, Vector2 scale)
            => spriteBatch.DrawString(font, text, position, color, 0, Vector2.Zero,
                scale, SpriteEffects.None, 0.25f);

        public static void DrawString(string text, Vector2 position, Color color, SpriteFont font, float scale)
            => spriteBatch.DrawString(font, text, position, color, 0, Vector2.Zero,
                scale, SpriteEffects.None, 0.25f);

        public static void DrawString(string text, Vector2 position, Color color, SpriteFont font, Vector2 origin, float scale, float rotation)
            => spriteBatch.DrawString(font, text, position, color, rotation, origin,
                scale, SpriteEffects.None, 0.25f);

        public static void DrawString(string text, Vector2 position, Color color, SpriteFont font, Vector2 origin, Vector2 scale, float rotation)
            => spriteBatch.DrawString(font, text, position, color, rotation, origin,
                scale, SpriteEffects.None, 0.25f);

        public static void DrawLine(Vector2 begin, Vector2 end, Color color, int thickness = 1)
        {
            float distance = Vector2.Distance(begin, end);
            float angle = (float)Math.Atan2(end.Y - begin.Y, end.X - begin.X);
            Vector2 scale = new Vector2(distance, thickness);
            spriteBatch.Draw(PointTexture, begin, null, color, angle, new Vector2(0f, 0.5f), scale, SpriteEffects.None, 1);
        }

        public static void DrawDottedLine(Vector2 begin, Vector2 end, Color color, int thickness, int dotLength, int gapLength)
        {
            float distance = Vector2.Distance(begin, end);
            float step = dotLength + gapLength;
            Vector2 normalized = (end - begin).Normalized();

            for(int i = 0; i * step < distance; i++)
            {
                Vector2 a = begin + normalized * i * step;
                Vector2 b = begin + normalized * (Math.Min(i * step + dotLength, distance));

                float angle = (float)Math.Atan2(b.Y - a.Y, b.X - a.X);
                Vector2 scale = new Vector2(i * step + dotLength < distance ? dotLength : Vector2.Distance(b, end), thickness);
                spriteBatch.Draw(PointTexture, a, null, color, angle, new Vector2(0f, 0.5f), scale, SpriteEffects.None, 1);
            }
        }

        public static void DrawEdge(Rectangle rectangle, int lineWidth, Color color)
        {
            spriteBatch.Draw(PointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(PointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(PointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(PointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        public static void DrawPoint(Vector2 pos, int thickness, Color color)
        {
            spriteBatch.Draw(PointTexture, new Rectangle((int)pos.X - (thickness / 2), (int)pos.Y - (thickness / 2), thickness, thickness), color);
        }

        public static void BeginPrimitives()
        {
            if (hasStartedBatching)
                throw new Exception("Batching already started");

            hasStartedBatching = true;
        }

        public static void EndPrimitives()
        {
            if (!hasStartedBatching)
                throw new Exception("Batching has not started");

            Flush();
            hasStartedBatching = false;
        }

        private static void Flush()
        {
            if (!hasStartedBatching)
                throw new Exception("Batching has not started");

            if (shapesCount <= 0)
                return;

            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;
            effect.FogEnabled = false;
            effect.LightingEnabled = false;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.Additive;
            effect.World = Engine.Cam.ViewMatrix * Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);

            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, vertexCount, indices, 0, indicesCount / 3);
            }

            vertexCount = 0;
            indicesCount = 0;
            shapesCount = 0;
        }

        private static void EnsureSpace(int shapeVerticesCount, int shapeIndicesCount)
        {
            if (shapeVerticesCount > vertices.Length)
                throw new Exception("Drawn shape has more vertices that the maximum vertices used per batch.");
            if(shapeIndicesCount > indices.Length)
                throw new Exception("Drawn shape has more indices that the maximum indices used per batch.");

            if (vertexCount + shapeVerticesCount > vertices.Length || indicesCount + shapeIndicesCount > indices.Length)
                Flush();
        }

        public static void DrawTriangle(Vector2 a, Color aColor, Vector2 b, Color bColor, Vector2 c, Color cColor)
        {
            EnsureSpace(3, 3);

            shapesCount++;

            indices[indicesCount++] = vertexCount + 0;
            indices[indicesCount++] = vertexCount + 1;
            indices[indicesCount++] = vertexCount + 2;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(a, 0), aColor);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(b, 0), bColor);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(c, 0), cColor);
        }

        public static void DebugString()
        {
#if DEBUG
            Vector2 pos = Vector2.Zero;

            if(DebugUpdate.Count * Font.MeasureString("A").Y + DebugForever.Count * Font.MeasureString("A").Y > Engine.ScreenSize.Y)
                DebugForever.Clear();

            foreach(string s in DebugUpdate)
            {
                Drawing.DrawString(s, pos, Color.Brown, Vector2.Zero);
                pos.Y += Font.MeasureString(s).Y;
            }

            pos.Y += Font.MeasureString(" ").Y;

            foreach (string s in DebugForever)
            {
                Drawing.DrawString(s, pos, Color.Brown, Vector2.Zero);
                pos.Y += Font.MeasureString(s).Y;
            }

            DebugUpdate.Clear();
#endif
        }

        public static void DebugPoint(int thickness, int screenScale)
        {
#if DEBUG

            foreach(Tuple<Vector2, Color> pos in DebugPos)
                DrawPoint(pos.Item1 * screenScale, thickness, pos.Item2);

            foreach (Tuple<Vector2, Color> pos in DebugPosUpdate)
                DrawPoint(pos.Item1 * screenScale, thickness, pos.Item2);

            DebugPosUpdate.Clear();
#endif

        }

        public static void DebugEvents()
        {
#if DEBUG
            DebugEvent();
            DebugEvent = delegate { };
#endif
        }
    }
}
