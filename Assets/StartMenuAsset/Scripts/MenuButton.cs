using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Wolfheat.Inputs;

namespace Wolfheat.StartMenu
{
    public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
    {
        [SerializeField] private bool isMenubutton = true; 
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
        }

        private void EnteringButton()
        {
            if (isMenubutton) {
                if (StartMenuController.lastButton == this) return;
                StartMenuController.lastButton = this;
            }
            SoundMaster.Instance.PlaySound(SoundName.MenuOver, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartMenuController.lastButton = null;
        }


        public void OnDeselect(BaseEventData eventData) => transform.localScale = Vector3.one;
        public void OnSelect(BaseEventData eventData)
        {
            EnteringButton();
            if (!isMenubutton) 
                transform.localScale = new Vector3(1.1f,1.1f,1.1f);
            
        }
    }
}
