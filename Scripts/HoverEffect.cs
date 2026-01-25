using UnityEngine;
using UnityEngine.EventSystems; // REQUIRED for Pointer Interfaces
using UnityEngine.SceneManagement; // Example: If you load scenes directly from here
// using UnityEngine.XR.Interaction.Toolkit; // Keep if needed for XR-specific logic elsewhere

// Add the necessary interface listeners here
public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Visual Settings")]
    [SerializeField] public float hoverScaleFactor = 1.1f;
    [SerializeField] public Color hoverColor = Color.yellow;
    [SerializeField] public float transitionDuration = 0.1f;

    [Header("Action Settings (Example)")]
    [SerializeField] public bool changeSceneOnClick = true; // Control if clicking changes scene
    [SerializeField] public string targetSceneName = ""; // Name of scene to load

    private Vector3 originalScale;
    private Color originalColor;
    private Renderer objectRenderer;
    private Material sharedMaterialInstance; // Use an instance to avoid affecting other objects

    private Coroutine currentTransition;
    private bool isHovering = false; // Track hover state

    void Awake()
    {
        originalScale = transform.localScale;
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null && objectRenderer.material != null)
        {
            // Create an instance of the material FOR THIS OBJECT ONLY
            // This prevents changing the color of all objects sharing the original material asset
            sharedMaterialInstance = objectRenderer.material;
            originalColor = sharedMaterialInstance.color;
        }
        else
        {
            Debug.LogWarning("HoverEffect: Renderer or Material not found on " + gameObject.name, this);
        }
    }

    // --- METHOD 1: Standard Event System Interface Implementation (for Mouse/Raycaster) ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        // This method is called by the Event System when the mouse pointer enters the collider
        Debug.Log($"--- POINTER ENTER Received by: {gameObject.name} (HoverEffect Script) ---"); 

        Debug.Log(gameObject.name + " - Pointer Enter (Mouse Hover Start)");
        isHovering = true;
        TriggerHoverEnterEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // This method is called by the Event System when the mouse pointer exits the collider
        Debug.Log($"--- POINTER EXIT Received by: {gameObject.name} (HoverEffect Script) ---"); 

        Debug.Log(gameObject.name + " - Pointer Exit (Mouse Hover End)");
        isHovering = false;
        TriggerHoverExitEffect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // This method is called by the Event System when the object is clicked
        Debug.Log(gameObject.name + " - Pointer Clicked (Mouse Click)");

        // --- PERFORM CLICK ACTION HERE ---
        if (changeSceneOnClick && !string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log("Loading Scene: " + targetSceneName);
            SceneManager.LoadScene(targetSceneName);
        }
        // Add other actions if needed (e.g., activating a panel)
        // FindObjectOfType<UIManager>()?.ShowPanel("MyDetailPanel");
    }


    // --- METHOD 2: Public Methods for XR Interaction Events (or Event Trigger) ---
    // These can still be used for your VR build by hooking them up to the
    // XR Simple Interactable's Hover Entered/Exited events in the Inspector.

    public void HandleHoverEntered()
    {
        // You might call this from XR Interactable's Hover Enter event
        // Optional: Add a check to prevent double-triggering if using both systems
        // if (isHovering) return; // Avoid re-triggering if already hovering via mouse

        Debug.Log(gameObject.name + " - XR Hover Entered");
        isHovering = true; // Keep track even if triggered by XR
        TriggerHoverEnterEffect();
    }

    public void HandleHoverExited()
    {
        // You might call this from XR Interactable's Hover Exit event
        // Optional: Add a check
        // if (!isHovering) return; // Avoid re-triggering if already exited via mouse

        Debug.Log(gameObject.name + " - XR Hover Exited");
        isHovering = false; // Keep track
        TriggerHoverExitEffect();
    }


    // --- Core Visual Logic ---

    private void TriggerHoverEnterEffect()
    {
        if (objectRenderer == null || sharedMaterialInstance == null) return;

        if (currentTransition != null) StopCoroutine(currentTransition);
        currentTransition = StartCoroutine(TransitionVisuals(originalScale * hoverScaleFactor, hoverColor));
    }

    private void TriggerHoverExitEffect()
    {
         if (objectRenderer == null || sharedMaterialInstance == null) return;

        if (currentTransition != null) StopCoroutine(currentTransition);
        currentTransition = StartCoroutine(TransitionVisuals(originalScale, originalColor));
    }


    private System.Collections.IEnumerator TransitionVisuals(Vector3 targetScale, Color targetColor)
    {
        float elapsedTime = 0f;
        Vector3 startingScale = transform.localScale;
        // Use the instance's color
        Color startingColor = (sharedMaterialInstance != null) ? sharedMaterialInstance.color : Color.white;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / transitionDuration);

            transform.localScale = Vector3.Lerp(startingScale, targetScale, progress);
            if (sharedMaterialInstance != null)
            {
                // Modify the instance color
                sharedMaterialInstance.color = Color.Lerp(startingColor, targetColor, progress);
            }

            yield return null;
        }

        transform.localScale = targetScale;
        if (sharedMaterialInstance != null)
        {
            sharedMaterialInstance.color = targetColor;
        }
        currentTransition = null;
    }

    // Optional: Cleanup the material instance when the object is destroyed
    void OnDestroy()
    {
        if (sharedMaterialInstance != null)
        {
            // Destroy the created material instance to prevent memory leaks
            Destroy(sharedMaterialInstance);
        }
    }
}