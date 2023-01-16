using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip _landingSound;
    [SerializeField]
    private AudioSource _audioSource;

    private static SoundManager _instance;

    private void Start()
    {
        if (_instance != null)
        {
            Debug.LogError($"Found multiple instances of SoundManager, deleting object '{gameObject.name}'.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public static void PlayGroundSound()
    {
        if (_instance == null)
        {
            Debug.LogError("Cannot play ground sound, SoundManager is null!");
            return;
        }

        _instance._audioSource.clip = _instance._landingSound;
        _instance._audioSource.Play();
    }
}
