using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RegionManeger : MonoBehaviour
{
    public Toggle overview;
    public RectTransform RegionPanel;
    public Toggle region1;
    public Toggle region2;
    public Toggle region3;
    public Toggle region4;
    public Toggle region5;

    public ScrollRect scrollRect;
    public Transform projectContainer;
    public Toggle projectPrefab;

    private string url;
    private string circleUrl;
    private RegionResponse regionResponse;
    private ProjectResponse projectResponse;
    private bool isOverView;
    private bool isRegion;

    private MyServices myservice = new MyServices();
    private RequestHandler requestHandler = new RequestHandler();

    private List<Region> regions;
    private List<Project> projects;
 
    void Start()
    {
        overview.onValueChanged.AddListener(OverviewChangedd);
        url = myservice.GetURL();
        circleUrl = myservice.GetCircleURL();
        InitRegionAndProject();
    }

    /*
     * ��ʼ���������Ŀ����
     */
    void InitRegionAndProject()
    {
        StartCoroutine(GetRegionsAndProjects());
    }

    /*
     * ���������ť���¼�
     */
    void OverviewChangedd(bool isOn)
    {
        if (isOn)
        {
            if (!isOverView)
            {
                StartCoroutine(GetRegionsAndProjects());
            }
            SimpleRequest(circleUrl + "notice?videoKey=overview");
            SimpleRequest(url + "dsCloudHallScreen/overview?eventType=1&bid");
        }
        else
        {
            isOverView = false;
        }
    }

    /*
     * ��ȡ�������������Ŀ����
     */
    IEnumerator GetRegionsAndProjects()
    {
        string regionRequestPath = url + "region/readRegion";
        UnityWebRequest regionRequest = UnityWebRequest.Get(regionRequestPath);
        yield return regionRequest.SendWebRequest();
        string regionRequestData = requestHandler.GetRequestText(regionRequest);
        regionResponse = JsonUtility.FromJson<RegionResponse>(regionRequestData);
        regions = regionResponse.data;
        InitRegionToggles(regions);

        string requestPath2 = url + "project/projectList?bid";
        UnityWebRequest projectRequest = UnityWebRequest.Get(requestPath2);
        yield return projectRequest.SendWebRequest();
        string projectRequestData = requestHandler.GetRequestText(projectRequest);
        projectResponse = JsonUtility.FromJson<ProjectResponse>(projectRequestData);
        projects = projectResponse.data.projectVoList;
        InitProjectToggles(projects);

        isOverView = true;
    }

    /*
     * Ϊ����������ӵ���¼�
     */
    void AddListenerOnRegionsClick()
    {
        foreach (Transform region in RegionPanel)
        {
            Toggle regionToggle = region.GetComponent<Toggle>();
            Text regionText = region.GetComponentInChildren<Text>();
            string regionCode = regionResponse.GetRegionCodeByName(regionText.text);
            regionToggle.onValueChanged.AddListener(delegate { OnRegionValueChanged(regionToggle, regionCode); });
        }
    }

    /*
     * Ϊ������Ŀ��ӵ���¼�
     */
    void AddListenerOnProjectsClick()
    {
        foreach (Transform project in projectContainer)
        {
            Toggle projectToggle = project.GetComponent<Toggle>();
            Text projectText = project.GetComponentInChildren<Text>();
            string id = projectResponse.GetRegionCodeByName(projectText.text);
            projectToggle.onValueChanged.AddListener(delegate { OnProjectValueChanged(projectToggle, id); });
        }
    }

    /*
     * �������鰴ť��ʾ
     */
    void InitRegionToggles(List<Region> regions)
    {
        Text toggleLabel;
        toggleLabel = region1.GetComponentInChildren<Text>();
        toggleLabel.text = regions[0].regionName;
        toggleLabel = region2.GetComponentInChildren<Text>();
        toggleLabel .text = regions[1].regionName;
        toggleLabel = region3.GetComponentInChildren<Text>();
        toggleLabel .text = regions[2].regionName;
        toggleLabel = region4.GetComponentInChildren<Text>();
        toggleLabel.text = regions[3].regionName;
        toggleLabel = region5.GetComponentInChildren<Text>();
        toggleLabel.text = regions[4].regionName;
        AddListenerOnRegionsClick();

    }

    /*
     * ���������������¼�
     */
    void OnRegionValueChanged(Toggle toggle, string regionCode)
    {
        if (toggle.isOn)
        {
            if (!isRegion)
            {
                StartCoroutine(GetProjectsByRegionCode(regionCode));
            }
            SimpleRequest(circleUrl + "notice?videoKey=" + regionCode);
            SimpleRequest(url + "dsCloudHallScreen/region?eventType=2&bid=" + regionCode);
        }
        else
        {
            isRegion = false;
        }
    }

    /*
     * ������Ŀ��ť��ʾ
     */
    void InitProjectToggles(List<Project> projects)
    {
        foreach (Transform project in projectContainer)
        {
            Destroy(project.gameObject);
        }

        ToggleGroup toggleGroup = projectContainer.GetComponent<ToggleGroup>();
        GridLayoutGroup gridLayoutGroup = projectContainer.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.cellSize = new Vector2(690, 72);
        gridLayoutGroup.spacing = new Vector2(60, 60);
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = 2;

        foreach (Project project in  projects)
        {
            Toggle projectToggle = Instantiate(projectPrefab, projectContainer);
            Text projectText = projectToggle.GetComponentInChildren<Text>();
            projectText.text = project.projName;
            projectToggle.group = toggleGroup;
        }
        AddListenerOnProjectsClick();
        scrollRect.verticalNormalizedPosition = 1;
    }

    /*
     * ��ȡ�����������Ŀ�б�
     */
    IEnumerator GetProjectsByRegionCode(string regionCode)
    {
        string projectRequestPath = url + "project/projectList?bid=" + regionCode;
        UnityWebRequest projectRequest = UnityWebRequest.Get(projectRequestPath);
        yield return projectRequest.SendWebRequest();
        string projectRequestData = requestHandler.GetRequestText(projectRequest);
        projectResponse = JsonUtility.FromJson<ProjectResponse>(projectRequestData);
        projects = projectResponse.data.projectVoList;
        InitProjectToggles(projects);
        isRegion = true;
    }

    /*
     * ������Ŀ����¼�
     */
    void OnProjectValueChanged(Toggle toggle, string id)
    {
        if (toggle.isOn)
        {
            SimpleRequest(url + "dsCloudHallScreen/project?eventType=3&bid=" + id);
        }
    }

    void SimpleRequest(string path)
    {
        StartCoroutine(SendSimpleRequest(path));
    }

    IEnumerator SendSimpleRequest(string path)
    {
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        requestHandler.GetRequestText(request);
    }

}
