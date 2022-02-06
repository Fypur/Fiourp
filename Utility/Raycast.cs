using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class Raycast
    {
        public bool hit;
        public Vector2 endPoint;

        public Raycast(Vector2 begin, Vector2 direction, float length)
            => FastRay(begin, direction, length);


        public Raycast(Vector2 begin, Vector2 end)
            => FastRay(begin, end - begin, Vector2.Distance(begin, end));

        void FastRay(Vector2 begin, Vector2 direction, float length)
        {
            #region Ray Direction, Step Size and Original Pos Tile

            Map map = Engine.CurrentMap;
            Vector2 end = begin + Vector2.Normalize(direction) * length;

            Vector2 rayDir = Vector2.Normalize(direction);

            //The hypothenus' size for one Unit (a tile width) on the x and y axis
            Vector2 rayUnitStep = new Vector2((float)Math.Sqrt(map.CurrentLevel.TileWidth * map.CurrentLevel.TileWidth + (rayDir.Y * map.CurrentLevel.TileWidth / rayDir.X) * (rayDir.Y * map.CurrentLevel.TileWidth / rayDir.X)),
                (float)Math.Sqrt(map.CurrentLevel.TileHeight * map.CurrentLevel.TileHeight + (rayDir.X * map.CurrentLevel.TileHeight / rayDir.Y) * (rayDir.X * map.CurrentLevel.TileHeight / rayDir.Y)));

            //The tile the begin point is on and the one the end point is on : position truncated to a multiple of the tile's width or height
            Vector2 mapPoint = new Vector2((float)Math.Floor(begin.X / map.CurrentLevel.TileWidth) * map.CurrentLevel.TileWidth, (float)Math.Floor(begin.Y / map.CurrentLevel.TileWidth) * map.CurrentLevel.TileHeight);
            #endregion

            #region Ray Direction for Each Dimension and Length for non tiled objects

            Vector2 rayStep;
            Vector2 rayLength1D;

            if (rayDir.X < 0)
            {
                rayStep.X = -map.CurrentLevel.TileWidth;
                rayLength1D.X = (begin.X - mapPoint.X) * rayUnitStep.X / map.CurrentLevel.TileWidth;
            }
            else
            {
                rayStep.X = map.CurrentLevel.TileWidth;
                rayLength1D.X = (map.CurrentLevel.TileWidth + mapPoint.X - begin.X) * rayUnitStep.X / map.CurrentLevel.TileWidth;

            }
            if (rayDir.Y < 0)
            {
                rayStep.Y = -map.CurrentLevel.TileHeight;
                rayLength1D.Y = (begin.Y - mapPoint.Y) * rayUnitStep.Y / map.CurrentLevel.TileWidth;
            }
            else
            {
                rayStep.Y = map.CurrentLevel.TileHeight;
                rayLength1D.Y = (map.CurrentLevel.TileWidth + mapPoint.Y - begin.Y) * rayUnitStep.Y / map.CurrentLevel.TileWidth;
            }
            #endregion

            #region Walking the Ray and Checking if it Hit

            float travelledDistance = 0;
            
            while (!hit && travelledDistance < length)
            {
                //Moving
                if (rayLength1D.X < rayLength1D.Y)
                {
                    mapPoint.X += rayStep.X;
                    travelledDistance = rayLength1D.X;
                    rayLength1D.X += rayUnitStep.X;
                }
                else
                {
                    mapPoint.Y += rayStep.Y;
                    travelledDistance = rayLength1D.Y;
                    rayLength1D.Y += rayUnitStep.Y;
                }
                
                //Checking
                if (map.CurrentLevel.Contains(mapPoint) && travelledDistance < length)
                    if (map.CurrentLevel.Organisation[(int)(mapPoint.Y - map.CurrentLevel.Pos.Y) / map.CurrentLevel.TileHeight, (int)(mapPoint.X - map.CurrentLevel.Pos.X) / map.CurrentLevel.TileWidth] > 0)
                        hit = true;
            }
            
            endPoint = begin + Vector2.Normalize(direction) * travelledDistance;

            #endregion
        }

    }
}