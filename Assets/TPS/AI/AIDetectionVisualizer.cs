using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetectionVisualizer : MonoBehaviour
{
    public Color detectionColor = Color.red;
    public float detectionRange = 10f;

    private void OnDrawGizmosSelected()
    {
        // ���û�����ɫ
        Gizmos.color = detectionColor;

        // ���������ʾ��ⷶΧ
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
