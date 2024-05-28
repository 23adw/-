using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManagerInput : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool Ctrl;
    public bool aim;
    public bool shoot;
    public bool reload;
    public bool jump;
    public bool aiming;
    private bool ctrlPressedLastFrame = false;
    public void OnMove(InputValue value)//检测到 Move 输入时的回调方法
    {
        MoveInput(value.Get<Vector2>());//取输入的 Vector2 值
    }
    public void OnLook(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }
    public void OnCtrl(InputValue value)
    {
        CtrlInput(value.isPressed);
    }
    public void OnAim(InputValue value)
    {
        AimInput(value.isPressed);
    }
    public void OnShoot(InputValue value)
    {
        ShootInput(value.isPressed);
    }
    public void OnReload(InputValue value)
    {
        ReloadInput(value.isPressed);
    }
    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }
    public void OnAiming(InputValue value)
    {
        AimingInput(value.isPressed);
    }

    public void MoveInput(Vector2 newMoveDirction)
    {
        move = newMoveDirction;
    }
    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }
    public void CtrlInput(bool newCtrlState)
    {
        if (newCtrlState && !ctrlPressedLastFrame)
        {
            // 如果当前按下，且上一帧未按下，则切换 Ctrl 状态
            Ctrl = !Ctrl;
        }
        ctrlPressedLastFrame = newCtrlState;
    }
    public void AimInput(bool newAimState)
    {
        aim = newAimState;
    }
    public void ShootInput(bool newShootState)
    {
        shoot = newShootState;
    }
    public void ReloadInput(bool newReloadState)
    {
        reload = newReloadState;
    }
    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }
    public void AimingInput(bool newAimingState)
    {
        aiming = newAimingState;
    }
}


