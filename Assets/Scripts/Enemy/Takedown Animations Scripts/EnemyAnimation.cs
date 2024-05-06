using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyAnimation : MonoBehaviour
{

    public float maxHealth = 100f; //full health
    public float lowHealth = 20f; //critical health
    public float takedownRange = 2f; //the range of the takedown initial
    protected Animator enemyAnimator; //enemy animator reference
    protected Animator playerAnimator; //player animator reference
    public float currentHealth;//enemy's current health
    protected bool isStaggered; //a boolean to check when the enemy reached critical state
    public Transform[] transferTargets; //for transfering the player to the correct position
    protected CharacterController playerController; //to disable the player movement during the animation

    protected bool isAnimating = false; //to prevent multiple takedowns in the same place
    protected string nearestTargetName;
    public Rigidbody[] ragdollRigidbodies; // Array of rigidbodies representing the enemy's body parts
    public float launchForce = 100f; // The force applied to launch the enemy

    protected bool isRagdollActive = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        isStaggered = false;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();//get player component
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();//get player component
        enemyAnimator = GetComponent<Animator>();
        
        SetRagdollActive(false);
    }
    protected abstract void Update();
    // Update is called once per frame
    //void Update()
    //{
    //    if (currentHealth <= lowHealth) //if enemy reached low health
    //    {
    //        isStaggered = true;
    //    }
    //    if (isStaggered)
    //    {
    //        enemyAnimator.SetBool("IsStaggered", isStaggered); //start the staggered animation
    //    }
    //    if (isStaggered && Input.GetKeyDown("q") && !isAnimating) // if the enemy is stagered and the takedown button is pressed
    //    {
    //        GameObject player = GameObject.FindGameObjectWithTag("Player"); //find the player object
    //        if (player != null && IsPlayerInRange(player.transform)) //check if player in range
    //        {
    //            (string nearestTargetName, Transform nearestTarget) = GetNearestTransferTarget(playerController.transform.position);
    //            PerformTakedown(player.transform, nearestTargetName); //initiate the takedown
    //            if (nearestTarget != null)
    //            {
    //                //disable the controller for the player,
    //                //this is to make sure the player looks at the enemy during the animation
    //                playerController.enabled = false;
    //                playerController.transform.position = nearestTarget.position;// Move the player to the target position
    //                playerController.transform.LookAt(transform);//Make the player look at the enemy

    //                //return the controls. Note: Player will still not be able to control,
    //                //as the animation have it's own function to diable and re-enable movement script
    //                //this is just to solve the issue of the looking part
    //                playerController.enabled = true;
    //                isAnimating = true;//to prevent more takedowns

    //            }
    //        }

    //    }
    //}
    /// <summary>
    /// Initiate the takedown animations for both, player and enemy
    /// </summary>
    /// <param name="player"></param>
    protected abstract void PerformTakedown(Transform player,string targetName);
    //void PerformTakedown(Transform player,string targetName)
    //{
    //    switch (targetName)
    //    {
    //        case "Front":
    //    enemyAnimator.SetTrigger("Front Takedown"); //play enemy animation
    //    playerAnimator.SetTrigger("Alien Front Takedown"); //play player animation
    //            break;
    //        case "Left":
    //    enemyAnimator.SetTrigger("Left Takedown"); //play enemy animation
    //    playerAnimator.SetTrigger("Alien Left Takedown"); //play player animation
    //            break;
    //        case "Right":
    //    enemyAnimator.SetTrigger("Right Takedown"); //play enemy animation
    //    playerAnimator.SetTrigger("Alien Right Takedown"); //play player animation
    //            break;
    //        case "Back":
    //    enemyAnimator.SetTrigger("Back Takedown"); //play enemy animation
    //    playerAnimator.SetTrigger("Alien Back Takedown"); //play player animation
    //            break;
    //        default: 
    //            Debug.Log("Error: name is not found: "+ targetName);
    //            break;
    //    }

    //}
    /// <summary>
    ///get the nearest target for the player to transform to,
    ///the enemy will have four targets srrounding him: front,left,right and back
    ///this is for choosing one of four animations to play depending on the player's location
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <returns>The position of the target for player to transform to</returns>

    protected (string, Transform) GetNearestTransferTarget(Vector3 playerPosition)
    {
        nearestTargetName = string.Empty;
        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;

        //a foreach loop to calculate the nearest target
        foreach (Transform target in transferTargets)
        {
            float distance = Vector3.Distance(playerPosition, target.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = target;
                nearestTargetName = target.name;
            }
        }

        return (nearestTargetName, nearestTarget);
    }

    /// <summary>
    /// calculate the distance between the player when he tries to takedown the enemy during staggered state
    /// </summary>
    /// <param name="playerTransform"></param>
    /// <returns>True if the player is in range, false if not</returns>
    protected bool IsPlayerInRange(Transform playerTransform)
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= takedownRange;
    }

    /// <summary>
    /// Increase enemy health if left unanswered
    /// </summary>
    public void OnStaggerAnimationEnd()
    {
        isStaggered = false;
        enemyAnimator.SetBool("IsStaggered", isStaggered); //stop the staggered animation
        currentHealth += 75;
    }

    /// <summary>
    /// disable the script
    /// </summary>
    public void Die()
    {
        enemyAnimator.enabled = false;
        SetRagdollActive(true);
        enabled = false;
    }


    /// <summary>
    /// A function used by an animation to launch the enemy
    /// </summary>
    public void ActivateRagdollAndLaunch()
    {

        // Activate the ragdoll physics
        SetRagdollActive(true);
        enemyAnimator.enabled = false;
        // Apply a launch force to the enemy
        Vector3 launchDirection = transform.forward; // Example: Launch in the forward direction of the enemy's transform
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
        }
        Die();
    }

    /// <summary>
    /// Activate or deactivate the ragdoll component on the enemy body
    /// </summary>
    /// <param name="isActive"></param>
    protected void SetRagdollActive(bool isActive)
    {
        // Enable or disable the ragdoll rigidbodies
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !isActive;
        }

        isRagdollActive = isActive;
    }
    private void Awake()
    {
        // Automatically populate the ragdollRigidbodies array with the rigidbody components of the enemy's body parts
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        
    }
}
