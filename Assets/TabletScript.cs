using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletScript : MonoBehaviour
{
    public GameObject[] array = null;
    public int counter = -1;

    
    public void ChangeUI()
    {
        counter++;
        if(counter > array.Length - 1)
        {
            counter = 0;
            array[array.Length - 1].SetActive(false);
        }
        if(counter - 1 != -1)
        {
            array[counter - 1].SetActive(false);
        }

        array[counter].SetActive(true);
    }
}
