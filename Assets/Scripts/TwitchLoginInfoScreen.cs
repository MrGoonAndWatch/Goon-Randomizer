using TMPro;
using UnityEngine;

public class TwitchLoginInfoScreen : MonoBehaviour
{
    private bool _visible = false;

    [SerializeField]
    private Canvas LoginCanvas;
    [SerializeField]
    private Canvas InstructionsCanvas;
    [SerializeField]
    private TMP_InputField UsernameInput;
    [SerializeField]
    private TMP_InputField TwitchChannelInput;
    [SerializeField]
    private TMP_InputField OauthTokenInput;

    void Start()
    {
        LoginCanvas.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
            ToggleScreen();
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
        ToggleScreen();
    }

    private void ToggleScreen()
    {
        _visible = !_visible;
        LoginCanvas.gameObject.SetActive(_visible);
        InstructionsCanvas.gameObject.SetActive(!_visible);
    }
}
