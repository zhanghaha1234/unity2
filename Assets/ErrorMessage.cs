using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
        string appDirectory = Application.dataPath;
        errorLogPath = Path.Combine(appDirectory, "../log.txt");
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
        yield return new WaitForSeconds(1f);  // ��ʾһ����
        errorComponent.SetActive(false);
        isDisplaying = false;
    }

    public void LogException(string message)
    {
        string logMessage = $"{DateTime.Now}: {message}\n";

        // ��������Ϣд����־�ļ�
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
            Debug.LogError($"�޷�д����־�ļ�: {fileEx.Message}");
        }
    }
}
