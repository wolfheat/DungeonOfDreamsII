public class DisplayOption : BasicMenuButton
{
    private int index = -1;
    public void SetIndexAndTexts(int index,string monitorName = "")
    {
        this.index = index;
        textfield.text = monitorName;
    }

    public void ButtonClicked() => DisplayOptionsController.Instance.SetDisplayOptionTo(index);

}


