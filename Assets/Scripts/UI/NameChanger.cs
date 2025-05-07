using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameChanger : MonoBehaviour
{

    [SerializeField] private TMP_InputField playerNameInputField;

    public bool IsEditingName => playerNameInputField.isFocused;

    public static NameChanger Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }



    public void PlayerEndEditName(TMP_InputField inputfield)
    {
        Debug.Log("Player ended with name "+inputfield.text);
        SavingUtility.gameSettingsData.SetPlayerName(inputfield.text);
    }

    public void SetPlayerName(string playerName) => playerNameInputField.text = playerName;
}
