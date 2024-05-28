using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIWalkAndDetect : MonoBehaviour
{
    public float patrolWaitTime = 3f; // 在每个路径点停留的时间
    public float detectionRange = 5f; // 检测范围
    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    public bool isChasingPlayer;
    float timer = 0.0f;
    public WeaponIK weapon;
    public AIAgent agent;
    void Start()
    {
        agent = GetComponent<AIAgent>();
        weapon=GetComponent<WeaponIK>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // 假设玩家标签为"Player"
        isChasingPlayer = false;

        // 初始化巡逻目标为第一个巡逻点
        SetPatrolDestination();
    }

    void Update()
    {
        
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)//剩余距离大于停止距离
        {
            weapon.weight = 0;
        }
        else
        {
           weapon.weight = 1;
        }
        timer -= Time.deltaTime;
        if (isChasingPlayer)
        {
            if (timer < 0.0f)
            {
                navMeshAgent.SetDestination(playerTransform.position);
                timer = agent.config.maxTime;
            }
        }
        else if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            // 如果没有正在追逐玩家，并且已到达当前巡逻点，则等待一段时间后前往下一个巡逻点
            Invoke("SetPatrolDestination", patrolWaitTime);
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) < detectionRange)
        {
            // 如果玩家进入检测范围，则开始追逐玩家
            isChasingPlayer = true;
        }
    }
    RespawnPoint SetPoint()
    {
        RespawnPoint[] respawnPoints = GameObject.FindObjectsOfType<RespawnPoint>();
        RespawnPoint respawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];
        return respawnPoint;
    }
    // 设置巡逻目标为下一个巡逻点
    void SetPatrolDestination()
    {
        if (isChasingPlayer)
        {
            return;
        }
        RespawnPoint RandomPoint = SetPoint();
        navMeshAgent.SetDestination(RandomPoint.transform.position);
       // currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

}
