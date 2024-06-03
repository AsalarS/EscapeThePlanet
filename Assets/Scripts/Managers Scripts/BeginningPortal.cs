using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;// Include the SceneManagement namespace
using UnityEngine.UI; 

public class BeginningPortal : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 7.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            /*PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.enabled = false;
            }*/

            // Load the next scene
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        
            FadeOut();
            
        
    }

    private void FadeOut()
    {
        float timer = 0f;
        Color startColor = new Color(1f, 1f, 1f, 0.1f); // Semi-transparent white
        Color endColor = new Color(1f, 1f, 1f, 1f); // Fully opaque white

        // Set the initial color to the start color
        fadeImage.color = startColor;

        while (timer < fadeDuration)
        {
            // Interpolate the alpha between start and end over time.
            fadeImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(startColor.a, endColor.a, timer / fadeDuration));
            timer += Time.deltaTime;
        }

        // Ensure the image is completely white at the end.
        fadeImage.color = endColor;
        // Assuming scenes are in a sequential order
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Check if the next scene index is within the valid range
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)

            SceneManager.LoadScene(nextSceneIndex);
    }
}
