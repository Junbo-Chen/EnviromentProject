using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnvironmentItemUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button environmentButton; 
    
    private string environmentId;

    public void Initialize(string environmentName, string envId, System.Action onDelete)
    {
        nameText.text = environmentName;
        environmentId = envId;

        deleteButton.onClick.AddListener(() => onDelete?.Invoke());
        environmentButton.onClick.AddListener(LoadWorldScene); 
    }

    private void LoadWorldScene()
    {
        UserSession.Instance.SetEnvironmentId(environmentId);

        SceneManager.LoadScene("World");
    }
}
