using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using System.Net;

public class ListManager : MonoBehaviour
{

    public GameObject regionButtonPrefab;
    public RectTransform regionButtonPanel;  // 按钮面板
    private GridLayoutGroup regionGridLayoutGroup;

    public GameObject projectButtonPrefab;  // 预制体按钮
    public RectTransform projectButtonPanel;  // 用于动态生成按钮的面板

    private GridLayoutGroup projectGridLayoutGroup;  // GridLayoutGroup组件

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
        api = configData.api;
        circleApi = configData.circleApi;

        //初始化模块panel的GridLayoutGroup组件
        ConfigRegionGridLayout();

        // 初始化GridLayoutGroup组件
        ConfigGridLayoutGroup();

        //初始化项目列表数据
        StartCoroutine(InitDatas());

    }

    //获取与处理请求数据
    IEnumerator InitDatas()
    {
        string url = api + "readRegion";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        UpdateRegionResponseData(request);

        //初始化区块按钮
        InitRegionButton();

        // 为每个区块按钮添加点击事件
        foreach (Transform regionButton in regionButtonPanel)
        {
            Text regionText = regionButton.GetComponentInChildren<Text>();
            regionButton.GetComponent<Button>().onClick.AddListener(() => OnRegionButtonClick(regionText.text));
        }
        regionButtonPanel.GetChild(0).GetComponent<Button>().Select();
        regionButtonPanel.GetChild(0).GetComponent<Button>().onClick.Invoke();
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
    void OnRegionButtonClick(string buttonText)
    {

        // 清空ContentPanel中的现有按钮
        foreach (Transform child in projectButtonPanel)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(UpdateProjectList(buttonText));



    }


    IEnumerator UpdateProjectList(string buttonText)
    {
        StartCoroutine(ButtonRequest(api + "region?message=2"));

        string blockId = GetBlockIdByText(buttonText);
        string url = api + "readProjectByRegionId?regionId=" + blockId;
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
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
    }
    IEnumerator ButtonRequest(string url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
    }
}

