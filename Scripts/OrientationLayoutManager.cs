using System.Collections;
using UnityEngine;

public class OrientationLayoutManager : MonoBehaviour
{
    public GameObject gameMenuLandscape; // Reference to the landscape menu
    public GameObject gameMenuPortrait;  // Reference to the portrait menu
    ScreenOrientation orientation;
    void Start()
    {
        AdjustMenuLayout();
    }
    void Update()
    {
        if (Screen.orientation != orientation)
        {
            AdjustMenuLayout();
            orientation = Screen.orientation;
        }
            
    }
    public void AdjustMenuLayout()
    {
        // Detect if the screen is in portrait or landscape
        if (Screen.orientation == ScreenOrientation.LandscapeRight || Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            SetLandscapeLayout();
        }
        else if (Screen.orientation == ScreenOrientation.Portrait)
        {
            SetPortraitLayout();
        }
    }
    void SetLandscapeLayout()
    {
        gameMenuLandscape.SetActive(true);
        gameMenuPortrait.SetActive(false);
        Debug.Log("Switched to Landscape Layout");
    }
    void SetPortraitLayout()
    {
        gameMenuLandscape.SetActive(false);
        gameMenuPortrait.SetActive(true);
        Debug.Log("Switched to Portrait Layout");
    }
    public void ForceUpdateOrientation()
    {
        Debug.Log("Orientation Forced");
        gameMenuLandscape.SetActive(false);
        gameMenuPortrait.SetActive(false);
        AdjustMenuLayout();
    }
}
