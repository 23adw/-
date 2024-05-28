using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("���ӵ���������");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("�������");
        RoomOptions roomOptions= new RoomOptions();
        roomOptions.MaxPlayers = 8;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
    public override void OnLeftLobby()
    {
        Debug.Log("�뿪����");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("���뷿��");
        PhotonNetwork.LoadLevel(2);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("���뷿��ʧ��");
        Debug.Log("�����·���");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
