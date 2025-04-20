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
        boss.Activated = true;

        foreach (GameObject obj in lockObjects)
            obj.SetActive(true);

        // Play door close Sound High

        // Make the Resetter valid to reset Bossarea on death
        unlockBlock.CanBeReset = true;

        // Start Boss Music?
        SoundMaster.Instance.PlayMusic(MusicName.BossMusic);

        // Show Boss health
        UIController.Instance.ShowBossHealth();
    }

}
