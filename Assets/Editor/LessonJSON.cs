using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LessonJSON : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Firebase variables
    [Header("MainUI")]
    public Transform mainPanel;
    public GameObject lessonCard;
    public GameObject lCSwitcher;
    public GameObject linearLesson;
    public GameObject flexibleLesson;

    [Header("Steps")]
    public GameObject steps;
    public GameObject buttonStep;
    public GameObject toggleStep;
    public GameObject dialogueStep;
    public GameObject objectPositionStep;
    public GameObject objectInteractStep;

    private GameObject ILesson;
    private BaseLessonController lessonType;
    public void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Set up!");
        EditorCoroutineUtility.StartCoroutine(StartMainUI(), this);
        EditorCoroutineUtility.StartCoroutine(LoadLesson(), this);
    }

    public IEnumerator StartMainUI()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("course").Child("lesson1").GetValueAsync();
        Debug.Log("1");
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        Debug.Log("2");
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.LogWarning(message: $"No Data {DBTask.Exception}");

        }
        else
        {
            Debug.Log("2");
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            GameObject myLessonCard = Instantiate(lessonCard);
            myLessonCard.transform.SetParent(mainPanel);
            myLessonCard.transform.localPosition = Vector3.zero;
            myLessonCard.transform.localScale = Vector3.one;
            myLessonCard.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("title").Value.ToString();
            
            RawImage rawImage = myLessonCard.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RawImage>();
            string url = snapshot.Child("thumbnail").Value.ToString();
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url); //Create request
            yield return request.SendWebRequest(); //Wait for request to complete
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Finished downloading!");

                Debug.Log(request.error);
            }
            else
            {
                rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
        }
    }

    private IEnumerator LoadLesson()
    {
        var DBTask = DBreference.Child("course").Child("lesson1").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.LogWarning(message: $"No Data {DBTask.Exception}");

        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            GameObject myLCSwitcher = Instantiate(lCSwitcher);
            string lessontype = snapshot.Child("type").Value.ToString();

            //Setting the Lesson Controller to Linear
            if (lessontype == "linear")
            {
                ILesson = Instantiate(linearLesson);
                lessonType = ILesson.GetComponent<LinearLessonController>();
                myLCSwitcher.GetComponent<LessonControllerSwitcher>().SetFirstLesson(ILesson.GetComponent<LinearLessonController>());
                ILesson.GetComponent<LinearLessonController>().SetStepAmount(snapshot.Child("stepcount").Value.ToString());
                StartCoroutine(LoadChildrenLinear());
            }
            //Setting the Lesson Controller to Flexible
            else if(lessontype == "flexible")
            {
                ILesson = Instantiate(flexibleLesson);
                lessonType = ILesson.GetComponent<FlexibleLessonController>();
                myLCSwitcher.GetComponent<LessonControllerSwitcher>().SetFirstLesson(ILesson.GetComponent<FlexibleLessonController>());
                ILesson.GetComponent<FlexibleLessonController>().SetStepAmount(snapshot.Child("stepcount").Value.ToString());
                StartCoroutine(LoadChildrenFlexible());
            }
        }
    }

    private IEnumerator LoadChildrenFlexible()
    {
        var DBTask = DBreference.Child("course").Child("lesson1").Child("steps").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.LogWarning(message: $"No Data {DBTask.Exception}");

        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            long stepCount = snapshot.ChildrenCount;
            GameObject stepGO = Instantiate(steps);
            for(int i = 0; i <= stepCount; i++)
            {
                Debug.Log(snapshot.Child("1").Value.ToString());
                if (snapshot.Child(i.ToString()).Value.ToString() == "buttonstep")
                {
                    GameObject step = Instantiate(buttonStep);
                    step.transform.SetParent(stepGO.transform);
                    lessonType.GetComponent<FlexibleLessonController>().SetSteps(step.GetComponent<BaseStep>(), i);
                }

                if (snapshot.Child(i.ToString()).Value.ToString() == "togglestep")
                {
                    GameObject step = Instantiate(toggleStep);
                    step.transform.SetParent(stepGO.transform);
                    lessonType.GetComponent<FlexibleLessonController>().SetSteps(step.GetComponent<BaseStep>(), i);
                }

                if (snapshot.Child(i.ToString()).Value.ToString() == "dialoguestep")
                {
                    GameObject step = Instantiate(dialogueStep);
                    step.transform.SetParent(stepGO.transform);
                    lessonType.GetComponent<FlexibleLessonController>().SetSteps(step.GetComponent<BaseStep>(), i);
                }

                if (snapshot.Child(i.ToString()).Value.ToString() == "objectinteractstep")
                {
                    GameObject step = Instantiate(objectInteractStep);
                    step.transform.SetParent(stepGO.transform);
                    lessonType.GetComponent<FlexibleLessonController>().SetSteps(step.GetComponent<BaseStep>(), i);
                }

                if (snapshot.Child(i.ToString()).Value.ToString() == "objectpositionstep")
                {
                    GameObject step = Instantiate(objectPositionStep);
                    step.transform.SetParent(stepGO.transform);
                    lessonType.GetComponent<FlexibleLessonController>().SetSteps(step.GetComponent<BaseStep>(), i);
                }
            }
        }
    }

    private IEnumerator LoadChildrenLinear()
    {
        var DBTask = DBreference.Child("course").Child("lesson1").Child("steps").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.LogWarning(message: $"No Data {DBTask.Exception}");

        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            long stepCount = snapshot.ChildrenCount;
            GameObject stepGO = Instantiate(steps);
            for (int i = 1; i <= stepCount; i++)
            {
                Debug.Log(snapshot.Child("1").Value.ToString());
                if (snapshot.Child(i.ToString()).Value.ToString() == "buttonstep")
                {
                    GameObject step = Instantiate(buttonStep);
                    step.transform.SetParent(stepGO.transform);
                    lessonType.GetComponent<LinearLessonController>().SetSteps(step.GetComponent<BaseStep>(), i);
                }

                if (snapshot.Child(i.ToString()).Value.ToString() == "togglestep")
                {
                    GameObject step = Instantiate(toggleStep);
                    step.transform.SetParent(stepGO.transform);
                    lessonType.GetComponent<LinearLessonController>().SetSteps(step.GetComponent<BaseStep>(), i);
                }

                if (snapshot.Child(i.ToString()).Value.ToString() == "dialoguestep")
                {
                    GameObject step = Instantiate(dialogueStep);
                    step.transform.SetParent(stepGO.transform);
                    lessonType.GetComponent<LinearLessonController>().SetSteps(step.GetComponent<BaseStep>(), i);
                }

                if (snapshot.Child(i.ToString()).Value.ToString() == "objectinteractstep")
                {
                    GameObject step = Instantiate(objectInteractStep);
                    step.transform.SetParent(stepGO.transform);
                    lessonType.GetComponent<LinearLessonController>().SetSteps(step.GetComponent<BaseStep>(), i);
                }

                if (snapshot.Child(i.ToString()).Value.ToString() == "objectpositionstep")
                {
                    GameObject step = Instantiate(objectPositionStep);
                    step.transform.SetParent(stepGO.transform);
                    lessonType.GetComponent<LinearLessonController>().SetSteps(step.GetComponent<BaseStep>(), i);
                }
            }
        }
    }

    private void BoilerplateXRTF()
    {

    }
}
