using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct CarRaceEntry
{
    public string displayName;
    [Tooltip("Scene instance (root object) for this car.")]
    public GameObject carObject;
    public bool isPlayer;
    [Tooltip("Counts checkpoints + laps progression.")]
    public int progress;
}

/// <summary>
/// Simple lap + position system driven by checkpoint proximity.
///
/// Setup:
/// - Add checkpoint transforms in order.
/// - Put this script on a FinishLine trigger collider.
/// - Make sure each car's collider root matches CarRaceEntry.carObject.
/// </summary>
public class LappingSystem : MonoBehaviour
{
    [Header("Track")]
    public Transform[] checkpoints;
    public float checkpointRange = 25f;
    public int totalLaps = 2;

    [Header("Cars")]
    public CarRaceEntry[] cars;

    [Header("UI")]
    public TextMeshProUGUI positionTxt;
    public TextMeshProUGUI lapTxt;
    public GameObject finishedPanel;
    public TextMeshProUGUI finishedPosTxt;

    private int[] order;
    private int playerIndex = -1;
    private bool finished;

    private int SegmentCount => (checkpoints == null ? 0 : checkpoints.Length) + 1; // +1 for finish line segment.

    private void Start()
    {
        if (finishedPanel != null) finishedPanel.SetActive(false);

        order = new int[cars.Length];
        for (int i = 0; i < cars.Length; i++)
        {
            order[i] = i;
            if (cars[i].isPlayer) playerIndex = i;
        }

        RenderLapText();
        RenderPositionText();
    }

    private void Update()
    {
        if (finished) return;

        UpdateProgressFromCheckpoints();
        SortOrder();
        RenderLapText();
        RenderPositionText();
    }

    private void UpdateProgressFromCheckpoints()
    {
        if (checkpoints == null || checkpoints.Length == 0) return;

        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].carObject == null) continue;

            int nextCheckpointIndex = cars[i].progress % SegmentCount;
            if (nextCheckpointIndex >= checkpoints.Length) continue; // finish segment handled by finish trigger.

            float d = Vector3.Distance(cars[i].carObject.transform.position, checkpoints[nextCheckpointIndex].position);
            if (d <= checkpointRange)
                cars[i].progress++;
        }
    }

    private void SortOrder()
    {
        Array.Sort(order, (a, b) =>
        {
            int pa = cars[a].progress;
            int pb = cars[b].progress;
            if (pa != pb) return pb.CompareTo(pa); // higher progress first

            // Tie-breaker: whoever is closer to the next segment marker.
            Vector3 marker = GetNextMarkerPosition(pa);
            float da = cars[a].carObject != null ? Vector3.Distance(cars[a].carObject.transform.position, marker) : float.MaxValue;
            float db = cars[b].carObject != null ? Vector3.Distance(cars[b].carObject.transform.position, marker) : float.MaxValue;
            return da.CompareTo(db);
        });
    }

    private Vector3 GetNextMarkerPosition(int progress)
    {
        int next = progress % SegmentCount;
        if (checkpoints != null && next < checkpoints.Length && checkpoints[next] != null)
            return checkpoints[next].position;
        return transform.position; // finish line
    }

    private void RenderPositionText()
    {
        if (positionTxt == null) return;

        string tmp = "";
        for (int i = 0; i < order.Length; i++)
        {
            var entry = cars[order[i]];
            tmp += (i + 1) + ": " + entry.displayName;

            if (i > 0 && cars[order[0]].carObject != null && entry.carObject != null)
            {
                float dist = Vector3.Distance(cars[order[0]].carObject.transform.position, entry.carObject.transform.position);
                tmp += " (" + dist.ToString("F1") + "m)";
            }
            tmp += "\n";
        }

        positionTxt.text = tmp;
    }

    private void RenderLapText()
    {
        if (lapTxt == null || playerIndex < 0 || playerIndex >= cars.Length) return;

        int lap = (cars[playerIndex].progress / SegmentCount) + 1;
        lapTxt.text = "Lap: " + lap + "/" + totalLaps;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        var root = other.attachedRigidbody != null ? other.attachedRigidbody.transform.root.gameObject : other.transform.root.gameObject;

        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].carObject == null || root != cars[i].carObject) continue;

            // Only count finish line if the car has already reached the last checkpoint segment.
            int nextSegment = cars[i].progress % SegmentCount;
            if (nextSegment == checkpoints.Length)
                cars[i].progress++;

            // Player finished?
            if (playerIndex >= 0)
            {
                int playerLapsCompleted = cars[playerIndex].progress / SegmentCount;
                if (playerLapsCompleted >= totalLaps)
                {
                    finished = true;
                    ShowFinishedPanel();
                }
            }
            break;
        }
    }

    private void ShowFinishedPanel()
    {
        if (finishedPanel != null) finishedPanel.SetActive(true);
        if (finishedPosTxt == null) return;

        SortOrder();
        int pos = 1;
        for (int i = 0; i < order.Length; i++)
        {
            if (cars[order[i]].isPlayer)
            {
                pos = i + 1;
                break;
            }
        }
        finishedPosTxt.text = "YOU FINISHED!\nYOUR POSITION: " + pos;

        if (lapTxt != null) lapTxt.text = "Finished!";
    }
}
