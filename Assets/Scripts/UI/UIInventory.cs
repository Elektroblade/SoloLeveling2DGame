using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIInventory : MonoBehaviour
{
    [System.NonSerialized] public UIInventoryItem[] uIInventoryItems;
    public GameObject hotbarSlotPrefab;
    public GameObject slotPrefab;
    public Transform slotPanel;
    [System.NonSerialized] private int numberOfSlots = 9;
    [SerializeField] public TextMeshProUGUI highlightedDescription;
    [System.NonSerialized] private int highlightedIndex = 0;
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

        if (Input.GetKeyDown(KeyCode.UpArrow) && highlightedIndex >= 9)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex -= 9;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && highlightedIndex % 9 < 8 && highlightedIndex < uIInventoryItems.Length - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            int newHighlightedIndex = highlightedIndex;
            while (newHighlightedIndex % 9 < 8 && highlightedIndex < uIInventoryItems.Length - 1)
            {
                newHighlightedIndex++;
                if (uIInventoryItems[newHighlightedIndex].inventoryItem != null)
                {
                    highlightedIndex = newHighlightedIndex;
                    break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex < uIInventoryItems.Length - 9)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex += 9;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex < uIInventoryItems.Length - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex = uIInventoryItems.Length - 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && highlightedIndex % 9 > 0)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            int newHighlightedIndex = highlightedIndex;
            while (newHighlightedIndex % 9 > 0)
            {
                newHighlightedIndex--;
                if (uIInventoryItems[newHighlightedIndex].inventoryItem != null)
                {
                    Debug.Log("index " + newHighlightedIndex + " is not null!");
                    highlightedIndex = newHighlightedIndex;
                    break;
                }
            }
        }

        if (Input.GetKey(KeyCode.UpArrow) && highlightedIndex >= 9 && holdArrowKeyCooldown == 0)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex -= 9;
        }
        if (Input.GetKey(KeyCode.RightArrow) && highlightedIndex % 9 < 8 && holdArrowKeyCooldown == 0 && highlightedIndex < uIInventoryItems.Length - 1)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            int newHighlightedIndex = highlightedIndex;
            while (newHighlightedIndex % 9 < 8 && highlightedIndex < uIInventoryItems.Length - 1)
            {
                newHighlightedIndex++;
                if (uIInventoryItems[newHighlightedIndex].inventoryItem != null)
                {
                    highlightedIndex = newHighlightedIndex;
                    break;
                }
            }
        }
        if (Input.GetKey(KeyCode.DownArrow) && highlightedIndex < uIInventoryItems.Length - 9 && holdArrowKeyCooldown == 0)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex += 9;
        }
        if (Input.GetKey(KeyCode.LeftArrow) && highlightedIndex % 9 > 0 && holdArrowKeyCooldown == 0)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            int newHighlightedIndex = highlightedIndex;
            while (newHighlightedIndex % 9 > 0)
            {
                 
                newHighlightedIndex--;
                if (uIInventoryItems[newHighlightedIndex].inventoryItem != null)
                {
                    highlightedIndex = newHighlightedIndex;
                    break;
                }
            }
        }

        if (uIInventoryItems[highlightedIndex].inventoryItem != null)
        {
            highlightedDescription.text = uIInventoryItems[highlightedIndex].inventoryItem.description;
            uIInventoryItems[highlightedIndex].HighlightMe();

            if (highlightedIndex != prevHighlightedIndex && prevHighlightedIndex < uIInventoryItems.Length)
            {
                uIInventoryItems[prevHighlightedIndex].UnhighlightMe();
            }
        }
    }

    public void UpdateSlot(int slot, InventoryItem inventoryItem)
    {
        if (inventoryItem != null)
        {
            uIInventoryItems[slot].UpdateInventoryItem(inventoryItem);
        }
        else
        {
            uIInventoryItems[slot].UpdateInventoryItem(null);
        }
    }

    public void AddNewInventoryItem(InventoryItem inventoryItem)
    {
        UIInventoryItem[] biggerStorage = new UIInventoryItem[uIInventoryItems.Length + 1];
        uIInventoryItems.CopyTo(biggerStorage, 0);
        uIInventoryItems = biggerStorage;

        GameObject instance = Instantiate(slotPrefab);
        instance.transform.SetParent(slotPanel);
        uIInventoryItems[uIInventoryItems.Length - 1] = instance.GetComponentInChildren<UIInventoryItem>();

        UpdateSlot((uIInventoryItems.Length - 1), inventoryItem);
    }

    public void RemoveInventoryItem()
    {
        UpdateSlot(uIInventoryItems.Length - 1, null);

        UIInventoryItem[] smallerStorage = new UIInventoryItem[uIInventoryItems.Length - 1];
        uIInventoryItems.CopyTo(smallerStorage, 0);
        uIInventoryItems = smallerStorage;
    }
}
