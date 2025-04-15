using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject doorPart;
    [SerializeField] BoxCollider doorCollider;


    public void OpenDoor()
    {
        Debug.Log("Animate opening door");

        GetComponent<Collider>().enabled = false;

        doorPart.transform.Rotate(0, -90, 0);
    }



}
