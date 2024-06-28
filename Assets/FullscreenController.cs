using UnityEngine;

public class FullscreenController : MonoBehaviour
{
    void Update()
    {
        // 检查用户是否按下了ESC键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 切换屏幕显示模式
            Screen.fullScreen = !Screen.fullScreen;

            // 如果你想在退出全屏时也关闭游戏，可以在这里添加Application.Quit();
        }
    }
}
