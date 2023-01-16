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
    [SerializeField] private List<Image> Qimages;
    [SerializeField] private List<Image> Aimages;


#pragma warning restore 649

    private float audioLength;          //store audio length
    private Question question;          //store current question data
    private bool answered = false;      //bool to keep track if answered or not

    private List<Sprite> ansOptions;
    private List<Image> qImage;
    private List<Image> aImage;

    private int iD = 0;

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

        //add the listener to all the buttons
        for (int i = 0; i < options.Count; i++)
        {
            
            Button localBtn = options[i];

            iD = i;

            //localBtn.name = localBtn.name + i; //to make an id number for each button
            
            localBtn.onClick.AddListener(() => OnClick(localBtn, iD));
        }

        //Invoke("CreateCategoryButtons", 5.0f);
        CreateCategoryButtons();

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
        Sprite answerSprite = question.correctAns;

        //shuffle the list of options
        // ansOptions = ShuffleList.ShuffleListItems(listOptions);

        ansOptions = listOptions;


        //question.iD = 0;

        for (int i = 0; i < aImage.Count; i++)
        {
            aImage[i].sprite = question.wrongAns;
            //important for getting the correct answer in QuizManager
            options[i].name = ansOptions[i].name;    //set the name of button

            //Button go = qImage[i].GetComponentInParent<Button>();

            if (options[i].name == question.correctAns.name)
                aImage[i].sprite = answerSprite;

        }




        //assign options to respective option buttons
        for (int i = 0; i < ansOptions.Count; i++)
        {
            //qImage[i] = GameObject.Find("Qimage" + i.ToString()).GetComponentInChildren<Image>();

            //set the child text
            //options[i].GetComponentInChildren<Text>().text = "";





            //take out if you don't want the answers shuffled.
            //options[i].image.sprite = ansOptions[i]; //set button image to ansOptions image.


            qImage[i].sprite = ansOptions[i];



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

                // wait 3 seconds around here.

                    StartCoroutine(BlinkImg(btn.image, val, ButtonID));
                    
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
            img.color = Color.white;
            yield return new WaitForSeconds(0.1f);

            if (correct) 
                img.color = correctCol;
            else
                img.color = wrongCol;
            
            yield return new WaitForSeconds(0.1f);

            img.color = Color.clear;

            qImage[iD].transform.gameObject.SetActive(false);
            yield return new WaitForSeconds(3.0f);
            qImage[iD].transform.gameObject.SetActive(true);

            //img.transform.Find("Qimage" + iD.ToString()).GetComponentInChildren<Image>().gameObject.SetActive(true);



            //makes the answer image change.
            //this.GetComponentInChildren<Image>().sprite = ansOptions.[i];
        }
    }

    public void RetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    

}
