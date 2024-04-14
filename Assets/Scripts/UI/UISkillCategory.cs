using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UISkillCategory : MonoBehaviour, SubMenu
{
    [System.NonSerialized] public List<UIClassItem> uIClassItems = new List<UIClassItem>();
    [SerializeField] public UISkillSelected uISkillSelected;
    [System.NonSerialized] public float uISkillCategoryDenominator = 0f;
    public GameObject classSlotPrefab;
    public Transform slotPanel;
    [SerializeField] public TextMeshProUGUI highlightedDescription;
    [System.NonSerialized] public int highlightedIndex = 0;
    [System.NonSerialized] public List<int> selectedIndices = new List<int>();
    [System.NonSerialized] private float holdArrowKeyCooldown = 0f;
    [System.NonSerialized] private float holdArrowKeyCooldownMax = 0.08f;
    [System.NonSerialized] public bool isDoingStuff = false;

    public void WakeMeUp()
    {
        isDoingStuff = true;
        if (uIClassItems.Count > 0)
        {
            uIClassItems[highlightedIndex].highlighted = true;
            highlightedDescription.text = uIClassItems[highlightedIndex].classItem.description;
        }

        if (uIClassItems[highlightedIndex].classItem != null)
        {
            highlightedDescription.text = uIClassItems[highlightedIndex].classItem.GetName() + "\n\n" 
                + uIClassItems[highlightedIndex].classItem.description;
            uIClassItems[highlightedIndex].HighlightMe();
            GameManager.Instance.skillStorage.RebuildUISkillListFor(uIClassItems[highlightedIndex].classItem.id);
        }
    }

    public void Goodbye()
    {
        isDoingStuff = false;
    }

    private void Update()
    {
        if (isDoingStuff && uIClassItems.Count > 0)
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

        if (Input.GetKeyDown(KeyCode.RightArrow) && highlightedIndex < uIClassItems.Count - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.gameObject.transform.parent.GetComponent<UISkills>().SetActivePanel(1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && highlightedIndex > 0)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex--;
        }

        if (Input.GetKey(KeyCode.RightArrow) && holdArrowKeyCooldown == 0 && highlightedIndex < uIClassItems.Count - 1)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKey(KeyCode.LeftArrow) && holdArrowKeyCooldown == 0)
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
                highlightedIndex = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                highlightedIndex = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                highlightedIndex = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                highlightedIndex = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                highlightedIndex = 4;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                highlightedIndex = 5;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                highlightedIndex = 6;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                highlightedIndex = 7;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                highlightedIndex = 8;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                highlightedIndex = 9;
            }
        }

        if (highlightedIndex >= uIClassItems.Count)
        {
            highlightedIndex = uIClassItems.Count - 1;
        }
        if (highlightedIndex < 0)
        {
            highlightedIndex = 0;
        }

        if (Input.GetButtonDown("Jump") && highlightedIndex < uIClassItems.Count)
        {
            bool foundMatch = false;
            if (uISkillSelected.uISlotItems[3] != null)
            {
                for (int i = 0; i < uISkillSelected.uISlotItems[3].Count; i++)
                {
                    if (uIClassItems[highlightedIndex].classItem.id == uISkillSelected.uISlotItems[3][i].GetItemId())
                    {
                        foundMatch = true;
                        uISkillSelected.RemoveElement(3, i);
                        uIClassItems[highlightedIndex].UnselectMe();
                        break;
                    }
                }
            }
            if (!foundMatch && (uISkillSelected.uISlotItems[3] == null || uISkillSelected.uISlotItems[3].Count < NewPlayer.Instance.maxSkillSlots[3]))
            {
                uISkillSelected.AddElement(3, uIClassItems[highlightedIndex]);
                uIClassItems[highlightedIndex].SelectMe();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            this.gameObject.transform.parent.GetComponent<UISkills>().SetActivePanel(2);
            return;
        }
        
        if (uIClassItems[highlightedIndex].classItem != null && highlightedIndex != prevHighlightedIndex)
        {
            highlightedDescription.text = uIClassItems[highlightedIndex].classItem.GetName() + "\n\n" 
                + uIClassItems[highlightedIndex].classItem.description;
            uIClassItems[highlightedIndex].HighlightMe();
            GameManager.Instance.skillStorage.RebuildUISkillListFor(uIClassItems[highlightedIndex].classItem.id);
        }
        else if (highlightedIndex != prevHighlightedIndex)
        {
            highlightedDescription.text = "";
        }
        if (highlightedIndex != prevHighlightedIndex && prevHighlightedIndex < uIClassItems.Count)
        {
            uIClassItems[prevHighlightedIndex].UnhighlightMe();
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

    public bool IsEmpty()
    {
        bool result = (uIClassItems.Count == 0);

        return result;
    }
}