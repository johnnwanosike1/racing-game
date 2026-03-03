using UnityEngine;

/// <summary>
/// Very lightweight waypoint-following AI.
/// Requires a CarController that exposes:
///   - float speed
///   - void SetInput(float throttle, float steering, float clutch, float handbrake)
///
/// Tuning notes:
/// - Increase <see cref="lookAheadDistance"/> to make the car smoother.
/// - Lower <see cref="targetSpeed"/> or raise <see cref="turnSlowdown"/> if it overshoots corners.
/// </summary>
[RequireComponent(typeof(CarController))]
public class AiCarController : MonoBehaviour
{
    [Header("Route")]
    [SerializeField] private WaypointContainer route;
    [SerializeField, Min(0.5f)] private float reachDistance = 12f;
    [SerializeField, Min(0f)] private float lookAheadDistance = 6f;

    [Header("Steering")]
    [SerializeField, Range(5f, 90f)] private float maxSteerAngleDeg = 35f;
    [SerializeField, Range(0.5f, 10f)] private float steerResponse = 3f;

    [Header("Speed")]
    [SerializeField, Min(1f)] private float targetSpeed = 110f;
    [SerializeField, Range(0f, 2f)] private float turnSlowdown = 1.1f;
    [SerializeField, Range(0.5f, 10f)] private float throttleResponse = 2.5f;

    [Header("Braking Zones")]
    [SerializeField, Min(0f)] private float brakingZoneTargetSpeed = 55f;

    private CarController controller;
    private int waypointIndex;

    private float throttleSmoothed;
    private float steerSmoothed;

    private bool inBrakingZone;

    public int CurrentWaypointIndex => waypointIndex;
    public bool IsInBrakingZone => inBrakingZone;

    private void Awake()
    {
        controller = GetComponent<CarController>();
    }

    private void Start()
    {
        if (route == null)
            route = FindObjectOfType<WaypointContainer>();

        if (route == null || route.Waypoints == null || route.Waypoints.Count == 0)
        {
            Debug.LogError("AiCarController: No WaypointContainer/waypoints found. Disabling AI.");
            enabled = false;
            return;
        }

        waypointIndex = Mathf.Clamp(waypointIndex, 0, route.Waypoints.Count - 1);
    }

    private void Update()
    {
        var waypoints = route.Waypoints;
        if (waypoints == null || waypoints.Count == 0) return;

        Transform wp = waypoints[waypointIndex];
        if (wp == null) return;

        // Advance waypoint when close enough.
        float dist = Vector3.Distance(transform.position, wp.position);
        if (dist <= reachDistance)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Count;
            wp = waypoints[waypointIndex];
            if (wp == null) return;
        }

        // Look-ahead point for smoother steering.
        Vector3 toWp = wp.position - transform.position;
        Vector3 aimPoint = wp.position;
        if (lookAheadDistance > 0f && toWp.sqrMagnitude > 0.01f)
            aimPoint = transform.position + toWp.normalized * Mathf.Max(lookAheadDistance, dist * 0.35f);

        // Steering: signed angle to aim point.
        Vector3 localTarget = transform.InverseTransformPoint(aimPoint);
        float angleDeg = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float steerTarget = Mathf.Clamp(angleDeg / maxSteerAngleDeg, -1f, 1f);

        // Speed control: slow down more on sharper turns.
        float turnFactor = Mathf.Clamp01(Mathf.Abs(angleDeg) / maxSteerAngleDeg);
        float desiredSpeed = Mathf.Lerp(targetSpeed, targetSpeed * (1f - turnSlowdown), turnFactor);
        if (inBrakingZone)
            desiredSpeed = Mathf.Min(desiredSpeed, brakingZoneTargetSpeed);

        float speed = controller.speed;
        float speedError = desiredSpeed - speed;

        // Throttle target: simple proportional controller.
        float throttleTarget;
        if (speedError >= 0f)
        {
            // Need to accelerate.
            throttleTarget = Mathf.Clamp01(speedError / Mathf.Max(1f, desiredSpeed));
        }
        else
        {
            // Need to brake / engine brake.
            throttleTarget = Mathf.Clamp(speedError / Mathf.Max(1f, desiredSpeed), -1f, 0f);
        }

        throttleSmoothed = Mathf.Lerp(throttleSmoothed, throttleTarget, Time.deltaTime * throttleResponse);
        steerSmoothed = Mathf.Lerp(steerSmoothed, steerTarget, Time.deltaTime * steerResponse);

        controller.SetInput(throttleSmoothed, steerSmoothed, 0f, 0f);

        Debug.DrawLine(transform.position + Vector3.up * 0.2f, wp.position + Vector3.up * 0.2f, Color.yellow);
    }

    /// <summary>Called by BrakingZone triggers.</summary>
    public void SetBrakingZone(bool active)
    {
        inBrakingZone = active;
    }
}
