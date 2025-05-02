using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


#if UNITY_EDITOR
        Resources.UnloadUnusedAssets();
        //CheckedForScenes();
#else      
            ChangeScene("StartMenu");            
#endif
    }

    private void CheckedForScenes()
    {
        
        Debug.Log("** Checking Scenes to Set active. **");
        if (SceneManager.GetSceneByName("StartMenu").IsValid() && SceneManager.GetSceneByName("StartMenu").isLoaded)
        {
            if (SceneManager.GetSceneByName("DreamsDungeon2").IsValid() && SceneManager.GetSceneByName("DreamsDungeon2").isLoaded)
            {
                Debug.Log("Both Start and DungeonII is loaded");
                // If both Menu and Dungeon is loaded unload the menu
                SceneManager.UnloadSceneAsync("StartMenu");
                if(!SceneManager.GetSceneByName("DreamsDungeon2").isLoaded)
                    SceneManager.LoadScene("DreamsDungeon2");
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("DreamsDungeon2"));
                return;
            }
            Debug.Log("Only Start is loaded");
            // If only Menu is loaded set it as active
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("StartMenu"));
            Debug.Log("  StartMenu is set as active.");
        }else if (SceneManager.GetSceneByName("DreamsDungeon2").IsValid() && SceneManager.GetSceneByName("DreamsDungeon2").isLoaded)
        {
            Debug.Log("Only Dungeon II is loaded");
            // If only Dungeon is loaded set it as active            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("DreamsDungeon2"));
            Debug.Log("  Dungeon is set as active.");
        }
        Resources.UnloadUnusedAssets();
    }

    public void ChangeScene(string name, bool additive = true, bool loadFromSaveFile = true)
    {
         
        StartCoroutine(ChangeToActive(name,additive)); // Changing this scene to be the active one
    }
    private IEnumerator ChangeToActive(string name, bool additive)
    {        
        string unloadScene = "";
        // Get active scene so it can be unloaded?
        if (SceneManager.GetActiveScene().name == "DreamsDungeon2")
            unloadScene = "DreamsDungeon2";
        else if (SceneManager.GetActiveScene().name == "StartMenu")
            unloadScene = "StartMenu";

        yield return SceneManager.LoadSceneAsync(name, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        yield return null;

        if (SceneManager.GetSceneByName(name).IsValid())
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));

        // Unload the previus scene
        UnloadScene(unloadScene);
    }

    internal void UnloadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded) {
            SceneManager.UnloadSceneAsync(sceneName);
            Resources.UnloadUnusedAssets();
        }
    }
}
