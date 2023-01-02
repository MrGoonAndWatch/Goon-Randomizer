using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    private int _currentLevel;
    private int _lastLevelIndex;

    private void Start()
    {
        _currentLevel = SceneManager.GetActiveScene().buildIndex;
        _lastLevelIndex = SceneManager.sceneCountInBuildSettings - 1;
    }
    
    private void Update()
    {
        if (_currentLevel > 0 && Input.GetKeyDown(KeyCode.LeftArrow))
            GoToLevel(_currentLevel - 1);
        else if (_currentLevel < _lastLevelIndex && Input.GetKeyDown(KeyCode.RightArrow))
            GoToLevel(_currentLevel + 1);
    }

    private static void GoToLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
}
