using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public List<UIInventoryItem> uIInventoryItems = new List<UIInventoryItem>();
    public GameObject slotPrefab;
    public Transform slotPanel;
    public int numberOfSlots = 16;

    private void Awake()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject instance = Instantiate(slotPrefab);
            instance.transform.SetParent(slotPanel);
            uIInventoryItems.Add(instance.GetComponentInChildren<UIInventoryItem>());
        }
    }

    public void UpdateSlot(int slot, InventoryItem inventoryItem)
    {
        uIInventoryItems[slot].UpdateInventoryItem(inventoryItem);
    }
    public void AddNewInventoryItem(InventoryItem inventoryItem)
    {
        UpdateSlot(uIInventoryItems.FindIndex(i => i.inventoryItem == null), inventoryItem);
    }
    public void RemoveInventoryItem(InventoryItem inventoryItem)
    {
        UpdateSlot(uIInventoryItems.FindIndex(i => i.inventoryItem == inventoryItem), null);
    }
}
