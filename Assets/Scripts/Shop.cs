using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;


public enum AltarTypes{Bomb, Chicken, Bananas, Scroll}

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject specificPanel;
    [SerializeField] private GameObject[] ShopItemsSpecific;
    [SerializeField] private Altar[] SpecificAltars;

    [SerializeField] private TextMeshProUGUI bombCostText; 
    [SerializeField] private TextMeshProUGUI keyCostText; 
    [SerializeField] private TextMeshProUGUI oxygenCostText; 

    private int bombCost = 1; 
    private int keyCost = 3; 
    private int otherCost = 5;

    private int activeShop = 0;

    public bool ShopIsOpen => panel.activeSelf;
    public bool ShopSpecificIsOpen => specificPanel.activeSelf;
    public bool WaitForPlayerToReleaseMouseButtonBeforeAllowingButtonCLicks = false;


    public void ShowPanel()
    {
        panel.SetActive(true);
        specificPanel.SetActive(false);
    }

    public void ShowPanel(int specificID)
    {
        Debug.Log(" ");
        WaitForPlayerToReleaseMouseButtonBeforeAllowingButtonCLicks = Mouse.current.leftButton.IsPressed();
        Debug.Log("Open panel with mouse pressed = "+WaitForPlayerToReleaseMouseButtonBeforeAllowingButtonCLicks);

        Debug.Log("Enter specific shop "+specificID);
        // Only show if item is still available
        if (!SpecificAltars[specificID].HasMineral)
            return;

        panel.SetActive(false);
        specificPanel.SetActive(true);
        // Show specific menu for one item
        for (int i = 0; i < ShopItemsSpecific.Length; i++) {
            ShopItemsSpecific[i].SetActive(i == specificID);
        }
        activeShop = specificID;
    }

    internal bool CloseIfOpen()
    {
        if (panel.activeSelf || specificPanel.activeSelf) {
            HidePanel();
            return true;
        }
        return false;
    }
    public void CloseClicked()
    {
        if (WaitForPlayerToReleaseMouseButtonBeforeAllowingButtonCLicks) return;
        HidePanel();
    }
    
    public void HidePanel()
    {
        panel.SetActive(false);
        specificPanel.SetActive(false);
    }

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

        //Debug.Log("Setting Bombs cost to "+bombCost+" = "+bombCostText.text);

        keyCostText.text = "Keys " + keyCost + "g";
        oxygenCostText.text = "Oxygen " + otherCost + "g";

        HidePanel();
    }

    public void BuyBananas()
    {
        Debug.Log("Buying Bananas");
        if (SpecificAltars[(int)AltarTypes.Bananas].gameObject.activeSelf && Inventory.Instance.RemoveCoins(20)) {
            Debug.Log("Bananas");
            SoundMaster.Instance.PlayCoinSound(true);
            SpecificAltars[(int)AltarTypes.Bananas].RemoveItemFromPillar();

            // Activate The boss bananas
            EndRoom.Instance.ActivateBananas();

            HidePanel();
        }
    }

    private void BuyFireSpell()
    {
        Debug.Log("Buying Fire Spell");
        if (SpecificAltars[(int)AltarTypes.Scroll].gameObject.activeSelf && Inventory.Instance.RemoveCoins(30)) {
            Debug.Log("Fire Spell");
            SoundMaster.Instance.PlayCoinSound(true);
            SpecificAltars[(int)AltarTypes.Scroll].RemoveItemFromPillar();
            HidePanel();
        }
    }

    private void BuyChicken()
    {
        Debug.Log("Buying Chicken");
        if (SpecificAltars[(int)AltarTypes.Chicken].gameObject.activeSelf && Inventory.Instance.RemoveCoins(40)) {
            Debug.Log("Speed Up player double");
            Stats.Instance.SetMovemenSpeedMultiplier(0.8f);            
            SoundMaster.Instance.PlayCoinSound(true);
            SpecificAltars[(int)AltarTypes.Chicken].RemoveItemFromPillar();
            HidePanel();
        }
    }
    
    private void Buy20Bombs()
    {
        Debug.Log("Buying 20 Bombs");
        if (SpecificAltars[(int)AltarTypes.Bomb].gameObject.activeSelf && Inventory.Instance.RemoveCoins(20)) {
            Inventory.Instance.AddBombs(20);
            SoundMaster.Instance.PlayCoinSound(true);
            //SpecificAltars[(int)AltarTypes.Bomb].RemoveItemFromPillar();
            HidePanel();
        }
    }

    private void BuyBomb()
    {
        if (Inventory.Instance.RemoveCoins(bombCost)) {
            Inventory.Instance.AddBombs();
            SoundMaster.Instance.PlayCoinSound(true);
        }
    }

    private void BuyKey()
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

    internal void ResetShop()
    {
        foreach (var altar in SpecificAltars) {
            altar.AddItemToPillar();
        }
    }



    public void BuyShopItem()
    {
        if (WaitForPlayerToReleaseMouseButtonBeforeAllowingButtonCLicks) return;

        Debug.Log("Try buy "+activeShop);
        switch (activeShop) {
            case 0:
                Buy20Bombs();
                return;
            case 1:
                BuyChicken();
                return;
            case 2:
                BuyBananas();
                return;
            case 3:
                BuyFireSpell();
                return;

            default:
                break;
        }
    }

    void Update()
    {
        if (WaitForPlayerToReleaseMouseButtonBeforeAllowingButtonCLicks && !Mouse.current.leftButton.IsPressed()) {
            Debug.Log("Mouse released");
            WaitForPlayerToReleaseMouseButtonBeforeAllowingButtonCLicks = false;
        }
    }
}
