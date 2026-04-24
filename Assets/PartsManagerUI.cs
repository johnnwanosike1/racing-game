using System.Collections;
using TMPro;
using UnityEngine;

public class PartsManagerUI : MonoBehaviour
{
    [Header("Canvases")]
    public CanvasGroup gearsCanvas;
    public CanvasGroup selectItemCanvas;

    [Header("UI")]
    public TMP_Text selectText;
    public GameObject nextButton;
    public GameObject previousButton;

    private PartsManager partsManager;
    private PartType selectedPart;
    private int committedIndex;
    private int previewIndex;

    private void Start()
    {
        partsManager = FindObjectOfType<PartsManager>();

        if (gearsCanvas != null) gearsCanvas.gameObject.SetActive(true);
        if (selectItemCanvas != null) selectItemCanvas.gameObject.SetActive(false);
    }

    public void GoToSelectMenu(string partName)
    {
        if (partsManager == null) return;
        selectedPart = partsManager.GetPartType(partName);
        if (selectedPart == null) return;

        committedIndex = selectedPart.selected;
        previewIndex = committedIndex;

        UpdateButtons();
        SetSelectText();

        if (gearsCanvas != null && selectItemCanvas != null)
            StartCoroutine(FadeBetween(selectItemCanvas, gearsCanvas));
    }

    public void ChangePart(int delta)
    {
        if (selectedPart == null || selectedPart.parts == null) return;

        int next = Mathf.Clamp(previewIndex + delta, 0, selectedPart.parts.Length - 1);
        if (next == previewIndex) return;

        previewIndex = next;
        partsManager.SetPartFromId(selectedPart, previewIndex);

        UpdateButtons();
        SetSelectText();
    }

    public void SelectPart()
    {
        committedIndex = previewIndex;
        SetSelectText();
    }

    public void GoBack()
    {
        if (selectedPart != null)
            partsManager.SetPartFromId(selectedPart, committedIndex);

        var cam = FindObjectOfType<GarageCamera>();
        if (cam != null) cam.state = CameraState.Orbiting;

        if (gearsCanvas != null && selectItemCanvas != null)
            StartCoroutine(FadeBetween(gearsCanvas, selectItemCanvas));
    }

    private void UpdateButtons()
    {
        if (selectedPart == null || selectedPart.parts == null) return;

        if (previousButton != null) previousButton.SetActive(previewIndex > 0);
        if (nextButton != null) nextButton.SetActive(previewIndex < selectedPart.parts.Length - 1);
    }

    private void SetSelectText()
    {
        if (selectText == null) return;
        selectText.text = (committedIndex == previewIndex) ? "Selected" : "Select";
    }

    private static IEnumerator FadeBetween(CanvasGroup enable, CanvasGroup disable)
    {
        if (enable == null || disable == null) yield break;

        enable.gameObject.SetActive(true);
        enable.alpha = 0f;
        disable.alpha = 1f;

        const float duration = 0.18f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            enable.alpha = p;
            disable.alpha = 1f - p;
            yield return null;
        }

        disable.gameObject.SetActive(false);
    }
}
