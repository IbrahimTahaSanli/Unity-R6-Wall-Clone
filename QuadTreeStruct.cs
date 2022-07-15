using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeStruct
{
    public Node RootNode;

    public QuadTreeStruct(float width, float height, float depth)
    {
        this.RootNode = new Node(width, height, depth, Vector3.zero);
    }
    public QuadTreeStruct(Vector3 scale)
    {
        this.RootNode = new Node(scale.x, scale.y, scale.z, Vector3.zero);
    }

    public void InsertHole(Vector3 Pos, Vector3 Size)
    {
    }

    public static void OptimizeTree(Node n)
    {
        if (n.IsNodeContainer())
        {
            if ((n.leftTop == null || n.leftTop.isDestroy) && (n.rightTop == null || n.rightTop.isDestroy) && (n.leftBottom == null || n.leftBottom.isDestroy) && (n.rightBottom == null || n.rightBottom.isDestroy))
            {
                n.isDestroy = true;
                n.leftTop = null;
                n.rightTop = null;
                n.leftBottom = null;
                n.rightBottom = null;
            }
            if (n.leftTop != null)
                OptimizeTree(n.leftTop);

            if (n.rightTop != null)
                OptimizeTree(n.rightTop);
            if (n.leftBottom != null)
                OptimizeTree(n.leftBottom);
            if (n.rightBottom != null)
                OptimizeTree(n.rightBottom);

        }
    }

    public static void DestroyCircle(Node n, Vector3 point, float radius, int samplePoint)
    {
        Dictionary<Vector3, Node> rays = new Dictionary<Vector3, Node>();
        for (int i = 0; i < samplePoint; i++)
            rays.Add(point - new Vector3(Mathf.Cos((Mathf.PI * 2) / samplePoint * i) * radius, Mathf.Sin((Mathf.PI * 2) / samplePoint * i) * radius, 0), null);


        Vector3 edgePoint;
        List<Vector3> edges = new List<Vector3>(rays.Keys);
        for (int i = 0; i < rays.Count; i++)
        {
            edgePoint = edges[i];
            rays[edgePoint] = SplitUntilPoint(n, edgePoint);
        }

        DestroyInRadius(n, point, radius);



    }

    private static void DestroyInRadius(Node n, Vector3 point, float radius)
    {
        if (n.IsNodeContainer())
        {
            if (n.leftTop != null) //  && ((((point) - n.leftTop.Position).magnitude < radius) || (((point) - n.leftTop.Position).magnitude < ((point) - n.Position).magnitude))
                DestroyInRadius(n.leftTop, point, radius);
            if (n.rightTop != null) //  && (((point) - n.rightTop.Position).magnitude < radius) || (((point) - n.rightTop.Position).magnitude < ((point) - n.Position).magnitude)
                DestroyInRadius(n.rightTop, point, radius);
            if (n.leftBottom != null) // && ((((point) - n.leftBottom.Position).magnitude < radius) || (((point) - n.leftBottom.Position).magnitude < ((point) - n.Position).magnitude))
                DestroyInRadius(n.leftBottom, point, radius);
            if (n.rightBottom != null) //   && ((((point) - n.rightBottom.Position).magnitude < radius) || (((point) - n.rightBottom.Position).magnitude < ((point) - n.Position).magnitude))
                DestroyInRadius(n.rightBottom, point, radius);
        }
        else if ((n.Position - (point)).magnitude < radius)
        {
            n.isDestroy = true;
        }
    }

    private static Node SplitUntilPoint(Node n, Vector3 point)
    {
        if (n.isDestroy)
            return null;

        var po = n.IsPointInside(point);
        if (po.Item1 == PointPos.NotIn)
        {
            return null;
        }

        else if (po.Item1 == PointPos.Inside)
        {
            Node tmp;
            if (n.IsHaveChildNode())
                n.SplitNode();

            Side[] arr = n.ClosesetChildNode(point);
            foreach (Side or in arr)
            {
                switch (or)
                {
                    case Side.LeftTop:
                        if ((tmp = SplitUntilPoint(n.leftTop, point)) != null)
                            return tmp;
                        break;

                    case Side.RightTop:
                        if ((tmp = SplitUntilPoint(n.rightTop, point)) != null)
                            return tmp;
                        break;
                    case Side.LeftBottom:
                        if ((tmp = SplitUntilPoint(n.leftBottom, point)) != null)
                            return tmp;
                        break;
                    case Side.RightBottom:
                        if ((tmp = SplitUntilPoint(n.rightBottom, point)) != null)
                            return tmp;
                        break;

                }
            }

        }
        return n;
    }

}

public enum Side
{
    None = -1,
    LeftTop = 0,
    RightTop = 1,
    LeftBottom = 2,
    RightBottom = 3,
}

public enum PointPos
{
    NotIn = 0,
    Inside = 1,
    AtCorner = 2
}


public class Node
{
    const float closeness = 0.001f;
    public Vector3 Position;

    public float Width;
    public float Height;
    public float Depth;

    public bool isDestroy;

    public Node? parentNode;
    public Side side = Side.None;

    public Node leftTop;
    public Node rightTop;
    public Node leftBottom;
    public Node rightBottom;


    public Node(float Width, float Height, float depth, Vector3 pos , Node parentNode = null, Side side = Side.None)
    {
        this.Width = Width;
        this.Height = Height;
        this.Depth = depth;

        this.side = side;

        this.Position = pos;

        this.isDestroy = false;
        this.parentNode = parentNode;
    }

    public Node(float Width, float Height, float depth, Vector3 pos, bool isDestroyed, Node parentNode = null, Side side = Side.None)
    {
        this.Width = Width;
        this.Height = Height;
        this.Depth = depth;

        this.side = side;

        this.Position = pos;

        this.isDestroy = isDestroyed;
        this.parentNode = parentNode;
    }

    public void SplitNode()
    {
        this.leftTop = new Node(this.Width / 2, this.Height / 2, this.Depth, new Vector3(this.Position.x - this.Width / 4, this.Position.y + this.Height / 4, 0), this, Side.LeftTop);
        this.rightTop = new Node(this.Width / 2, this.Height / 2, this.Depth, new Vector3(this.Position.x + this.Width / 4, this.Position.y + this.Height / 4, 0), this, Side.RightTop);
        this.leftBottom = new Node(this.Width / 2, this.Height / 2, this.Depth, new Vector3(this.Position.x - this.Width / 4, this.Position.y - this.Height / 4, 0), this, Side.LeftBottom);
        this.rightBottom = new Node(this.Width / 2, this.Height / 2, this.Depth, new Vector3(this.Position.x + this.Width / 4, this.Position.y - this.Height / 4, 0), this, Side.RightBottom);
    }

    public bool IsNodeContainer()
    {
        return this.leftTop != null || this.rightTop != null || this.leftBottom != null || this.rightBottom != null;
    }

    public bool IsHaveChildNode()
    {
        return this.leftTop == null || this.rightTop == null || this.leftBottom == null || this.rightBottom == null;
    }




    public (PointPos, Side) IsPointInside(Vector3 point)
    {
        Vector3 currentPos = this.Position;
        Side s;
        float tmp = point.x - currentPos.x;
        if (Mathf.Abs(Mathf.Abs(tmp) - this.Width / 2) < closeness)
        {
            s = tmp < 0 ? Side.LeftTop : Side.RightTop;
            tmp = point.y - currentPos.y;
            if (Mathf.Abs(Mathf.Abs(tmp) - this.Height / 2) < closeness)
            {
                return (PointPos.AtCorner, tmp > 0 ? s == Side.LeftTop ? Side.LeftBottom : Side.RightBottom : s == Side.LeftTop ? Side.LeftTop : Side.RightTop);
            }
            else if ((Mathf.Abs(tmp) - this.Height / 2) < 1)
                return (PointPos.Inside, Side.None);
            else
                return (PointPos.NotIn, Side.None);
        }
        else if ((Mathf.Abs(tmp) - this.Width / 2) <= 0)
        {
            tmp = point.y - currentPos.y;
            if ((Mathf.Abs(tmp) - this.Height / 2) <= 0)
                return (PointPos.Inside, Side.None);
            else
                return (PointPos.NotIn, Side.None);
        }
        else
            return (PointPos.NotIn, Side.None);
    }

    public Side[] ClosesetChildNode(Vector3 point)
    {
        List<Side> retSide = new List<Side>();
        float[] dist = new float[4];
        dist[0] = (point - this.leftTop.Position).magnitude;
        dist[1] = (point - this.rightTop.Position).magnitude;
        dist[2] = (point - this.leftBottom.Position).magnitude;
        dist[3] = (point - this.rightBottom.Position).magnitude;

        string ASD = "";
        foreach(float i in dist)
        {
            ASD += i + " - ";
        }

        float minVal;
        int ind;
        for (int i = 0; i < 4; i++) {
            minVal = dist[i];
            ind = i;
            for (int j = 0; j < 4; j++)
            {
                if (dist[j] < minVal)
                {
                    minVal = dist[j];
                    ind = j;
                }
            }

            dist[ind] = float.MaxValue;
            retSide.Add(ind == 0 ? Side.LeftTop : ind == 1 ? Side.RightTop : ind == 2 ? Side.LeftBottom : Side.RightBottom);
        }

        return retSide.ToArray();

    }

    public static explicit operator Vector3(Node node)
    {
        return new Vector3(node.Width, node.Height, node.Depth);
    }


}
