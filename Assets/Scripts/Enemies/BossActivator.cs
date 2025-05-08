using UnityEngine;
using Wolfheat.StartMenu;

public class BossActivator : MonoBehaviour
{
    [SerializeField] EnemyController boss;
    [SerializeField] private GameObject[] lockObjects;
    [SerializeField] private UnlockBlockAtEntrance unlockBlock;

    public void ActivateBoss()
    {
        Debug.Log("Activating Boss");
        //if (boss == null || boss.Activated)
          //  return;

        foreach (GameObject obj in lockObjects)
            obj.SetActive(true);

        // Play door close Sound High


        // Start Boss Music?
        if (!boss.Activated) {
            SoundMaster.Instance.PlayMusic(MusicName.BossMusic);

            Debug.Log("** Store Boss start time");
            // Store boss start time in stats
            Stats.Instance.StoreBossStartTime();

            // Make the Resetter valid to reset Bossarea on death
            unlockBlock.CanBeReset = true;

            // Show Boss health
            UIController.Instance.ShowBossHealth();
        }

        boss.Activated = true;
    }

}
