using UnityEngine;
using Wolfheat.StartMenu;

public class BossDoor : Door
{
    override public bool IsBossDoor => true;

    [SerializeField] private GameObject[] doorGems;
    [SerializeField] private GameObject[] doorGemsMockups;
    private bool unlocked = false;
    public bool IsUnlocked => unlocked;

    public bool PlaceGems()
    {
        Debug.Log("Place all players gems in the door");

        bool allPlaced = true;
        bool anyPlaced = false;
        for (int i = 0; i < Inventory.Instance.HeldGems.Length; i++) {
            bool held = Inventory.Instance.HeldGems[i];
            
            if(!doorGems[i].activeSelf && held)
                anyPlaced = true;
            doorGems[i].SetActive(held);
            doorGemsMockups[i].SetActive(!held);
            if (!held)
                allPlaced = false;
        }
        if(allPlaced)
            unlocked = true;
        if (anyPlaced)
            SoundMaster.Instance.PlaySound(SoundName.GemPickup);
        return unlocked;
    }
    internal void Unlock() => unlocked = true;

}
