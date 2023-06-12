using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryStorage : MonoBehaviour
{
    InventoryItem[] inventoryItems = new InventoryItem[9];
    public UIInventory inventoryUI;

    public void AddItem(InventoryItem inventoryItem)
    {
        for(int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] != null)
            {
                InventoryItem checkingItem = inventoryItems[i];
                if (checkingItem.id.CompareTo(inventoryItem.id) == 0)
                {
                    checkingItem.IncrementQuantity();
                    return;                 // We found a duplicate and increased the quantity!
                }
            }
        }
        // If we did not find a duplicate, proceed.

        if (inventoryItem.itemType == "WEAPON")
        {
            for (int i = 0; i < 9; i++)
            {
                if (inventoryItems[i] == null)
                {
                    inventoryItems[i] = inventoryItem;
                    inventoryUI.UpdateSlot(i, inventoryItem);
                    return;             // We found an empty hotbar slot!
                }
            }
        }
        // If we did not find an empty slot on the hotbar, proceed.
        
        InventoryItem[] biggerStorage = new InventoryItem[inventoryItems.Length + 1];
        inventoryItems.CopyTo(biggerStorage, 0);
        inventoryItems = biggerStorage;

        inventoryItems[inventoryItems.Length - 1] = inventoryItem;
        inventoryUI.AddNewInventoryItem(inventoryItem);
    }

    public InventoryItem Find(string id)
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] != null && inventoryItems[i].id.CompareTo(id) == 0)
            {
                return inventoryItems[0];
            }
        }
        return null;
    }

    public void Remove(InventoryItem inventoryItem)
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] != null && inventoryItems[i].id == inventoryItem.id)
            {
                inventoryItems[i] = null;
                inventoryUI.UpdateSlot(i, null);
                if (i >= 9)
                {
                    for (int j = i; j < inventoryItems.Length - 1; j++)
                    {
                        InventoryItem temp = inventoryItems[j+1];
                        inventoryItems[j] = temp;
                        inventoryUI.UpdateSlot(j, temp);
                    }

                    InventoryItem[] smallerStorage = new InventoryItem[inventoryItems.Length - 1];
                    inventoryItems.CopyTo(smallerStorage, 0);
                    inventoryItems = smallerStorage;
                    inventoryUI.RemoveInventoryItem();
                }
            }
        }
    }

    public void Clear()
    {
        InventoryItem[] emptyStorage = new InventoryItem[9];
        inventoryItems.CopyTo(emptyStorage, 0);
        inventoryItems = emptyStorage;

        for (int i = 0; i < inventoryItems.Length; i++)
        {
            inventoryItems[i] = null;
        }
    }
}
