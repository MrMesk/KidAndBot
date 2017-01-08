using UnityEngine;
using System.Collections;
using Utility;

public class CharacterControllerUtility
{

    public static PointData GetClosestPointOnCharacterController(CharacterController cController, Vector3 point)
    {
        
        Vector3 localPoint = TransformUtility.FromWorldToLocalPosition(cController.transform, point);

        float radius = cController.radius;
        float height = cController.height;
        Vector3 center = cController.center;

        Vector3 upperSphereCenter = center + Vector3.up * (height / 2 - radius);
        Vector3 lowerSphereCenter = center - Vector3.up * (height / 2 - radius);

        // Result to return
        PointData output = new PointData();

        if (localPoint.y > upperSphereCenter.y)
        {
            // Closest point is on the upper hemisphere
            Vector3 closestPointOnSphere = (localPoint - upperSphereCenter).normalized * radius;
            output.position = TransformUtility.FromLocalToWorldPosition(cController.transform, closestPointOnSphere + upperSphereCenter);
            output.normal = TransformUtility.FromLocalToWorldDirection(cController.transform, -closestPointOnSphere.normalized);

        }
        else if (localPoint.y < lowerSphereCenter.y)
        {
            // Closest point is on the lower hemisphere
            Vector3 closestPointOnSphere = (localPoint - lowerSphereCenter).normalized * radius;
            output.position = TransformUtility.FromLocalToWorldPosition(cController.transform, closestPointOnSphere + lowerSphereCenter);
            output.normal = TransformUtility.FromLocalToWorldDirection(cController.transform, -closestPointOnSphere.normalized);
            
        }
        else
        {
            // Closest point is on the cylinder
            Vector3 closestPointOnCylinder = Vector3.ProjectOnPlane((localPoint - center), Vector3.up).normalized * radius;
            output.position = TransformUtility.FromLocalToWorldPosition(cController.transform, closestPointOnCylinder + Vector3.Project(localPoint, Vector3.up) + center);
            output.normal = TransformUtility.FromLocalToWorldDirection(cController.transform, -closestPointOnCylinder.normalized);

        }

        return output;
    }

}