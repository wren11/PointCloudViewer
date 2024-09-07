namespace ConsoleApp2;

public class OctreeNode
{
    public BoundingBox BoundingBox { get; private set; }
    public List<Point3D> Points { get; private set; }
    public OctreeNode[] Children { get; private set; }

    public OctreeNode(BoundingBox boundingBox)
    {
        BoundingBox = boundingBox;
        Points = new List<Point3D>();
    }

    public void InsertPoint(Point3D point)
    {
        if (!BoundingBoxContains(point)) return;

        if (Points.Count < 28 || Children == null)
        {
            Points.Add(point);
        }
        else
        {
            if (Children == null) Subdivide();
            foreach (var child in Children)
            {
                child.InsertPoint(point);
            }
        }
    }

    private bool BoundingBoxContains(Point3D point)
    {
        return point.X >= BoundingBox.Min.X && point.X <= BoundingBox.Max.X &&
                point.Y >= BoundingBox.Min.Y && point.Y <= BoundingBox.Max.Y &&
                point.Z >= BoundingBox.Min.Z && point.Z <= BoundingBox.Max.Z;
    }

    private void Subdivide()
    {
        var center = new Point3D
        {
            X = (BoundingBox.Min.X + BoundingBox.Max.X) / 2,
            Y = (BoundingBox.Min.Y + BoundingBox.Max.Y) / 2,
            Z = (BoundingBox.Min.Z + BoundingBox.Max.Z) / 2
        };

        Children = new OctreeNode[8];

        Children[0] = new OctreeNode(new BoundingBox { Min = BoundingBox.Min, Max = center });
        Children[1] = new OctreeNode(new BoundingBox { Min = new Point3D { X = center.X, Y = BoundingBox.Min.Y, Z = BoundingBox.Min.Z }, Max = new Point3D { X = BoundingBox.Max.X, Y = center.Y, Z = center.Z } });
        Children[2] = new OctreeNode(new BoundingBox { Min = new Point3D { X = BoundingBox.Min.X, Y = BoundingBox.Min.Y, Z = center.Z }, Max = new Point3D { X = center.X, Y = center.Y, Z = BoundingBox.Max.Z } });
        Children[3] = new OctreeNode(new BoundingBox { Min = new Point3D { X = center.X, Y = BoundingBox.Min.Y, Z = center.Z }, Max = new Point3D { X = BoundingBox.Max.X, Y = center.Y, Z = BoundingBox.Max.Z } });
        Children[4] = new OctreeNode(new BoundingBox { Min = new Point3D { X = BoundingBox.Min.X, Y = center.Y, Z = BoundingBox.Min.Z }, Max = new Point3D { X = center.X, Y = BoundingBox.Max.Y, Z = center.Z } });
        Children[5] = new OctreeNode(new BoundingBox { Min = new Point3D { X = center.X, Y = center.Y, Z = BoundingBox.Min.Z }, Max = new Point3D { X = BoundingBox.Max.X, Y = BoundingBox.Max.Y, Z = center.Z } });
        Children[6] = new OctreeNode(new BoundingBox { Min = new Point3D { X = BoundingBox.Min.X, Y = center.Y, Z = center.Z }, Max = new Point3D { X = center.X, Y = BoundingBox.Max.Y, Z = BoundingBox.Max.Z } });
        Children[7] = new OctreeNode(new BoundingBox { Min = center, Max = BoundingBox.Max });
    }
}