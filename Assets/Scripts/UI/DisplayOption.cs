using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayOption : MonoBehaviour
{
    [SerializeField] Color defaultColor;
    [SerializeField] Color activeColor;
    [SerializeField] Button button;

    [SerializeField] private TextMeshProUGUI optionText;
    [SerializeField] private TextMeshProUGUI optionname;

    private int index = -1;
    public void SetIndexAndTexts(int index,string monitorName = "")
    {
        this.index = index;
        optionText.text = ((char)('A'+index)).ToString();
        optionname.text = monitorName;
    }

    public void ButtonClicked() => DisplayOptionsController.Instance.SetDisplayOptionTo(index);



    public void SetAsSelected(bool doSet)
    {
        // This sets the button default value
        ColorBlock cb = button.colors;
        cb.normalColor = doSet ? activeColor : defaultColor;
        button.colors = cb;
    }

}
