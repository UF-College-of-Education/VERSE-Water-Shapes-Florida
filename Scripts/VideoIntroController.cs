using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using UnityEngine.Video;          // Required for VideoPlayer
using System.Collections;          // Required for Coroutines

[RequireComponent(typeof(VideoPlayer))] // Ensures a VideoPlayer component exists
public class VideoIntroControllerAsyncDelayed : MonoBehaviour // Renamed again for clarity
{
    [Header("Scene Management")]
    [Tooltip("The exact name of the scene to load asynchronously.")]
    [SerializeField] private string nextSceneName = "Scenes/VerseIntroSplash";

    [Tooltip("Delay in seconds after the scene starts before beginning the background load.")]
    [SerializeField] private float loadDelaySeconds = 2.0f; // <<< ADJUST THIS DELAY AS NEEDED

    private VideoPlayer videoPlayer;
    private AsyncOperation asyncLoadOperation;
    private bool videoFinished = false;
    private bool sceneReadyToActivate = false; // Flag to indicate loading reached 90%

    void Awake()
    {
        // Get the VideoPlayer component
        videoPlayer = GetComponent<VideoPlayer>();

        // Ensure looping is off for the finish event to work correctly
        if (videoPlayer.isLooping)
        {
            Debug.LogWarning("VideoPlayer looping is enabled. Disabling loop for scene transition.");
            videoPlayer.isLooping = false;
        }

        // Validate scene name
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("Next Scene Name is not set in the script!");
            this.enabled = false; // Disable script if no scene name
            return;
        }

        // Ensure the video player is prepared IF Play On Awake is false,
        // otherwise playback might be delayed or fail.
        // If Play On Awake is true, Unity handles preparation.
        if (!videoPlayer.playOnAwake && videoPlayer.clip != null)
        {
             videoPlayer.Prepare(); // Start preparing the video non-blockingly
        }
    }

    void Start()
    {
        // Subscribe to the video end event
        videoPlayer.loopPointReached += OnVideoFinished;

        // Start the coroutine that will handle the delay and then the loading
        StartCoroutine(DelayedLoadSceneInBackground());

        // If Play On Awake is checked on the VideoPlayer, it will start automatically.
        // If Play On Awake is NOT checked, you might want to start it here or after preparation.
        // Let's assume Play On Awake is generally preferred for intros.
        // If you manually control play:
        // if (!videoPlayer.playOnAwake && videoPlayer.isPrepared) // Check if prepared if you called Prepare()
        // {
        //     videoPlayer.Play();
        // }
        // else if (!videoPlayer.playOnAwake)
        // {
        //      // If not prepared yet, you might need another coroutine to wait for prepare completion
        //      StartCoroutine(PlayWhenPrepared());
        // }

         // Recommended: Ensure "Play On Awake" is CHECKED on the VideoPlayer component for simplicity.
         if(videoPlayer.playOnAwake)
         {
             Debug.Log("VideoPlayer has Play On Awake enabled. Video should start automatically.");
         }
         else if (videoPlayer.clip != null) // Only try to play if a clip exists and not playing on awake
         {
             Debug.LogWarning("VideoPlayer does not have Play On Awake enabled. Starting manually. Consider enabling Play On Awake for intros.");
             videoPlayer.Play(); // Attempt to play immediately
         }
    }

     // Coroutine to initiate loading *after* a delay
    IEnumerator DelayedLoadSceneInBackground()
    {
        // --- WAIT FOR THE SPECIFIED DELAY ---
        if (loadDelaySeconds > 0)
        {
            Debug.Log($"Waiting {loadDelaySeconds} seconds before starting background scene load...");
            yield return new WaitForSeconds(loadDelaySeconds);
        }
        else
        {
             Debug.Log("Load delay is zero or negative, starting background load immediately.");
        }


        // --- START THE ASYNCHRONOUS LOAD ---
        Debug.Log($"Starting background load for scene: {nextSceneName}");
        asyncLoadOperation = SceneManager.LoadSceneAsync(nextSceneName);

        // Handle potential error starting the load (e.g., scene not in Build Settings)
        if (asyncLoadOperation == null)
        {
            Debug.LogError($"Failed to start loading scene '{nextSceneName}'. Check scene name and Build Settings.");
            yield break; // Stop the coroutine
        }

        // Prevent automatic activation
        asyncLoadOperation.allowSceneActivation = false;

        // Wait until the asynchronous scene is loaded (reaches 0.9 progress)
        while (asyncLoadOperation.progress < 0.9f)
        {
            // Optional: Update a loading bar here if you have one
            // loadingBar.value = asyncLoadOperation.progress;
            yield return null; // Wait for the next frame
        }

        // Scene is now loaded and ready to activate when allowed
        Debug.Log($"Scene {nextSceneName} is loaded and ready to activate.");
        sceneReadyToActivate = true;

        // Important: Check if the video *already* finished while we were loading/waiting
        MaybeActivateScene();
    }

    // --- REMAINDER OF THE SCRIPT IS LARGELY UNCHANGED ---

    // Called when the video finishes
    void OnVideoFinished(VideoPlayer vp)
    {
        if (vp != videoPlayer) return; // Ensure it's our player

        Debug.Log("Intro video finished playing.");
        videoFinished = true;

        // Unsubscribe - good practice
        videoPlayer.loopPointReached -= OnVideoFinished;

        // Try to activate the scene now that video is done
        MaybeActivateScene();
    }

    // Checks if both conditions are met to activate the new scene
    void MaybeActivateScene()
    {
        // Only activate if BOTH video is done AND scene loading is ready
        if (videoFinished && sceneReadyToActivate)
        {
            Debug.Log($"Both conditions met. Activating scene: {nextSceneName}");
            // Allow the loaded scene to activate (completes the load and switches view)
            asyncLoadOperation.allowSceneActivation = true;

            // Optional: Disable this script component once activation starts
            // this.enabled = false;
        }
    }

    // Unsubscribe logic for safety
    void OnDisable()
    {
        // Stop the coroutine if the object is disabled mid-process
        StopAllCoroutines();

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

     void OnDestroy() // Also unsubscribe on destroy
    {
         if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    // Optional: Coroutine if you need to wait for manual Prepare() call
    // IEnumerator PlayWhenPrepared()
    // {
    //     if(!videoPlayer.isPrepared)
    //     {
    //          Debug.Log("Waiting for video preparation...");
    //          yield return new WaitUntil(() => videoPlayer.isPrepared);
    //          Debug.Log("Video prepared.");
    //     }
    //     videoPlayer.Play();
    // }
}