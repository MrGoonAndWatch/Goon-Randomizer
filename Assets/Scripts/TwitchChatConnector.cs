using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TwitchChatConnector : MonoBehaviour
{
    private static TwitchChatConnector _instance;
    private GameObject TwitchCommandsUi;

    private bool _initialized;
    private TcpClient _tcpClient;
    private NetworkStream _stream;

    private void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _instance = this;

        TwitchCommandsUi = FindObjectOfType<TwitchCommandsDisplay>().gameObject;
        TwitchCommandsUi.SetActive(false);

        Init();
    }

    private void Update()
    {
        if (!_initialized) return;

        ParseChat();
    }

    public void Init()
    {
        try
        {
            Initialize();
        }
        catch (Exception e)
        {
            DebugLogger.LogMessage($"Failed to initialize TwitchChatConnector due to exception: {e}");
        }
    }

    private void Initialize()
    {
        DebugLogger.LogMessage("Initializing TwitchChatConnector");
        var saveManager = FindObjectOfType<SaveManager>();
        if (saveManager == null) return;
        DebugLogger.LogMessage("Found SaveManager!");
        var loginInfo = saveManager.LoadCredentials();
        if (loginInfo == null) return;
        DebugLogger.LogMessage("Found valid credentials file!");

        _tcpClient = new TcpClient("irc.chat.twitch.tv", 6667);
        _stream = _tcpClient.GetStream();

        var authBytes = Encoding.UTF8.GetBytes("PASS " + loginInfo.OauthToken + "\r\n");
        _stream.Write(authBytes, 0, authBytes.Length);
        authBytes = Encoding.UTF8.GetBytes("NICK " + loginInfo.Username + "\r\n");
        _stream.Write(authBytes, 0, authBytes.Length);

        var channelBytes = Encoding.UTF8.GetBytes("JOIN #" + loginInfo.Channel + "\r\n");
        _stream.Write(channelBytes, 0, channelBytes.Length);

        DebugLogger.LogMessage("Finished initializing TwitchChatConnector!");

        _initialized = true;
        TwitchCommandsUi.SetActive(true);
    }

    private void ParseChat()
    {
        if (_stream.DataAvailable)
        {
            var data = new byte[_tcpClient.ReceiveBufferSize];
            var bytesRead = _stream.Read(data, 0, data.Length);
            var message = Encoding.UTF8.GetString(data, 0, bytesRead);

            if (!message.Contains("PRIVMSG")) return;

            var trimmedMessage = message.Substring(message.LastIndexOf(':') + 1);

            DebugLogger.LogMessage($"Got twitch message '{trimmedMessage}'");
            if (!trimmedMessage.StartsWith(TwitchChatCommands.CommandPrefix)) return;

            var rawCommand = trimmedMessage.Substring(TwitchChatCommands.CommandPrefix.Length);
            TwitchCommandHandler.HandleCommand(rawCommand);
        }
    }
}
