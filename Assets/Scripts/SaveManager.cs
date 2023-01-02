using System;
using System.IO;
using Assets.My_Assets.Scripts.Custom.Crypto;
using Newtonsoft.Json;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static string SaveDir;
    private const string TwitchIntegrationFilename = "twitch-login.data";

    private const string Password = "It'sASecretToEveryone!";
    private const string Salt = "RealGamersMineSalt4Life";

    private void Start()
    {
        SaveDir = Application.persistentDataPath;
    }

    public void SaveCredentials(TwitchIntegrationLoginInfo loginInfo)
    {
        var loginInfoJson = JsonConvert.SerializeObject(loginInfo);
        var saveData = AESEncryption.Encrypt(loginInfoJson, Password, Salt);
        var filePath = Path.Combine(SaveDir, TwitchIntegrationFilename);
        if(!Directory.Exists(SaveDir))
            Directory.CreateDirectory(SaveDir);
        File.WriteAllText(filePath, saveData);
    }

    public TwitchIntegrationLoginInfo LoadCredentials()
    {
        var filePath = Path.Combine(SaveDir, TwitchIntegrationFilename);
        if (!Directory.Exists(SaveDir) || !File.Exists(filePath))
        {
            Debug.Log($"Couldn't load Twitch integration credentials, file doesn't exist!");
            return null;
        }

        try
        {
            var encrypedData = File.ReadAllText(filePath);
            var loginInfoJson = AESEncryption.Decrypt(encrypedData, Password, Salt);
            return JsonConvert.DeserializeObject<TwitchIntegrationLoginInfo>(loginInfoJson);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse Twitch Integration Login info due to exception: \r\n{e}");
            return null;
        }
    }
}
