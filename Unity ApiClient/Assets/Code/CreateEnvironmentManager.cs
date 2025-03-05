using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class CreateEnvironmentManager : MonoBehaviour
{
    [Header("UI References")]
    public InputField nameInput;
    public InputField widthInput;
    public InputField heightInput;
    public Button createButton;
    public Text errorText;

    [Header("API")]
    public Environment2DApiClient environmentApiClient;
    public WebClient webClient;

    [Header("Constraints")]
    private const int MIN_LENGTH = 20;
    private const int MAX_LENGTH = 200;
    private const int MIN_HEIGHT = 10;
    private const int MAX_HEIGHT = 100;
    private const int MAX_NAME_LENGTH = 25;
    private const int MAX_ENVIRONMENTS = 5;

    private void Start()
    {
        nameInput.characterLimit = MAX_NAME_LENGTH;
        createButton.onClick.AddListener(TryCreateEnvironment);
    }

    private async void TryCreateEnvironment()
    {
        errorText.text = "";
        createButton.interactable = false;
        bool isValid = true;

        try
        {
            string token = PlayerPrefs.GetString("accessToken");
            if (string.IsNullOrEmpty(token))
            {
                ShowError("Je moet ingelogd zijn!", true);
                return;
            }

            string environmentName = nameInput.text.Trim();
            if (!ValidateName(environmentName))
            {
                isValid = false;
            }

            if (!ValidateDimensions(out int width, out int height))
            {
                isValid = false;
            }

            var environments = await GetUserEnvironments(token);
            if (environments == null)
            {
                isValid = false;
            }
            else
            {
                if (!ValidateEnvironmentCount(environments))
                {
                    isValid = false;
                }

                if (!ValidateUniqueName(environments, environmentName))
                {
                    isValid = false;
                }
            }

            if (isValid)
            {
                Debug.Log("test");
                await CreateNewEnvironment(environmentName, width, height, token);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Fout: {ex.Message}");
        }
        finally
        {
            createButton.interactable = true;
        }
    }


    private bool ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_LENGTH)
        {
            ShowError($"Naam moet 1-{MAX_NAME_LENGTH} tekens bevatten!");
            return false;
        }
        return true;
    }

    private bool ValidateDimensions(out int width, out int height)
    {
        width = height = 0;
        if (!int.TryParse(widthInput.text, out width) || width < MIN_LENGTH || width > MAX_LENGTH)
        {
            ShowError($"Breedte moet tussen {MIN_LENGTH}-{MAX_LENGTH} zijn!");
            return false;
        }
        if (!int.TryParse(heightInput.text, out height) || height < MIN_HEIGHT || height > MAX_HEIGHT)
        {
            ShowError($"Hoogte moet tussen {MIN_HEIGHT}-{MAX_HEIGHT} zijn!");
            return false;
        }
        return true;
    }

    private async Awaitable<List<Environment2D>> GetUserEnvironments(string token)
    {
        try
        {
            webClient.SetToken(token);
            var response = await environmentApiClient.GetUserEnvironments();

            if (response is WebRequestData<List<Environment2D>> successResponse)
            {
                return successResponse.Data;
            }
            if (response is WebRequestError error)
            {
                ShowError($"Melding: {error.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Exception: {ex.Message}");
        }
        return new List<Environment2D>();
    }

    private bool ValidateEnvironmentCount(List<Environment2D> environments)
    {
        if (environments.Count >= MAX_ENVIRONMENTS)
        {
            ShowError($"Maximaal {MAX_ENVIRONMENTS} omgevingen toegestaan!");
            return false;
        }
        return true;
    }

    private bool ValidateUniqueName(List<Environment2D> environments, string newName)
    {
        foreach (var env in environments)
        {
            if (env.name.Equals(newName, StringComparison.OrdinalIgnoreCase))
            {
                ShowError("Deze naam bestaat al!");
                return false;
            }
        }
        return true;
    }

    private async Awaitable CreateNewEnvironment(string name, int width, int height, string token)
    {
        if (string.IsNullOrWhiteSpace(name) || width <= 0 || height <= 0)
        {
            ShowError("Ongeldige gegevens, kan geen omgeving aanmaken!");
            return;
        }

        var newEnv = new Environment2D { name = name, maxLength = width, maxHeight = height };
        var response = await environmentApiClient.CreateEnvironment(newEnv);

        if (response is WebRequestData<Environment2D>)
        {
            SceneManager.LoadScene("Enviroment");
        }
        else if (response is WebRequestError error)
        {
            ShowError($"Aanmaken mislukt: {error.ErrorMessage}");
        }
    }

    private void ShowError(string message, bool redirectToLogin = false)
    {
        errorText.text = message;
        errorText.color = Color.red;
        if (redirectToLogin) SceneManager.LoadScene("Login");
    }
}
