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
        }
    }


    public void Resume()
    {
        gameObject.SetActive(false);
        GameManager.Instance.IsPaused = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.IsPaused = false;
    }

    public void QuitToMain()
    {
        GameManager.Instance.IsPaused = false;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
