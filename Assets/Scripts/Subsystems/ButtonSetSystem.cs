using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonSetSystem : MonoBehaviour
{
    //Buttons are pressed in order from array
    [SerializeField] private Button[] TrackedButtons = null;
    [SerializeField] private int ButtonsToActivate = 0;

    private int NumPressed = 0;
    private bool[] PressedButtons;

    public bool Correct => (NumPressed == ButtonsToActivate);
    public bool Activated { get; private set; } = false;

    public UnityEvent BtnPressedEvent = new UnityEvent(); 

    private void Awake()
    {
        for (int i = 0; i < TrackedButtons.Length; i++)
        {
            int index = i;
            TrackedButtons[i].onClick.AddListener(() => ButtonPressed(index));
        }

        PressedButtons = new bool[TrackedButtons.Length];
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
            Debug.LogWarning($"{nameof(ButtonSetSystem)} not active!", this);
            return;
        }

        Debug.Log("ButonPressed " + index.ToString());

        if (Correct == true) return;

        if (PressedButtons[index] == false)
        {
            NumPressed++;
            PressedButtons[index] = true;
        }

       

        BtnPressedEvent.Invoke();
    }
}
