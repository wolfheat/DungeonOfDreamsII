using UnityEngine.EventSystems;

public class SelectableBasicMenuButton : BasicMenuButton, ISelectHandler, IDeselectHandler
{
    private void TextSetColor(bool active) => textfield.color = active ? activeTextColor : defaultTextColor;

    public void OnSelect(BaseEventData eventData) => TextSetColor(true);
    public void OnDeselect(BaseEventData eventData) => TextSetColor(false);
}


