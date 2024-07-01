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

    public GameObject blockButtonPrefab;
    public RectTransform blockButtonPanel;  // 按钮面板
    private GridLayoutGroup blockGridLayoutGroup;

    public GameObject projectButtonPrefab;  // 预制体按钮
    public RectTransform contentPanel;  // 用于动态生成按钮的面板

    private GridLayoutGroup gridLayoutGroup;  // GridLayoutGroup组件

    private string api;
    private string circleApi;
    private BlockResponse blockResponse;
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
        ConfigBlockGridLayout();

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

        UpdateBlockResponseData(request);

        //初始化区块按钮
        InitBlockButton();

        // 为每个区块按钮添加点击事件
        foreach (Transform blockButton in blockButtonPanel)
        {
            Text buttonText = blockButton.GetComponentInChildren<Text>();
            blockButton.GetComponent<Button>().onClick.AddListener(() => OnBlockButtonClick(buttonText.text));
        }
        blockButtonPanel.GetChild(0).GetComponent<Button>().Select();
        blockButtonPanel.GetChild(0).GetComponent<Button>().onClick.Invoke();
    }

    //请求发送
    IEnumerator SendRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
    }

    //配置ConfigBlockGridLayout
    void ConfigBlockGridLayout()
    {
        blockGridLayoutGroup = blockButtonPanel.GetComponent<GridLayoutGroup>();
        if (blockGridLayoutGroup == null)
        {
            blockGridLayoutGroup = blockButtonPanel.gameObject.AddComponent<GridLayoutGroup>();
        }

        blockGridLayoutGroup.cellSize = new Vector2(110, 44);
        blockGridLayoutGroup.spacing = new Vector2(10, 10);
        blockGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        blockGridLayoutGroup.constraintCount = 5;
    }

    //配置gridLayoutGroup
    void ConfigGridLayoutGroup()
    {
        // 获取GridLayoutGroup组件
        gridLayoutGroup = contentPanel.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup == null)
        {
            gridLayoutGroup = contentPanel.gameObject.AddComponent<GridLayoutGroup>();
        }

        // 设置GridLayoutGroup的属性
        gridLayoutGroup.cellSize = new Vector2(285, 30);  // 设置每个按钮的大小
        gridLayoutGroup.spacing = new Vector2(10, 10);  // 设置按钮之间的间距
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        //gridLayoutGroup.constraintCount = 2;

        gridLayoutGroup.padding = new RectOffset(10, 10, 10, 10);  // 设置容器内边距

    }

    //设置response数据
    void UpdateBlockResponseData(UnityWebRequest request)
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
                blockResponse = JsonUtility.FromJson<BlockResponse>(json);
                Debug.Log("Parsed response: " + blockResponse);
            }
            catch (System.Exception e)
            {
                Debug.LogError("JSON parsing error: " + e.Message);
            }
        }
    }

    //初始化区块按钮样式
    void InitBlockButton()
    {
        foreach (Transform blockButton in blockButtonPanel)
        {
            Destroy(blockButton.gameObject);
        }

        List<Block> buttons = blockResponse.data;
        foreach (Block block in buttons) {
            {
                GameObject newBlockButton = Instantiate(blockButtonPrefab, blockButtonPanel);
                Text blockButtonText = newBlockButton.transform.Find("Text").GetComponent<Text>();
                blockButtonText.text = block.regionName;
                blockButtonText.GetComponent<RectTransform>().offsetMin = new Vector2(5, 5);

            }


        }
    }

    //区块按钮绑定事件
    void OnBlockButtonClick(string buttonText)
    {

        // 清空ContentPanel中的现有按钮
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(UpdateProjectList(buttonText));



    }


    IEnumerator UpdateProjectList(string buttonText)
    {
        StartCoroutine(ButtonRequest(api + "region?message=2"));

        string blockId = GetBlockIdByText(buttonText);
        string url = api + "readProjectByRegionId/?regionId=" + blockId;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        UpdataProjectResponseData(request);

        //初始化project按钮
        InitProjectButton();
    }

    //获取区块id
    string GetBlockIdByText(string blockName)
    {
        List<Block> blocks = blockResponse.data;
        foreach (Block block in blocks)
        {
            if(block.regionName == blockName)
                return block.regionId;
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
        foreach (Transform projectButton in contentPanel)
        {
            Destroy(projectButton.gameObject);
        }

        List<Project> projects = projectResponse.data;
        // 动态创建按钮
        foreach (Project project in projects)
        {
            GameObject newProjectButton = Instantiate(projectButtonPrefab, contentPanel);
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

