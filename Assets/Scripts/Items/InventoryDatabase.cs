using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDatabase : MonoBehaviour
{
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    private void Awake()
    {
        Debug.Log("Inventory Database is awake.");
        BuildDatabase();
        Debug.Log("Database has been built.");
    }

    public InventoryItem GetInventoryItem(string id)
    {
        foreach (InventoryItem item in inventoryItems)
        {
            //Debug.Log(item.name);
        }

        return inventoryItems.Find(inventoryItem => (inventoryItem.id.CompareTo(id) == 0));
    }

    void BuildDatabase()
    {
        inventoryItems = new List<InventoryItem>() {

            // Weapons
            new InventoryItem("Barca's Dagger", "BarcasDagger", "WEAPON", "DAGGER", "A dagger used by the great warrior Barca. A powerful spell of weightlessness imbued in the dagger allows the user to be more agile while wielding it.\nATTACK POWER +100\nAGILITY +10",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 50.0},
                {"Agility", 50.0},
                {"AttackPower", 100.0},
                {"Agility+", 10.0}
            }),

            new InventoryItem("Morgul Flail", "MorgulFlail", "WEAPON", "FLAIL", "An unweildy flail wrought from cursed steel with a great, deadly head capable of shattering shields or killing enemies in one swing.",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 65.0},
                {"Strength=", 65.0},               // = means Threshold. First Threshold is used for attackRate scaling.
                {"AttackPower", (65.0 / 4.0) * 3.3 * 0.95},    // (Level / combo value) * combo time * stagger modifier
                {"Stagger*", 1.5}
            }),

            //Helmets
            new InventoryItem("Crimson Knight's Helmet", "CrimsonKnightsHelmet", "ARMOUR", "HELMET", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 60.0}
            }),

            // Collars
            new InventoryItem("Warden's Collar", "WardensCollar", "ADORNMENT", "COLLAR", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 40.0}
            }),

            // Chestplates
            new InventoryItem("High-Rank Knight's Chestplate", "HighRankKnightsChestplate", "ARMOUR", "CHESTPLATE", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 20.0}
            }),

            // Gloves
            new InventoryItem("High-Rank Knight's Gauntlets", "HighRankKnightsGauntlets", "ARMOUR", "GLOVES", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 20.0}
            }),

            // Rings
            new InventoryItem("High-Rank Mage's Ring", "HighRankMagesRing", "ADORNMENT", "RING", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 20.0}
            }),

            // Boots
            new InventoryItem("Mid-Rank Assassin's Boots", "MidRankAssassinsBoots", "ARMOUR", "BOOTS", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 20.0}
            })
            
            // Next item
        };
    }
}
