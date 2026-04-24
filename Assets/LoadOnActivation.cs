using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineSceneLoader : MonoBehaviour
{
    private PlayableDirector director;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.stopped += OnTimelineFinished;
    }

    void OnDestroy()
    {
        director.stopped -= OnTimelineFinished;  // always unsubscribe
    }

    private void OnTimelineFinished(PlayableDirector pd)
    {
        SceneManager.LoadScene("selectstages");
    }
}