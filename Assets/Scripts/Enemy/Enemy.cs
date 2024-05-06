using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour

{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;
    private PlayerHealth playerHealth; // Reference to PlayerHealth component


    public NavMeshAgent Agent { get => agent; }
    public GameObject Player { get => player; }
    public PlayerHealth PlayerHealth { get => playerHealth; } // Expose PlayerHealth component


    public Path path;
    [Header("Sight Values")]
    public float sightDistance = 20f; //Enemy sight
    public float fieldOFView = 85f; //Enemy FOV
    public float eyeHeight =1.0f;

    [Header("Gun Values")]
    public Transform gunBarrel;
    [Range(0.1f, 10f)]
    public float fireRate;



    //This is for debugging
    [SerializeField]
    private string currentState;

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialize();
        player = GameObject.FindGameObjectWithTag("Player"); //Assign the object to the player object in unity
        playerHealth = player.GetComponent<PlayerHealth>(); // Assign reference to PlayerHealth

    }

    // Update is called once per frame
    void Update()
    {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
    }

    public bool CanSeePlayer()
    {
        if (player != null)
        {
            // Calculate direction from enemy to player
            Vector3 targetDirection = (player.transform.position - transform.position).normalized;

            // Check angle between enemy forward direction and direction to player
            float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);

            // Check if player is within field of view and distance
            if (angleToPlayer <= fieldOFView * 0.5f && Vector3.Distance(transform.position, player.transform.position) < sightDistance)
            {
                // Calculate raycast origin (enemy's eye level)
                Vector3 raycastOrigin = transform.position + Vector3.up * eyeHeight;

                // Perform raycast
                RaycastHit hitInfo;
                if (Physics.Raycast(raycastOrigin, targetDirection, out hitInfo, sightDistance))
                {
                    //Debug.Log("Object hit: " + hitInfo.collider.gameObject.name);
                    //Debug.Log("Object tag: " + hitInfo.collider.gameObject.tag);
                    //Debug.Log("Raycast direction: " + targetDirection);

                    // Check if the hit object is the player
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }

                Ray ray = new Ray(transform.position, targetDirection);
                Debug.DrawRay(ray.origin, ray.direction * sightDistance, Color.red); // Debug draw the ray in red color      
            }
            }

        return false;
    }


}