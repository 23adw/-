using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class TopDwonController : MonoBehaviour
{
    [Range(0, 1)]
    public float weight = 0;
    Animator animator;
    Vector2 playerInput;
    bool isRunning;
    Vector3 playermovement;
    public float rotatSpeed = 1000f;
    Transform playerTransform;

    float currentSpeed, targetSpeed, walkSpeed = 1.5f, runSpeed = 3.9f;
    bool armedRifle;
    //public Transform righHandPosition;
    //public Transform leftHandPosition;

    public GameObject rifleInhard;
    public GameObject rifleonback;

    public TwoBoneIKConstraint rightHandConstraint;
    public TwoBoneIKConstraint leftHandConstraint;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerTransform=transform;
    }

   
    void Update()
    {
        RotatePlayer();
        MovePlayer();
        SetTwoHandsWeight();
    }
    public void GetPlayerAim(InputAction.CallbackContext ctx)
    {
        if (ctx.action.phase==InputActionPhase.Started)
        {
            animator.SetBool("IsAiming", true);
        }
        else if (ctx.action.phase==InputActionPhase.Canceled)
        {
            animator.SetBool("IsAiming", false);
        }
    }
    public void GetPlayerMoveInput(InputAction.CallbackContext context)
    {
        playerInput=context.ReadValue<Vector2>();
    }
    public void GetPlayerRunInput(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValue<float>()>0?true:false;
    }
    void RotatePlayer()
    {
        if (playerInput.Equals(Vector2.zero))
            return;
        playermovement.x=playerInput.x;
        playermovement.z=playerInput.y;
        Quaternion tagerRotation = Quaternion.LookRotation(playermovement, Vector3.up);//向上旋转的位置
        playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, tagerRotation,rotatSpeed*Time.deltaTime);//旋转角色
    }

    void MovePlayer()
    {
        targetSpeed = isRunning ? runSpeed : walkSpeed;
        targetSpeed *= playerInput.magnitude;//摇杆和键盘推动幅度,松开为0，停止移动
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 0.5f);//在当前速度和目标速度之间线性插值
        animator.SetFloat("Vertical",currentSpeed);
    }
    public void GetArmedRifleInput(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>()==0)
        {
            armedRifle = !armedRifle;
            animator.SetBool("Rifle", armedRifle);
        }
    }
    public void PutGrabRifle(int riflePosition)
    {
        if (riflePosition==1)
        {
            rifleonback.SetActive(true);
            rifleInhard.SetActive(false);
        }
        else if (riflePosition == 0)
        {
            rifleonback.SetActive(false);
            rifleInhard.SetActive(true);
        }
    }
    void SetTwoHandsWeight()
    {
        rightHandConstraint.weight = animator.GetFloat("Right Hand Weight");
        leftHandConstraint.weight = animator.GetFloat("Left Hand Weight");
    }
    //private void OnAnimatorIK(int layerIndex)
    //{
    //    animator.SetIKPosition(AvatarIKGoal.RightHand, righHandPosition.position);
    //    animator.SetIKRotation(AvatarIKGoal.RightHand, righHandPosition.rotation);
    //    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition.position);
    //    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPosition.rotation);
    //    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
    //    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
    //    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
    //    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
    //}
}
