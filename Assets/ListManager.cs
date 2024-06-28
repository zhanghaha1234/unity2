using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using System.Linq;
using System.Collections;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using UnityEditor.PackageManager.Requests;

public class ListManager : MonoBehaviour
{

    public GameObject blockButtonPrefab;
    public RectTransform blockButtonPanel;  // 按钮面板
    private GridLayoutGroup blockGridLayoutGroup;

    public GameObject projectButtonPrefab;  // 预制体按钮
    public RectTransform contentPanel;  // 用于动态生成按钮的面板

    private GridLayoutGroup gridLayoutGroup;  // GridLayoutGroup组件

    private string api;
    private string yqb;
    private ApiResponse response;

    void Start()
    {
        api = "http://localhost:8080/";
        yqb = "http://192.168.1.4:8080/dsCloudHallService/dsCloudHallScreen/";

        //初始化模块panel的GridLayoutGroup组件
        ConfigBlockGridLayout();

        // 初始化GridLayoutGroup组件
        ConfigGridLayoutGroup();

        //初始化项目列表数据
        StartCoroutine(GetProjectDatas());

    }

    //获取与处理请求数据
    IEnumerator GetProjectDatas()
    {
        string url = api + "getInitDatas";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        InitResponseData(request);

        //初始化区块按钮
        InitBlockButton();

        // 为每个按钮添加点击事件
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
        gridLayoutGroup.constraintCount = 2;  // 每行两个按钮  
        gridLayoutGroup.padding = new RectOffset(10, 10, 10, 10);  // 设置容器内边距

    }

    //设置response数据
    void InitResponseData(UnityWebRequest request)
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
                response = JsonUtility.FromJson<ApiResponse>(json);
                Debug.Log("Parsed response: " + response);
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

        List<Block> buttons = response.data;
        foreach (Block block in buttons) {
            {
                GameObject newBlockButton = Instantiate(blockButtonPrefab, blockButtonPanel);
                Text blockButtonText = newBlockButton.transform.Find("Text").GetComponent<Text>();
                blockButtonText.text = block.blockName;
                blockButtonText.GetComponent<RectTransform>().offsetMin = new Vector2(5, 5);

            }


        }
    }

    //模块按钮绑定事件
    void OnBlockButtonClick(string buttonText)
    {

        // 清空ContentPanel中的现有按钮
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // 获取对应按钮的数据列表
        List<Project> projects = GetProjects(buttonText);

        // 动态创建按钮
        foreach (Project project in projects)
        {
            GameObject newProjectButton = Instantiate(projectButtonPrefab, contentPanel);
            Text projectButtonText = newProjectButton.transform.Find("Text").GetComponent<Text>();
            projectButtonText.text = project.projectName;
            projectButtonText.GetComponent<RectTransform>().offsetMin = new Vector2(10, projectButtonText.GetComponent<RectTransform>().offsetMin.y);
            newProjectButton.GetComponent<Button>().onClick.AddListener(() => OnDynamicButtonClick());
        }
        //string complet_url = url + "block";
        string url = yqb + "block?message=block~";
        StartCoroutine(SendRequest(url));

    }

    List<Project> GetProjects(string blockName)
    {
        List<Block> blocks = response.data;
        foreach (Block block in blocks)
        {
            if (block.blockName == blockName)
                return block.projects;
        }
        return null;
    }
    


    void OnDynamicButtonClick()
    {
        string url = yqb + "project?message=project~";
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.SendWebRequest();
    }
}

