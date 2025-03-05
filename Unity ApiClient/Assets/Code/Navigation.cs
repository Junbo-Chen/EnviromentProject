using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    public Text welcomeText;
    public Button CreateButton;
    public Button HomeButton;
    public Button LogoutButton;

    private void Start()
    {
        // Alleen uitvoeren als welcomeText bestaat
        if (welcomeText != null)
        {
            string email = PlayerPrefs.GetString("email", "Gebruiker");
            //string accessToken = PlayerPrefs.GetString("accessToken");
            welcomeText.text = "Welkom, " + email + "!";
        }

        // Alleen CreateButton initialiseren als hij bestaat
        if (CreateButton != null)
        {
            CreateButton.onClick.AddListener(OnCreateButtonClicked);
        }

        // Alleen HomeButton initialiseren als hij bestaat
        if (HomeButton != null)
        {
            HomeButton.onClick.AddListener(OnHomeButtonClicked);
        }
        if (LogoutButton != null){
            LogoutButton.onClick.AddListener(OnLogoutButtonClicked);
        }
    }

    private async void OnCreateButtonClicked()
    {
        SceneManager.LoadScene("CreateEnvironment");
    }

    private async void OnHomeButtonClicked()
    {
        SceneManager.LoadScene("Enviroment");
    }

    private async void OnLogoutButtonClicked()
    {
        PlayerPrefs.DeleteKey("email");
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
}
