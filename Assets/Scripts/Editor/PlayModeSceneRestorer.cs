// Save as Assets/Editor/PlayModeSceneRestorer.cs

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

//[InitializeOnLoad]
public static class PlayModeSceneRestorer
{
    private const string SceneSetupPath = "Library/PlayModeSceneSetup.json"; // Temporary file, not in source control

    static PlayModeSceneRestorer()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        switch (state) {
            case PlayModeStateChange.ExitingEditMode:
                SaveCurrentSceneSetup();
                // Optionally load a clean bootstrap scene here:


                EditorSceneManager.OpenScene("Assets/ExternalResources/StartMenuAsset/Scenes/Managers.unity", OpenSceneMode.Single);
                EditorSceneManager.OpenScene("Assets/ExternalResources/StartMenuAsset/Scenes/StartMenu.unity", OpenSceneMode.Additive);
                EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName("StartMenu"));

                break;

            case PlayModeStateChange.EnteredEditMode:
                RestoreSceneSetup();
                break;
        }
    }

    private static void SaveCurrentSceneSetup()
    {
        var setup = EditorSceneManager.GetSceneManagerSetup();
        if (setup.Length == 0) return;

        string json = JsonUtility.ToJson(new SceneSetupWrapper { scenes = setup });
        File.WriteAllText(SceneSetupPath, json);
    }

    private static void RestoreSceneSetup()
    {
        if (!File.Exists(SceneSetupPath)) return;

        string json = File.ReadAllText(SceneSetupPath);
        var wrapper = JsonUtility.FromJson<SceneSetupWrapper>(json);
        if (wrapper?.scenes != null && wrapper.scenes.Length > 0) {
            EditorSceneManager.RestoreSceneManagerSetup(wrapper.scenes);
        }

        File.Delete(SceneSetupPath); // Clean up
    }

    [System.Serializable]
    private class SceneSetupWrapper
    {
        public SceneSetup[] scenes;
    }
}