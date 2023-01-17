using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions.Must;

public class QuizGameUI : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private QuizManager quizManager;               //ref to the QuizManager script
    [SerializeField] private CategoryBtnScript categoryBtnPrefab;
    [SerializeField] private GameObject scrollHolder;
    [SerializeField] private Text scoreText, timerText;
    [SerializeField] private List<Image> lifeImageList;
    [SerializeField] private GameObject gameOverPanel, mainMenu, gamePanel;
    [SerializeField] private Color correctCol, wrongCol, normalCol; //color of buttons
    [SerializeField] private Image questionImg;                     //image component to show image
    [SerializeField] private UnityEngine.Video.VideoPlayer questionVideo;   //to show video
    [SerializeField] private AudioSource questionAudio;             //audio source for audio clip
    [SerializeField] private Text questionInfoText;                 //text to show question
    [SerializeField] private List<Button> options;                  //options button reference
    [SerializeField] private List<Image> Qimages;                   //list of questioninfo images
    [SerializeField] private List<Image> Aimages;                   //list of correct answer info images


#pragma warning restore 649

    private float audioLength;          //store audio length
    private Question question;          //store current question data
    private bool answered = false;      //bool to keep track if answered or not
    private bool chosen; //whether that option has been chosen already for the current question

    private List<Sprite> ansOptions;
    private List<Image> qImage;
    private List<Image> aImage;

    //private int iD = 0; //this did not work because it was persisting beyond the method so was each iteration of loop it was changing.

    public Text TimerText { get => timerText; }                     //getter
    public Text ScoreText { get => scoreText; }                     //getter
    public GameObject GameOverPanel { get => gameOverPanel; }                     //getter
    
    public static QuizGameUI instance = null; //Needed as part of the functionality in Awake, so there can only be one instance. 
    

    public static QuizGameUI GetInstance()
    {
        return instance;
    }

    
    void Awake()
    {
        //Check if there is already an instance of this.
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance.
            Destroy(gameObject);

        //Set this to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
       // DontDestroyOnLoad(gameObject);
       
       
    }


    private void Start()
    {

        qImage = new List<Image> (Qimages);
        aImage = new List<Image>(Aimages);

        ActivateOptionButtons();

        //Invoke("CreateCategoryButtons", 5.0f);
        CreateCategoryButtons();

    }

    void ActivateOptionButtons()
    {
        //add the listener to all the buttons
        for (int i = 0; i < options.Count; i++)
        {
            
            Button localBtn = options[i];

            int iD = i; //fixed this issue, needs to be local variable to work otherwise it kept being 2.

            //localBtn.name = localBtn.name + i; //to make an id number for each button
            
            localBtn.onClick.AddListener(() => OnClick(localBtn, iD));
            
        }

        chosen = false;

    }
    

    
    /// <summary>
    /// Method which populate the question on the screen
    /// </summary>
    /// <param name="question"></param>
    public void SetQuestion(Question question)
    {
        //set the question
        this.question = question;
        
        
        switch (question.questionType)
        {
            case QuestionType.TEXT:
                questionImg.transform.parent.gameObject.SetActive(false);   //deactivate image holder
                break;
            case QuestionType.IMAGE:
                questionImg.transform.parent.gameObject.SetActive(true);    //activate image holder
                questionVideo.transform.gameObject.SetActive(false);        //deactivate questionVideo
                questionImg.transform.gameObject.SetActive(true);           //activate questionImg
                questionAudio.transform.gameObject.SetActive(false);        //deactivate questionAudio

                questionImg.sprite = question.questionImage;                //set the image sprite
                break;
            case QuestionType.AUDIO:
                questionVideo.transform.parent.gameObject.SetActive(true);  //activate image holder
                questionVideo.transform.gameObject.SetActive(false);        //deactivate questionVideo
                questionImg.transform.gameObject.SetActive(false);          //deactivate questionImg
                questionAudio.transform.gameObject.SetActive(true);         //activate questionAudio
                
                audioLength = question.audioClip.length;                    //set audio clip
                StartCoroutine(PlayAudio());                                //start Coroutine
                break;
            case QuestionType.VIDEO:
                questionVideo.transform.parent.gameObject.SetActive(true);  //activate image holder
                questionVideo.transform.gameObject.SetActive(true);         //activate questionVideo
                questionImg.transform.gameObject.SetActive(false);          //deactivate questionImg
                questionAudio.transform.gameObject.SetActive(false);        //deactivate questionAudio

                questionVideo.clip = question.videoClip;                    //set video clip
                questionVideo.Play();                                       //play video
                break;
        }

        questionInfoText.text = question.questionInfo;                      //set the question text

        List<Sprite> listOptions = new List<Sprite>(question.options);
        Sprite answerSprite = question.correctImage;

        //shuffle the list of options
        // ansOptions = ShuffleList.ShuffleListItems(listOptions);

        ansOptions = listOptions;


        //question.iD = 0;

        for (int i = 0; i < aImage.Count; i++)
        {
            aImage[i].sprite = question.wrongAns; //all answer images set to the try again image.
            
            //important for getting the correct answer in QuizManager
            options[i].name = ansOptions[i].name;    //set the name of button

            //Button go = qImage[i].GetComponentInParent<Button>();

            if (options[i].name == question.correctAns.name)
                aImage[i].sprite = answerSprite; //the correct answer set to the correct answer info image.

        }




        //assign options to respective option buttons
        for (int i = 0; i < ansOptions.Count; i++)
        {
            //qImage[i] = GameObject.Find("Qimage" + i.ToString()).GetComponentInChildren<Image>();

            //set the child text
            //options[i].GetComponentInChildren<Text>().text = "";





            //take out if you don't want the answers shuffled.
            //options[i].image.sprite = ansOptions[i]; //set button image to ansOptions image.
            
            if(!chosen) //if option hasn't been chosen already.
                qImage[i].transform.gameObject.SetActive(true); //reactivate all question images for the new round.

            qImage[i].sprite = ansOptions[i]; //question images for the next question are loaded.



        }

        answered = false;                       

    }

    public void ReduceLife(int remainingLife)
    {
        lifeImageList[remainingLife].color = Color.red;
    }

    /// <summary>
    /// IEnumerator to repeate the audio after some time
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayAudio()
    {
        //if questionType is audio
        if (question.questionType == QuestionType.AUDIO)
        {
            //PlayOneShot
            questionAudio.PlayOneShot(question.audioClip);
            //wait for few seconds
            yield return new WaitForSeconds(audioLength + 0.5f);
            //play again
            StartCoroutine(PlayAudio());
        }
        else //if questionType is not audio
        {
            //stop the Coroutine
            StopCoroutine(PlayAudio());
            //return null
            yield return null;
        }
    }

    /// <summary>
    /// Method assigned to the buttons
    /// </summary>
    /// <param name="btn">ref to the button object</param>
    void OnClick(Button btn, int ButtonID)
    {
        if (quizManager.GameStatus == GameStatus.PLAYING)
        {
            //if answered is false
            if (!answered)
            {
                //set answered true
                answered = true;
                //get the bool value, if val is true answer is correct
                bool val = quizManager.Answer(btn);

                if (val) //if it's the right answer pause the timer while more info is shown
                {
                    quizManager.gameStatus = GameStatus.PAUSE;

                    ActivateOptionButtons(); //reactivate all options for next question.
                }
                else
                {
                    btn.onClick.RemoveAllListeners(); //take off listener. put back on next question.
                    //btn.transform.gameObject.SetActive(false); //stop it being possible to do the wrong answer more than once
                    chosen = true;

                }
                
                StartCoroutine(BlinkImg(btn.GetComponentInChildren<Image>(), val, ButtonID));
                    
                quizManager.NextQuestion(val);
                    
            }
        }
    }

    /// <summary>
    /// Method to create Category Buttons dynamically
    /// </summary>
    public void CreateCategoryButtons()
    {
        //we loop through all the available catgories in our QuizManager
        for (int i = 0; i < quizManager.QuizData.Count; i++)
        {
                        
            //Create new CategoryBtn
            CategoryBtnScript categoryBtn = Instantiate(categoryBtnPrefab, scrollHolder.transform);
            //Set the button default values
            categoryBtn.SetButton(quizManager.QuizData[i].categoryName, quizManager.QuizData[i].questions.Count);
            int index = i;
            //Add listner to button which calls CategoryBtn method
            categoryBtn.Btn.onClick.AddListener(() => CategoryBtn(index, quizManager.QuizData[index].categoryName));


                StartCoroutine(ActivateButtons(0f));
        }
        
        scrollHolder.SetActive(false);


    }


    public IEnumerator ActivateButtons(float timer)
    {
        yield return new WaitForSeconds(timer);

        scrollHolder.SetActive(true);




    }
    

    //Method called by Category Button
    private void CategoryBtn(int index, string category)
    {
        quizManager.StartGame(index, category); //start the game
        mainMenu.SetActive(false);              //deactivate mainMenu
        gamePanel.SetActive(true);              //activate game panel
    }

    //this give blink effect [if needed use or dont use]
    IEnumerator BlinkImg(Image img, bool correct, int iD)
    {
        for (int i = 0; i < 2; i++)
        {
            //qImage[iD].color = Color.white;
            //img.color = Color.white;
            yield return new WaitForSeconds(0.4f);

           // if (correct) 
                //qImage[iD].color = correctCol;
                //img.color = correctCol;
            //else
                //qImage[iD].color = wrongCol;
                //img.color = wrongCol;
            
            yield return new WaitForSeconds(0.4f);

            //img.color = Color.clear;
            //qImage[iD].color = Color.clear;

            img.transform.Find("Qimage").gameObject.SetActive(false);
            
            //yield return new WaitForSeconds(3.0f);
            //img.transform.Find("Qimage").gameObject.SetActive(true);

            //img.transform.Find("Qimage" + iD.ToString()).GetComponentInChildren<Image>().gameObject.SetActive(true);



            //makes the answer image change.
            //this.GetComponentInChildren<Image>().sprite = ansOptions.[i];
        }
        

        
        Debug.Log(iD);
    }

    public void RetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    

}
