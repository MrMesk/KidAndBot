using UnityEngine;
using System.Collections;

public class TransformUtility
{

    // Local / World transition

    public static Vector3 FromWorldToLocalPosition(Transform transform, Vector3 position)
    {
        return transform.worldToLocalMatrix * (position - transform.position);
    }

    public static Vector3 FromLocalToWorldPosition(Transform transform, Vector3 position)
    {
        return (Vector3)(transform.localToWorldMatrix * position) + transform.position;
    }
    public static Vector3 FromWorldToLocalDirection(Transform transform, Vector3 direction)
    {
        return Quaternion.Inverse(transform.rotation) * direction;
    }

    public static Vector3 FromLocalToWorldDirection(Transform transform, Vector3 direction)
    {
        return transform.rotation * direction;
    }

    public static Quaternion FromWorldToLocalRotation(Transform transform, Quaternion rotation)
    {
        return rotation * Quaternion.Inverse(transform.rotation);
    }

    public static Quaternion FromLocalToWorldRotation(Transform transform, Quaternion rotation)
    {
        return rotation * transform.rotation;
    }
}
