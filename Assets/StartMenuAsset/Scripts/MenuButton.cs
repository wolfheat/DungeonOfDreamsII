using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wolfheat.StartMenu
{
    public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
    {
        public void AnimationComplete()
        {

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //SoundMaster.Instance.PlaySound(SoundName.MenuClick);
            //Debug.Log("Click in Button: "+Time.realtimeSinceStartup);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EnteringButton();
            StartMenuController.PlayerUsingMouse = true;
        }

        private void EnteringButton()
        {
            if (StartMenuController.lastButton == this) return;

            StartMenuController.lastButton = this;
            SoundMaster.Instance.PlaySound(SoundName.MenuOver, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartMenuController.lastButton = null;
        }

        public void OnSelect(BaseEventData eventData)
        {
            EnteringButton();
        }
    }
}
