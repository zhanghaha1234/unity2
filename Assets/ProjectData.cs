using System.Collections.Generic;

[System.Serializable]
public class Project
{
    public int id;
    public string projectName;
}

[System.Serializable]
public class Block
{
    public string blockName;
    public List<Project> projects;
}

[System.Serializable]
public class ApiResponse
{
    public string status;
    public string message;
    public List<Block> data;
}
