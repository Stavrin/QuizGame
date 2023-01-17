using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
#pragma warning disable 649
    //ref to the QuizGameUI script
    [SerializeField] private QuizGameUI quizGameUI;
    //ref to the scriptableobject file
    [SerializeField] private List<QuizDataScriptable> quizDataList;
    [SerializeField] private float timeInSeconds;
#pragma warning restore 649

    private string currentCategory = "";
    private int correctAnswerCount = 0;
    //questions data
    private List<Question> questions;
    //current question data
    private Question selectedQuestion = new Question();
    private int gameScore;
    private int livesRemaining;
    private float currentTime;
    private QuizDataScriptable dataScriptable;

    public GameStatus gameStatus = GameStatus.NEXT;

    public GameStatus GameStatus { get { return gameStatus; } } //public getter.

    public List<QuizDataScriptable> QuizData { get => quizDataList; }

    public void StartGame(int categoryIndex, string category)
    {
        currentCategory = category;
        correctAnswerCount = 0;
        gameScore = 0;
        livesRemaining = 3;
        currentTime = timeInSeconds;
        //set the questions data
        questions = new List<Question>();
        dataScriptable = quizDataList[categoryIndex];
        questions.AddRange(dataScriptable.questions);
        //select the question
        SelectQuestion();
        gameStatus = GameStatus.PLAYING;
    }

    /// <summary>
    /// Method used to randomly select the question form questions data
    /// </summary>
    private void SelectQuestion()
    {
        gameStatus = GameStatus.PLAYING; //important that this is here to resume timer on next question.
        
        //get the random number
        int val = UnityEngine.Random.Range(0, questions.Count);
        //set the selectedQuestion
        selectedQuestion = questions[val];
        //send the question to quizGameUI
        quizGameUI.SetQuestion(selectedQuestion);

        questions.RemoveAt(val); //remove the question so it doesn't show again during this game.
    }

    private void Update()
    {
        if (gameStatus == GameStatus.PLAYING)
        {
            currentTime -= Time.deltaTime;
            SetTime(currentTime);
        }
        
        
    }

    void SetTime(float value)
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);                       //set the time value
        quizGameUI.TimerText.text = time.ToString("mm':'ss");   //convert time to Time format

        if (currentTime <= 0)
        {
            //Game Over
            GameEnd();
        }
    }

    /// <summary>
    /// Method called to check if the answer is correct or not
    /// </summary>
    /// <param name="selectedOption">answer string</param>
    /// <returns></returns>
    public bool Answer(Button selectedOption)
    {
        //set default to false
        bool correct = false;
        //if selected answer is the same as the correctAns
        if (selectedQuestion.correctAns.name == selectedOption.name)
        {
            //Yes, Ans is correct
            correctAnswerCount++;
            correct = true;
            gameScore += 50;
            quizGameUI.ScoreText.text = "Score:" + gameScore;
            

            //important, how you find a child gameobject, you have to use transform.
            //selectedOption.transform.Find("Aimage").GetComponentInChildren<Image>().sprite = selectedQuestion.correctAns;;
            

        }
        else
        {
            //No, Ans is wrong
            //Reduce Life
            livesRemaining--;
            quizGameUI.ReduceLife(livesRemaining);
            
            //selectedOption.transform.Find("Aimage").GetComponentInChildren<Image>().sprite = selectedQuestion.wrongAns;

            if (livesRemaining == 0)
            {
                GameEnd();
            }
        }


        return correct;


    }

    public void NextQuestion(bool correct)
    {
        if (gameStatus == GameStatus.PLAYING || gameStatus == GameStatus.PAUSE)
        {
            if (questions.Count > 0 && correct)
            {
                
                //change gamestatus to paused and pause timer while displaying answer.
                
                //call SelectQuestion method again after 3s
                Invoke("SelectQuestion", 3.0f);
                
                //gameStatus = GameStatus.PLAYING;
            }
            else if (questions.Count > 0)
            {
                quizGameUI.SetQuestion(selectedQuestion);
            }

            if (!(questions.Count > 0))
            {
                GameEnd();
            }
        }
    }

    private void GameEnd()
    {
        gameStatus = GameStatus.NEXT;
        quizGameUI.GameOverPanel.SetActive(true);

        //fi you want to save only the highest score then compare the current score with saved score and if more save the new score
        //eg:- if correctAnswerCount > PlayerPrefs.GetInt(currentCategory) then call below line

        //Save the score
        PlayerPrefs.SetInt(currentCategory, correctAnswerCount); //save the score for this category
    }
}

//Datastructure for storing the questions data
[System.Serializable]
public class Question
{
    public string questionInfo = "Which bye-law is correct?";         //question text
    public QuestionType questionType;   //type
    public Sprite questionImage;        //image for Image Type
    public AudioClip audioClip;         //audio for audio type
    public UnityEngine.Video.VideoClip videoClip;   //video for video type
    
    public Sprite[] options  = new Sprite[4];        //options to select
    public Sprite correctAns;           //correct option sprite
    public Sprite wrongAns;           //wrong option sprite
    
    public Sprite correctImage; //correct answer writing in image form.

    public int iD;

    
}

[System.Serializable]
public enum QuestionType
{
    TEXT,
    IMAGE,
    AUDIO,
    VIDEO

}

[SerializeField]
public enum GameStatus
{
    PLAYING,
    NEXT,
    PAUSE
}