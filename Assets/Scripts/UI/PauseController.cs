using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wolfheat.StartMenu;

public class PauseController : MonoBehaviour, IPointerMoveHandler
{
    [SerializeField] Button defaultButton;
    [SerializeField] UIController UIController;
    [SerializeField] GameObject panel;

    public void ToMainMenu()
    {
        Debug.Log("Pause Controller Main Menu Clicked");

        // Save player data here
        if(SavingUtility.playerGameData == null)
        {
            Debug.LogWarning("Going to Main Menu, saving but playerGameData is null");
        }
        else
        {
            Debug.Log("** SAVING LEVEL **");
            //LevelLoader.Instance.DefineGameDataForSave();
            //SavingUtility.Instance.SavePlayerDataToFile();
        }

        // Send Analytics for Game Abandoned
        UGS_Analytics.Instance.GameAbandonedEvent();

        UIController.Instance.ToMainMenu();
        
    }
    public void SetActive(bool doSetActive)
    {
        Debug.Log("Setting pause menu active: "+doSetActive+" Savingutility.playerGameData: "+SavingUtility.playerGameData);
        panel.SetActive(doSetActive);
        if (panel.activeSelf) {
            Debug.Log("Selecting Default Button");
            defaultButton.Select();
        }
    }

    public void CloseClicked()
    {
        Debug.Log("Pause Controller Close clicked");
        UIController.Pause(false);
        SetActive(false);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
