using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public string name;
    public string id;
    public int quantity;
    public string itemType;
    public string subType;
    public string description;
    public Sprite icon;
    public Dictionary<string, double> stats = new Dictionary<string, double>();

    public InventoryItem(string name, string id, string itemType, string subType, string description, Dictionary<string,double> stats)
    {
        this.name = name;
        this.id = id;
        this.quantity = 1;
        this.itemType = itemType;
        this.subType = subType;

        double acquisitionDifficulty = stats["AcquisitionDifficulty"];
        string rank = "";

        if (acquisitionDifficulty < 4)
            rank = "E";
        else if (acquisitionDifficulty < 8)
            rank = "D";
        else if (acquisitionDifficulty < 20)
            rank = "C";
        else if (acquisitionDifficulty < 40)
            rank = "B";
        else if (acquisitionDifficulty < 60)
            rank = "A";
        else if (acquisitionDifficulty < 70)
            rank = "S";
        else if (acquisitionDifficulty < 90)
            rank = "SS";
        else if (acquisitionDifficulty < 100)
            rank = "SSS";
        else
            rank = "NATIONAL";

        this.description += "ACQUISITION DIFFICULTY: " + rank + "\nCATEGORY: " + subType + "\n\n";

        this.description += description;
        this.icon = Resources.Load<Sprite>("UI/InventoryItems/" + id);
        this.stats = stats;
    }

    public InventoryItem(InventoryItem inventoryItem)
    {
        this.name = inventoryItem.name;
        this.id = inventoryItem.id;
        this.quantity = inventoryItem.quantity;
        this.itemType = inventoryItem.itemType;
        this.subType = inventoryItem.subType;
        this.description = inventoryItem.description;
        this.icon = Resources.Load<Sprite>("UI/InventoryItems/" + inventoryItem.id);
        this.stats = inventoryItem.stats;
    }

    public void IncrementQuantity()
    {
        quantity++;
    }
}
