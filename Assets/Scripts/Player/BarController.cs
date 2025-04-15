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
        Debug.Log("StartWidth BAr = "+Barwidth);
    }

    private void Start()
    {
        oxygenController.OxygenUpdated += SetBar;
    }


    public void SetBar(float value, float MaxValue)
    {
        float percent = value / MaxValue;

        text.text = ((int)value).ToString();
        bar.sizeDelta = new Vector2(-Barwidth * (1 - percent), 0);
        //bar.sizeDelta = new Vector2(-Barwidth * (1 - value), 0);
        //Debug.Log("Bar = "+bar.sizeDelta.x);
    }
}
