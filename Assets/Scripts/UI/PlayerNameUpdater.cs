using System;
using TMPro;
using UnityEngine;

public class PlayerNameUpdater : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI playerNameText;

    private void OnEnable()
    {
        GameSettingsData.GameSettingsUpdated += UpdateName;
        SavingUtility.LoadingComplete += UpdateName;
        UpdateName();
    }

    private void OnDisable()
    {
        GameSettingsData.GameSettingsUpdated -= UpdateName;
        SavingUtility.LoadingComplete -= UpdateName;
    }
     
    private void UpdateName()
    {
        Debug.Log("-- Updating Player Name in Start Menu");
        if(SavingUtility.gameSettingsData != null)
            playerNameText.text = SavingUtility.gameSettingsData.PlayerName;
    }
}
