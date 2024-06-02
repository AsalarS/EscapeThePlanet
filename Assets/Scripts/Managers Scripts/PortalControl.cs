using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalControl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovment player = other.gameObject.GetComponent<PlayerMovment>();
            if (player != null)
            {
                player.DisableMovement();
                //TODO transfer player to another scene
            }
        }
    }
}
