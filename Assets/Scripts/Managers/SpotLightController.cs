using UnityEngine;
using Wolfheat.StartMenu;

public class SpotLightController : MonoBehaviour
{
    [SerializeField] Light spotLight;

    int postProcessingRoom;
    int AltarRoomTrigger;
    int BossRoom;

    private void Start()
    {
        postProcessingRoom = LayerMask.NameToLayer("PostProcessingRoom");
        AltarRoomTrigger = LayerMask.NameToLayer("AltarRoomTrigger");
        BossRoom = LayerMask.NameToLayer("BossPostProcessingRoom");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == postProcessingRoom)
        {
            SoundMaster.Instance.PlayMusic(MusicName.OutDoorMusic);
            LightEnvironmentManager.Instance.SetNormalColor();            
        }
        else if(other.gameObject.layer == AltarRoomTrigger)
        {
            SoundMaster.Instance.PlayMusic(MusicName.IndoorMusic);
        }else if(other.gameObject.layer == BossRoom)
        {
            SoundMaster.Instance.PlayMusic(MusicName.BossMusic);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == postProcessingRoom)
        {
            //Debug.Log("Turn On Player Spotlight and Resume Music");
            SoundMaster.Instance.PlayMusic(MusicName.OutDoorMusic);
            spotLight.enabled = true;            
        }
        else if (other.gameObject.layer == AltarRoomTrigger)
        {
            SoundMaster.Instance.PlayMusic(MusicName.OutDoorMusic);
        }
    }

}
