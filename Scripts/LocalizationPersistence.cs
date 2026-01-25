using UnityEngine;
using Lean.Localization; // Required to reference LeanLocalization

/// <summary>
/// Ensures the GameObject this is attached to persists across scenes
/// and provides easy access to the LeanLocalization component on it.
/// Assumes LeanLocalization is attached to the same GameObject.
/// </summary>
[RequireComponent(typeof(LeanLocalization))] // Ensures LeanLocalization is present
public class LocalizationPersistence : MonoBehaviour
{
    // Singleton instance
    public static LocalizationPersistence Instance { get; private set; }

    // Public reference to the LeanLocalization component for easy access
    public LeanLocalization LeanLocalizationInstance { get; private set; }

    void Awake()
    {
        // --- Singleton Pattern Implementation ---
        if (Instance != null && Instance != this)
        {
            // If another instance exists, destroy this new one to enforce singleton.
            Debug.LogWarning("Duplicate LocalizationPersistence found. Destroying the new one.");
            Destroy(this.gameObject);
            return; // Stop execution for this instance
        }
        // This is the first instance, make it the singleton.
        Instance = this;
        // --- End Singleton ---

        // Make this GameObject persist across scene loads
        DontDestroyOnLoad(this.gameObject);

        // Get the LeanLocalization component attached to this same GameObject
        LeanLocalizationInstance = GetComponent<LeanLocalization>();

        if (LeanLocalizationInstance == null)
        {
            // This shouldn't happen due to [RequireComponent], but good practice.
            Debug.LogError("Critical Error: LeanLocalization component not found on the LocalizationManager GameObject!");
        }
        else
        {
            // Optional: You could trigger an initial update or check here if needed,
            // but LeanLocalization's own OnEnable/Update should handle its setup.
            Debug.Log("Localization Persistence Initialized.");
            // LeanLocalization.UpdateTranslations(); // Usually not needed here, Lean handles it.
        }
    }

    // Optional: Add helper methods if needed, though direct access via
    // LocalizationPersistence.Instance.LeanLocalizationInstance is often sufficient.
    // public void SetGameLanguage(string languageName)
    // {
    //     if (LeanLocalizationInstance != null)
    //     {
    //         LeanLocalizationInstance.SetCurrentLanguage(languageName);
    //     }
    // }
}