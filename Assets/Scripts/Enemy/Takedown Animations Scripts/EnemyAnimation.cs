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
    
    /// <summary>
    /// Initiate the takedown animations for both, player and enemy
    /// </summary>
    /// <param name="player"></param>
    protected abstract void PerformTakedown(Transform player,string targetName);
    
    
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
