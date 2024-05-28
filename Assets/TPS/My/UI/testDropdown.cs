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
        // ʹ��HashSet�洢Ψһ�ķֱ���,List���ظ�
        uniqueResolutions = new HashSet<string>();
        resolutionDropdown.ClearOptions();
        foreach (Resolution resolution in allResolutions)
        {
            string option = resolution.width + " x " + resolution.height;

            // ���HashSet�в�������ͬ�ķֱ��ʣ�����ӵ������б��HashSet��
            if (!uniqueResolutions.Contains(option))
            {
                uniqueResolutions.Add(option);
            }
        }

        // ��ѡ����ӵ������б�
        resolutionDropdown.AddOptions(new List<string>(uniqueResolutions));
        // ����Ĭ��ѡ�еķֱ���Ϊ��߷ֱ���
        resolutionDropdown.value = resolutionDropdown.options.Count - 1;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(UpdateResolution);
    }

    void UpdateResolution(int index)
    {
        // �����������÷ֱ���
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
