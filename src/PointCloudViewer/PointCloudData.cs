namespace ConsoleApp2;

public class PointCloudData
{
    public List<Point3D> Points { get; set; }
    public BoundingBox BoundingBox { get; set; }

    public PointCloudData(List<Point3D> points)
    {
        Points = points;
        BoundingBox = CalculateBoundingBox(points);
    }

    private BoundingBox CalculateBoundingBox(List<Point3D> points)
    {
        var min = new Point3D { X = float.MaxValue, Y = float.MaxValue, Z = float.MaxValue };
        var max = new Point3D { X = float.MinValue, Y = float.MinValue, Z = float.MinValue };

        foreach (var point in points)
        {
            if (point.X < min.X) min.X = point.X;
            if (point.Y < min.Y) min.Y = point.Y;
            if (point.Z < min.Z) min.Z = point.Z;

            if (point.X > max.X) max.X = point.X;
            if (point.Y > max.Y) max.Y = point.Y;
            if (point.Z > max.Z) max.Z = point.Z;
        }

        return new BoundingBox { Min = min, Max = max };
    }
}