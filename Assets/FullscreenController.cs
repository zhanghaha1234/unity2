using UnityEngine;

public class FullscreenController : MonoBehaviour
{
    void Update()
    {
        // ����û��Ƿ�����ESC��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �л���Ļ��ʾģʽ
            Screen.fullScreen = !Screen.fullScreen;

            // ����������˳�ȫ��ʱҲ�ر���Ϸ���������������Application.Quit();
        }
    }
}
