using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyAnimation : MonoBehaviour
{

    public float maxHealth { get; set; } //full health
    public float lowHealth { get; set; } = 40f; //critical health
    public float takedownRange = 2f; //the range of the takedown initial
    protected Animator enemyAnimator; //enemy animator reference
    protected Animator playerAnimator; //player animator reference
    public float currentHealth;//enemy's current health
    protected bool isStaggered; //a boolean to check when the enemy reached critical state
    public Transform[] transferTargets; //for transfering the player to the correct position
    protected CharacterController playerController; //to disable the player movement during the animation
    protected bool isAnimating = false; //to prevent multiple takedowns in the same place
    protected string nearestTargetName; //to transfer the player to one of the four objects' name
    [HideInInspector]public Rigidbody[] ragdollRigidbodies; // Array of rigidbodies representing the enemy's body parts
    //[HideInInspector]public Collider[] ragdollColliders; // Array of Colliders representing the enemy's body parts
    public int XPAmount { get; set; }
    protected StateMachine stateMachine;
    protected Enemy enemy;

    // Start is called before the first frame update
    protected abstract void Start();
    /// <summary>
    /// This method contains the shared elements to be called with the childs class start method
    /// </summary>
    protected void SharedStart()
    {
        currentHealth = maxHealth;
        isStaggered = false;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();//get player component
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();//get player component
        enemyAnimator = GetComponent<Animator>();
        SetRagdollActive(false);
        stateMachine = GetComponent<StateMachine>();
        enemy = GetComponent<Enemy>();
    }
    protected abstract void Update();
    
    /// <summary>
    /// Initiate the takedown animations for both, player and enemy
    /// </summary>
    /// <param name="player"></param>
    protected abstract void PerformTakedown(Transform player,string targetName);
    
    
    /// <summary>
    ///get the nearest target for the player to transform to,
    ///the enemy will have four targets srrounding him: front,left,right and back. <br></br>
    ///This is for choosing one of four animations to play depending on the player's location
    /// </summary>
    /// <param name="playerPosition">The player's current position</param>
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
    /// calculate the distance between the player and enemy when he tries to takedown the enemy during staggered state
    /// </summary>
    /// <param name="playerTransform">Player's current position</param>
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
        stateMachine.enabled = true;
        enemy.enabled = true;
        enemy.Agent.enabled = true;
        stateMachine.ChangeState(new PetrolState());
    }

    /// <summary>
    /// disable the script
    /// </summary>
    public void Die(int amount)
    {
        enemyAnimator.enabled = false; //turn off the animator
        SetRagdollActive(true); //turn on rigidbody components for ragdoll effect
        Destroy(gameObject,10f); //remove object from the game
        
        ExperienceManager.Instace.AddExperience(amount);

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
        //foreach (Collider collider in ragdollColliders)
        //{ 
        //    collider.enabled = isActive; 
        //}
        
    }
    private void Awake()
    {
        // Automatically populate the ragdollRigidbodies array with the rigidbody components of the enemy's body parts
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        //ragdollColliders = GetComponentsInChildren<Collider>();
        
    }
    /// <summary>
    /// Called by public methods in animation events to apply force
    /// </summary>
    /// <param name="force">force power to apply</param>
    /// <param name="launchDirection">direction of force</param>
    protected void addForce(float force, Vector3 launchDirection)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.AddForce(launchDirection * force, ForceMode.Impulse);
        }
    }
}
