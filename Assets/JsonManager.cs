using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Playables;
using static UnityEngine.GraphicsBuffer;

public class JsonManager : MonoBehaviour
{

    private UnityWebRequest _webRequest;
    [SerializeField] private List<QuizDataScriptable> quiz;
    //GDocResponse result = new GDocResponse();

    public GDocResponse data;

    private string JsonTxt;
    private string JsonLink = "https://script.google.com/macros/s/AKfycbyqSyn7He9t5tg9Tzd1Ps_Q6i_IoF6VIy0RxDNOI0jEvDf_F1oRLw4zxjwS9I3Zfb8/exec";
    public void Start()
{
        //quiz = (QuizDataScriptable)Target;

        StartCoroutine(GetJson(JsonLink));





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

        GDocResponse[] data = JsonHelper.FromJson<GDocResponse>(JsonTxt);


        //var result = JsonUtility.FromJson<GDocResponse>(JsonTxt);
        //QuizDataScriptable myTarget = (QuizDataScriptable)target;
        //myTarget.Value = result.question;
        //Repaint();

        //print(result);

        for (int i = 0; i < data.Length; i++)
            {

            //Load(JsonTxt, quiz); ;
            //might work, but the fields would probably have to be identical to the GDocResponse class, in the scriptable object.

            quiz[0].questions[i].questionInfo = data[i].question;
            quiz[0].questions[i].correctAns = data[i].answer;

                for (int a = 0; a < 4; a++)
                    quiz[0].questions[i].options[a] = data[i].arr[a];

            }
       // }
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

            print("Json is downloaded");
            JsonTxt = www.text.ToString();

            //JsonTxt = JsonTxt.Trim('[', ']');

            JsonTxt = fixJson(JsonTxt);

            print(JsonTxt);

            CheckForImportRequestEnd(); // the only way to wait for a process to finish is with this

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
