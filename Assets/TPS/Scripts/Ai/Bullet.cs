using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform muzzle; // 枪口位置
    public GameObject bulletPrefab; // 子弹预制体
    public float bulletSpeed = 20f; // 子弹速度
    public float shootInterval = 0.5f; // 射击间隔，单位为秒
    private float lastShootTime; // 上次射击的时间
    public AudioSource audioSource;
    public AudioClip shootSound; 
    AIAgent AIAgent;    
    private void Start()
    {
        AIAgent = GetComponent<AIAgent>();
        lastShootTime = -shootInterval;//以确保能够立即射击
    }
    public void Shoot(Transform target)
    {
        // 检查是否满足射击间隔条件
        if (Time.time - lastShootTime < shootInterval)
        {
            return;
        }
        lastShootTime = Time.time;
        Ray ray = new Ray(muzzle.position, (target.position - muzzle.position).normalized);
        // 设置射线检测的最大距离
        float maxRayDistance = AIAgent.config.DistanceMax;
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            if (hit.transform == target)
            {
                // 执行射击逻辑
                GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
                if (audioSource && shootSound != null)
                {
                    audioSource.PlayOneShot(shootSound);
                }
            }
            else
            {
                // 如果射线未击中玩家，则表示有阻挡物，无法射击
                Debug.Log("Hit object name: " + hit.collider.gameObject.name);
            }
        }

        
    }
}
