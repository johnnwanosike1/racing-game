using UnityEngine;

public enum CameraState
{
    Orbiting,
    LookingAt
}

/// <summary>
/// Simple showroom/garage camera:
/// - Orbit with left mouse drag.
/// - Smoothly move to look at a selected part.
/// </summary>
public class GarageCamera : MonoBehaviour
{
    [Header("Orbit")]
    public Transform orbitTarget;
    public float rotationSpeed = 120f;
    public float damping = 6f;

    [Header("Look At")]
    public float lookAtDistance = 1.2f;
    public float lookAtHeightOffset = 0.15f;

    public CameraState state = CameraState.Orbiting;

    private Vector3 lastMousePosition;
    private Transform ghost;
    private Transform lookTarget;
    private PartsManagerUI cachedUI;

    private void Awake()
    {
        cachedUI = FindObjectOfType<PartsManagerUI>();
    }

    private void Start()
    {
        ghost = new GameObject("GarageCamera_Ghost").transform;
        ghost.position = transform.position;
        ghost.rotation = transform.rotation;

        if (orbitTarget == null)
        {
            Debug.LogWarning("GarageCamera: orbitTarget is not assigned. Camera will still work but won't RotateAround correctly.");
        }
    }

    private void OnDestroy()
    {
        if (ghost != null) Destroy(ghost.gameObject);
    }

    private void LateUpdate()
    {
        if (state == CameraState.Orbiting)
        {
            HandleOrbit();
            SmoothToGhost();
            return;
        }

        if (state == CameraState.LookingAt)
        {
            HandleLookAt();
        }
    }

    private void HandleOrbit()
    {
        if (Input.GetMouseButtonDown(0))
            lastMousePosition = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            float yaw = delta.x * rotationSpeed * Time.deltaTime;

            if (orbitTarget != null)
                ghost.RotateAround(orbitTarget.position, Vector3.up, yaw);
            else
                ghost.Rotate(Vector3.up, yaw, Space.World);
        }
    }

    private void SmoothToGhost()
    {
        transform.position = Vector3.Lerp(transform.position, ghost.position, Time.deltaTime * damping);
        transform.rotation = Quaternion.Slerp(transform.rotation, ghost.rotation, Time.deltaTime * damping);
    }

    private void HandleLookAt()
    {
        if (lookTarget == null)
        {
            state = CameraState.Orbiting;
            return;
        }

        Vector3 targetPos = lookTarget.position + Vector3.up * lookAtHeightOffset;
        Vector3 desiredPos = targetPos - lookTarget.forward * lookAtDistance;

        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * damping);
        Quaternion desiredRot = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * damping);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            state = CameraState.Orbiting;
            if (cachedUI != null) cachedUI.GoBack();
        }
    }

    public void LookAt(Transform target)
    {
        lookTarget = target;
        state = CameraState.LookingAt;
    }
}
