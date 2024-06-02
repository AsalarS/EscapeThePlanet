using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingControl : MonoBehaviour
{
    private Coroutine currentHealingCoroutine;
    private bool isObjectActive = true;
    public float deactivationDuration = 30f; // Duration the object is disabled, in seconds

    private ParticleSystem particleSystem;
    private ParticleSystem.MainModule particleMainModule;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleMainModule = particleSystem.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player.health < player.maxHealth)
            {
                StartHealing(player);
            }
        }
    }

    private void StartHealing(PlayerHealth player)
    {
        if (isObjectActive)
        {
            // Stop any previous healing coroutine
            StopHealing();

            // Start a new healing coroutine
            currentHealingCoroutine = StartCoroutine(HealPlayer(player));

            // Deactivate the object and change the particle color
            isObjectActive = false;
            ChangeParticleColor(new Color(1, 0, 0, 1)); // Red color
            StartCoroutine(ReactivateObject());
        }
    }

    private void StopHealing()
    {
        if (currentHealingCoroutine != null)
        {
            StopCoroutine(currentHealingCoroutine);
            currentHealingCoroutine = null;
        }
    }

    private IEnumerator HealPlayer(PlayerHealth playerHealth)
    {
        float healingDuration = 5f; // 5 seconds of healing
        float healingRate = 10f; // Heal 1 health point per second

        float elapsedTime = 0f;
        while (elapsedTime < healingDuration)
        {
            // Heal the player using the RestoreHealth() method
            playerHealth.RestoreHealth(healingRate * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime); // Wait for the next frame
            elapsedTime += Time.deltaTime;
        }
    }

    private IEnumerator ReactivateObject()
    {
        yield return new WaitForSeconds(deactivationDuration);
        isObjectActive = true;
        ChangeParticleColor(new Color(0, 1, 0, 1)); // Green color
    }

    private void ChangeParticleColor(Color newColor)
    {
        particleMainModule.startColor = newColor;
        particleSystem.Emit(1); // Trigger a particle emission to update the color
    }
}