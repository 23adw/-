using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    public GameObject Play2D;
    private PhotonView pv;
    public Transform[] newPoints;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        SpawnPlayer();
    }
    void SpawnPlayer()
    {
        Transform spawnPoint = GetRandomSpawnPoint();
        playerPrefab = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
    }
    Transform GetRandomSpawnPoint()
    {
        // 随机选择一个新点
        int randomIndex = Random.Range(0, newPoints.Length);
        return newPoints[randomIndex];
    }
    public static int ctrl;
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}
        //if (PhotonNetwork.IsConnectedAndReady)
        //{
        //    if (Input.GetKeyDown(KeyCode.LeftControl) && ctrl != 1)
        //    {
        //        SetActiveState(false);
        //        if (Play2D == null)
        //        {
        //            Play2D = PhotonNetwork.Instantiate("Player2D", playerPrefab.transform.position, playerPrefab.transform.rotation);
        //        }
        //        else
        //        {
        //            Play2D.transform.position = playerPrefab.transform.position;
        //            Play2D.transform.rotation = playerPrefab.transform.rotation;
        //            Play2D.SetActive(true);
        //        }
        //        Play2D.GetComponent<ManagerInput>().move = playerPrefab.GetComponent<ThirdPersonScend>().starterAssetsInputs.move;
        //        Play2D.GetComponent<ManagerInput>().look = playerPrefab.GetComponent<ThirdPersonScend>().starterAssetsInputs.look;
        //        Play2D.GetComponent<Test>().g = playerPrefab.gameObject;
        //        playerPrefab.GetComponent<ThirdPersonScend>().CinemachineCameraTarget.transform.SetParent(Play2D.transform);
        //        playerPrefab.GetComponent<ThirdPersonScend>().Canvas.transform.SetParent(Play2D.transform);
        //        Play2D.GetComponent<Test>().SetCamera(playerPrefab.GetComponent<ThirdPersonScend>().CinemachineCameraTarget);
        //        Play2D.GetComponent<Test>().SetCanvan(playerPrefab.GetComponent<ThirdPersonScend>().Canvas);
        //        playerPrefab.GetComponent<ThirdPersonScend>().starterAssetsInputs.Ctrl = false;

        //            PhotonView photonView = playerPrefab.GetComponent<PhotonView>();
                    
        //            //if (PhotonNetwork.IsConnectedAndReady && !photonView.IsMine)
        //            //{
        //            //    photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        //            //    photonView.RPC("SetActiveState", RpcTarget.Others, false);
        //            //}



        //        ctrl = 1;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.LeftControl) && ctrl != 0)
        //    {
        //        Play2D.GetComponent<Test>().animator.SetBool("Triggle", true);
        //        Invoke("Test", 0.3997607f);
        //        ctrl = 0;
        //    }

        //}

    }
    [PunRPC]
    void SetActiveState(bool state)
    {
        playerPrefab.SetActive(state);
    }
    void Test()
    {
        playerPrefab.transform.position = Play2D.transform.position;

        Play2D.GetComponent<Test>().CinemachineCameraTarget.transform.SetParent(playerPrefab.GetComponent<ThirdPersonScend>().Gun.transform);
        Play2D.GetComponent<Test>().CinemachineCameraTarget.transform.localPosition = Vector3.zero;
        playerPrefab.SetActive(true);
        Play2D.GetComponent<Test>().Canvas.transform.SetParent(playerPrefab.transform);
        playerPrefab.GetComponent<ThirdPersonScend>().SetCamera(Play2D.GetComponent<Test>().CinemachineCameraTarget);
        playerPrefab.GetComponent<ManagerInput>().move = Play2D.GetComponent<ManagerInput>().move;
        playerPrefab.GetComponent<ManagerInput>().look = Play2D.GetComponent<ManagerInput>().look;
        Play2D.SetActive(false);

    }
}
