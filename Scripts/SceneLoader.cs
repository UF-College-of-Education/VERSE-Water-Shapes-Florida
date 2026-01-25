using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


public class SceneLoader : MonoBehaviour
{
    public Image fadeScreen;

    // Scene name to load after the splash screen
    public string nextSceneName;

    // Delay in seconds before switching scenes
    public float delay = 4f;

    // Start is called before the first frame update
    void OnEnable()
    {
        // Start a coroutine to load the next scene after the delay
        StartCoroutine(LoadSceneAfterDelay());
    }

    // Coroutine to load the next scene after a delay
    private IEnumerator LoadSceneAfterDelay()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);

        // Fade screen to black
        if (fadeScreen != null)
        {
            float alpha = 0f;
            while (alpha < 1f)
            {
               alpha += 3 * Time.deltaTime;
               fadeScreen.color = new Vector4(0, 0, 0, alpha);
               yield return null;
           }
        }

        // Load the next scene
        LoadScene(nextSceneName);
    }

    // Method to load a scene
    public void LoadScene(string sceneName)
    {
        // Check if the sceneName is not empty or null
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
