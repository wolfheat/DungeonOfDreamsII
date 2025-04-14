using UnityEngine;

public class GUIBarController : MonoBehaviour
{
    [SerializeField] BarController oxygenBar;
    [SerializeField] BarController healthBar;
    [SerializeField] OxygenController oxygenController;

    public static float Barwidth;

    private void OnEnable()
    {
        Barwidth = oxygenBar.GetComponent<RectTransform>().rect.size.x;
    }
}
