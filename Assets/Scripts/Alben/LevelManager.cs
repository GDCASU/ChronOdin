using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles changing levels (scenes)
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region Singleton
    private static LevelManager instance;

    /// <summary>
    /// Grab the singleton for the LevelManager
    /// </summary>
    public static LevelManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            else
            {
                Debug.LogError("The LevelManager is NULL.");
                return null;
            }
        }
    }

    /// <summary>
    /// Set the LevelManager instance here
    /// </summary>
    private void Awake()
    {
        instance = this;
    }
    #endregion

    /// <summary>
    /// Go to the title screen/scene (at scene's build index 0)
    /// </summary>
    public void TitleScene()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Go to the level/scene after this level/scene's build index
    /// </summary>
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Pick a level/scene based on it's build index
    /// </summary>
    /// <param name="sceneIndex">The build index for the selected level/scene</param>
    public void PickLevel(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Reload the current level/scene
    /// </summary>
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
