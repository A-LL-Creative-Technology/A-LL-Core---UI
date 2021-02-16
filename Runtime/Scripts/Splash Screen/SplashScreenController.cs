using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashScreenController : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject splashScreenPanel;

    [SerializeField] private GameObject splashScreenCanvas;

    [SerializeField] private GameObject blackBackground;

    private AsyncOperation asyncOperation;

#pragma warning restore 0649

    private void Awake()
    {
        // we first wait for the scene to be ready as otherwise it will make the splash screen lag
        blackBackground.SetActive(true);

        // prepare the loading of the new scene right away but make it wait for completeness
        asyncOperation = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = false;
        asyncOperation.completed += SceneIsReady;

        videoPlayer.loopPointReached += VideoReachedEndCallback;
        videoPlayer.started += VideoStartedCallback;

        videoPlayer.Prepare();
    }

    private void SceneIsReady(AsyncOperation obj)
    {
        videoPlayer.Play();
    }

    private void VideoStartedCallback(VideoPlayer source)
    {
        Invoke("ActivateScene", 0.5f);

    }

    private void ActivateScene()
    {
        asyncOperation.allowSceneActivation = true;
    }

    private void VideoReachedEndCallback(VideoPlayer videoPlayer)
    {
        blackBackground.SetActive(false);

        // we do the animation
        AnimationController animationController = splashScreenCanvas.GetComponent<AnimationController>();

        // define the animation callback
        animationController.animations[0].SetCustomCallbacks(delegate {
            splashScreenCanvas.SetActive(false);

            SceneManager.UnloadSceneAsync("Splash Screen");
        });

        animationController.AnimateAll();
    }

    
}
