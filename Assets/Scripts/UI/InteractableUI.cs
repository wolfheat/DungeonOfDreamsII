using System.Collections.Generic;
using UnityEngine;
using Wolfheat.StartMenu;

public class InteractableUI : MonoBehaviour
{
    [SerializeField] InteractableUIItem uiItemPrefab;
    [SerializeField] RectTransform pickedUpRect;

    private List<InteractableUIItem> items;
    private const float PickedScreenLowPosition = 0f;
    private const float PickedScreenHighPosition = -40f;
    private Vector2 pickedUpStartAnchoredPosition;

    private void Start()
    {
        pickedUpStartAnchoredPosition = pickedUpRect.anchoredPosition;
    }

    //public void UpdateItems(List<ItemData> itemDatas, bool resetList)
    //{
    //    if (resetList)
    //        foreach (Transform child in holder.transform)
    //            Destroy(child.gameObject);
    //
    //    foreach (var data in itemDatas)
    //    {
    //        if (data == null) continue;
    //        InteractableUIItem item = Instantiate(uiItemPrefab, holder.transform);
    //        //Debug.Log("data"+data+" resetList: "+resetList);
    //        item.SetName(data.itemName);
    //        item.SetSprite(data.sprite);
    //    }
    //}
    //
    private List<InteractableUIItem> pickedUp = new();
    public void AddPickedUp(ItemData data)
    {
        if (data is PowerUpData)
        {
            if (((PowerUpData)data).powerUpType == PowerUpType.Health)
            {
                Debug.Log("Adding health with heart " + data.value);
                Stats.Instance.AddHealth(data.value); // Dont add health to picked up list?
                //SoundMaster.Instance.PlaySound(SoundName.MoreLifeNow);
                return;
            }if (((PowerUpData)data).powerUpType == PowerUpType.Banana)
            {
                Debug.Log("Refill health with banana " + data.value);
                Stats.Instance.Heal();
                return;
            }else if (((PowerUpData)data).powerUpType == PowerUpType.Coin)
            {
                Debug.Log("Adding coin " + data.value);
                Inventory.Instance.AddCoins(data.value);
                //Stats.Instance.AddHealth(data.value); // Dont add health to picked up list?
                SoundMaster.Instance.PlayCoinSound();

                return;
            }

            // Check If boost is allready active and if so updat the timer
            //BoostUIItem[] uiBoosts = boostsHolder.GetComponentsInChildren<BoostUIItem>();
            //foreach (BoostUIItem item in uiBoosts)
            //{
            //    if (item.nameString == data.itemName)
            //    {
            //        item.AddBoost(data as PowerUpData);
            //        return; // Dont add boosts to picked up list?
            //    }
            //}
            //
            //Debug.Log("Picking Up never used Power Up: " + (data as PowerUpData).itemName);
            //BoostUIItem boostItem = Instantiate(boostuiItemPrefab, boostsHolder.transform);
            //boostItem.SetName(data.itemName);
            //boostItem.SetSprite(data.sprite);
            //boostItem.AddBoost(data as PowerUpData);


            //SoundMaster.Instance.PlaySound(SoundName.Energize);
            return;
        }
        else if (data is UsableData)
        {
            if (((UsableData)data).usableType == UsableType.Bomb)
            {
                Debug.Log("Adding bomb to inventory " + data.value);
                SoundMaster.Instance.PlaySound(SoundName.PickUp);
                Inventory.Instance.AddBombs();
            }else if (((UsableData)data).usableType == UsableType.Map)
            {
                Debug.Log("Adding map " + data.value);
                SoundMaster.Instance.PlaySound(SoundName.PickUpMap);
                Stats.Instance.ActivateMap();
                //Stats.Instance.AddBomb(data.value);
            }else if (((UsableData)data).usableType == UsableType.Key)
            {
                Debug.Log("Adding key ");
                SoundMaster.Instance.PlaySound(SoundName.PickUpKey);
                Inventory.Instance.AddKey();
                //Stats.Instance.AddBomb(data.value);
            }else if (((UsableData)data).usableType == UsableType.Gem)
            {
                int gemtype = ((UsableData)data).value;
                Debug.Log("Adding gem of type "+gemtype);

                SoundMaster.Instance.PlaySound(SoundName.GemPickup);
                Inventory.Instance.Gem(gemtype);
                //Stats.Instance.AddBomb(data.value);
            }
        }

        //InteractableUIItem pickedUpItem = Instantiate(uiItemPrefab, pickedHolder.transform);
        //pickedUpItem.SetName(data.itemName);
        //pickedUpItem.SetSprite(data.sprite);
        //StartCoroutine(pickedUpItem.StartRemoveTimer());
        //pickedUp.Add(pickedUpItem);
        
    } 
    
    public void PositionPickedUpMenu(bool low)
    {
        pickedUpRect.anchoredPosition = new Vector2() { x = pickedUpStartAnchoredPosition.x, y = pickedUpStartAnchoredPosition.y + (low ?  PickedScreenLowPosition: PickedScreenHighPosition) };
    }

}
