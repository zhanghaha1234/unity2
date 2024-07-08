using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class RegionManeger : MonoBehaviour
{
    public Toggle overview;
    public RectTransform RegionPanel;
    public Sprite selectedSprite;
    public Toggle region1;
    public Toggle region2;
    public Toggle region3;
    public Toggle region4;
    public Toggle region5;

    public ScrollRect scrollRect;
    public Transform projectContainer;
    public Toggle projectPrefab;

    public Font defaultFont; // 默认字体，包含所有字符
    public Font backupFont;  // 备用字体，用于显示默认字体无法显示的字符

    private string url;
    private string circleUrl;
    private RegionResponse regionResponse;
    private bool isOverView;

    private MyServices myservice = new MyServices();
    private List<Region> regions;
    private List<Project> projects;
    private List<string> circleTip = new List<string>() { "overview", "yysbsqly", "pdjcnc", "lcgkq", "xysdqk", "shng" };

    // Start is called before the first frame update
    void Start()
    {
        url = myservice.GetURL();
        circleUrl = myservice.GetCircleURL();

        overview.onValueChanged.AddListener(OverviewChangedd);
        InitRegionAndProject();


    }

    void InitRegionAndProject()
    {
        StartCoroutine(OverviewSelected());
    }

    void OverviewChangedd(bool isOn)
    {
        if (isOn)
        {
            if (!isOverView)
            {
                StartCoroutine(OverviewSelected());
            }
            SimpleRequest(circleUrl + "notice?videoKey=overview");
            SimpleRequest(url + "dsCloudHallScreen/overview?eventType=1&bid");
        }
        else
        {
            isOverView = false;
        }
    }

    IEnumerator OverviewSelected()
    {
        string requestPath1 = url + "region/readRegion";
        UnityWebRequest request1 = UnityWebRequest.Get(requestPath1);
        yield return request1.SendWebRequest();
        regions = myservice.GetRegions(request1);
        InitRegionToggles(regions);

        string requestPath2 = url + "project/projectList?bid";
        UnityWebRequest request2 = UnityWebRequest.Get(requestPath2);
        yield return request2.SendWebRequest();
        projects = myservice.GetProjectss(request2);
        InitProjectToggles(projects);

        isOverView = true;

    }


    void OnRegionsClick()
    {
        foreach (Transform region in RegionPanel)
        {
            Toggle regionToggle = region.GetComponent<Toggle>();
            Text regionText = region.GetComponentInChildren<Text>();
            string regionCode = myservice.GetRegionIdByText(regionText.text);
            regionToggle.onValueChanged.AddListener(delegate { OnRegionValueChanged(regionToggle, regionCode); });
        }
    }
    void OnProjectsClick()
    {
        foreach (Transform project in projectContainer)
        {
            Toggle projectToggle = project.GetComponent<Toggle>();
            Text projectText = project.GetComponentInChildren<Text>();
            string id = myservice.GetProjectIdByText(projectText.text);
            projectToggle.onValueChanged.AddListener(delegate { OnProjectValueChanged(projectToggle, id); });
        }
    }

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
        OnRegionsClick();


    }


    void OnRegionValueChanged(Toggle toggle, string regionCode)
    {
        if (toggle.isOn)
        {
            StartCoroutine(OnRegionButtonClick(regionCode));
            SimpleRequest(circleUrl + "notice?videoKey=" + regionCode);
            SimpleRequest(url + "dsCloudHallScreen/region?eventType=2&bid=" + regionCode);
        }
    }

    void InitProjectToggles(List<Project> projects)
    {
        foreach (Transform project in projectContainer)
        {
            Destroy(project.gameObject);
        }
        ToggleGroup toggleGroup = projectContainer.GetComponent<ToggleGroup>();
        GridLayoutGroup gridLayoutGroup = projectContainer.GetComponent<GridLayoutGroup>();

        gridLayoutGroup.cellSize = new Vector2(690, 72); // 根据需要调整
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
        OnProjectsClick();
        scrollRect.verticalNormalizedPosition = 1;
    }

    IEnumerator OnRegionButtonClick(string regionCode)
    {
        string requestPath = url + "project/projectList?bid=" + regionCode;
        UnityWebRequest request1 = UnityWebRequest.Get(requestPath);
        yield return request1.SendWebRequest();

        projects = myservice.GetProjectss(request1);
        InitProjectToggles(projects);
    }

    void OnProjectValueChanged(Toggle toggle, string id)
    {
        if (toggle.isOn)
        {
            StartCoroutine(OnProjectButtonClick(id));
            SimpleRequest(url + "dsCloudHallScreen/project?eventType=3&bid=" + id);
        }
    }

    IEnumerator OnProjectButtonClick(string projectId)
    {
        string requestPath = url + "project?bid=" + projectId;
        UnityWebRequest request = UnityWebRequest.Get(requestPath);
        yield return request.SendWebRequest();
    }

    void SimpleRequest(string path)
    {
        StartCoroutine(SendSimpleRequest(path));
    }

    IEnumerator SendSimpleRequest(string path)
    {
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
    }


}
