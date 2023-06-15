using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryStorage : MonoBehaviour
{
    InventoryItem[] inventoryItems = new InventoryItem[9];
    int inventorySize = 9;
    public UIInventory inventoryUI;

    public void AddItem(InventoryItem inventoryItem, bool bypassHotbar, int index)
    {
        if (index >= 0)
        {
            if (inventoryItems[index] != null && inventoryItems[index].id.CompareTo(inventoryItem.id) == 0)
            {
                inventoryItems[index].quantity += inventoryItem.quantity;
                inventoryUI.UpdateSlot(index, inventoryItem);
            }
            else
            {
                if (inventoryItems[index] == null && index >= 9) inventorySize++;
                inventoryItems[index] = inventoryItem;
                inventoryUI.UpdateSlot(index, inventoryItem);
            }
            return;
        }

        for(int i = 0; i < inventorySize; i++)
        {
            if (inventoryItems[i] != null)
            {
                InventoryItem checkingItem = inventoryItems[i];
                if (checkingItem.id.CompareTo(inventoryItem.id) == 0)
                {
                    checkingItem.quantity += inventoryItem.quantity;
                    inventoryUI.UpdateSlot(i, inventoryItem);
                    return;                 // We found a duplicate and increased the quantity!
                }
            }
        }
        // If we did not find a duplicate, proceed.

        if (inventoryItem.itemType == "WEAPON" && !bypassHotbar)
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
        
        Debug.Log("inventorySize = " + inventorySize + ", inventoryItems.Length = " + inventoryItems.Length);

        if (inventorySize >= inventoryItems.Length)
        {
            InventoryItem[] biggerStorage = new InventoryItem[inventorySize + 1];
            inventoryItems.CopyTo(biggerStorage, 0);
            inventoryItems = biggerStorage;
        }
        inventorySize++;

        inventoryItems[inventorySize - 1] = inventoryItem;
        inventoryUI.AddNewInventoryItem(inventoryItem);
    }

    public InventoryItem Find(string id)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventoryItems[i] != null && inventoryItems[i].id.CompareTo(id) == 0)
            {
                return inventoryItems[i];
            }
        }
        return null;
    }

    public void RemoveItem(InventoryItem inventoryItem)
    {
        Debug.Log("Removing " + inventoryItem.name);
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventoryItems[i] != null && inventoryItems[i].id == inventoryItem.id)
            {
                Debug.Log("removing at index = " + i + ", inventorySize = " + inventorySize);
                inventoryItems[i] = null;
                inventoryUI.UpdateSlot(i, null);
                if (i >= 9)
                {
                    Debug.Log("j = " + i + ", inventorySize - 1 = " + (inventorySize - 1));
                    for (int j = i; j < inventorySize - 1; j++)
                    {
                        InventoryItem temp = inventoryItems[j+1];
                        inventoryItems[j] = temp;
                        inventoryUI.UpdateSlot(j, temp);

                        Debug.Log("j + 1 = " + (j+1) + ", id = " + inventoryItems[j+1].id);
                    }

                    inventoryItems[inventorySize - 1] = null;
                    inventorySize--;

                    inventoryUI.RemoveInventoryItem();
                }

                break;
            }
        }
    }

    public void SwapItemIndices(int slot1, int slot2)
    {
        Debug.Log("slot1 = " + slot1 + ", slot2 = " + slot2 + ", inventorySize = " + inventorySize);

        if (slot1 != slot2 && slot1 >= 0 && slot2 >= 0 && slot1 < inventorySize && slot2 < inventorySize)
        {
            InventoryItem tempItem1 = inventoryItems[slot1];
            InventoryItem tempItem2 = inventoryItems[slot2];
            inventoryItems[slot1] = tempItem2;
            inventoryItems[slot2] = tempItem1;
            inventoryUI.UpdateSlot(slot1, tempItem2);
            inventoryUI.UpdateSlot(slot2, tempItem1);
        }
    }

    public void Clear()
    {
        InventoryItem[] emptyStorage = new InventoryItem[9];
        inventoryItems.CopyTo(emptyStorage, 0);
        inventoryItems = emptyStorage;

        for (int i = 0; i < inventorySize; i++)
        {
            inventoryItems[i] = null;
        }

        inventorySize = 9;
    }
}
