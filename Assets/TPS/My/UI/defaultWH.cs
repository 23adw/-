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
        // ��ȡ��߷ֱ���
        Resolution[] resolutions = Screen.resolutions;
        Resolution highestResolution = resolutions[resolutions.Length - 1];

        // ����ȫ����ʹ����߷ֱ���
        Screen.SetResolution(highestResolution.width, highestResolution.height, FullScreenMode.FullScreenWindow);
    }
}
