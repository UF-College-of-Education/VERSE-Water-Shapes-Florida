using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoWaypointAnimPlayer : MonoBehaviour
{
    Renderer subject;
    Animation anim;
    bool played = false;
    // Start is called before the first frame update
    void Start()
    {
        subject = GetComponent<Renderer>();
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (subject != null)
        {
            if (subject.isVisible && played == false)
            {
                anim.Play();
                played = true;

            }
            else if (subject.isVisible == false)
            {
                anim.Stop();
                played = false;
            }
        }
    }
}
