using UnityEngine;
using UnityEngine.UI;

public class DisplayModeControl : MonoBehaviour
{
    public Dropdown displayModeDropdown;

    void Start()
    {
        // �������б��ֵ�ı��¼����ӵ�����
        displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
    }
    void OnDisplayModeChanged(int index)
    {
        // ����ѡ�������������ʾģʽ
        switch (index)
        {
            case 0: // Fullscreen
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1: // Windowed
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }
}
