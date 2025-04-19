using System;
using UnityEngine;
using Wolfheat.StartMenu;
public class Door : MonoBehaviour
{
    [SerializeField] GameObject doorPart;
    [SerializeField] BoxCollider doorCollider;
    protected bool bossDoor = false;
    public virtual bool IsBossDoor => false;

    public void OpenDoor()
    {
        Debug.Log("Animate opening door");

        GetComponent<Collider>().enabled = false;
        SoundMaster.Instance.PlaySound(SoundName.OpenDoor);

        doorPart.transform.Rotate(0, -90, 0);
    }



}
