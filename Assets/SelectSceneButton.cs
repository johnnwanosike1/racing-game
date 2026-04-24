using UnityEngine;
using UnityEngine.SceneManagement;

// Separate standalone script ONLY for the button
// Attach this directly to the Button GameObject
public class SelectSceneButton : MonoBehaviour
{
    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}