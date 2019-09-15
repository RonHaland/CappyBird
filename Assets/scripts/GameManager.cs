using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public GameObject newPlayerPage;
    public GameObject backgroundController;

    GameObject highScorePage;
    public Text scoreText;

    public InputField inputTextName;
    string tutorialText;

    private int level;
    public int[] scoresPerLevel;
    public Sprite[] backgrounds;
    
    void StartSave()
    {
        tutorialText = PlayerPrefs.GetString("username");
        inputTextName.text = tutorialText;
    }

    public void SaveThis()
    {
        tutorialText = inputTextName.text;
        PlayerPrefs.SetString("username", tutorialText);

    }

    enum PageState
    {
        None,
        Start,
        Countdown,
        GameOver,
        NewPlayer,
        HighScore
    }

    int score = 0;
    bool gameOver = true;

    public bool GameOver { get { return gameOver; } }

    void Awake()
    {
        highScorePage = GameObject.Find("/HighScorePage");
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    void OnEnable()
    {
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;
    }

    void OnDisable()
    {
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
        gameOver = false;
    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
        if (scoresPerLevel.Contains(score))
        {
            ChangeBackground();
        }
    }

    private void ChangeBackground()
    {
        var ind = Array.IndexOf(scoresPerLevel, score);
        var spr = backgrounds[ind];
        backgroundController.GetComponent<SpriteRenderer>().sprite = spr;
        backgroundController.GetComponent<Transform>().position = Vector3.zero;
        var w = spr.rect.width;
        var h = spr.rect.height;
        backgroundController.transform.localScale = new Vector3(w/800, h/800, 0);
    }

    void OnPlayerDied()
    {
        gameOver = true;
        PlayerPrefs.SetString("score", score.ToString());
        int highScore = PlayerPrefs.GetInt("HighScore");
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

        string username = PlayerPrefs.GetString("username");
        string phone = PlayerPrefs.GetString("phone");

        HighscoreTable.AddHighscoreEntry(score, username, phone);

        SetPageState(PageState.GameOver);
        // WRITE TO TEXT FILE
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"./highscore.txt", true))
        {
            file.WriteLine("username:{0};phone:{1};score:{2}", PlayerPrefs.GetString("username"), PlayerPrefs.GetString("phone"), PlayerPrefs.GetString("score"));
        }
    }

    void Update()
    {
        //PlayerPrefs.DeleteAll();
        if (highScorePage == null)
        {
            highScorePage = GameObject.Find("/HighScorePage");
        }
        if (startPage.active)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SetPageState(PageState.NewPlayer);
            }
            else if (Input.GetKeyDown("r"))
            {
                SetPageState(PageState.Countdown);
            }
        }
        else if (gameOverPage.active)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ConfirmGameOver();
            }
            else if (Input.GetKeyDown("r"))
            {
                ConfirmGameOverAndReplay();
            }
        }
    }


    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                newPlayerPage.SetActive(false);
                highScorePage.SetActive(true);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                newPlayerPage.SetActive(false);
                highScorePage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                newPlayerPage.SetActive(false);
                highScorePage.SetActive(true);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                newPlayerPage.SetActive(false);
                highScorePage.SetActive(true);
                break;
            case PageState.NewPlayer:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                newPlayerPage.SetActive(true);
                highScorePage.SetActive(false);
                inputTextName.ActivateInputField();
                break;
        }
    }

    public void ConfirmGameOver()
    {
        SetPageState(PageState.Start);
        scoreText.text = "0";
        OnGameOverConfirmed();
    }

    public void ConfirmGameOverAndReplay()
    {
        SetPageState(PageState.Start);
        scoreText.text = "0";
        OnGameOverConfirmed();
        SetPageState(PageState.Countdown);
    }

    public void StartGame()
    {
        SetPageState(PageState.Countdown);
    }


    public void setNewPlayerPage()
    {
        SetPageState(PageState.NewPlayer);
    }

    public void StartGameAndSavePlayerPrefs()
    {
        PlayerPrefs.Save();
        SetPageState(PageState.Countdown);
    }

    public void ShowHighScoreTable()
    {

    }

}
