using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonSetStep : BaseStep
{
    [SerializeField] private ButtonSetSystem BtnStep = null;

    public override void Started()
    {
        BtnStep.BtnPressedEvent.RemoveListener(Pressed);
        BtnStep.BtnPressedEvent.AddListener(Pressed);

        BtnStep.Activate();
    }

    public override void Ended()
    {
        base.Ended(); 

        BtnStep.BtnPressedEvent.RemoveListener(Pressed);
        BtnStep.Deactivate();
    }

    protected override void DerivedProgress()
    {
        base.DerivedProgress();
    }

    protected override void DerivedRegress()
    {
        base.DerivedRegress();
        BtnStep.ResetSystem();
    }

    private void Pressed()
    {
        if (BtnStep.Correct == true)
        {
            StepState = StepStates.Correct;

            OnComplete.Invoke();
        }
    }
}
