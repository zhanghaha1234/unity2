using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour
{
    public static ErrorMessage Instance { get; private set; }

    public GameObject errorComponent;
    private bool isDisplaying = false;
    private string errorLogPath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        errorComponent.SetActive(false);
        
    }

    public void ThrowErrorMessage(string message)
    {
        if (!isDisplaying)
        {
            StartCoroutine(DisplayErrorMessage(message));
        }
    }

    private IEnumerator DisplayErrorMessage(string message)
    {

        isDisplaying = true;
        errorComponent.GetComponentInChildren<Text>().text = message;
        errorComponent.SetActive(true);
        yield return new WaitForSeconds(1f);  // 显示一秒钟
        errorComponent.SetActive(false);
        isDisplaying = false;


    }

    public void LogException(Exception ex)
    {
        string appDirectory = Application.dataPath;
        errorLogPath = Path.Combine(appDirectory, "../error.log");
        string logMessage = $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n";

        // 将错误消息写入日志文件
        try
        {
            if (!File.Exists(errorLogPath))
            {
                File.Create(errorLogPath).Dispose();
            }
            File.AppendAllText(errorLogPath, logMessage);
        }
        catch (Exception fileEx)
        {
            Debug.LogError($"无法写入日志文件: {fileEx.Message}");
            LogException(fileEx);
        }
    }

    public void Test(string a)
    {
        string appDirectory = Application.dataPath;
        errorLogPath = Path.Combine(appDirectory, "../error.log");
        string logMessage = $"{DateTime.Now}: {a}\n";

        // 将错误消息写入日志文件
        try
        {
            if (!File.Exists(errorLogPath))
            {
                File.Create(errorLogPath).Dispose();
            }
            File.AppendAllText(errorLogPath, logMessage);
        }
        catch (Exception fileEx)
        {
            Debug.LogError($"无法写入日志文件: {fileEx.Message}");
            LogException(fileEx);
        }
    }

    public void ThrowRequestException(UnityWebRequest request)
    {
        string errorMessage = request.error;
        long responseCode = request.responseCode;
        Exception exception = new Exception($"Network Error: {errorMessage}, Response Code: {responseCode}");
        LogException(exception);
    }
}
