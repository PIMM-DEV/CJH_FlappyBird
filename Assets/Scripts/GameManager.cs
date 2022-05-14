using System.Collections;
using System.Collections.Generic;
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
    public GameObject settingPage;
    public GameObject boomButton;
    public Text scoreText;
    public Slider slider;

    public float difficulty;

    bool doSetting;
    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown,
        Setting
    }

    int score = 0;
    bool gameOver = true;

    public int Score { get { return score; } }
    public bool GameOver
    {
        get
        {
            return gameOver;
        }
    }
    void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        SetPageState(PageState.Start);
        slider.minValue = 0.3f;
        
    }

    void OnEnable()
    {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
    }


    void OnDisable()
    {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
        gameOver = false;
    }

    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if(score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }



    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                boomButton.SetActive(true);
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                settingPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                settingPage.SetActive(false);
                boomButton.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                settingPage.SetActive(false);
                boomButton.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                settingPage.SetActive(false);
                boomButton.SetActive(false);
                break;
            case PageState.Setting:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                settingPage.SetActive(true);
                boomButton.SetActive(false);
                break;
        }
    }

    public void ConfirmGameOver()
    {
        OnGameOverConfirmed();
        scoreText.text = "0";
        SetPageState(PageState.Start);
    }

    public void StartGame()
    {
        SetPageState(PageState.Countdown);
    }

    public void Setting()
    {
        slider.value = difficulty;
        SetPageState(PageState.Setting);
    }

    public void SettingStart()
    {
        difficulty = slider.value;
        SetPageState(PageState.Countdown);
    }
}
