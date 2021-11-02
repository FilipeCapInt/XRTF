using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSkipToStep : MonoBehaviour
{

    public LessonControllerSwitcher lcs;
    public int StepToSkipTo;
    public bool activate;
    

    // Update is called once per frame
    void Update()
    {
        if (activate)
        {
          
            //lcs.ActiveLesson.SkipToStep(StepToSkipTo);
            activate = false;
        }
    }
}
