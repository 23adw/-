using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetectionVisualizer : MonoBehaviour
{
    public Color detectionColor = Color.red;
    public float detectionRange = 10f;

    private void OnDrawGizmosSelected()
    {
        // 设置绘制颜色
        Gizmos.color = detectionColor;

        // 绘制球体表示检测范围
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
