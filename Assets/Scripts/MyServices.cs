using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MyServices : MonoBehaviour
{
    private RegionResponse regionResponse;
    private ProjectResponse projectResponse;
    private string url;
    private string circleUrl;
    private ConfigData configData;

    public List<Region> GetRegions(UnityWebRequest request)
    {
        
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            ErrorMessage.Instance.ThrowErrorMessage("项目数据请求失败");
            ErrorMessage.Instance.ThrowRequestException(request);
            return null;
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
                return regionResponse.data;
            }
            catch (System.Exception e)
            {
                Debug.LogError("JSON parsing error: " + e.Message);
                ErrorMessage.Instance.ThrowErrorMessage("项目数据解析失败");
                ErrorMessage.Instance.ThrowRequestException(request);
                return null;
            }
        }
    }


    public List<Project> GetProjectss(UnityWebRequest request)
    {

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            ErrorMessage.Instance.ThrowErrorMessage("项目数据请求失败");
            ErrorMessage.Instance.ThrowRequestException(request);
            return null;
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
                return projectResponse.data.projectVoList; ;
            }
            catch (System.Exception e)
            {
                Debug.LogError("JSON parsing error: " + e.Message);
                ErrorMessage.Instance.ThrowErrorMessage("项目数据解析失败");
                ErrorMessage.Instance.ThrowRequestException(request);
                return null;
            }
        }
    }

    public string GetRegionIdByText(string regionName)
    {
        List<Region> regions = regionResponse.data;
        foreach (Region region in regions)
        {
            if (region.regionName == regionName)
                return region.regionCode;
        }
        return null;
    }
    public string GetProjectIdByText(string projectName)
    {
        List<Project> projects = projectResponse.data.projectVoList;
        foreach (Project project in projects)
        {
            if (project.projName == projectName)
                return project.id;
        }
        return null;
    }

    public string GetURL()
    {
        string appDirectory = Application.dataPath;
        string configPath = Path.Combine(appDirectory, "../config.json");
        string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
        configData = JsonUtility.FromJson<ConfigData>(jsonContent);
        url = configData.apiRootUrl + "/dsCloudHallService/";

        return url;
    }

    public string GetCircleURL()
    {
        string appDirectory = Application.dataPath;
        string configPath = Path.Combine(appDirectory, "../config.json");
        string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
        configData = JsonUtility.FromJson<ConfigData>(jsonContent);
        circleUrl = configData.apiRootUrl + "/dsCloudHallService/circularScreen/";
        return circleUrl;
    }

    public Font SetTextFont(Font defaultFont, Font backupFont, string text)
    {
        Font targetFont = defaultFont;
        foreach (char c in text)
        {
            if (!defaultFont.HasCharacter(c))
            {
                targetFont = backupFont;
            }
        }
        return targetFont;
    }



}
