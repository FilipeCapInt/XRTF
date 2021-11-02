using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayCurrentStepInfo : MonoBehaviour
{
    [SerializeField] private LessonControllerSwitcher Switcher = null;

    [SerializeField] private TextMeshProUGUI TextObj = null;

    private void Update()
    {
        TextObj.enabled = false;

        if (Input.GetKey(KeyCode.LeftShift) == false)
        {
            return;
        }

        if (Switcher.ActiveLessons.Count == 0)
        {
            return;
        }

        BaseLessonController firstLesson = Switcher.ActiveLessons[0];
        StepData stepData = firstLesson.CurStepInfo;

        TextObj.enabled = true;

        if (stepData == null)
        {
            TextObj.text = "Lesson completed!";
            return;
        }

        TextObj.text = $"Current Step: \"{stepData.StepName}\"";
    }
}
