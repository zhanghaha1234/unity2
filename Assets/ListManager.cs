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
    public RectTransform blockButtonPanel;  // ��ť���
    private GridLayoutGroup blockGridLayoutGroup;

    public GameObject projectButtonPrefab;  // Ԥ���尴ť
    public RectTransform contentPanel;  // ���ڶ�̬���ɰ�ť�����

    private GridLayoutGroup gridLayoutGroup;  // GridLayoutGroup���

    private string api;
    private string circleApi;
    private BlockResponse blockResponse;
    private ProjectResponse projectResponse;
    private ConfigData configData;
    private string selectedRegion;

    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

        // ��ȡӦ�ó���ĸ�Ŀ¼
        string appDirectory = Application.dataPath;
        string configPath = Path.Combine(appDirectory, "../config.json");
        string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
        configData = JsonUtility.FromJson<ConfigData>(jsonContent);
        api = configData.api;
        circleApi = configData.circleApi;

        //��ʼ��ģ��panel��GridLayoutGroup���
        ConfigBlockGridLayout();

        // ��ʼ��GridLayoutGroup���
        ConfigGridLayoutGroup();

        //��ʼ����Ŀ�б�����
        StartCoroutine(InitDatas());

    }

    //��ȡ�봦����������
    IEnumerator InitDatas()
    {
        string url = api + "readRegion";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        UpdateBlockResponseData(request);

        //��ʼ�����鰴ť
        InitBlockButton();

        // Ϊÿ�����鰴ť��ӵ���¼�
        foreach (Transform blockButton in blockButtonPanel)
        {
            Text buttonText = blockButton.GetComponentInChildren<Text>();
            blockButton.GetComponent<Button>().onClick.AddListener(() => OnBlockButtonClick(buttonText.text));
        }
        blockButtonPanel.GetChild(0).GetComponent<Button>().Select();
        blockButtonPanel.GetChild(0).GetComponent<Button>().onClick.Invoke();
    }

    //������
    IEnumerator SendRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
    }

    //����ConfigBlockGridLayout
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

    //����gridLayoutGroup
    void ConfigGridLayoutGroup()
    {
        // ��ȡGridLayoutGroup���
        gridLayoutGroup = contentPanel.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup == null)
        {
            gridLayoutGroup = contentPanel.gameObject.AddComponent<GridLayoutGroup>();
        }

        // ����GridLayoutGroup������
        gridLayoutGroup.cellSize = new Vector2(285, 30);  // ����ÿ����ť�Ĵ�С
        gridLayoutGroup.spacing = new Vector2(10, 10);  // ���ð�ť֮��ļ��
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        //gridLayoutGroup.constraintCount = 2;

        gridLayoutGroup.padding = new RectOffset(10, 10, 10, 10);  // ���������ڱ߾�

    }

    //����response����
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

    //��ʼ�����鰴ť��ʽ
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

    //���鰴ť���¼�
    void OnBlockButtonClick(string buttonText)
    {

        // ���ContentPanel�е����а�ť
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

        //��ʼ��project��ť
        InitProjectButton();
    }

    //��ȡ����id
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
        // ��̬������ť
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

