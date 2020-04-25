using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TextPrompts is placed on a GameObject that will act as a trigger to display text to the player 
/// </summary>
public class TextPrompts : MonoBehaviour
{
    private TextMesh textObject;
    [SerializeField] private float lifeTime;
    [SerializeField] private string textString;
    private bool begin;
    private float timer;
    private BoxCollider boxCollider;

    private void Start()
    {
        textObject = gameObject.transform.GetChild(0).GetComponent<TextMesh>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (begin)
        {
            StartCoroutine(TypeWriterEffect());
            begin = false;
        }

        if (CheckIfFinished())
        {
            timer += Time.deltaTime;
            if(timer >= lifeTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private bool CheckIfFinished()
    {
        return textObject.text == textString;
    }

    private IEnumerator TypeWriterEffect()
    {
        foreach(char character in textString.ToCharArray())
        {
            textObject.text += character;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            begin = true;
            //boxCollider.isTrigger = false;
            Destroy(boxCollider);
        }
    }
}
