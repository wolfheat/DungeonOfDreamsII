using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSystemSetter : MonoBehaviour
{
    [SerializeField] private Image image; 
    [SerializeField] private TextMeshProUGUI textfield;

    // WIN, WEBL, UNITY
    [Header("WIN, WEBL, UNITY, UNIX, ANDROID")]
    [SerializeField] private Sprite[] systemSprites; 

    public void SetAsSystem(int systemID, string versionString)
    {
        // Only keep the last part of the string
        versionString = versionString.Split('.').ToArray().Last();

        image.sprite = systemSprites[systemID];
        textfield.text = versionString;
    }
}
