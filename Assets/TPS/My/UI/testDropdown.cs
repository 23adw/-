using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testDropdown : MonoBehaviour
{
    public Dropdown resolutionDropdown;

    private HashSet<string> uniqueResolutions;

    void Start()
    {
        Resolution[] allResolutions = Screen.resolutions;
        // 使用HashSet存储唯一的分辨率,List会重复
        uniqueResolutions = new HashSet<string>();
        resolutionDropdown.ClearOptions();
        foreach (Resolution resolution in allResolutions)
        {
            string option = resolution.width + " x " + resolution.height;

            // 如果HashSet中不存在相同的分辨率，则添加到下拉列表和HashSet中
            if (!uniqueResolutions.Contains(option))
            {
                uniqueResolutions.Add(option);
            }
        }

        // 将选项添加到下拉列表
        resolutionDropdown.AddOptions(new List<string>(uniqueResolutions));
        // 设置默认选中的分辨率为最高分辨率
        resolutionDropdown.value = resolutionDropdown.options.Count - 1;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(UpdateResolution);
    }

    void UpdateResolution(int index)
    {
        // 根据索引设置分辨率
        if (index >= 0 && index < Screen.resolutions.Length)
        {
            Resolution selectedResolution = Screen.resolutions[index];
            if (Screen.fullScreenMode==FullScreenMode.FullScreenWindow)
            {
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, FullScreenMode.FullScreenWindow);
            }
            else
            {
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, FullScreenMode.Windowed);
            }
        }
    }
}
