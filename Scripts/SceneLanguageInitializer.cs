using UnityEngine;
using Lean.Localization;

public class SceneLanguageInitializer : MonoBehaviour
{
    void Start()
    {
        // Apply the language stored in the static variable
        if (!string.IsNullOrEmpty(GameSettings.CurrentLanguage))
        {
            LeanLocalization leanLoc = GetComponent<LeanLocalization>(); // Assumes attached to same object
            if (leanLoc == null) {
               leanLoc = FindObjectOfType<LeanLocalization>(); // Fallback search
            }


            if (leanLoc != null)
            {
                if (leanLoc.CurrentLanguage != GameSettings.CurrentLanguage)
                {
                     Debug.Log($"SceneInitializer applying language from GameSettings: {GameSettings.CurrentLanguage}");
                     leanLoc.SetCurrentLanguage(GameSettings.CurrentLanguage);
                }
            }
            else
            {
                 Debug.LogError("SceneLanguageInitializer could not find LeanLocalization component in the scene!", this.gameObject);
            }
        }
    }
}