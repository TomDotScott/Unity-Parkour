using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private enum Type
    {
        MadHatter, Teapot, PocketWatch, PlayingCard, CheshireCat, ShrinkingPotion, GrowingPotion, HookShot
    }

    [SerializeField] private Type type;
    [SerializeField] private TextMesh text;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        //If a hidden item, check it hasn't already been collected
        if (type != Type.CheshireCat && type != Type.MadHatter && type != Type.PlayingCard &&
            type != Type.PocketWatch && type != Type.Teapot) return;
        if (PlayerPrefs.GetString(type.ToString(), "False") != "False")
        {
            //Delete the object because it has already been collected
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //spinny boi
        transform.Rotate(new Vector3(0f, 50f, 0f) * Time.deltaTime);
    }

    private int SecretCount()
    {
        int count = 0; 
        if (PlayerPrefs.GetString("MadHatter") == "True")
        {
            count++;
        }
        if (PlayerPrefs.GetString("Teapot") == "True")
        {
            count++;
        }
        if (PlayerPrefs.GetString("PlayingCard") == "True")
        {
            count++;
        }
        if (PlayerPrefs.GetString("CheshireCat") == "True")
        {
            count++;
        }
        return count;
    }

    private void RewardPrompt()
    {
        //work out how many secrets the player found
        text.text = "You have found " + SecretCount() + " of 5 Secrets"; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (type == Type.CheshireCat || type == Type.MadHatter || type == Type.PlayingCard || type == Type.PocketWatch || type == Type.Teapot)
        {
            PlayerPrefs.SetString(type.ToString(), "True");
            RewardPrompt();
            SoundManager.Instance.PlaySFX(type.ToString());
        }
        else
        {
            switch (type)
            {
                case Type.GrowingPotion:
                    other.gameObject.GetComponent<PlayerController>().CanGrow = true;
                    break;
                case Type.ShrinkingPotion:
                    other.gameObject.GetComponent<PlayerController>().CanShrink = true;
                    break;
                case Type.HookShot:
                    other.gameObject.GetComponent<PlayerController>().CanHookshot = true;
                    break;
            }
            SoundManager.Instance.PlaySFX("Item");
        }
        Destroy(gameObject);
    }
}