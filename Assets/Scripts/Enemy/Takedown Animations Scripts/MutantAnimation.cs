using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutantAnimation : EnemyAnimation
{
    

    // Start is called before the first frame update
    protected override void Start()
    {
        XPAmount = 300; //set the default xp value given upon death
        maxHealth = 750f; //set the health for the enemy
        stateMachine = GetComponent<StateMachine>();
        enemy = GetComponent<Enemy>();
        currentHealth = maxHealth;
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(currentHealth <= 0)
        {
            MutantDeath();
        }
    }

    private void MutantDeath()
    {
        enemyAnimator.SetBool("IsDead", true);
        Destroy(gameObject, 10f); //remove object from the game
        stateMachine.enabled = false;
        enemy.enabled = false;
        enemy.Agent.enabled = false;
        ExperienceManager.Instace.AddExperience(XPAmount);
        enabled = false;
    }




    protected override void PerformTakedown(Transform player, string targetName)
    {
        throw new System.NotImplementedException();
    }
}
