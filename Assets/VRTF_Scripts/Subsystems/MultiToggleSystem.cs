using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MultiToggleSystem : MonoBehaviour
{
    //Buttons are pressed in order from array
    //[SerializeField] private Button[] ButtonPressOrder = null;
    [SerializeField] private Toggle[] ToggleSet = null;
    [SerializeField] private bool[] CorrectPositions = null;

    //private int NumPressed = 0;
    private int TogglesCorrect = 0;

    public bool Correct => (TogglesCorrect == ToggleSet.Length );
    public bool Activated { get; private set; } = false;

    public UnityEvent BtnPressedEvent = new UnityEvent();
    public UnityEvent FailureEvent = new UnityEvent();

    public bool ResetOnFail = true;

    public BaseLessonController FailLesson = null;
    public LessonControllerSwitcher Switcher = null;

    private void Awake()
    {
        //TODO replace with toggle logic 
        /*
        for (int i = 0; i < ButtonPressOrder.Length; i++)
        {
            int index = i;
            ButtonPressOrder[i].onClick.AddListener(() => ButtonPressed(index));
        }
        */

        for (int i = 0; i < ToggleSet.Length; i++)
        {
            int index = i;
            ToggleSet[i].onValueChanged.AddListener((bool x) => TogglePressed(x,index));
        }

    }

    private void OnDestroy()
    {
        BtnPressedEvent.RemoveAllListeners();
        FailureEvent.RemoveAllListeners();
    }

    public void Activate()
    {
        Activated = true;
    }

    public void Deactivate()
    {
        Activated = false;
    }

    public void ResetSystem()
    {
        //NumPressed = 0;
        Debug.LogWarning("Reset Subsystem");
    }

    private void TogglePressed(bool x, int index)
    {
        if (Activated == false)
        {
            Debug.LogWarning($"{nameof(MultiToggleSystem)} not active!", this);
            return;
        }

        Debug.Log("ButonPressed " + index.ToString());

        if (Correct == true) return;

        for (int i = 0; i < ToggleSet.Length; i++)
        {
            
        }


    }

    private void ButtonPressed(int index)
    {
        if (Activated == false)
        {
            Debug.LogWarning($"{nameof(MultiToggleSystem)} not active!", this);
            return;
        }

        Debug.Log("ButonPressed " + index.ToString());

        if (Correct == true) return;

        //TODO Replace with toggle logic
        /*
        if (index == NumPressed)
        {
            NumPressed++;
        }
        else
        {
            if (ResetOnFail == true)
            {
                ResetSystem();
            }
            else
            {
                FailureEvent.Invoke();

                //Trigger fail condition
                Switcher.SetActiveLessons(new BaseLessonController[] { FailLesson });
                return;
            }
        }
        */

        BtnPressedEvent.Invoke();
    }
}
