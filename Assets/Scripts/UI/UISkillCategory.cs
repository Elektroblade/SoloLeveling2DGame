using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UISkillCategory : MonoBehaviour
{
    [System.NonSerialized] public List<UIClassItem> uIClassItems;
    [System.NonSerialized] public float uISkillCategoryDenominator = 0f;
    public GameObject classSlotPrefab;
    public Transform slotPanel;
    [SerializeField] public TextMeshProUGUI highlightedDescription;
    [System.NonSerialized] public int highlightedIndex = 0;
    [System.NonSerialized] public int selectedIndex = -1;
    [System.NonSerialized] private float holdArrowKeyCooldown = 0f;
    [System.NonSerialized] private float holdArrowKeyCooldownMax = 0.08f;

    private void Awake()
    {
        uIClassItems = new List<UIClassItem>();

        if (uIClassItems.Count > 0)
        {
            uIClassItems[highlightedIndex].highlighted = true;
            highlightedDescription.text = uIClassItems[highlightedIndex].classItem.description;
        }

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
        if (uIClassItems.Count > 0)
            HandleUserInput();
    }

    private void HandleUserInput()
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
        if (Input.GetKeyDown(KeyCode.RightArrow) && highlightedIndex % 9 < 8 && highlightedIndex < uIClassItems.Count - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex < uIClassItems.Count - 9)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex += 9;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex < uIClassItems.Count - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex = uIClassItems.Count - 1;
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
        if (Input.GetKey(KeyCode.RightArrow) && highlightedIndex % 9 < 8 && holdArrowKeyCooldown == 0 && highlightedIndex < uIClassItems.Count - 1)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKey(KeyCode.DownArrow) && highlightedIndex < uIClassItems.Count - 9 && holdArrowKeyCooldown == 0)
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

            if (highlightedIndex >= uIClassItems.Count)
            {
                highlightedIndex = uIClassItems.Count - 1;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            // Change to the Hollow Knight charm system but for classes
            if (selectedIndex != highlightedIndex && selectedIndex >= 0 && selectedIndex < uIClassItems.Count && highlightedIndex < uIClassItems.Count 
                && (uIClassItems[selectedIndex].classItem != null || uIClassItems[highlightedIndex].classItem != null))
            {
                //DoASwap();

                selectedIndex = -1;
            }
            else if (selectedIndex < 0)
            {
                selectedIndex = highlightedIndex;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Do nothing
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
        if (highlightedIndex != prevHighlightedIndex && prevHighlightedIndex < uIClassItems.Count)
        {
            uIClassItems[prevHighlightedIndex].UnhighlightMe();
        }

        if (selectedIndex >= 0 && selectedIndex < uIClassItems.Count && selectedIndex != prevSelectedIndex)
        {
            uIClassItems[selectedIndex].SelectMe();
        }
        if (prevSelectedIndex >= 0 && prevSelectedIndex < uIClassItems.Count && prevSelectedIndex != selectedIndex)
        {
            uIClassItems[prevSelectedIndex].UnselectMe();
        }
    }

    public void RebuildList(List<List<ClassItem>> classItemCategories)
    {
        for (int i = 0; i < uIClassItems.Count; i++)
        {
            Destroy(uIClassItems[i].gameObject);
        }
        uIClassItems.Clear();
        uISkillCategoryDenominator = 1;

        for (int i = 0; i < classItemCategories.Count; i++)
        {
            for (int j = 0; j < classItemCategories[i].Count; j++)
            {
                GameObject instance = Instantiate(classSlotPrefab);
                instance.transform.SetParent(slotPanel);
                instance.GetComponentInChildren<UIClassItem>().UpdateClassItem(classItemCategories[i][j], uISkillCategoryDenominator + 1);
                uIClassItems.Add(instance.GetComponentInChildren<UIClassItem>());

                uISkillCategoryDenominator += 2;
            }

            uISkillCategoryDenominator++;
        }

        for (int i = 0; i < uIClassItems.Count; i++)
        {
            RectTransform rectTransform = uIClassItems[i].transform.parent.GetComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2((uIClassItems[i].numerator/uISkillCategoryDenominator), 0.5f);
            rectTransform.anchorMax = new Vector2((uIClassItems[i].numerator/uISkillCategoryDenominator), 0.5f);
            rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
        }
    }
}