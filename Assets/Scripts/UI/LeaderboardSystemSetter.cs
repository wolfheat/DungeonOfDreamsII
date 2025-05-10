using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSystemSetter : MonoBehaviour
{
    [SerializeField] private Image image; 
    [SerializeField] private TextMeshProUGUI textfield;
    [SerializeField] private SystemID systemID_Data;


    public void SetAsSystem(int systemID, string versionString)
    {
        // Only keep the last part of the string
        versionString = versionString.Split('.').ToArray().Last();

        textfield.text = versionString;
        image.sprite = SystemIDController.Instance.SystemID.GetSprite(systemID);
            

    }
}
