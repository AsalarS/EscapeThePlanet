using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        if (hitTransform.CompareTag("Player")) //If player got hit
        {
            Debug.Log("hit");
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(10);
        }

        Destroy(gameObject);
    }
}
