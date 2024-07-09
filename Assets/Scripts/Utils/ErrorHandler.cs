using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ErrorHandler : MonoBehaviour
{
    public GameObject errorMessageComponent;
    private string errorLogPath;
    private bool isDisplaying = false;

    void Awake()
    {
        Application.logMessageReceived += HandleLog;
        errorLogPath = Path.Combine(Application.dataPath, "../error.log");
        errorMessageComponent.SetActive(false);
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    /*
     * ����unity�쳣��ʾ
     */
    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            Debug.Log($"From ErrorHandler:Error: {logString}\nStackTrace: {stackTrace}");
            SaveErrorToLogFile(logString, stackTrace);
            StartCoroutine(DisplayErrorMessage());
        }
    }

    /*
     * ���쳣��ջд��error.log
     */
    void SaveErrorToLogFile(string logString, string stackTrace)
    {
        string logMessage = $"\n\n{DateTime.Now}: {logString}\n{stackTrace}";
        if (!File.Exists(errorLogPath))
        {
            File.Create(errorLogPath).Dispose();
        }
        File.AppendAllText(errorLogPath, logMessage);
    }

    /*
     * �����쳣��ʾ
     */
    IEnumerator DisplayErrorMessage()
    {
        isDisplaying = true;
        errorMessageComponent.GetComponentInChildren<Text>().text = "���������쳣";
        errorMessageComponent.SetActive(true);
        yield return new WaitForSeconds(5f);
        errorMessageComponent.SetActive(false);
        isDisplaying = false;


    }

}
