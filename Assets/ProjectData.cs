using JetBrains.Annotations;
using System.Collections.Generic;

[System.Serializable]
public class Project
{
    public string regionId;
    public string regionName;
    public string projectId;
    public string projectName;
}

[System.Serializable]
public class Region
{
    public string regionId;
    public string regionName;
}

[System.Serializable]
public class RegionResponse
{
    public string success;
    public string message;
    public List<Region> data;
}

[System.Serializable]
public class ProjectResponse
{
    public string success;
    public string message;
    public List<Project> data;
}


[System.Serializable]
public class ConfigData
{
    public string circleApi;
    public string api;
}