using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for Image component, Canvas
using UnityEngine.XR.Interaction.Toolkit; // Required for XRSimpleInteractable
using TMPro; // Recommended for Text, use UnityEngine.UI.Text if preferred

/// <summary>
/// Manages an interactive image slideshow UI.
/// Allows navigation through a list of images using next/previous buttons.
/// Can be toggled visible/hidden via the ToggleVisibility method.
/// Can also activate/deactivate an associated Canvas and hide/show a list of other GameObjects.
/// </summary>
public class ImageSlideshowController : MonoBehaviour
{
    [Header("Image Source")]
    [Tooltip("The list of Sprites to display in the slideshow.")]
    [SerializeField] private List<Sprite> images = new List<Sprite>();

    [Header("UI Elements")]
    [Tooltip("The UI Image component where the current slideshow image will be displayed.")]
    [SerializeField] private Image displayImage;

    [Tooltip("The XR Interactable representing the 'Next' button within the slideshow.")]
    [SerializeField] private XRSimpleInteractable nextButtonInteractable;

    [Tooltip("The XR Interactable representing the 'Previous' button within the slideshow.")]
    [SerializeField] private XRSimpleInteractable previousButtonInteractable;

    [Tooltip("The XR Interactable representing the 'Previous' button within the slideshow.")]
    [SerializeField] private XRSimpleInteractable closeButtonInteractable;

    [Tooltip("Optional TextMeshProUGUI component to display the current page number (e.g., '1 / 30').")]
    [SerializeField] private TextMeshProUGUI pageIndicatorText; // Use Text if not using TextMeshPro


    [Header("Associated Elements")]
    [Tooltip("(Optional) Assign another Canvas GameObject that should be activated/deactivated along with this slideshow.")]
    [SerializeField] private Canvas associatedCanvas;

    [Tooltip("A list of GameObjects (e.g., hotspots, nav buttons) to automatically hide when the slideshow is shown and show when it's hidden.")]
    [SerializeField] private List<GameObject> objectsToHide = new List<GameObject>();


    [Header("Configuration")]
    [Tooltip("Should the slideshow wrap around from the last image to the first, and vice-versa?")]
    [SerializeField] private bool wrapAround = true;

    [Tooltip("Sound effect to play when changing images.")]
    [SerializeField] private AudioClip changeImageSound;
    // --- NEW: Optional sounds for open/close ---
    [Tooltip("Sound effect to play when the slideshow opens.")]
    [SerializeField] private AudioClip openSound;
     [Tooltip("Sound effect to play when the slideshow closes.")]
    [SerializeField] private AudioClip closeSound;
    // --- END NEW ---
    [SerializeField] private AudioSource audioSource; // Assign an AudioSource component
[Header("Toggle Settings")]
[Tooltip("Minimum time in seconds between toggling the slideshow visibility.")]
[SerializeField] private float toggleCooldown = 1.0f; // Exposed to Inspector
private float lastToggleTime = -1.0f; // Internal timer
    // Internal state
    private int currentIndex = 0;

    void Awake()
    {
        ValidateSetup();

        if (nextButtonInteractable != null)
        {
            nextButtonInteractable.selectEntered.AddListener(HandleNextButtonSelected);
        }
        if (previousButtonInteractable != null)
        {
            previousButtonInteractable.selectEntered.AddListener(HandlePreviousButtonSelected);
        }
        if (closeButtonInteractable != null)
        {
            closeButtonInteractable.selectEntered.AddListener(HandleCloseButtonSelected);
        }

        // Ensure the slideshow AND associated canvas start hidden.
        gameObject.SetActive(false);
        if (associatedCanvas != null)
        {
            associatedCanvas.gameObject.SetActive(false);
        }
         lastToggleTime = -toggleCooldown;
    }
    private void OnEnable()
    {
        ShowImageAtIndex(currentIndex, false);
        UpdateNavigationButtons();
        ToggleExternalObjects(false);
    }

    void OnDestroy()
    {
        if (nextButtonInteractable != null)
        {
            nextButtonInteractable.selectEntered.RemoveListener(HandleNextButtonSelected);
        }
        if (previousButtonInteractable != null)
        {
            previousButtonInteractable.selectEntered.RemoveListener(HandlePreviousButtonSelected);
        }
    }

    private void ValidateSetup()
    {
        // (Validation code remains the same)
         if (displayImage == null) { Debug.LogError("...", this); enabled = false; return; }
         if (nextButtonInteractable == null || previousButtonInteractable == null) { Debug.LogError("...", this); enabled = false; return; }
         if (audioSource == null) { /* Find or add AudioSource */ Debug.LogWarning("...", this); }
    }

    // --- Public Methods to Control the Slideshow ---

     public void ToggleVisibility()
    {
        // --- NEW: Cooldown Check ---
        if (Time.time < lastToggleTime + toggleCooldown)
        {
            Debug.Log("ToggleVisibility called too soon, ignoring due to cooldown.");
            return; // Exit if within the cooldown period
        }
        // --- END NEW ---

        // Check if the slideshow's main GameObject is currently active
        if (gameObject.activeSelf)
        {
            // If it's active, hide it
            Hide();
        }
        else
        {
            // If it's inactive, show it
            Show();
        }

        // --- NEW: Update Cooldown Timer ---
        // Record the time this toggle occurred AFTER successfully showing/hiding
        lastToggleTime = Time.time;
        // --- END NEW ---
    }
    // --- END MODIFIED ---

    /// <summary>
    /// Internal logic to show the slideshow and associated elements.
    /// Called by ToggleVisibility when the slideshow is hidden.
    /// </summary>
    private void Show() // Changed to private, called by ToggleVisibility
    {
        // Hide the specified external objects FIRST
        ToggleExternalObjects(false); // false = set inactive

        // Activate this slideshow object
        gameObject.SetActive(true);

        // Activate the associated canvas if it exists
        if (associatedCanvas != null)
        {
            associatedCanvas.gameObject.SetActive(true);
        }

        currentIndex = 0; // Reset to the first image when shown
        ShowImageAtIndex(currentIndex, false); // Update display without sound on show
        UpdateNavigationButtons();
        PlaySound(openSound); // Play open sound
    }

    /// <summary>
    /// Internal logic to hide the slideshow and associated elements.
    /// Called by ToggleVisibility when the slideshow is visible.
    /// </summary>
    private void Hide() // Changed to private, called by ToggleVisibility
    {
        // Deactivate this slideshow object
        gameObject.SetActive(false);

        // Deactivate the associated canvas if it exists
        if (associatedCanvas != null)
        {
            associatedCanvas.gameObject.SetActive(false);
        }

        // Show the specified external objects again
        ToggleExternalObjects(true); // true = set active

        PlaySound(closeSound); // Play close sound
    }

    // ToggleExternalObjects remains the same
    private void ToggleExternalObjects(bool activate)
    {
        if (objectsToHide == null || objectsToHide.Count == 0) return;
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
            {
                obj.SetActive(activate);
            }
        }
    }

    // ShowNextImage and ShowPreviousImage remain public as they are called by internal buttons
    public void ShowNextImage()
    {
        if (images == null || images.Count == 0) return;
        int nextIndex = currentIndex + 1;
        if (!wrapAround && nextIndex >= images.Count) { return; }
        if (wrapAround && nextIndex >= images.Count) { nextIndex = 0; }
        ShowImageAtIndex(nextIndex); // Uses default playSound = true
    }

    public void ShowPreviousImage()
    {
         if (images == null || images.Count == 0) return;
        int prevIndex = currentIndex - 1;
         if (!wrapAround && prevIndex < 0) { return; }
        if (wrapAround && prevIndex < 0) { prevIndex = images.Count - 1; }
        ShowImageAtIndex(prevIndex); // Uses default playSound = true
    }


    // --- Internal Logic --- (ShowImageAtIndex, UpdateNavigationButtons, etc. remain the same) ---

    private void ShowImageAtIndex(int index, bool playSound = true)
    {
        // (Method remains the same)
        if (images == null || images.Count == 0) { /* Handle empty list */ return; }
        currentIndex = index;
        if (displayImage != null) displayImage.sprite = images[currentIndex];
        UpdatePageIndicator(currentIndex + 1, images.Count);
        UpdateNavigationButtons();
        // Use changeImageSound only when actually changing image via Next/Prev
        if (playSound) PlaySound(changeImageSound);
    }

    private void UpdateNavigationButtons()
    {
        // (Method remains the same)
        if (images.Count <= 1) { /* Disable both */ return; }
        if (!wrapAround) { /* Set based on index */ } else { /* Enable both */ }
    }

    private void SetButtonInteractability(XRSimpleInteractable interactable, bool isInteractable)
    {
        // (Method remains the same)
        if (interactable != null) { /* Enable/disable interactable, collider, renderer */ }
    }

    private void UpdatePageIndicator(int current, int total)
    {
        // (Method remains the same)
        if (pageIndicatorText != null) { /* Set text */ }
    }

    private void PlaySound(AudioClip clip)
    {
        // (Method remains the same)
        if (audioSource != null && clip != null && audioSource.isActiveAndEnabled) { audioSource.PlayOneShot(clip); }
    }

    private void HandleNextButtonSelected(SelectEnterEventArgs args)
    {
        ShowNextImage();
    }
    private void HandlePreviousButtonSelected(SelectEnterEventArgs args)
    {
        ShowPreviousImage();
    }
    private void HandleCloseButtonSelected(SelectEnterEventArgs args)
    {
        ToggleExternalObjects(true);
        gameObject.SetActive(false);
    }
}