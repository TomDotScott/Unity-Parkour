using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private GameObject optionsMenu = null;

    private void Update()
    {
        if (!AnimatorIsPlaying())
        {
            Cursor.lockState = CursorLockMode.Confined;
            Debug.Log("PAUSED");
        }
    }

    private void Awake()
    {
        if(this.transform.name == "GameOverMenu")
        {
            SoundManager.Instance.PlaySFX("GameOver");
        }else if(this.transform.name == "LevelCompleteMenu")
        {
            SoundManager.Instance.PlaySFX("LevelComplete");
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Resume()
    {
        Debug.Log("UNPAUSED");
        gameObject.SetActive(false);
        GameManager.Instance.IsPaused = false;
    }

    public void Restart()
    {
        Debug.Log("RESTARTED");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.IsPaused = false;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitToMain()
    {
        Debug.Log("QUIT TO MAIN MENU");
        GameManager.Instance.IsPaused = false;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }

    public void ShowHideOptions()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }

    public bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
