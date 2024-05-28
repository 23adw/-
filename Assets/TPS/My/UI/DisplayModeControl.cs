using UnityEngine;
using UnityEngine.UI;

public class DisplayModeControl : MonoBehaviour
{
    public Dropdown displayModeDropdown;

    void Start()
    {
        // 将下拉列表的值改变事件连接到方法
        displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
    }
    void OnDisplayModeChanged(int index)
    {
        // 根据选择的索引设置显示模式
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
