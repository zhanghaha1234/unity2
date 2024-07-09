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
     * 捕获unity异常提示
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
     * 将异常堆栈写入error.log
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
     * 界面异常提示
     */
    IEnumerator DisplayErrorMessage()
    {
        isDisplaying = true;
        errorMessageComponent.GetComponentInChildren<Text>().text = "程序运行异常";
        errorMessageComponent.SetActive(true);
        yield return new WaitForSeconds(5f);
        errorMessageComponent.SetActive(false);
        isDisplaying = false;


    }

}
