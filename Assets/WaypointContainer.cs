using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collects waypoint transforms from this GameObject's children.
/// Waypoint objects (WP_01, WP_02, ...) as children of this container.
/// </summary>
public class WaypointContainer : MonoBehaviour
{
    [Tooltip("Waypoints are auto-collected from this object's children (excluding itself).")]
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    /// <summary>
    /// Read-only list used by AI.
    /// </summary>
    public IReadOnlyList<Transform> Waypoints => waypoints;

    private void Awake()
    {
        Refresh();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Keep it up-to-date in the editor.
        if (!Application.isPlaying)
            Refresh();
    }
#endif

    [ContextMenu("Refresh Waypoints")]
    public void Refresh()
    {
        if (waypoints == null) waypoints = new List<Transform>();
        waypoints.Clear();

        foreach (Transform tr in GetComponentsInChildren<Transform>(includeInactive: false))
        {
            if (tr == transform) continue;
            waypoints.Add(tr);
        }

        // Predictable order: by hierarchy sibling index.
        waypoints.Sort((a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));
    }
}
