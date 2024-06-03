using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrierControl : MonoBehaviour
{
    [SerializeField] private Text playerMessage; // Reference to the UI Text element

    MeshRenderer barrierRenderer; //the barriers mesh to fade out

    private void Start()
    {
        barrierRenderer = GetComponent<MeshRenderer>();
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovment player = collision.gameObject.GetComponent<PlayerMovment>(); //get the player script
            if (player.HasToken) //if player collected a token
            {
                player.HasToken = false; //remove the token
                StartCoroutine(FadeAndDestroyBarrier()); // Deactivate the barrier
            }
            else
            {
                playerMessage.text = "You need to collect a token to pass the barrier."; // Display the message
            }
        }
    }

    IEnumerator FadeAndDestroyBarrier()
    {
        //slowly fade out the barrier, then destroy it
        for (float f = 1; f >= -0.05f; f -= 0.05f)
        {
            Color c = barrierRenderer.material.color;
            c.a = f;
            barrierRenderer.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }

        // Destroy the barrier after it has fully faded out
        Destroy(gameObject);
        
    }

    private void OnTriggerExit(Collider other)
    {
        playerMessage.text = ""; 
    }
}
