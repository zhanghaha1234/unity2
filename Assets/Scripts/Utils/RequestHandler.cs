using UnityEngine.Networking;

/*
 * request��Ӧ����
 */
[System.Serializable]
public class RequestHandler 
{ 
    public string GetRequestText(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            throw new System.Exception($"�޷��ӡ�{request.url}����ȡ����ֵ��{request.error}");
        }
        return request.downloadHandler.text;
    }

}
