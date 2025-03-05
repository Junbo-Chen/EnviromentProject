using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;

public class WorldObjectSpawner : MonoBehaviour
{
    public GameObject eyePrefab;
    public GameObject goldApplePrefab;
    public GameObject lavaPrefab;

    public Button eyeButton;
    public Button goldAppleButton;
    public Button lavaButton;

    public Transform gridContainer;
    public Object2DApiClient apiClient;
    public WebClient webClient;

    private Dictionary<string, GameObject> prefabDictionary;

    private void Start()
    {
        // Initialize the prefab dictionary
        prefabDictionary = new Dictionary<string, GameObject>
        {
            { eyePrefab.name, eyePrefab },
            { goldApplePrefab.name, goldApplePrefab },
            { lavaPrefab.name, lavaPrefab }
        };

        eyeButton.onClick.AddListener(() => SpawnItem(eyePrefab));
        goldAppleButton.onClick.AddListener(() => SpawnItem(goldApplePrefab));
        lavaButton.onClick.AddListener(() => SpawnItem(lavaPrefab));

        // Load existing objects when the scene starts
        LoadExistingObjects();
    }

    private async void LoadExistingObjects()
    {
        string environmentId = PlayerPrefs.GetString("environmentId");
        string token = PlayerPrefs.GetString("accessToken");
        webClient.SetToken(token);
        var response = await apiClient.ReadObject2Ds(environmentId);

        if (response is WebRequestData<List<Object2D>> data)
        {
            foreach (Object2D obj in data.Data)
            {
                Transform cell = FindCell(obj.positionX, obj.positionY);
                if (cell != null && cell.childCount == 0)
                {
                    SpawnObjectInCell(obj, cell);
                }
            }
        }
    }

    private void SpawnObjectInCell(Object2D objData, Transform cell)
    {
        if (prefabDictionary.TryGetValue(objData.prefabId, out GameObject prefab))
        {
            GameObject obj = Instantiate(prefab, cell);
            obj.transform.localPosition = Vector3.zero;

            // Voeg direct het Object2DDataHolder component toe voordat Awake wordt aangeroepen
            var dataHolder = obj.GetComponent<Object2DDataHolder>();
            if (dataHolder == null)
            {
                dataHolder = obj.AddComponent<Object2DDataHolder>();
                dataHolder.Data = objData; // Zorg ervoor dat de data meteen wordt toegewezen
            }

            DraggableItem draggable = obj.GetComponent<DraggableItem>();
            if (draggable != null)
            {
                draggable.Data = objData; // Zorg ervoor dat de Data correct wordt toegewezen
                draggable.spawner = this; // Koppel de spawner
            }
            else
            {
                Debug.LogError("Geen DraggableItem component op prefab: " + prefab.name);
            }
        }
        else
        {
            Debug.LogError("Prefab niet gevonden voor: " + objData.prefabId);
        }
    }



    public async Task<bool> UpdateObjectInApi(Object2D updatedObject)
    {
        var response = await apiClient.UpdateObject2D(updatedObject);
        return !(response is WebRequestError);
    }

    private Transform FindCell(float x, float y)
    {
        foreach (Transform cell in gridContainer)
        {
            // Gebruik wereldcoördinaten
            Vector3 cellPosition = cell.position;
            if (Mathf.Approximately(cellPosition.x, x) &&
                Mathf.Approximately(cellPosition.y, y))
            {
                return cell;
            }
        }
        return null;
    }


    private async void SpawnItem(GameObject prefab)
    {
        foreach (Transform cell in gridContainer)
        {
            if (cell.childCount == 0)
            {
                Object2D newObject = await CreateObjectData(prefab, cell);
                var response = await apiClient.CreateObject2D(newObject);

                if (response is WebRequestData<Object2D> responseData)
                {
                    GameObject item = Instantiate(prefab, cell);
                    item.transform.localPosition = Vector3.zero;

                    var dataHolder = item.AddComponent<Object2DDataHolder>();
                    dataHolder.Data = responseData.Data;

                    DraggableItem draggable = item.GetComponent<DraggableItem>();
                    if (draggable != null) draggable.spawner = this;
                }
                return;
            }
        }
    }

    private async Task<Object2D> CreateObjectData(GameObject prefab, Transform cell)
    {
        string environmentId = PlayerPrefs.GetString("environmentId", "");
        string token = PlayerPrefs.GetString("accessToken");
        webClient.SetToken(token);
        if (string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("Environment ID is missing.");
            return null;
        }

        return new Object2D
        {
            id = System.Guid.NewGuid().ToString(),
            prefabId = prefab.name,
            environmentId = environmentId,
            positionX = cell.position.x,
            positionY = cell.position.y,
            scaleX = prefab.transform.localScale.x,
            scaleY = prefab.transform.localScale.y,
            rotationZ = prefab.transform.eulerAngles.z,
            sortingLayer = 0
        };
    }
}