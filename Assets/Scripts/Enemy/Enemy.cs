using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour

{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }

    //This is for debuging
    [SerializeField]
    private string currentState;
    public Path path;
    private GameObject player;
    public float sightDistance = 20f; //Enemy sight
    public float fieldOFView = 85f; //Enemy FOV


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
    }

    public bool CanSeePlayer()
    {
        if (player != null)
        {
            //is the player close enough to be seen??
            if (Vector3.Distance(transform.position, player.transform.position) < sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position;
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if (angleToPlayer >= -fieldOFView && angleToPlayer <= fieldOFView)
                {
                    Ray ray = new Ray(transform.position, targetDirection.normalized); // Normalizing the direction
                    Debug.DrawRay(ray.origin, ray.direction * sightDistance, Color.red); // Adding color for better visualization
                }
            }

        }
        return true;
    }
}
