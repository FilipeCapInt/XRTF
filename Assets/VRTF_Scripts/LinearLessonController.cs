using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinearLessonController : BaseLessonController
{
    [SerializeField] DataManager data;
    [SerializeField] private bool resetOnFail = false;
    [SerializeField] private bool score = false;

    [SerializeField] private BaseStep[] Steps = null;
    [SerializeField] private int CurrentStepIndex = 0;
    
    private bool off = false;
    private bool initialized = false;
    public int[] collections;

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

    public void SetStepAmount(string stepLength)
    {
        Steps = new BaseStep[int.Parse(stepLength)];
    }
    public void SetSteps(BaseStep step, int position)
    {
        Steps[position] = step;
    }

    public override void ResetLesson()
    {
        for (int i = 0; i < Steps.Length; i++)
        {
            int closureIndex = i; // Prevents the closure problem
            Steps[closureIndex].OnTriggered.RemoveAllListeners();
        }

        Deinitialize();
        OnDestroy();
        off = false;
        CurrentStepIndex = 0;
        Initialize();
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
            CheckTriggered();
            CurrentStep.Started();
        }
        else
        {
            Debug.LogError($"Trying to initialize LessonController when it's finished! Call {nameof(ResetLesson)} to restart it.", this);
        }

        InitializeData();
    }

    //Inistialized when the lesson is active within the Lesson Controller Switcher
    public void InitializeData()
    {
       if (initialized == false)
        {
            data.LessonDataStart(LessonInfo.lessonName);
            initialized = true;
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
        //   LessonStepEnded.Invoke(StepInfo[CurrentStepIndex], Steps[CurrentStepIndex].StepState);
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
    public void CheckTriggered()
    {
        SetDataCollection();
        collections = new int[Steps.Length];
        for (int i = 0; i < Steps.Length; i++)
        {
            int closureIndex = i; // Prevents the closure problem
            Steps[closureIndex].OnTriggered.RemoveListener(delegate { CheckStep(closureIndex); });
            Steps[closureIndex].OnTriggered.AddListener(delegate { CheckStep(closureIndex); });
            Steps[closureIndex].Triggered();
        }
    }
    
    public void SetDataCollection()
    {
        
    }
    //Checks if Current Step Index is equal to the Step Triggered
    private int type = 0;
    public void CheckStep(int step)
    {
        if (resetOnFail == true && off == false)
        {
            type = 1;
            if (Steps[CurrentStepIndex] == Steps[0])
            {
                for (int i = 0; i < Steps.Length; i++)
                {
                    collections[i] = 2;
                }
            }
        }

        if(score == true)
        {
            type = 2;
        }

        switch (type)
        {
            case 0:
                if (Steps[CurrentStepIndex] == Steps[step])
                {
                    data.SetAmountIncorrect(collections);

                    //Lesson Complete
                    if (CurrentStepIndex == Steps.Length - 1)
                    {
                        data.LessonDataEnd();
                    }
                }
                else
                {
                    collections[step] = collections[step] + 1;
                    data.SetAmountIncorrect(collections);
                }
                break;

            case 1:
                off = true;
                if (Steps[CurrentStepIndex] == Steps[step])
                {
                    collections[step] = 0;

                    //Lesson Complete
                    if (CurrentStepIndex == Steps.Length - 1)
                    {
                        data.SetAmountResetC(collections, this.LessonInfo.lessonName);
                    }
                }
                else
                {
                    collections[step] = 1;
                    data.SetAmountReset(collections, this.LessonInfo.lessonName);
                    ResetLesson();
                }
                break;

            case 2:
                if (Steps[CurrentStepIndex] == Steps[step])
                {
                    if(collections[step] != 1)
                    {
                        collections[CurrentStepIndex] = 0;
                    }

                    //Lesson Complete
                    if (CurrentStepIndex == Steps.Length - 1)
                    {
                        data.SetAmountResetC(collections, this.LessonInfo.lessonName);
                    }
                }
                else
                {
                    collections[CurrentStepIndex] = 1;
                }
                break;
        }
    }

    private void CheckForMistake()
    {

    }

    private void StepComplete()
    {
        StepComplete(1);
    }
}
