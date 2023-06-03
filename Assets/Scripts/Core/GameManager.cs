using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Manages inventory, keeps several component references, and any other future control of the game itself you may need*/

public class GameManager : MonoBehaviour
{
    public AudioSource audioSource; //A primary audioSource a large portion of game sounds are passed through
    public DialogueBoxController dialogueBoxController;
    public HUD hud; //A reference to the HUD holding your health UI, coins, dialogue, etc.
    private static GameManager instance;
    [SerializeField] public AudioTrigger gameMusic;
    [SerializeField] public AudioTrigger gameAmbience;
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public InventoryDatabase inventoryDatabase;
    public UIInventory inventoryUI;
    public Transform pfDamagePopup;
    [System.NonSerialized] public int testingLocalDifficulty = 000;
    [System.NonSerialized] public int testingLocalDifficultyVariance = 30;

    // Singleton instantiation
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<GameManager>();
            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start Called");

        inventoryUI.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        GiveItem("Barca's Dagger");
        GiveItem("Crimson Knight's Helmet");
        GiveItem("Warden's Collar");
        GiveItem("High-Rank Knight's Chestplate");
        GiveItem("High-Rank Knight's Gauntlets");
        GiveItem("High-Rank Mage's Ring");
        GiveItem("Mid-Rank Assassin's Boots");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inventoryUI.gameObject.activeSelf)
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
            }
            
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
        }
    }

    public void GiveItem(string name)
    {
        //Debug.Log("Trying to add item: " + name);
        InventoryItem inventoryItemToAdd = inventoryDatabase.GetInventoryItem(name);
        if (inventoryItemToAdd == null)
        {
            Debug.Log("null!");
        }
        inventoryItems.Add(inventoryItemToAdd);
        inventoryUI.AddNewInventoryItem(inventoryItemToAdd);
        //Debug.Log("Added item: " + inventoryItemToAdd.name);

        //hud.SetInventoryImage(Resources.Load<Sprite>("UI/InventoryItems/" + name));
    }

    public InventoryItem CheckForInventoryItem(string name)
    {
        return inventoryItems.Find(inventoryItem => (inventoryItem.name.CompareTo(name) == 0));
    }

    public void RemoveInventoryItem(string name)
    {
        InventoryItem inventoryItemToRemove = CheckForInventoryItem(name);
        if (inventoryItemToRemove != null)
        {
            inventoryItems.Remove(inventoryItemToRemove);
            inventoryUI.RemoveInventoryItem(inventoryItemToRemove);
            Debug.Log("Item removed: " + inventoryItemToRemove.name);
        }

        //hud.SetInventoryImage(hud.blankUI);
    }

    public void ClearInventory()
    {   
        inventoryItems.Clear();
        //hud.SetInventoryImage(hud.blankUI);
    }

    public double GetRandomDouble(double minimum, double maximum)
    {
        System.Random random = new System.Random();
        return random.NextDouble() * (maximum - minimum) + minimum;
    }
}
