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
        holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
        isDoingStuff = true;
        if (uISkillItems[highlightedPanel] == null)
            uISkillItems[highlightedPanel] = new List<UISkillItem>();

        if (highlightedIndex < uISkillItems[highlightedPanel].Count)
        {
            uISkillItems[highlightedPanel][highlightedIndex].HighlightMe();
            highlightedDescription.text = uISkillItems[highlightedPanel][highlightedIndex].skill.ToString() + "\n\n" 
                + uISkillItems[highlightedPanel][highlightedIndex].skill.GetDescription();
        }
    }
    
    public void Goodbye()
    {
        if (highlightedIndex < uISkillItems[highlightedPanel].Count)
            uISkillItems[highlightedPanel][highlightedIndex].UnhighlightMe();
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
        int prevHighlightedPanel = highlightedPanel;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            bool wentUp = false;
            if (highlightedIndex - rowLength >= 0)
            {
                highlightedIndex -= rowLength;
                wentUp = true;
            }
            else if (highlightedPanel > 0)
            {
                for (int i = highlightedPanel - 1; i >= 0; i--)
                {
                    if (uISkillItems[i] != null && uISkillItems[i].Count > 0)
                    {
                        highlightedPanel = i;
                        wentUp = true;
                        break;
                    }
                }
            }
            if (!wentUp)
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
            else if (highlightedPanel < uISkillItems.Length - 1)
            {
                for (int i = highlightedPanel + 1; i < uISkillItems.Length; i++)
                {
                    if (uISkillItems[i] != null && uISkillItems[i].Count > 0)
                    {
                        highlightedPanel = i;
                        break;
                    }
                }
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
            bool wentUp = false;
            if (highlightedIndex - rowLength >= 0)
            {
                highlightedIndex -= rowLength;
                wentUp = true;
            }
            else if (highlightedPanel > 0)
            {
                for (int i = highlightedPanel - 1; i >= 0; i--)
                {
                    if (uISkillItems[i] != null && uISkillItems[i].Count > 0)
                    {
                        highlightedPanel = i;
                        wentUp = true;
                        break;
                    }
                }
            }
            if (!wentUp)
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
            else if (highlightedPanel < uISkillItems.Length - 1)
            {
                for (int i = highlightedPanel + 1; i < uISkillItems.Length; i++)
                {
                    if (uISkillItems[i] != null && uISkillItems[i].Count > 0)
                    {
                        highlightedPanel = i;
                        break;
                    }
                }
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
                for (int i = 0; i < uISkillSelected.uISlotItems[highlightedPanel].Count; i++)
                {
                    if (uISkillItems[highlightedPanel][recogHighlightedIndex].skill.GetId() == uISkillSelected.uISlotItems[highlightedPanel][i].GetItemId())
                    {
                        foundMatch = true;
                        uISkillSelected.RemoveElement(highlightedPanel, i);
                        uISkillItems[highlightedPanel][recogHighlightedIndex].UnselectMe();
                        break;
                    }
                }
            }
            if (!foundMatch && (uISkillSelected.uISlotItems[highlightedPanel] == null 
            || uISkillSelected.uISlotItems[highlightedPanel].Count < NewPlayer.Instance.maxSkillSlots[highlightedPanel] || highlightedPanel == 2))
            {
                uISkillSelected.AddElement(highlightedPanel, uISkillItems[highlightedPanel][recogHighlightedIndex]);
                uISkillItems[highlightedPanel][recogHighlightedIndex].SelectMe();
                Debug.Log("highlightedPanel = " + highlightedPanel + ", recogHighlightedIndex = " + recogHighlightedIndex);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Do nothing
        }
        
        if (uISkillItems[highlightedPanel][recogHighlightedIndex].skill != null && (highlightedIndex != prevHighlightedIndex || highlightedPanel != prevHighlightedPanel))
        {
            highlightedDescription.text = uISkillItems[highlightedPanel][recogHighlightedIndex].skill.ToString() + "\n\n" 
                + uISkillItems[highlightedPanel][recogHighlightedIndex].skill.GetDescription();
            uISkillItems[highlightedPanel][recogHighlightedIndex].HighlightMe();
        }
        else if (highlightedIndex != prevHighlightedIndex || highlightedPanel != prevHighlightedPanel)
        {
            highlightedDescription.text = "";
        }
        if ((highlightedIndex != prevHighlightedIndex || highlightedPanel != prevHighlightedPanel) && uISkillItems[prevHighlightedPanel] != null 
            && prevHighlightedIndex < uISkillItems[prevHighlightedPanel].Count)
        {
            uISkillItems[prevHighlightedPanel][prevHighlightedIndex].UnhighlightMe();
        }
        else if ((highlightedIndex != prevHighlightedIndex || highlightedPanel != prevHighlightedPanel) && uISkillItems[prevHighlightedPanel] != null)
        {
            uISkillItems[prevHighlightedPanel][uISkillItems[prevHighlightedPanel].Count - 1].UnhighlightMe();
        }

        //Debug.Log("[" + highlightedPanel + "], [" + highlightedIndex + "]");
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
            int j;
            for (j = 0; j < skillItemCategories[i].Count; j++)
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
            if (j > 0)
            {
                uISkillClassDenominator += 2;
            }
        }
        uISkillClassDenominator++;
        bool setHighlightedPanel = false;

        for (int i = 0; i < uISkillItems.Length; i++)
        {
            for (int j = 0; j < uISkillItems[i].Count; j++)
            {
                RectTransform rectTransform = uISkillItems[i][j].transform.parent.GetComponent<RectTransform>();

                rectTransform.anchorMin = new Vector2(((j * 1f) % rowLength) / rowLength, 1 - (uISkillItems[i][j].numerator/uISkillClassDenominator));
                rectTransform.anchorMax = new Vector2(((j * 1f) % rowLength + 1) / rowLength, 1 - (uISkillItems[i][j].numerator/uISkillClassDenominator));
                rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
            }

            // highlight a nonempty panel
            if (!setHighlightedPanel && uISkillItems[i].Count > 0)
            {
                highlightedPanel = i;
                setHighlightedPanel = true;
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