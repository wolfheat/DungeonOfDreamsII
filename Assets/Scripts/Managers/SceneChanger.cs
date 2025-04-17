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
        CheckedForScenes();
#else      
            ChangeScene("StartMenu");            
#endif
    }

    private void CheckedForScenes()
    {
        Debug.Log("** Checking Scenes to Set active. **");
        if (SceneManager.GetSceneByName("StartMenu").IsValid())
        {
            if (SceneManager.GetSceneByName("DreamsDungeon2").IsValid())
            {
                // If both Menu and Dungeon is loaded unload the menu
                SceneManager.UnloadSceneAsync("StartMenu");
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("DreamsDungeon2"));
                return;
            }
            // If only Menu is loaded set it as active
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("StartMenu"));
            Debug.Log("  StartMenu is set as active.");
        }else if (SceneManager.GetSceneByName("DreamsDungeon2").IsValid())
        {
            // If only Dungeon is loaded set it as active            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("DreamsDungeon2"));
            Debug.Log("  Dungeon is set as active.");
        }
    }

    public void ChangeScene(string name, bool additive = true, bool loadFromSaveFile = true)
    {
        SceneManager.LoadSceneAsync(name, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
         
        StartCoroutine(ChangeToActive(name)); // Changing this scene to be the active one
    }
    private IEnumerator ChangeToActive(string name)
    {
        yield return new WaitForSeconds(0.05f); 
        if (SceneManager.GetSceneByName(name).IsValid())
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
    }
}
