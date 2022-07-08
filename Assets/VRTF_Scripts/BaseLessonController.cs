using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseLessonController : MonoBehaviour, ILessonController
{
    [SerializeField] private LessonData lessonInfo = null;

    public LessonData LessonInfo
    {
        get
        {
            if (lessonInfo == null)
                lessonInfo = ScriptableObject.CreateInstance<LessonData>();

            return lessonInfo;
        }
        set => lessonInfo = value;
    }

    public abstract bool IsFinished { get; }

    public abstract void ResetLesson();

    public abstract void Deinitialize();

    public abstract void Initialize();

    public abstract StepData CurStepInfo { get; }

    [SerializeField] private UnityEvent lessonComplete = new UnityEvent();
    public UnityEvent LessonComplete => lessonComplete;

    [SerializeField] private StepDataEvent lessonStepEnded = new StepDataEvent();
    public StepDataEvent LessonStepEnded => lessonStepEnded;
}
