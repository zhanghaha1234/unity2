using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MyServices : MonoBehaviour
{
    private RegionResponse regionResponse;
    private ProjectResponse projectResponse;
    private string api;
    private string url;
    private string circleUrl;
    private ConfigData configData;

    /*
     * 获取大屏url
     */
    public string GetURL()
    {
        string appDirectory = Application.dataPath;
        string configPath = Path.Combine(appDirectory, "../config.json");
        string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
        configData = JsonUtility.FromJson<ConfigData>(jsonContent);
        api = configData.apiRootUrl;
        url = api + "/dsCloudHallService/";
        return url;
    }

    /*
     * 获取圆屏url
     */
    public string GetCircleURL()
    {
        circleUrl = api + "/dsCloudHallService/circularScreen/";
        return circleUrl;
    }

}
