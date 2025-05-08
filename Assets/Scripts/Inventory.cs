using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Wolfheat.StartMenu;
public class Inventory : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI keys;
    [SerializeField] private TextMeshProUGUI bombs;
    [SerializeField] private TextMeshProUGUI others;

    [SerializeField] private TextMeshProUGUI coins;

    [SerializeField] private GemInventory gemInventory;

public static Inventory Instance { get; private set; }
    public int KeysHeld { get; private set; } = 0;
    public int BombsHeld { get; private set; } = 0;
    public int OthersHeld { get; private set;} = 0;
    public int CoinsHeld { get; private set;} = 0;
    public bool[] HeldGems => gemInventory.HeldGems;

    public int BombsUsed { get; internal set; } = 0;

    private void Start()
    {
        UpdateInventory();
    }

    internal void AddCoins(int value=1)
    {
        CoinsHeld+=value;
        UpdateInventory();
    }
    
    internal bool RemoveCoins(int value)
    {
        if(CoinsHeld < value) {
            SoundMaster.Instance.PlaySound(SoundName.CantAfford);
            return false;
        }
        CoinsHeld-=value;
        UpdateInventory();
        return true;
    }

    public void AddKey()
    {
        KeysHeld++;
        UpdateInventory();
    }
    
    public void SetCoins(int amt = 1)
    {
        CoinsHeld = amt;
        UpdateInventory();
    }
    
    public void SetBombs(int amt = 1)
    {
        Debug.Log("Set Bombs to "+amt);
        BombsHeld = amt;
        UpdateInventory();
    }
    
    public void AddBombs(int amt = 1)
    {
        BombsHeld+=amt;
        UpdateInventory();
    }

    public void AddOthers()
    {
        OthersHeld++;
        UpdateInventory();
    }
    
    public bool RemoveKey()
    {
        if(KeysHeld <= 0)
            return false;
        KeysHeld--;
        UpdateInventory();
        return true;
    }
    
    public bool RemoveBombs()
    {
        if (BombsHeld <= 0)
            return false;
        BombsHeld--;
        BombsUsed++;
        UpdateInventory();
        return true;
    }
    
    public bool RemoveOthers()
    {
        if (OthersHeld <= 0)
            return false;
        OthersHeld--;
        UpdateInventory();
        return true;
    }


    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateInventory()
    {
        keys.text = KeysHeld.ToString();
        bombs.text = BombsHeld.ToString();
        others.text = OthersHeld.ToString();
        coins.text = CoinsHeld.ToString();
    }

    internal void Gem(int gemtype)
    {
        gemInventory.ActivateGem(gemtype);
    }
}
