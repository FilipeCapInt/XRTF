using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonOrderSystem : MonoBehaviour
{
    //Buttons are pressed in order from array
    [SerializeField] private Button[] ButtonPressOrder = null;

    //0 - A
    //1 - C
    //2 - B

    private int NumPressed = 0;

    public bool Correct => (NumPressed == ButtonPressOrder.Length);
    public bool Activated { get; private set; } = false;

    public UnityEvent BtnPressedEvent = new UnityEvent();
    public UnityEvent FailureEvent = new UnityEvent();

    public bool ResetOnFail = true;

    public BaseLessonController FailLesson = null;
    public LessonControllerSwitcher Switcher = null;

    private void Awake()
    {
        for (int i = 0; i < ButtonPressOrder.Length; i++)
        {
            int index = i;
            ButtonPressOrder[i].onClick.AddListener(() => ButtonPressed(index));
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
        NumPressed = 0;
        Debug.LogWarning("Reset Subsystem");
    }

    private void ButtonPressed(int index)
    {
        if (Activated == false)
        {
            Debug.LogWarning($"{nameof(ButtonOrderSystem)} not active!", this);
            return;
        }

        Debug.Log("ButonPressed " + index.ToString());

        if (Correct == true) return;

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

        BtnPressedEvent.Invoke();
    }
}
