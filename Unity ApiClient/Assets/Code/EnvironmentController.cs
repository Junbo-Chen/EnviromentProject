using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EnvironmentManager : MonoBehaviour
{
    [Header("API Reference")]
    public Environment2DApiClient environmentApiClient;

    [Header("UI References")]
    public Transform environmentsContainer;
    public GameObject environmentItemPrefab;

    private List<GameObject> spawnedItems = new List<GameObject>();

    private void Start()
    {
        LoadEnvironments();
    }

    private async void LoadEnvironments()
    {
        // Clear previous items
        foreach (var item in spawnedItems)
        {
            Destroy(item);
        }
        spawnedItems.Clear();

        // Fetch environments from API
        IWebRequestReponse response = await environmentApiClient.ReadEnvironment2Ds();

        if (response is WebRequestData<List<Environment2D>> data)
        {
            foreach (Environment2D environment in data.Data)
            {
                GameObject item = Instantiate(environmentItemPrefab, environmentsContainer);
                item.GetComponent<EnvironmentItemUI>().Initialize(environment.name, environment.id, () => DeleteEnvironment(environment.id));
                spawnedItems.Add(item);
            }
        }
        else if (response is WebRequestError error)
        {
            Debug.LogError($"Fout bij laden: {error.ErrorMessage}");
        }
    }

    private async void DeleteEnvironment(string environmentId)
    {
        IWebRequestReponse response = await environmentApiClient.DeleteEnvironment(environmentId);

        if (response is WebRequestData<string>)
        {
            Debug.Log("Verwijderd!");
            LoadEnvironments();
        }
        else if (response is WebRequestError error)
        {
            Debug.LogError($"Verwijderfout: {error.ErrorMessage}");
        }
    }

    public void GoToCreateEnvironment()
    {
        SceneManager.LoadScene("CreateEnvironment");
    }
}