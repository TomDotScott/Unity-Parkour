using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// GameManager handles the core aspects of the game
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject pausedMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private Transform killBox;

    private PlayerInput playerInput;
    private PlayerController playerController;

    private bool isPaused;
    private bool gameOver;

    public bool IsPaused { get => isPaused; set => isPaused = value; }
    public bool GameOver { get => gameOver; set => gameOver = value; }

    private void Awake()
    {
        playerInput = player.GetComponent<PlayerInput>();
        playerController = player.GetComponent<PlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        if (gameOverMenu.activeSelf == false)
        {
            CheckIfPlayerDead();
            CheckPause();
        }
    }

    private void CheckPause()
    {
        CheckInput();
        //Only pause the game after the animation is finished
        Time.timeScale = (isPaused && !pausedMenu.GetComponent<Menus>().AnimatorIsPlaying()) ? 0f : 1f;
    }

    private void CheckInput()
    {
        if (playerInput.GetPaused())
        {
            PauseUnpause();
        }
    }

    private void PauseUnpause()
    {
        pausedMenu.SetActive(!pausedMenu.activeSelf);
        isPaused = !isPaused;
    }

    private void CheckIfPlayerDead()
    {
        if (playerController.IsDead)
        {
            gameOverMenu.SetActive(true);
            gameOver = true;
        }
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
