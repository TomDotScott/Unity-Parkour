using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI completedText;
    
    // Start is called before the first frame update
    void Start()
    {
        int count = 0;
        if(PlayerPrefs.GetString("MadHatter") == "True")
        {
            count++;
        }
        if(PlayerPrefs.GetString("Teapot") == "True")
        {
            count++;
        }
        if(PlayerPrefs.GetString("PlayingCard") == "True")
        {
            count++;
        }
        if(PlayerPrefs.GetString("CheshireCat") == "True")
        {
            count++;
        }
        completedText.text = "In your journey to have tea with the Queen of Hearts, you found " + count + " of 5 secrets.";
    }

    public void QuitToMain()
    {
        SceneManager.LoadScene(0);
    }
}
