using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour

{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }

    //This is for debugging
    [SerializeField]
    private string currentState;
    public Path path;
    private GameObject player;
    public float sightDistance = 20f; //Enemy sight
    public float fieldOFView = 85f; //Enemy FOV
    public float eyeHeight =1.0f;


    // Start is called before the first frame update
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialize();
        player = GameObject.FindGameObjectWithTag("Player"); //Assign the object to the player object in unity
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
                    Debug.Log("Object hit: " + hitInfo.collider.gameObject.name);
                    Debug.Log("Object tag: " + hitInfo.collider.gameObject.tag);
                    Debug.Log("Raycast direction: " + targetDirection);

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