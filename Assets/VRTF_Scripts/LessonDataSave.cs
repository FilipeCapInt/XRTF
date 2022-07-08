using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LessonDataSave : MonoBehaviour
{
    public LessonData lessonData;
    public string filename = "content.json";
    string myObject = "";
    public void SaveLessonArray()
    {
        for (int i = 0; i < lessonData.StepInfo.Length; i++)
        {
            string content = JsonUtility.ToJson(lessonData.StepInfo[i], true);
            myObject = myObject + content;
            var path = System.IO.Path.Combine(Application.persistentDataPath, "contents.json");
            File.WriteAllText(path, myObject);
        }
    }

    public void SaveLesson(LessonDataCollection contents)
    {
         string content = JsonUtility.ToJson(contents, true);
         myObject = myObject + content;
         var path = System.IO.Path.Combine(Application.persistentDataPath, "content.json");
         File.WriteAllText(path, myObject);
    }
}
