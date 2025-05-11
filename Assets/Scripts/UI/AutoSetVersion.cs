using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoSetVersion : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private Image image;
    [SerializeField] private Sprite testSprite;
    [SerializeField] private SystemID systemID_Data;


#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateVersion();    
    }
     
    [ContextMenu("Update Version text")]
    public void UpdateVersion()
    {
        if (versionText != null) {
            versionText.text = "v. " + Application.version;
        }

        // Also show the system symbol        
        image.sprite = systemID_Data.GetCurrentBuildSystemSprite();
    }
#endif

}
