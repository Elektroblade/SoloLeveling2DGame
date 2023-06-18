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
    [System.NonSerialized] public InventoryStorage inventoryItems;
    [SerializeField] public UIStatus uIStatus;
    public InventoryDatabase inventoryDatabase;
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

        audioSource = GetComponent<AudioSource>();
        inventoryItems = GetComponent<InventoryStorage>();
        inventoryItems.inventoryUI.gameObject.SetActive(false);
        uIStatus.Goodbye();
        GiveItem("BarcasDagger");
        GiveItem("MorgulFlail");
        GiveItem("CrimsonKnightsHelmet");
        GiveItem("WardensCollar");
        GiveItem("HighRankKnightsChestplate");
        GiveItem("HighRankKnightsGauntlets");
        GiveItem("HighRankMagesRing");
        GiveItem("MidRankAssassinsBoots");
        GiveItem("TheOneRing");

        //RemoveInventoryItem("MidRankAssassinsBoots");
        //RemoveInventoryItem("CrimsonKnightsHelmet");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inventoryItems.inventoryUI.gameObject.activeSelf)
            {
                NewPlayer.Instance.hasInventoryOpen = true;
            }
            else
            {
                NewPlayer.Instance.hasInventoryOpen = false;
            }
            
            inventoryItems.inventoryUI.gameObject.SetActive(!inventoryItems.inventoryUI.gameObject.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!uIStatus.gameObject.activeSelf)
            {
                NewPlayer.Instance.hasStatusOpen = true;
                uIStatus.gameObject.SetActive(true);
                uIStatus.WakeMeUp();
            }
            else
            {
                NewPlayer.Instance.hasStatusOpen = false;
                uIStatus.Goodbye();
            }
        }
    }

    public void GiveItem(string id)
    {
        //Debug.Log("Trying to add item: " + name);
        InventoryItem inventoryItemToAdd = inventoryDatabase.GetInventoryItem(id);
        inventoryItems.AddItem(inventoryItemToAdd, false, -1);
        //Debug.Log("Added item: " + inventoryItemToAdd.name);

        //hud.SetInventoryImage(Resources.Load<Sprite>("UI/InventoryItems/" + name));
    }

    public InventoryItem CheckForInventoryItem(string id)
    {
        return inventoryItems.Find(id);
    }

    public void RemoveInventoryItem(string id)
    {
        InventoryItem inventoryItemToRemove = CheckForInventoryItem(id);
        if (inventoryItemToRemove != null)
        {
            Debug.Log("id to remove = " + id + ", found id " + inventoryItemToRemove.id);
            inventoryItems.RemoveItem(inventoryItemToRemove);
            Debug.Log("Item removed: " + inventoryItemToRemove.id);
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
