using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LessonControllerSwitcher : MonoBehaviour
{
    public List<BaseLessonController> ActiveLessons { get; private set; } = new List<BaseLessonController>(8);

    [SerializeField] private BaseLessonController FirstLesson = null;

    [SerializeField] private OnLessonAdded LessonAddedEvent = new OnLessonAdded();
    public OnLessonAdded LessonAddedEvt => LessonAddedEvent;

    [SerializeField] private OnLessonRemoved LessonRemovedEvent = new OnLessonRemoved();
    public OnLessonRemoved LessonRemovedEvt => LessonRemovedEvent;

    private void Awake()
    {
        SetActiveLessons(new BaseLessonController[] { FirstLesson });
    }

    private void OnDestroy()
    {
        LessonAddedEvent.RemoveAllListeners();
        LessonRemovedEvent.RemoveAllListeners();
    }

    public void AddActiveLesson(BaseLessonController newLesson)
    {
        ActiveLessons.Add(newLesson);

        newLesson.Initialize();

        LessonAddedEvent.Invoke(newLesson);
    }

    public void RemoveActiveLesson(BaseLessonController lesson)
    {
        bool removed = ActiveLessons.Remove(lesson);

        if (removed == true)
        {
            if (lesson != null)
            {
                lesson.Deinitialize();
            }

            LessonRemovedEvent.Invoke(lesson);
        }
    }

    public void SetActiveLessons(IList<BaseLessonController> lessons)
    {
        for (int i = ActiveLessons.Count - 1; i >= 0; i--)
        {
            RemoveActiveLesson(ActiveLessons[i]);
        }

        for (int i = 0; i < lessons.Count; i++)
        {
            AddActiveLesson(lessons[i]);
        }
    }

    [System.Serializable]
    public class OnLessonSwitched : UnityEvent<BaseLessonController, BaseLessonController>
    {

    }

    [System.Serializable]
    public class OnLessonAdded : UnityEvent<BaseLessonController>
    {

    }

    [System.Serializable]
    public class OnLessonRemoved : UnityEvent<BaseLessonController>
    {

    }
}
