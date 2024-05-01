using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{

    public float maxHealth = 100f; //full health
    public float lowHealth = 20f; //critical health
    public float takedownRange = 2f; //the range of the takedown initial
    Animator enemyAnimator; //enemy animator reference
    Animator playerAnimator; //player animator reference
    public float currentHealth;//enemy's current health
    private bool isStaggered; //a boolean to check when the enemy reached critical state
    public Transform[] transferTargets; //for transfering the player to the correct position
    private CharacterController playerController; //to disable the player movement during the animation
    
    private bool isAnimating = false; //to prevent multiple takedowns in the same place

    public Rigidbody[] ragdollRigidbodies; // Array of rigidbodies representing the enemy's body parts
    public float launchForce = 100f; // The force applied to launch the enemy

    private bool isRagdollActive = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        isStaggered = false;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();//get player component
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();//get player component
        enemyAnimator = GetComponent<Animator>();
        // Disable the ragdoll initially
        SetRagdollActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= lowHealth) //if enemy reached low health
        {
            isStaggered = true;
        }
        if (isStaggered)
        {
            enemyAnimator.SetBool("IsStaggered", isStaggered); //start the staggered animation
        }
        if (isStaggered && Input.GetKeyDown("q") && !isAnimating) // if the enemy is stagered and the takedown button is pressed
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player"); //find the player object
            if (player != null && IsPlayerInRange(player.transform)) //check if player in range
            {
                (string nearestTargetName, Transform nearestTarget) = GetNearestTransferTarget(playerController.transform.position);
                PerformTakedown(player.transform,nearestTargetName); //initiate the takedown
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
    /// <summary>
    /// Initiate the takedown animations for both, player and enemy
    /// </summary>
    /// <param name="player"></param>
    void PerformTakedown(Transform player,string targetName)
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
                Debug.Log("Error: name is not found: "+ targetName);
                break;
        }

    }
    /// <summary>
    ///get the nearest target for the player to transform to,
    ///the enemy will have four targets srrounding him: front,left,right and back
    ///this is for choosing one of four animations to play depending on the player's location
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <returns>The position of the target for player to transform to</returns>
    
    private (string,Transform) GetNearestTransferTarget(Vector3 playerPosition)
    {
        string nearestTargetName = string.Empty;
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
    private bool IsPlayerInRange(Transform playerTransform)
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

        enabled = false;
    }



    public void ActivateRagdollAndLaunch()
    {
        // Activate the ragdoll physics
        SetRagdollActive(true);

        // Apply a launch force to the enemy
        Vector3 launchDirection = transform.forward; // Example: Launch in the forward direction of the enemy's transform
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
        }
    }

    private void SetRagdollActive(bool isActive)
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
