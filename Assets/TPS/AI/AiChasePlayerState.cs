using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiChasePlayerState : AIState
{
    
    float timer = 0.0f;
    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Enter(AIAgent agent)
    {
    
    }
    public void Update(AIAgent agent)
    {
        if (!agent.enabled)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (agent.navMeshAgent.remainingDistance>agent.navMeshAgent.stoppingDistance)
        {
            agent.weaponIK.weight = 0;
        }
        else
        {
            agent.weaponIK.weight = 1;
        }
        if (Vector3.Distance(agent.transform.position, agent.playerTransform.position) <= agent.config.stopDistance)
        {
            // 触发射击行为
            agent.bullet.Shoot(agent.playerTransform);
        }
        //else
        //{
        //    // 如果距离还未到达停止距离，则继续追逐玩家
        //    if (!agent.navMeshAgent.hasPath)
        //    {
        //        agent.navMeshAgent.destination = agent.playerTransform.position;
        //    }

        //    if (timer < 0.0f)
        //    {
        //        Vector3 direction = (agent.playerTransform.position - agent.transform.position);
        //        direction.y = 0;
        //        if (direction.sqrMagnitude > agent.config.maxDistance * agent.config.maxDistance)
        //        {
        //            if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
        //            {
        //                agent.navMeshAgent.destination = agent.playerTransform.position;
        //            }
        //        }
        //        timer = agent.config.maxTime;
        //    }
        //}
    }



    public void Exit(AIAgent agent)
    {
     
    }

}
