using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct CarPositionEntry
{
    public string displayName;
    [Tooltip("Scene instance (root object) for this car.")]
    public GameObject carObject;
    public bool isPlayer;

    [HideInInspector] public int progress;
}

/// <summary>
/// Lightweight race-position overlay.
/// If you're already using LappingSystem, you likely don't need this.
/// This version is kept separate so you can use it for split-screen / debug.
/// </summary>
public class CarPositioning : MonoBehaviour
{
    [Header("Track")]
    public Transform[] waypoints;
    public float waypointRange = 20f;
    public int lapCount = 3;

    [Header("Cars")]
    public CarPositionEntry[] cars;

    [Header("UI")]
    public TextMeshProUGUI positioningText;
    public TextMeshProUGUI lapText;

    private int[] order;
    private int playerIndex = -1;
    private bool finished;

    private int SegmentCount => (waypoints == null ? 0 : waypoints.Length) + 1; // +1 for finish line segment.

    private void Start()
    {
        order = new int[cars.Length];
        for (int i = 0; i < cars.Length; i++)
        {
            order[i] = i;
            if (cars[i].isPlayer) playerIndex = i;
        }

        RenderUI();
    }

    private void Update()
    {
        if (finished) return;

        UpdateProgress();
        SortCars();
        RenderUI();
    }

    private void UpdateProgress()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].carObject == null) continue;

            int nextWp = cars[i].progress % SegmentCount;
            if (nextWp >= waypoints.Length) continue; // finish handled by trigger.

            float d = Vector3.Distance(cars[i].carObject.transform.position, waypoints[nextWp].position);
            if (d <= waypointRange)
                cars[i].progress++;
        }
    }

    private void SortCars()
    {
        Array.Sort(order, (a, b) =>
        {
            int pa = cars[a].progress;
            int pb = cars[b].progress;
            if (pa != pb) return pb.CompareTo(pa);

            Vector3 marker = GetNextMarker(pa);
            float da = cars[a].carObject != null ? Vector3.Distance(cars[a].carObject.transform.position, marker) : float.MaxValue;
            float db = cars[b].carObject != null ? Vector3.Distance(cars[b].carObject.transform.position, marker) : float.MaxValue;
            return da.CompareTo(db);
        });
    }

    private Vector3 GetNextMarker(int progress)
    {
        int next = progress % SegmentCount;
        if (waypoints != null && next < waypoints.Length && waypoints[next] != null)
            return waypoints[next].position;
        return transform.position;
    }

    private void RenderUI()
    {
        if (positioningText != null)
        {
            string tmp = "";
            for (int i = 0; i < order.Length; i++)
            {
                tmp += (i + 1) + ": " + cars[order[i]].displayName;
                tmp += "\n";
            }
            positioningText.text = tmp;
        }

        if (lapText != null && playerIndex >= 0)
        {
            int lap = (cars[playerIndex].progress / SegmentCount) + 1;
            lapText.text = "Lap " + lap + "/" + lapCount;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        var root = other.attachedRigidbody != null ? other.attachedRigidbody.transform.root.gameObject : other.transform.root.gameObject;

        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].carObject == null || root != cars[i].carObject) continue;

            int nextSegment = cars[i].progress % SegmentCount;
            if (nextSegment == waypoints.Length)
                cars[i].progress++;

            if (playerIndex >= 0)
            {
                int lapsCompleted = cars[playerIndex].progress / SegmentCount;
                if (lapsCompleted >= lapCount)
                {
                    finished = true;
                    if (lapText != null) lapText.text = "Finished!";
                }
            }
            break;
        }
    }
}
