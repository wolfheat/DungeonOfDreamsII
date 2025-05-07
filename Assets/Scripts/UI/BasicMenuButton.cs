using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BasicMenuButton : MonoBehaviour
{
    [SerializeField] Color defaultColor;
    [SerializeField] Color activeColor;
    [SerializeField] protected Color defaultTextColor;
    [SerializeField] protected Color activeTextColor;
    [SerializeField] Button button;

    [SerializeField] protected TextMeshProUGUI textfield;

    public void SetAsSelected(bool doSet)
    {
        // This sets the button default value
        ColorBlock cb = button.colors;
        cb.normalColor = doSet ? activeColor : defaultColor;
        button.colors = cb;
        textfield.color = doSet ? activeTextColor : defaultTextColor;
    }

}


