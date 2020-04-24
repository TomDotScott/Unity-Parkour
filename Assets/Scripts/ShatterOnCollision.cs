using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterOnCollision : MonoBehaviour
{
    [SerializeField] private GameObject replacement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.GetComponent<PlayerController>().GrowShrinkState == PlayerStates.GrowShrinkState.giant)
            {
                Destroy(GameObject.Instantiate(replacement, transform.position, transform.rotation), 3f);
                Destroy(gameObject);
            }
        }
    }
}
