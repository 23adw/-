using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using Random = UnityEngine.Random;

public class ThirdPersonController : MonoBehaviour
{
    #region Cinemachine
    [Header("Cinemachine")]
    [Tooltip("��Cinemachine Virtual Camera�����õĸ���Ŀ�꣬�����������")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("��������������ƶ���Զ")]
    public float TopClamp = 70.0f;

    [Tooltip("����԰���������ƶ���Զ")]
    public float BottomClamp = -30.0f;

    [Tooltip("��������������������ϵ�λ��")]
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
    private Animator animator;
    private Transform cameraTransform;
    private CharacterController characterController;
    private ManagerInput input;
    private Vector3 averageVel;
    private AudioSource playerSource;
    private float VerticalVelocity;//��ֱ�����ٶ�
    private float fallMultiplier = 1.5f;//�½��ı���
    private float gravity = -15f;//����
    private float maxHeight = 1.5f;//����Ծ�����߶�
    private int jumpint;
    private int horizontalHash;
    private int verticalHash;
    private int jumpSpeedHash;
    private int jumpHash;
    private int groundedHash;
    private PhotonView pv;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraTransform = Camera.main.transform;
            horizontalHash = Animator.StringToHash("Horizontal");
            verticalHash = Animator.StringToHash("Vertical");
            jumpSpeedHash = Animator.StringToHash("JumpSpeed");
            jumpHash = Animator.StringToHash("Jump");
            groundedHash = Animator.StringToHash("Grounded");
            input = GetComponent<ManagerInput>();
            playerSource = GetComponent<AudioSource>();
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        }
    }
void OnLand()
    {

    }
    void Update()
    {
        if (pv.IsMine)
        {
            CaculateGravity();
            Jump();
            PlayerMove();
            PlayerMoveSound();
        }
    }
    private void CameraRotation()
    {
        // ����������������λ�ò��̶�w
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
        // �н����ǵ���ת���������ǵ�ֵ������Ϊ360��
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
        // Cinemachine����ѭ���Ŀ��
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
    private void LateUpdate()
    {
        if (pv.IsMine)
        {
            CameraRotation();
            SetRecoil();
        }
    }

    void PlayerMove()
    {
        animator.SetFloat(horizontalHash, input.move.x, 0.2f, Time.deltaTime);//ÿ֡��Ŀ��ֵ��ֵ20%
        animator.SetFloat(verticalHash, input.move.y, 0.2f, Time.deltaTime);
        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;//��λ����
        if (input.move != Vector2.zero)
        {
            PlayerOffset(inputDirection, offset, offset2, offset3);
        }
    }
    void PlayerOffset(Vector3 inputdirection, float offset, float offset2, float offset3)
    {
        if (input.move.y < 0)
        {
            inputdirection = -inputdirection; //ȡ��
        }
        ////������뷽���� z ��ļн�,����������������ص�����ϵ��
        _targetRotation = Mathf.Atan2(inputdirection.x, inputdirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        #region ����ƫ����
        if (input.move.x > 0)
        {
            _targetRotation -= offset;
        }
        else if (input.move.x < 0)
        {
            _targetRotation += offset;
        }
        if (input.move.x > 0 && input.move.y < 0)
        {
            _targetRotation += offset2;
        }
        else if (input.move.x < 0 && input.move.y < 0)
        {
            _targetRotation += -offset2;
        }

        else if (input.move.x < 0 && input.move.y > 0)
        {
            _targetRotation += -offset3 - 15f;
        }
        else if (input.move.x > 0 && input.move.y > 0)
        {
            _targetRotation += offset3;
        }
        #endregion
        //ref:���������޸ĵ��÷��ṩ�ı���,ȷ��ƽ����ת��Ч��
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }
    
    void PlayerMoveSound()
    {
        if (characterController.isGrounded && input.move.sqrMagnitude > 0)//������ƽ������
        {
            playerSource.clip = run;
            if (!playerSource.isPlaying)
            {
                playerSource.Play();
            }
            if (playerSource.clip == run && playerSource.isPlaying && (input.aim || input.aiming))
            {
                playerSource.Pause();
            }
        }
        else if (!characterController.isGrounded || input.move.sqrMagnitude == 0)
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
            if (VerticalVelocity <= 0)//��ɫ�����½��׶�
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
        animator.SetBool(jumpHash, input.jump);
        animator.SetBool(groundedHash, characterController.isGrounded);
        animator.SetFloat(jumpSpeedHash, VerticalVelocity);
        if (characterController.isGrounded && input.jump && jumpint == 1)
        {
            VerticalVelocity = Mathf.Sqrt(-2 * gravity * maxHeight);//���¼�����ٶȸ߶�=������2gh
            //VerticalVelocity = jumpVelocity;
            playerSource.clip = jump;
            playerSource.Play();
            jumpint = 0;
            // Debug.Log("����");
        }
        else if (characterController.isGrounded && jumpint == 0)
        {
            playerSource.clip = fall;
            playerSource.Play();
            jumpint = 1;
            //Debug.Log("����");
        }
    }
    private void OnAnimatorMove()
    {
        if (pv.IsMine)
        {
            if (characterController.isGrounded)
            {
                Vector3 playerDeltaMovement = animator.deltaPosition;//deltaPosition����֡��Ӱ��
                playerDeltaMovement.y = VerticalVelocity * Time.deltaTime;//ȷ����ɫ�ڴ�ֱ�������ܹ���ȷ�ƶ�
                characterController.Move(playerDeltaMovement);//root motion����һ֡��λ��(������)
                //characterController.SimpleMove(animator.velocity);
                averageVel = Vector3.Lerp(averageVel, animator.velocity, 0.1f);
            }
            else
            {
                averageVel.y = VerticalVelocity;
                Vector3 playerDeltaMovement = averageVel * Time.deltaTime;
                characterController.Move(playerDeltaMovement);//root motion����һ֡��λ��(������)
            }
        }
    }
    private float initialCinemachineTargetPitch;
    private float recoilInterpolationSpeed = 5f; // ������Ҫ�����ٶ�
    private bool isFirstShot = true;
    private bool isRecovering = true;
    void SetRecoil()
    {
        if ((input.aim || input.aiming) && input.shoot)
        {
            Recoil(5);
        }
        else if (input.shoot)
        {
            Recoil(10);
        }
        if (!isFirstShot && !input.shoot)
        {
            if (initialCinemachineTargetPitch < _cinemachineTargetPitch)//ѹǹС�ڳ�ʼ��
            {
                initialCinemachineTargetPitch = _cinemachineTargetPitch;
            }

            isRecovering = true;
            _cinemachineTargetPitch = Mathf.Lerp(_cinemachineTargetPitch, initialCinemachineTargetPitch, recoilInterpolationSpeed * Time.deltaTime);
            if (ApproximatelyCustom(_cinemachineTargetPitch, initialCinemachineTargetPitch))
            {
                isFirstShot = true; // ���ﵽԭʼ����ʱֹͣ��������ֵ
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
                // ��¼��һ�ο�ǹʱ��λ��
                initialCinemachineTargetPitch = _cinemachineTargetPitch;
                isFirstShot = false;
                //Debug.Log("111111");
            }
            _cinemachineTargetPitch -= speed * Time.deltaTime;
        }
        //initialCinemachineTargetPitch -= speed * Time.deltaTime;
        // Debug.Log("initialCinemachineTargetPitch��" + initialCinemachineTargetPitch + "       _cinemachineTargetPitch:" + _cinemachineTargetPitch + "    relativePitchOffset��" + relativePitchOffset);
    }
}
