using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class HumanBone
{
    public HumanBodyBones bone;
    public float weight = 1.0f;
}
public class WeaponIK : MonoBehaviour
{
    public Transform targetTransform;
    public Transform aimTransform;
    public int iterations = 10;
    [Range(0, 1)]
    public float weight = 1.0f;
    public HumanBone[] humanBones;
    Transform[] boneTransforms;
    public float angleLimit = 90.0f;
    public float distanceMinLimit = 1.5f;
    public float distanceMaxLimit = 10.0f;
    public float rotationSpeed = 5.0f;
    void Start()
    {
        var target = FindObjectOfType<DrawSphere>();
        SetTargetTransform(target.transform);
        Animator animator = GetComponent<Animator>();
        boneTransforms = new Transform[humanBones.Length];
        for (int i = 0; i < boneTransforms.Length; i++)
        {
            boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
        }
    }

    Vector3 GetTargetPosition()
    {
        Vector3 targetDirection=targetTransform.position-aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        float blendOut = 0.0f;
        float targetAngle=Vector3.Angle(targetDirection,aimDirection);
        if (targetAngle>angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50.0f;
        }
        float targetDistance=targetDirection.magnitude;
        if (targetDistance < distanceMinLimit)
        {
            blendOut += distanceMinLimit - targetDistance;
        }
        Vector3 direction=Vector3.Slerp(targetDirection,aimDirection, blendOut);
        return direction+aimTransform.position;
    }
    void LateUpdate()
    {
        if (targetTransform == null|| aimTransform == null)
        {
            return;
        }

        Vector3 targetPosition=GetTargetPosition();
        float distanceToTarget = Vector3.Distance(targetPosition, aimTransform.position);
        // 如果目标距离超出指定范围，则不执行瞄准逻辑
        if (distanceToTarget > distanceMaxLimit)
        {
            return;
        }
        Vector3 targetDirection = targetPosition - transform.position;
        targetDirection.y = 0; // 确保只在XZ平面旋转

        //// 旋转AI朝向目标方向
        //Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < boneTransforms.Length; j++)
            {
                Transform bone = boneTransforms[j];
                float boneWeight = humanBones[j].weight * weight;
                AimAtTarget(bone, targetPosition, boneWeight);
            }
        }
    }
    private void AimAtTarget(Transform bone, Vector3 targetPosition,float weight)
    {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection=targetPosition-aimTransform.position;
        Quaternion aimTowards=Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        
        bone.rotation = blendedRotation * bone.rotation;
    }
    public void SetTargetTransform(Transform target)
    {
        targetTransform = target;
    }
    public void SetAimTransform(Transform aim)
    {
        aimTransform = aim;
    }
}
