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
        Debug.Log("链接到主服务器");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("进入大厅");
        RoomOptions roomOptions= new RoomOptions();
        roomOptions.MaxPlayers = 8;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
    public override void OnLeftLobby()
    {
        Debug.Log("离开大厅");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("加入房间");
        PhotonNetwork.LoadLevel(2);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("加入房间失败");
        Debug.Log("建立新房间");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
