/*This script created by using docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
using System.Net.NetworkInformation;
using ExitGames.Client.Photon;

public class ParticleCollisionInstance : MonoBehaviourPun
{
    public GameObject[] EffectsOnCollision;//������ײʱ������Ч��
    public float DestroyTimeDelay = 5;
    public bool UseWorldSpacePosition;
    public float Offset = 0;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public bool useOnlyRotationOffset = true;//��ʹ����תƫ��
    public bool UseFirePointRotation;//��������ת
    public bool DestoyMainEffect = true;//�Ƿ�����
    private ParticleSystem part;//����ϵͳ���
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();//���ڴ洢������ײ�¼����б�


    public PhotonView ownerPhotonView; // �������ӵ�ʱ����ӵ����
    public PhotonView pv;
    public float damage = 10;
    public void SetOwner(PhotonView owner)
    {
        ownerPhotonView = owner;
    }
    void Awake()
    {
        part = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);//��ȡ������ϵͳ��ײ���¼�����
        for (int i = 0; i < numCollisionEvents; i++)//����ÿ���¼�
        {
            foreach (var effect in EffectsOnCollision)//������ײЧ��
            {
                var instance = Instantiate(effect, collisionEvents[i].intersection + collisionEvents[i].normal * Offset, new Quaternion()) as GameObject;//���¼��Ľ��㣬���ݷ��߷���ƫ��
                if (!UseWorldSpacePosition) instance.transform.parent = transform;
                if (UseFirePointRotation) { instance.transform.LookAt(transform.position); }
                else if (rotationOffset != Vector3.zero && useOnlyRotationOffset) { instance.transform.rotation = Quaternion.Euler(rotationOffset); }
                else
                {
                    instance.transform.LookAt(collisionEvents[i].intersection + collisionEvents[i].normal);
                    instance.transform.rotation *= Quaternion.Euler(rotationOffset);
                }
                Destroy(instance, DestroyTimeDelay);
            }
        }
        if (DestoyMainEffect == true)
        {
            Destroy(gameObject, DestroyTimeDelay + 0.5f);
        }
        if (other.tag == "Player")
        {
            if (PhotonNetwork.IsConnected)
            {
                if (ownerPhotonView != null && ownerPhotonView != other.GetComponent<PhotonView>())
                {
                    other.GetComponent<Damage>().DecreaseHealth(20);
                    if (other.GetComponent<Damage>().hpImg.fillAmount <= 0f)
                    {
                        AddScore();
                    }
                }
            }
            else
            {
                other.GetComponent<Damage>().DecreaseHealth(20);
            }
        }
        if (other.tag=="Enemy")
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            other.GetComponent<Health>().TakeDamage(10f,ray.direction);
            if (other.GetComponent<Health>().currentHealth <= 0.0f)
            {
                AddScore();
            }
        }
    }
    void AddScore()
    {
        if (PhotonNetwork.IsConnected)
        {
            var FindPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (var item in FindPlayers)
            {
                var player = item.GetComponent<Damage>();
                pv = item.GetComponent<PhotonView>();
                if (player != null)
                {
                    if (pv.ViewID == ownerPhotonView.ViewID)
                    {
                        player.AddScoreLocally();
                    }
                    if (pv.ViewID != ownerPhotonView.ViewID)
                    {
                        // Debug.Log($"�ܻ�����{pv.ViewID}�������ӵ�����{ownerPhotonView.ViewID}   {pv.ViewID}Ѫ��Ϊ0");
                    }
                }
            }
        }
        else
        {
            var FindPlayers = GameObject.FindGameObjectWithTag("Player");
            var player=FindPlayers.GetComponent<Damage>();
            player.AddScoreLocally();
        }
    }
}
