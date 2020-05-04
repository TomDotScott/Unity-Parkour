using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// GameManager handles the core aspects of the game
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject pausedMenu = null;
    [SerializeField] private GameObject gameOverMenu = null;
    [SerializeField] private GameObject levelCompleteMenu = null;
    [SerializeField] private TextMeshProUGUI secondsTimer = null;

    private PlayerInput playerInput;
    private PlayerController playerController;

    private float score = 0;

    private bool isPaused;
    private bool gameOver;
    private bool levelComplete;

    public bool IsPaused { get => isPaused; set => isPaused = value; }
    public bool GameOver { get => gameOver; set => gameOver = value; }
    public bool LevelComplete
    {
        get => levelComplete; 
        
        set
        {
            levelComplete = value;
            if (!value) return;
            levelCompleteMenu.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = "Score: " + SecondsToMinutesAndSeconds(score);
            string levelName = "Level" + SceneManager.GetActiveScene().buildIndex.ToString();


            //If the score is greater than the saved score then we have set a new highscore!
            float highScore = PlayerPrefs.GetFloat(levelName, 10000);
            if (highScore > score)
            {
                Debug.Log("IT'S A NEW HIGHSCORE");
                PlayerPrefs.SetFloat(levelName, score);
                highScore = score;
            }


            levelCompleteMenu.transform.Find("HighScore").GetComponent<TextMeshProUGUI>().text = "HighScore: " + SecondsToMinutesAndSeconds(highScore);
            levelCompleteMenu.SetActive(true);
        }
    }

    private void Awake()
    {
        playerInput = player.GetComponent<PlayerInput>();
        playerController = player.GetComponent<PlayerController>();
        levelCompleteMenu.transform.Find("Level").GetComponent<TextMeshProUGUI>().text = "Level " + SceneManager.GetActiveScene().buildIndex + " Complete!";
    }


    // Update is called once per frame
    void Update()
    {
        if (!levelComplete)
        {
            CheckIfPlayerDead();
            CheckPause();
            if (!isPaused)
            {
                //Increase the timer
                score += Time.deltaTime;
                secondsTimer.text = SecondsToMinutesAndSeconds(score);
            }
        }
    }

    string SecondsToMinutesAndSeconds(float timeElapsed)
    {
        TimeSpan t = TimeSpan.FromSeconds(timeElapsed);
        return string.Format("{0}:{1}.{2}", t.Minutes, t.Seconds, t.Milliseconds);
    }


    private void CheckPause()
    {
        CheckInput();
        //Only pause the game after the animation is finished
        Time.timeScale = (isPaused && !pausedMenu.GetComponent<Menus>().AnimatorIsPlaying()) ? 0f : 1f;
        isPaused = pausedMenu.activeSelf;
    }

    private void CheckInput()
    {
        if (PlayerInput.GetPaused())
        {
            PauseUnpause();
        }
    }

    private void PauseUnpause()
    {
        secondsTimer.gameObject.SetActive(!secondsTimer.gameObject.activeSelf);
        pausedMenu.SetActive(!pausedMenu.activeSelf);
        //isPaused = !isPaused;
        //hide the options menu too if it's open
        if (pausedMenu.transform.parent.Find("OptionsMenu").gameObject.activeSelf)
        {
            pausedMenu.transform.parent.Find("OptionsMenu").gameObject.SetActive(false);
        }
    }

    private void CheckIfPlayerDead()
    {
        if (playerController.IsDead)
        {
            gameOverMenu.SetActive(true);
            secondsTimer.gameObject.SetActive(false);
            gameOver = true;
        }
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
