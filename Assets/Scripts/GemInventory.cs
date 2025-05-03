using System;
using UnityEngine;

public class GemInventory : MonoBehaviour
{
    [SerializeField] private Gem[] gems;
    private bool[] heldGems;
    public bool[] HeldGems => heldGems;
    private void Start()
    {
        heldGems = new bool[gems.Length];
    }

    public void ActivateGem(int index)
    {
        Debug.Log("Show gem index "+index+" in the gem inventory.");
        gems[index].Show(true);
        heldGems[index] = true;
    }
    public void InactivateAllGems()
    {
        for (int i = 0; i < gems.Length; i++) {
            Gem gem = gems[i];
            heldGems[i] = false;
            gem.Show(false);
        }
    }

}
