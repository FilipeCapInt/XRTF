using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public LessonDataCollection lessonData = new LessonDataCollection();
    private LessonDataSave lessonDataSave = new LessonDataSave();
    public TimerController timer;

    private string elapsedTime;
    private string lessonName;
    private float percentCorrect;
    private string filename = "Attempt";
    private string[] amountIncorrect;

    // Controlled currently through the LessonControllerSwitcher
    public void LessonDataStart(string lesson)
    {
        lessonName = lesson;
        timer.BeginTimer();
    }
    // Saves Data in a json file in LessonDataSave class
    public void LessonDataSave()
    {
        
        lessonData.lessonName = lessonName;
        lessonDataSave.SaveLesson(lessonData);
    }

    // Calls to TimerController to stop. Controller in the onComplete function for the Lesson Controller
    public void LessonDataEnd()
    {
        timer.EndTimer();
    }

    // Controlled in the TimerController class
    public void SetTime(string time)
    {
        elapsedTime = time;
        lessonData.elapsedTime = elapsedTime;
        LessonDataSave();
    }

    public void SetAmountIncorrect(int[] array)
    {
        string[] amountIncorrect = new string[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            amountIncorrect[i] = "Step " + i + ": " + array[i].ToString() + " Times pressed incorrectly";
        }
        lessonData.amountIncorrect = amountIncorrect;
    }

    //Assigns Incorrect
    public void SetAmountReset(int[] array, string lessonName)
    {
        amountIncorrect = new string[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            amountIncorrect[i] = "Step " + i + ": " + array[i].ToString();
        }
        lessonData.percentCorrect = 0;
        lessonData.attempt = lessonData.attempt + 1;
        lessonData.amountIncorrect = amountIncorrect;
        LessonDataEnd();
        LessonDataStart(lessonName);
    }

    //Assign when Lesson Completed
    public void SetAmountResetC(int[] array, string lessonName)
    {
        amountIncorrect = new string[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == 0)
            {
                percentCorrect++;
            }
            amountIncorrect[i] = "Step " + i + ": " + array[i].ToString();
        }
        percentCorrect = ((percentCorrect / array.Length) * 100);
        lessonData.percentCorrect = Mathf.Round(percentCorrect * 1);
        lessonData.attempt = lessonData.attempt + 1;
        lessonData.amountIncorrect = amountIncorrect;
        LessonDataEnd();
    }
}
