using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public struct RaycastData
    {
        public bool Hit;
        public Vector2 BeginPoint;
        public Vector2 EndPoint;

        private float? distance = null;
        private float? distanceSquared = null;

        public float Distance
        {
            get
            {
                if (distance == null)
                {
                    if (distanceSquared == null)
                        distanceSquared = Vector2.DistanceSquared(BeginPoint, EndPoint);
                    distance = (float)Math.Sqrt((float)distanceSquared);
                }
                return (float)distance;
            }
        }

        public float DistanceSquared
        {
            get
            {
                if (distanceSquared == null)
                    distanceSquared = Vector2.DistanceSquared(BeginPoint, EndPoint);
                return (float)distanceSquared;
            }
        }

        public RaycastData(Vector2 beginPoint)
        {
            BeginPoint = beginPoint;
        }
    }

    public class Raycast
    {
        public static RaycastData FastRay(Vector2 begin, Vector2 end, GridCollider grid)
            => FastRay(begin, end - begin, Vector2.Distance(begin, end), grid);

        public static RaycastData FastRay(Vector2 begin, Vector2 direction, float length, GridCollider grid)
        {
            //Ray Direction, Step Size and Original Pos Tile

            Vector2 end = begin + Vector2.Normalize(direction) * length;

            Vector2 rayDir = Vector2.Normalize(direction);
            //The hypothenus' size for one Unit (a tile width) on the x and y axis
            Vector2 rayUnitStep = new Vector2((float)Math.Sqrt(grid.TileWidth * grid.TileWidth + (rayDir.Y * grid.TileWidth / rayDir.X) * (rayDir.Y * grid.TileWidth / rayDir.X)),
                (float)Math.Sqrt(grid.TileHeight * grid.TileHeight + (rayDir.X * grid.TileHeight / rayDir.Y) * (rayDir.X * grid.TileHeight / rayDir.Y)));

            //The tile the begin point is on and the one the end point is on : position truncated to a multiple of the tile's width or height
            Vector2 mapPoint = new Vector2((float)Math.Floor(begin.X / grid.TileWidth) * grid.TileWidth, (float)Math.Floor(begin.Y / grid.TileWidth) * grid.TileHeight);

            //Ray Direction for Each Dimension and Length for non tiled objects

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

            //Walking the Ray and Checking if it Hit

            float travelledDistance = 0;
            RaycastData data = new RaycastData(begin);

            while (!data.Hit && travelledDistance < length)
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

                if (grid.Contains(mapPoint) && travelledDistance < length && grid.GridLayout[(int)(mapPoint.Y - grid.WorldPos.Y) / grid.TileHeight, (int)(mapPoint.X - grid.WorldPos.X) / grid.TileWidth])
                    data.Hit = true;
            }

            if (data.Hit)
                data.EndPoint = begin + Vector2.Normalize(direction) * travelledDistance;
            else
                data.EndPoint = end;

            return data;
        }

        public static RaycastData SlowRay(Vector2 begin, Vector2 direction, float length, List<Kinematic> checkedEntities)
        {
            direction = direction.Normalized() * 0.5f;
            RaycastData data = new RaycastData(begin);

            for (int i = 0; i < length; i++)
            {
                Vector2 end = begin + i * direction;
                foreach (Kinematic entity in checkedEntities)
                {
                    if (entity.Collider.Contains(end))
                    {
                        data.Hit = true;
                        data.EndPoint = end;
                        return data;
                    }
                }
            }

            data.Hit = false;
            data.EndPoint = begin + direction * length;

            return data;
        }
    }
}