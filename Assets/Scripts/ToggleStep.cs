using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleStep : BaseStep
{
    [SerializeField] private Toggle Tg = null;

    public bool CorrectBool;
    
    public override void Started()
    {
        base.Started();

        Tg.onValueChanged.RemoveListener(DoneListener);
        Tg.onValueChanged.AddListener(DoneListener);
    }

    public override void Ended()
    {
        base.Ended();

        Tg.onValueChanged.RemoveListener(DoneListener);
    }

    private void DoneListener(bool tog)
    {
        Debug.Log("Toggled button", this);
        if (tog == CorrectBool)
        {
            StepState = StepStates.Correct;

            OnComplete.Invoke();
        }
        
    }

    protected override void DerivedProgress()
    {
        base.DerivedProgress();
        Tg.SetIsOnWithoutNotify(CorrectBool);
    }

    protected override void DerivedRegress()
    {
        base.DerivedRegress();
        Tg.SetIsOnWithoutNotify(!CorrectBool);
    }
}
