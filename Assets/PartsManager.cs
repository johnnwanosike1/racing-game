using UnityEngine;

public class PartsManager : MonoBehaviour
{
    public PartType[] partTypes;
    private const string PrefKeyPrefix = "Parts_v2_";

    private void Start()
    {
        ApplySavedSelectionToAll();
    }

    public PartType GetPartType(string partName)
    {
        if (string.IsNullOrWhiteSpace(partName) || partTypes == null) return null;

        for (int i = 0; i < partTypes.Length; i++)
        {
            if (partTypes[i] != null && partTypes[i].partName == partName)
                return partTypes[i];
        }

        Debug.LogWarning($"PartsManager: Part type '{partName}' not found.");
        return null;
    }

    public void SetPartFromName(string partName, int selectedIndex)
    {
        var pt = GetPartType(partName);
        if (pt == null) return;
        SetPartFromId(pt, selectedIndex);
    }

    public void SetPartFromId(PartType partType, int selectedIndex)
    {
        if (partType == null || partType.parts == null || partType.parts.Length == 0) return;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, partType.parts.Length - 1);
        partType.selected = selectedIndex;
        PlayerPrefs.SetInt(PrefKeyPrefix + partType.partName, selectedIndex);

        for (int i = 0; i < partType.parts.Length; i++)
        {
            var part = partType.parts[i];
            if (part == null || part.partsObjects == null) continue;

            bool active = (i == selectedIndex);
            foreach (GameObject go in part.partsObjects)
            {
                if (go != null) go.SetActive(active);
            }
        }
    }

    private void ApplySavedSelectionToAll()
    {
        if (partTypes == null) return;
        foreach (var pt in partTypes)
        {
            if (pt == null) continue;
            int saved = PlayerPrefs.GetInt(PrefKeyPrefix + pt.partName, 0);
            SetPartFromId(pt, saved);
        }
    }
}

[System.Serializable]
public class PartType
{
    public string partName;
    public Part[] parts;
    [HideInInspector] public int selected;
}

[System.Serializable]
public class Part
{
    public GameObject[] partsObjects;
}
