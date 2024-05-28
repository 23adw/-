using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    Animator animator;  
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        DeactivateRagdoll();
    }
    public void DeactivateRagdoll()
    {
        foreach (var item in rigidbodies)
        {
            item.isKinematic = true;
        }
        animator.enabled = true;
    }
    public void ActivatteRagdoll()
    {
        foreach (var item in rigidbodies)
        {
            item.isKinematic = false;
        }
        animator.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ApplyForce(Vector3 force)
    {
        var rigidBody=animator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
        rigidBody.AddForce(force,ForceMode.VelocityChange);
    }
}
