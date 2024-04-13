using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIClass : MonoBehaviour, SubMenu
{
    [System.NonSerialized] public List<UISkillItem>[] uISkillItems = new List<UISkillItem>[3];
    [SerializeField] public UISkillSelected uISkillSelected;
    [SerializeField] public Transform panel;
    [SerializeField] public TextMeshProUGUI highlightedDescription;
    public GameObject skillSlotPrefab;
    [System.NonSerialized] public int highlightedIndex = 0;
    [System.NonSerialized] public int highlightedPanel = 0;
    [System.NonSerialized] public int rowLength = 6;
    [System.NonSerialized] private float holdArrowKeyCooldown = 0f;
    [System.NonSerialized] private float holdArrowKeyCooldownMax = 0.08f;
    [System.NonSerialized] public bool isDoingStuff = false;

    public void WakeMeUp()
    {
        isDoingStuff = true;
        if (uISkillItems[highlightedPanel] == null)
            uISkillItems[highlightedPanel] = new List<UISkillItem>();

        if (uISkillItems[highlightedPanel].Count > 0)
        {
            uISkillItems[highlightedPanel][highlightedIndex].highlighted = true;
            highlightedDescription.text = uISkillItems[highlightedPanel][highlightedIndex].skill.GetDescription();
        }

        if (uISkillItems[highlightedPanel][highlightedIndex].skill != null)
        {
            highlightedDescription.text = uISkillItems[highlightedPanel][highlightedIndex].skill.GetDescription();
            uISkillItems[highlightedPanel][highlightedIndex].HighlightMe();
        }
    }
    
    public void Goodbye()
    {
        isDoingStuff = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (highlightedPanel < 0)
            highlightedPanel = 0;
        if (isDoingStuff && uISkillItems != null && uISkillItems.Length > 0 && uISkillItems[highlightedPanel] != null && uISkillItems[highlightedPanel].Count > 0)
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

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            if (highlightedIndex - rowLength >= 0)
            {
                highlightedIndex -= rowLength;
            }
            else if (highlightedPanel > 0 && uISkillItems[highlightedPanel - 1] != null && uISkillItems[highlightedPanel - 1].Count > 0)
            {
                highlightedPanel--;
            }
            else
            {
                this.gameObject.transform.parent.GetComponent<UISkills>().SetActivePanel(0);
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && highlightedIndex < uISkillItems[highlightedPanel].Count - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.gameObject.transform.parent.GetComponent<UISkills>().SetActivePanel(2);
            return;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            if (highlightedIndex + rowLength < uISkillItems[highlightedPanel].Count)
            {
                highlightedIndex += rowLength;
            }
            else if (highlightedPanel < uISkillItems.Length - 1 && uISkillItems[highlightedPanel + 1] != null && uISkillItems[highlightedPanel + 1].Count > 0)
            {
                highlightedPanel++;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && highlightedIndex > 0)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            if (highlightedIndex < uISkillItems[highlightedPanel].Count)
                highlightedIndex--;
            else
                highlightedIndex = uISkillItems[highlightedPanel].Count - 2;
        }

        if (Input.GetKey(KeyCode.UpArrow) && holdArrowKeyCooldown == 0)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            if (highlightedIndex - rowLength >= 0)
            {
                highlightedIndex -= rowLength;
            }
            else if (highlightedPanel > 0 && uISkillItems[highlightedPanel - 1] != null && uISkillItems[highlightedPanel - 1].Count > 0)
            {
                highlightedPanel--;
            }
            else
            {
                this.gameObject.transform.parent.GetComponent<UISkills>().SetActivePanel(0);
            }
        }
        if (Input.GetKey(KeyCode.RightArrow) && holdArrowKeyCooldown == 0 && highlightedIndex < uISkillItems[highlightedPanel].Count - 1)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKey(KeyCode.DownArrow) && holdArrowKeyCooldown == 0)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            if (highlightedIndex + rowLength < uISkillItems[highlightedPanel].Count)
            {
                highlightedIndex += rowLength;
            }
            else if (highlightedPanel < uISkillItems.Length - 1 && uISkillItems[highlightedPanel + 1] != null && uISkillItems[highlightedPanel - 1].Count > 0)
            {
                highlightedPanel++;
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow) && holdArrowKeyCooldown == 0 && highlightedIndex > 0)
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

        int recogHighlightedIndex = highlightedIndex;
        if (highlightedIndex >= uISkillItems[highlightedPanel].Count)
        {
            recogHighlightedIndex = uISkillItems[highlightedPanel].Count - 1;
        }

        if (Input.GetButtonDown("Jump"))
        {
            bool foundMatch = false;
            if (uISkillSelected.uISlotItems[highlightedPanel] != null)
            {
                for (int i = 0; i < uISkillSelected.uISlotItems[3].Count; i++)
                {
                    if (uISkillItems[highlightedPanel][highlightedIndex].skill.GetId() == uISkillSelected.uISlotItems[highlightedPanel][i].GetItemId())
                    {
                        foundMatch = true;
                        uISkillSelected.RemoveElement(highlightedPanel, i);
                        uISkillItems[highlightedPanel][highlightedIndex].UnselectMe();
                        break;
                    }
                }
            }
            if (!foundMatch && (uISkillSelected.uISlotItems[highlightedPanel] == null || uISkillSelected.uISlotItems[highlightedPanel].Count < NewPlayer.Instance.maxSkillSlots[highlightedPanel]))
            {
                uISkillSelected.AddElement(highlightedPanel, uISkillItems[highlightedPanel][highlightedIndex]);
                uISkillItems[highlightedPanel][highlightedIndex].SelectMe();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Do nothing
        }
        
        if (uISkillItems[highlightedPanel][recogHighlightedIndex].skill != null && highlightedIndex != prevHighlightedIndex)
        {
            highlightedDescription.text = uISkillItems[highlightedPanel][recogHighlightedIndex].skill.GetDescription();
            uISkillItems[highlightedPanel][recogHighlightedIndex].HighlightMe();
        }
        else if (highlightedIndex != prevHighlightedIndex)
        {
            highlightedDescription.text = "";
        }
        if (highlightedIndex != prevHighlightedIndex && prevHighlightedIndex < uISkillItems[highlightedPanel].Count)
        {
            uISkillItems[highlightedPanel][prevHighlightedIndex].UnhighlightMe();
        }
    }

    /*
    public void AddUISkill(int panelIndex, UISkillItem uISkillItem)
    {
        if (uISkillItems[panelIndex] == null)
        {
            uISkillItems[panelIndex] = new List<UISkillItem>();
        }
        if (uISkillItems[panelIndex].Count % rowLength == 0)
        {
            uISkillSelectedDenominator += 2;
            UpdateYAnchors();
        }

        int tmpNumerator = 0;

        for (int i = 0; i < panelIndex; i++)
        {
            if (uISkillItems[i] == null)
                uISkillItems[i] = new List<UISkillItem>();
            tmpNumerator += (uISkillItems[i].Count / rowLength);
        }
        for (int i = 0; i <= uISkillItems[panelIndex].Count; i += rowLength)
        {
            tmpNumerator++;
        }

        GameObject instance = Instantiate(skillSlotPrefab);
        instance.transform.SetParent(panel);
        instance.GetComponentInChildren<UISkillItem>().UpdateSkillItem(uISkillItem.skill, 1 + panelIndex * 3 + tmpNumerator * 2);
        uISkillItems[panelIndex].Add(instance.GetComponentInChildren<UISkillItem>());

        RectTransform rectTransform = uISkillItems[panelIndex][uISkillItems[panelIndex].Count - 1].transform.parent.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(((uISkillItems[panelIndex].Count - 1) % rowLength)/(rowLength*1f), (1 - uISkillItems[panelIndex][uISkillItems[panelIndex].Count - 1].numerator/uISkillSelectedDenominator));
        rectTransform.anchorMax = new Vector2(((uISkillItems[panelIndex].Count - 1) % rowLength + 1)/(rowLength*1f), (1 - uISkillItems[panelIndex][uISkillItems[panelIndex].Count - 1].numerator/uISkillSelectedDenominator));
        rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
    }

    public void UpdateYAnchors()
    {
        foreach (Transform slotTranform in panel)
        {
            UISkillItem slotUISkillItem = slotTranform.GetComponentInChildren<UISkillItem>();
            RectTransform slotRectTransform = slotTranform.GetComponent<RectTransform>();
            slotRectTransform.anchorMin = new Vector2(slotRectTransform.anchorMin.x, (1 - slotUISkillItem.numerator/uISkillSelectedDenominator));
            slotRectTransform.anchorMax = new Vector2(slotRectTransform.anchorMax.x, (1 - slotUISkillItem.numerator/uISkillSelectedDenominator));
            slotRectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
        }
    }
     */

    public void RebuildList(List<List<Skill>> skillItemCategories)
    {
        for (int i = 0; i < uISkillItems.Length; i++)
        {
            if (uISkillItems[i] == null)
                uISkillItems[i] = new List<UISkillItem>();
            for (int j = uISkillItems[i].Count - 1; j >= 0; j--)
            {
                Destroy(uISkillItems[i][j].gameObject);
                uISkillItems[i].RemoveAt(j);
            }
            uISkillItems[i].Clear();
        }
        float uISkillClassDenominator = 1f;

        for (int i = 0; i < skillItemCategories.Count; i++)
        {
            int rowIndex = 0;
            uISkillClassDenominator += 3f;
            for (int j = 0; j < skillItemCategories[i].Count; j++)
            {
                GameObject instance = Instantiate(skillSlotPrefab);
                instance.transform.SetParent(panel);
                instance.GetComponentInChildren<UISkillItem>().UpdateSkillItem(skillItemCategories[i][j], uISkillClassDenominator);
                uISkillItems[i].Add(instance.GetComponentInChildren<UISkillItem>());
                if ((j + 1) % rowLength == 0)
                {
                    uISkillClassDenominator += 2;
                }
            }
        }
        uISkillClassDenominator++;

        for (int i = 0; i < uISkillItems.Length; i++)
        {
            for (int j = 0; j < uISkillItems[i].Count; j++)
            {
                RectTransform rectTransform = uISkillItems[i][j].transform.parent.GetComponent<RectTransform>();

                rectTransform.anchorMin = new Vector2(((j * 1f) % rowLength) / rowLength, (uISkillItems[i][j].numerator/uISkillClassDenominator));
                rectTransform.anchorMax = new Vector2(((j * 1f) % rowLength + 1) / rowLength, (uISkillItems[i][j].numerator/uISkillClassDenominator));
                rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
            }
        }
    }

    public bool IsEmpty()
    {
        bool result = true;
        bool changeActivePanel = false;

        for (int i = uISkillItems.Length - 1; i >= 0; i--)
        {
            if (uISkillItems[i] == null || uISkillItems[i].Count == 0)
            {
                if (i == highlightedPanel)
                {
                    changeActivePanel = true;
                }
            }
            else
            {
                result = false;

                if (changeActivePanel)
                {
                    highlightedPanel = i;
                    changeActivePanel = false;
                }
            }
        }

        return result;
    }
}