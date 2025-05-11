using System;
using System.Collections;
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

        // Disable Door so it wont double open
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Collider>().enabled = false;

        SoundMaster.Instance.PlaySound(SoundName.OpenDoor);

        // Animated opening of door
        StartCoroutine(OpenDoorCO());

        // Instant opening of door
        //doorPart.transform.Rotate(0, -90, 0);
    }
    
    public void OpenDoorAnimate()
    {
        Debug.Log("Animate opening door");

        GetComponent<Collider>().enabled = false;
        SoundMaster.Instance.PlaySound(SoundName.OpenDoor);
        StartCoroutine(OpenDoorCO());
        doorPart.transform.Rotate(0, -90, 0);
    }

    private IEnumerator OpenDoorCO()
    {
        float startRotation = doorPart.transform.rotation.eulerAngles.y;
        Quaternion startRotationQuaternion = doorPart.transform.localRotation;
        float currentRotation = startRotation;
        //Quaternion endRotationQuaternion = Quaternion.LookRotation(-doorPart.transform.right, Vector3.up);
        Quaternion endRotationQuaternion = startRotationQuaternion * Quaternion.AngleAxis(-95f, Vector3.up);
        Quaternion fullyOpenedRotationQuaternion = startRotationQuaternion * Quaternion.AngleAxis(-105f, Vector3.up);
            //Quaternion.RotateTowards(startRotationQuaternion,endRotationQuaternion,90+15f);
                    
        const float RotateTime = 0.2f;
        const float OpenDoorHitTime = 0.15f;
        float rotateTimer = 0;
        bool haveSlammed = false;
        // Opening rotation fast
        while(rotateTimer < RotateTime) {

            if(!haveSlammed && rotateTimer > OpenDoorHitTime) {
                Debug.Log(".. SLAM");
                SoundMaster.Instance.PlaySound(SoundName.OpenDoorHitWall);
                CameraShakeController.Instance.Shake(ShakeDataType.Door);
                haveSlammed = true;
            }

            rotateTimer += Time.deltaTime;
            doorPart.transform.localRotation = Quaternion.Lerp(startRotationQuaternion,fullyOpenedRotationQuaternion,rotateTimer/RotateTime);
            yield return null;
        }


        rotateTimer = 0;

        // Rotate back and stop
        while(rotateTimer < RotateTime) {
            rotateTimer += Time.deltaTime;
            doorPart.transform.localRotation = Quaternion.Lerp(doorPart.transform.localRotation, endRotationQuaternion, 0.1f);
            yield return null;
        }
        doorPart.transform.localRotation = endRotationQuaternion;            
    }
}
