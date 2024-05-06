using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer; //Make the agent move slightly to make the accuracy less 
    private float losePlayerTimer; //How long the enemy remain in attack state before going back to search
    private float shootTimer;
    public float bulletSpeed =40f;

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {
            if (!enemy.PlayerHealth.IsDead())
            {
                // Player is visible and alive, perform attack actions
                losePlayerTimer = 0; // Reset lose player timer
                moveTimer += Time.deltaTime; // Increment move timer
                shootTimer += Time.deltaTime; // Increment shoot timer

                // Perform shooting actions based on shoot timer
                if (shootTimer > enemy.fireRate)
                {
                    Shoot();
                }

                // Perform movement actions based on move timer
                if (moveTimer > Random.Range(3, 7))
                {
                    enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                    moveTimer = 0; // Reset move timer
                }
            }
            else
            {
                // Player is dead, switch to patrol state
                stateMachine.ChangeState(new PetrolState());
            }
        }
        else // Player can't be seen
        {
            losePlayerTimer += Time.deltaTime; // Increment lose player timer
            if (losePlayerTimer > 8)
            {
                // Switch to patrol state if player is dead
                if (enemy.PlayerHealth.IsDead())
                {
                    stateMachine.ChangeState(new PetrolState());
                }
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
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(-2f,2f),Vector3.up) * gunbarrel.forward * bulletSpeed; //The higher the number the less the accurate

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
