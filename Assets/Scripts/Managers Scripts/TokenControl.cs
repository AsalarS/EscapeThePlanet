using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenControl : MonoBehaviour
{
    PlayerMovment player; //player's script reference
    public AudioSource Token;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovment>(); //find the player script

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.HasToken = true;
            Destroy(gameObject);
            Token.Play();
        }
    }

}
