using UnityEngine.Networking;

/*
 * request响应处理
 */
[System.Serializable]
public class RequestHandler 
{ 
    public string GetRequestText(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            throw new System.Exception($"无法从“{request.url}”获取返回值，{request.error}");
        }
        return request.downloadHandler.text;
    }

}
