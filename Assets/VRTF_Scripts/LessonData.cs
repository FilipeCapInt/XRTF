using UnityEngine;

[CreateAssetMenu(fileName = "LessonData", menuName = "Create Lesson Data")]
[System.Serializable]
public class LessonData : ScriptableObject
{
    public StepData[] StepInfo = System.Array.Empty<StepData>();

    public string lessonName;
}
