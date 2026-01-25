using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics.Tracing; // Note: System.Diagnostics.Tracing might not be needed unless used elsewhere.
using UnityEngine.Experimental.Rendering; // Note: UnityEngine.Experimental.Rendering might not be needed unless used elsewhere.
using UnityEngine.Rendering; // Required for GraphicsFormat in recent Unity versions

public class PlayMP4Video : MonoBehaviour
{
    // Reference to the VideoPlayer component
    private VideoPlayer videoPlayer;

    // Reference to RawImage for displaying the video
    public RawImage rawImage;

    // RenderTexture to display video
    public RenderTexture renderTexture;

    // Path to the video file in the StreamingAssets folder (relative path)
    public string videoFileName = "SampleVideo.mp4";

    // Inspector options
    [Header("Playback Options")]
    public bool infiniteLoop = true;
    public bool muteAudio = false;

    // Reference to RectTransform to adjust the aspect ratio
    private RectTransform rawImageRectTransform;
    void Start()
    {
        // Add VideoPlayer component if not already attached
        videoPlayer = gameObject.GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

        // Set the source to the video from the StreamingAssets folder
        string videoPath = GetVideoFilePath();
        videoPlayer.source = VideoSource.Url; // Explicitly set source type
        videoPlayer.url = videoPath;
        videoPlayer.frame = 1; // Start from the first frame

        // Set the output mode for the video
        if (rawImage != null)
        {
            rawImageRectTransform = rawImage.GetComponent<RectTransform>();

            // For rendering to a UI element (RawImage)
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;

            if (renderTexture == null)
            {
                // Create a new RenderTexture if one isn't assigned
                // Use video dimensions if possible after preparation, or default
                renderTexture = new RenderTexture(1920, 1080, 0); // Default size, adjusted later
            }
            // Use a compatible format, e.g., ARGB32
            renderTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.ARGB32, true);
            // Depth buffer not typically needed for video display
            renderTexture.depthStencilFormat = GraphicsFormat.None;

            rawImage.texture = renderTexture;
            videoPlayer.targetTexture = renderTexture;

            // Set the aspect ratio once the video is prepared
            videoPlayer.prepareCompleted += AdjustAspectRatio;
        }
        else
        {
            // Handle cases where rawImage is null if necessary
            Debug.LogWarning("RawImage reference is not set. Video will not be displayed on UI.");
            videoPlayer.renderMode = VideoRenderMode.CameraNearPlane; // Or another suitable mode
        }

        // Set playback options from Inspector
        videoPlayer.playOnAwake = true;
        videoPlayer.isLooping = infiniteLoop;

        // Setup audio output and mute option
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct; // Or AudioSource if you need effects/mixing
        videoPlayer.SetDirectAudioMute(0, muteAudio);

        // Prepare the video asynchronously
        videoPlayer.Prepare();

        // Optional: Log when preparation starts
        Debug.Log("Preparing video...");
    }

    // Get the video file path for different platforms
   // Get the video file path for different platforms
    private string GetVideoFilePath()
    {
        string videoPath;
        string baseStreamingAssetsPath = Application.streamingAssetsPath;

        #if UNITY_EDITOR
            // In the Editor, Application.streamingAssetsPath is the file path. Prepend file://
            // Use Path.Combine for correct directory separators then add prefix.
            videoPath = "file://" + System.IO.Path.Combine(baseStreamingAssetsPath, videoFileName);
        #elif UNITY_WEBGL
            // In WebGL, Application.streamingAssetsPath is already a URL. Use it directly.
            // Avoid Path.Combine for URLs, manually concatenate with forward slash.
            videoPath = System.IO.Path.Combine(baseStreamingAssetsPath, videoFileName);
             // Optional: Ensure forward slashes just in case Path.Combine used backslashes
             videoPath = videoPath.Replace("\\", "/");
        #elif UNITY_ANDROID
            // Android needs the path directly (VideoPlayer handles internal JAR access)
            videoPath = System.IO.Path.Combine(baseStreamingAssetsPath, videoFileName);
        #elif UNITY_IOS
            // iOS needs the path directly
            videoPath = System.IO.Path.Combine(baseStreamingAssetsPath, videoFileName);
        #else
            // Other Standalone Platforms (Windows, Mac, Linux)
            videoPath = "file://" + System.IO.Path.Combine(baseStreamingAssetsPath, videoFileName);
        #endif

        // Log the path being used for the current platform
        Debug.Log($"Attempting to load video for platform {Application.platform} from path: {videoPath}");
        return videoPath;
    }

    // Adjust the aspect ratio of the RawImage to match the video
    void AdjustAspectRatio(VideoPlayer vp)
    {
        if (rawImage == null || rawImageRectTransform == null) return;

        // Ensure the texture dimensions are valid
        if (vp.texture == null || vp.texture.width == 0 || vp.texture.height == 0)
        {
            Debug.LogError("Video texture is not ready or has invalid dimensions.");
            return;
        }

        float videoWidth = vp.texture.width;
        float videoHeight = vp.texture.height;
        float videoAspectRatio = videoWidth / videoHeight;

        Debug.Log($"Video Prepared. Resolution: {videoWidth}x{videoHeight} (Aspect Ratio: {videoAspectRatio})");

        // Adjust the RenderTexture size to match the video
        if (renderTexture != null && (renderTexture.width != videoWidth || renderTexture.height != videoHeight))
        {
            // Release existing texture before resizing
            if (renderTexture.IsCreated())
            {
                renderTexture.Release();
            }
            renderTexture.width = (int)videoWidth;
            renderTexture.height = (int)videoHeight;
            renderTexture.Create();
            Debug.Log($"RenderTexture resized to {videoWidth}x{videoHeight}");
        }


        // Option 1: Fit width, adjust height
        // float parentWidth = rawImageRectTransform.rect.width;
        // float newHeight = parentWidth / videoAspectRatio;
        // rawImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

        // Option 2: Use AspectRatioFitter component (Recommended)
        // If you add an AspectRatioFitter component to the RawImage GameObject,
        // you can set its mode and aspect ratio like this:
         var fitter = rawImage.GetComponent<AspectRatioFitter>();
         if (fitter != null)
         {
             fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent; // Or WidthControlsHeight, HeightControlsWidth etc.
             fitter.aspectRatio = videoAspectRatio;
         }
         else
         {
             Debug.LogWarning("AspectRatioFitter component not found on RawImage. Add one for automatic aspect ratio handling.");
              // Fallback to manual sizing if fitter is not present
             float parentWidth = rawImageRectTransform.rect.width;
             float newHeight = parentWidth / videoAspectRatio;
             rawImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
         }


        // Start video playback after preparation and adjustments (optional, can be controlled externally)
        // vp.Play(); // You might want to control playback explicitly via buttons
        Debug.Log("Video ready to play.");
    }

    // Public method to toggle play/pause.
    // Assign this function to a Button's OnClick event in the Inspector.
    public void TogglePlayPause()
    {
        if (videoPlayer == null) return; // Safety check

        if (!videoPlayer.isPrepared)
        {
            Debug.LogWarning("Video not prepared yet. Cannot toggle play/pause.");
            return;
        }

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            Debug.Log("Video Paused");
        }
        else
        {
            videoPlayer.Play();
            Debug.Log("Video Playing");
        }
    }

    public void PlayVideo()
    {
        if (videoPlayer == null) return; // Safety check

        if (!videoPlayer.isPrepared)
        {
             Debug.LogWarning("Video not prepared yet. Cannot play.");
            // Optionally, you could call Prepare() again or wait.
            return;
        }

        // Check if the video is NOT currently playing
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            Debug.Log("Video Play requested (was paused or stopped).");
        }
        else
        {
            Debug.Log("Video is already playing.");
        }
    }
  


 
    public void PauseVideo()
    {
         if (videoPlayer == null) return;
         if (!videoPlayer.isPrepared) return;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            Debug.Log("Video Paused");
        }
    }


    public void StopVideo()
    {
         if (videoPlayer == null) return;
         if (!videoPlayer.isPrepared) return;

        videoPlayer.Stop();
         Debug.Log("Video Stopped");
    }

    // Clean up RenderTexture when the object is destroyed
    void OnDestroy()
    {
        if (videoPlayer != null)
        {
             // Unsubscribe from the event to prevent memory leaks
            videoPlayer.prepareCompleted -= AdjustAspectRatio;
        }

        // Release the RenderTexture if it was created by this script
        if (renderTexture != null && renderTexture.IsCreated())
        {
            // Check if the render texture was created dynamically or assigned
            // A simple check might be if it was null initially in Start
            // For simplicity here, we assume if it exists and was created, we release it
            renderTexture.Release();
             // Optionally destroy the asset if it was created via 'new RenderTexture()'
             // Destroy(renderTexture); // Be careful if it's an asset in the project
            Debug.Log("RenderTexture released.");
        }
    }
}