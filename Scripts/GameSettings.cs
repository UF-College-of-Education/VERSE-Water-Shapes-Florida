using UnityEngine;
using Lean.Localization; // For default value potentially

public static class GameSettings
{
    // Initialize with a default or load from PlayerPrefs if you want session persistence
    public static string CurrentLanguage { get; set; } = LeanLocalization.GetFirstCurrentLanguage() ?? "English"; // Sensible default
    // Or just: public static string CurrentLanguage { get; set; } = "English";

    // Optional: Load initial value from PlayerPrefs (run once at game start)
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    // private static void InitializeLanguageFromPrefs() {
    //     if (PlayerPrefs.HasKey("SelectedLanguage")) { // Use your own key
    //         CurrentLanguage = PlayerPrefs.GetString("SelectedLanguage");
    //     } else {
    //         CurrentLanguage = "English"; // Default if nothing saved
    //     }
    //     Debug.Log($"Initial Language Set To: {CurrentLanguage}");
    // }
}