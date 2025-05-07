using TMPro;
using UnityEngine;

public class AutoSetVersion : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI versionText;


#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateVersion();    
    }
#endif
     
    [ContextMenu("Update Version text")]
    public void UpdateVersion()
    {
        if (versionText != null) {
            versionText.text = "v. " + Application.version;
        }
    }

}
