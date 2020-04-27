using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles the events that happen when a level is completed, including the ranking of the level, 
/// the destruction of the previous level and the loading in of the next level
/// </summary>
public class NextLevel : MonoBehaviour
{
    //For now, just load the next level
    public void LoadNextLevel()
    {
        Debug.Log("I HIT THE PLAYER");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            LoadNextLevel();
        }
    }
}
