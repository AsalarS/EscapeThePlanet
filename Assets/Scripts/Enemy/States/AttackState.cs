using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer; //Make the agent move slightly to make the accuracy less 
    private float losePlayerTimer; //How long the enemy remain in attack state before going back to search

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Perform()
    {
        if(enemy.CanSeePlayer())
        {
            losePlayerTimer = 0;
            moveTimer += Time.deltaTime;
            if(moveTimer > Random.Range(3, 7))
            {
                enemy.Agent.SetDestination(enemy.transform.position  + (Random.insideUnitSphere * 5)); //Move the agent to random location 
                moveTimer =0;
            }
        }
        else //Player cant be seen
        {
            losePlayerTimer += Time.deltaTime;
            if(losePlayerTimer > 8)
            {
                stateMachine.ChangeState(new PetrolState());
            }
        }
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
