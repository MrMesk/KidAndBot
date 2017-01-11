using UnityEngine;
using System.Collections;
using Utility;

public static class ColliderUtility
{

    public static PointData GetClosestPointOnClollider(Collider collider, Vector3 point)
    {

        // Box collider
        if (collider.GetType() == typeof(BoxCollider))
        {
            return GetClosestPointOnBoxCollider((BoxCollider)collider, point);
        }

        // Sphere collider
        if (collider.GetType() == typeof(SphereCollider))
        {
            return GetClosestPointOnSphereCollider((SphereCollider)collider, point);
        }

        // Capsule collider
        if (collider.GetType() == typeof(CapsuleCollider))
        {
            return GetClosestPointOnBoxCapsuleCollider((CapsuleCollider)collider, point);
        }

        // Mesh collider
        if (collider.GetType() == typeof(MeshCollider))
        {
            return GetClosestPointOnBoxMeshCollider((MeshCollider)collider, point);
        }

        // Unknow collider
        throw new System.NotImplementedException();
    }

    private static PointData GetClosestPointOnBoxCollider(BoxCollider collider, Vector3 point)
    {

        // Retrieve necesary data
        Vector3 localPoint = TransformUtility.FromWorldToLocalPosition(collider.transform, point);
        Vector3 center = collider.center;
        Vector3 size = collider.size;

        // Calculate min / max bounds
        float halfSizeX = size.x / 2;
        float halfSizeY = size.y / 2;
        float halfSizeZ = size.z / 2;

        float xA = center.x - halfSizeX;
        float xB = center.x + halfSizeX;
        float yA = center.y - halfSizeY;
        float yB = center.y + halfSizeY;
        float zA = center.z - halfSizeZ;
        float zB = center.z + halfSizeZ;

        Vector3 min = new Vector3();
        Vector3 max = new Vector3();
        if (xA < xB)
        {
            min.x = xA;
            max.x = xB;
        }
        else
        {
            min.x = xB;
            max.x = xA;
        }
        if (yA < yB)
        {
            min.y = yA;
            max.y = yB;
        }
        else
        {
            min.y = yB;
            max.y = yA;
        }
        if (zA < zB)
        {
            min.z = zA;
            max.z = zB;
        }
        else
        {
            min.z = zB;
            max.z = zA;
        }

        // Find closest point on box
        Vector3 closestPointOnBox = new Vector3(
            Mathf.Clamp(
                localPoint.x,
                min.x,
                max.x
                ),
            Mathf.Clamp(
                localPoint.y,
                min.y,
                max.y
                ),
            Mathf.Clamp(
                localPoint.z,
                min.z,
                max.z
                )
            );

        // Calculate normal
        Vector3 normal = new Vector3();
        if (closestPointOnBox.x >= max.x)
        {
            normal.x = 1;
        }
        else if (closestPointOnBox.x <= min.x)
        {
            normal.x = -1;
        }
        if (closestPointOnBox.y >= max.y)
        {
            normal.y = 1;
        }
        else if (closestPointOnBox.y <= min.y)
        {
            normal.y = -1;
        }
        if (closestPointOnBox.z >= max.z)
        {
            normal.z = 1;
        }
        else if (closestPointOnBox.z <= min.z)
        {
            normal.z = -1;
        }

        // Return result
        PointData output = new PointData();
        output.position = TransformUtility.FromLocalToWorldPosition(collider.transform, closestPointOnBox);
        output.normal = TransformUtility.FromLocalToWorldDirection(collider.transform, normal);

        return output;
    }

    private static PointData GetClosestPointOnSphereCollider(SphereCollider collider, Vector3 point)
    {

        // Find closest point on sphere
        Vector3 closestPointOnSphere = (collider.center - TransformUtility.FromWorldToLocalPosition(collider.transform, point)).normalized * collider.radius;

        // Return result
        PointData output = new PointData();
        output.position = TransformUtility.FromLocalToWorldPosition(collider.transform, closestPointOnSphere);
        output.normal = TransformUtility.FromLocalToWorldDirection(collider.transform, -closestPointOnSphere.normalized);

        return output;
    }

    private static PointData GetClosestPointOnBoxCapsuleCollider(CapsuleCollider collider, Vector3 point)
    {
        throw new System.NotImplementedException();
    }

    private static PointData GetClosestPointOnBoxMeshCollider(MeshCollider collider, Vector3 point)
    {
        throw new System.NotImplementedException();
    }


}
