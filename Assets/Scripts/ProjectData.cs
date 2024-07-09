using System.Collections.Generic;

[System.Serializable]
public class Project
{
    public string regionCode;
    public string regionName;
    public string id;
    public string projName;
}

[System.Serializable]
public class Region
{
    public string regionCode;
    public string regionName;
}

[System.Serializable]
public class RegionResponse
{
    public string success;
    public string message;
    public List<Region> data;

    public string GetRegionCodeByName(string regionName)
    {
        foreach (Region region in data)
        {
            if (region.regionName == regionName)
                return region.regionCode; 
        }
        return null;
    }
}

[System.Serializable]
public class ProjectResponse
{
    public string success;
    public string message;
    public ProjectData data;

    public string GetRegionCodeByName(string projectName)
    {
        foreach (Project project in data.projectVoList)
        {
            if (project.projName == projectName)
                return project.id;
        }
        return null;
    }
}

[System.Serializable]
public class ConfigData
{
    public string apiRootUrl;
}

[System.Serializable]
public class ProjectData
{
    public List<Project> projectVoList;
}
