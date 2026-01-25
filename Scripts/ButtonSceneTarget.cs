    using UnityEngine;

    /// <summary>
    /// A simple data component to store the target scene name
    /// for a button, intended to be used by interaction scripts
    /// in WebGL builds where event arguments aren't easily accessible.
    /// </summary>
    public class ButtonSceneTarget : MonoBehaviour
    {
        [Tooltip("The name of the scene this button should load.")]
        public string targetSceneName = "";
    }
    