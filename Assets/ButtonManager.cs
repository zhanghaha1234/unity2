using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.Networking;
using System.Collections;

using System.Net;
using System.IO;
using System.Text;

public class ButtonManager : MonoBehaviour
{
    public Button playButton;
    public Button pauseButton;
    public Button replayButton;
    public Button overviewButton;
    private string api;
    private string circleApi;
    private ConfigData configData;

    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

        // 获取应用程序的根目录
        string appDirectory = Application.dataPath;
        string configPath = Path.Combine(appDirectory, "../config.json");
        string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
        configData = JsonUtility.FromJson<ConfigData>(jsonContent);
        api = configData.api;
        circleApi = configData.circleApi;


        playButton.onClick.AddListener(() => StartCoroutine(SendRequest(circleApi + "notice?action=1")));
        pauseButton.onClick.AddListener(() => StartCoroutine(SendRequest(circleApi + "notice?action=2")));
        replayButton.onClick.AddListener(() => StartCoroutine(SendRequest(circleApi + "notice?action=3")));
        overviewButton.onClick.AddListener(() => StartCoroutine(SendRequest(api + "overview?message=1")));
    }

    void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
    IEnumerator SendRequest(string url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
    }
}
