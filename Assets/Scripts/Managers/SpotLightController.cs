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
            Debug.Log("Spotlight off");
            spotLight.enabled = false;
            Debug.Log("Play outdoor music");
            SoundMaster.Instance.PlayMusic(MusicName.OutDoorMusic);
        }else if(other.gameObject.layer == AltarRoomTrigger)
        {
            SoundMaster.Instance.PlayMusic(MusicName.IndoorMusic);
        }else if(other.gameObject.layer == BossRoom)
        {
            Debug.Log("PLAY BOSS MUSIC");
            SoundMaster.Instance.PlayMusic(MusicName.BossMusic);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == postProcessingRoom)
        {
            //Debug.Log("Turn On Player Spotlight and Resume Music");
            SoundMaster.Instance.PlayMusic(MusicName.OutDoorMusic);
            SoundMaster.Instance.PlayerExitingStartRoom();
            spotLight.enabled = true;            
        }
        else if (other.gameObject.layer == AltarRoomTrigger)
        {
            SoundMaster.Instance.PlayMusic(MusicName.OutDoorMusic);
        }
    }

}
