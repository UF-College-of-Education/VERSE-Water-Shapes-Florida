using UnityEngine;
using Lean.Localization;

public class LanguageButtonHandler : MonoBehaviour
{
    public void SetLanguage(string languageName)
    {
        // Set the static variable
        GameSettings.CurrentLanguage = languageName;
        Debug.Log($"GameSettings.CurrentLanguage set to: {languageName}");

        // Also update the current scene's localization immediately
        LeanLocalization localLeanLoc = FindObjectOfType<LeanLocalization>();
        if (localLeanLoc != null)
        {
            localLeanLoc.SetCurrentLanguage(languageName);
        }

        // Optional: Save to PlayerPrefs if you want session persistence too
        // PlayerPrefs.SetString("SelectedLanguage", languageName);
        // PlayerPrefs.Save();
    }
}