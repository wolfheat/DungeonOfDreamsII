using UnityEngine;

using TMPro;

public class BarController : MonoBehaviour
{
    [SerializeField] RectTransform bar;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] OxygenController oxygenController;

    public static float Barwidth;
    private void OnEnable()
    {
        Barwidth = GetComponent<RectTransform>().rect.size.x;
    }

    private void Start()
    {
        oxygenController.OxygenUpdated += SetBar;
    }


    public void SetBar(float value, float MaxValue)
    {
        text.text = value.ToString();
        bar.sizeDelta = new Vector2(-Barwidth * (1 - value), 0);
    }
}
