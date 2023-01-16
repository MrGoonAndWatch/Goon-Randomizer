using UnityEngine;
using UnityEngine.SceneManagement;

public class TwitchCommandHandler : MonoBehaviour
{
    private Spinner[] _spinners;

    [Tooltip("The time (in seconds) the !fast and !slow commands should last.")]
    [SerializeField]
    private float _speedChangeDuration = 5.0f;

    [Tooltip("The time (in seconds) the !moon and !heavy commands should last.")]
    [SerializeField]
    private float _gravityChangeDuration = 5.0f;

    private float _secondsUntilGravityResets;
    private float _secondsUntilSpinnerSpeedsReset;

    private static TwitchCommandHandler _instance;

    private void Start()
    {
        if (_instance != null)
        {
            Debug.LogWarning($"Found multiple TwitchCommandHandlers, deleting this one on '{gameObject.name}'");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _spinners = FindObjectsOfType<Spinner>();
        SceneManager.sceneLoaded += SetSceneReferences;
    }

    private void SetSceneReferences(Scene scene, LoadSceneMode mode)
    {
        _spinners = FindObjectsOfType<Spinner>();
    }
    
    private void Update()
    {
        HandleSpeedReset();
        HandleGravityReset();
    }

    private void HandleSpeedReset()
    {
        if (_secondsUntilSpinnerSpeedsReset <= 0) return;

        _secondsUntilSpinnerSpeedsReset -= Time.deltaTime;
        if (_secondsUntilSpinnerSpeedsReset <= 0)
        {
            for (var i = 0; i < _spinners.Length; i++)
            {
                var spinner = _spinners[i];
                spinner.ResetSpeed();
            }
        }
    }

    private void HandleGravityReset()
    {
        if (_secondsUntilGravityResets <= 0) return;

        _secondsUntilGravityResets -= Time.deltaTime;
        if (_secondsUntilGravityResets <= 0)
        {
            var ball = GetBall();
            ball.SetGravity(1);
        }
    }

    public static void HandleCommand(string rawCommand)
    {
        if (_instance == null)
        {
            Debug.LogError("Cannot process Twitch Chat command, TwitchCommandHandler is null!");
            return;
        }

        var commandSection = rawCommand.Replace("\r", "").Replace("\n", "").Replace("\t", "").Split(' ')[0].ToLower();
        TwitchChatCommand command;
        if (!TwitchChatCommand.TryParse(commandSection, out command)) return;
        _instance.HandleCommand(command);
    }

    private void HandleCommand(TwitchChatCommand command)
    {
        DebugLogger.LogMessage($"Received a '{command}' command!");

        switch (command)
        {
            case TwitchChatCommand.bump:
                BumpBall();
                break;
            case TwitchChatCommand.reverse:
                ReverseSpinners();
                break;
            case TwitchChatCommand.fast:
                DoubleSpinnerSpeed();
                break;
            case TwitchChatCommand.slow:
                HalfSpinnerSpeed();
                break;
            case TwitchChatCommand.stop:
                StopSpinners();
                break;
            case TwitchChatCommand.moon:
                SetBallGravity(0.5f);
                break;
            case TwitchChatCommand.heavy:
                SetBallGravity(5);
                break;
        }
    }

    private static BallDropper GetBall()
    {
        return FindObjectOfType<BallDropper>();
    }

    private static void BumpBall()
    {
        var ballDropper = GetBall();
        ballDropper.BumpBall();
    }

    private void ReverseSpinners()
    {
        for (var i = 0; i < _spinners.Length; i++)
        {
            _spinners[i].Reverse();
        }
    }

    private void DoubleSpinnerSpeed()
    {
        _secondsUntilSpinnerSpeedsReset = _speedChangeDuration;
        for (var i = 0; i < _spinners.Length; i++)
        {
            _spinners[i].DoubleSpeed();
        }
    }

    private void HalfSpinnerSpeed()
    {
        _secondsUntilSpinnerSpeedsReset = _speedChangeDuration;
        for (var i = 0; i < _spinners.Length; i++)
        {
            _spinners[i].HalfSpeed();
        }
    }

    private void StopSpinners()
    {
        _secondsUntilSpinnerSpeedsReset = _speedChangeDuration;
        for (var i = 0; i < _spinners.Length; i++)
        {
            _spinners[i].Stop();
        }
    }

    private void SetBallGravity(float newGravity)
    {
        _secondsUntilGravityResets = _gravityChangeDuration;
        var ballDropper = GetBall();
        ballDropper.SetGravity(newGravity);
    }
}
