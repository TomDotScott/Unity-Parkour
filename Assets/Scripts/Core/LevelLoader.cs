using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;

    public void LoadLevel()
    {
        gameObject.SetActive(true);
        StartCoroutine(loadAsynchronously(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private IEnumerator loadAsynchronously(int buildIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);
        while (!operation.isDone)
        {
            progressSlider.value = Mathf.Clamp01(operation.progress / .9f);
            yield return null; 
        }
    }
}
