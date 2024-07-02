using UnityEngine;

public class FullscreenController : MonoBehaviour
{
    private void Start()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    void Update()
    {
        // 检测是否按下了Esc键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 切换全屏状态
            ToggleFullscreen();
        }
    }

    void ToggleFullscreen()
    {
        // 检查当前的全屏模式
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            // 切换到全屏模式
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            // 切换到窗口模式
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
