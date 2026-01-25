using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
// using UnityEngine.EventSystems; // <<< REMOVE or comment out if not needed elsewhere

/// <summary>
/// Handles interactions with navigation buttons to move between skyboxes.
/// Action is triggered by XR selection events OR by an EventTrigger component (for Mouse/Touch).
/// </summary>
// VVV REMOVE IPointerClickHandler INTERFACE VVV
[RequireComponent(typeof(Collider))] // Still need collider for EventTrigger/Raycaster
[RequireComponent(typeof(XRSimpleInteractable))]
public class NavigationButtonController : MonoBehaviour
{
    [SerializeField] private int targetSkyboxIndex;
    [SerializeField] private SkyboxManager skyboxManager;
    [SerializeField] private AudioManager audioManager;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        // Keep existing Awake logic...
        interactable = GetComponent<XRSimpleInteractable>();
        if (skyboxManager == null) skyboxManager = FindObjectOfType<SkyboxManager>();
        if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        // Keep existing Start logic...
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnButtonSelectedXR);
            interactable.hoverEntered.AddListener(OnButtonHoveredXR);
        }
    }

    // --- XR Interaction Event Handlers ---

    private void OnButtonSelectedXR(SelectEnterEventArgs args)
    {
        // Called by XR Interaction Toolkit
        TriggerNavigationAction();
    }

    private void OnButtonHoveredXR(HoverEnterEventArgs args)
    {
        // Called by XR Interaction Toolkit
        TriggerHoverSound();
    }

    // REMOVED: OnPointerClick(PointerEventData eventData) method

    // --- Shared Action & Sound Logic ---

    /// <summary>
    /// Public method containing the core navigation logic.
    /// Can be called by XR events or connected to an EventTrigger's PointerClick event.
    /// </summary>
    public void TriggerNavigationAction() // <<< MUST BE PUBLIC
    {
        // Common logic for both XR select and Mouse click
        if (skyboxManager == null || audioManager == null)
        {
             Debug.LogError($"Missing SkyboxManager or AudioManager reference on {gameObject.name}", this);
             return;
        }

        Debug.Log($"Triggering Navigation Action for {gameObject.name} -> Skybox Index {targetSkyboxIndex}");

        skyboxManager.PlayClickSound();

        // Stop any ongoing narration
        audioManager.StopNarration();

        // Navigate to the target skybox
        skyboxManager.NavigateToSkybox(targetSkyboxIndex);
    }

    private void TriggerHoverSound() // Keep private unless EventTrigger needs it
    {
        if (skyboxManager != null)
        {
            skyboxManager.PlayHoverSound();
        }
    }
public void TestPointerClickLog() {
    Debug.Log($"--- Pointer Click Detected on {gameObject.name} via Event Trigger! ---");
}

    private void OnDestroy()
    {
        // Keep existing OnDestroy logic...
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnButtonSelectedXR);
            interactable.hoverEntered.RemoveListener(OnButtonHoveredXR);
        }
    }
}