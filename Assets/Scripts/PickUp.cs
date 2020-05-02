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

    private Vector3 startPosition;

    private float bob = 10f;

    private void Start()
    {
        startPosition = transform.position;
        //If a hidden item, check it hasn't already been collected
        if (type == Type.CheshireCat || type == Type.MadHatter || type == Type.PlayingCard || type == Type.PocketWatch || type == Type.Teapot)
        {
            if (PlayerPrefs.GetString(type.ToString(), "False") != "False")
            {
                //Delete the object because it has already been collected
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        //spinny boi
        transform.Rotate(new Vector3(0f, 50f, 0f) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (type == Type.CheshireCat || type == Type.MadHatter || type == Type.PlayingCard || type == Type.PocketWatch || type == Type.Teapot)
            {
                PlayerPrefs.SetString(type.ToString(), "True");
                SoundManager.Instance.PlaySFX(type.ToString());
            }
            else
            {
                if (type == Type.GrowingPotion)
                {
                    other.gameObject.GetComponent<PlayerController>().CanGrow = true;
                }
                if (type == Type.ShrinkingPotion)
                {
                    other.gameObject.GetComponent<PlayerController>().CanShrink = true;
                }
                if (type == Type.HookShot)
                {
                    other.gameObject.GetComponent<PlayerController>().CanHookshot = true;
                }
                SoundManager.Instance.PlaySFX("Item");
            }
            Destroy(gameObject);
        }
    }
}