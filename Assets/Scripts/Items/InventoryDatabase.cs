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

    public InventoryItem GetInventoryItem(string name)
    {
        foreach (InventoryItem item in inventoryItems)
        {
            //Debug.Log(item.name);
        }

        return inventoryItems.Find(inventoryItem => (inventoryItem.name.CompareTo(name) == 0));
    }

    void BuildDatabase()
    {
        inventoryItems = new List<InventoryItem>() {

            // Weapons
            new InventoryItem("Barca's Dagger", "ACQUISITION DIFFICULTY: A\nCATEGORY:Dagger\n\nA dagger used by the great warrior Barca. A powerful spell of weightlessness imbued in the dagger allows the user to be more agile while wielding it.\nATTACK POWER +100\nAGILITY +10",
            new Dictionary<string, int>
            {
                {"AttackPower", 110},
                {"Agility", 10}
            }),

            //Helmets
            new InventoryItem("Crimson Knight's Helmet", "ACQUISITION DIFFICULTY: S",
            new Dictionary<string, int>{}),

            // Collars
            new InventoryItem("Warden's Collar", "ACQUISITION DIFFICULTY: A",
            new Dictionary<string, int>{}),

            // Chestplates
            new InventoryItem("High-Rank Knight's Chestplate", "ACQUISITION DIFFICULTY: B",
            new Dictionary<string, int>{}),

            // Gloves
            new InventoryItem("High-Rank Knight's Gauntlets", "ACQUISITION DIFFICULTY: B",
            new Dictionary<string, int>{}),

            // Rings
            new InventoryItem("High-Rank Mage's Ring", "ACQUISITION DIFFICULTY: B",
            new Dictionary<string, int>{}),

            // Boots
            new InventoryItem("Mid-Rank Assassin's Boots", "ACQUISITION DIFFICULTY: B",
            new Dictionary<string, int>{})
            
            // Next item
        };
    }
}
