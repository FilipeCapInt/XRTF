using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{ 
    public string timeCounter = "Time: 00:00.00";
    public DataManager dataSave;
    private TimeSpan timePlaying;
    private bool timerGoing = false;

    private float elapsedTime;

    public void BeginTimer()
    {
        timerGoing = true;
        elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public void EndTimer()
    {
        timerGoing = false;
        dataSave.GetComponent<DataManager>().SetTime(timeCounter);
    }

    private IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timePlayingStr = "Time: " + timePlaying.ToString("mm':'ss'.'ff");
            timeCounter = timePlayingStr;
            //Debug.Log(elapsedTime);

            yield return null;
        }
    }
}
