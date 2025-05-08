using UnityEngine;
using UnityEngine.EventSystems;

public class MouseReleaseRegistrator : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse release on Mouse Registator");
    }


}
