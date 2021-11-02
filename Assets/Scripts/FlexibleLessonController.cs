using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlexibleLessonController : BaseLessonController
{
    [SerializeField] private BaseStep[] Steps = null;

    private bool[] CompletedBools = null;

    public int StepCount => Steps.Length;
    public override bool IsFinished => (StepsCompleted >= StepCount);

    public int StepsCompleted { get; private set; } = 0;

    private List<UnityAction> CompleteDelegates = new List<UnityAction>();

    public override StepData CurStepInfo
    {
        get
        {
            for (int i = 0; i < CompletedBools.Length; i++)
            {
                if (CompletedBools[i] == false)
                {
                    return LessonInfo.StepInfo[i];
                }
            }

            return null;
        }
    }

    private void OnDestroy()
    {
        LessonComplete.RemoveAllListeners();
    }

    public override void ResetLesson()
    {
        StepsCompleted = 0;
    }

    public override void Deinitialize()
    {
        Debug.Log($"Stopped lesson controller: {name}", this);

        InitStepInfo();

        CompletedBools = new bool[StepCount];

        if (IsFinished == false)
        {
            for (int i = 0; i < StepCount; i++)
            {
                if (CompleteDelegates.Count != 0)
                {
                    Steps[i].OnComplete.RemoveListener(CompleteDelegates[i]);
                }

                Steps[i].Ended();

                LessonStepEnded.Invoke(LessonInfo.StepInfo[i], Steps[i].StepState);
            }

            CompleteDelegates.Clear();
        }
        else
        {
            Debug.LogError($"Trying to deinitialize LessonController when it's finished! Call {nameof(ResetLesson)} to restart it.");
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
            for (int i = 0; i < StepCount; i++)
            {
                int index = i;

                Debug.Log("ADDING AT INDEX: " + index);

                UnityAction completeDel = () => StepComplete(index);
                CompleteDelegates.Add(completeDel);

                Steps[i].OnComplete.RemoveListener(completeDel);
                Steps[i].OnComplete.AddListener(completeDel);

                Steps[i].Started();
            }  
        }
        else
        {
            Debug.LogError($"Trying to initialize LessonController when it's finished! Call {nameof(ResetLesson)} to restart it.");
        }
    }

    private void StepComplete(int stepIndex)
    {
        StepsCompleted++;

        CompletedBools[stepIndex] = true;

        Debug.Log("COMPLETED INDEX: " + stepIndex);

        UnityAction action = CompleteDelegates[stepIndex];

        Steps[stepIndex].OnComplete.RemoveListener(action);

        Steps[stepIndex].Ended();

        LessonStepEnded.Invoke(LessonInfo.StepInfo[stepIndex], Steps[stepIndex].StepState);

        Debug.Log($"Step completed! Steps complete: {StepsCompleted} / {StepCount}", this);

        if (IsFinished == true)
        {
            LessonComplete.Invoke();
            Debug.Log($"{nameof(FlexibleLessonController)} \"{name}\" COMPLETE!!", this);
        }
    }
}
