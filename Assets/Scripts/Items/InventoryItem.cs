using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public string name;
    public string description;
    public Sprite icon;
    public Dictionary<string, int> stats = new Dictionary<string, int>();

    public InventoryItem(string name, string description, Dictionary<string,int> stats)
    {
        this.name = name;
        this.description = description;
        this.icon = Resources.Load<Sprite>("UI/InventoryItems/" + name);
        this.stats = stats;
    }

    public InventoryItem(InventoryItem inventoryItem)
    {
        this.name = inventoryItem.name;
        this.description = inventoryItem.description;
        this.icon = Resources.Load<Sprite>("UI/InventoryItems/" + inventoryItem.name);
        this.stats = inventoryItem.stats;
    }
}
