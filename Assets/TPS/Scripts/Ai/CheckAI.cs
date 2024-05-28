using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAI : MonoBehaviour
{
    public GameObject aiPrefab; // AI Ԥ����
    public int maxAICount = 3; // ��� AI ����
    public float checkInterval = 2f; // �����
    void Start()
    {
        StartCoroutine(SpawnAIIfNeeded());
    }
    IEnumerator SpawnAIIfNeeded()
    {
        while (true) // ����ѭ��
        {
            yield return new WaitForSeconds(checkInterval);
            CheckSenceAI();
        }
    }

    void CheckSenceAI()
    {
        // ���ҳ��������е� AI
        AIAgent[] aiList = FindObjectsOfType<AIAgent>();

        // ��� AI ����С������������������µ� AI
        if (aiList.Length < maxAICount)
        {
            int clonesNeeded = maxAICount - aiList.Length;
            for (int i = 0; i < clonesNeeded; i++)
            {
                SpawnAI();
            }
        }
    }

    void SpawnAI()
    {
        // ��ȡ����λ��
        RespawnPoint spawnPoint = SetPoint();
        // �����µ� AI
        Instantiate(aiPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }

    RespawnPoint SetPoint()
    {
        RespawnPoint[] respawnPoints = GameObject.FindObjectsOfType<RespawnPoint>();
        RespawnPoint respawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];
        return respawnPoint;
    }
}
