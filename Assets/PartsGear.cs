using UnityEngine;

/// <summary>
/// Attach to a selectable garage part. Hook SelectItem() from a UI Button or OnMouseDown.
/// </summary>
public class PartsGear : MonoBehaviour
{
    public string partName;

    private GarageCamera garageCamera;
    private PartsManagerUI ui;

    private void Awake()
    {
        garageCamera = FindObjectOfType<GarageCamera>();
        ui = FindObjectOfType<PartsManagerUI>();
    }

    public void SelectItem()
    {
        if (garageCamera != null)
            garageCamera.LookAt(transform);

        if (ui != null)
            ui.GoToSelectMenu(partName);
    }
}
