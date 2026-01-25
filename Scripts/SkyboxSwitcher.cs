using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;


public class SkyboxSwitcher : MonoBehaviour
{
     // Variables for WebGL (HTML5) mouse/touch control
    private Vector2 lastMousePosition;
    private bool isDragging = false;

    // Array of skybox materials corresponding to each video
    public Material[] skyboxMaterials;

    // Array of video players for each video
    public VideoPlayer[] videoPlayers;
    

    // UI elements for Element 1
    public GameObject button1; // Button for Element 1 (appears after 5 seconds)


    public GameObject prompt2A;  
    public GameObject buttons3A; 
    public GameObject button3ALeft; 
    public GameObject button3ARight; 
    public GameObject lobbyPrompt; 
    public GameObject buttons4A; 
    public GameObject button4ALeft;
    public GameObject button4ARight;

    public GameObject clerkPrompt; 
    public GameObject buttons5A; 
    public GameObject button5ALeft;
    public GameObject button5ARight; 

    public GameObject enterButterlfyPrompt;
    public GameObject buttons6A; 
    public GameObject button6ALeft; 
    public GameObject button6ARight; 

    public GameObject continueButterflyPrompt; 
    public GameObject buttons8A; 
    public GameObject button8ALeft; 
    public GameObject button8AMiddle; 
    public GameObject button8ARight; 

    public GameObject butterflyPromptB; 
    public GameObject buttons8B; 
    public GameObject button8BLeft; 
    public GameObject button8BMiddle; 
    public GameObject button8BRight; 

    public GameObject butterflyPromptC; 
    public GameObject buttons8C; 
    public GameObject button8CLeft; 
    public GameObject button8CMiddle; 
    public GameObject button8CRight; 

    public GameObject benchPrompt; 
    public GameObject buttons10A; 
    public GameObject button10ALeft; 
    public GameObject button10ARight; 

    
    public GameObject buttons9A; 
    public GameObject button9AMiddle; 

    
    public GameObject buttonsReset; 
    public GameObject buttonReset; 

    public GameObject buttons9B; 
    public GameObject button9BMiddle; 

    public AudioSource wooshSound;

    public AudioSource hoverSound;  // Sound when hovering over a button or prompt
    public AudioSource clickSound;  // Sound when clicking a button or prompt


    // Variable to track current index of skybox and video
    private int currentSkyboxIndex = 0;

    // Variables for skipping videos
    [Header("Debug")]
    public int skipToVideo = -1; // Set this in the Inspector to skip to a specific video by index

    // Variables for XR interaction
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;
    public LayerMask buttonLayer; // Ensure this layer includes the button objects

    public GameObject XROrigin; // XR Rig for VR
    public Camera WebGLCamera;  // Camera for WebGL builds

    void Start()
    {
        

         //OLD XR GRAB INTERACTABLES BUGGY
    // button1.GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnElement1ButtonClicked);
    // prompt2A.GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnPrompt2Clicked);

    // button3ALeft.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On3ALeftButtonClicked);
    // button3ARight.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On3ARightButtonClicked);

    // button4ALeft.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On4ALeftButtonClicked);
    // button4ARight.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On4ARightButtonClicked);

    // button5ALeft.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On5ALeftButtonClicked);
    // button5ARight.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On5ARightButtonClicked);

    // button6ALeft.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On6ALeftButtonClicked);
    // button6ARight.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On6ARightButtonClicked);

    // button8ALeft.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On8ALeftButtonClicked);
    // button8AMiddle.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On8AMiddleButtonClicked);
    // button8ARight.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On8ARightButtonClicked);

    // button8BLeft.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On8BLeftButtonClicked);
    // button8BMiddle.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On8BMiddleButtonClicked);
    // button8BRight.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On8BRightButtonClicked);

    // button8CLeft.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On8CLeftButtonClicked);
    // button8CMiddle.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On8CMiddleButtonClicked);
    // button8CRight.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On8CRightButtonClicked);

    // button9AMiddle.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On9AMiddleButtonClicked);
    // button9BMiddle.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On9BMiddleButtonClicked);

    // button10ALeft.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On10ALeftButtonClicked);
    // button10ARight.GetComponent<XRGrabInteractable>().selectEntered.AddListener(On10ARightButtonClicked);

  
    // buttonReset.GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnRestartButtonClicked);



    //Refactor buttons to use XRSimpleInteractable

  button1.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(OnElement1ButtonClicked);
    prompt2A.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(OnPrompt2Clicked);

     button3ALeft.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On3ALeftButtonClicked);
     button3ARight.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On3ARightButtonClicked);

     button4ALeft.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On4ALeftButtonClicked);
     button4ARight.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On4ARightButtonClicked);

     button5ALeft.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On5ALeftButtonClicked);
     button5ARight.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On5ARightButtonClicked);

     button6ALeft.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On6ALeftButtonClicked);
     button6ARight.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On6ARightButtonClicked);

    button8ALeft.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On8ALeftButtonClicked);
    button8AMiddle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On8AMiddleButtonClicked);
    button8ARight.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On8ARightButtonClicked);

    button8BLeft.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On8BLeftButtonClicked);
    button8BMiddle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On8BMiddleButtonClicked);
    button8BRight.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On8BRightButtonClicked);

    button8CLeft.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On8CLeftButtonClicked);
    button8CMiddle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On8CMiddleButtonClicked);
    button8CRight.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On8CRightButtonClicked);

    button9AMiddle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On9AMiddleButtonClicked);
    button9BMiddle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On9BMiddleButtonClicked);

    button10ALeft.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On10ALeftButtonClicked);
    button10ARight.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(On10ARightButtonClicked);

    // // Restart button interaction
     buttonReset.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(OnRestartButtonClicked);



         #if UNITY_WEBGL
    // Disable hover sound for WebGL
    hoverSound = null;

    // Ensure the cursor is unlocked for WebGL and visible
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    // Disable XR Origin and enable the WebGL camera
    if (XROrigin != null)
    {
        XROrigin.SetActive(false);
    }

    if (WebGLCamera != null)
    {
        WebGLCamera.gameObject.SetActive(true);
    }
    #else
    // Enable XR for other builds
    if (XROrigin != null)
    {
        XROrigin.SetActive(true);
    }

    if (WebGLCamera != null)
    {
        WebGLCamera.gameObject.SetActive(false);
    }
    #endif

        // Initially hide all prompts and buttons
        HideAllUI();

        // Set the first skybox material
        RenderSettings.skybox = skyboxMaterials[currentSkyboxIndex];

        // Play the corresponding video
        videoPlayers[currentSkyboxIndex].Play();

        // Hook into the event for the current video player
        videoPlayers[currentSkyboxIndex].loopPointReached += OnVideoEnd;

        // Only start showing the Element 1 button during the playback of Element 1
        if (currentSkyboxIndex == 1) // Element 1 video
        {
            StartCoroutine(ShowButtonAfterDelay(5f)); // Show button after 5 seconds
            videoPlayers[currentSkyboxIndex].isLooping = true; // Loop Element 1 video
        }

  #if UNITY_WEBGL
        // Unlock the cursor in WebGL (it defaults to locked in full-screen mode)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        #endif
    }

    // Switch to next skybox and video
    public void SwitchSkyboxAndVideo()
    {
        // Unsubscribe from the current video end event
        videoPlayers[currentSkyboxIndex].loopPointReached -= OnVideoEnd;

        // Stop the current video if playing
        if (videoPlayers[currentSkyboxIndex].isPlaying)
        {
            videoPlayers[currentSkyboxIndex].Stop();
        }

        // Increment the index to load the next video and skybox
        currentSkyboxIndex = (currentSkyboxIndex + 1) % skyboxMaterials.Length;

        // Set the new skybox
        RenderSettings.skybox = skyboxMaterials[currentSkyboxIndex];

        // Play the corresponding video
        videoPlayers[currentSkyboxIndex].Play();

        // Hook into the event for the new video player
        videoPlayers[currentSkyboxIndex].loopPointReached += OnVideoEnd;

      //  Debug.Log("Playing video element: " + currentSkyboxIndex + " - Video name: " + videoPlayers[currentSkyboxIndex].clip.name);

       HideAllUI(); // Hide all UI elements at the start of each video
        HandleUIForCurrentVideo();
 
    }

    // This method is called when a video ends
    public void OnVideoEnd(VideoPlayer vp)
    {
        if (currentSkyboxIndex == 3) 
        {
            vp.isLooping = true;
            Debug.Log("Ticket Counter video looping, waiting for button input.");
           buttons3A.SetActive(true);
        }

        else if (currentSkyboxIndex == 4) 
        {
            vp.isLooping = true;
            Debug.Log("Ticket handed over, going to lobby.");
           
        Debug.Log("Going to lobby");
        currentSkyboxIndex = 4; //
        SwitchSkyboxAndVideo(); // Switch to Element 5
        }


 else if (currentSkyboxIndex == 5) 
        {
            vp.isLooping = true;
            Debug.Log("Lobby video, waiting for button input.");
             lobbyPrompt.SetActive(true);
             buttons4A.SetActive(true);
        }

           

     

          else if (currentSkyboxIndex == 8) //butterfly art wall, prompt heading into woosh room
        {
             vp.isLooping = true;
             Debug.Log("ButterflyArt video looping, waiting for button input.");
             enterButterlfyPrompt.SetActive(true);
             buttons6A.SetActive(true);
             
        }

         

        else
        {
            SwitchSkyboxAndVideo();
        }
    }

    // Handle UI visibility logic for each video
    private void HandleUIForCurrentVideo()
    {
        
           if (currentSkyboxIndex == 1) // Element 1
        {
            StartCoroutine(ShowButtonAfterDelay(5f)); // Show Element 1 button after 5 seconds
        }

       else if (currentSkyboxIndex == 2) // Element 2
        {
            StartCoroutine(ShowButtonAfterDelay(5f));
          
        }


    else if (currentSkyboxIndex == 6) // Screens
        {
            StartCoroutine(ShowButtonAfterDelay(5f));
          
        }

    else if (currentSkyboxIndex == 8) // Going into Woosh room
        {
            StartCoroutine(ShowButtonAfterDelay(5f));
          
        }

            else if (currentSkyboxIndex == 9)
        {
                wooshSound.Play();
                Debug.Log("Playing Woosh sound");
        }

           else if (currentSkyboxIndex == 10) // Going into Woosh room
        {
            StartCoroutine(ShowButtonAfterDelay(5f));
          
        }

           else if (currentSkyboxIndex == 11) // Going into Woosh room
        {
            StartCoroutine(ShowButtonAfterDelay(5f));
          
        }

           else if (currentSkyboxIndex == 12) // Going into Woosh room
        {
            StartCoroutine(ShowButtonAfterDelay(5f));
          
        }

           else if (currentSkyboxIndex == 13) // Going into Woosh room
        {
            StartCoroutine(ShowButtonAfterDelay(5f));
          
        }





      else  if (currentSkyboxIndex == 14) // Leaving Butterfly Room
        {
          buttons9A.SetActive(true); 
        }

       else if (currentSkyboxIndex == 15) // Mirror Room
        {
          buttons9B.SetActive(true); 
        }
    }

    // Coroutine to show Element 1 button after delay
    IEnumerator ShowButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentSkyboxIndex == 1) // Only show if still on Element 1
        {
            button1.SetActive(true); // Show Element 1 button
        }

         else if (currentSkyboxIndex == 2) // Only show if still on Element 1
        {
             prompt2A.SetActive(true); // Show the prompt for the third video
        }

 
          else  if (currentSkyboxIndex == 6) 
        {
            
             Debug.Log("ButterflyScreens video looping, waiting for button input.");
             clerkPrompt.SetActive(true);
             buttons5A.SetActive(true);
             
        }

        else  if (currentSkyboxIndex == 8) //butterfly art wall, prompt heading into woosh room
        {
           
             Debug.Log("ButterflyArt video looping, waiting for button input.");
             enterButterlfyPrompt.SetActive(true);
             buttons6A.SetActive(true);
             
        }


          else if (currentSkyboxIndex == 10)  //enter butterfly garden 8A
        {
             //vp.isLooping = true;
             Debug.Log("Enter Butterfly Garden video looping, waiting for button input.");
             continueButterflyPrompt.SetActive(true);
             buttons8A.SetActive(true);
             
        }

            else if (currentSkyboxIndex == 11)  //butterfly garden 8B
        {
            // vp.isLooping = true;
             Debug.Log("Butterfly Garden 8B video looping, waiting for button input.");
             butterflyPromptB.SetActive(true);
             buttons8B.SetActive(true);
             
        }


        else if (currentSkyboxIndex == 12)  //butterfly garden 8C - leave
        {
            // vp.isLooping = true;
             Debug.Log("Butterfly Garden 8C video looping, waiting for button input.");
             butterflyPromptC.SetActive(true);
             buttons8C.SetActive(true);
             
        }

        else if (currentSkyboxIndex == 13)  //bench
        {
           //  vp.isLooping = true;
             Debug.Log("Bench 10A video looping, waiting for button input.");
             benchPrompt.SetActive(true);
             buttons10A.SetActive(true);
             
        }


    }


        // Handle Element 1 Button Click (move to Element 2)
   public void OnRestartButtonClicked(SelectEnterEventArgs args)
{
    Debug.Log("Restart Button Clicked.");
    clickSound.Play();
    
    // Reload the first scene (assuming it's the first in your build order)
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reloads the current scene
}


    // Handle Element 1 Button Click (move to Element 2)
    public void OnElement1ButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("Element 1 button clicked.");
           clickSound.Play();
        currentSkyboxIndex = 1; // Move to Element 2
        button1.SetActive(false);
        SwitchSkyboxAndVideo();
    }


    // Handle Element 1 Button Click (move to Element 2)
    public void OnPrompt2Clicked(SelectEnterEventArgs args)
    {
        Debug.Log("Prompt 2 button clicked.");
           clickSound.Play();
       prompt2A.SetActive(false);
        currentSkyboxIndex = 2; // Move to Element 2
        SwitchSkyboxAndVideo();
    }


    // Handle 3A Left Button Click (move to Element 4)
    public void On3ALeftButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("3A Left button clicked.");
           clickSound.Play();
        buttons3A.SetActive(false);
        currentSkyboxIndex = 3; 
        SwitchSkyboxAndVideo(); // Switch to Element 4
    }

    // Handle 3A Right Button Click (replay Element 3)
    public void On3ARightButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("3A Right button clicked.");
           clickSound.Play();
        currentSkyboxIndex = 3; // Replay Element 3
        videoPlayers[currentSkyboxIndex].Play(); // Replay the current video
        buttons3A.SetActive(false); // Hide the buttons again until video ends
    }


  


     public void On4ALeftButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("4A Left button clicked.");
           clickSound.Play();
        lobbyPrompt.SetActive(false);
        currentSkyboxIndex = 5; 
        SwitchSkyboxAndVideo(); // Switch to Element 4
        
    }

    // Handle 3A Right Button Click (replay Element 3)
    public void On4ARightButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("4A Right button clicked.");
           clickSound.Play();
        lobbyPrompt.SetActive(false);
        currentSkyboxIndex = 0; // Go to outside
       // videoPlayers[currentSkyboxIndex].Play(); // Replay the current video
        buttons4A.SetActive(false); // Hide the buttons again until video ends
        SwitchSkyboxAndVideo();
       
    }

       public void On5ALeftButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("5A Left button clicked.");
           clickSound.Play();
       buttons5A.SetActive(false);
        currentSkyboxIndex = 6; 
        SwitchSkyboxAndVideo(); // Switch to Element 4
        
    }

    // Handle 3A Right Button Click (replay Element 3)
    public void On5ARightButtonClicked(SelectEnterEventArgs args) // go back to lobby
    {
        Debug.Log("5A Right button clicked.");
           clickSound.Play();
        currentSkyboxIndex = 4; // Go to outside
       // videoPlayers[currentSkyboxIndex].Play(); // Replay the current video
        buttons4A.SetActive(false); // Hide the buttons again until video ends
                buttons5A.SetActive(false);
           clerkPrompt.SetActive(false);
        SwitchSkyboxAndVideo();
       
    }
           public void On6ALeftButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("6A Left button clicked.");
           clickSound.Play();
          enterButterlfyPrompt.SetActive(false);
               buttons6A.SetActive(false);
        currentSkyboxIndex = 8; 
        SwitchSkyboxAndVideo(); // go to woosh room
        
    }

    // Handle 3A Right Button Click (replay Element 3)
    public void On6ARightButtonClicked(SelectEnterEventArgs args) // go back to lobby
    {
        Debug.Log("6A Right button clicked.");
           clickSound.Play();
             buttons6A.SetActive(false);
        currentSkyboxIndex = 4; // Go to lobby
       // videoPlayers[currentSkyboxIndex].Play(); // Replay the current video
        buttons4A.SetActive(false); // Hide the buttons again until video ends
        SwitchSkyboxAndVideo();
       
    }

    public void On8ALeftButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("8A Left button clicked."); //continue to butterlfy B
           clickSound.Play();
                buttons8A.SetActive(false);
        currentSkyboxIndex = 10; 
        SwitchSkyboxAndVideo(); // go to video 11 
        
    }

        public void On8AMiddleButtonClicked(SelectEnterEventArgs args) //sit down
    {
        Debug.Log("8A Middle button clicked."); 
           clickSound.Play();
                buttons8A.SetActive(false);
        currentSkyboxIndex = 12; 
        SwitchSkyboxAndVideo(); // go to video 13
        
    }

    
    public void On8ARightButtonClicked(SelectEnterEventArgs args) // exit butterfly
    {
        Debug.Log("8A Right button clicked.");
           clickSound.Play();
                buttons8A.SetActive(false);
        currentSkyboxIndex = 13; // exit butterfly door
       // videoPlayers[currentSkyboxIndex].Play(); // Replay the current video
        buttons8A.SetActive(false); // Hide the buttons again until video ends
        SwitchSkyboxAndVideo();
       
    }

        public void On8BLeftButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("8B Left button clicked."); //continue to butterlfy B
           clickSound.Play();
          buttons8B.SetActive(false);
        currentSkyboxIndex = 11; 
        SwitchSkyboxAndVideo(); // go to video 12
        
    }

        public void On8BMiddleButtonClicked(SelectEnterEventArgs args) //sit down
    {
        Debug.Log("8B Middle button clicked."); 
           clickSound.Play();
           buttons8B.SetActive(false);
        currentSkyboxIndex = 12; 
        SwitchSkyboxAndVideo(); // go to video 13
        
    }

    
    public void On8BRightButtonClicked(SelectEnterEventArgs args) // exit butterfly
    {
        Debug.Log("8B Right button clicked.");
           clickSound.Play();
        buttons8B.SetActive(false);
        currentSkyboxIndex = 13; // exit butterfly door
       // videoPlayers[currentSkyboxIndex].Play(); // Replay the current video
        buttons8A.SetActive(false); // Hide the buttons again until video ends
        SwitchSkyboxAndVideo();
       
    }

        public void On8CLeftButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("8C Left button clicked."); // had fun, leave
           clickSound.Play();
        buttons8C.SetActive(false);
        currentSkyboxIndex = 13; 
        SwitchSkyboxAndVideo(); // go to video 12
        
    }

        public void On8CMiddleButtonClicked(SelectEnterEventArgs args) //sit down
    {
        Debug.Log("8C Middle button clicked."); 
           clickSound.Play();
          buttons8C.SetActive(false);
        currentSkyboxIndex = 12; 
        SwitchSkyboxAndVideo(); // go to video 13
        
    }

        public void On8CRightButtonClicked(SelectEnterEventArgs args) // exit butterfly
    {
        Debug.Log("8C Right button clicked.");
           clickSound.Play();
        currentSkyboxIndex = 10; // had fun, go back to middle
      
        buttons8C.SetActive(false); // Hide the buttons again until video ends
        SwitchSkyboxAndVideo();
       
    }



        public void On9AMiddleButtonClicked(SelectEnterEventArgs args) //sit down
    {
        Debug.Log("9A Middle button clicked."); 
           clickSound.Play();
           buttons9A.SetActive(false);
        currentSkyboxIndex = 14; 
        SwitchSkyboxAndVideo(); // go to video 15
        
    }

    
        public void On9BMiddleButtonClicked(SelectEnterEventArgs args) //sit down
    {
        Debug.Log("9B Middle button clicked."); 
           clickSound.Play();
                buttons9B.SetActive(false);
        currentSkyboxIndex = 15; 
        SwitchSkyboxAndVideo(); // go to video 16
        
    }
    

    
   public void On10ALeftButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("10A Left button clicked."); //  yes, go to garden C
           clickSound.Play();
        buttons10A.SetActive(false);
        currentSkyboxIndex = 11; 
        SwitchSkyboxAndVideo(); //  yes, go to garden C
        
    }
    
    public void On10ARightButtonClicked(SelectEnterEventArgs args) //leave
    {
        Debug.Log("10A Right button clicked.");
           clickSound.Play();
        buttons10A.SetActive(false);
        currentSkyboxIndex = 13; // leave
      
        buttons10A.SetActive(false); // Hide the buttons again until video ends
        SwitchSkyboxAndVideo();
       
    }

private void LockButtonMovement(GameObject button)
{
    var grabInteractable = button.GetComponent<XRGrabInteractable>();

    // Ensure that the object cannot move when grabbed
    grabInteractable.movementType = XRBaseInteractable.MovementType.Instantaneous;

    // Optional: If the button uses a Rigidbody, freeze its position/rotation
    Rigidbody rb = button.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}

//    private void HandleButtonClick(GameObject hitObject)
// {
//                // Check each button and trigger the associated action
//             if (hitObject == button1)
//             {
//                 clickSound.Play();
//                 OnElement1ButtonClicked(); // Trigger Element 1 button action
//                 button1.SetActive(false);
//             }
//             else if (hitObject == buttonReset)
//                         {
//                             clickSound.Play();
//                             OnRestartButtonClicked(); // Restart Game
//                         }

//             else if (hitObject == prompt2A)
//             {
//                 clickSound.Play();
//                 OnPrompt2Clicked(); // Trigger Prompt 2 action
//                 prompt2A.SetActive(false);
//             }
//             else if (hitObject == button3ALeft)
//             {
//                 clickSound.Play();
//                 On3ALeftButtonClicked(); // Trigger 3A Left button action
//                 buttons3A.SetActive(false);
//             }
//             else if (hitObject == button3ARight)
//             {
//                 clickSound.Play();
//                 On3ARightButtonClicked(); // Trigger 3A Right button action
//                 buttons3A.SetActive(false);
//             }
//             else if (hitObject == button4ALeft)
//             {
//                 clickSound.Play();
//                 On4ALeftButtonClicked(); // Trigger 4A Left button action
//                  buttons4A.SetActive(false);
//             }
//             else if (hitObject == button4ARight)
//             {
//                 clickSound.Play();
//                 On4ARightButtonClicked(); // Trigger 4A Right button action
//                     buttons4A.SetActive(false);
//             }
//          else if (hitObject ==  button5ALeft)
//                 {
//                       clickSound.Play();
//                     On5ALeftButtonClicked(); // Trigger 3A Left button action
//                    buttons5A.SetActive(false);
//                 }
//                 else if (hitObject == button5ARight)
//                 {
//                       clickSound.Play();
//                     On5ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hitObject ==  button6ALeft)
//                 {
//                      clickSound.Play();
//                     On6ALeftButtonClicked(); // Trigger 3A Left button action
//                             buttons6A.SetActive(false);
//                 }
//                 else if (hitObject ==  button6ARight)
//                 {
//                       clickSound.Play();
//                     On6ARightButtonClicked(); // Trigger 3A Right button action
//                             buttons6A.SetActive(false);
//                 }
//                 else if (hitObject == button8ALeft)
//                 {
//                       clickSound.Play();
//                     On8ALeftButtonClicked(); // Trigger 3A Left button action
//                             buttons8A.SetActive(false);
//                 }
//                 else if (hitObject ==  button8AMiddle)
//                 {
//                      clickSound.Play();
//                     On8AMiddleButtonClicked(); // Trigger 3A Left button action
//                             buttons8A.SetActive(false);
//                 }
//                 else if (hitObject ==  button8ARight)
//                 {
//                      clickSound.Play();
//                     On8ARightButtonClicked(); // Trigger 3A Right button action
//                                  buttons8B.SetActive(false);
//                 }
//                 else if (hitObject ==  button8BLeft)
//                 {
//                       clickSound.Play();
//                     On8BLeftButtonClicked(); // Trigger 3A Left button action
//                                buttons8B.SetActive(false);
//                 }
//                 else if (hitObject ==  button8BMiddle)
//                 {
//                      clickSound.Play();
//                     On8BMiddleButtonClicked(); // Trigger 3A Left button action
//                             buttons8B.SetActive(false);
//                 }
//                 else if (hitObject == button8BRight)
//                 {
//                      clickSound.Play();
//                     On8BRightButtonClicked(); // Trigger 3A Right button action
//                             buttons8B.SetActive(false);
//                 }
//                 else if (hitObject ==  button8CLeft)
//                 {
//                       clickSound.Play();
//                     On8CLeftButtonClicked(); // Trigger 3A Left button action
//                      buttons8C.SetActive(false);
//                 }
//                 else if (hitObject ==  button8CMiddle)
//                 {
//                       clickSound.Play();
//                     On8CMiddleButtonClicked(); // Trigger 3A Left button action
//                      buttons8C.SetActive(false);
//                 }
//                 else if (hitObject ==  button8CRight)
//                 {
//                       clickSound.Play();
//                     On8CRightButtonClicked(); // Trigger 3A Right button action
//                      buttons8C.SetActive(false);
//                 }
//                    else if (hitObject ==  button9AMiddle)
//                 {
//                       clickSound.Play();
//                     On9AMiddleButtonClicked(); // Trigger 3A Left button action
//                       buttons9A.SetActive(false);
//                 }
//                 else if (hitObject ==  button9BMiddle)
//                 {
//                       clickSound.Play();
//                     On9BMiddleButtonClicked(); // Trigger 3A Left button action
//                       buttons9A.SetActive(false);
//                 }


//                  else if (hitObject ==  button10ALeft)
//                 {
//                       clickSound.Play();
//                     On10ALeftButtonClicked(); // Trigger 3A Left button action
//                           buttons10A.SetActive(false);
//                 }
//                 else if (hitObject == button10ARight)
//                 {
//                      clickSound.Play();
//                     On10ARightButtonClicked(); // Trigger 3A Right button action
//                           buttons10A.SetActive(false);
//                 }
// }
 

    // Handle VR and Mouse Interactions
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        // Detect mouse hover
if (Physics.Raycast(ray, out hit, Mathf.Infinity, buttonLayer))
{
    // Play hover sound
    if (hoverSound != null && hit.collider != null)
    {
        hoverSound.Play();
        Debug.Log("Hovering over: " + hit.collider.gameObject.name);
    }
}

// Detect VR raycast trigger for the left ray interactor
// if (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out hit))
// {
//     // Replace "Fire1" with the input action for the left controller's trigger
//     if (leftRayInteractor.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool leftTriggerPressed) && leftTriggerPressed)
//     {
//         if (hit.collider != null)
//         {
//             // Your interaction logic for the left ray interactor
//             GameObject hitObject = leftRayInteractor.GetOldestInteractableSelected()?.transform.gameObject;
//             if (hitObject != null)
//             {
//                 HandleButtonClick(hitObject); // Centralized method to handle button clicks
//             }
//         }
//     }
// }

// Detect VR raycast trigger for the right ray interactor
// if (rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out hit))
// {
//     // Replace "Fire1" with the input action for the right controller's trigger
//     if (rightRayInteractor.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool rightTriggerPressed) && rightTriggerPressed)
//     {
//         if (hit.collider != null)
//         {
//             // Your interaction logic for the right ray interactor
//             GameObject hitObject = rightRayInteractor.GetOldestInteractableSelected()?.transform.gameObject;
//             if (hitObject != null)
//             {
//                 HandleButtonClick(hitObject); // Centralized method to handle button clicks
//             }
//         }
//     }
// }



        // Detect mouse click
        if (Input.GetMouseButtonDown(0))
        {


//  if (Physics.Raycast(ray, out hit, Mathf.Infinity, buttonLayer))
//     {
//         //WEBGL mouse click
//         GameObject hitObject = hit.collider.gameObject;

//             // Check each button and trigger the associated action
//             if (hitObject == button1)
//             {
//                 clickSound.Play();
//                 OnElement1ButtonClicked(); // Trigger Element 1 button action
//             }

//               else if (hitObject == buttonReset)
//                         {
//                             clickSound.Play();
//                             OnRestartButtonClicked(); // Restart Game
//                         }

//             else if (hitObject == prompt2A)
//             {
//                 clickSound.Play();
//                 OnPrompt2Clicked(); // Trigger Prompt 2 action
//             }
//             else if (hitObject == button3ALeft)
//             {
//                 clickSound.Play();
//                 On3ALeftButtonClicked(); // Trigger 3A Left button action
//             }
//             else if (hitObject == button3ARight)
//             {
//                 clickSound.Play();
//                 On3ARightButtonClicked(); // Trigger 3A Right button action
//             }
//             else if (hitObject == button4ALeft)
//             {
//                 clickSound.Play();
//                 On4ALeftButtonClicked(); // Trigger 4A Left button action
//             }
//             else if (hitObject == button4ARight)
//             {
//                 clickSound.Play();
//                 On4ARightButtonClicked(); // Trigger 4A Right button action
//             }
//          else if (hitObject ==  button5ALeft)
//                 {
//                       clickSound.Play();
//                     On5ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hitObject == button5ARight)
//                 {
//                       clickSound.Play();
//                     On5ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hitObject ==  button6ALeft)
//                 {
//                      clickSound.Play();
//                     On6ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hitObject ==  button6ARight)
//                 {
//                       clickSound.Play();
//                     On6ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hitObject == button8ALeft)
//                 {
//                       clickSound.Play();
//                     On8ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hitObject ==  button8AMiddle)
//                 {
//                      clickSound.Play();
//                     On8AMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hitObject ==  button8ARight)
//                 {
//                      clickSound.Play();
//                     On8ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hitObject ==  button8BLeft)
//                 {
//                       clickSound.Play();
//                     On8BLeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hitObject ==  button8BMiddle)
//                 {
//                      clickSound.Play();
//                     On8BMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hitObject == button8BRight)
//                 {
//                      clickSound.Play();
//                     On8BRightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hitObject ==  button8CLeft)
//                 {
//                       clickSound.Play();
//                     On8CLeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hitObject ==  button8CMiddle)
//                 {
//                       clickSound.Play();
//                     On8CMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hitObject ==  button8CRight)
//                 {
//                       clickSound.Play();
//                     On8CRightButtonClicked(); // Trigger 3A Right button action
//                 }
//                    else if (hitObject ==  button9AMiddle)
//                 {
//                       clickSound.Play();
//                     On9AMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hitObject ==  button9BMiddle)
//                 {
//                       clickSound.Play();
//                     On9BMiddleButtonClicked(); // Trigger 3A Left button action
//                 }


//                  else if (hitObject ==  button10ALeft)
//                 {
//                       clickSound.Play();
//                     On10ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hitObject == button10ARight)
//                 {
//                      clickSound.Play();
//                     On10ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//     }

//             if (Physics.Raycast(ray, out hit, Mathf.Infinity, buttonLayer))
//             {
//                 // Play click sound when any button is clicked
//                 if (clickSound != null)
//                 {
//                     clickSound.Play();
//                     Debug.Log("Button clicked: " + hit.collider.gameObject.name);
//                 }

//                 // Check if hit object is the Element 1 button or 3A buttons
//                else if (hit.collider.gameObject == button1)
//                 {
//                       clickSound.Play();
//                     OnElement1ButtonClicked(); // Trigger Element 1 button action
//                 }

//                   else if (hit.collider.gameObject == buttonReset)
//                         {
//                             clickSound.Play();
//                             OnRestartButtonClicked(); // Restart Game
//                         }

//                 else if (hit.collider.gameObject == prompt2A)
//                 {
//                     clickSound.Play();
//                    OnPrompt2Clicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button3ALeft)
//                 {
//                      clickSound.Play();
//                     On3ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button3ARight)
//                 {
//                       clickSound.Play();
//                     On3ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button4ALeft)
//                 {
//                       clickSound.Play();
//                     On4ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button4ARight)
//                 {
//                      clickSound.Play();
//                     On4ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                  else if (hit.collider.gameObject == button5ALeft)
//                 {
//                       clickSound.Play();
//                     On5ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button5ARight)
//                 {
//                       clickSound.Play();
//                     On5ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button6ALeft)
//                 {
//                      clickSound.Play();
//                     On6ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button6ARight)
//                 {
//                       clickSound.Play();
//                     On6ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button8ALeft)
//                 {
//                       clickSound.Play();
//                     On8ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8AMiddle)
//                 {
//                      clickSound.Play();
//                     On8AMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8ARight)
//                 {
//                      clickSound.Play();
//                     On8ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button8BLeft)
//                 {
//                       clickSound.Play();
//                     On8BLeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8BMiddle)
//                 {
//                      clickSound.Play();
//                     On8BMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8BRight)
//                 {
//                      clickSound.Play();
//                     On8BRightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button8CLeft)
//                 {
//                       clickSound.Play();
//                     On8CLeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8CMiddle)
//                 {
//                       clickSound.Play();
//                     On8CMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8CRight)
//                 {
//                       clickSound.Play();
//                     On8CRightButtonClicked(); // Trigger 3A Right button action
//                 }

//                  else if (hit.collider.gameObject ==  button9AMiddle)
//                 {
//                       clickSound.Play();
//                     On9AMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject ==  button9BMiddle)
//                 {
//                       clickSound.Play();
//                     On9BMiddleButtonClicked(); // Trigger 3A Left button action
//                 }


//                  else if (hit.collider.gameObject == button10ALeft)
//                 {
//                       clickSound.Play();
//                     On10ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button10ARight)
//                 {
//                      clickSound.Play();
//                     On10ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//             }
//         }

//         // Detect VR raycast trigger
//         if (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out hit))
//         {
//             if (Input.GetButtonDown("Fire1") && hit.collider != null)
//             {
//                 if (hit.collider.gameObject == button1)
//                 {
//                       clickSound.Play();
//                     OnElement1ButtonClicked(); // Trigger Element 1 button action
//                 }

//                     else if (hit.collider.gameObject == buttonReset)
//                         {
//                             clickSound.Play();
//                             OnRestartButtonClicked(); // Restart Game
//                         }

//                 else if (hit.collider.gameObject == prompt2A)
//                 {
//                      clickSound.Play();
//                    OnPrompt2Clicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button3ALeft)
//                 {
//                      clickSound.Play();
//                     On3ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button3ARight)
//                 {
//                       clickSound.Play();
//                     On3ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                  else if (hit.collider.gameObject == button4ALeft)
//                 {
//                      clickSound.Play();
//                     On4ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button4ARight)
//                 {
//                       clickSound.Play();
//                     On4ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                  else if (hit.collider.gameObject == button5ALeft)
//                 {
//                       clickSound.Play();
//                     On5ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button5ARight)
//                 {
//                      clickSound.Play();
//                     On5ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button6ALeft)
//                 {
//                       clickSound.Play();
//                     On6ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button6ARight)
//                 {
//                      clickSound.Play();
//                     On6ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button8ALeft)
//                 {
//                       clickSound.Play();
//                     On8ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8AMiddle)
//                 {
//                      clickSound.Play();
//                     On8AMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8ARight)
//                 {
//                       clickSound.Play();
//                     On8ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button8BLeft)
//                 {
//                       clickSound.Play();
//                     On8BLeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8BMiddle)
//                 {
//                      clickSound.Play();
//                     On8BMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8BRight)
//                 {
//                       clickSound.Play();
//                     On8BRightButtonClicked(); // Trigger 3A Right button action
//                 }

//                 //8C
//                 else if (hit.collider.gameObject == button8CLeft)
//                 {
//                      clickSound.Play();
//                     On8CLeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8CMiddle)
//                 {
//                       clickSound.Play();
//                     On8CMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8CRight)
//                 {
//                       clickSound.Play();
//                     On8CRightButtonClicked(); // Trigger 3A Right button action
//                 }
//                        else if (hit.collider.gameObject ==  button9AMiddle)
//                 {
//                       clickSound.Play();
//                     On9AMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject ==  button9BMiddle)
//                 {
//                       clickSound.Play();
//                     On9BMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
                
//                 else if (hit.collider.gameObject == button10ALeft)
//                 {
//                       clickSound.Play();
//                     On10ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button10ARight)
//                 {
//                       clickSound.Play();
//                     On10ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//             }
//         }

//         if (rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out hit))
//         {
//             if (Input.GetButtonDown("Fire1") && hit.collider != null)
//             {
//                 if (hit.collider.gameObject == button1)
//                 {
//                      clickSound.Play();
//                     OnElement1ButtonClicked(); // Trigger Element 1 button action
//                 }

//                     else if (hit.collider.gameObject == buttonReset)
//                         {
//                             clickSound.Play();
//                             OnRestartButtonClicked(); // Restart Game
//                         }

//                 else if (hit.collider.gameObject == prompt2A)
//                 {
//                      clickSound.Play();
//                    OnPrompt2Clicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button3ALeft)
//                 {
//                      clickSound.Play();
//                     On3ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button3ARight)
//                 {
//                      clickSound.Play();
//                     On3ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                  else if (hit.collider.gameObject == button4ALeft)
//                 {
//                       clickSound.Play();
//                     On4ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button4ARight)
//                 {
//                      clickSound.Play();
//                     On4ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                  else if (hit.collider.gameObject == button5ALeft)
//                 {
//                       clickSound.Play();
//                     On5ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button5ARight)
//                 {
//                       clickSound.Play();
//                     On5ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button6ALeft)
//                 {
//                       clickSound.Play();
//                     On6ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button6ARight)
//                 {
//                      clickSound.Play();
//                     On6ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button8ALeft)
//                 {
//                       clickSound.Play();
//                     On8ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8AMiddle)
//                 {
//                       clickSound.Play();
//                     On8AMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8ARight)
//                 {
//                       clickSound.Play();
//                     On8ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//                 else if (hit.collider.gameObject == button8BLeft)
//                 {
//                       clickSound.Play();
//                     On8BLeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8BMiddle)
//                 {
//                       clickSound.Play();
//                     On8BMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8BRight)
//                 {
//                      clickSound.Play();
//                     On8BRightButtonClicked(); // Trigger 3A Right button action
//                 }


//                 else if (hit.collider.gameObject == button8CLeft)
//                 {
//                       clickSound.Play();
//                     On8CLeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8CMiddle)
//                 {
//                       clickSound.Play();
//                     On8CMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button8CRight)
//                 {
//                       clickSound.Play();
//                     On8CRightButtonClicked(); // Trigger 3A Right button action
//                 }

//                        else if (hit.collider.gameObject ==  button9AMiddle)
//                 {
//                       clickSound.Play();
//                     On9AMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject ==  button9BMiddle)
//                 {
//                       clickSound.Play();
//                     On9BMiddleButtonClicked(); // Trigger 3A Left button action
//                 }
                
//                 else if (hit.collider.gameObject == button10ALeft)
//                 {
//                      clickSound.Play();
//                     On10ALeftButtonClicked(); // Trigger 3A Left button action
//                 }
//                 else if (hit.collider.gameObject == button10ARight)
//                 {
//                       clickSound.Play();
//                     On10ARightButtonClicked(); // Trigger 3A Right button action
//                 }
//             }
        }

         #if UNITY_WEBGL
        // Handle Mouse Drag to rotate the camera (for WebGL build only)
        HandleMouseDragCameraControl();
        #endif


    }

       // Camera rotation handler for WebGL builds
    #if UNITY_WEBGL
    private void HandleMouseDragCameraControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 deltaMousePosition = (Vector2)Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            // Rotate camera based on mouse movement
            float rotationSpeed = 0.1f; // Adjust rotation speed
            float horizontalRotation = -deltaMousePosition.x * rotationSpeed;
            float verticalRotation = deltaMousePosition.y * rotationSpeed;

            // Apply horizontal rotation (yaw)
            Camera.main.transform.Rotate(0, horizontalRotation, 0, Space.World);

            // Apply vertical rotation (pitch), clamping to avoid over-rotation
          float currentXRotation = Camera.main.transform.localEulerAngles.x;
if (currentXRotation > 180f) 
{
    currentXRotation -= 360f;
}
float newXRotation = currentXRotation + verticalRotation;
newXRotation = Mathf.Clamp(newXRotation, -80f, 80f); // Clamp to avoid extreme angles
Camera.main.transform.localEulerAngles = new Vector3(newXRotation, Camera.main.transform.localEulerAngles.y, 0);


        }
    }
    #endif



    // Helper method to hide all UI elements
    private void HideAllUI()
    {
        button1.SetActive(false);
        prompt2A.SetActive(false);
        buttons3A.SetActive(false);
        lobbyPrompt.SetActive(false);
        buttons4A.SetActive(false);
        clerkPrompt.SetActive(false);
        buttons5A.SetActive(false);
        enterButterlfyPrompt.SetActive(false);
        buttons6A.SetActive(false);
        continueButterflyPrompt.SetActive(false);
        buttons8A.SetActive(false);
        butterflyPromptB.SetActive(false);
        buttons8B.SetActive(false);
        butterflyPromptC.SetActive(false);
        buttons8C.SetActive(false);
        benchPrompt.SetActive(false);
        buttons10A.SetActive(false);
        buttons9A.SetActive(false);
        buttons9B.SetActive(false);
    }
}
