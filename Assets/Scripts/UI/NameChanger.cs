using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

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

    private void Start()
    {
        playerNameInputField.onValueChanged.AddListener(ValidateInput);
    }

    private void ValidateInput(string input)
    {
        // Only allow letters, numbers, spaces, and underscores
        string valid = Regex.Replace(input, @"[^a-zA-Z0-9. ]","");
        //string valid = Regex.Replace(input, @"[^a-zA-Z0-9_. ]", "");
        valid = valid.Length > 20 ? valid.Substring(0, 20) : valid;


        if (valid != input) {
            playerNameInputField.text = valid; 
        }
    }


    public void PlayerEndEditName(TMP_InputField inputfield)
    {
        Debug.Log("Player ended with name "+inputfield.text);
        SavingUtility.gameSettingsData.SetPlayerName(inputfield.text);
    }

    public void SetPlayerName(string playerName) => playerNameInputField.text = playerName;
}
