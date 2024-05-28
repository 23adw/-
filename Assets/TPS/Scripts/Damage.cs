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
    private float buffTime = 1f;//缓动时间
    private float maxScore = 1.0f; // 设置 Score 的最大值为1.0
    private float scoreIncrement = 0.1f; // 每次击杀增加的分数
    private float currentScore;
    private float timer = 0f;
    private float deleteInterval = 5f;//删除间隔
    public float maxShield = 50f; // 护盾的最大值
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
        // 计算添加子弹后的总数量
        int totalBullets = newbullet.bulletLeft + amount;
        // 如果超过上限，则将子弹数量设置为上限值
        if (totalBullets > newbullet.bulletMag * 5)
        {
            newbullet.bulletLeft = newbullet.bulletMag * 5;
        }
        else
        {
            // 否则正常增加子弹数量
            newbullet.bulletLeft += amount;
        }
        newbullet.UpdateAmmoUI();
    }
    public void DecreaseHealth(float amount)
    {
        if (currentShield > 0)
        {
            float remainingDamage = amount - currentShield;//护盾无法完全抵挡的部分
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
                // Debug.Log($"{pv.ViewID}被{bullet.ownerPhotonView.ViewID}玩家的子弹击中");
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
        
        if (player.activeSelf)//角色处于激活状态
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
        float effectLength = hpEffectImg.fillAmount - hpImg.fillAmount;//缓冲的血量
        float elapsedTime = 0f;//记录协程运行时间
        while (elapsedTime < buffTime && effectLength != 0)
        {
            elapsedTime += Time.deltaTime;
            //生命值条填充效果的目标值，即当前生命值条的值加上缓冲的血量,在1秒时间 buffTime 内的插值比例
            hpEffectImg.fillAmount = Mathf.Lerp(hpImg.fillAmount + effectLength, hpImg.fillAmount, elapsedTime / buffTime);
            yield return null;
        }
        hpEffectImg.fillAmount = hpImg.fillAmount;//确保最终填充效果与血条一致
    }
    RespawnPoint SetPoint()
    {
        RespawnPoint[] respawnPoints = GameObject.FindObjectsOfType<RespawnPoint>();
        RespawnPoint respawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];
        return respawnPoint;
    }
    void Respawn()
    {
        Debug.Log("复活");
        // 重新设置玩家位置
        RespawnPoint RandomPoint = SetPoint();
        transform.position = RandomPoint.transform.position;
        //transform.position = new Vector3(0f, 0f, 0f);
        player.SetActive(true);
        // 重置血量
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
        // 找到对应的content
        Transform targetContent = FindContentByName(content.name);
        newChildTransform.SetParent(targetContent);//默认在世界坐标
        // 设置kill_ui的文本
        Text[] playertext = newChildTransform.GetComponentsInChildren<Text>();
        #region 颜色判断
        //Color killerColor = new Color(93f / 255f, 110f / 255f, 132f / 255f, 1f);
        //Color killedColor = new Color(180f / 255f, 86f / 255f, 66f / 255f, 1f);
        //if (pv.IsMine)
        //{
        //    // 被击杀者
        //    playertext[0].color = killedColor;
        //    playertext[1].color = killerColor;
        //}
        //else if (pv.ViewID == killer)
        //{
        //    // 击杀者
        //    playertext[0].color = killerColor;
        //    playertext[1].color = killedColor;
        //}
        //else//现在是直接到这来了
        //{
        //    // 旁观者
        //    playertext[0].color = killedColor;
        //    playertext[1].color = killedColor;
        //}
        #endregion
        playertext[0].text = killer.ToString();
        playertext[1].text = killed.ToString();
    }
    Transform FindContentByName(string contentName)
    {
        // 返回对应content的Transform
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
                childObject.transform.DOKill();//清楚缓存
                layoutElement.ignoreLayout = true;
                float leftMoveTarget = 65f;
                float targetPositionX = 359f;
                rectTransform.DOAnchorPosX(leftMoveTarget, 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    // 在第一个动画完成后，向右移动
                    rectTransform.DOAnchorPosX(targetPositionX, 1f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        // 移动完成后将ignoreLayout设置为false
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
        // 显示胜利界面，结束游戏
        Debug.Log($"{pv.ViewID}游戏胜利！");
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