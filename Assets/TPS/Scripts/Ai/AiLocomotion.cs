using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiLocomotion : MonoBehaviour
{
  
    NavMeshAgent agent;
    Animator animator;
    private int speedHash;
    void Start()
    {
      
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        speedHash = Animator.StringToHash("Speed");
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.hasPath)
        {
            animator.SetFloat(speedHash, agent.velocity.magnitude);
        }
        else
        {
            animator.SetFloat(speedHash,0);
        }
        
    }
}
