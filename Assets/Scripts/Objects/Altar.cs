using System;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] GameObject ownCrystalactivation;
    [SerializeField] GameObject mineralObject;
    [SerializeField] int acceptsMineralID;
    public bool HasMineral { get { return mineralObject.activeSelf; }}
    public int MineralAccepted { get { return acceptsMineralID;}}

    public static Action AltarActivated;

    private void OnEnable()
    {
        Stats.MineralsUpdate += SetAsOwned;
    }
    
    private void OnDisable()
    {
        Stats.MineralsUpdate -= SetAsOwned;
    }

    public void RemoveItemFromPillar()
    {
        mineralObject.SetActive(false);
    }
    
    private void SetAsOwned()
    {
        if (Stats.Instance.MineralsOwned[acceptsMineralID])
            ownCrystalactivation.SetActive(true);
    }

    internal void AddItemToPillar()
    {
        mineralObject.SetActive(true);
    }

    internal bool IsAvailable()
    {
        return mineralObject.activeSelf;
    }
}
