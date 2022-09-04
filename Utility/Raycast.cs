using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class Raycast
    {
        public bool Hit;
        public Vector2 BeginPoint;
        public Vector2 EndPoint;
        private float? distance = null;

        public float Distance
        {
            get
            {
                if (distance == null)
                    distance = Vector2.Distance(BeginPoint, EndPoint);
                return (float)distance;
            }
        }
        
        public enum RayTypes { Normal, MapTiles }

        public Raycast(RayTypes rayType, Vector2 begin, Vector2 direction, float length)
        {
            BeginPoint = begin;
            switch (rayType)
            {
                case RayTypes.MapTiles:
                    FastRay(begin, direction, length);
                    break;
                case RayTypes.Normal:
                    SlowRay(begin, direction, length, new(Engine.CurrentMap.Data.Solids));
                    break;
            }   
        }


        public Raycast(RayTypes rayType, Vector2 begin, Vector2 end)
        {
            BeginPoint = begin;
            switch (rayType)
            {
                case RayTypes.MapTiles:
                    FastRay(begin, end - begin, Vector2.Distance(begin, end));
                    break;
                case RayTypes.Normal:
                    SlowRay(begin, end - begin, Vector2.Distance(begin, end), new(Engine.CurrentMap.Data.Solids));
                    break;
            }   
        }

        void FastRay(Vector2 begin, Vector2 direction, float length)
        {
            #region Ray Direction, Step Size and Original Pos Tile

            Map map = Engine.CurrentMap;
            Vector2 end = begin + Vector2.Normalize(direction) * length;

            Vector2 rayDir = Vector2.Normalize(direction);

            Grid grid = (Grid)Engine.CurrentMap.Data.EntitiesByType[typeof(Grid)][0];

            //The hypothenus' size for one Unit (a tile width) on the x and y axis
            Vector2 rayUnitStep = new Vector2((float)Math.Sqrt(grid.TileWidth * grid.TileWidth + (rayDir.Y * grid.TileWidth / rayDir.X) * (rayDir.Y * grid.TileWidth / rayDir.X)),
                (float)Math.Sqrt(grid.TileHeight * grid.TileHeight + (rayDir.X * grid.TileHeight / rayDir.Y) * (rayDir.X * grid.TileHeight / rayDir.Y)));

            //The tile the begin point is on and the one the end point is on : position truncated to a multiple of the tile's width or height
            Vector2 mapPoint = new Vector2((float)Math.Floor(begin.X / grid.TileWidth) * grid.TileWidth, (float)Math.Floor(begin.Y / grid.TileWidth) * grid.TileHeight);
            #endregion

            #region Ray Direction for Each Dimension and Length for non tiled objects

            Vector2 rayStep;
            Vector2 rayLength1D;

            if (rayDir.X < 0)
            {
                rayStep.X = -grid.TileWidth;
                rayLength1D.X = (begin.X - mapPoint.X) * rayUnitStep.X / grid.TileWidth;
            }
            else
            {
                rayStep.X = grid.TileWidth;
                rayLength1D.X = (grid.TileWidth + mapPoint.X - begin.X) * rayUnitStep.X / grid.TileWidth;

            }
            if (rayDir.Y < 0)
            {
                rayStep.Y = -grid.TileHeight;
                rayLength1D.Y = (begin.Y - mapPoint.Y) * rayUnitStep.Y / grid.TileWidth;
            }
            else
            {
                rayStep.Y = grid.TileHeight;
                rayLength1D.Y = (grid.TileWidth + mapPoint.Y - begin.Y) * rayUnitStep.Y / grid.TileWidth;
            }
            #endregion

            #region Walking the Ray and Checking if it Hit

            float travelledDistance = 0;
            
            while (!Hit && travelledDistance < length)
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
                if (grid.Collider.Bounds.Contains(mapPoint) && travelledDistance < length)
                    if (((GridCollider)grid.Collider).Organisation[(int)(mapPoint.Y - grid.Collider.AbsoluteTop) / grid.TileHeight, (int)(mapPoint.X - grid.Collider.AbsoluteLeft) / grid.TileWidth] > 0)
                        Hit = true;
            }
            
            EndPoint = begin + Vector2.Normalize(direction) * travelledDistance;

            #endregion
        }

        void SlowRay(Vector2 begin, Vector2 direction, float length, List<Entity> checkedEntities)
        {
            direction = direction.Normalized() * 0.5f;

            for (int i = 0; i < length; i++)
            {
                Vector2 end = begin + i * direction;
                foreach (Entity entity in checkedEntities)
                {
                    if (entity.Collider.Collide(end))
                    {
                        Hit = true;
                        EndPoint = end;
                        return;
                    }
                }
            }

            Hit = false;
            EndPoint = begin + direction * length;
        }
    }
}