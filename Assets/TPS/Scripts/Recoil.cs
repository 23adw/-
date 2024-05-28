using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private PhotonView pv;
    private Vector3 recoil;
    public float addSpeed = 0.1f;//�����ٶ�
    public float subSpeed = 1f;//�ָ��ٶ�
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
            recoil.x = Mathf.MoveTowards(recoil.x, orginPosition.x, subSpeed * Time.deltaTime);//ƽ���ع��ʼֵ,����֡�ʣ�����Ϊ��λ
            recoil.z = Mathf.MoveTowards(recoil.z, orginPosition.z, subSpeed * Time.deltaTime);
            transform.localPosition = new Vector3(recoil.x, orginPosition.y, recoil.z);
        }
    }
    public void AddRecoil()
    {
        recoil.z -= addSpeed;//ǰ��
        recoil.x += addSpeed;//����
    }
}
