using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.Networking;
using System.Collections;

using System.Net;
using System.Security.Cryptography.X509Certificates;

public class ButtonManager : MonoBehaviour
{
    public Button playButton;
    public Button pauseButton;
    public Button replayButton;
    public Button overviewButton;
    private string api;

    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

        api = "http://localhost:8080/";
        string a = "http://192.168.1.4:8080/dsCloudHallService/dsCloudHallScreen/regionOverview?message=lisi";

        playButton.onClick.AddListener(() => StartCoroutine(SendRequest(api + "play")));
        pauseButton.onClick.AddListener(() => StartCoroutine(SendRequest(api + "pause")));
        replayButton.onClick.AddListener(() => StartCoroutine(SendRequest(api + "replay")));
        overviewButton.onClick.AddListener(() => StartCoroutine(SendRequest(a)));
    }

    void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
    IEnumerator SendRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
    }
}
