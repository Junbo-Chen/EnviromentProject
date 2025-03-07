using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserSession
{
    private static UserSession _instance;
    public static UserSession Instance => _instance ??= new UserSession();

    public string Email { get; private set; }
    public string AccessToken { get; private set; }
    public string EnvironmentId { get; private set; }

    private UserSession() { }

    public void SetUser(string email, string accessToken)
    {
        Email = email;
        AccessToken = accessToken;
    }

    public void SetEnvironmentId(string environmentId)
    {
        EnvironmentId = environmentId;
    }

    public void ClearSession()
    {
        Email = null;
        EnvironmentId = null;
        AccessToken = null;
    }
}
