using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                LoadNextScene();
            }
        }
    }

    private void LoadNextScene()
    {
        // Assuming scenes are in a sequential order
        

        // Check if the next scene index is within the valid range
        
        
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);

    }
}
