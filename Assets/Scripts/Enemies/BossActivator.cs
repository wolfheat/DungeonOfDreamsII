using UnityEngine;
using Wolfheat.StartMenu;

public class BossActivator : MonoBehaviour
{
    [SerializeField] EnemyController boss;
    [SerializeField] private GameObject[] lockObjects;

    public void ActivateBoss()
    {
        Debug.Log("Activating Boss");
        boss.Activated = true;

        foreach (GameObject obj in lockObjects)
            obj.SetActive(true);

        // Play door close Sound High

        // Start Boss Music?
        SoundMaster.Instance.PlayMusic(MusicName.BossMusic);

        // Show Boss health
        UIController.Instance.ShowBossHealth();
    }

}
