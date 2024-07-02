using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using System.Net;
using UnityEditor.PackageManager.Requests;

public class ListManager : MonoBehaviour
{

    public GameObject regionButtonPrefab;
    public RectTransform regionButtonPanel;  
    private GridLayoutGroup regionGridLayoutGroup;

    public GameObject projectButtonPrefab;  
    public RectTransform projectButtonPanel;  
    private GridLayoutGroup projectGridLayoutGroup;

    private string api;
    private string circleApi;
    private RegionResponse regionResponse;
    private ProjectResponse projectResponse;
    private ConfigData configData;
    private string selectedRegion;

    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

        // 获取应用程序的根目录
        string appDirectory = Application.dataPath;
        string configPath = Path.Combine(appDirectory, "../config.json");
        string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
        configData = JsonUtility.FromJson<ConfigData>(jsonContent);
        api = configData.apiRootUrl + "/dsCloudHallService/dsCloudHallScreen/";
        circleApi = configData.apiRootUrl + "/dsCloudHallService/circularScreen/";

        //初始化模块panel的GridLayoutGroup组件
        ConfigRegionGridLayout();

        // 初始化GridLayoutGroup组件
        ConfigGridLayoutGroup();



        //初始化项目列表数据
        StartCoroutine(InitDatas());
        InitEmptyRegions();

    }

    //获取与处理请求数据
    IEnumerator InitDatas()
    {
        string url = api + "readRegion";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        UpdateRegionResponseData(request);

        //初始化区块按钮
        if (regionResponse != null)
        {
            InitRegionButton();
        }

        // 为每个区块按钮添加点击事件
        foreach (Transform regionButton in regionButtonPanel)
        {
            Text regionText = regionButton.GetComponentInChildren<Text>();
            string regionId = GetBlockIdByText(regionText.text);
            regionButton.GetComponent<Button>().onClick.AddListener(() => OnRegionButtonClick(regionId));
        }
        regionButtonPanel.GetChild(5).GetComponent<Button>().Select();
        regionButtonPanel.GetChild(5).GetComponent<Button>().onClick.Invoke();
        //OnRegionButtonClick("1");
    }

    //请求发送
    IEnumerator SendRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
    }

    //配置ConfigBlockGridLayout
    void ConfigRegionGridLayout()
    {
        regionGridLayoutGroup = regionButtonPanel.GetComponent<GridLayoutGroup>();
        if (regionGridLayoutGroup == null)
        {
            regionGridLayoutGroup = regionButtonPanel.gameObject.AddComponent<GridLayoutGroup>();
        }

        regionGridLayoutGroup.cellSize = new Vector2(110, 44);
        regionGridLayoutGroup.spacing = new Vector2(10, 10);
        regionGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        regionGridLayoutGroup.constraintCount = 5;
    }

    //配置gridLayoutGroup
    void ConfigGridLayoutGroup()
    {
        // 获取GridLayoutGroup组件
        projectGridLayoutGroup = projectButtonPanel.GetComponent<GridLayoutGroup>();
        if (projectGridLayoutGroup == null)
        {
            projectGridLayoutGroup = projectButtonPanel.gameObject.AddComponent<GridLayoutGroup>();
        }

        // 设置GridLayoutGroup的属性
        projectGridLayoutGroup.cellSize = new Vector2(285, 30);  // 设置每个按钮的大小
        projectGridLayoutGroup.spacing = new Vector2(10, 10);  // 设置按钮之间的间距
        projectGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        //gridLayoutGroup.constraintCount = 2;

        projectGridLayoutGroup.padding = new RectOffset(10, 10, 10, 10);  // 设置容器内边距

    }

    //设置response数据
    void UpdateRegionResponseData(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            ErrorMessage.Instance.ThrowErrorMessage("区块数据请求失败");
            ErrorMessage.Instance.LogException(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("Received JSON: " + json);

            // Attempt to parse JSON
            try
            {
                regionResponse = JsonUtility.FromJson<RegionResponse>(json);
                Debug.Log("Parsed response: " + regionResponse);
            }
            catch (System.Exception e)
            {
                Debug.LogError("JSON parsing error: " + e.Message);
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + request.error);
                    ErrorMessage.Instance.ThrowErrorMessage("区块数据解析失败");
                    ErrorMessage.Instance.LogException(request.error);
                }
            }
        }
    }

    //初始化区块按钮样式
    void InitRegionButton()
    {
        foreach (Transform blockButton in regionButtonPanel)
        {
            Destroy(blockButton.gameObject);
        }

        List<Region> buttons = regionResponse.data;
        foreach (Region region in buttons) {
            {
                GameObject newRegionButton = Instantiate(regionButtonPrefab, regionButtonPanel);
                Text regionButtonText = newRegionButton.transform.Find("Text").GetComponent<Text>();
                regionButtonText.text = region.regionName;
                regionButtonText.GetComponent<RectTransform>().offsetMin = new Vector2(5, 5);

            }


        }
    }

    //区块按钮绑定事件
    void OnRegionButtonClick(string regionId)
    {

        // 清空ContentPanel中的现有按钮
        foreach (Transform child in projectButtonPanel)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(UpdateProjectList(regionId));



    }

    //更新项目列表
    IEnumerator UpdateProjectList(string regionId)
    {
        StartCoroutine(ButtonRequest(api + "region?message=2"));

        string url = api + "readProjectByRegionId?regionId=" + regionId;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        UpdataProjectResponseData(request);

        //初始化project按钮
        InitProjectButton();
    }

    //获取区块id
    string GetBlockIdByText(string regionName)
    {
        List<Region> regions = regionResponse.data;
        foreach (Region region in regions)
        {
            if(region.regionName == regionName)
                return region.regionId;
        }
        return null;
    }
    
    void UpdataProjectResponseData(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            ErrorMessage.Instance.ThrowErrorMessage("项目数据请求失败");
            ErrorMessage.Instance.LogException(request.error);

        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("Received JSON: " + json);

            // Attempt to parse JSON
            try
            {
                projectResponse = JsonUtility.FromJson<ProjectResponse>(json);
                Debug.Log("Parsed response: " + projectResponse);
            }
            catch (System.Exception e)
            {
                Debug.LogError("JSON parsing error: " + e.Message);
                ErrorMessage.Instance.ThrowErrorMessage("项目数据解析失败");
                ErrorMessage.Instance.LogException(request.error);
            }
        }
    }

    void InitProjectButton()
    {
        foreach (Transform projectButton in projectButtonPanel)
        {
            Destroy(projectButton.gameObject);
        }

        List<Project> projects = projectResponse.data;
        // 动态创建按钮
        foreach (Project project in projects)
        {
            GameObject newProjectButton = Instantiate(projectButtonPrefab, projectButtonPanel);
            Text projectButtonText = newProjectButton.transform.Find("Text").GetComponent<Text>();
            projectButtonText.text = project.projectName;
            projectButtonText.GetComponent<RectTransform>().offsetMin = new Vector2(10, projectButtonText.GetComponent<RectTransform>().offsetMin.y);
            newProjectButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OnProjectButtonClick(project.projectId)));
        }

    }

    IEnumerator OnProjectButtonClick(string projectId)
    {
        string url = api + "project?message=3";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            ErrorMessage.Instance.ThrowErrorMessage("区块数据请求失败");
            ErrorMessage.Instance.LogException(request.error);
        }
    }
    IEnumerator ButtonRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            ErrorMessage.Instance.ThrowErrorMessage("区块数据请求失败");
            ErrorMessage.Instance.LogException(request.error);
        }
    }

    void InitEmptyRegions()
    {
        for (int i = 0; i < 5; i += 1)
        {
            GameObject regionButton = Instantiate(regionButtonPrefab, regionButtonPanel);
            Text text = regionButton.transform.Find("Text").GetComponent<Text>();
            text.text = null;
        }
    }
}

