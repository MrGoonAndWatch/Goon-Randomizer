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
        var saveManager = FindObjectOfType<SaveManager>();
        if (saveManager == null) return;
        var loginInfo = saveManager.LoadCredentials();
        if (loginInfo == null) return;

        _tcpClient = new TcpClient("irc.chat.twitch.tv", 6667);
        _stream = _tcpClient.GetStream();

        var authBytes = Encoding.UTF8.GetBytes("PASS " + loginInfo.OauthToken + "\r\n");
        _stream.Write(authBytes, 0, authBytes.Length);
        authBytes = Encoding.UTF8.GetBytes("NICK " + loginInfo.Username + "\r\n");
        _stream.Write(authBytes, 0, authBytes.Length);

        var channelBytes = Encoding.UTF8.GetBytes("JOIN #" + loginInfo.Channel + "\r\n");
        _stream.Write(channelBytes, 0, channelBytes.Length);

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

            if (!trimmedMessage.StartsWith(TwitchChatCommands.CommandPrefix)) return;

            var rawCommand = trimmedMessage.Substring(TwitchChatCommands.CommandPrefix.Length);
            HandleCommand(rawCommand);
        }
    }

    // TODO: This should really be a separate behavior that we call with the command.
    private static void HandleCommand(string rawCommand)
    {
        var commandSection = rawCommand.Replace("\r", "").Replace("\n", "").Replace("\t", "").Split(' ')[0].ToLower();
        TwitchChatCommand command;
        if (!TwitchChatCommand.TryParse(commandSection, out command)) return;

        switch (command)
        {
            case TwitchChatCommand.bump:
                BumpBall();
                break;
            case TwitchChatCommand.reverse:
                ReverseSpinners();
                break;
        }
    }

    private static void BumpBall()
    {
        var ballDropper = FindObjectOfType<BallDropper>();
        if(ballDropper != null)
            ballDropper.BumpBall();
    }

    private static void ReverseSpinners()
    {
        var spinners = FindObjectsOfType<Spinner>();
        for (var i = 0; i < spinners.Length; i++)
        {
            spinners[i].Reverse();
        }
    }
}
