using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class JsonManager : MonoBehaviour
{

    private UnityWebRequest _webRequest;
    [SerializeField] private List<QuizDataScriptable> quiz;

    [SerializeField] private QuizGameUI qUI;
    
    //GDocResponse result = new GDocResponse();

    private GDocResponse[] data;

    private string JsonTxt = null;
    private string JsonLink = "https://script.google.com/macros/s/AKfycbyqSyn7He9t5tg9Tzd1Ps_Q6i_IoF6VIy0RxDNOI0jEvDf_F1oRLw4zxjwS9I3Zfb8/exec";
    
    public static JsonManager instance; //Needed as part of the functionality in Awake, so there can only be one instance.  
    
    
    public static JsonManager GetInstance()
    {
        return instance;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        QuizGameUI qUI = QuizGameUI.GetInstance();
        qUI.loaded = true;
    }
    
    void Awake()
    {
        //Check if there is already an instance of JsonManager.
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance.
            Destroy(gameObject);

        //Set this to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        //Also so that the json isn't fetched every time the player restarts the quiz.
        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        //quiz = (QuizDataScriptable)Target;
        
        qUI = QuizGameUI.GetInstance(); //to use its functions.

        StartCoroutine(GetJson(JsonLink));





    }
    
    void Update () {
        SceneManager.sceneLoaded += OnSceneLoaded;
        

    }
    /*
    public void Update()
    {
        _webRequest = UnityWebRequest.Get("https://script.google.com/macros/s/AKfycbyqSyn7He9t5tg9Tzd1Ps_Q6i_IoF6VIy0RxDNOI0jEvDf_F1oRLw4zxjwS9I3Zfb8/exec");
        _webRequest.SendWebRequest();

        if (_webRequest != null && _webRequest.isDone)
            CheckForImportRequestEnd(); // the only way to wait for a process to finish is with this

    }
    */

    [System.Serializable]
    public class GDocResponse // this class is used to parse the JSON
    {
        public string question; //{ get; set; }
        public string answer; //{ get; set; }
        public List<string> arr; //{ get; set; }
        //[SerializeField] public List<QuizDataScriptable> jsonData;
    }

   // public static GDocResponse CreateFromJSON(string jsonString)
   //{
   //     return JsonHelper.FromJson<GDocResponse>(jsonString);
    //}

    private void CheckForImportRequestEnd()
    {
        //if (_webRequest != null && _webRequest.isDone)
        //{

        //GDocResponse[] data = JsonHelper.FromJson<GDocResponse>(JsonTxt);
        data = JsonHelper.FromJson<GDocResponse>(JsonTxt);



        //var result = JsonUtility.FromJson<GDocResponse>(JsonTxt);
        //QuizDataScriptable myTarget = (QuizDataScriptable)target;
        //myTarget.Value = result.question;
        //Repaint();

        //print(result);


        //quiz[0].questions = new List<Question>();

        //for (int i = 0; i < data.Length; i++)

        //make quiz[0]questions be reset to a new list of type quizdatascriptable, with 1 count, then put new stuff
        //in, would make it able to expand or shrink, need at least 1 question, if 0 questions could make warning appear
        //in debug log.

        //loop through each question and see if it's all there, if not do the json deserialize again.

        for (int i = 0; i < quiz[0].questions.Count; i++)
        {
            if (data[i].question == quiz[0].questions[i].questionInfo)
            {


                if (quiz[0].questions.Count < data.Length)
                    quiz[0].questions.AddRange(new Question[data.Length - 1]);
            
                if (!(quiz[0].questions[i].questionInfo == null))
                    StartCoroutine(AddData(5f, data));
                else
                {
                    Debug.LogError("list error, try again.");
                    Invoke("CheckForImportRequestEnd", 1.0f);
                }
            }
            else
            {
                //CheckForImportRequestEnd(); //try again if data is not synced.
                Debug.LogError("list error, try again.");
                Invoke("CheckForImportRequestEnd", 1.0f);
            }

        }





    }
    
    void HandleLog(string logString, string stackTrace, LogType type) {
        
        //if (type == LogType.Error) {
            StartCoroutine(AddData(1f, data));
        //}   
    }

    IEnumerator AddData(float timer, GDocResponse[] data)
    {
        yield return new WaitForSeconds(timer);

        if (data != null)
        {



            for (int i = 0; i < data.Length; i++)
            {

                //Load(JsonTxt, quiz); ;
                //might work, but the fields would probably have to be identical to the GDocResponse class, in the scriptable object.

                //quiz[0].questions.Count = data.Length;

                quiz[0].questions[i].questionInfo = data[i].question;
                
                //Application.logMessageReceived += HandleLog; //if there's an error, try again. Doesn't work.

                quiz[0].questions[i].correctAns = data[i].answer;

                for (int a = 0; a < 4; a++)
                    quiz[0].questions[i].options[a] = data[i].arr[a];

            }


            StartCoroutine(qUI.ActivateButtons(0f)); //only activate the buttons when the data is loaded.


        }


    }

    public void Load(string newData, string oldData)
    {
        JsonUtility.FromJsonOverwrite(newData, oldData);

    }

    // Given JSON input:
    // {"lives":3, "health":0.8}
    // the Load function will change the object on which it is called such that
    // lives == 3 and health == 0.8
    // the 'playerName' field will be left unchanged

    IEnumerator GetJson(string link)
    {
        using (WWW www = new WWW(link))
        {
            yield return www;

            if (www.text != "") //only reset the question list if there's a network connection and the json is downloaded.
            {

                print("Json is downloaded");
                JsonTxt = www.text.ToString();

                //JsonTxt = JsonTxt.Trim('[', ']');

                JsonTxt = fixJson(JsonTxt);

                print(JsonTxt);

                if (!(JsonTxt == null)) //if there's a json txt remove all questions apart from 1.
                {
                    quiz[0].questions.RemoveRange(1, quiz[0].questions.Count - 1);
                    //quiz[0].questions.RemoveRange(quiz[0].questions.Count - 1, 1);

                }


                //quiz[0].questions = new List<Question>(1); //reset question list in preparation for json
                
                yield return quiz[0].questions[0];

                if(quiz[0].questions[0] != null)
                    CheckForImportRequestEnd(); // the only way to wait for a process to finish is with this
            }

            else
            {
                Debug.LogError("There was no JSON file downloaded from server, check network connection.");
            }

        }
    }

    string fixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }


    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }


}
