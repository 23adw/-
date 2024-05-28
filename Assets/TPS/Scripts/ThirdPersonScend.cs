using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;
using Input = UnityEngine.Input;

public class ThirdPersonScend : MonoBehaviourPun
{
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject ADS_Camera;
    public Transform fireTransform;
    public Transform casingSpawnTransform;
    public AudioSource mainAudioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip aimSound;
    public AudioClip reaimSound;
    public Canvas Canvas;
    public Image aimImage;
    public Text ammoTextUI;//当前子弹文本
    public Text shootModeTextUI;
    public Recoil recoil;
    public Rig aimRig;
    public SkinnedMeshRenderer characterRenderer;
    public float fireRate;//射速
    public int bulletMag;//当前武器的每个弹夹子弹数
    public int currentBullets;//当前子弹数
    public int bulletLeft;//备弹
    private PhotonView pv;
    public ManagerInput starterAssetsInputs;
    private Animator animator;
    private Transform aimPoint;
    private Camera mainCamera;
    private GameObject defaultCamera;
    private CinemachineVirtualCamera aimCamera;
    private Vector3 mouserWorldPosition;
    private Vector3 worldAimTarget;
    private Vector3 aimDirection;
    private bool isReload;
    private bool isADS;
    private bool switchMode;
    private float aimRigWeight;
    private float fireTimer;
    private float aimTimer;
    private int aimint;
    private int modelNum;
    private string shootModeName;
    private const float RaycastDistance = 1000f;
    private int aimHash;
    private int shootHash;
    private int reloadHash;
    private void Awake()
    {
       pv = GetComponent<PhotonView>();
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            SwitchNetwork();
        }
        else if(!PhotonNetwork.IsConnected)
        {
            SwitchNetwork();
        }
    }
    void SwitchNetwork()
    {
        starterAssetsInputs = GetComponent<ManagerInput>();
        animator = GetComponent<Animator>();
        Canvas.GetComponent<CanvasGroup>().alpha = 1;
        defaultCamera = GameObject.FindGameObjectWithTag("Camera");
        aimPoint = GameObject.FindGameObjectWithTag("AimPointTag").transform;
        aimCamera = GameObject.FindGameObjectWithTag("AimCamera").GetComponent<CinemachineVirtualCamera>();
        bulletLeft = bulletMag * 5;
        currentBullets = bulletMag;
        shootModeName = "全自动";
        UpdateAmmoUI();
        switchMode = true;
        aimTimer = 2f;
        mainCamera = Camera.main;
        aimHash = Animator.StringToHash("aim");
        shootHash = Animator.StringToHash("Shoot");
        reloadHash = Animator.StringToHash("reload");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraTransform = Camera.main.transform;
        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");
        jumpSpeedHash = Animator.StringToHash("JumpSpeed");
        jumpHash = Animator.StringToHash("Jump");
        groundedHash = Animator.StringToHash("Grounded");
        playerSource = GetComponent<AudioSource>();
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        characterController = GetComponent<CharacterController>();
    }
    void SetRay()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        aimPoint.position = mainCamera.transform.position + ray.direction * RaycastDistance;//目标点=起始点+方向*距离
        if (Physics.Raycast(ray, out RaycastHit raycastHit))//射线是否与物体相交，并存储信息
        {
            mouserWorldPosition = raycastHit.point;
        }
    }
    void IsAim(float LayerSpeed, float returnSpeed)
    {
        if (aimint != 1)
        {
            mainAudioSource.clip = aimSound;
            mainAudioSource.Play();
            aimint = 1;
        }
        animator.SetBool(aimHash, starterAssetsInputs.aim);
        animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1f, LayerSpeed * Time.deltaTime)); 
        worldAimTarget = aimPoint.position;
        worldAimTarget.y = transform.position.y;//y值不参与转换
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;//获取朝向世界目标的瞄准方向
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * returnSpeed);//(a + (b-a)×t)
        aimRigWeight = 1f;
        if (PhotonNetwork.IsConnected)
        {
            pv.RPC("SyncAimRigWeight", RpcTarget.All, aimRigWeight);
        }
        else
        {
            SyncAimRigWeight(aimRigWeight);
        }
        if (starterAssetsInputs.aiming)
        {
            animator.SetBool(aimHash, starterAssetsInputs.aiming);
        }
    }
    void ContinuousAiming()
    {
        animator.SetBool(aimHash, starterAssetsInputs.aim);
        animator.SetLayerWeight(2, 1f);
        worldAimTarget = aimPoint.position;
        worldAimTarget.y = transform.position.y;//y值不参与转换
        aimDirection = (worldAimTarget - transform.position).normalized;//获取朝向世界目标的瞄准方向
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        aimRigWeight = 1f;
        aimRig.weight = aimRigWeight;
    }
    void ClearAim()
    {
        if (aimint != 0)//使音频只播放一次
        {
            if (isReload)
            {
                mainAudioSource.clip = reloadSound;
            }
            else
            {
                mainAudioSource.clip = reaimSound;
            }
            mainAudioSource.Play();
            aimint = 0;
        }
        animator.SetBool(aimHash, starterAssetsInputs.aim);
        animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, 10f * Time.deltaTime));
        aimRigWeight = 0f;
        if (PhotonNetwork.IsConnected)
        {
            pv.RPC("SyncAimRigWeight", RpcTarget.All, aimRigWeight);
        }
        else
        {
            SyncAimRigWeight(aimRigWeight);
        }
        animator.SetBool(shootHash, starterAssetsInputs.shoot);
    }
    [PunRPC]
    void SyncAimRigWeight(float syncWeight)
    {
        aimRig.weight = Mathf.Lerp(aimRig.weight, syncWeight, Time.deltaTime * 10f);
    }
    public void GunState()
    {
        if (starterAssetsInputs.aim)
        {
            if (isReload || starterAssetsInputs.aiming)
            {
                starterAssetsInputs.aim = false;
                return;
            }
            IsAim(10f, 30f);
            aimCamera.gameObject.SetActive(true);
            if (starterAssetsInputs.aiming)
            {
                aimCamera.gameObject.SetActive(false);
                ADS();
            }
        }
        else if (!starterAssetsInputs.shoot && !starterAssetsInputs.aiming)
        {
            if (isReload)
            {
                aimTimer = 2f;
            }
            aimCamera.gameObject.SetActive(false);
            if (aimTimer < 2f)
            {
                ContinuousAiming();
            }
            else
            {
                ClearAim();
            }
        }
        if (starterAssetsInputs.shoot)
        {
            Shoot();
        }
    }
    void Shoot()
    {
        if (isReload)
        {
            starterAssetsInputs.shoot = false;
            return;
        }
        IsAim(30f, 40f);
        if (fireTimer < fireRate || currentBullets <= 0)//频率
        {
            return;
        }
        mainAudioSource.clip = shootSound;
        mainAudioSource.Play();
        Vector3 aimDir = (mouserWorldPosition - fireTransform.position).normalized;//向量长度为1
        if (PhotonNetwork.IsConnected)
        {
            pv.RPC("FireBullet", RpcTarget.All, fireTransform.position, aimDir);
        }
        else
        {
            FireBullet(fireTransform.position,aimDir);
        }
        aimTimer = 0f;
        fireTimer = 0f;//归零
        currentBullets--;
        UpdateAmmoUI();
        if (currentBullets == 0 && bulletLeft > 0)
        {
            isReload = true;
            starterAssetsInputs.shoot = false;
            Invoke("Rload", 2.11f);
            mainAudioSource.clip = reloadSound;
            mainAudioSource.Play();
            return;
        }
        animator.SetBool(shootHash, starterAssetsInputs.shoot);
        recoil.AddRecoil();
        starterAssetsInputs.shoot = switchMode;
    }
    [PunRPC]
    void FireBullet(Vector3 spawnPosition, Vector3 aimDirection)
    {
        GameObject Bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(aimDirection));//创造朝向aimdir方向的旋转
        Bullet.GetComponent<ParticleCollisionInstance>().SetOwner(pv);
        Instantiate(casingPrefab, casingSpawnTransform.position, casingSpawnTransform.rotation);
    }

    public void UpdateAmmoUI()
    {
        ammoTextUI.text = currentBullets + "/" + bulletLeft;
        shootModeTextUI.text = shootModeName;
    }
    public void Rload()
    {
        if (bulletLeft <= 0) return;
        int bulletToLoad = bulletMag - currentBullets;//计算需要填充的子弹
        int bulletToReduce = bulletLeft >= bulletToLoad ? bulletToLoad : bulletLeft;//需扣除的子弹
        bulletLeft -= bulletToReduce;//备弹减少
        currentBullets += bulletToReduce;//前置弹夹增加
        isReload = false;
        UpdateAmmoUI();
    }
    public void DoReLoadAnimation()
    {
        if (currentBullets > 0 && bulletLeft > 0)
        {
            isReload = true;
            Invoke("Rload", 2.11f);
        }
    }
    private void Animat()
    {
        animator.SetBool(reloadHash, isReload);
        aimTimer += Time.deltaTime;
    }
    void GunMode()
    {
        if (starterAssetsInputs.reload && currentBullets < bulletMag && bulletLeft > 0 && !isReload)
        {
            DoReLoadAnimation();
        }
        if (Input.GetKeyDown(KeyCode.X) && modelNum != 0)
        {
            shootModeName = "全自动";
            switchMode = true;
            UpdateAmmoUI();
            modelNum = 0;
        }
        else if (Input.GetKeyDown(KeyCode.X) && modelNum != 1)
        {
            shootModeName = "半自动";
            switchMode = false;
            UpdateAmmoUI();
            modelNum = 1;
        }
        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }
   public GameObject Play2D;

    void Update()
    {
       if (Time.timeScale != 0)    
       {
            if (PhotonNetwork.IsConnected && pv.IsMine)
            {
                SwitchNetwork2();
            }
            else if (!PhotonNetwork.IsConnected)
            {
                SwitchNetwork2();
            }
       }
    }
    void SwitchNetwork2()
    {
        // animator.SetBool("reload", IsReload);
        // aimrate += Time.deltaTime;
        Animat();
        SetRay();
        GunState();
        GunMode();
        CaculateGravity();
        Jump();
        PlayerMove();
        PlayerMoveSound();
        //Debug.Log(starterAssetsInputs.Ctrl);
        //if (starterAssetsInputs.Ctrl)
        //{
        //    if (Play2D == null)
        //    {
        //        Play2D = PhotonNetwork.Instantiate("Player2D", transform.position, transform.rotation);
        //    }
        //    else
        //    {
        //        Play2D.transform.position = transform.position;
        //        Play2D.transform.rotation = transform.rotation;
        //        Play2D.SetActive(true);
        //    }

        //    //   this.gameObject.SetActive(false);
        //    pv.RPC("SyncCtrlOperation", RpcTarget.All);

        //}
    }
    [PunRPC]
    void SyncCtrlOperation()
    {
            Play2D.GetComponent<ManagerInput>().move = starterAssetsInputs.move;
            Play2D.GetComponent<ManagerInput>().look = starterAssetsInputs.look;
            Play2D.GetComponent<Test>().g = gameObject;
            CinemachineCameraTarget.transform.SetParent(Play2D.transform);
            Canvas.transform.SetParent(Play2D.transform);
            Play2D.GetComponent<Test>().SetCamera(CinemachineCameraTarget);
            Play2D.GetComponent<Test>().SetCanvan(Canvas);
            starterAssetsInputs.Ctrl = false;
            this.gameObject.SetActive(false);

        // PhotonNetwork.Destroy(this.gameObject);

    }
    public GameObject Gun;
    public void SetCamera(GameObject c)
    {
        CinemachineCameraTarget = c;
    }
    private void LateUpdate()
    {
        if(Time.timeScale!=0)
        {
            if (PhotonNetwork.IsConnected && pv.IsMine)
            {
                SwitchNetwork3();
            }
            else if (!PhotonNetwork.IsConnected)
            {
                SwitchNetwork3();
            }
        }
    }
    void SwitchNetwork3()
    {
        CameraRotation();
        SetRecoil();
        if (starterAssetsInputs.aiming)//瞄准
        {
            ADS();
        }
        else//取消瞄准
        {
            characterRenderer.enabled = true;
            aimImage.enabled = true;
            ADS_Camera.SetActive(false);//相机2
            defaultCamera.SetActive(true);//相机1  
            isADS = false;
        }
    }
    void ADS()
    {
        if (isReload)
        {
            starterAssetsInputs.aiming = false;
            return;
        }
        aimImage.enabled = false;
        IsAim(10f, 40f);
        defaultCamera.SetActive(false);
        if (!isADS)
        {
            Invoke("Waitads", 0.001f);
        }
    }
    void Waitads()
    {
        ADS_Camera.SetActive(true);
        StartCoroutine(CharacterVisible(0.15f));
        isADS = true;
    }
    IEnumerator CharacterVisible(float time)
    {
        yield return new WaitForSeconds(time);
        characterRenderer.enabled = false;
    }
    #region 虚拟相机
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
    #endregion
    public AudioClip run;
    public AudioClip jump;
    public AudioClip fall;
    private Transform cameraTransform;
    private CharacterController characterController;
    private Vector3 averageVel;
    private AudioSource playerSource;
    private float VerticalVelocity;//下落力
    private float fallMultiplier = 1.5f;//下落倍率
    private float gravity = -15f;//重力
    private float maxHeight = 1.5f;//最大高度
    private int jumpint;
    private int horizontalHash;
    private int verticalHash;
    private int jumpSpeedHash;
    private int jumpHash;
    private int groundedHash;
    private void CameraRotation()
    {
        if (starterAssetsInputs.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            if (starterAssetsInputs.aiming)
            {
                _cinemachineTargetYaw += starterAssetsInputs.look.x * 0.1f;
                _cinemachineTargetPitch += -starterAssetsInputs.look.y * 0.1f;
            }
            else
            {
                _cinemachineTargetYaw += starterAssetsInputs.look.x * 0.2f;
                _cinemachineTargetPitch += -starterAssetsInputs.look.y * 0.2f;
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

    void PlayerMove()
    {
        animator.SetFloat(horizontalHash, starterAssetsInputs.move.x, 0.2f, Time.deltaTime);//???????????20%
        animator.SetFloat(verticalHash, starterAssetsInputs.move.y, 0.2f, Time.deltaTime);
        Vector3 inputDirection = new Vector3(starterAssetsInputs.move.x, 0.0f, starterAssetsInputs.move.y).normalized;//??λ????
        if (starterAssetsInputs.move != Vector2.zero)
        {
            PlayerOffset(inputDirection, offset, offset2, offset3);
        }
    }
    void PlayerOffset(Vector3 inputdirection, float offset, float offset2, float offset3)
    {
        if (starterAssetsInputs.move.y < 0)
        {
            inputdirection = -inputdirection; //???
        }
        ////??????????? z ???н?,?????????????????????????
        _targetRotation = Mathf.Atan2(inputdirection.x, inputdirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        #region Offset
        if (starterAssetsInputs.move.x > 0)
        {
            _targetRotation -= offset;
        }
        else if (starterAssetsInputs.move.x < 0)
        {
            _targetRotation += offset;
        }
        if (starterAssetsInputs.move.x > 0 && starterAssetsInputs.move.y < 0)
        {
            _targetRotation += offset2;
        }
        else if (starterAssetsInputs.move.x < 0 && starterAssetsInputs.move.y < 0)
        {
            _targetRotation += -offset2;
        }

        else if (starterAssetsInputs.move.x < 0 && starterAssetsInputs.move.y > 0)
        {
            _targetRotation += -offset3 - 15f;
        }
        else if (starterAssetsInputs.move.x > 0 && starterAssetsInputs.move.y > 0)
        {
            _targetRotation += offset3;
        }
        #endregion
        //ref:?????????????÷????????,???????????Ч??
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }
    void OnLand()
    { 
    }
    void PlayerMoveSound()
    {
        if (characterController.isGrounded && starterAssetsInputs.move.sqrMagnitude > 0)//?????????????
        {
            playerSource.clip = run;
            if (!playerSource.isPlaying)
            {
                playerSource.Play();
            }
            if (playerSource.clip == run && playerSource.isPlaying && (starterAssetsInputs.aim || starterAssetsInputs.aiming))
            {
                playerSource.Pause();
            }
        }
        else if (!characterController.isGrounded || starterAssetsInputs.move.sqrMagnitude == 0)
        {
            if (playerSource.clip == run)
            {
                playerSource.Pause();

                if (jumpint == 0 && !characterController.isGrounded)
                {
                    playerSource.clip = jump;
                    playerSource.Play();
                    jumpint = 1;
                }
            }
            if (jumpint == 1 && playerSource.clip == jump)
            {
                if (!playerSource.isPlaying)
                {
                    playerSource.clip = fall;
                    playerSource.Play();
                    jumpint = 0;
                }
            }
        }
    }
    void CaculateGravity()
    {
        if (characterController.isGrounded)
        {
            VerticalVelocity = gravity * Time.deltaTime;
            return;
        }
        else
        {
            if (VerticalVelocity <= 0)//?????????????
            {
                VerticalVelocity += gravity * fallMultiplier * Time.deltaTime;
            }
            else
            {
                VerticalVelocity += gravity * Time.deltaTime;
            }
        }
    }
    void Jump()
    {
        animator.SetBool(jumpHash, starterAssetsInputs.jump);
        animator.SetBool(groundedHash, characterController.isGrounded);
        animator.SetFloat(jumpSpeedHash, VerticalVelocity);
        if (characterController.isGrounded && starterAssetsInputs.jump && jumpint == 1)
        {
            VerticalVelocity = Mathf.Sqrt(-2 * gravity * maxHeight);//?????????????=??????2gh
            //VerticalVelocity = jumpVelocity;
            playerSource.clip = jump;
            playerSource.Play();
            jumpint = 0;
            // Debug.Log("????");
        }
        else if (characterController.isGrounded && jumpint == 0)
        {
            playerSource.clip = fall;
            playerSource.Play();
            jumpint = 1;
            //Debug.Log("????");
        }
    }
    private void OnAnimatorMove()
    {
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            SwitchNetwork4();
        }
        else if (!PhotonNetwork.IsConnected)
        {
            SwitchNetwork4();
        }
    }
    void SwitchNetwork4()
    {
        if (characterController.isGrounded)
        {
            Vector3 playerDeltaMovement = animator.deltaPosition;//deltaPosition??????????
            playerDeltaMovement.y = VerticalVelocity * Time.deltaTime;//?????????????????????????
            characterController.Move(playerDeltaMovement);//root motion????????λ??(??????)
                                                          //characterController.SimpleMove(animator.velocity);
            averageVel = Vector3.Lerp(averageVel, animator.velocity, 0.1f);
        }
        else
        {
            averageVel.y = VerticalVelocity;
            Vector3 playerDeltaMovement = averageVel * Time.deltaTime;
            characterController.Move(playerDeltaMovement);//root motion????????λ??(??????)
        }
    }
    private float initialCinemachineTargetPitch;
    private float recoilInterpolationSpeed = 5f; // ??????????????
    private bool isFirstShot = true;
    private bool isRecovering = true;
    void SetRecoil()
    {
        if ((starterAssetsInputs.aim || starterAssetsInputs.aiming) && starterAssetsInputs.shoot)
        {
            Recoil(5);
        }
        else if (starterAssetsInputs.shoot)
        {
            Recoil(10);
        }
        if (!isFirstShot && !starterAssetsInputs.shoot)
        {
            if (initialCinemachineTargetPitch < _cinemachineTargetPitch)//??С??????
            {
                initialCinemachineTargetPitch = _cinemachineTargetPitch;
            }

            isRecovering = true;
            _cinemachineTargetPitch = Mathf.Lerp(_cinemachineTargetPitch, initialCinemachineTargetPitch, recoilInterpolationSpeed * Time.deltaTime);
            if (ApproximatelyCustom(_cinemachineTargetPitch, initialCinemachineTargetPitch))
            {
                isFirstShot = true; // ??????????????????????
                isRecovering = false;
            }
        }
    }
    private bool ApproximatelyCustom(float a, float b, float epsilon = 0.1f)
    {
        return Mathf.Abs(a - b) < epsilon;
    }
    void Recoil(float speed)
    {
        var newbullet = GetComponent<ThirdPersonScend>();
        if (newbullet.currentBullets != 0)
        {
            if (isRecovering)
            {
                isFirstShot = true;
                isRecovering = false;
            }
            if (isFirstShot)
            {
                initialCinemachineTargetPitch = _cinemachineTargetPitch;
                isFirstShot = false;
                //Debug.Log("111111");
            }
            _cinemachineTargetPitch -= speed * Time.deltaTime;
        }
        //initialCinemachineTargetPitch -= speed * Time.deltaTime;
        // Debug.Log("initialCinemachineTargetPitch??" + initialCinemachineTargetPitch + "       _cinemachineTargetPitch:" + _cinemachineTargetPitch + "    relativePitchOffset??" + relativePitchOffset);
    }
}
