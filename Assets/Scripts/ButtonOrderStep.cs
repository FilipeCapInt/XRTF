using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonOrderStep : BaseStep
{
    [SerializeField] private ButtonOrderSystem BtnOrder = null;

    public override void Started()
    {
        BtnOrder.BtnPressedEvent.RemoveListener(Pressed);
        BtnOrder.BtnPressedEvent.AddListener(Pressed);

        BtnOrder.FailureEvent.RemoveListener(OnFailure);
        BtnOrder.FailureEvent.AddListener(OnFailure);

        BtnOrder.Activate();
    }

    public override void Ended()
    {
        base.Ended();

        BtnOrder.BtnPressedEvent.RemoveListener(Pressed);
        BtnOrder.FailureEvent.RemoveListener(OnFailure);
        BtnOrder.Deactivate();
    }

    protected override void DerivedProgress()
    {
        base.DerivedProgress();
    }

    protected override void DerivedRegress()
    {
        base.DerivedRegress();
        BtnOrder.ResetSystem();
    }

    private void Pressed()
    {
        if (BtnOrder.Correct == true)
        {
            StepState = StepStates.Correct;

            OnComplete.Invoke();
        }
        else
        {
            StepState = StepStates.Incomplete;
        }
    }

    private void OnFailure()
    {
        StepState = StepStates.Incorrect;
    }
}
