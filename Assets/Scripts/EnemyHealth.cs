using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float health;

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}