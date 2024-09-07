using System.Numerics;

namespace ConsoleApp2;

public struct Plane
{
    public Vector3 Normal;
    public float D;

    public Plane(float a, float b, float c, float d)
    {
        Normal = new Vector3(a, b, c);
        D = d;
    }

    public Plane Normalize()
    {
        var length = Normal.Length();
        return new Plane(Normal.X / length, Normal.Y / length, Normal.Z / length, D / length);
    }

    public float GetDistanceToPoint(Vector3 point)
    {
        return Vector3.Dot(Normal, point) + D;
    }
}