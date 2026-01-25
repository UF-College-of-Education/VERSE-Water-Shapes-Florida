using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Enhanced MouseInteractableHelper with improved debugging and interaction handling
/// </summary>
[RequireComponent(typeof(Collider))]
public class MouseInteractableHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Public settings
    [HideInInspector] public float hoverScaleFactor = 1.1f;
    [HideInInspector] public Color hoverColor = Color.yellow;
    [HideInInspector] public float transitionDuration = 0.1f;
    [HideInInspector] public bool verboseLogging = false;

    // Event for click actions
    public UnityEvent OnClickAction = new UnityEvent();

    // Private fields
    private Vector3 originalScale;
    private Color originalColor;
    private Renderer objectRenderer;
    private Material sharedMaterialInstance;
    private Coroutine currentTransition;
    private bool isInitialized = false;
    private bool isHovering = false;

    // Diagnostic fields
    private int enterCount = 0;
    private int exitCount = 0;
    private int clickCount = 0;
    private float lastClickTime = 0f;

    void Awake()
    {
        // Early initialization in Awake
        TryInitialize();
    }

    void Start()
    {
        // Ensure initialization in Start as well
        Initialize();
        
        // Optional: Debug initial state
        if (verboseLogging)
        {
            Collider col = GetComponent<Collider>();
            Debug.Log($"[{gameObject.name}] MouseInteractableHelper Start - Initialized: {isInitialized}, " +
                      $"Has collider: {col != null}, Collider enabled: {col != null && col.enabled}, " +
                      $"Has renderer: {objectRenderer != null}, OnClickAction count: {GetListenerCount()}");
        }
    }

    /// <summary>
    /// Try to initialize early in Awake if possible
    /// </summary>
    private void TryInitialize()
    {
        if (isInitialized) return;
        
        originalScale = transform.localScale;
        objectRenderer = GetComponent<Renderer>();
        
        if (objectRenderer != null && objectRenderer.sharedMaterial != null)
        {
            // Don't create the material instance yet, just capture the original color
            originalColor = objectRenderer.sharedMaterial.color;
        }
    }

    /// <summary>
    /// Full initialization that creates material instances
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;
        
        // If we haven't set these yet in TryInitialize
        if (originalScale == Vector3.zero)
            originalScale = transform.localScale;
            
        if (objectRenderer == null)
            objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            if (objectRenderer.sharedMaterial != null)
            {
                // Create an instance of the material to avoid affecting other objects
                sharedMaterialInstance = new Material(objectRenderer.sharedMaterial);
                originalColor = sharedMaterialInstance.color;
                objectRenderer.material = sharedMaterialInstance;
                
                if (verboseLogging)
                    Debug.Log($"[{gameObject.name}] Created material instance with color: {originalColor}");
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] No material found on renderer. Hover color effects won't work.", this);
            }
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] No renderer found. Visual effects won't work.", this);
        }
        
        // Ensure we have a collider that's set up correctly
        Collider collider = GetComponent<Collider>();
        if (collider != null && !collider.enabled)
        {
            Debug.LogWarning($"[{gameObject.name}] Collider is disabled. Enabling it for interaction.", this);
            collider.enabled = true;
        }
        
        isInitialized = true;
        
        if (verboseLogging)
            Debug.Log($"[{gameObject.name}] MouseInteractableHelper fully initialized with {GetListenerCount()} listeners");
    }

    /// <summary>
    /// Refresh the helper's state (useful after scene changes or focus events)
    /// </summary>
    public void RefreshState()
    {
        isHovering = false;
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
            currentTransition = null;
        }
        
        // Reset to original appearance
        transform.localScale = originalScale;
        if (objectRenderer != null && sharedMaterialInstance != null)
        {
            sharedMaterialInstance.color = originalColor;
        }
        
        // Re-check component setup
        Collider collider = GetComponent<Collider>();
        if (collider != null && !collider.enabled)
        {
            collider.enabled = true;
        }
        
        if (verboseLogging)
            Debug.Log($"[{gameObject.name}] State refreshed. Hover: {isHovering}, Scale: {transform.localScale}");
    }

    #region Event System Interface Implementation

    public void OnPointerEnter(PointerEventData eventData)
    {
        enterCount++;
        isHovering = true;
        
        if (verboseLogging)
            Debug.Log($"[{gameObject.name}] POINTER ENTER #{enterCount} - ID: {eventData.pointerId}");
        
        TriggerHoverEnterEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        exitCount++;
        isHovering = false;
        
        if (verboseLogging)
            Debug.Log($"[{gameObject.name}] POINTER EXIT #{exitCount} - ID: {eventData.pointerId}");
        
        TriggerHoverExitEffect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickCount++;
        lastClickTime = Time.time;
        
        Debug.Log($"[{gameObject.name}] POINTER CLICK #{clickCount} - Button: {eventData.button}, Listener count: {GetListenerCount()}");
        
        // Invoke the stored action
        if (OnClickAction != null)
        {
            Debug.Log($"[{gameObject.name}] Invoking OnClickAction with {GetListenerCount()} listeners");
            OnClickAction.Invoke();
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] OnClickAction is null", this);
        }
    }

    #endregion

    #region Visual Effects

    private void TriggerHoverEnterEffect()
    {
        if (!isInitialized) Initialize();
        
        if (currentTransition != null)
            StopCoroutine(currentTransition);
            
        currentTransition = StartCoroutine(TransitionVisuals(originalScale * hoverScaleFactor, hoverColor));
    }

    private void TriggerHoverExitEffect()
    {
        if (!isInitialized) Initialize();
        
        if (currentTransition != null)
            StopCoroutine(currentTransition);
            
        currentTransition = StartCoroutine(TransitionVisuals(originalScale, originalColor));
    }

    private IEnumerator TransitionVisuals(Vector3 targetScale, Color targetColor)
    {
        float elapsedTime = 0f;
        Vector3 startingScale = transform.localScale;
        Color startingColor = (sharedMaterialInstance != null) ? sharedMaterialInstance.color : Color.white;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Update scale
            transform.localScale = Vector3.Lerp(startingScale, targetScale, progress);
            
            // Update color if possible
            if (sharedMaterialInstance != null)
            {
                sharedMaterialInstance.color = Color.Lerp(startingColor, targetColor, progress);
            }

            yield return null;
        }

        // Ensure final values are set exactly
        transform.localScale = targetScale;
        if (sharedMaterialInstance != null)
        {
            sharedMaterialInstance.color = targetColor;
        }
        
        currentTransition = null;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Simulates a click programmatically (for testing)
    /// </summary>
    public void SimulateClick()
    {
        if (verboseLogging)
            Debug.Log($"[{gameObject.name}] Simulating click with {GetListenerCount()} listeners");
            
        if (OnClickAction != null)
        {
            OnClickAction.Invoke();
        }
    }
    
    /// <summary>
    /// Gets the listener count using reflection
    /// </summary>
    private int GetListenerCount()
    {
        if (OnClickAction == null) return 0;
        
        // Use reflection to get the listener count
        var field = typeof(UnityEventBase).GetField("m_Calls", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field != null)
        {
            var invokableCallList = field.GetValue(OnClickAction);
            var countField = invokableCallList.GetType().GetField("m_Count", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (countField != null)
            {
                return (int)countField.GetValue(invokableCallList);
            }
        }
        
        // Fallback method
        try {
            var dummyAction = new UnityAction(() => {});
            OnClickAction.AddListener(dummyAction);
            OnClickAction.RemoveListener(dummyAction);
            return 1; // At least one listener can be added
        } catch {
            return 0; // Can't add listeners
        }
    }

    #endregion

    #region Cleanup

    void OnDestroy()
    {
        if (sharedMaterialInstance != null)
        {
            // Safely destroy the material instance
            Destroy(sharedMaterialInstance);
        }
    }

    #endregion
}