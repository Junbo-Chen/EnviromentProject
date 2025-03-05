using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Object2DApiClient : MonoBehaviour
{
    public WebClient webClient;

    // Lees Object2D's op basis van environmentId
    public async Awaitable<IWebRequestReponse> ReadObject2Ds(string environmentId)
    {
        string route = "api/objects/environment/" + environmentId;  // Verander route om environment ID te gebruiken
        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);
        return ParseObject2DListResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateObject2D(Object2D object2D)
    {
        string token = PlayerPrefs.GetString("accessToken");
        webClient.SetToken(token);
        string route = "api/objects";
        string data = JsonUtility.ToJson(object2D);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequest(route, data);
        return ParseObject2DResponse(webRequestResponse);

        return ParseObject2DResponse(webRequestResponse);
    }

    // Werk een bestaand Object2D bij
    public async Awaitable<IWebRequestReponse> UpdateObject2D(Object2D object2D)
    {
        string route = "api/objects/" + object2D.id;  // Zorg ervoor dat ID en environmentId in de route staan
        string data = JsonUtility.ToJson(object2D);

        return await webClient.SendPutRequest(route, data);
    }

    private IWebRequestReponse ParseObject2DResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                Object2D object2D = JsonUtility.FromJson<Object2D>(data.Data);
                WebRequestData<Object2D> parsedWebRequestData = new WebRequestData<Object2D>(object2D);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseObject2DListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<Object2D> environments = JsonHelper.ParseJsonArray<Object2D>(data.Data);
                WebRequestData<List<Object2D>> parsedData = new WebRequestData<List<Object2D>>(environments);
                return parsedData;
            default:
                return webRequestResponse;
        }
    }
}
