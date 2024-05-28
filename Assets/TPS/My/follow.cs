using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follow : MonoBehaviour
{
    public Transform target; // 子对象的Transform引用
    public Transform target2;
    void Update()
    {
        if (target != null)
        {
           
            transform.position = target.position;
            if (target.GetComponent<ManagerInput>().Ctrl == true)
            {
                transform.position = target2.position;
            }
        }
    }
}
