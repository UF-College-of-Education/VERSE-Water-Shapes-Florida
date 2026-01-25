
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using System.Collections;

public class SceneLoaderManager : MonoBehaviour
{
    public static SceneLoaderManager instance;
    public string previousScene; // Tracks the scene that loaded the current scene
    private List<GameObject> currentSceneUI = new List<GameObject>(); // Tracks UI objects in the current scene
    public List<string> SettingsBacklog;
    public InputActionMap IAmap;
    private InputAction Back;

    // Consolidated Scene Loader and scene load manager, as these scripts had essentially the same functionality.
    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Game has multiple Scene Managers!");
            Destroy(this);
        }
        instance = this;
        Back = IAmap.FindAction("Back");
    }
    void OnEnable()
    {
        Back.Enable();
    }
    public void LoadScene(string sceneName)
    {
        // Check if the sceneName is not empty or null
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is empty or null!");
        }
    }
    // Opens the SettingsMenu scene additively and pauses the current game if necessary.
    public void OpenSettings(string setting)
    {
        
        // Record the current active scene as the "previous scene"
        previousScene = SceneManager.GetActiveScene().name;
        //is this a gamemode scene?
    if (IsGameModeScene(SceneManager.GetActiveScene().name))
    {
        // Find and disable all UI elements in the current scene
        TrackAndDisableSceneUI();
    }

        /* Check if the current scene is a game mode and pause the game - RUINS ANIMATIONS
        if (IsGameModeScene())
        {
            Time.timeScale = 0f; // Pause the game
        }*/
        // Add the last scene to Settings Backlog
        SettingsBacklog.Add(previousScene);
        // Load the SettingsMenu scene additively
        SceneManager.LoadSceneAsync(setting, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Closes the SettingsMenu scene and restores the previous scene.
    /// </summary>
    public void CloseSettings()
    {
        if (SettingsBacklog.Count > 0)
        {
            previousScene = SettingsBacklog.Last();
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            // For other scenes, unload the settings scene as before
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(previousScene));
            // Allows android video to be played
#if UNITY_ANDROID
            if (SceneManager.GetActiveScene().name == "HowToPlayMenu")
            {
                StartCoroutine(ReloadHowToPlay());
            }
#endif
            //Removes Scene from Backlog
            SettingsBacklog.Remove(SettingsBacklog.Last());
            if (SettingsBacklog.Count == 0)
            {
                ReenableSceneUI();
            }
        }
        // Check if the previous scene is "2_GameMenu"
        if (previousScene == "2_GameMenu")
        {
            {
                // load the "2_GameMenu" scene in Single mode
                SceneManager.LoadScene("2_GameMenu", LoadSceneMode.Single);
                ReenableSceneUI();
                GameObject.Find("LayoutManager").GetComponent<OrientationLayoutManager>().AdjustMenuLayout();
            }
        }
    }
    IEnumerator ReloadHowToPlay()
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        yield return ao;
        SceneManager.LoadScene("HowToPlayMenu", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("HowToPlayMenu"));
    }
    public void BackAny()
    {
        // Check if the previous scene is "2_GameMenu"
        if (SceneManager.GetActiveScene().name == "2_GameMenu")
        {
            SceneUtility.instance.SceneBacklog.Clear();
        }
        //Check if any game scenes are in the backlog
        else if (SceneUtility.instance.SceneBacklog.Count > 0 && SceneManager.GetActiveScene().name.Contains("_"))
        {
            SceneManager.LoadScene(SceneUtility.instance.SceneBacklog.Last());
            SceneUtility.instance.SceneBacklog.Remove(SceneUtility.instance.SceneBacklog.Last());
        }
        else if (SettingsBacklog.Count > 0)
        {
            CloseSettings();
        }
        else if (SceneUtility.instance.SceneBacklog.Count == 0)
        {
            SceneManager.LoadScene("2_GameMenu");
        }
    }
    /// <summary>
    /// Tracks and disables all UI GameObjects in the current scene.
    /// </summary>
    private void TrackAndDisableSceneUI()
    {
        // Find all GameObjects in the current scene with the "UI" tag
        GameObject[] uiObjects = GameObject.FindGameObjectsWithTag("UI");

        foreach (GameObject uiObject in uiObjects)
        {
            // Add each UI GameObject to the list and disable it
            currentSceneUI.Add(uiObject);
            uiObject.SetActive(false);
        }
    }

    /// <summary>
    /// Re-enables all tracked UI GameObjects.
    /// </summary>
    private void ReenableSceneUI()
    {
        Debug.Log("Running Reeanble!");
        foreach (GameObject uiObject in currentSceneUI)
        {
            if (uiObject != null)
            {
                uiObject.SetActive(true);
            }
        }
        // Clear the list to avoid cross-scene references
        currentSceneUI.Clear();
    }

    /// <summary>
    /// Determines if the current or given scene is a game mode.
    /// </summary>
    /// <param name="sceneName">Optional scene name to check. Defaults to the active scene.</param>
    private bool IsGameModeScene(string sceneName = null)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        // Replace this logic with your game mode scene names
        return sceneName.Contains("_"); // Example: Scene names with "_" are considered game modes
    }
    private void Update()
    {
        if (Back.WasPressedThisFrame())
        {
            BackAny();
        }
           
    }
    void OnDisable()
    {
        if (Back != null)
            Back.Disable();
    }
}
