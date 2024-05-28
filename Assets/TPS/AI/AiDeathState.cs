using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiDeathState : AIState
{
    public Vector3 direction;
   
    public AiStateId GetId()
    {
        return AiStateId.Death;
    }

    public void Enter(AIAgent agent)
    {
        agent.weaponIK.enabled = false;
        agent.ragdoll.ActivatteRagdoll();
        direction.y = 1;
        agent.ragdoll.ApplyForce(direction * agent.config.dieForce);
        agent.ui.gameObject.SetActive(false);
        
    }
    public void Update(AIAgent agent)
    {

    }
    public void Exit(AIAgent agent)
    {
        
    }
}
