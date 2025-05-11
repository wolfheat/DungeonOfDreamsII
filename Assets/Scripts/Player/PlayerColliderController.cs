using System;
using UnityEngine;
using Wolfheat.StartMenu;

public class PlayerColliderController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    public static bool IsPlayerInRegainArea = false;

    LayerMask itemsLayerMask;

    private void Start()
    {
        itemsLayerMask = LayerMask.GetMask("Items", "ItemsSeeThrough");
    }

    public void TakeDamage(int amt, bool bombDamage = false) => playerController.TakeDamage(amt, null, bombDamage);


    [SerializeField] TakeFireDamage takeFireDamage;
    public void SetOnFire()
    {
        //Debug.Log("Set on fire");
        takeFireDamage.StartFireTimer();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out HealingArea healingArea)) {

            //Debug.Log("Exiting Healing Area");
            IsPlayerInRegainArea = false;
        }
        else if (other.TryGetComponent(out ShopItem shop)) {

            //Debug.Log("Exiting Shop");
            Shop.Instance.HidePanel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Colliding with "+other.name+" ID:"+other.gameObject.GetInstanceID()+" player ID: "+this.gameObject.GetInstanceID());
        
        if ((1<<other.gameObject.layer & itemsLayerMask) != 0)
        {
            //Debug.Log("Colliding with layer in mask");
            if (other.GetComponent<Bomb>() != null)
                return;
            other.gameObject.GetComponent<Interactable>()?.InteractWith();

        }
        else if(other.TryGetComponent(out ExitPoint exitPoint))
        {
            // Exit points with boss save values class attached saves the players stats so by respawn these are loaded
            if (other.TryGetComponent<BossSaveValues>(out BossSaveValues bossSave)) {
                Debug.Log("SAVE BOSS VALUES HERE");
                Stats.Instance.SaveBossValues();
                LightEnvironmentManager.Instance.SetBossColor();
                UGS_Analytics.Instance.ReachedShopEvent();
            }

            //Play portal sound
            SoundMaster.Instance.PlaySound(SoundName.Teleport);

            if(exitPoint.LeadsTo == -1)
                TransitionScreen.Instance.Darken(PlayerController.Instance.GotoNextStartPosition, 0.14f);
            else
                TransitionScreen.Instance.Darken(PlayerController.Instance.GotoStartPosition, 0.14f);

            // remove all softlock items that might be spawned
            HealingAreaController.Instance.DisableAllHealingAreas();

        }
        else if(other.TryGetComponent(out ExitPortal portal))
        {
            UIController.Instance.ShowWinScreen();
        }
        else if (other.TryGetComponent(out BossActivator bossActivator)) {
            bossActivator.ActivateBoss();
        }else if (other.TryGetComponent(out HealingArea healingArea)) {
            Debug.Log("** Hitting Healing area "+healingArea.name);
            IsPlayerInRegainArea = true;
            healingArea.ActivateBombGenerator();

        }
        else if (other.TryGetComponent(out RespawnPoint respawnPoint)) {
            Stats.Instance.SetNewRespawnPoint(respawnPoint.transform.position);
            if(respawnPoint.TryGetComponent<UnlockBlockAtEntrance>(out UnlockBlockAtEntrance unlockBLocks)) {
                unlockBLocks.UnlockEntranceToBoss();
            }
        }
        else if (other.TryGetComponent(out ShopItem shop)) {
            // ShopItem is also considered Respawn point
            Shop.Instance.ShowPanel();
        }
    }

}
