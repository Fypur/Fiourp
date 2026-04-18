using System;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using Microsoft.Xna.Framework;

namespace Fiourp;

public class BoxCollider : Collider
{
    public int Width;
    public int Height;

    public float Rotation;
    public Vector2 PivotPoint;

    public Vector2[] Coords { get; private set; }

    public override Rectangle Bounds => new Rectangle((ParentEntity.Pos + new Vector2(LocalLeft, LocalTop)).ToPoint(), new Vector2(LocalRight - LocalLeft, LocalBottom - LocalTop).ToPoint());
    
    public BoxCollider(Vector2 localPosition, int width, int height, float rotationDeg, Vector2 pivotPoint) : base()
    {
        LocalPos = localPosition;
        Width = width;
        Height = height;
        Rotation = MathHelper.ToRadians(rotationDeg);
        PivotPoint = pivotPoint;
    }

    public override void Added()
    {
        base.Added();
        
        RefreshVertices();
    }

    public override void Update()
    {
        base.Update();

        //put rotation between -pi and +pi
        Rotation = Rotation - (float)Math.Floor(Rotation / (2 * float.Pi)) * 2f * float.Pi;
        if (Rotation > Math.PI) Rotation -= 2 * float.Pi;
        
        RefreshVertices();
    }

    public void RefreshVertices()
    {
        Coords = new Vector2[4]
        {
            VectorHelper.RotateAround(WorldPos, AbsolutePivotPoint, Rotation),
            (VectorHelper.RotateAround(WorldPos + new Vector2(Width, 0), AbsolutePivotPoint, Rotation)),
            (VectorHelper.RotateAround(WorldPos + new Vector2(Width, Height), AbsolutePivotPoint, Rotation)),
            (VectorHelper.RotateAround(WorldPos + new Vector2(0, Height), AbsolutePivotPoint, Rotation)),
        };
    }

    public Vector2 AbsolutePivotPoint
    {
        get => ParentEntity.Pos + PivotPoint;
    }

    public override bool CollideRaw(Collider other)
    {
        if(other is BoxCollider box)
            return Collision.BoxBoxSAT(Coords, box.Coords).IsCollision;
        else if(other is AABBCollider aabb)
            return Collision.BoxBoxSAT(Coords, other.Bounds.ToPoints()).IsCollision;
        else if(other is CircleCollider circle)
            return Collision.RotatedRectCircle(Coords, other.WorldPos, circle.Radius);
        else if(other is GridCollider grid)
            return grid.CollideRaw(this);
        else
            throw new NotImplementedException($"Collision from BoxCollider with {other.GetType().Name} is not yet implemented.");
    }

    public override bool Contains(Vector2 point)
    {
        /* Idk Why I did this complicated bullshit when scalar products exist, this is only useful for convex polygons
        //Rectangle ABCD and point P
        //Calculate sum of Areas APD, DPC, CPB and PBA
        //If sum is greater than area of rectangle then point is outside the rectangle
        //Else it's in or on the rectangle (Thanks StackOverflow)
        
        float TriangleArea(Vector2 a, Vector2 b, Vector2 c)
            => Math.Abs((b.X * a.Y - a.X * b.Y) + (c.X * b.Y - b.X * c.Y) + (a.X * c.Y - c.X * a.Y)) / 2;
        
        if(TriangleArea(point, Rect[0], Rect[3]) + TriangleArea(point, Rect[3], Rect[2]) + TriangleArea(point, Rect[2], Rect[1]) + TriangleArea(point, Rect[0], Rect[1]) > TriangleArea(Rect[0], Rect[1], Rect[2]) + TriangleArea(Rect[3], Rect[1], Rect[2]))
            return false;
        return true;*/

        //project along rectangle and compare scalar products
        Vector2 r = point - Coords[0];
        float sc1 = Vector2.Dot(r, Coords[1] - Coords[0]);
        float sc2 = Vector2.Dot(r, Coords[3] - Coords[0]);
        if(sc1 < 0 || sc1 > (Coords[1] - Coords[0]).LengthSquared() || sc2 < 0 || sc2 > (Coords[3] - Coords[0]).LengthSquared())
            return false;
        return true;
    }

    //Should replace this to be handled by the physics engine
    public void Rotate(float radians, float minRot, List<Entity> checkedCollision, Action onCollision)
    {
        int sign = Math.Sign(radians);
        radians = Math.Abs(radians);

        while(radians > 0)
        {
            float oldRotation = Rotation;
            Rotation += minRot * sign;
            Update();

            foreach(Entity e in checkedCollision){
                if(e != ParentEntity && Collide(e)){
                    Rotation = oldRotation;
                    Update();
                    onCollision();
                    return;
                }
            }

            radians -= minRot;
        }
    }

    protected override void DebugRender()
    {
        Drawing.DrawLine(Coords[0], Coords[1], DebugColor, 1);
        Drawing.DrawLine(Coords[1], Coords[2], DebugColor, 1);
        Drawing.DrawLine(Coords[2], Coords[3], DebugColor, 1);
        Drawing.DrawLine(Coords[3], Coords[0], DebugColor, 1);
    }

    public float LocalLeft
    {
        get
        {
            float minX = float.PositiveInfinity;
            foreach (Vector2 point in Coords)
            {
                if (point.X < minX)
                    minX = point.X;
            }

            return minX - ParentEntity.Pos.X;
        }
    }

    public float LocalRight
    {
        get
        {
            float maxX = float.NegativeInfinity;
            foreach (Vector2 point in Coords)
            {
                if (point.X > maxX)
                    maxX = point.X;
            }

            return maxX - ParentEntity.Pos.X;
        }
    }

    public float LocalTop
    {
        get
        {
            float minY = float.PositiveInfinity;
            foreach (Vector2 point in Coords)
            {
                if (point.Y < minY)
                    minY = point.Y;
            }

            return minY - ParentEntity.Pos.Y;
        }
    }

    public float LocalBottom
    {
        get
        {
            float maxY = float.NegativeInfinity;
            foreach (Vector2 point in Coords)
            {
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            return maxY - ParentEntity.Pos.Y;
        }
    }
}