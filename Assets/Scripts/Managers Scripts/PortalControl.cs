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
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Check if the next scene index is within the valid range
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Next scene index is out of range. Make sure you have added the scenes in Build Settings.");
        }
    }
}
