using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class ReadFile : MonoBehaviour
{
    public string json;
    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://54ae-2806-2f0-9041-e193-994f-a2d-8c0b-d215.ngrok.io/api/course/62bcca5792ea317d3c3e8661"))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                json = www.downloadHandler.text;
                Debug.Log(json);
                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;
            }
        }
    }
}
