using UnityEngine;

public class BossDoor : Door
{
    override public bool IsBossDoor => true;

    [SerializeField] private GameObject[] doorGems;
    private bool unlocked = false;
    public bool IsUnlocked => unlocked;

    public bool PlaceGems()
    {
        Debug.Log("Place all players gems in the door");

        bool allPlaced = true;
        for (int i = 0; i < Inventory.Instance.HeldGems.Length; i++) {
            bool held = Inventory.Instance.HeldGems[i];
            doorGems[i].SetActive(held);
            if (!held)
                allPlaced = false;
        }
        if(allPlaced)
            unlocked = true;
        return unlocked;
    }
    internal void Unlock() => unlocked = true;

}
