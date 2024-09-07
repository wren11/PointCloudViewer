using System.Numerics;

namespace ConsoleApp2;

public class Camera
{
    public Matrix4x4 ViewMatrix { get; set; }
    public Matrix4x4 ProjectionMatrix { get; set; }
    private Plane[] FrustumPlanes = new Plane[6];

    public void UpdateFrustumPlanes()
    {
        var viewProjection = ViewMatrix * ProjectionMatrix;

        FrustumPlanes[0] = new Plane(viewProjection.M14 + viewProjection.M11, viewProjection.M24 + viewProjection.M21, viewProjection.M34 + viewProjection.M31, viewProjection.M44 + viewProjection.M41);
        FrustumPlanes[1] = new Plane(viewProjection.M14 - viewProjection.M11, viewProjection.M24 - viewProjection.M21, viewProjection.M34 - viewProjection.M31, viewProjection.M44 - viewProjection.M41);
        FrustumPlanes[2] = new Plane(viewProjection.M14 + viewProjection.M12, viewProjection.M24 + viewProjection.M22, viewProjection.M34 + viewProjection.M32, viewProjection.M44 + viewProjection.M42);
        FrustumPlanes[3] = new Plane(viewProjection.M14 - viewProjection.M12, viewProjection.M24 - viewProjection.M22, viewProjection.M34 - viewProjection.M32, viewProjection.M44 - viewProjection.M42);
        FrustumPlanes[4] = new Plane(viewProjection.M13, viewProjection.M23, viewProjection.M33, viewProjection.M43);
        FrustumPlanes[5] = new Plane(viewProjection.M14 - viewProjection.M13, viewProjection.M24 - viewProjection.M23, viewProjection.M34 - viewProjection.M33, viewProjection.M44 - viewProjection.M43);

        for (var i = 0; i < 6; i++)
        {
            FrustumPlanes[i] = FrustumPlanes[i].Normalize();
        }
    }

    public bool IsInView(BoundingBox box)
    {
        foreach (var plane in FrustumPlanes)
        {
            var nearest = new Vector3(
                plane.Normal.X >= 0 ? box.Min.X : box.Max.X,
                plane.Normal.Y >= 0 ? box.Min.Y : box.Max.Y,
                plane.Normal.Z >= 0 ? box.Min.Z : box.Max.Z);

            Vector3? farthest = new Vector3(
                plane.Normal.X >= 0 ? box.Max.X : box.Min.X,
                plane.Normal.Y >= 0 ? box.Max.Y : box.Min.Y,
                plane.Normal.Z >= 0 ? box.Max.Z : box.Min.Z);

            if (plane.GetDistanceToPoint(nearest) < 0)
            {
                return false;
            }
        }
        return true;
    }
}