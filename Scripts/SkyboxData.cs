using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single 360° environment in the museum with its associated interactive elements.
/// </summary>
[System.Serializable]
public class SkyboxData
{
    [Header("Basic Information")]
    public string skyboxId;
    public string displayName;
    public Material skyboxMaterial;
    
    [Header("Navigation")]
    public NavigationButton[] navigationButtons;
    
    [Header("Interactive Elements")]
    public MuseumHotspot[] hotspots;
    public GameObject hotspotParent;
    
    [Header("Audio")]
    public AudioClip entryAudioClip;
    public AudioClip entryAudioClip_Spanish;

     /// <summary>
    /// Gets the appropriate entry audio clip based on the currently selected language
    /// stored in GameSettings.CurrentLanguage.
    /// </summary>
    public AudioClip LocalizedEntryAudioClip
    {
        get
        {
            // Check the statically stored language preference
            // Using OrdinalIgnoreCase for case-insensitive comparison is robust.
            if (!string.IsNullOrEmpty(GameSettings.CurrentLanguage) &&
                GameSettings.CurrentLanguage.Equals("Spanish", System.StringComparison.OrdinalIgnoreCase))
            {
                // If Spanish is selected and the Spanish clip exists, return it.
                if (entryAudioClip_Spanish != null)
                {
                    return entryAudioClip_Spanish;
                }
                else
                {
                    // Log a warning if Spanish is selected but the clip is missing, fall back to default.
                    // Use skyboxId or displayName for better context in the warning.
                    Debug.LogWarning($"Skybox '{displayName} ({skyboxId})': Spanish language selected, but 'entryAudioClip_Spanish' is not assigned. Falling back to default audio.");
                    return entryAudioClip; // Fallback to default
                }
            }
            else
            {
                // Otherwise, return the default (English) clip.
                // Optionally check if the default clip is null too.
                // if (entryAudioClip == null) {
                //     Debug.LogWarning($"Skybox '{displayName} ({skyboxId})': Default 'entryAudioClip' is not assigned.");
                // }
                return entryAudioClip;
            }
        }
    }

    
    [Header("Ambient Audio")]
    public AudioClip ambientAudioClip;
    public float ambientVolume = 0.5f;
    public bool loopAmbientAudio = true;
    public bool crossfadeAmbientAudio = true;
    
    [Header("UI Elements")]
    public GameObject associatedPrompt;
    
    /// <summary>
    /// Activates all navigation buttons and interactive hotspots associated with this skybox.
    /// </summary>
    public void ActivateInteractables()
    {
        if (associatedPrompt != null)
        {
            associatedPrompt.SetActive(true);
        }
        
        if (navigationButtons != null)
        {
            foreach (var button in navigationButtons)
            {
                if (button != null && button.buttonObject != null)
                {
                    button.buttonObject.SetActive(true);
                }
            }
        }
        
        if ((hotspots != null) && (hotspotParent != null))
        {
            hotspotParent.SetActive(true);
            foreach (var hotspot in hotspots)
            {
                if (hotspot != null && hotspot.hotspotObject != null)
                {
                    hotspot.hotspotObject.SetActive(true);
                }
            }
        }
    }
    
    /// <summary>
    /// Deactivates all navigation buttons and interactive hotspots associated with this skybox.
    /// </summary>
    public void DeactivateAllInteractables()
    {
        if (associatedPrompt != null)
        {
            associatedPrompt.SetActive(false);
        }
        
        if (navigationButtons != null)
        {
            foreach (var button in navigationButtons)
            {
                if (button != null && button.buttonObject != null)
                {
                    button.buttonObject.SetActive(false);
                }
            }
        }
        
        if (hotspots != null)
        {
            foreach (var hotspot in hotspots)
            {
                if (hotspot != null && hotspot.hotspotObject != null)
                {
                    hotspot.hotspotObject.SetActive(false);
                    
                    // Also hide any UI elements that might be showing
                    if (hotspot.detailPanel != null)
                    {
                        hotspot.detailPanel.SetActive(false);
                    }
                }
            }
        }
    }
}

/// <summary>
/// Represents a navigation button that transitions to another skybox.
/// </summary>
[System.Serializable]
public class NavigationButton
{
    public string buttonId;
    public GameObject buttonObject;
    public int targetSkyboxIndex;
}