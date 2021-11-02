using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonStep : BaseStep
{
    [SerializeField] private Button Btn = null;

    public override void Started()
    {
        Btn.onClick.RemoveListener(DoneListener);
        Btn.onClick.AddListener(DoneListener);
    }

    public override void Ended()
    {
        base.Ended();

        Btn.onClick.RemoveListener(DoneListener);
    }

    private void DoneListener()
    {
        Debug.Log("Pressed button!");
        StepState = StepStates.Correct;

        OnComplete.Invoke();
    }

    protected override void DerivedProgress()
    {
        base.DerivedProgress();
    }

    protected override void DerivedRegress()
    {
        base.DerivedRegress();
    }
}
