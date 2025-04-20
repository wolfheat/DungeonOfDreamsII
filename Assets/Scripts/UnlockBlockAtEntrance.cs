using UnityEngine;
using Wolfheat.StartMenu;

public class UnlockBlockAtEntrance : MonoBehaviour
{
    [SerializeField] private GameObject[] unlockObjects;
    [SerializeField] EnemyController boss;
    
    public bool CanBeReset { get; set; } = true;

    public void UnlockEntranceToBoss()
    {
        if(!CanBeReset) return;
        CanBeReset = false;

        Debug.Log("Unlock Entrance to Boss");
        Debug.Log("Reset Boss");
        // Player collides with this, allow reset if have passed by the bossentrance once

        // Reset Boss
        boss.Reset();

        // Deactivate Bananas
        EndRoom.Instance.ActivateBananas(false);
        
        // Open The door to Boss
        foreach (GameObject obj in unlockObjects)  
            obj.SetActive(false);

        // PLay Correct Music
        SoundMaster.Instance.PlayMusic(MusicName.IndoorMusic);



    }
}
