using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{

    public float maxHealth = 100f;
    public float lowHealth = 20f;
    public float takedownRange = 2f;
    public Animator enemyAnimator;
    public Animator playerAnimator;
    public float currentHealth;
    private bool isStaggered;
    public Transform[] transferTargets;
    private CharacterController playerController;
    
    private bool isAnimating = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        isStaggered = false;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= lowHealth)
        {
            isStaggered = true;
        }
        if (isStaggered)
        {
            enemyAnimator.SetBool("IsStaggered", true);
        }
        if (isStaggered && Input.GetKeyDown("q") && !isAnimating) // Replace 'KeyCode.T' with the desired button input
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && IsPlayerInRange(player.transform))
            {
                PerformTakedown(player.transform);
                Transform nearestTarget = GetNearestTransferTarget(playerController.transform.position);
                if (nearestTarget != null)
                {
                    playerController.enabled = false;
                    // Move the player to the target position
                    playerController.transform.position = nearestTarget.position;
                    // Transfer the player to the nearest transfer target
                    playerController.transform.LookAt(transform);
                    playerController.enabled = true;
                    
                    isAnimating = true;

                }
            }
            
        }
    }

    void PerformTakedown(Transform player)
    {

        enemyAnimator.SetTrigger("Takedown");
        playerAnimator.SetTrigger("Takedown");

    }
    private Transform GetNearestTransferTarget(Vector3 playerPosition)
    {
        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (Transform target in transferTargets)
        {
            float distance = Vector3.Distance(playerPosition, target.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = target;
            }
        }

        return nearestTarget;
    }
    private bool IsPlayerInRange(Transform playerTransform)
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= takedownRange;
    }
    
    public void die()
    {
        enabled = false;
    }
}
