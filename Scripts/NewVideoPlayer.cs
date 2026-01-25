using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO; // Needed for Path combine if using StreamingAssets URL fallback

// --- Make sure the VideoSourceData class is defined here or in another file ---
[System.Serializable]
public class VideoSourceData {
    public string identifier = "DefaultVideoName";
    public VideoClip clipAsset;
    public string webGLVideoUrl = "http://example.com/video.mp4"; // For external hosting
    // OR if using StreamingAssets for WebGL:
    // public string streamingAssetsFilename; // e.g., "MyVideo.mp4"
}
// --- End of VideoSourceData definition ---


public class NewVideoPlayer : MonoBehaviour
{
    // Replace clipList with the new list type
    [Tooltip("List of video sources, each containing an identifier, clip asset (for Android/Quest), and URL (for WebGL).")]
    public List<VideoSourceData> videoSources;

    [Tooltip("The identifier (e.g., hotspot ID) determining which video source to play.")]
    public string clipToPlay; // This identifier should match one of the 'identifier' fields in the videoSources list

    private VideoPlayer videoPlayer;

    void Awake()
    {
        // Get the VideoPlayer component safely
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null) {
            Debug.LogError($"VideoPlayer component missing on {gameObject.name}! Disabling NewVideoPlayer.", this);
            enabled = false; // Disable this script if component is missing
            return;
        }
        // Configure defaults we control
        videoPlayer.playOnAwake = false; // We will control playback
        videoPlayer.clip = null;       // Clear any clip assigned in inspector initially
        videoPlayer.url = null;        // Clear any URL assigned in inspector initially
    }

    void OnEnable()
    {
        // OnEnable runs when the GameObject is activated. Prepare and play the selected video.
        PlaySelectedVideo();
    }

    void OnDisable() {
        // Optional: Stop playback when the panel is disabled
        if (videoPlayer != null && videoPlayer.isPlaying) {
            videoPlayer.Stop();
        }
         // Unsubscribe if using prepareCompleted
         // if (videoPlayer != null) videoPlayer.prepareCompleted -= OnPrepareCompleted;
    }

    private void PlaySelectedVideo() {
        if (videoPlayer == null) return; // Safety check from Awake
        if (videoSources == null || videoSources.Count == 0) { Debug.LogError("videoSources list is empty!", this); return; }
        if (string.IsNullOrEmpty(clipToPlay)) { Debug.LogWarning($"clipToPlay identifier is null or empty on {gameObject.name}. Cannot select video.", this); return; }

        Debug.Log($"<<<< NewVideoPlayer PlaySelectedVideo CALLED on {gameObject.name}. Target Identifier: '{clipToPlay}' >>>>");

        // Find the correct VideoSourceData entry based on the identifier
        VideoSourceData selectedSource = null;
        foreach (var sourceData in videoSources)
        {
            if (sourceData != null && sourceData.identifier.Equals(clipToPlay, System.StringComparison.Ordinal))
            {
                selectedSource = sourceData;
                break;
            }
        }

        if (selectedSource == null)
        {
            Debug.LogError($"<<<< Identifier '{clipToPlay}' NOT FOUND in the videoSources list on {gameObject.name}! Available identifiers: {string.Join(", ", videoSources.ConvertAll(s => s?.identifier ?? "NULL"))} >>>>");
            return;
        }

        // --- Platform-Specific Loading ---
        #if UNITY_WEBGL && !UNITY_EDITOR // Target actual WebGL builds (NOT the Editor when platform is set to WebGL)
            // --- WebGL: Use URL ---
            if (!string.IsNullOrEmpty(selectedSource.webGLVideoUrl))
            {
                 Debug.Log($"[WebGL] Match FOUND! Setting source to URL: '{selectedSource.webGLVideoUrl}'");
                 videoPlayer.source = VideoSource.Url;
                 videoPlayer.url = selectedSource.webGLVideoUrl; // Use the URL from Inspector
                 // OR if using StreamingAssets URL:
                 // string filename = selectedSource.streamingAssetsFilename; // Get filename from data
                 // string urlPath = Path.Combine(Application.streamingAssetsPath, filename).Replace("\\", "/");
                 // Debug.Log($"[WebGL] Match FOUND! Setting source to StreamingAssets URL: '{urlPath}'");
                 // videoPlayer.source = VideoSource.Url;
                 // videoPlayer.url = urlPath;

                 PrepareAndPlay();
            }
            else
            {
                 Debug.LogError($"[WebGL] Match FOUND for identifier '{clipToPlay}', but webGLVideoUrl is empty in videoSources list on {gameObject.name}!", this);
            }

        #else
            // --- Editor, Android, Quest, Standalone: Use VideoClip asset ---
            if (selectedSource.clipAsset != null)
            {
                 Debug.Log($"[Non-WebGL Platform] Match FOUND! Setting source to VideoClip: '{selectedSource.clipAsset.name}'");
                 videoPlayer.source = VideoSource.VideoClip;
                 videoPlayer.clip = selectedSource.clipAsset; // Use the clip asset
                 PrepareAndPlay();
            }
            else
            {
                 Debug.LogError($"[Non-WebGL Platform] Match FOUND for identifier '{clipToPlay}', but clipAsset is null in videoSources list on {gameObject.name}!", this);
            }
        #endif
    }

    /// <summary>
    /// Helper method to Prepare and Play the video player.
    /// Uses prepareCompleted event for better reliability, especially with URLs.
    /// </summary>
    private void PrepareAndPlay() {
        if (videoPlayer == null) return;
        Debug.Log($"Preparing video: {(videoPlayer.source == VideoSource.Url ? videoPlayer.url : videoPlayer.clip.name)}");
        // Unsubscribe first to prevent multiple subscriptions if called rapidly
        videoPlayer.prepareCompleted -= OnPrepareCompleted;
        // Subscribe to know when preparation is done
        videoPlayer.prepareCompleted += OnPrepareCompleted;
        // Start asynchronous preparation
        videoPlayer.Prepare();
    }

    /// <summary>
    /// Called by the VideoPlayer when preparation is complete.
    /// </summary>
    private void OnPrepareCompleted(VideoPlayer source) {
        // Unsubscribe after handling to prevent leaks
        source.prepareCompleted -= OnPrepareCompleted;

        if (source == videoPlayer) // Ensure it's our player
        {
             Debug.Log($"<<<< Video Prepared: {(source.source == VideoSource.Url ? source.url : source.clip.name)}. Starting Playback. >>>>");
             source.Play();
        }
    }

} // End of NewVideoPlayer class