using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Test : MonoBehaviour
{
    private int horizontalHash;
    private int verticalHash;
    public Animator animator;
    private PhotonView ownerPv;
    private ManagerInput input;
    private CharacterController characterController;
    [Header("Cinemachine")]
    [Tooltip("??Cinemachine Virtual Camera????????????????????????")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("???????????????????")]
    public float TopClamp = 70.0f;

    [Tooltip("???????????????????")]
    public float BottomClamp = -30.0f;

    [Tooltip("??????????????????????λ??")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private const float _threshold = 0.01f;

    private float _targetRotation = 0.0f;
    private float _rotationVelocity = 0.1f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    public float offset;
    public float offset2;
    public float offset3;
    public void SetCamera(GameObject c)
    {
        CinemachineCameraTarget = c;
    }
    public void SetPv(int pv)
    {
        ownerPv.ViewID = pv;
    }
    void Start()
    {
        ownerPv = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        if (ownerPv.IsMine)
        {
            input = GetComponent<ManagerInput>();
           
            characterController = GetComponent<CharacterController>();
            horizontalHash = Animator.StringToHash("Horizontal");
            verticalHash = Animator.StringToHash("Vertical");
        }
    }
    private void CameraRotation()
    {
        // ????????????????λ?ò????w
        if (input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            if (input.aiming)
            {
                _cinemachineTargetYaw += input.look.x * 0.1f;
                _cinemachineTargetPitch += -input.look.y * 0.1f;
            }
            else
            {
                _cinemachineTargetYaw += input.look.x * 0.2f;
                _cinemachineTargetPitch += -input.look.y * 0.2f;
            }

        }
        // ?н????????????????????????????360??
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
        // Cinemachine???????????
        // Debug.Log("_cinemachineTargetPitch:"+_cinemachineTargetPitch + "      _cinemachineTargetYaw:" + _cinemachineTargetYaw);
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,
            _cinemachineTargetYaw, 0.0f);

    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void PlayerMove()
    {
        float horizontal = input.move.x;
        float vertical = input.move.y;
        animator.SetFloat(horizontalHash, horizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(verticalHash, vertical, 0.1f, Time.deltaTime);
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0.0f; // 忽略y轴方向的影响

        // 将移动方向转换为相对于相机的方向
        Vector3 movementDirection = Vector3.Normalize(horizontal * Camera.main.transform.right + vertical * cameraForward);

        if (movementDirection != Vector3.zero)
        {
            Vector3 inputdirection = new Vector3(horizontal, 0.0f, vertical).normalized;
            if (vertical < 0)
            {
                inputdirection = -inputdirection; //???
            }
            _targetRotation = Mathf.Atan2(inputdirection.x, inputdirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y + 90f;
            if (horizontal > 0)
            {
                _targetRotation -= offset;
            }
            else if (horizontal < 0)
            {
                _targetRotation += offset;
            }
            if (horizontal > 0 && vertical < 0)
            {
                _targetRotation += offset2;
            }
            else if (horizontal < 0 && vertical < 0)
            {
                _targetRotation += -offset2;
            }

            else if (horizontal < 0 && vertical > 0)
            {
                _targetRotation += -offset3 - 15f;
            }
            else if (horizontal > 0 && vertical > 0)
            {
                _targetRotation += offset3;
            }
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        }

        float speed = 3.0f;
        Vector3 movement = movementDirection * speed * Time.deltaTime;

        // 应用移动
        characterController.Move(movement);
    }

    void Update()
    {
        if (ownerPv.IsMine)
        {
            CameraRotation();
            PlayerMove();
            //if (input.Ctrl)
            //{
            //    animator.SetBool("Triggle", input.Ctrl);
            //    input.Ctrl = false;
            //    Invoke("a", 0.3997607f);
            //}
        }
       // Debug.Log(g);
    }
    public GameObject g;
    public Canvas Canvas;
    public void SetCanvan(Canvas c)
    {
        Canvas = c;
    }
    public void PlAYER(GameObject newplayer)
    {
        g = newplayer;
    }
    [PunRPC]
    void SetPlayerInfo(int viewID)
    {
        // 使用 PhotonNetwork.GetPhotonView 获取新的玩家对象
        g = PhotonNetwork.GetPhotonView(viewID).gameObject;
    }
    void a()
    {
        g.transform.position = transform.position;
       
        CinemachineCameraTarget.transform.SetParent(g.GetComponent<ThirdPersonScend>().Gun.transform);
        CinemachineCameraTarget.transform.localPosition = Vector3.zero;
        g.SetActive(true);
        ownerPv.RPC("ActivateTrue", RpcTarget.Others);
        Canvas.transform.SetParent(g.transform);
        g.GetComponent<ThirdPersonScend>().SetCamera(CinemachineCameraTarget);
        g.GetComponent<ManagerInput>().move = input.move;
        g.GetComponent<ManagerInput>().look = input.look;
        //this.gameObject.SetActive(false);
        ownerPv.RPC("ActivateFalse", RpcTarget.All);
        
        // PhotonNetwork.Destroy(this.gameObject);
    }
    [PunRPC]
    void ActivateTrue()
    {
        if (g != null)
        {
            g.SetActive(true);
        }
        else
        {
            Debug.Log("空了");
        }
    }
    [PunRPC]
    void ActivateFalse()
    {
      
        this.gameObject.SetActive(false);
     
    }
}