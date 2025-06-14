using System;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using Microsoft.Xna.Framework;

namespace Fiourp;

public class BoxColliderRotated : Collider
{
    private float rotation;
    public float Rotation
    {
        get => rotation;
        set
        {
            rotation = value;
            Rect = GetVertices();
        }
    }
    public Vector2 PivotPoint;
    
    private float widthPercentage;
    private float heightPercentage;

    public Vector2[] Rect;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="localPosition"></param>
    /// <param name="width">Width of the collider when facing right</param>
    /// <param name="height">Height of the collider when facing right</param>
    /// <param name="rotationDeg"></param>
    /// <param name="pivotPoint">Point around which the collider rotates</param>
    public BoxColliderRotated(Vector2 localPosition, int width, int height, float rotationDeg, Vector2 pivotPoint) : base()
    {
        Pos = localPosition;
        widthPercentage = width;
        heightPercentage = height;
        this.rotation = MathHelper.ToRadians(rotationDeg);
        PivotPoint = pivotPoint;
    }

    public override void Added()
    {
        base.Added();
        
        widthPercentage = widthPercentage / ParentEntity.Width;
        heightPercentage = heightPercentage / ParentEntity.Height;
        
        Rect = GetVertices();
    }

    public override void Update()
    {
        base.Update();

        while(rotation >= Math.PI) rotation -= (float)Math.PI * 2;
        while(rotation <= -Math.PI) rotation += (float)Math.PI * 2;
        
        Rect = GetVertices();
    }

    public Vector2[] GetVertices()
    {
        return new Vector2[4]
        {
            VectorHelper.RotateAround(AbsolutePosition, AbsolutePivotPoint, rotation),
            (AbsolutePosition + VectorHelper.RotateAround(new Vector2(TrueWidth, 0), AbsolutePivotPoint, rotation)),
            (AbsolutePosition + VectorHelper.RotateAround(new Vector2(TrueWidth, TrueHeight), AbsolutePivotPoint, rotation)),
            (AbsolutePosition + VectorHelper.RotateAround(new Vector2(0, TrueHeight), AbsolutePivotPoint, rotation)),
        };
    }

    public Vector2 AbsolutePivotPoint
    {
        get => ParentEntity.Pos + PivotPoint;
    }

    public override bool Collide(Vector2 point)
    {
        //Rectangle ABCD and point P
        //Calculate sum of Areas APD, DPC, CPB and PBA
        //If sum is greater than area of rectangle then point is outside the rectangle
        //Else it's in or on the rectangle (Thanks StackOverflow)
        
        float TriangleArea(Vector2 a, Vector2 b, Vector2 c)
            => Math.Abs((b.X * a.Y - a.X * b.Y) + (c.X * b.Y - b.X * c.Y) + (a.X * c.Y - c.X * a.Y)) / 2;
        
        if(TriangleArea(point, Rect[0], Rect[3]) + TriangleArea(point, Rect[3], Rect[2]) + TriangleArea(point, Rect[2], Rect[1]) + TriangleArea(point, Rect[0], Rect[1]) > TriangleArea(Rect[0], Rect[1], Rect[2]) + TriangleArea(Rect[3], Rect[1], Rect[2]))
            return false;
        return true;
    }

    public override bool Collide(BoxColliderRotated other)
        => Collision.SeparatingAxisTheorem(Rect, other.Rect);

    public override bool Collide(BoxCollider other)
        => Collision.SeparatingAxisTheorem(Rect, other.Bounds.ToPoints());

    public override bool Collide(CircleCollider other)
        => Collision.RectCircle(Bounds, other.AbsolutePosition, other.Radius);

    public override bool Collide(GridCollider other)
        => other.Collide(this);

    public void Rotate(float radians, float minRot, List<Entity> checkedCollision, Action onCollision)
    {
        int sign = Math.Sign(radians);
        radians = Math.Abs(radians);

        while(radians > 0)
        {
            float oldRot = rotation;
            rotation += minRot * sign;
            Update();

            foreach(Entity e in checkedCollision){
                if(e != ParentEntity && Collide(e)){
                    rotation = oldRot;
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
        Drawing.DrawLine(Rect[0], Rect[1], DebugColor, 1);
        Drawing.DrawLine(Rect[1], Rect[2], DebugColor, 1);
        Drawing.DrawLine(Rect[2], Rect[3], DebugColor, 1);
        Drawing.DrawLine(Rect[3], Rect[0], DebugColor, 1);
    }

    public float TrueWidth { get => widthPercentage * ParentEntity.Width; set => widthPercentage = value / ParentEntity.Width; }
    public float TrueHeight { get => heightPercentage * ParentEntity.Height; set => heightPercentage = value / ParentEntity.Height; }

    public override float Width
    {
        get => Right - Left;
        set => throw new Exception("Can't set width of BoxRotatedCollider, use TrueWidth instead");
    }
    
    public override float Height
    {
        get => Bottom - Top;
        set => throw new Exception("Can't set height of BoxRotatedCollider, use TrueHeight instead");
    }

    public override float Left
    {
        get
        {
            float minX = float.PositiveInfinity;
            foreach (Vector2 point in Rect)
            {
                if (point.X < minX)
                    minX = point.X;
            }

            return minX - ParentEntity.Pos.X;
        }
        
        set
        {
            Pos.X += value - Left;
            Rect = GetVertices();
        }
    }

    public override float Right
    {
        get
        {
            float maxX = float.NegativeInfinity;
            foreach (Vector2 point in Rect)
            {
                if (point.X > maxX)
                    maxX = point.X;
            }

            return maxX - ParentEntity.Pos.X;
        }
        
        set
        {
            Pos.X += value - Right;
            Rect = GetVertices();
        }
    }

    public override float Top
    {
        get
        {
            float minY = float.PositiveInfinity;
            foreach (Vector2 point in Rect)
            {
                if (point.Y < minY)
                    minY = point.Y;
            }

            return minY - ParentEntity.Pos.Y;
        }
        
        set
        {
            Pos.Y += value - Top;
            Rect = GetVertices();
        }
    }

    public override float Bottom
    {
        get
        {
            float maxY = float.NegativeInfinity;
            foreach (Vector2 point in Rect)
            {
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            return maxY - ParentEntity.Pos.Y;
        }

        set
        {
            Pos.Y += value - Bottom;
            Rect = GetVertices();
        }
    }
}