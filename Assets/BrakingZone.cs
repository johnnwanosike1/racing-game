using UnityEngine;

/// <summary>
/// Place this on a trigger collider (isTrigger = true).
/// When an AI car enters, it will reduce its target speed.
/// </summary>
[RequireComponent(typeof(Collider))]
public class BrakingZone : MonoBehaviour
{
    private void Reset()
    {
        // Ensure it's a trigger.
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private static AiCarController FindAiController(Collider other)
    {
        // Prefer root (works when collider is on wheel/child).
        var root = other.attachedRigidbody != null ? other.attachedRigidbody.transform.root : other.transform.root;
        return root != null ? root.GetComponentInChildren<AiCarController>() : other.GetComponentInParent<AiCarController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var ai = FindAiController(other);
        if (ai != null) ai.SetBrakingZone(true);
    }

    private void OnTriggerExit(Collider other)
    {
        var ai = FindAiController(other);
        if (ai != null) ai.SetBrakingZone(false);
    }
}
