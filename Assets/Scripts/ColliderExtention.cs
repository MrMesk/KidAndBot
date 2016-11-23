using UnityEngine;
using System.Collections;

public static class ColliderHelper {

    public struct PointData {
        public Vector3 position;
        public Vector3 normal;
        
        public PointData(Vector3 position, Vector3 normal) {
            this.position = position;
            this.normal = normal;
        }
    }

    public static PointData GetClosestPointOnClollider(Collider collider, Vector3 point) {
        
        // Box collider
        if (collider.GetType() == typeof(BoxCollider)) {
            throw new System.NotImplementedException();
        }

        // Sphere collider
        if (collider.GetType() == typeof(SphereCollider)) {
            return GetClosestPointOnSphereCollider((SphereCollider)collider, point);
        }

        // Capsule collider
        if (collider.GetType() == typeof(CapsuleCollider)) {
            throw new System.NotImplementedException();
        }

        // Mesh collider
        if (collider.GetType() == typeof(MeshCollider)) {
            throw new System.NotImplementedException();
        }

        // Unknow collider
        throw new System.NotImplementedException();
    }

    private static PointData GetClosestPointOnBoxClollider(BoxCollider collider, Vector3 point) {



        return new PointData();
    }

    private static PointData GetClosestPointOnSphereCollider(SphereCollider collider, Vector3 point) {
        
        Vector3 localPoint = FromWorldToLocal(collider.transform, point);

        // Debug.Log("localPoint = " + localPoint + " : v = ");
        
        Vector3 pointOnSphere = (collider.center - localPoint).normalized * collider.radius;


        // Debug.DrawLine(FromLocalToWorld(collider.transform, Vector3.zero), FromLocalToWorld(collider.transform, pointOnSphere));
        
        // Debug.DrawLine(FromLocalToWorld(collider.transform, collider.center), FromLocalToWorld(collider.transform, localPoint), Color.blue);





        PointData output = new PointData();
        output.position = FromLocalToWorld(collider.transform, pointOnSphere);
        output.normal = FromLocalToWorld(collider.transform, Quaternion.LookRotation(-pointOnSphere)) * Vector3.forward;
        //Quaternion.LookRotation()

        Debug.DrawRay(point, output.normal, Color.red);


        return output;
    }

    private static PointData GetClosestPointOnBoxCapsuleCollider(CapsuleCollider collider, Vector3 point) {
        return new PointData();
    }

    private static PointData GetClosestPointOnBoxMeshCollider(MeshCollider collider, Vector3 point) {
        return new PointData();
    }

    public static Vector3 FromWorldToLocal(Transform transform, Vector3 point) {
        return transform.worldToLocalMatrix * (point - transform.position);
    }

    public static Vector3 FromLocalToWorld(Transform transform, Vector3 point) {
        return (Vector3)(transform.localToWorldMatrix * point) + transform.position;
    }

    public static Quaternion FromWorldToLocal(Transform transform, Quaternion rotation) {
        return rotation * Quaternion.Inverse(transform.rotation);
    }

    public static Quaternion FromLocalToWorld(Transform transform, Quaternion rotation) {
        return rotation * transform.rotation;
    }

}
