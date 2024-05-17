using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienTakedown : EnemyAnimation
{
    
        protected override void Update()
        {
            if (currentHealth <= lowHealth) //if enemy reached low health
            {
                isStaggered = true;
            }
            if (isStaggered)
            {
                enemyAnimator.SetBool("IsStaggered", isStaggered); //start the staggered animation
            }
            if (isStaggered && Input.GetKeyDown("q") && !isAnimating && !PlayerMovment.IsAnimating) // if the enemy is stagered and the takedown button is pressed
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player"); //find the player object
                if (player != null && IsPlayerInRange(player.transform)) //check if player in range
                {
                    (string nearestTargetName, Transform nearestTarget) = GetNearestTransferTarget(playerController.transform.position);
                PlayerMovment.IsAnimating = true;
                    PerformTakedown(player.transform, nearestTargetName); //initiate the takedown
                    if (nearestTarget != null)
                    {
                        //disable the controller for the player,
                        //this is to make sure the player looks at the enemy during the animation
                        playerController.enabled = false;
                        playerController.transform.position = nearestTarget.position;// Move the player to the target position
                        playerController.transform.LookAt(transform);//Make the player look at the enemy

                        //return the controls. Note: Player will still not be able to control,
                        //as the animation have it's own function to diable and re-enable movement script
                        //this is just to solve the issue of the looking part
                        playerController.enabled = true;
                        isAnimating = true;//to prevent more takedowns

                    }
                }

            }
        }
        protected override void PerformTakedown(Transform player, string targetName)
        {
        
            switch (targetName)
            {
                case "Front":
                    enemyAnimator.SetTrigger("Front Takedown"); //play enemy animation
                    playerAnimator.SetTrigger("Alien Front Takedown"); //play player animation
                    break;
                case "Left":
                    enemyAnimator.SetTrigger("Left Takedown"); //play enemy animation
                    playerAnimator.SetTrigger("Alien Left Takedown"); //play player animation
                    break;
                case "Right":
                    enemyAnimator.SetTrigger("Right Takedown"); //play enemy animation
                    playerAnimator.SetTrigger("Alien Right Takedown"); //play player animation
                    break;
                case "Back":
                    enemyAnimator.SetTrigger("Back Takedown"); //play enemy animation
                    playerAnimator.SetTrigger("Alien Back Takedown"); //play player animation
                    break;
                default:
                    Debug.Log("Error: name is not found: " + targetName);
                    break;
            }

        }
    public void AlienActivateRagdollAndLaunch()
    {

        // Activate the ragdoll physics
        SetRagdollActive(true);
        enemyAnimator.enabled = false;
        float launchForce = 100f;
        Vector3 launchDirection = transform.forward;
        addForce(launchForce, launchDirection);
        Die();
    }
    public void AlienActivateRagdollAndLaunchTwo()
    {

        // Activate the ragdoll physics
        SetRagdollActive(true);
        enemyAnimator.enabled = false;
        float launchForce = 70f;
        Vector3 launchDirection = -transform.right;
        addForce(launchForce, launchDirection);
        Die();
    }
}

