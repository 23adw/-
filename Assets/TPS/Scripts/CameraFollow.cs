using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform tr;
    private PhotonView pv;
    private ManagerInput ThirdPersonScend;
    void Awake()
    {
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();
        if (PhotonNetwork.IsConnected&&pv.IsMine)
        {
            SwitchMode();
        }
        else if (!PhotonNetwork.IsConnected)
        {
           SwitchMode();
        }
      
    }
    void SwitchMode()
    {
        ThirdPersonScend = GameObject.FindGameObjectWithTag("Player").GetComponent<ManagerInput>();
        GameObject Camera = GameObject.FindGameObjectWithTag("Camera");
        Camera.GetComponent<CinemachineVirtualCamera>().Follow = tr;
        GameObject AimCamera = GameObject.FindGameObjectWithTag("AimCamera");
        AimCamera.GetComponent<CinemachineVirtualCamera>().Follow = tr;
        if (ThirdPersonScend.Ctrl == true)
        {
            GameObject CameraCam = GameObject.FindGameObjectWithTag("test");
            tr.SetParent(CameraCam.transform);
            Debug.Log("111");
        }
    }
}
