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

            Debug.Log("Exiting Healing Area");
            IsPlayerInRegainArea = false;
        }
        else if (other.TryGetComponent(out ShopItem shop)) {

            Debug.Log("Exiting Shop");
            Shop.Instance.HidePanel();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colliding with "+other.name);
        
        if ((1<<other.gameObject.layer & itemsLayerMask) != 0)
        {
            //Debug.Log("Colliding with layer in mask");
            if (other.GetComponent<Bomb>() != null)
                return;
            else if (other.TryGetComponent(out Mineral mineral)) {                
                Stats.Instance.AddMineral(mineral.Data);
            }
            other.gameObject.GetComponent<Interactable>()?.InteractWith();

        }
        else if(other.TryGetComponent(out ExitPortal portal))
        {
            //Debug.Log("Exit portal collission "+portal);
            UIController.Instance.ShowWinScreen();
        }
        else if (other.TryGetComponent(out HealingArea healingArea)) {

            Debug.Log("Entering Healing Area");
            IsPlayerInRegainArea = true;
        }else if (other.TryGetComponent(out RespawnPoint respawnPoint)) {
            Debug.Log("Entering Respawn Point - set this as respawn point");
            Stats.Instance.SetNewRespawnPoint(respawnPoint.transform.position);
        }
        else if (other.TryGetComponent(out ShopItem shop)) {
            // ShopItem is also considered Respawn point
            Debug.Log("Entering Shop");
            Shop.Instance.ShowPanel();
        }
    }

}
