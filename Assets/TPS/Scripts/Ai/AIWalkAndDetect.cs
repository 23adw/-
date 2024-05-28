using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIWalkAndDetect : MonoBehaviour
{
    public float patrolWaitTime = 3f; // ��ÿ��·����ͣ����ʱ��
    public float detectionRange = 5f; // ��ⷶΧ
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
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // ������ұ�ǩΪ"Player"
        isChasingPlayer = false;

        // ��ʼ��Ѳ��Ŀ��Ϊ��һ��Ѳ�ߵ�
        SetPatrolDestination();
    }

    void Update()
    {
        
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)//ʣ��������ֹͣ����
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
            // ���û������׷����ң������ѵ��ﵱǰѲ�ߵ㣬��ȴ�һ��ʱ���ǰ����һ��Ѳ�ߵ�
            Invoke("SetPatrolDestination", patrolWaitTime);
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) < detectionRange)
        {
            // �����ҽ����ⷶΧ����ʼ׷�����
            isChasingPlayer = true;
        }
    }
    RespawnPoint SetPoint()
    {
        RespawnPoint[] respawnPoints = GameObject.FindObjectsOfType<RespawnPoint>();
        RespawnPoint respawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];
        return respawnPoint;
    }
    // ����Ѳ��Ŀ��Ϊ��һ��Ѳ�ߵ�
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
