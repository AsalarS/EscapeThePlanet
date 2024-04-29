using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrolState : BaseState
{
    private int wayPointIndex = 0; // Initialize waypoint index
    private float waitTime = 3f; // Time to wait at each waypoint
    private float elapsedTime = 0f; // Time elapsed since reaching the waypoint

    public override void Enter()
    {
        // Set the initial destination to the first waypoint
        SetNextDestination();
    }

    public override void Perform()
    {
        // Check if the agent has reached the current waypoint
        if (enemy.Agent.remainingDistance < 0.2f)
        {
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Check if the agent has waited long enough
            if (elapsedTime >= waitTime)
            {
                // Move to the next waypoint
                MoveToNextWaypoint();

                // Reset the elapsed time
                elapsedTime = 0f;
            }
        }
    }

    public override void Exit()
    {
        // Clean up any state exit tasks (if needed)
    }

    private void MoveToNextWaypoint()
    {
        // Increment the waypoint index
        wayPointIndex++;

        // Check if the waypoint index exceeds the number of waypoints
        if (wayPointIndex >= enemy.path.waypoints.Count)
        {
            // If it does, reset the waypoint index to 0 (loop back to the beginning)
            wayPointIndex = 0;
        }

        // Set the destination to the next waypoint
        SetNextDestination();
    }

    private void SetNextDestination()
    {
        // Set the agent's destination to the position of the next waypoint
        enemy.Agent.SetDestination(enemy.path.waypoints[wayPointIndex].position);
    }
}
