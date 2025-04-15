using System;
using TMPro;
using UnityEngine;
using Wolfheat.StartMenu;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject panel;


    [SerializeField] private TextMeshProUGUI bombCostText; 
    [SerializeField] private TextMeshProUGUI keyCostText; 
    [SerializeField] private TextMeshProUGUI oxygenCostText; 

    private int bombCost = 1; 
    private int keyCost = 3; 
    private int otherCost = 5; 
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

        ShowPanel();
        // Set the costs here
        bombCostText.text = "Bombs " + bombCost + "g";

        Debug.Log("Setting Bombs cost to "+bombCost+" = "+bombCostText.text);

        keyCostText.text = "Keys " + keyCost + "g";
        oxygenCostText.text = "Oxygen " + otherCost + "g";

        HidePanel();
    }

    public void BuyBomb()
    {
        if (Inventory.Instance.RemoveCoins(bombCost)) {
            Inventory.Instance.AddBombs();
            SoundMaster.Instance.PlayCoinSound(true);
        }
    }

    public void BuyKey()
    {
        if (Inventory.Instance.RemoveCoins(keyCost)) {
            Inventory.Instance.AddKey();
            SoundMaster.Instance.PlayCoinSound(true);
        }
         
    }
    
    public void BuyOther()
    {
        if (Inventory.Instance.RemoveCoins(otherCost)) {
            Inventory.Instance.AddOthers();
            SoundMaster.Instance.PlayCoinSound(true);
        }
    }

}
