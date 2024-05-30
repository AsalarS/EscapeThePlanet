using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer; //Make the agent move slightly to make the accuracy less 
    private float losePlayerTimer; //How long the enemy remain in attack state before going back to search
    private float shootTimer;
    public float bulletSpeed = 40f;
    public bool chase = false; //if chase state make true, if shoot enemy make false
    public override void Enter()
    {
    }

    public override void Exit()
    {
    }
    public void ChasePlayer()
    {
        // Set destination to player position for chasing
        enemy.Agent.SetDestination(enemy.Player.transform.position);
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {

            if (!enemy.PlayerHealth.IsDead())
            {
                if (enemy.enemyType == 0) //IF enemy is not a shooter, 
                {
                    enemy.Agent.speed = 4f;
                    ChasePlayer();
                }
                else //IF enemy is a shooter
                {


                    // Player is visible and alive, perform attack actions

                    //Lock the lose player timer and increment move timer and shot timer
                    losePlayerTimer = 0;
                    moveTimer += Time.deltaTime;
                    shootTimer += Time.deltaTime;
                    enemy.transform.LookAt(enemy.Player.transform);

                    if (shootTimer > enemy.fireRate)
                    {
                        Shoot();
                    }

                    if (moveTimer > Random.Range(3, 7))
                    {
                        enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5)); //Move the agent to random location 
                        moveTimer = 0;
                    }
                }
                enemy.LastKnownPos = enemy.Player.transform.position;
            }
            else //Player cant be seen
            {
                losePlayerTimer += Time.deltaTime;
                if (losePlayerTimer > 8)
                {
                    stateMachine.ChangeState(new SearchState());
                }
            }

            if (enemy.PlayerHealth.IsDead()) // Assuming IsDead() returns true if player health is zero or below
            {
                // Switch to patrol state
                stateMachine.ChangeState(new PetrolState());
            }

        }
    }


    public void Shoot()
    {
        // Store reference to the gun barrel
        Transform gunbarrel = enemy.gunBarrel;

        // Calculate the direction to the player
        Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.transform.position).normalized;

        // Aim the gun barrel towards the player
        gunbarrel.LookAt(enemy.Player.transform);

        // Instantiate a new bullet
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefab/Bulet") as GameObject, gunbarrel.position, gunbarrel.rotation);

        // Add force to the bullet in the direction of the gun barrel
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(0f, 0f), Vector3.up) * gunbarrel.forward * bulletSpeed; //The higher the number the less the accurate

        shootTimer = 0;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
