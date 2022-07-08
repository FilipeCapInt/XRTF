using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ILessonController
{
    bool IsFinished { get; }

    void ResetLesson();

    void Deinitialize();

    void Initialize();

    LessonData LessonInfo { get; }

    StepData CurStepInfo { get; }

    UnityEvent LessonComplete { get; }

    StepDataEvent LessonStepEnded { get; }
}

[System.Serializable]
public class StepDataEvent : UnityEvent<StepData, StepStates>
{

}