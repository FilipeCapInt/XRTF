using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LessonData", menuName = "Create Lesson Data")]
public class LessonData : ScriptableObject
{
    public StepData[] StepInfo = System.Array.Empty<StepData>();
}
