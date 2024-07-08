using UnityEngine;

public class FullscreenController : MonoBehaviour
{
    private void Start()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    void Update()
    {
        // ����Ƿ�����Esc��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �л�ȫ��״̬
            ToggleFullscreen();
        }
    }

    void ToggleFullscreen()
    {
        // ��鵱ǰ��ȫ��ģʽ
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            // �л���ȫ��ģʽ
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            // �л�������ģʽ
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
