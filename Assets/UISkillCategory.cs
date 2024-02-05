using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UISkillCategory : MonoBehaviour
{
    [System.NonSerialized] public UIClassItem[] uIClassItems;
    [System.NonSerialized] public int uISkillCategorySize = 12;
    public GameObject slotPrefab;
    public Transform slotPanel;
    [System.NonSerialized] private int numberOfSlots = 12;
    [System.NonSerialized] public int highlightedIndex = 0;
    [System.NonSerialized] public int selectedIndex = -1;
    [System.NonSerialized] private float holdArrowKeyCooldown = 0f;
    [System.NonSerialized] private float holdArrowKeyCooldownMax = 0.08f;

    private void Awake()
    {
        uIClassItems = new UIClassItem[9];

        for (int i = 0; i < numberOfSlots; i++)
        {
            instance.transform.SetParent(slotPanel);
            uIClassItems[i] = instance.GetComponentInChildren<UIClassItem>();
        }

        uIClassItems[highlightedIndex].highlighted = true;

        if (selectedIndex >= 0)
        {
            uIClassItems[selectedIndex].selected = true;
        }

        /*
        uIClassItems[highlightedIndex].HighlightMe();

        if (highlightedIndex < uISkillCategorySize && uIClassItems[highlightedIndex] != null)
        {
            uIClassItems[highlightedIndex].HighlightMe();
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
        if (Input.GetKeyDown(KeyCode.RightArrow) && highlightedIndex % 9 < 8 && highlightedIndex < uISkillCategorySize - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex < uISkillCategorySize - 9)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex += 9;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex < uISkillCategorySize - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex = uISkillCategorySize - 1;
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
        if (Input.GetKey(KeyCode.RightArrow) && highlightedIndex % 9 < 8 && holdArrowKeyCooldown == 0 && highlightedIndex < uISkillCategorySize - 1)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKey(KeyCode.DownArrow) && highlightedIndex < uISkillCategorySize - 9 && holdArrowKeyCooldown == 0)
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
            if (highlightedIndex >= uISkillCategorySize)
            {
                highlightedIndex = uISkillCategorySize - 1;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (selectedIndex != highlightedIndex && selectedIndex >= 0 && selectedIndex < uISkillCategorySize && highlightedIndex < uISkillCategorySize 
                && (uIClassItems[selectedIndex].classItem != null || uIClassItems[highlightedIndex].classItem != null))
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
                InventoryStorage inventoryStorage = GameManager.Instance.classItems;
                ClassItem classItemToMove = uIClassItems[highlightedIndex].classItem;
                inventoryStorage.RemoveItem(classItemToMove);       // Removal must occur first, otherwise the item will simply be stacked upon itself
                inventoryStorage.AddItem(classItemToMove, true, -1);
                UpdateSlot(highlightedIndex, null);
            }
        }
        
        if (uIClassItems[highlightedIndex].classItem != null && highlightedIndex != prevHighlightedIndex)
        {
            highlightedDescription.text = uIClassItems[highlightedIndex].classItem.description;
            uIClassItems[highlightedIndex].HighlightMe();
        }
        else if (highlightedIndex != prevHighlightedIndex)
        {
            highlightedDescription.text = "";
        }
        if (highlightedIndex != prevHighlightedIndex && prevHighlightedIndex < uISkillCategorySize)
        {
            uIClassItems[prevHighlightedIndex].UnhighlightMe();
        }

        if (selectedIndex >= 0 && selectedIndex < uISkillCategorySize && selectedIndex != prevSelectedIndex)
        {
            uIClassItems[selectedIndex].SelectMe();
        }
        if (prevSelectedIndex >= 0 && prevSelectedIndex < uISkillCategorySize && prevSelectedIndex != selectedIndex)
        {
            uIClassItems[prevSelectedIndex].UnselectMe();
        }
    }

    public void UpdateSlot(int slot, ClassItem classItem)
    {
        if (classItem != null)
        {
            uIClassItems[slot].UpdateClassItem(classItem);
        }
        else
        {
            Debug.Log("updating slot " + slot + " to null..." + ", uISkillCategorySize = " + uISkillCategorySize);
            uIClassItems[slot].UpdateClassItem(null);
        }
    }

    public void AddNewClassItem(ClassItem classItem)
    {
        if (uISkillCategorySize >= uIClassItems.Length)
        {
            UIClassItem[] biggerStorage = new UIClassItem[uISkillCategorySize + 1];
            uIClassItems.CopyTo(biggerStorage, 0);
            uIClassItems = biggerStorage;
        }
        uISkillCategorySize++;

        GameObject instance = Instantiate(slotPrefab);
        instance.transform.SetParent(slotPanel);
        uIClassItems[uISkillCategorySize - 1] = instance.GetComponentInChildren<UIClassItem>();

        UpdateSlot((uISkillCategorySize - 1), classItem);
    }
}