using UnityEngine;
using Wolfheat.StartMenu;

public class UnlockBLockAteNTER : MonoBehaviour
{
    [SerializeField] private GameObject[] unlockObjects;
    [SerializeField] EnemyController boss;
    
    public void Unlock()
    {
        // Reset Boss
        boss.Reset();

        foreach (GameObject obj in unlockObjects)  
            obj.SetActive(false);

        SoundMaster.Instance.PlayMusic(MusicName.OutDoorMusic);

    }
}
