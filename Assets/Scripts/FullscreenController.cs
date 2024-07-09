using UnityEngine;

/*
 * 全屏切换
 */
public class FullscreenController : MonoBehaviour
{
    private void Start()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleFullscreen();
        }
    }

    void ToggleFullscreen()
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            // 切换到窗口模式
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
