using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandWindow : MonoBehaviour
{
    public float maxSize;
    public float xFactor;
    public float yFactor;
    public float positionFactor;
    public float waitTime;

    void Start()
    {
        StartCoroutine(Scale());
    }

    public IEnumerator Scale()
    {
        float timer = 0;
            while (maxSize > transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 0, 0) * Time.deltaTime * xFactor;
                transform.localScale += new Vector3(0, 1, 0) * Time.deltaTime * yFactor;
                transform.position += new Vector3(1 * Time.deltaTime * positionFactor, 0, 0);
                yield return null;
            }
            
    }
}
