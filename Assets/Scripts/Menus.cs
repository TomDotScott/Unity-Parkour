using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public Animator animator;


    private void Update()
    {
        if (!AnimatorIsPlaying())
        {
            Cursor.lockState = CursorLockMode.Confined;
            Debug.Log("PAUSED");
        }
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

    public bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
