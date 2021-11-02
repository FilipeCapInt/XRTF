using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GroupingStep : BaseStep
{
    [SerializeField] private LessonControllerSwitcher LessonSwitcher = null;
    [SerializeField] private LinearLessonController LessonCntrl = null;

    public override void Started()
    {
        LessonCntrl.LessonComplete.RemoveListener(OnLessonComplete);
        LessonCntrl.LessonComplete.AddListener(OnLessonComplete);

        LessonCntrl.LessonStepEnded.RemoveListener(StepComplete);
        LessonCntrl.LessonStepEnded.AddListener(StepComplete);

        if (LessonCntrl.IsFinished == false)
        {
            LessonCntrl.Deinitialize();
        }

        LessonCntrl.ResetLesson();

        LessonSwitcher.AddActiveLesson(LessonCntrl);

        //LessonCntrl.Initialize();

        //(LessonSwitcher.ActiveLesson as LinearLessonController).CurStepInfo = LessonCntrl.CurStepInfo;
    }

    protected override void DerivedProgress()
    {
        base.DerivedProgress();

        if (LessonCntrl.IsFinished == false)
        {
            LessonCntrl.SkipToStep(LessonCntrl.StepCount);
        }
    }

    protected override void DerivedRegress()
    {
        base.DerivedRegress();
        LessonCntrl.SkipToStep(0);
    }

    private void StepComplete(StepData stepData, StepStates stepState)
    {
        //(LessonSwitcher.ActiveLesson as LinearLessonController).CurStepInfo = stepData;
    }

    public override void Ended()
    {
        base.Ended();

        LessonCntrl.LessonComplete.RemoveListener(OnLessonComplete);
        LessonCntrl.LessonStepEnded.RemoveListener(StepComplete);

        LessonSwitcher.RemoveActiveLesson(LessonCntrl);
    }

    private void OnLessonComplete()
    {
        StepState = StepStates.Correct;
        OnComplete.Invoke();
    }
}
