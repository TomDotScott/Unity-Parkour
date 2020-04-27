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
    [SerializeField] private Transform startPosition;
    [SerializeField] private Quaternion startRotation;
    [SerializeField] private Transform killBox;

    private PlayerInput playerInput;

    private bool isPaused;

    public bool IsPaused { get => isPaused; set => isPaused = value; }

    // Start is called before the first frame update
    void Start()
    {
        playerInput = player.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfPlayerDead();
        CheckPause();
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
        if(player.transform.position.y <= killBox.position.y)
        {
            Debug.Log("THE PLAYER DIED!");
        }
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
