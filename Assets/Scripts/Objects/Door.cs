using UnityEngine;
using Wolfheat.StartMenu;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject doorPart;
    [SerializeField] BoxCollider doorCollider;


    public void OpenDoor()
    {
        Debug.Log("Animate opening door");

        GetComponent<Collider>().enabled = false;
        SoundMaster.Instance.PlaySound(SoundName.OpenDoor);

        doorPart.transform.Rotate(0, -90, 0);
    }



}
