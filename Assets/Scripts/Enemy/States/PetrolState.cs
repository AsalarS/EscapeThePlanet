using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrolState : BaseState
{
    // Start is called before the first frame update
    public int wayPointIndex; 
    public override void Enter()
    {

    }public override void Perform()
    {
        PetrolCycle();

    }public override void Exit()
    {

    }

    public void PetrolCycle()
    {
        if(enemy.Agent.remainingDistance < 0.2f)
        {
            if(wayPointIndex < enemy.path.waypoints.Count - 1)
            {
                wayPointIndex++;
            }
            else
            {
                wayPointIndex = 0;
                enemy.Agent.SetDestination(enemy.path.waypoints[wayPointIndex].position);
            }
        }
    }
}
