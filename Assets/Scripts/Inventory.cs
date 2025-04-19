using System;
using TMPro;
using UnityEngine;
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

    private void Start()
    {
        UpdateInventory();
    }

    internal void AddCoins(int value)
    {
        CoinsHeld++;
        UpdateInventory();
    }
    
    internal bool RemoveCoins(int value)
    {
        if(CoinsHeld < value)
            return false;
        CoinsHeld-=value;
        return true;
    }

    public void AddKey()
    {
        KeysHeld++;
        UpdateInventory();
    }
    
    public void AddBombs()
    {
        BombsHeld++;
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
        Debug.Log("Adding Gem "+gemtype+" to player Inventory");
        gemInventory.ActiveateGem(gemtype);
    }
}
