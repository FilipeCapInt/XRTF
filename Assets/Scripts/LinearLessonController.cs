using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinearLessonController : BaseLessonController
{
    [SerializeField] private BaseStep[] Steps = null;
    [SerializeField] private int CurrentStepIndex = 0;

    //public StepData CurStepInfo = null;

    public int StepCount => Steps.Length;
    public IStep CurrentStep => Steps[CurrentStepIndex];
    public override bool IsFinished => (CurrentStepIndex >= StepCount);

    public override StepData CurStepInfo
    {
        get
        {
            if (CurrentStepIndex < 0 || CurrentStepIndex >= LessonInfo.StepInfo.Length)
            {
                return null;
            }

            return LessonInfo.StepInfo[CurrentStepIndex];
        }
    }

    private void OnDestroy()
    {
        LessonComplete.RemoveAllListeners();
        LessonStepEnded.RemoveAllListeners();
    }

    public override void ResetLesson()
    {
        CurrentStepIndex = 0;
    }

    public override void Deinitialize()
    {
        Debug.Log($"Stopped lesson controller: {name}", this);

        InitStepInfo();

        if (IsFinished == false)
        {
            CurrentStep.OnComplete.RemoveListener(StepComplete);

            CurrentStep.Ended();

            LessonStepEnded.Invoke(LessonInfo.StepInfo[CurrentStepIndex], CurrentStep.StepState);
        }
        else
        {
            Debug.LogError($"Trying to deinitialize LessonController when it's finished! Call {nameof(ResetLesson)} to restart it.", this);
        }
    }

    private void InitStepInfo()
    {
        int prevLength = LessonInfo.StepInfo.Length;

        if (LessonInfo.StepInfo.Length < Steps.Length)
        {
            System.Array.Resize(ref LessonInfo.StepInfo, Steps.Length);
        }

        //Create default data where it doesn't exist
        for (int i = 0; i < LessonInfo.StepInfo.Length; i++)
        {
            if (LessonInfo.StepInfo[i] == null)
            {
                LessonInfo.StepInfo[i] = ScriptableObject.CreateInstance<StepData>();
            }
        }
    }

    public override void Initialize()
    {
        InitStepInfo();

        Debug.Log($"Started lesson controller: {name}", this);

        if (IsFinished == false)
        {
            CurrentStep.OnComplete.RemoveListener(StepComplete);
            CurrentStep.OnComplete.AddListener(StepComplete);

            //CurStepInfo = StepInfo[CurrentStepIndex];

            CurrentStep.Started();
        }
        else
        {
            Debug.LogError($"Trying to initialize LessonController when it's finished! Call {nameof(ResetLesson)} to restart it.", this);
        }
    }

    public void SkipToStep(int stepNum)
    {
        if (IsFinished == true)
        {
            CurrentStepIndex = StepCount - 1;
            //CurStepInfo = StepInfo[CurrentStepIndex];
        }

        int modifier = (CurrentStepIndex < stepNum) ? 1 : -1;

        if (modifier > 0)
        {
            //stepNum is greater
            for (int i = CurrentStepIndex; i < stepNum; i += modifier)
            {
                StepComplete(modifier);
            }
        }
        else
        {
            //stepNum is lower
            for (int i = CurrentStepIndex; i > stepNum; i += modifier)
            {
                StepComplete(modifier);
            }
        }
    }

    private void StepComplete(in int stepCount)
    {
        CurrentStep.OnComplete.RemoveListener(StepComplete);

        //Don't invoke the complete event if the step isn't actually complete
        //if (actualComplete == true)
        //{
        //    LessonStepEnded.Invoke(StepInfo[CurrentStepIndex], Steps[CurrentStepIndex].StepState);
        //}

        if (stepCount > 0)
        {
            CurrentStep.Progress();
        }
        else
        {
            CurrentStep.Regress();
        }

        CurrentStep.Ended();

        LessonStepEnded.Invoke(LessonInfo.StepInfo[CurrentStepIndex], Steps[CurrentStepIndex].StepState);

        Debug.Log($"Step {CurrentStepIndex} complete!", this);

        CurrentStepIndex += stepCount;

        if (IsFinished == false)
        {
            CurrentStep.OnComplete.AddListener(StepComplete);

            //CurStepInfo = StepInfo[CurrentStepIndex];

            CurrentStep.Started();
        }
        else
        {
            LessonComplete.Invoke();
            Debug.Log($"{nameof(LinearLessonController)} \"{name}\" COMPLETE!!", this);
        }
    }

    private void StepComplete()
    {
        StepComplete(1);
    }
}
