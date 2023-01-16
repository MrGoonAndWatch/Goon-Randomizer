using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class TwitchLoginInfoScreen : MonoBehaviour
{
    private bool _loginVisible = false;
    private bool _debugVisible = false;

    [SerializeField]
    private Canvas LoginCanvas;
    [SerializeField]
    private Canvas InstructionsCanvas;
    [SerializeField]
    private Canvas DebugLogCanvas;
    [SerializeField]
    private TMP_InputField UsernameInput;
    [SerializeField]
    private TMP_InputField TwitchChannelInput;
    [SerializeField]
    private TMP_InputField OauthTokenInput;
    [SerializeField]
    private TMP_Text DebugLogInput;

    void Start()
    {
        LoginCanvas.gameObject.SetActive(false);
        DebugLogCanvas.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
            ToggleLoginScreen();
        if (Input.GetKeyUp(KeyCode.F2))
            ToggleDebugScreen();
    }

    public void Connect()
    {
        var username = UsernameInput.text;
        var twitchChannel = TwitchChannelInput.text;
        var oauth = OauthTokenInput.text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(twitchChannel) || string.IsNullOrEmpty(oauth)) return;

        var saveManager = FindObjectOfType<SaveManager>();
        var loginInfo = new TwitchIntegrationLoginInfo
        {
            Username = username,
            Channel = twitchChannel,
            OauthToken = oauth
        };
        saveManager.SaveCredentials(loginInfo);
        var connector = FindObjectOfType<TwitchChatConnector>();
        connector.Init();
        ToggleLoginScreen();
    }

    private void ToggleLoginScreen()
    {
        _loginVisible = !_loginVisible;
        _debugVisible = false;
        LoginCanvas.gameObject.SetActive(_loginVisible);
        InstructionsCanvas.gameObject.SetActive(!_loginVisible);
        DebugLogCanvas.gameObject.SetActive(false);
    }

    private void ToggleDebugScreen()
    {
        RefreshDebugLogs();
        _debugVisible = !_debugVisible;
        _loginVisible = false;
        DebugLogCanvas.gameObject.SetActive(_debugVisible);
        InstructionsCanvas.gameObject.SetActive(!_debugVisible);
        LoginCanvas.gameObject.SetActive(false);
    }

    private const int DebugLinesThatFitOnScreen = 16;
    private void RefreshDebugLogs()
    {
        var logs = DebugLogger.GetLogs();
        var logsOnScreen = logs.Skip(logs.Count - DebugLinesThatFitOnScreen);
        var logScreen = new StringBuilder();
        foreach (var logMessage in logsOnScreen)
        {
            logScreen.AppendLine(logMessage.Replace("\r", "").Replace("\n", ""));
        }

        DebugLogInput.text = logScreen.ToString();
    }
}
