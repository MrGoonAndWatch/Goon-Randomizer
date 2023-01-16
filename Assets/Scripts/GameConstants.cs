public class TwitchIntegrationLoginInfo
{
    public string Username { get; set; }
    public string OauthToken { get; set; }
    public string Channel { get; set; }
}

public static class TwitchChatCommands
{
    public const string CommandPrefix = "!";
}

public enum TwitchChatCommand
{
    bump,
    reverse,
    fast,
    slow,
    moon,
    stop,
    heavy,
}