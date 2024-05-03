using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifespan = 5f; // Increased lifespan of the bullet in seconds

    public void OnCollisionEnter(Collision collision)
    {
        // Get the GameObject reference from the collision
        GameObject collidedObject = collision.gameObject;

        // Check if the collided object or any of its parents have the PlayerHealth component
        PlayerHealth playerHealth = collidedObject.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Transform parent = collidedObject.transform.parent;
            while (parent != null && playerHealth == null)
            {
                playerHealth = parent.GetComponent<PlayerHealth>();
                parent = parent.parent;
            }
        }

        // If the collided object or its parent has the PlayerHealth component, it's the player
        if (playerHealth != null)
        {
            // Apply damage to the player's health component
            playerHealth.TakeDamage(10);

            // Log the collision with the player
            Debug.Log("Bullet hit the player!");
        }
        else
        {
            // Log the collision with other objects
            Debug.Log("Bullet hit something else.");
        }

        // Start coroutine to destroy the bullet after a specified duration
        StartCoroutine(DestroyBulletAfterDelay());
    }

    private IEnumerator DestroyBulletAfterDelay()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(lifespan);

        // Destroy the bullet GameObject
        Destroy(gameObject);
    }
}
