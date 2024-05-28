using DG.Tweening;
using Photon.Pun;
    using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
    using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Damage : MonoBehaviourPunCallbacks
{
    public Image hpImg;
    public Image hpEffectImg;
    public Image scoreImg;
    public Text hpText;
    public Text scoreText;
    public GameObject kill_ui;
    private GameObject content;
    private GameObject player;
    private PhotonView pv;
    private ParticleCollisionInstance bullet;
    private float maxHp = 100f;
    private float currentHp = 100f;
    private float buffTime = 1f;//����ʱ��
    private float maxScore = 1.0f; // ���� Score �����ֵΪ1.0
    private float scoreIncrement = 0.1f; // ÿ�λ�ɱ���ӵķ���
    private float currentScore;
    private float timer = 0f;
    private float deleteInterval = 5f;//ɾ�����
    public float maxShield = 50f; // ���ܵ����ֵ
    private float currentShield;
    public Image shieldImg;
    public Text shieldText;
    private ThirdPersonScend newbullet;
    void Start()
    {
        currentHp = maxHp;
        currentShield = maxShield;
        player = gameObject;
        pv = GetComponent<PhotonView>();
        newbullet=GetComponent<ThirdPersonScend>();
        content = GameObject.FindGameObjectWithTag("content");
        UpdateHealthBar();
        UpdateShieldBar();
    }
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Bullet")
        {
            bullet = other.GetComponent<ParticleCollisionInstance>();
        }
    }
    void GenerateCubeAtPlayerPosition()
    {
        if (PhotonNetwork.IsConnected&&pv.IsMine)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            GameObject cube = PhotonNetwork.Instantiate("CubePrefab", pos, Quaternion.identity);
        }
    }
    public void HandleCubeCollision()
    {
        if (currentShield < maxShield)
        {
            AddShield(30f);
        }
        if (newbullet.bulletLeft < newbullet.bulletMag * 5)
        {
            AddBullets(30);
        }
    }
    public void AddShield(float amount)
    {
        currentShield += amount;
        UpdateShieldBar();
    }
    public void AddBullets(int amount)
    {
        // ��������ӵ����������
        int totalBullets = newbullet.bulletLeft + amount;
        // ����������ޣ����ӵ���������Ϊ����ֵ
        if (totalBullets > newbullet.bulletMag * 5)
        {
            newbullet.bulletLeft = newbullet.bulletMag * 5;
        }
        else
        {
            // �������������ӵ�����
            newbullet.bulletLeft += amount;
        }
        newbullet.UpdateAmmoUI();
    }
    public void DecreaseHealth(float amount)
    {
        if (currentShield > 0)
        {
            float remainingDamage = amount - currentShield;//�����޷���ȫ�ֵ��Ĳ���
            currentShield = Mathf.Max(0f, currentShield - amount);
            amount = Mathf.Max(0f, remainingDamage);
            UpdateShieldBar();
        }
        SetHealth(currentHp - amount);
        if (currentHp <= 0f)
        {
            if (PhotonNetwork.IsConnected)
            {
                GenerateCubeAtPlayerPosition();
                player.GetComponent<CharacterController>().enabled = false;
                player.SetActive(false);
                Invoke("Respawn", 5f);
                // Debug.Log($"{pv.ViewID}��{bullet.ownerPhotonView.ViewID}��ҵ��ӵ�����");
                if (pv.IsMine)
                {
                    SetKillText(bullet.ownerPhotonView.ViewID, pv.ViewID);
                }
            }
            else
            {
                SceneManager.LoadSceneAsync(3);
            }
        }
    }
    void SetHealth(float health)
    {
        currentHp = Mathf.Clamp(health, 0f, maxHp);
        UpdateHealthBar();
       // Debug.Log($"Player{pv.ViewID}, health: {health}");
    }
    void UpdateHealthBar()
    {
        hpImg.fillAmount = currentHp / maxHp;
        
        if (player.activeSelf)//��ɫ���ڼ���״̬
        {
            StartCoroutine(UpdateHpEffect());
        }
        hpText.text = $"{hpImg.fillAmount * 100f}";

    }
    void UpdateShieldBar()
    {
        shieldImg.fillAmount = currentShield / maxShield;
        shieldText.text = $"{shieldImg.fillAmount * 50}";
    }
    IEnumerator UpdateHpEffect()
    {
        float effectLength = hpEffectImg.fillAmount - hpImg.fillAmount;//�����Ѫ��
        float elapsedTime = 0f;//��¼Э������ʱ��
        while (elapsedTime < buffTime && effectLength != 0)
        {
            elapsedTime += Time.deltaTime;
            //����ֵ�����Ч����Ŀ��ֵ������ǰ����ֵ����ֵ���ϻ����Ѫ��,��1��ʱ�� buffTime �ڵĲ�ֵ����
            hpEffectImg.fillAmount = Mathf.Lerp(hpImg.fillAmount + effectLength, hpImg.fillAmount, elapsedTime / buffTime);
            yield return null;
        }
        hpEffectImg.fillAmount = hpImg.fillAmount;//ȷ���������Ч����Ѫ��һ��
    }
    RespawnPoint SetPoint()
    {
        RespawnPoint[] respawnPoints = GameObject.FindObjectsOfType<RespawnPoint>();
        RespawnPoint respawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];
        return respawnPoint;
    }
    void Respawn()
    {
        Debug.Log("����");
        // �����������λ��
        RespawnPoint RandomPoint = SetPoint();
        transform.position = RandomPoint.transform.position;
        //transform.position = new Vector3(0f, 0f, 0f);
        player.SetActive(true);
        // ����Ѫ��
        SetHealth(maxHp);
        currentShield = maxShield;
        UpdateShieldBar();
        newbullet.currentBullets = 30;
        newbullet.bulletLeft = newbullet.currentBullets * 5;
        newbullet.UpdateAmmoUI();
        player.GetComponent<CharacterController>().enabled = true;
    }
    public void IncreaseHealth(float amount)
    {
        SetHealth(currentHp + amount);
    }
    void SetKillText(int killer, int killed)
    {
        pv.RPC("RPCSpawnKillUI", RpcTarget.All, killer, killed);
    }
    [PunRPC]
    void RPCSpawnKillUI(int killer, int killed)
    {
        GameObject newChild = Instantiate(kill_ui);
        Transform newChildTransform = newChild.transform;
        newChildTransform.localScale = Vector3.zero;
        newChildTransform.transform.DOScale(1f, 1f).SetEase(Ease.OutBounce);
        // �ҵ���Ӧ��content
        Transform targetContent = FindContentByName(content.name);
        newChildTransform.SetParent(targetContent);//Ĭ������������
        // ����kill_ui���ı�
        Text[] playertext = newChildTransform.GetComponentsInChildren<Text>();
        #region ��ɫ�ж�
        //Color killerColor = new Color(93f / 255f, 110f / 255f, 132f / 255f, 1f);
        //Color killedColor = new Color(180f / 255f, 86f / 255f, 66f / 255f, 1f);
        //if (pv.IsMine)
        //{
        //    // ����ɱ��
        //    playertext[0].color = killedColor;
        //    playertext[1].color = killerColor;
        //}
        //else if (pv.ViewID == killer)
        //{
        //    // ��ɱ��
        //    playertext[0].color = killerColor;
        //    playertext[1].color = killedColor;
        //}
        //else//������ֱ�ӵ�������
        //{
        //    // �Թ���
        //    playertext[0].color = killedColor;
        //    playertext[1].color = killedColor;
        //}
        #endregion
        playertext[0].text = killer.ToString();
        playertext[1].text = killed.ToString();
    }
    Transform FindContentByName(string contentName)
    {
        // ���ض�Ӧcontent��Transform
        GameObject foundContent = GameObject.Find(contentName);
        return foundContent.transform;
    }
    void Update()
    {
        if (PhotonNetwork.IsConnected&&pv.IsMine)
        {
            //Debug.Log(timer);
            CheckKillUi();
        }
       
    }
    void CheckKillUi()
    {
        if (content.transform.childCount != 0)
        {
                timer += Time.deltaTime;
            if (content.transform.childCount > 4|| timer > deleteInterval)
            {
                GameObject childObject = content.transform.GetChild(0).gameObject;
                LayoutElement layoutElement = childObject.GetComponent<LayoutElement>();
                RectTransform rectTransform = childObject.GetComponent<RectTransform>();
                childObject.transform.DOKill();//�������
                layoutElement.ignoreLayout = true;
                float leftMoveTarget = 65f;
                float targetPositionX = 359f;
                rectTransform.DOAnchorPosX(leftMoveTarget, 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    // �ڵ�һ��������ɺ������ƶ�
                    rectTransform.DOAnchorPosX(targetPositionX, 1f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        // �ƶ���ɺ�ignoreLayout����Ϊfalse
                        layoutElement.ignoreLayout = false;
                        Destroy(childObject);
                    });
                });
               timer = 0f;
            }
        }
    }
    public void AddScoreLocally()
    {
        currentScore = Mathf.Clamp(currentScore + scoreIncrement, 0f, maxScore);
        UpdateScore(currentScore);
        UpdateScoreText(currentScore, maxScore);
        if (currentScore >= maxScore)
        {
            if (PhotonNetwork.IsConnected)
            {
                pv.RPC("NotifyVictory", RpcTarget.All);
            }
            else
            {
                SceneManager.LoadSceneAsync(4);
            }
        }
    }
    void UpdateScore(float newScore)
    {
        scoreImg.fillAmount = newScore;
    }
    [PunRPC]
    public void UpdateScoreText(float newScore, float maxScore)
    {
        scoreText.text = (int)(newScore * 10) + "/" + maxScore * 10;
    }
    [PunRPC]
    void NotifyVictory()
    {
        // ��ʾʤ�����棬������Ϸ
        Debug.Log($"{pv.ViewID}��Ϸʤ����");
        if (pv.IsMine)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(4);
        }
        else
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(3);
        }
    }
}