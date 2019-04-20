using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Change a scene.
/// </summary>
public class SceneChanger : MonoBehaviour
{
    /// <summary>
    /// Loads the scene.
    /// </summary>
    /// <param name="sceneName">Scene name.</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Loads the current scene.
    /// </summary>
    public void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitApp()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit ();
    #endif
    }
}