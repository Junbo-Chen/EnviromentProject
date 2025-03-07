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
            string email = UserSession.Instance.Email ?? "Gebruiker";
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
        if (LogoutButton != null)
        {
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
        UserSession.Instance.ClearSession();
        SceneManager.LoadScene("SampleScene");
    }
}