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

        this.description += "ACQUISITION DIFFICULTY: " + rank + "\nCATEGORY: " + subType + "\n\n" + description + "\n\n";

        if (itemType.CompareTo("WEAPON") == 0)
        {
            AppendWeaponAttributes(stats);
        }

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

    public int FindNextScalingAttributeIndex(string[] scalingAttributes)
    {
        for (int i = 0; i < 5; i++)
        {
            if (scalingAttributes[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    public void AppendWeaponAttributes(Dictionary<string,double> stats)
    {
        string thresholdAttribute = "";
        double thresholdValue = -1;                    // If still -1, there is no threshold found
        string[] scalingAttributes = new string[5];

        if (stats.ContainsKey("Strength="))
        {
            scalingAttributes[0] = "STRENGTH";
        }
        else if (stats.ContainsKey("Strength=="))
        {
            thresholdAttribute =  "STRENGTH";
            thresholdValue = stats["Strength=="];
            scalingAttributes[0] = "STRENGTH";
        }

        if (stats.ContainsKey("Agility="))
        {
            scalingAttributes[FindNextScalingAttributeIndex(scalingAttributes)] = "AGILITY";
        }
        else if (stats.ContainsKey("Agility=="))
        {
            thresholdAttribute = "AGILITY";
            thresholdValue = stats["Agility=="];
            scalingAttributes[FindNextScalingAttributeIndex(scalingAttributes)] = "AGILITY";
        }
        
        if (stats.ContainsKey("Stamina="))
        {
            scalingAttributes[FindNextScalingAttributeIndex(scalingAttributes)] = "STAMINA";
        }
        else if (stats.ContainsKey("Stamina=="))
        {
            thresholdAttribute = "STAMINA";
            thresholdValue = stats["Stamina=="];
            scalingAttributes[FindNextScalingAttributeIndex(scalingAttributes)] = "STAMINA";
        }

        if (stats.ContainsKey("Intelligence="))
        {
            scalingAttributes[FindNextScalingAttributeIndex(scalingAttributes)] = "INTELLIGENCE";
        }
        else if (stats.ContainsKey("Intelligence=="))
        {
            thresholdAttribute = "INTELLIGENCE";
            thresholdValue = stats["Intelligence=="];
            scalingAttributes[FindNextScalingAttributeIndex(scalingAttributes)] = "INTELLIGENCE";
        }
        
        if (stats.ContainsKey("Perception="))
        {
            scalingAttributes[FindNextScalingAttributeIndex(scalingAttributes)] = "PERCEPTION";
        }
        else if (stats.ContainsKey("Perception=="))
        {
            thresholdAttribute = "PERCEPTION";
            thresholdValue = stats["Perception=="];
            scalingAttributes[FindNextScalingAttributeIndex(scalingAttributes)] = "PERCEPTION";
        }

        this.description += "SCALES WITH: ";
        int i = 0;
        while (i < 5 && scalingAttributes[i] != null)
        {
            this.description += scalingAttributes[i];

            if (i < 4 && scalingAttributes[i+1] != null)
            {
                this.description += ", ";
            }

            i++;
        }

        this.description += "\nMINIMUM ATTRIBUTE REQUIREMENT: " + thresholdAttribute + " " + (int) thresholdValue + "\n";

        if (stats.ContainsKey("PhysicalPower"))
        {
            this.description += "PHYSICAL POWER: " + ((int)(10*stats["PhysicalPower"]))/10.0 + "\n";
        }
        if (stats.ContainsKey("MagicalPower"))
        {
            this.description += "MAGICAL POWER: " + ((int)(10*stats["MagicalPower"]))/10.0 + "\n";
        }
    }
}
