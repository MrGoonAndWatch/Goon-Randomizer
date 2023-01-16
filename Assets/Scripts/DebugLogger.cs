using System.Collections.Generic;
using UnityEngine;

public class DebugLogger : MonoBehaviour
{
    private static DebugLogger _instance;
    private List<string> _logs;

    void Start()
    {
        if (_instance != null)
        {
            Debug.LogWarning($"There's already a debug logger in the scene, destroying duplicate on '{gameObject.name}'");
            Destroy(gameObject);
            return;
        }

        _logs = new List<string>();
        _instance = this;
    }

    public static void LogMessage(string message)
    {
        Debug.Log(message);
        if (_instance == null)
        {
            Debug.LogError("Failed to save message log, DebugLogger is null!");
            return;
        }

        _instance._logs.Add(message);
    }

    public static List<string> GetLogs()
    {
        if (_instance == null)
        {
            Debug.LogError("Failed to get logged messages, DebugLogger is null!");
            return new List<string>();
        }

        return _instance._logs;
    }
}
