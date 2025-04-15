using System;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject panel;


    [SerializeField] private int bombCost = 10; 
    [SerializeField] private int keyCost = 5; 
    [SerializeField] private int otherCost = 15; 
    public void ShowPanel() => panel.SetActive(true);

    internal void HidePanel() => panel.SetActive(false);

    public static Shop Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void BuyBomb()
    {
        if (Inventory.Instance.RemoveCoins(bombCost)) {
            Inventory.Instance.AddBombs();
        }
    }

    public void BuyKey()
    {
        if (Inventory.Instance.RemoveCoins(keyCost)) {
            Inventory.Instance.AddKey();
        }
         
    }
    
    public void BuyOther()
    {
        if (Inventory.Instance.RemoveCoins(otherCost)) {
            Inventory.Instance.AddOthers();
        }
    }

}
