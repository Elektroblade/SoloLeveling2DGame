using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIInventory : MonoBehaviour
{
    [System.NonSerialized] public UIInventoryItem[] uIInventoryItems;
    [System.NonSerialized] public int uIInventorySize = 9;
    public GameObject hotbarSlotPrefab;
    public GameObject slotPrefab;
    public Transform slotPanel;
    [System.NonSerialized] private int numberOfSlots = 9;
    [SerializeField] public TextMeshProUGUI highlightedDescription;
    [System.NonSerialized] public int highlightedIndex = 0;
    [System.NonSerialized] public int selectedIndex = -1;
    [System.NonSerialized] private float holdArrowKeyCooldown = 0f;
    [System.NonSerialized] private float holdArrowKeyCooldownMax = 0.08f;

    private void Awake()
    {
        uIInventoryItems = new UIInventoryItem[9];

        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject instance = Instantiate(hotbarSlotPrefab);
            instance.transform.SetParent(slotPanel);
            uIInventoryItems[i] = instance.GetComponentInChildren<UIInventoryItem>();
        }

        uIInventoryItems[highlightedIndex].highlighted = true;

        if (selectedIndex >= 0)
        {
            uIInventoryItems[selectedIndex].selected = true;
        }

        /*
        uIInventoryItems[highlightedIndex].HighlightMe();

        if (highlightedIndex < uIInventorySize && uIInventoryItems[highlightedIndex] != null)
        {
            uIInventoryItems[highlightedIndex].HighlightMe();
        }
        */
    }

    private void Update()
    {
        if (holdArrowKeyCooldown > 0f)
        {
            holdArrowKeyCooldown -= Time.deltaTime;
        }
        else if (holdArrowKeyCooldown < 0f)
        {
            holdArrowKeyCooldown = 0f;
        }

        int prevHighlightedIndex = highlightedIndex;    // Remember previous highlighted index to unhighlight if another index is highlighted.
        int prevSelectedIndex = selectedIndex;

        if (Input.GetKeyDown(KeyCode.UpArrow) && highlightedIndex >= 9)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex -= 9;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && highlightedIndex % 9 < 8 && highlightedIndex < uIInventorySize - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex < uIInventorySize - 9)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex += 9;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex < uIInventorySize - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex = uIInventorySize - 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && highlightedIndex % 9 > 0)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex--;
        }

        if (Input.GetKey(KeyCode.UpArrow) && highlightedIndex >= 9 && holdArrowKeyCooldown == 0)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex -= 9;
        }
        if (Input.GetKey(KeyCode.RightArrow) && highlightedIndex % 9 < 8 && holdArrowKeyCooldown == 0 && highlightedIndex < uIInventorySize - 1)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKey(KeyCode.DownArrow) && highlightedIndex < uIInventorySize - 9 && holdArrowKeyCooldown == 0)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex += 9;
        }
        if (Input.GetKey(KeyCode.LeftArrow) && highlightedIndex % 9 > 0 && holdArrowKeyCooldown == 0)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Alpha6)
            || Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log("Pressed an Alpha key");
            int realSelectedIndex = selectedIndex;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedIndex = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedIndex = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectedIndex = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectedIndex = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                selectedIndex = 4;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                selectedIndex = 5;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                selectedIndex = 6;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                selectedIndex = 7;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                selectedIndex = 8;
            }

            Debug.Log("selectedIndex num = " + selectedIndex);
            DoASwap();
            selectedIndex = realSelectedIndex;
            if (highlightedIndex >= uIInventorySize)
            {
                highlightedIndex = uIInventorySize - 1;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (selectedIndex != highlightedIndex && selectedIndex >= 0 && selectedIndex < uIInventorySize && highlightedIndex < uIInventorySize 
                && (uIInventoryItems[selectedIndex].inventoryItem != null || uIInventoryItems[highlightedIndex].inventoryItem != null))
            {
                DoASwap();

                selectedIndex = -1;
            }
            else if (selectedIndex < 0)
            {
                selectedIndex = highlightedIndex;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedIndex >= 0)
            {
                selectedIndex = -1;
            }
            else if (highlightedIndex < 9)
            {
                InventoryStorage inventoryStorage = GameManager.Instance.inventoryItems;
                InventoryItem inventoryItemToMove = uIInventoryItems[highlightedIndex].inventoryItem;
                inventoryStorage.RemoveItem(inventoryItemToMove);       // Removal must occur first, otherwise the item will simply be stacked upon itself
                inventoryStorage.AddItem(inventoryItemToMove, true, -1);
                UpdateSlot(highlightedIndex, null);
            }
        }
        
        if (uIInventoryItems[highlightedIndex].inventoryItem != null && highlightedIndex != prevHighlightedIndex)
        {
            highlightedDescription.text = uIInventoryItems[highlightedIndex].inventoryItem.description;
            uIInventoryItems[highlightedIndex].HighlightMe();
        }
        else if (highlightedIndex != prevHighlightedIndex)
        {
            highlightedDescription.text = "";
        }
        if (highlightedIndex != prevHighlightedIndex && prevHighlightedIndex < uIInventorySize)
        {
            uIInventoryItems[prevHighlightedIndex].UnhighlightMe();
        }

        if (selectedIndex >= 0 && selectedIndex < uIInventorySize && selectedIndex != prevSelectedIndex)
        {
            uIInventoryItems[selectedIndex].SelectMe();
        }
        if (prevSelectedIndex >= 0 && prevSelectedIndex < uIInventorySize && prevSelectedIndex != selectedIndex)
        {
            uIInventoryItems[prevSelectedIndex].UnselectMe();
        }
    }

    public void UpdateSlot(int slot, InventoryItem inventoryItem)
    {
        if (inventoryItem != null)
        {
            if (uIInventoryItems == null)
            {
                Debug.Log("list of slots does not exist");
            }
            if (uIInventoryItems[slot] == null)
            {
                Debug.Log("slot does not exist");
            }
            uIInventoryItems[slot].UpdateInventoryItem(inventoryItem);
        }
        else
        {
            Debug.Log("updating slot " + slot + " to null..." + ", uIInventorySize = " + uIInventorySize);
            uIInventoryItems[slot].UpdateInventoryItem(null);
        }
    }

    public void AddNewInventoryItem(InventoryItem inventoryItem)
    {
        if (uIInventorySize >= uIInventoryItems.Length)
        {
            UIInventoryItem[] biggerStorage = new UIInventoryItem[uIInventorySize + 1];
            uIInventoryItems.CopyTo(biggerStorage, 0);
            uIInventoryItems = biggerStorage;
        }
        uIInventorySize++;

        GameObject instance = Instantiate(slotPrefab);
        instance.transform.SetParent(slotPanel);
        uIInventoryItems[uIInventorySize - 1] = instance.GetComponentInChildren<UIInventoryItem>();

        UpdateSlot((uIInventorySize - 1), inventoryItem);
    }

    public void RemoveInventoryItem()
    {
        UpdateSlot(uIInventorySize - 1, null);
        GameObject slotToErase = uIInventoryItems[uIInventorySize - 1].transform.parent.gameObject;
        Destroy(uIInventoryItems[uIInventorySize -1]);
        Destroy(slotToErase);

        uIInventoryItems[uIInventorySize - 1] = null;
        uIInventorySize--;
    }

    public void DoASwap()
    {
        Debug.Log("selectedIndex while swapping = " + selectedIndex + ", highlightedIndex = " + highlightedIndex);

        InventoryStorage inventoryStorage = GameManager.Instance.inventoryItems;

        if (uIInventoryItems[selectedIndex].inventoryItem == null && uIInventoryItems[highlightedIndex].inventoryItem.itemType == "WEAPON")
        {
            Debug.Log("SelectedIndex is NULL");
            InventoryItem inventoryItemToMove = uIInventoryItems[highlightedIndex].inventoryItem;
            inventoryStorage.RemoveItem(inventoryItemToMove);       // Removal must occur first, otherwise the item will simply be stacked upon itself
            inventoryStorage.AddItem(inventoryItemToMove, false, selectedIndex);

            if (highlightedIndex < 9)
                UpdateSlot(highlightedIndex, null);
        }
        else if (uIInventoryItems[highlightedIndex].inventoryItem == null && uIInventoryItems[selectedIndex].inventoryItem.itemType == "WEAPON")
        {
            Debug.Log("HighlightedIndex is NULL");
            InventoryItem inventoryItemToMove = uIInventoryItems[selectedIndex].inventoryItem;
            inventoryStorage.RemoveItem(inventoryItemToMove);       // Removal must occur first, otherwise the item will simply be stacked upon itself
            inventoryStorage.AddItem(inventoryItemToMove, false, highlightedIndex);
            if (selectedIndex < 9)
                UpdateSlot(selectedIndex, null);
        }
        else if (selectedIndex < 9 && highlightedIndex < 9)
        {
            Debug.Log("Both items in hotbar");
            inventoryStorage.SwapItemIndices(highlightedIndex, selectedIndex);
        }
        else if (selectedIndex < 9 && uIInventoryItems[highlightedIndex].inventoryItem.itemType == "WEAPON")
        {
            Debug.Log("selectedIndex in hotbar");
            InventoryItem inventoryItemToMove1 = uIInventoryItems[highlightedIndex].inventoryItem;
            inventoryStorage.RemoveItem(inventoryItemToMove1);       // Removal must occur first, otherwise the item will simply be stacked upon itself
            InventoryItem inventoryItemToMove2 = uIInventoryItems[selectedIndex].inventoryItem;
            inventoryStorage.AddItem(inventoryItemToMove1, false, selectedIndex);
            inventoryStorage.AddItem(inventoryItemToMove2, true, highlightedIndex);
        }
        else if (highlightedIndex < 9 && uIInventoryItems[selectedIndex].inventoryItem.itemType == "WEAPON")
        {
            Debug.Log("highlightedIndex in hotbar");
            InventoryItem inventoryItemToMove1 = uIInventoryItems[selectedIndex].inventoryItem;
            inventoryStorage.RemoveItem(inventoryItemToMove1);       // Removal must occur first, otherwise the item will simply be stacked upon itself
            InventoryItem inventoryItemToMove2 = uIInventoryItems[highlightedIndex].inventoryItem;
            inventoryStorage.AddItem(inventoryItemToMove1, false, highlightedIndex);
            inventoryStorage.AddItem(inventoryItemToMove2, true, selectedIndex);
        }
        else if (selectedIndex >= 9 && highlightedIndex >= 9)
        {
            Debug.Log("Not in hotbar");
            inventoryStorage.SwapItemIndices(highlightedIndex, selectedIndex);
        }
    }
}