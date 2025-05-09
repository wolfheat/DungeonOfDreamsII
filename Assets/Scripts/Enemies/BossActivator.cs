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

        // Close door always
        foreach (GameObject obj in lockObjects)
            obj.SetActive(true);

        // If boss is dead do not activate boss stuff
        if (boss == null || boss.Dead) {
            Debug.Log("Boss is dead");


        }else if (!boss.Activated) {
        // Start Boss Music?
            SoundMaster.Instance.PlayMusic(MusicName.BossMusic);

            Debug.Log("** Store Boss start time");
            // Store boss start time in stats
            Stats.Instance.StoreBossStartTime();

            // Make the Resetter valid to reset Bossarea on death
            unlockBlock.CanBeReset = true;

            // Show Boss health
            UIController.Instance.ShowBossHealth();

            boss.Activated = true;
        }

    }

}
