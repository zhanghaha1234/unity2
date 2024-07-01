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
    public RectTransform blockButtonPanel;  // ��ť���
    private GridLayoutGroup blockGridLayoutGroup;

    public GameObject projectButtonPrefab;  // Ԥ���尴ť
    public RectTransform contentPanel;  // ���ڶ�̬���ɰ�ť�����

    private GridLayoutGroup gridLayoutGroup;  // GridLayoutGroup���

    private string api;
    private string yqb;
    private ApiResponse response;

    void Start()
    {
        api = "http://localhost:8080/";
        yqb = "http://192.168.1.4:8080/dsCloudHallService/dsCloudHallScreen/";

        //��ʼ��ģ��panel��GridLayoutGroup���
        ConfigBlockGridLayout();

        // ��ʼ��GridLayoutGroup���
        ConfigGridLayoutGroup();

        //��ʼ����Ŀ�б�����
        StartCoroutine(GetProjectDatas());

    }

    //��ȡ�봦����������
    IEnumerator GetProjectDatas()
    {
        string url = api + "getInitDatas";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        InitResponseData(request);

        //��ʼ�����鰴ť
        InitBlockButton();

        // Ϊÿ����ť��ӵ���¼�
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
        gridLayoutGroup.constraintCount = 2;  // ÿ��������ť  
        gridLayoutGroup.padding = new RectOffset(10, 10, 10, 10);  // ���������ڱ߾�

    }

    //����response����
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

    //��ʼ�����鰴ť��ʽ
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

    //ģ�鰴ť���¼�
    void OnBlockButtonClick(string buttonText)
    {

        // ���ContentPanel�е����а�ť
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // ��ȡ��Ӧ��ť�������б�
        List<Project> projects = GetProjects(buttonText);

        // ��̬������ť
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

