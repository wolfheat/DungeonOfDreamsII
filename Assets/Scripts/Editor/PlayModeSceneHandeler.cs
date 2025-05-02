// Put this in an Editor folder: Assets/Editor/PlayModeSceneHandler.cs

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Comment out to disable
//[InitializeOnLoad]
public static class PlayModeSceneHandler
{
    static SceneSetup[] cachedSceneSetup;

    static PlayModeSceneHandler()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        switch (state) {
            case PlayModeStateChange.ExitingEditMode:
                // Save current scene setup before entering play mode
                cachedSceneSetup = EditorSceneManager.GetSceneManagerSetup();
                // Optionally: Open only your bootstrap scene here
                //EditorSceneManager.NewScene("\Assets\Scenes\DreamsII\", NewSceneMode.Single);
                Scene startMenuScene = EditorSceneManager.GetSceneByName("StartMenu");

                EditorSceneManager.OpenScene("Assets/ExternalResources/StartMenuAsset/Scenes/StartMenu.unity", OpenSceneMode.Single);
                EditorSceneManager.OpenScene("Assets/ExternalResources/StartMenuAsset/Scenes/Managers.unity", OpenSceneMode.Additive);
                EditorSceneManager.SetActiveScene(startMenuScene);
                break;

            case PlayModeStateChange.EnteredEditMode:
                // Restore the saved scene setup after exiting play mode
                if (cachedSceneSetup != null && cachedSceneSetup.Length > 0) {
                    EditorSceneManager.RestoreSceneManagerSetup(cachedSceneSetup);
                    cachedSceneSetup = null;
                }
                break;
        }
    }
}
