using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessonProgressDataHandler : MonoBehaviour
{
    [SerializeField] private LessonControllerSwitcher LCSwitcher = null;

    public Dictionary<StepData, StepStates> EndedSteps = new Dictionary<StepData, StepStates>(64);

    private void Start()
    {
        LCSwitcher.LessonAddedEvt.RemoveListener(OnLessonAdded);
        LCSwitcher.LessonAddedEvt.AddListener(OnLessonAdded);

        LCSwitcher.LessonRemovedEvt.RemoveListener(OnLessonRemoved);
        LCSwitcher.LessonRemovedEvt.AddListener(OnLessonRemoved);

        SubscribeLessons();
    }

    private void OnDestroy()
    {
        LCSwitcher.LessonAddedEvt.RemoveListener(OnLessonAdded);
        LCSwitcher.LessonRemovedEvt.RemoveListener(OnLessonRemoved);
    }

    private void SubscribeLessons()
    {
        for (int i = 0; i < LCSwitcher.ActiveLessons.Count; i++)
        {

            SubscribeLesson(LCSwitcher.ActiveLessons[i]);
        }
    }

    private void SubscribeLesson(BaseLessonController lesson)
    {
        lesson.LessonStepEnded.RemoveListener(OnStepEnded);
        lesson.LessonStepEnded.AddListener(OnStepEnded);
    }

    private void UnsubscribeLesson(BaseLessonController lesson)
    {
        lesson.LessonStepEnded.RemoveListener(OnStepEnded);
    }

    private void OnStepEnded(StepData stepData, StepStates stepState)
    {
        //Step complete!
        Debug.Log($"Ended Step \"{stepData.StepName}\" with state {stepState}");

        EndedSteps[stepData] = stepState;
    }

    private void OnLessonAdded(BaseLessonController newLesson)
    {
        Debug.Log("yeeeeee");
        SubscribeLesson(newLesson);
    }

    private void OnLessonRemoved(BaseLessonController lesson)
    {
        UnsubscribeLesson(lesson);
    }

    /// <summary>
    /// Returns progress report.
    /// </summary>
    /// <returns></returns>
    public Dictionary<StepData, StepStates> GetProgressReport()
    {
        return new Dictionary<StepData, StepStates>(EndedSteps);
    }
}
