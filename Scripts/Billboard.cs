using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    public bool curvedCanvas;
    public Vector3 portraitImageOffset;
    public Vector3 landscapeImageOffset;
    void Start()
    {
        // Cache the main camera reference
        mainCamera = Camera.main;

        // If no main camera is found, log a warning
        if (mainCamera == null)
        {
            Debug.LogWarning("Main Camera not found. Ensure the scene has a Camera tagged as MainCamera.");
        }
    }
    void OnEnable()
    {
        Transform ogParent;
        ogParent = transform.parent;
        transform.SetParent(Camera.main.transform.GetChild(0));
        if (curvedCanvas && GetComponent<Renderer>().material.mainTexture != null)
        {
            if (GetComponent<Renderer>().material.mainTexture.width < GetComponent<Renderer>().material.mainTexture.height)
            {
                transform.localPosition = portraitImageOffset;
            }
            else
            {
                transform.localPosition = landscapeImageOffset;
            }
        }
        else if (GetComponentInChildren<Image>() != null)
        {
            if (GetComponentInChildren<Image>().sprite.texture.width < GetComponentInChildren<Image>().sprite.texture.height)
            {
                transform.localPosition = portraitImageOffset;
            }
            else
            {
                transform.localPosition = landscapeImageOffset;
            }
        }
        else if (TryGetComponent<VideoPlayer>(out VideoPlayer vp))
        {
            transform.localPosition = landscapeImageOffset;
        }
        transform.SetParent(ogParent);
    }
    void Update()
    {
        if (mainCamera != null)
        {
            if (curvedCanvas)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-mainCamera.transform.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Adjust 5f for speed
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(mainCamera.transform.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Adjust 5f for speed
            }
        }
    }
}
