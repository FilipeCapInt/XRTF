using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Unity.EditorCoroutines.Editor;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Video;
using TriLibCore;
using TriLibCore.Utils;
using TriLibCore.General;

public class XRTF_Editor  : EditorWindow
{
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Firebase variables
    [Header("Main Panel Objects")]
    public GameObject courseCard;
    public GameObject groupObject;
    public GameObject coursePreviewPanel;
    public GameObject lessonPreviewPanel;
    public GameObject lessonSidebarObject;
    public GameObject defaultTablet;
    public GameObject fullScreen;
    public GameObject mediaPanel;
    public GameObject image;
    public GameObject video;
    public GameObject model;
    public GameObject imagecl;
    public GameObject videocl;
    public GameObject modelcl;
    public GameObject textPanel;

    [Header("XRTF Lessons")]
    public GameObject lCSwitcher;
    public GameObject linearLesson;
    public GameObject flexibleLesson;

    [Header("XRTF Lessons")]
    public GameObject steps;
    public GameObject buttonStep;
    public GameObject toggleStep;
    public GameObject dialogueStep;
    public GameObject objectPositionStep;
    public GameObject objectInteractStep;

    private GameObject mainPanel;
    private GameObject ILesson;
    private GameObject mainPanelObject;
    private BaseLessonController lessonType;
    private int lessonCount = 0;

    [SerializeField] public int toolbarInt = 0;
    string[] toolbarStrings = { "Configuration", "Lessons", "Help" };

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/My Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        
        XRTF_Editor window = (XRTF_Editor)EditorWindow.GetWindow(typeof(XRTF_Editor));
        window.Show();
    }

    void OnGUI()
    {
        mainPanel = GameObject.Find("[Main UI]");
        mainPanelObject = GameObject.Find("Course View Panel");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
        //DrawSidebar();
        
        

        EditorGUILayout.EndVertical();toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

        switch (toolbarInt)
        {
            case 0:
                if (GUILayout.Button("Profile", GUILayout.Height(25)))
                {
                    EditorCoroutineUtility.StartCoroutine(LoadProfile(), this);
                }

                if (GUILayout.Button("Previews", GUILayout.Height(25)))
                {
                    EditorCoroutineUtility.StartCoroutine(LoadCourseLessonPreview(), this);
                }

                if (GUILayout.Button("Course Cards", GUILayout.Height(25)))
                {
                    EditorCoroutineUtility.StartCoroutine(CourseCard(), this);
                }

                if (GUILayout.Button("Step Display", GUILayout.Height(25)))
                {
                    EditorCoroutineUtility.StartCoroutine(LoadStepDisplay(), this);
                }

                if (GUILayout.Button("Content Library", GUILayout.Height(25)))
                {
                    EditorCoroutineUtility.StartCoroutine(LoadContentLibrary(), this);
                }

                break;

            case 1:
                SetLessonButtons();
                break;

            case 2:
               // MakeAddressable.SetAddressableGroup(steps, "TestYou");
                break;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSidebar()
    {
        if (GUILayout.Button("App Settings"))
        {
                
        }

        if (GUILayout.Button("App Settings"))
        {

        }


    }

    private void SetLessonAmount()
    {
        EditorCoroutineUtility.StartCoroutine(CourseLessonCount("course"), this);
    }

    private void SetLessonButtons()
    {
        GUILayout.Label("Instantiate Lesson", EditorStyles.boldLabel);
        for (int i = 1; i <= lessonCount; i++)
        {
            Debug.Log("Yes");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Lesson " + i, GUILayout.Height(25)))
            {
                string lessonNumber = "lesson" + i;
                LoadSpecificLesson(lessonNumber);
            }
            GUILayout.EndHorizontal();
        }
    }
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
        SetLessonAmount();
    }

    private IEnumerator DelayInstantiate()
    {
        for (int i = 1; i <= 3; i++)
        {
            string lessonNumber = "lesson" + i;
            EditorCoroutineUtility.StartCoroutine(LoadLesson(lessonNumber), this);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void LoadSpecificLesson(string lesson)
    {
        EditorCoroutineUtility.StartCoroutine(LoadLesson(lesson), this);
    }

    private IEnumerator CourseLessonCount(string course)
    {
        var DBTask = DBreference.Child(course).GetValueAsync();

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
            string count = snapshot.Child("lessoncount").Value.ToString();
            lessonCount = int.Parse(count);
        }
    }

    public IEnumerator CourseCard()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("course").GetValueAsync();
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
            GameObject myLessonCard = (GameObject)Instantiate(courseCard);
            myLessonCard.transform.SetParent(mainPanelObject.transform);
            myLessonCard.transform.localPosition = Vector3.zero;
            myLessonCard.transform.localScale = Vector3.one;

            //Card Title
            myLessonCard.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("coursename").Value.ToString();

            //Card Thumbnail
            RawImage rawImage = myLessonCard.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RawImage>();
            string url = snapshot.Child("coursethumbnail").Value.ToString();
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


            //Card Description
            myLessonCard.transform.GetChild(1).GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("coursedesc").Value.ToString();
        }
    }
    
    private IEnumerator LoadLesson(string lesson)
    {
        var DBTask = DBreference.Child("course").Child(lesson).GetValueAsync();

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
            GameObject myLCSwitcher = (GameObject)Instantiate(lCSwitcher);
            string lessontype = snapshot.Child("type").Value.ToString();

            //Setting the Lesson Controller to Linear
            if (lessontype == "linear")
            {
                ILesson = (GameObject)Instantiate(linearLesson);
                lessonType = ILesson.GetComponent<LinearLessonController>();
                myLCSwitcher.GetComponent<LessonControllerSwitcher>().SetFirstLesson(ILesson.GetComponent<LinearLessonController>());
                ILesson.GetComponent<LinearLessonController>().SetStepAmount(snapshot.Child("stepcount").Value.ToString());
                EditorCoroutineUtility.StartCoroutine(LoadChildrenLinear(lesson, lessonType), this);
            }
            //Setting the Lesson Controller to Flexible
            else if (lessontype == "flexible")
            {
                ILesson = (GameObject)Instantiate(flexibleLesson);
                lessonType = ILesson.GetComponent<FlexibleLessonController>();
                myLCSwitcher.GetComponent<LessonControllerSwitcher>().SetFirstLesson(ILesson.GetComponent<FlexibleLessonController>());
                ILesson.GetComponent<FlexibleLessonController>().SetStepAmount(snapshot.Child("stepcount").Value.ToString());
                EditorCoroutineUtility.StartCoroutine(LoadChildrenFlexible(lesson, lessonType), this);
            }
        }
    }

    private IEnumerator LoadChildrenFlexible(string lesson, BaseLessonController lessonType)
    {
        var DBTask = DBreference.Child("course").Child(lesson).Child("steps").GetValueAsync();

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
            for (int i = 0; i <= stepCount; i++)
            {
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

    private IEnumerator LoadChildrenLinear(string lesson, BaseLessonController lessonType)
    {
        var DBTask = DBreference.Child("course").Child(lesson).Child("steps").GetValueAsync();

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
            for (int i = 0; i <= stepCount; i++)
            {
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

    public IEnumerator LoadProfile()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("profile").GetValueAsync();
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
            GameObject profilePanel = GameObject.Find("ProfileViewPanel");

            //UserName
            profilePanel.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("username").Value.ToString();

            //Profile Photo
            RawImage rawImage = profilePanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<RawImage>();
            string url = snapshot.Child("userprofilephoto").Value.ToString();
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
            
            //Groups
            string groupCount = snapshot.Child("usergroups").Child("groupcount").Value.ToString();
            for(int i = 0; i < int.Parse(groupCount); i++)
            {
                GameObject myGroupObject = (GameObject)Instantiate(groupObject);
                myGroupObject.transform.SetParent(profilePanel.transform.GetChild(2));
                myGroupObject.transform.localPosition = Vector3.zero;
                myGroupObject.transform.localScale = Vector3.one;
                myGroupObject.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("usergroups").Child("group" + i).Value.ToString();
            }
            
        }
    }

    public IEnumerator LoadCourseLessonPreview()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("course").GetValueAsync();
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
            GameObject coursePreviewParent = GameObject.Find("Course Preview Panel");
            GameObject lessonPreviewParent = GameObject.Find("Lesson Preview Panels");
            GameObject lessonSidebarParent = GameObject.Find("Lesson Sidebar Holder");
            GameObject myCoursePreviewPanel = (GameObject)Instantiate(coursePreviewPanel);
            myCoursePreviewPanel.transform.SetParent(coursePreviewParent.transform);
            myCoursePreviewPanel.transform.localPosition = Vector3.zero;
            myCoursePreviewPanel.transform.localScale = new Vector3(2f,2f,2f);

            //Course Title
            myCoursePreviewPanel.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("coursename").Value.ToString();

            //Course Thumbnail
            RawImage rawImage = myCoursePreviewPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<RawImage>();
            string url = snapshot.Child("coursethumbnail").Value.ToString();
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

            //Course Description
            myCoursePreviewPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("coursedesc").Value.ToString();

            //Lesson Preview Instantiation
            string lessonCount = snapshot.Child("lessoncount").Value.ToString();
            for (int i = 1; i <= int.Parse(lessonCount); i++)
            {
                //Data has been retrieved
                GameObject myLessonObject = (GameObject)Instantiate(lessonPreviewPanel);
                myLessonObject.transform.SetParent(lessonPreviewParent.transform);
                myLessonObject.transform.localPosition = Vector3.zero;
                myLessonObject.transform.localScale = new Vector3(2f, 2f, 2f);

                //Course Title
                myLessonObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("coursename").Value.ToString();

                //Lesson Title
                myLessonObject.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("lesson" + i).Child("lessontitle").Value.ToString();

                //Lesson Thumbnail
                RawImage rawImageLesson = myLessonObject.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<RawImage>();
                string urlLesson = snapshot.Child("lesson" + i).Child("lessonthumbnail").Value.ToString();
                UnityWebRequest requestLesson = UnityWebRequestTexture.GetTexture(urlLesson); //Create request
                yield return requestLesson.SendWebRequest(); //Wait for request to complete
                if (requestLesson.isNetworkError || requestLesson.isHttpError)
                {
                    Debug.Log("Finished downloading!");

                    Debug.Log(requestLesson.error);
                }
                else
                {
                    rawImageLesson.texture = ((DownloadHandlerTexture)requestLesson.downloadHandler).texture;
                }

                //Lesson Description
                myLessonObject.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(3).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("lesson" + i).Child("lessondesc").Value.ToString();

                //Lesson Sidebar
                GameObject myLessonSidebar = (GameObject)Instantiate(lessonSidebarObject);
                myLessonSidebar.transform.SetParent(lessonSidebarParent.transform);
                myLessonSidebar.transform.localPosition = Vector3.zero;
                myLessonSidebar.transform.localRotation = Quaternion.identity;
                myLessonSidebar.transform.localScale = new Vector3(1f, 1f, 1f);

                //Lesson Sidebar Title
                myLessonSidebar.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("lesson" + i).Child("lessontitle").Value.ToString();

                //Lesson Sidebar Number
                myLessonSidebar.transform.GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = i.ToString();

                //Lesson Sidebar Time
                myLessonSidebar.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("lesson" + i).Child("lessontime").Value.ToString();
            }


        }
    }

    public IEnumerator LoadStepDisplay()
    {
        //Get the currently logged in user data
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
            GameObject uiDefault = null;
            GameObject uiWorld = null;
            GameObject current = null;
            bool typePanel = true;
            bool typeImage = true;
            bool typeVideo = true;
            bool typeModel = true;
            bool typeUI = true;

            string stepCount = snapshot.Child("stepcount").Value.ToString();
            for (int i = 1; i <= int.Parse(stepCount); i++)
            {
                Debug.Log(i);
                string type;
                string stepDisplay = snapshot.Child("steps").Child("step" + i).Child("stepdisplay").Value.ToString();

                //Instantiate Default Display
                if (stepDisplay == "Default" && uiDefault == null)
                {
                    uiDefault = (GameObject)Instantiate(defaultTablet);
                }

                //Instantiate Full Screen Display
                else if (stepDisplay == "Full Screen" && uiWorld == null)
                {
                    uiWorld = (GameObject)Instantiate(fullScreen);
                }

                //Check Display Type
                if (stepDisplay == "Default")
                {
                    current = uiDefault;
                }

                else if (stepDisplay == "Full Screen")
                {
                    current = uiWorld;
                }

                //Is there an Image?
                try
                {
                    type = snapshot.Child("steps").Child("step" + i).Child("stepimage").Value.ToString();
                }
                catch (Exception ex)
                {
                    typeImage = false;
                    Debug.Log(typeImage);
                }

                //Is there an Video?
                try
                {
                    type = snapshot.Child("steps").Child("step" + i).Child("stepvideo").Value.ToString();
                }
                catch (Exception ex)
                {
                    typeVideo = false;
                    Debug.Log(typeVideo);
                }

                //Is there an Model?
                try
                {
                    type = snapshot.Child("steps").Child("step" + i).Child("stepmodel").Value.ToString();
                }
                catch (Exception ex)
                {
                    typeModel = false;
                    Debug.Log(typeModel);
                }

                //Check Panel Type
                if (typeImage == true || typeVideo == true || typeModel == true)
                {
                    GameObject myTypePanel = (GameObject)Instantiate(mediaPanel);
                    myTypePanel.transform.SetParent(current.transform.GetChild(0));
                    myTypePanel.transform.localPosition = Vector3.zero;
                    myTypePanel.transform.localRotation = Quaternion.identity;
                    myTypePanel.transform.localScale = new Vector3(1f, 1f, 1f);

                    //Step Title
                    myTypePanel.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("steps").Child("step" + i).Child("steptitle").Value.ToString();

                    //Step Description
                    myTypePanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("steps").Child("step" + i).Child("stepdesc").Value.ToString();

                    //Image Download
                    if (typeImage == true && typeVideo == false && typeModel == false)
                    {
                        GameObject myPhotoPanel = (GameObject)Instantiate(image);
                        myPhotoPanel.transform.SetParent(myTypePanel.transform.GetChild(1));
                        myPhotoPanel.transform.localPosition = Vector3.zero;
                        myPhotoPanel.transform.localRotation = Quaternion.identity;
                        myPhotoPanel.transform.localScale = new Vector3(1f, 1f, 1f);
                        RawImage rawImageLesson = myTypePanel.transform.GetChild(1).GetChild(1).gameObject.GetComponent<RawImage>();
                        string url = snapshot.Child("steps").Child("step" + i).Child("stepimage").Value.ToString();
                        UnityWebRequest requestLesson = UnityWebRequestTexture.GetTexture(url); //Create request
                        yield return requestLesson.SendWebRequest(); //Wait for request to complete
                        if (requestLesson.isNetworkError || requestLesson.isHttpError)
                        {
                            Debug.Log("Finished downloading!");

                            Debug.Log(requestLesson.error);
                        }
                        else
                        {
                            rawImageLesson.texture = ((DownloadHandlerTexture)requestLesson.downloadHandler).texture;
                        }
                        typeImage = true;
                        typeVideo = true;
                        typeModel = true;
                    }
                    //Video Download
                    else if (typeImage == false && typeVideo == true && typeModel == false)
                    {
                        GameObject myVideoPanel = (GameObject)Instantiate(video);
                        myVideoPanel.transform.SetParent(myTypePanel.transform.GetChild(1));
                        myVideoPanel.transform.localPosition = Vector3.zero;
                        myVideoPanel.transform.localRotation = Quaternion.identity;
                        myVideoPanel.transform.localScale = new Vector3(1f, 1f, 1f);

                        string url = snapshot.Child("steps").Child("step" + i).Child("stepvideo").Value.ToString();
                        UnityWebRequest _videoRequest = UnityWebRequest.Get(url);

                        yield return _videoRequest.SendWebRequest();

                        if (_videoRequest.isDone == false || _videoRequest.error != null)
                        { Debug.Log("Request = " + _videoRequest.error); }
                        Debug.Log("Video Done - " + _videoRequest.isDone);
                        byte[] _videoBytes = _videoRequest.downloadHandler.data;
                        string _pathToFile = Path.Combine(Application.persistentDataPath, "123.mp4");
                        yield return new EditorWaitForSeconds(4f);
                        File.WriteAllBytes(_pathToFile, _videoBytes);

                        VideoPlayer myVideoPlayer = myVideoPanel.transform.GetChild(0).gameObject.GetComponent<VideoPlayer>();

                        myVideoPlayer.source = UnityEngine.Video.VideoSource.Url;
                        myVideoPlayer.url = _pathToFile;
                        myVideoPlayer.Prepare();

                        while (myVideoPlayer.isPrepared == false)
                        { yield return null; }

                        Debug.Log("Video should play");
                        myVideoPlayer.Play();
                        typeImage = true;
                        typeVideo = true;
                        typeModel = true;
                    }
                    //Model Download
                    else if (typeImage == false && typeVideo == false && typeModel == true)
                    {
                        GameObject myVideoPanel = (GameObject)Instantiate(model);
                        myVideoPanel.transform.SetParent(myTypePanel.transform.GetChild(1));
                        myVideoPanel.transform.localPosition = Vector3.zero;
                        myVideoPanel.transform.localRotation = Quaternion.identity;
                        myVideoPanel.transform.localScale = new Vector3(1f, 1f, 1f);

                        
                        string url = snapshot.Child("steps").Child("step" + i).Child("stepmodel").Value.ToString();UnityWebRequest _videoRequest1 = UnityWebRequest.Get(url);
                        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
                        //var webRequest = AssetDownloader.CreateWebRequest(url);
                        var assetDownloader = new GameObject("Asset Downloader").AddComponent<AssetDownloaderBehaviour>();
                        EditorCoroutineUtility.StartCoroutine(assetDownloader.DownloadAsset(_videoRequest1, OnLoad, OnMaterialsLoad, OnProgress, null, OnError, assetLoaderOptions, null, null, null), this);
                        typeImage = true;
                        typeVideo = true;
                        typeModel = true;
                    }
                }
                //Text Panel
                else
                {
                    //Instantiate initial 
                    GameObject myTypePanel = (GameObject)Instantiate(textPanel);
                    myTypePanel.transform.SetParent(current.transform.GetChild(0));
                    myTypePanel.transform.localPosition = Vector3.zero;
                    myTypePanel.transform.localRotation = Quaternion.identity;
                    myTypePanel.transform.localScale = new Vector3(1f, 1f, 1f);

                    //Step Title
                    myTypePanel.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("steps").Child("step" + i).Child("steptitle").Value.ToString();

                    //Step Description
                    myTypePanel.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("steps").Child("step" + i).Child("stepdesc").Value.ToString();
                    typeImage = true;
                    typeVideo = true;
                    typeModel = true;
                }
            }
        }
    }

    public IEnumerator LoadContentLibrary()
    {
        //Get the currently logged in user data
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
            GameObject uiDefault = null;
            GameObject uiWorld = null;
            GameObject current = null;
            bool typePanel = true;
            bool typeImage = true;
            bool typeVideo = true;
            bool typeModel = true;
            bool typeUI = true;

            string stepCount = snapshot.Child("stepcount").Value.ToString();
            for (int i = 1; i <= int.Parse(stepCount); i++)
            {
                //Is there an Image?
                try
                {
                    snapshot.Child("steps").Child("step" + i).Child("stepimage").Value.ToString();
                }
                catch (Exception ex)
                {
                    typeImage = false;
                    Debug.Log(typeImage);
                }

                //Is there an Video?
                try
                {
                    snapshot.Child("steps").Child("step" + i).Child("stepvideo").Value.ToString();
                }
                catch (Exception ex)
                {
                    typeVideo = false;
                    Debug.Log(typeVideo);
                }

                //Is there an Model?
                try
                {
                    snapshot.Child("steps").Child("step" + i).Child("stepmodel").Value.ToString();
                }
                catch (Exception ex)
                {
                    typeModel = false;
                    Debug.Log(typeModel);
                }
                GameObject contentLibrary = GameObject.Find("Content Library Wrap");
                if (typeImage == true && typeVideo == false && typeModel == false)
                {
                    GameObject myPhotoPanel = (GameObject)Instantiate(imagecl);
                    myPhotoPanel.transform.SetParent(contentLibrary.transform);
                    myPhotoPanel.transform.localPosition = Vector3.zero;
                    myPhotoPanel.transform.localRotation = Quaternion.identity;
                    myPhotoPanel.transform.localScale = new Vector3(1f, 1f, 1f);

                    myPhotoPanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("steps").Child("step" + i).Child("steptitle").Value.ToString();
                    /*RawImage rawImageLesson = myPhotoPanel.transform.GetChild(1).GetChild(1).gameObject.GetComponent<RawImage>();
                    string url = snapshot.Child("steps").Child("step" + i).Child("stepimage").Value.ToString();
                    UnityWebRequest requestLesson = UnityWebRequestTexture.GetTexture(url); //Create request
                    yield return requestLesson.SendWebRequest(); //Wait for request to complete
                    if (requestLesson.isNetworkError || requestLesson.isHttpError)
                    {
                        Debug.Log("Finished downloading!");

                        Debug.Log(requestLesson.error);
                    }
                    else
                    {
                        rawImageLesson.texture = ((DownloadHandlerTexture)requestLesson.downloadHandler).texture;
                    }*/
                    typeImage = true;
                    typeVideo = true;
                    typeModel = true;
                }

                //Video Download
                else if (typeImage == false && typeVideo == true && typeModel == false)
                {
                    GameObject myVideoPanel = (GameObject)Instantiate(videocl);
                    myVideoPanel.transform.SetParent(contentLibrary.transform);
                    myVideoPanel.transform.localPosition = Vector3.zero;
                    myVideoPanel.transform.localRotation = Quaternion.identity;
                    myVideoPanel.transform.localScale = new Vector3(1f, 1f, 1f);

                    myVideoPanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("steps").Child("step" + i).Child("steptitle").Value.ToString();
                    /*string url = snapshot.Child("steps").Child("step" + i).Child("stepvideo").Value.ToString();
                    UnityWebRequest _videoRequest = UnityWebRequest.Get(url);

                    yield return _videoRequest.SendWebRequest();

                    if (_videoRequest.isDone == false || _videoRequest.error != null)
                    { Debug.Log("Request = " + _videoRequest.error); }
                    Debug.Log("Video Done - " + _videoRequest.isDone);
                    byte[] _videoBytes = _videoRequest.downloadHandler.data;
                    string _pathToFile = Path.Combine(Application.persistentDataPath, "123.mp4");
                    yield return new EditorWaitForSeconds(4f);
                    File.WriteAllBytes(_pathToFile, _videoBytes);

                    VideoPlayer myVideoPlayer = myVideoPanel.transform.GetChild(0).gameObject.GetComponent<VideoPlayer>();

                    myVideoPlayer.source = UnityEngine.Video.VideoSource.Url;
                    myVideoPlayer.url = _pathToFile;
                    myVideoPlayer.Prepare();

                    while (myVideoPlayer.isPrepared == false)
                    { yield return null; }

                    Debug.Log("Video should play");
                    myVideoPlayer.Play();
*/
                    typeImage = true;
                    typeVideo = true;
                    typeModel = true;
                }
                //Model Download
                else if (typeImage == false && typeVideo == false && typeModel == true)
                {
                    GameObject myModelPanel = (GameObject)Instantiate(modelcl);
                    myModelPanel.transform.SetParent(contentLibrary.transform);
                    myModelPanel.transform.localPosition = Vector3.zero;
                    myModelPanel.transform.localRotation = Quaternion.identity;
                    myModelPanel.transform.localScale = new Vector3(1f, 1f, 1f);

                    myModelPanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = snapshot.Child("steps").Child("step" + i).Child("steptitle").Value.ToString();
                    /*string url = snapshot.Child("steps").Child("step" + i).Child("stepmodel").Value.ToString(); UnityWebRequest _videoRequest1 = UnityWebRequest.Get(url);
                    var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
                    //var webRequest = AssetDownloader.CreateWebRequest(url);
                    var assetDownloader = new GameObject("Asset Downloader").AddComponent<AssetDownloaderBehaviour>();
                    EditorCoroutineUtility.StartCoroutine(assetDownloader.DownloadAsset(_videoRequest1, OnLoad, OnMaterialsLoad, OnProgress, null, OnError, assetLoaderOptions, null, null, null), this);*/

                    typeImage = true;
                    typeVideo = true;
                    typeModel = true;
                }
            }   
        }    
    }
    private void OnError(IContextualizedError obj)
    {
        Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
    }
    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
        Debug.Log($"Loading Model. Progress: {progress:P}");
    }
    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Materials loaded. Model fully loaded.");
    }
    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model loaded. Loading materials.");
    }
}