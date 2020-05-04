using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterOnCollision : MonoBehaviour
{
    [SerializeField] private GameObject replacement = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PlayerController>().GrowShrinkState == PlayerStates.GrowShrinkState.Giant)
            {
                Destroy(GameObject.Instantiate(replacement, transform.position, transform.rotation), 3f);
                Destroy(gameObject);
            }
        }
    }
}
