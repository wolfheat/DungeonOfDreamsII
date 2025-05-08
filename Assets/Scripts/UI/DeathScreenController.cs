using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wolfheat.StartMenu;
public class DeathScreenController : MonoBehaviour
{
    [SerializeField] UIController UIController;
    [SerializeField] GameObject panel;
    [SerializeField] Button defaultButton;

    public void Show()
    {
        panel.SetActive(true);
        UIController.Pause(true);
        UIController.HideDarkening();

        // Set Active default
        defaultButton.Select();

        Debug.Log("Death screen Active Pause game");
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void SetActive(bool doSetActive)
    {
        Debug.Log("Setting pause menu active: " + doSetActive + " Savingutility.playerGameData: " + SavingUtility.playerGameData);
        panel.SetActive(doSetActive);
    }
    public void ToMainMenu()
    {
        Debug.Log("Death Controller To Main Menu Clicked");

        // Save player data here
        if (SavingUtility.playerGameData == null)
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

    public void CloseClicked()
    {
        Debug.Log("Death Controller Close clicked");
        PlayerController.Instance.Reset();

        SoundMaster.Instance.ResetMusic();
        UIController.Pause(false);
        SetActive(false);
    }
}
