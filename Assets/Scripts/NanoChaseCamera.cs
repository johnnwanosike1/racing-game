using UnityEngine;

public class NanoChaseCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 localOffset = new Vector3(0f, 3f, -7f);
    public float positionLerp = 10f;
    public float rotationLerp = 10f;

    Rigidbody targetRb;

    void Awake()
    {
        if (target != null) targetRb = target.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Use velocity to keep camera behind where the car is going
        Vector3 forward = target.forward;
        if (targetRb != null && targetRb.linearVelocity.sqrMagnitude > 0.5f)
            forward = targetRb.linearVelocity.normalized;

        Vector3 desiredPos =
            target.position
            + target.TransformVector(localOffset)
            + forward * (-2.5f);

        transform.position = Vector3.Lerp(transform.position, desiredPos, positionLerp * Time.deltaTime);

        Quaternion desiredRot = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotationLerp * Time.deltaTime);
    }
}