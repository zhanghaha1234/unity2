using UnityEngine;

/*
 * ȫ���л�
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
            // �л�������ģʽ
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
