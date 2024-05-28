using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAI : MonoBehaviour
{
    public GameObject aiPrefab; // AI 预制体
    public int maxAICount = 3; // 最大 AI 数量
    public float checkInterval = 2f; // 检查间隔
    void Start()
    {
        StartCoroutine(SpawnAIIfNeeded());
    }
    IEnumerator SpawnAIIfNeeded()
    {
        while (true) // 无限循环
        {
            yield return new WaitForSeconds(checkInterval);
            CheckSenceAI();
        }
    }

    void CheckSenceAI()
    {
        // 查找场景中所有的 AI
        AIAgent[] aiList = FindObjectsOfType<AIAgent>();

        // 如果 AI 数量小于最大数量，则生成新的 AI
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
        // 获取生成位置
        RespawnPoint spawnPoint = SetPoint();
        // 生成新的 AI
        Instantiate(aiPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }

    RespawnPoint SetPoint()
    {
        RespawnPoint[] respawnPoints = GameObject.FindObjectsOfType<RespawnPoint>();
        RespawnPoint respawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];
        return respawnPoint;
    }
}
