using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI; // Needed for Image/Text if you use them directly in MuseumHotspot
using UnityEngine.Video; // Needed for VideoPlayer reference
using UnityEngine.EventSystems; // REQUIRED for Pointer Interfaces


/// <summary>
/// Represents an interactive hotspot in a museum skybox that can show detailed information or play audio.
/// </summary>
[System.Serializable]
public class MuseumHotspot
{
    [Header("Basic Setup")]
    public string hotspotId;
    public GameObject hotspotObject;
    
    [Header("Content")]
    public GameObject detailPanel;
    public Image detailImage;
    public Text detailText;
    
    [Header("Audio")]
    public AudioClip narrationClip;
    public AudioClip narrationClip_Spanish;
}

/// <summary>
/// Handles the interaction with hotspots in the museum experience.
/// Supports both XR controller selection and EventSystem Pointer Clicks (Mouse/Touch).
/// Interacts with AudioManager, SkyboxManager, GameSettings, and potentially specific detail panel components like NewVideoPlayer or Image components.
/// </summary>
[RequireComponent(typeof(Collider))] // Ensure collider is present for PhysicsRaycaster
[RequireComponent(typeof(XRSimpleInteractable))] // Ensure XR interactable is present
public class HotspotInteraction : MonoBehaviour, IPointerClickHandler // Implement click handler
{
    [Header("Hotspot Data")]
    [Tooltip("Core data defining this hotspot's content and behavior. Should be configured externally (e.g., via SkyboxManager setup).")]
    public MuseumHotspot hotspotData; // Assign this externally, holds panel refs, audio clips etc.

    [Header("System References")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private SkyboxManager skyboxManager;

    // Internal State
    private bool isDetailPanelActive = false; // Tracks if the associated detail panel is shown
    private XRSimpleInteractable interactable;
    private float lastInteractionTime = -1f;
    private const float debounceTime = 0.5f; // Prevent rapid re-triggering

    void Awake()
    {
        // --- Essential Component References ---
        interactable = GetComponent<XRSimpleInteractable>(); // Guaranteed by RequireComponent

        // --- Find Managers (Fallback) ---
        // Best practice is to assign these in the inspector if possible,
        // but FindObjectOfType provides a fallback.
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
            if (audioManager == null) Debug.LogError($"HotspotInteraction on {gameObject.name} could not find AudioManager!", this);
        }
        if (skyboxManager == null)
        {
            skyboxManager = FindObjectOfType<SkyboxManager>();
             if (skyboxManager == null) Debug.LogError($"HotspotInteraction on {gameObject.name} could not find SkyboxManager!", this);
        }

        // --- Initialization ---
        // Ensure this hotspot's GameObject is linked in its data (if not already done)
        if (hotspotData != null && hotspotData.hotspotObject == null)
        {
             hotspotData.hotspotObject = this.gameObject;
             Debug.LogWarning($"Hotspot '{hotspotData.hotspotId}' had null hotspotObject, assigned to {this.gameObject.name}.", this);
        }

        // Start with detail panel hidden and internal state inactive
        isDetailPanelActive = false;
        if (hotspotData != null && hotspotData.detailPanel != null)
        {
            hotspotData.detailPanel.SetActive(false);
        }

        // Hotspot GameObject itself should be activated/deactivated by SkyboxManager
        // based on the current SkyboxData. Avoid setting active state here in Awake.
        // gameObject.SetActive(false); // REMOVED - Let SkyboxManager handle this

        lastInteractionTime = Time.time - debounceTime; // Allow first interaction immediately
    }

    void Start()
    {
        // --- Subscribe to Events ---
        if (interactable != null)
        {
            // XR Interaction Events
            interactable.selectEntered.AddListener(OnHotspotSelectedXR);
            interactable.hoverEntered.AddListener(OnHotspotHoveredXR);
        } else {
            // Should not happen due to RequireComponent, but good practice
             Debug.LogError($"HotspotInteraction on {gameObject.name} missing XRSimpleInteractable component after Awake.", this);
        }

         // Ensure panel state matches internal state after Awake/OnEnable cycle
         if (hotspotData != null && hotspotData.detailPanel != null)
         {
            hotspotData.detailPanel.SetActive(isDetailPanelActive);
         }
    }

    void OnDestroy()
    {
        // --- Unsubscribe from Events ---
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnHotspotSelectedXR);
            interactable.hoverEntered.RemoveListener(OnHotspotHoveredXR);
        }
    }

    //---------------------------------------------------------------------
    // Event Handlers (XR & Pointer Click)
    //---------------------------------------------------------------------

    private void OnHotspotSelectedXR(SelectEnterEventArgs args)
    {
        Debug.Log($"Hotspot '{hotspotData?.hotspotId ?? name}' - XR Selected");
        TriggerHotspotAction();
    }

    private void OnHotspotHoveredXR(HoverEnterEventArgs args)
    {
        // Optional: Add debug log if needed
        // Debug.Log($"Hotspot '{hotspotData?.hotspotId ?? name}' - XR Hover Entered");
        TriggerHoverSound();
        // Note: Visual hover effect is handled by HoverEffect script listening to these events
    }

    // --- IPointerClickHandler Implementation (for Mouse/Touch via EventSystem) ---
    public void OnPointerClick(PointerEventData eventData)
    {
        // This method is called by the Event System when the object this script
        // is attached to (which must have a Collider) is clicked by the mouse,
        // provided a PhysicsRaycaster is on the camera.
        Debug.Log($"Hotspot '{hotspotData?.hotspotId ?? name}' - Pointer Clicked (Mouse/Touch)");
        TriggerHotspotAction();
    }

    //---------------------------------------------------------------------
    // Core Action & Sound Logic
    //---------------------------------------------------------------------

    /// <summary>
    /// Central method containing the logic to execute when the hotspot is activated/deactivated.
    /// Called by both XR selection and mouse click handlers.
    /// </summary>
    private void TriggerHotspotAction()
    {
        // --- Debounce Check ---
        if (Time.time < lastInteractionTime + debounceTime)
        {
            Debug.Log($"Ignoring interaction on '{hotspotData?.hotspotId ?? name}' - debounce active.");
            return;
        }
        lastInteractionTime = Time.time;

        // --- Null Checks for Managers ---
        if (skyboxManager == null || audioManager == null) {
             Debug.LogError($"Missing SkyboxManager or AudioManager reference on {gameObject.name}. Cannot trigger action.", this);
             return;
        }
         if (hotspotData == null) {
             Debug.LogError($"Missing HotspotData on {gameObject.name}. Cannot trigger action.", this);
             return;
         }

        // --- Action ---
        skyboxManager.PlayClickSound(); // Play click sound feedback

        // Toggle the active state
        isDetailPanelActive = !isDetailPanelActive;
        Debug.Log($"Hotspot '{hotspotData.hotspotId}' toggled. New panel state: {isDetailPanelActive}");

        // --- Handle Detail Panel and Audio ---
        if (hotspotData.detailPanel != null)
        {
            if (isDetailPanelActive)
            {
                // Prepare content *before* activating the panel GameObject
                PrepareDetailPanelContent();
                hotspotData.detailPanel.SetActive(true); // Activate the panel
                Debug.Log($"Detail panel activated for: '{hotspotData.hotspotId}'");
                PlayNarration(); // Play narration associated with the hotspot
            }
            else
            {
                hotspotData.detailPanel.SetActive(false); // Deactivate the panel
                Debug.Log($"Detail panel deactivated for: '{hotspotData.hotspotId}'");
                StopNarration(); // Stop any narration that was playing
            }
        }
        else if (HasNarration()) // Handle audio-only hotspots (no visual panel)
        {
             Debug.Log($"Hotspot '{hotspotData.hotspotId}' is audio-only.");
            if (isDetailPanelActive) // Use the same toggle bool to control audio playback
            {
                PlayNarration();
            }
            else
            {
                StopNarration();
            }
        }
        else
        {
            // Hotspot has neither a panel nor narration - log a warning.
            Debug.LogWarning($"Hotspot '{hotspotData.hotspotId}' has no detailPanel assigned and no narration clips. Click does nothing.", this);
        }
    }

     /// <summary>
    /// Plays the hover sound effect via the SkyboxManager.
    /// </summary>
    private void TriggerHoverSound()
    {
        if (skyboxManager != null)
        {
            skyboxManager.PlayHoverSound();
        }
         // Visual hover effects are handled by the separate HoverEffect script.
    }


    //---------------------------------------------------------------------
    // Detail Panel Content Preparation
    //---------------------------------------------------------------------

    /// <summary>
    /// Configures the content of the detail panel based on its type and the hotspot data.
    /// This is called *before* the panel GameObject is set active.
    /// </summary>
    // Inside HotspotInteraction.cs
private void PrepareDetailPanelContent()
{
    if (hotspotData == null || hotspotData.detailPanel == null) {
        Debug.LogError("PrepareDetailPanelContent: hotspotData or detailPanel is null.", this);
        return;
    }
    GameObject panel = hotspotData.detailPanel;
    string hotspotId = hotspotData.hotspotId; // Cache for logging clarity

    Debug.Log($"PrepareDetailPanelContent started for hotspot '{hotspotId}' targeting panel '{panel.name}'");

    // --- CHECK FOR VIDEO PLAYER FIRST ---
    if (panel.TryGetComponent<NewVideoPlayer>(out NewVideoPlayer newVideoPlayer))
    {
        string targetClipName = hotspotId; // Get the ID from data
        Debug.Log($"Panel '{panel.name}' has NewVideoPlayer. Trying to set clip based on hotspotId: '{targetClipName}'");

        if (!string.IsNullOrEmpty(targetClipName))
        {
            newVideoPlayer.clipToPlay = targetClipName; // Assign the ID
            Debug.Log($"SUCCESS - Set newVideoPlayer.clipToPlay = '{newVideoPlayer.clipToPlay}'");
        }
        else
        {
            newVideoPlayer.clipToPlay = ""; // Ensure it's empty if ID is invalid
            Debug.LogWarning($"Hotspot '{hotspotId}' targets NewVideoPlayer, but hotspotId in data is null or empty. Cannot set clipToPlay.", this);
        }
        // It's okay if this panel ALSO has a renderer, the core logic is video playback.
        // Do NOT return here necessarily if renderer setup is also needed for video display.
        // Consider if NewVideoPlayer itself should handle renderer setup.
    }
    // --- Check for PlayMP4Video (if you might use it elsewhere) ---
    else if (panel.TryGetComponent<PlayMP4Video>(out PlayMP4Video playMP4Video))
    {
         // ...(Your PlayMP4Video handling code)...
         return; // Likely return after handling this type
    }
     // --- Check for ImageSlideshowController ---
     else if (panel.TryGetComponent<ImageSlideshowController>(out var slideshow))
     {
          Debug.Log($"Panel for '{hotspotId}' is an ImageSlideshowController. Activating it.");
          // ...(Specific slideshow logic if needed)...
          return; // Likely return after handling this type
     }
    // --- Check for UI Image ---
    else if (panel.TryGetComponent<Image>(out Image uiImage) || (uiImage = panel.GetComponentInChildren<Image>()) != null)
    {
        if (hotspotData.detailImage != null && hotspotData.detailImage.sprite != null)
        {
            uiImage.sprite = hotspotData.detailImage.sprite;
            Debug.Log($"Prepared UI Image content for '{hotspotId}'");
        } else {
            Debug.LogWarning($"Hotspot '{hotspotId}' targets a UI Image panel, but hotspotData.detailImage/sprite is null.", this);
        }
        return; // Return after handling image
    }
    // --- Check for Renderer (as a fallback for static images on renderers?) ---
    // Place this check LATER now
    else if (panel.TryGetComponent<Renderer>(out Renderer rend))
    {
        // Check if it's supposed to be an image for the renderer
         if (hotspotData.detailImage != null && hotspotData.detailImage.sprite != null && hotspotData.detailImage.sprite.texture != null)
         {
             if (rend.material != null) {
                rend.material.mainTexture = hotspotData.detailImage.sprite.texture;
                Debug.Log($"Prepared Renderer Texture content for '{hotspotId}' (Image Fallback)");
             } else {
                 Debug.LogError($"Renderer on panel '{panel.name}' for hotspot '{hotspotId}' has no material.", this);
             }
         } else {
            // This warning is now more meaningful - it has a Renderer but wasn't identified as Video/Image etc.
             Debug.LogWarning($"Panel '{panel.name}' uses Renderer but has no valid image/texture AND wasn't identified as a known specific panel type (Video, Slideshow etc.).", this);
         }
         return; // Return after handling renderer
    }
    // --- Fallback ---
    else
    {
        Debug.LogWarning($"Detail panel '{panel.name}' for hotspot '{hotspotId}' doesn't match any known component types for content preparation.", this);
    }
    Debug.Log($"PrepareDetailPanelContent finished for hotspot '{hotspotId}'");
} // End of PrepareDetailPanelContent

    //---------------------------------------------------------------------
    // Audio Handling
    //---------------------------------------------------------------------

    /// <summary>
    /// Checks if this hotspot has any narration clip assigned (English or Spanish).
    /// </summary>
    private bool HasNarration()
    {
        return hotspotData != null && (hotspotData.narrationClip != null || hotspotData.narrationClip_Spanish != null);
    }

    /// <summary>
    /// Gets the appropriate narration clip based on the current language setting.
    /// </summary>
    /// <returns>The localized AudioClip, or null if none is found for the current language.</returns>
    private AudioClip GetLocalizedNarrationClip()
    {
        if (hotspotData == null) return null;

        // Check GameSettings static class for current language preference
        // Using OrdinalIgnoreCase for robust comparison (e.g., "Spanish", "spanish")
        if (!string.IsNullOrEmpty(GameSettings.CurrentLanguage) &&
            GameSettings.CurrentLanguage.Equals("Spanish", System.StringComparison.OrdinalIgnoreCase))
        {
            // If Spanish is selected and the Spanish clip exists, return it.
            if (hotspotData.narrationClip_Spanish != null)
            {
                return hotspotData.narrationClip_Spanish;
            }
            else
            {
                // Log a warning if Spanish is selected but clip is missing, fall back to default.
                 Debug.LogWarning($"Hotspot '{hotspotData.hotspotId}': Spanish language selected, but 'narrationClip_Spanish' is not assigned. Falling back to default audio.", this);
                return hotspotData.narrationClip; // Fallback to default (English) or null
            }
        }
        else
        {
            // Otherwise (English or other language set), return the default clip.
            // Optionally check if the default clip is null too.
             if (hotspotData.narrationClip == null) {
                  Debug.LogWarning($"Hotspot '{hotspotData.hotspotId}': Default 'narrationClip' (English) is not assigned.", this);
             }
            return hotspotData.narrationClip;
        }
    }

    /// <summary>
    /// Plays the localized narration clip for this hotspot using the AudioManager.
    /// </summary>
    private void PlayNarration()
    {
        if (audioManager == null) return;

        AudioClip clipToPlay = GetLocalizedNarrationClip();
        if (clipToPlay != null)
        {
            audioManager.PlayNarration(clipToPlay);
            Debug.Log($"Playing narration for hotspot '{hotspotData.hotspotId}' ({GameSettings.CurrentLanguage}) using clip: {clipToPlay.name}");
        }
        else
        {
             Debug.Log($"No narration clip found to play for hotspot '{hotspotData.hotspotId}' in language '{GameSettings.CurrentLanguage}'. Ensuring narration is stopped.");
             // Ensure any previous narration IS stopped if the new selection has none
             if (audioManager.IsNarrationPlaying()) {
                 audioManager.StopNarration();
             }
        }
    }

    /// <summary>
    /// Stops any currently playing narration via the AudioManager.
    /// </summary>
    private void StopNarration()
    {
        if (audioManager != null && audioManager.IsNarrationPlaying())
        {
            audioManager.StopNarration();
            Debug.Log($"Stopping narration potentially related to hotspot '{hotspotData?.hotspotId ?? name}'");
        }
    }


    //---------------------------------------------------------------------
    // Public Accessors / Control Methods
    //---------------------------------------------------------------------

    /// <summary>
    /// Externally forces the detail panel associated with this hotspot to close.
    /// Also stops related narration.
    /// </summary>
    public void CloseDetailPanel()
    {
        if (isDetailPanelActive) // Only act if it's currently considered active
        {
             Debug.Log($"Force closing detail panel for hotspot '{hotspotData?.hotspotId ?? name}'");
             if (hotspotData != null && hotspotData.detailPanel != null)
             {
                hotspotData.detailPanel.SetActive(false);
             }
            StopNarration();
            isDetailPanelActive = false; // Update internal state
        }
    }

    /// <summary>
    /// Checks if the detail panel associated with this hotspot is currently active.
    /// </summary>
    /// <returns>True if the panel is active, false otherwise.</returns>
    public bool IsActive()
    {
        return isDetailPanelActive;
    }
}