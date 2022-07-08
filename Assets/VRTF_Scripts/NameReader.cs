using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameReader : MonoBehaviour
{
    public BaseLessonController LC;

    public void SetTex(string textToSet)
    {
        GetComponent<Text>().text = textToSet;
    }

    
}
