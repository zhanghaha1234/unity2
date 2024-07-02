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

        // ��ȡӦ�ó���ĸ�Ŀ¼
        string appDirectory = Application.dataPath;
        string configPath = Path.Combine(appDirectory, "../config.json");
        string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
        configData = JsonUtility.FromJson<ConfigData>(jsonContent);
        api = configData.apiRootUrl + "/dsCloudHallService/dsCloudHallScreen/";
        circleApi = configData.apiRootUrl + "/dsCloudHallService/circularScreen/";

        //��ʼ��ģ��panel��GridLayoutGroup���
        ConfigRegionGridLayout();

        // ��ʼ��GridLayoutGroup���
        ConfigGridLayoutGroup();



        //��ʼ����Ŀ�б�����
        StartCoroutine(InitDatas());
        InitEmptyRegions();

    }

    //��ȡ�봦����������
    IEnumerator InitDatas()
    {
        string url = api + "readRegion";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        UpdateRegionResponseData(request);

        //��ʼ�����鰴ť
        if (regionResponse != null)
        {
            InitRegionButton();
        }

        // Ϊÿ�����鰴ť��ӵ���¼�
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

    //������
    IEnumerator SendRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
    }

    //����ConfigBlockGridLayout
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

    //����gridLayoutGroup
    void ConfigGridLayoutGroup()
    {
        // ��ȡGridLayoutGroup���
        projectGridLayoutGroup = projectButtonPanel.GetComponent<GridLayoutGroup>();
        if (projectGridLayoutGroup == null)
        {
            projectGridLayoutGroup = projectButtonPanel.gameObject.AddComponent<GridLayoutGroup>();
        }

        // ����GridLayoutGroup������
        projectGridLayoutGroup.cellSize = new Vector2(285, 30);  // ����ÿ����ť�Ĵ�С
        projectGridLayoutGroup.spacing = new Vector2(10, 10);  // ���ð�ť֮��ļ��
        projectGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        //gridLayoutGroup.constraintCount = 2;

        projectGridLayoutGroup.padding = new RectOffset(10, 10, 10, 10);  // ���������ڱ߾�

    }

    //����response����
    void UpdateRegionResponseData(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            ErrorMessage.Instance.ThrowErrorMessage("������������ʧ��");
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
                    ErrorMessage.Instance.ThrowErrorMessage("�������ݽ���ʧ��");
                    ErrorMessage.Instance.LogException(request.error);
                }
            }
        }
    }

    //��ʼ�����鰴ť��ʽ
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

    //���鰴ť���¼�
    void OnRegionButtonClick(string regionId)
    {

        // ���ContentPanel�е����а�ť
        foreach (Transform child in projectButtonPanel)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(UpdateProjectList(regionId));



    }

    //������Ŀ�б�
    IEnumerator UpdateProjectList(string regionId)
    {
        StartCoroutine(ButtonRequest(api + "region?message=2"));

        string url = api + "readProjectByRegionId?regionId=" + regionId;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        UpdataProjectResponseData(request);

        //��ʼ��project��ť
        InitProjectButton();
    }

    //��ȡ����id
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
            ErrorMessage.Instance.ThrowErrorMessage("��Ŀ��������ʧ��");
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
                ErrorMessage.Instance.ThrowErrorMessage("��Ŀ���ݽ���ʧ��");
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
        // ��̬������ť
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
            ErrorMessage.Instance.ThrowErrorMessage("������������ʧ��");
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
            ErrorMessage.Instance.ThrowErrorMessage("������������ʧ��");
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

