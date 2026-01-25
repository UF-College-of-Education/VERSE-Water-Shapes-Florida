using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Main controller for the VR Museum experience that manages skyboxes and transitions.
/// </summary>
public class SkyboxManager : MonoBehaviour
{
    [Header("Skybox Configuration")]
    [SerializeField] public List<SkyboxData> skyboxes = new List<SkyboxData>();
    [SerializeField] private int startingSkyboxIndex = 0;
    [SerializeField] private float fadeDuration = 1.0f;
    
    [Header("Debug Options")]
    [Tooltip("Set to 0 to use normal navigation. Set to any valid skybox index to skip to that skybox on Start.")]
    [SerializeField] private int skipToSkyboxIndex = 0;
    [SerializeField] private bool inspectSkyboxButtonsEnabled = true;
    
    [Header("Input Configuration")]
    [SerializeField] private XRRayInteractor leftRayInteractor;
    [SerializeField] private XRRayInteractor rightRayInteractor;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private bool enableKeyboardNavigation = true;
    public GameObject imageCanvas;

    [Header("Platform Configuration")]
    [SerializeField] private GameObject xrRigObject;
    [SerializeField] private Camera webGLCamera;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    
    [Header("Ambient Audio")]
    [SerializeField] private bool useAmbientAudio = true;
    [SerializeField] private bool crossfadeAmbientAudio = true;
    [SerializeField] private AudioClip defaultAmbientAudio;
    [SerializeField] private float defaultAmbientVolume = 0.3f;

    // Current state
    private int currentSkyboxIndex = -1;
    private SkyboxData currentSkybox;
    private bool isTransitioning = false;
    private float keyboardNavigationCooldown = 0.5f;
    private float lastKeyPressTime = 0f;

    // Reference to globally accessible reset button
    [SerializeField] private XRSimpleInteractable resetButton;
    
    // Reference to the fade panel for transitions
    [SerializeField] private CanvasGroup fadePanel;
    
    // Skybox navigation buttons for inspector mode
    [SerializeField] private GameObject inspectButtonsContainer;

      // --- Variables for Manual Input Handling ---
    #if UNITY_WEBGL || UNITY_EDITOR
    private GameObject currentlyHoveredObject = null; // Tracks the object currently under the mouse
    private PointerEventData pointerEventData;      // Reusable event data for ExecuteEvents
    private Vector2 lastMousePosition;              // For drag delta
    private bool isDragging = false;                // Camera drag state
    #endif

    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = GetComponent<AudioManager>();
            if (audioManager == null)
            {
                audioManager = gameObject.AddComponent<AudioManager>();
            }
        }

        // Configure platform-specific settings
        ConfigurePlatformSettings();

         // Initialize PointerEventData if we might run WebGL/Editor logic
        #if UNITY_WEBGL || UNITY_EDITOR
        if (EventSystem.current != null)
        {
            pointerEventData = new PointerEventData(EventSystem.current);
        } else {
             Debug.LogError("No EventSystem found! Mouse Hover/Click will not work.");
        }
        #endif
        
        // Initialize fade panel if it exists
        if (fadePanel != null)
        {
            fadePanel.alpha = 1f; // Start fully faded out
        }
    }

    private void Start()
    {
        // Register reset button listener
        if (resetButton != null)
        {
            resetButton.selectEntered.AddListener(OnResetButtonPressed);
        }
        
        // Start ambient audio if available
        if (useAmbientAudio && defaultAmbientAudio != null)
        {
            audioManager.PlayAmbientAudio(defaultAmbientAudio, defaultAmbientVolume, true, false);
        }

        // Set up inspection UI if enabled
        SetupInspectionUI();
        SetupSkyboxHotspots();
        
        // Navigate to starting or skip-to skybox
        int initialSkyboxIndex = skipToSkyboxIndex > 0 && skipToSkyboxIndex < skyboxes.Count 
            ? skipToSkyboxIndex 
            : startingSkyboxIndex;
            
        NavigateToSkybox(initialSkyboxIndex);
    }
    
    private void SetupInspectionUI()
    {
        if (!inspectSkyboxButtonsEnabled || inspectButtonsContainer == null)
        {
            if (inspectButtonsContainer != null)
                inspectButtonsContainer.SetActive(false);
            return;
        }
        
        // Clear existing buttons
        foreach (Transform child in inspectButtonsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        // Create buttons for each skybox
        for (int i = 0; i < skyboxes.Count; i++)
        {
            int skyboxIndex = i; // Capture for lambda
            
            // Create a button for each skybox
            GameObject buttonObj = new GameObject($"Button_Skybox_{i}");
            buttonObj.transform.SetParent(inspectButtonsContainer.transform, false);
            
            // Add interactable component
            var interactable = buttonObj.AddComponent<XRSimpleInteractable>();
            
            // Add collider
            var collider = buttonObj.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.1f, 0.1f, 0.05f);
            
            // Position buttons vertically
            buttonObj.transform.localPosition = new Vector3(0, 0.2f - (i * 0.12f), 0);
            
            // Add callback to navigate to the skybox
            interactable.selectEntered.AddListener((args) => {
                NavigateToSkybox(skyboxIndex);
            });
            
            // Create a text label
            var textObj = new GameObject("Label");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            // Add TextMesh component
            var textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = $"{i}: {skyboxes[i].displayName ?? skyboxes[i].skyboxId}";
            textMesh.characterSize = 0.01f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = Color.white;
            textObj.transform.localPosition = new Vector3(0, 0, -0.03f);
            textObj.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        
        inspectButtonsContainer.SetActive(true);
    }
    void SetupSkyboxHotspots()
    {
        foreach (Transform child in GameObject.Find("Hotspots").transform)
        {
            foreach (Transform grandchild in child)
            {
                if (grandchild.GetComponent<HotspotInteraction>() != null)
                {
                    string id = grandchild.gameObject.name.Substring(8, 2);
                    var index = Array.FindIndex(skyboxes.Find(x => x.skyboxId.Contains(id)).hotspots, row => row.hotspotId == grandchild.GetComponent<HotspotInteraction>().hotspotData.hotspotId);
                    skyboxes.Find(x => x.skyboxId.Contains(id)).hotspots[index] = grandchild.GetComponent<HotspotInteraction>().hotspotData;
                }
            }
        }
    }

    private void ConfigurePlatformSettings()
    {
        #if UNITY_WEBGL
        // Configure for WebGL
        if (xrRigObject != null) xrRigObject.SetActive(false);
        if (webGLCamera != null) webGLCamera.gameObject.SetActive(true);
        
        // Ensure cursor is visible and unlocked
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        #else
        // Configure for VR
        if (xrRigObject != null) xrRigObject.SetActive(true);
        if (webGLCamera != null) webGLCamera.gameObject.SetActive(false);
        #endif
    }

    /// <summary>
    /// Navigates to the specified skybox.
    /// </summary>
    public void NavigateToSkybox(int skyboxIndex)
    {
        if (isTransitioning || skyboxIndex < 0 || skyboxIndex >= skyboxes.Count)
            return;

        StartCoroutine(TransitionToSkybox(skyboxIndex));
    }

    /// <summary>
    /// Navigates to the next skybox in the list.
    /// </summary>
    public void NavigateToNextSkybox()
    {
        if (isTransitioning)
            return;
            
        int nextIndex = (currentSkyboxIndex + 1) % skyboxes.Count;
        NavigateToSkybox(nextIndex);
    }
    
    /// <summary>
    /// Navigates to the previous skybox in the list.
    /// </summary>
    public void NavigateToPreviousSkybox()
    {
        if (isTransitioning)
            return;
            
        int prevIndex = (currentSkyboxIndex - 1 + skyboxes.Count) % skyboxes.Count;
        NavigateToSkybox(prevIndex);
    }

    private IEnumerator TransitionToSkybox(int targetIndex)
    {
        isTransitioning = true;

        // Deactivate current skybox
        if (currentSkybox != null)
        {
            if (currentSkybox.hotspotParent != null)
                currentSkybox.hotspotParent.SetActive(false);
            currentSkybox.DeactivateAllInteractables();
            imageCanvas.SetActive(false);

            // Fade out screen
            if (fadePanel != null)
            {
                yield return StartCoroutine(FadeScreen(0f, 1f, fadeDuration * 0.5f));
            }
            else
            {
                // Simple delay if no fade panel
                // Using WaitForSeconds is simpler here than manual time checking
                yield return new WaitForSeconds(fadeDuration * 0.5f);
            }
        }

        // Set new skybox
        currentSkyboxIndex = targetIndex;
        currentSkybox = skyboxes[currentSkyboxIndex];
        // This logic is causing strange things to happen with the canvas. Turning it off for now - JJ

        /*// Activate image canvas only if needed for the specific skybox (assuming this logic is correct)
        if (currentSkybox.hotspots != null && currentSkybox.hotspots.Length != 0 && imageCanvas != null)
        {
            imageCanvas.SetActive(true);
        }
        else if (imageCanvas != null)
        {
             imageCanvas.SetActive(false); // Ensure it's off otherwise
        }*/


        // Set the skybox material
        RenderSettings.skybox = currentSkybox.skyboxMaterial;
        DynamicGI.UpdateEnvironment(); // Good practice after changing skybox material

        // ----- MODIFICATION START -----
        // Play entry audio using the localized property

        // Get the correct localized clip using the property we added to SkyboxData
        AudioClip clipToPlay = currentSkybox.LocalizedEntryAudioClip;

        // Play the single localized clip using the (presumably modified) AudioManager
        // Ensure audioManager reference is valid
        if (audioManager != null)
        {
            if (clipToPlay != null)
            {
                // Call the modified PlayNarration with the single, correct clip
                audioManager.PlayNarration(clipToPlay);
                Debug.Log($"Playing entry audio for {currentSkybox.displayName} ({GameSettings.CurrentLanguage}) using clip: {clipToPlay.name}");
            }
            else
            {
                // Optional: Stop any previous narration if no new clip is found for this skybox
                // audioManager.StopNarration(); // Add StopNarration method to AudioManager if needed
                Debug.LogWarning($"Skybox '{currentSkybox.displayName}': No localized entry audio clip found to play for language '{GameSettings.CurrentLanguage}'.");
                 // Ensure narration stops if the new skybox has no clip
                 if(audioManager.IsNarrationPlaying()) // Add IsNarrationPlaying method to AudioManager
                 {
                    audioManager.StopNarration(); // Add StopNarration method to AudioManager
                 }
            }
        }
        else
        {
             Debug.LogError("AudioManager reference is missing in SkyboxManager!");
        }
        // ----- MODIFICATION END -----


        // Handle ambient audio for the new skybox
        if (useAmbientAudio && audioManager != null) // Check audioManager again
        {
            AudioClip targetAmbientClip = currentSkybox.ambientAudioClip ?? defaultAmbientAudio;
            float targetAmbientVolume = (currentSkybox.ambientAudioClip != null) ? currentSkybox.ambientVolume : defaultAmbientVolume;
            bool loopTargetAmbient = (currentSkybox.ambientAudioClip != null) ? currentSkybox.loopAmbientAudio : true; // Default usually loops

            // If target is null, stop current ambient
             if (targetAmbientClip == null) {
                 audioManager.StopAmbientAudio(crossfadeAmbientAudio);
             } else {
                 // If target is different from what's currently playing, play the new one
                 if (!audioManager.IsAmbientClipPlaying(targetAmbientClip)) // Requires IsAmbientClipPlaying method in AudioManager
                 {
                     audioManager.PlayAmbientAudio(
                         targetAmbientClip,
                         targetAmbientVolume,
                         loopTargetAmbient,
                         currentSkybox.crossfadeAmbientAudio && crossfadeAmbientAudio // Use skybox setting AND manager setting
                     );
                 }
                 // If it's the same clip, do nothing - let it continue playing.
             }
        }

        // Activate appropriate interactables
        currentSkybox.ActivateInteractables();

        // Fade in screen
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeScreen(1f, 0f, fadeDuration * 0.5f));
        }
         else
        {
            // Simple delay if no fade panel
             yield return new WaitForSeconds(fadeDuration * 0.5f);
        }


        isTransitioning = false;

        // Debug output
        Debug.Log($"Navigated to Skybox {currentSkyboxIndex}: {currentSkybox.displayName ?? currentSkybox.skyboxId}");
    }
    
    /// <summary>
    /// Fades the screen between alpha values.
    /// </summary>
    private IEnumerator FadeScreen(float startAlpha, float endAlpha, float duration)
    {
        if (fadePanel == null)
            yield break;
            
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;
            
            fadePanel.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            
            yield return null;
        }
        
        fadePanel.alpha = endAlpha;
    }

    public void OnResetButtonPressed(SelectEnterEventArgs args)
    {
        // Reset the experience to the starting skybox
        NavigateToSkybox(startingSkyboxIndex);
        audioManager.StopAllAudio();
        
        // Restart default ambient audio if available
        if (useAmbientAudio && defaultAmbientAudio != null)
        {
            audioManager.PlayAmbientAudio(defaultAmbientAudio, defaultAmbientVolume, true, false);
        }
    }
    
    /// <summary>
    /// Handles runtime changes to the skipToSkyboxIndex field.
    /// </summary>
    public void UpdateSkipToSkyboxIndex(int newIndex)
    {
        skipToSkyboxIndex = newIndex;
        if (skipToSkyboxIndex >= 0 && skipToSkyboxIndex < skyboxes.Count)
        {
            NavigateToSkybox(skipToSkyboxIndex);
        }
    }

    /// <summary>
    /// Plays the click sound effect.
    /// </summary>
    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
        }
    }

    /// <summary>
    /// Plays the hover sound effect.
    /// </summary>
    public void PlayHoverSound()
    {
        if (hoverSound != null)
        {
            AudioSource.PlayClipAtPoint(hoverSound, Camera.main.transform.position);
        }
    }

    private void Update()
    {
        #if UNITY_WEBGL || UNITY_EDITOR
        HandleMouseDragCameraControl();
        #endif
        
        // Handle arrow key navigation in all build targets
        if (enableKeyboardNavigation)
        {
            HandleKeyboardNavigation();
        }

    #if UNITY_WEBGL || UNITY_EDITOR
    // Execute WebGL/Mouse specific logic only if the required components are active
    if (webGLCamera != null && webGLCamera.gameObject.activeInHierarchy && EventSystem.current != null)
    {
        // --- ADD THESE LINES IF THEY ARE MISSING ---
        HandleManualHover();            // <<< CALL HOVER LOGIC
        HandleManualClick();            // <<< CALL CLICK LOGIC
        // --- END OF ADDED LINES ---

        // Make sure the drag handling is also called
        HandleMouseDragCameraControl(); // <<< CALL DRAG LOGIC (Likely already here)
    }
    #endif // UNITY_WEBGL || UNITY_EDITOR

    // --- Keyboard Navigation (Platform Independent) ---
    if (enableKeyboardNavigation)
    {
        HandleKeyboardNavigation();
    }

// Inside the Update() method of SkyboxManager.cs
#if UNITY_WEBGL || UNITY_EDITOR
if (webGLCamera != null && webGLCamera.gameObject.activeInHierarchy)
{
    Ray ray = webGLCamera.ScreenPointToRay(Input.mousePosition);
    Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

    // Perform manual raycast on mouse down
    if (Input.GetMouseButtonDown(0))
    {
        // Use the interactionLayer mask your other systems expect
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactionLayer))
        {
            Debug.Log($"Manual Raycast Hit Interactable Layer: {hit.collider.name}");

            // --- Try to manually execute the PointerClick event ---
            GameObject hitObject = hit.collider.gameObject;
            PointerEventData pointerData = new PointerEventData(EventSystem.current); // Get current event system

            // Use ExecuteEvents to correctly trigger IPointerClickHandler or EventTrigger
            ExecuteEvents.Execute(hitObject, pointerData, ExecuteEvents.pointerClickHandler);

            // Optional: Check if the log fired AFTER the ExecuteEvents call
            // You might need a small delay or check next frame if the log doesn't appear immediately
            StartCoroutine(CheckIfLogFired(hitObject.name)); // Example coroutine check


            // --- Alternative: Directly call the known function (less flexible) ---
            // NavigationButtonController navButton = hitObject.GetComponent<NavigationButtonController>();
            // if (navButton != null)
            // {
            //     Debug.Log($"Manually calling TriggerNavigationAction on {hitObject.name}");
            //     navButton.TriggerNavigationAction();
            // }
            // HotspotInteraction hotspot = hitObject.GetComponent<HotspotInteraction>();
            // if (hotspot != null)
            // {
            //     // Add a public method to HotspotInteraction if needed, or use ExecuteEvents
            //      ExecuteEvents.Execute(hitObject, pointerData, ExecuteEvents.pointerClickHandler);
            // }

        }
        // Optional: Add else if needed
        // else { Debug.Log("Manual Raycast Hit Nothing on Interaction Layer."); }
    }
}
#endif

    }

  #if UNITY_WEBGL || UNITY_EDITOR
    /// <summary>
    /// Performs a raycast every frame to detect hovered objects and manually triggers
    /// PointerEnter and PointerExit events via ExecuteEvents on objects with compatible handlers
    /// (e.g., scripts implementing IPointerEnter/ExitHandler like HoverEffect).
    /// </summary>
    private void HandleManualHover()
    {
        // Update pointer data position for the events
        pointerEventData.position = Input.mousePosition;

        Ray ray = webGLCamera.ScreenPointToRay(pointerEventData.position);
        bool hitInteractable = Physics.Raycast(ray, out RaycastHit hit, 100f, interactionLayer); // Use interactionLayer
        GameObject objectUnderPointer = hitInteractable ? hit.collider.gameObject : null;

        // Check if the object under the pointer has changed since last frame
        if (currentlyHoveredObject != objectUnderPointer)
        {
            // --- Trigger PointerExit on the previous object ---
            if (currentlyHoveredObject != null)
            {
                // ExecuteEvents.ExecuteHierarchy sends events up the hierarchy (good for Enter/Exit)
                ExecuteEvents.ExecuteHierarchy(currentlyHoveredObject, pointerEventData, ExecuteEvents.pointerExitHandler);
                
                 Debug.Log($"Manual Pointer Exit: {currentlyHoveredObject.name}"); // Optional
            }

            // --- Trigger PointerEnter on the new object ---
            if (objectUnderPointer != null)
            {
                ExecuteEvents.ExecuteHierarchy(objectUnderPointer, pointerEventData, ExecuteEvents.pointerEnterHandler);
                Debug.Log($"Manual Pointer Enter: {objectUnderPointer.name}"); // Optional
            }

            // Update the tracked hovered object
            currentlyHoveredObject = objectUnderPointer;
        }
    }

    /// <summary>
    /// Handles mouse clicks, targeting the object currently marked as hovered by HandleManualHover,
    /// and manually triggers PointerClick events via ExecuteEvents.
    /// </summary>
    private void HandleManualClick()
    {
        // Only process click if not dragging camera
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            // Use the object identified by HandleManualHover as the click target
            if (currentlyHoveredObject != null)
            {
                // Debug.Log($"Manual Click on Hovered: {currentlyHoveredObject.name}"); // Optional

                // ExecuteEvents.Execute sends event only to the target object
                ExecuteEvents.Execute(currentlyHoveredObject, pointerEventData, ExecuteEvents.pointerClickHandler);
            }
        }
    }

    /// <summary>
    /// Handles camera panning via mouse drag. Prevents dragging if the mouse button
    /// is pressed down while over an interactable object.
    /// </summary>
    private void HandleMouseDragCameraControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Only start dragging if the pointer is NOT over an interactable object
            if (currentlyHoveredObject == null)
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            // If over an interactable, HandleManualClick takes precedence, drag doesn't start.
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Stop dragging when mouse button is released
            isDragging = false;
        }

        if (isDragging)
        {
            // --- Camera Rotation Logic (Keep your working implementation) ---
            Vector2 deltaMousePosition = (Vector2)Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;
            float rotationSpeed = 0.1f;
            float horizontalRotation = -deltaMousePosition.x * rotationSpeed;
            float verticalRotation = deltaMousePosition.y * rotationSpeed;
            webGLCamera.transform.Rotate(0, horizontalRotation, 0, Space.World);
            float currentXRotation = webGLCamera.transform.localEulerAngles.x;
            if (currentXRotation > 180f) currentXRotation -= 360f;
            float newXRotation = currentXRotation + verticalRotation;
            newXRotation = Mathf.Clamp(newXRotation, -80f, 80f);
            webGLCamera.transform.localEulerAngles = new Vector3(newXRotation, webGLCamera.transform.localEulerAngles.y, 0);
            // --- End Camera Rotation Logic ---
        }
    }
    #endif // UNITY_WEBGL || UNITY_EDITOR


    // Add this coroutine to SkyboxManager.cs if using the ExecuteEvents check
private System.Collections.IEnumerator CheckIfLogFired(string objectName)
{
    yield return null; // Wait one frame
    // Check your console manually here to see if the TestPointerClickLog appeared
    // You could add more sophisticated checks if needed
    Debug.Log($"Checked for Event Trigger log on {objectName} one frame after manual ExecuteEvents call.");
}
    
    private void HandleKeyboardNavigation()
    {
        // Only process key presses after cooldown period
        if (Time.time - lastKeyPressTime < keyboardNavigationCooldown)
            return;
            
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NavigateToNextSkybox();
            lastKeyPressTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            NavigateToPreviousSkybox();
            lastKeyPressTime = Time.time;
        }
        
        // Support for directly entering a skybox index using number keys
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (skipToSkyboxIndex >= 0 && skipToSkyboxIndex < skyboxes.Count)
            {
                NavigateToSkybox(skipToSkyboxIndex);
                lastKeyPressTime = Time.time;
            }
        }
    }

   
    
    // Editor keyboard shortcuts
    #if UNITY_EDITOR
    private void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            // Number keys 1-9 to navigate to skyboxes 1-9
            if (Event.current.keyCode >= KeyCode.Alpha1 && Event.current.keyCode <= KeyCode.Alpha9)
            {
                int index = (int)Event.current.keyCode - (int)KeyCode.Alpha1 + 1;
                if (index < skyboxes.Count)
                {
                    NavigateToSkybox(index);
                }
            }
            // 0 key for skybox 0
            else if (Event.current.keyCode == KeyCode.Alpha0)
            {
                NavigateToSkybox(0);
            }
        }
    }
    #endif

    /// <summary>
    /// Public method to set the skip to skybox index through code or inspector.
    /// </summary>
    public void SetSkipToSkyboxIndex(int index)
    {
        skipToSkyboxIndex = index;
    }
}