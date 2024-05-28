using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform muzzle; // ǹ��λ��
    public GameObject bulletPrefab; // �ӵ�Ԥ����
    public float bulletSpeed = 20f; // �ӵ��ٶ�
    public float shootInterval = 0.5f; // ����������λΪ��
    private float lastShootTime; // �ϴ������ʱ��
    public AudioSource audioSource;
    public AudioClip shootSound; 
    AIAgent AIAgent;    
    private void Start()
    {
        AIAgent = GetComponent<AIAgent>();
        lastShootTime = -shootInterval;//��ȷ���ܹ��������
    }
    public void Shoot(Transform target)
    {
        // ����Ƿ���������������
        if (Time.time - lastShootTime < shootInterval)
        {
            return;
        }
        lastShootTime = Time.time;
        Ray ray = new Ray(muzzle.position, (target.position - muzzle.position).normalized);
        // �������߼���������
        float maxRayDistance = AIAgent.config.DistanceMax;
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            if (hit.transform == target)
            {
                // ִ������߼�
                GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
                if (audioSource && shootSound != null)
                {
                    audioSource.PlayOneShot(shootSound);
                }
            }
            else
            {
                // �������δ������ң����ʾ���赲��޷����
                Debug.Log("Hit object name: " + hit.collider.gameObject.name);
            }
        }

        
    }
}
