using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class defaultWH : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetFullScreenMode();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetFullScreenMode()
    {
        // 获取最高分辨率
        Resolution[] resolutions = Screen.resolutions;
        Resolution highestResolution = resolutions[resolutions.Length - 1];

        // 设置全屏并使用最高分辨率
        Screen.SetResolution(highestResolution.width, highestResolution.height, FullScreenMode.FullScreenWindow);
    }
}
