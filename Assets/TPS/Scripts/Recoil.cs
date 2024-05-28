using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private PhotonView pv;
    private Vector3 recoil;
    public float addSpeed = 0.1f;//生成速度
    public float subSpeed = 1f;//恢复速度
    private Vector3 orginPosition;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            orginPosition = transform.localPosition;
            recoil = orginPosition;
        }
    }
    void Update()
    {
        if (pv.IsMine)
        {
            recoil.x = Mathf.MoveTowards(recoil.x, orginPosition.x, subSpeed * Time.deltaTime);//平滑回归初始值,无视帧率，以秒为单位
            recoil.z = Mathf.MoveTowards(recoil.z, orginPosition.z, subSpeed * Time.deltaTime);
            transform.localPosition = new Vector3(recoil.x, orginPosition.y, recoil.z);
        }
    }
    public void AddRecoil()
    {
        recoil.z -= addSpeed;//前后
        recoil.x += addSpeed;//左右
    }
}
