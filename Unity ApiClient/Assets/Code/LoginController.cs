using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    public InputField emailInputField; 
    public InputField passwordInputField;
    public Button loginButton;
    public Button registerButton;
    public Text messageText;

    // Referentie naar de WebClient
    public WebClient webClient;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    private async void OnLoginButtonClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        string jsonData = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";

        try
        {
            var response = await webClient.SendPostRequest("/account/login", jsonData);
            if (response is WebRequestData<string> successResponse)
            {
                Debug.Log("Login successful! Response: " + successResponse.Data);
                var responseData = JsonUtility.FromJson<LoginResponse>(successResponse.Data);
                Debug.Log("Token opslaan: " + responseData.accessToken);
                Debug.Log("Response JSON: " + successResponse.Data);

                PlayerPrefs.SetString("email", email);
                PlayerPrefs.SetString("accessToken", responseData.accessToken);
                PlayerPrefs.Save();

                SceneManager.LoadScene("Enviroment");
            }
            else if (response is WebRequestError errorResponse)
            {
                Debug.LogError("Login failed: " + errorResponse.ErrorMessage);
                messageText.text = "Gebruikersnaam of wachtwoord is niet correct.";
                messageText.color = Color.black;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during login: " + ex.Message);
        }
    }


    private async void OnRegisterButtonClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        if (!IsValidEmail(email))
        {
            Debug.LogError("Ongeldig e-mailadres!");
            messageText.text = "Ongeldig e-mailadres!";
            messageText.color = Color.black;
            return;
        }

        if (!IsValidPassword(password))
        {
            Debug.LogError("Wachtwoord voldoet niet aan de eisen!");
            messageText.text = "Wachtwoord voldoet niet aan de eisen!";
            messageText.color = Color.black;
            return;
        }

        string jsonData = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";

        try
        {
            var response = await webClient.SendPostRequest("/account/register", jsonData);
            if (response is WebRequestData<string> successResponse)
            {
                Debug.Log("Registratie succesvol! Response: " + successResponse.Data);
                var responseData = JsonUtility.FromJson<LoginResponse>(successResponse.Data);

                PlayerPrefs.SetString("email", email);
                PlayerPrefs.SetString("accessToken", responseData.accessToken);
                PlayerPrefs.Save();
                SceneManager.LoadScene("Enviroment");
            }
            else if (response is WebRequestError errorResponse)
            {
                Debug.LogError("Registratie mislukt: " + errorResponse.ErrorMessage);
                messageText.text = "Dit e-mailadres is al in gebruik!";
                messageText.color = Color.black;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during registration: " + ex.Message);
        }
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool IsValidPassword(string password)
    {
        return password.Length >= 10 &&
               Regex.IsMatch(password, @"[a-z]") &&
               Regex.IsMatch(password, @"[A-Z]") &&
               Regex.IsMatch(password, @"\d") &&
               Regex.IsMatch(password, @"[^a-zA-Z0-9]");
    }
    [System.Serializable]
    public class LoginResponse
    {
        public string accessToken;
        public string email;
    }
}
