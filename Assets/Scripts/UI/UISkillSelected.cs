using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkillSelected : MonoBehaviour, SubMenu
{
    [System.NonSerialized] public List<UIItem>[] uISlotItems = new List<UIItem>[4];
    //[System.NonSerialized] public List<UISkillItem> uIActiveItems = new List<UISkillItem>();
    //[System.NonSerialized] public List<UIClassItem> uIActiveItems = new List<UISlottableItem>();
    //[System.NonSerialized] public List<UIClassItem> uIActiveItems = new List<UIAutoItem>();
    [SerializeField] public Transform selectedPanel;
    [SerializeField] public TextMeshProUGUI highlightedDescription;
    
    [System.NonSerialized] public float uISkillSelectedDenominator = 14f;
    public GameObject classSlotPrefab;
    public GameObject skillSlotPrefab;
    [System.NonSerialized] public int highlightedIndex = 0;
    [System.NonSerialized] public int highlightedPanel = 3;
    [System.NonSerialized] public int rowLength = 4;
    [System.NonSerialized] private float holdArrowKeyCooldown = 0f;
    [System.NonSerialized] private float holdArrowKeyCooldownMax = 0.08f;
    [System.NonSerialized] public bool isDoingStuff = false;

    public void WakeMeUp()
    {
        holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
        isDoingStuff = true;
        if (uISlotItems[highlightedPanel] != null && highlightedIndex < uISlotItems[highlightedPanel].Count)
        {
            uISlotItems[highlightedPanel][highlightedIndex].HighlightMe();
            highlightedDescription.text = uISlotItems[highlightedPanel][highlightedIndex].GetName() + "\n\n" 
                + uISlotItems[highlightedPanel][highlightedIndex].GetDescription();
        }
    }
    
    public void Goodbye()
    {
        if (uISlotItems[highlightedPanel] != null && highlightedIndex < uISlotItems[highlightedPanel].Count)
            uISlotItems[highlightedPanel][highlightedIndex].UnhighlightMe();
        isDoingStuff = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (highlightedPanel < 0)
            highlightedPanel = 0;
        if (isDoingStuff && uISlotItems != null && uISlotItems.Length > 0 && uISlotItems[highlightedPanel] != null && uISlotItems[highlightedPanel].Count > 0)
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
                    if (uISlotItems[i] != null && uISlotItems[i].Count > 0)
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
        if (Input.GetKeyDown(KeyCode.RightArrow) && highlightedIndex < uISlotItems[highlightedPanel].Count - 1)
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            holdArrowKeyCooldown = 4f*holdArrowKeyCooldownMax;
            if (highlightedIndex + rowLength < uISlotItems[highlightedPanel].Count)
            {
                highlightedIndex += rowLength;
            }
            else if (highlightedPanel < uISlotItems.Length - 1)
            {
                for (int i = highlightedPanel + 1; i < uISlotItems.Length; i++)
                {
                    if (uISlotItems[i] != null && uISlotItems[i].Count > 0)
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
            if (highlightedIndex < uISlotItems[highlightedPanel].Count)
                highlightedIndex--;
            else
                highlightedIndex = uISlotItems[highlightedPanel].Count - 2;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.gameObject.transform.parent.GetComponent<UISkills>().SetActivePanel(1);
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
                    if (uISlotItems[i] != null && uISlotItems[i].Count > 0)
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
        if (Input.GetKey(KeyCode.RightArrow) && holdArrowKeyCooldown == 0 && highlightedIndex < uISlotItems[highlightedPanel].Count - 1)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            highlightedIndex++;
        }
        if (Input.GetKey(KeyCode.DownArrow) && holdArrowKeyCooldown == 0)
        {
            holdArrowKeyCooldown = holdArrowKeyCooldownMax;
            if (highlightedIndex + rowLength < uISlotItems[highlightedPanel].Count)
            {
                highlightedIndex += rowLength;
            }
            else if (highlightedPanel < uISlotItems.Length - 1)
            {
                for (int i = highlightedPanel + 1; i < uISlotItems.Length; i++)
                {
                    if (uISlotItems[i] != null && uISlotItems[i].Count > 0)
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
        if (highlightedIndex >= uISlotItems[highlightedPanel].Count)
        {
            recogHighlightedIndex = uISlotItems[highlightedPanel].Count - 1;
        }

        if (Input.GetButtonDown("Jump"))
        {
            RemoveElement(highlightedPanel, recogHighlightedIndex);
            if (uISlotItems[highlightedPanel] == null)
            {
                uISlotItems[highlightedPanel] = new List<UIItem>();
            }
            if (uISlotItems[highlightedPanel].Count == 0)
            {
                if (IsEmpty())
                {
                    Goodbye();
                    this.gameObject.transform.parent.GetComponent<SubMenu>().IsEmpty();
                    return;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Do nothing
        }
        
        if (!uISlotItems[highlightedPanel][recogHighlightedIndex].ItemIsNull() && (highlightedIndex != prevHighlightedIndex || highlightedPanel != prevHighlightedPanel))
        {
            highlightedDescription.text = uISlotItems[highlightedPanel][recogHighlightedIndex].GetName() + "\n\n" 
                + uISlotItems[highlightedPanel][recogHighlightedIndex].GetDescription();
            uISlotItems[highlightedPanel][recogHighlightedIndex].HighlightMe();
        }
        else if (highlightedIndex != prevHighlightedIndex || highlightedPanel != prevHighlightedPanel)
        {
            highlightedDescription.text = "";
        }
        if ((highlightedIndex != prevHighlightedIndex || highlightedPanel != prevHighlightedPanel) && prevHighlightedIndex < uISlotItems[highlightedPanel].Count)
        {
            uISlotItems[prevHighlightedPanel][prevHighlightedIndex].UnhighlightMe();
        }
    }

    public void AddElement(int panelIndex, UIItem uIItem)
    {
        if (uISlotItems[panelIndex] == null)
        {
            uISlotItems[panelIndex] = new List<UIItem>();
        }
        if (uISlotItems[panelIndex].Count % rowLength == 0)
        {
            uISkillSelectedDenominator += 2;
            for (int i = panelIndex + 1; i < uISlotItems.Length; i++)
            {
                if (uISlotItems[i] == null)
                    uISlotItems[i] = new List<UIItem>();
                for (int j = 0; j < uISlotItems[i].Count; j++)
                {
                    uISlotItems[i][j].IncrementNumerator();
                }
            }
            UpdateYAnchors();
        }

        int tmpNumerator = 0;

        for (int i = 0; i < panelIndex; i++)
        {
            if (uISlotItems[i] == null)
                uISlotItems[i] = new List<UIItem>();
            //Debug.Log("((uISlotItems[i].Count + rowLength - 1) / rowLength) = " + ((uISlotItems[i].Count + rowLength - 1) / rowLength));
            tmpNumerator += ((uISlotItems[i].Count + rowLength - 1) / rowLength);
        }
        for (int i = rowLength; i <= uISlotItems[panelIndex].Count; i += rowLength)
        {
            tmpNumerator++;
        }

        //Debug.Log("4 + panelIndex * 3 + tmpNumerator * 2 = " + (5 + panelIndex * 3 + tmpNumerator * 2));
        //Debug.Log("uISkillSelectedDenominator = " + uISkillSelectedDenominator);

        GameObject instance;
        if (panelIndex == 3)
        {
            instance = Instantiate(classSlotPrefab);
            instance.transform.SetParent(selectedPanel);
            instance.GetComponentInChildren<UIClassItem>().UpdateClassItem((uIItem as UIClassItem).classItem, 5 + panelIndex * 3 + tmpNumerator * 2);
            
        }
        else
        {
            instance = Instantiate(skillSlotPrefab);
            instance.transform.SetParent(selectedPanel);
            instance.GetComponentInChildren<UISkillItem>().UpdateSkillItem((uIItem as UISkillItem).skill, 5 + panelIndex * 3 + tmpNumerator * 2);
        }
        uISlotItems[panelIndex].Add(instance.GetComponentInChildren<UIItem>());
        NewPlayer.Instance.AddSkillSlot(instance.GetComponentInChildren<UIItem>(), panelIndex);

        RectTransform rectTransform = uISlotItems[panelIndex][uISlotItems[panelIndex].Count - 1].GetTransform().parent.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(((uISlotItems[panelIndex].Count - 1) % rowLength)/(rowLength*1f), (1 - uISlotItems[panelIndex][uISlotItems[panelIndex].Count - 1].GetNumerator()/uISkillSelectedDenominator));
        rectTransform.anchorMax = new Vector2(((uISlotItems[panelIndex].Count - 1) % rowLength + 1)/(rowLength*1f), (1 - uISlotItems[panelIndex][uISlotItems[panelIndex].Count - 1].GetNumerator()/uISkillSelectedDenominator));
        rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
    }

    public void RemoveElement(int panelIndex, int slotIndex)
    {
        UIItem elementToRemove = uISlotItems[panelIndex][slotIndex];
        RectTransform removedRectTransform = elementToRemove.GetTransform().parent.GetComponent<RectTransform>();
        Vector2 removedAnchorMin = removedRectTransform.anchorMin;
        Vector2 removedAnchorMax = removedRectTransform.anchorMax;
        float removedNumerator = elementToRemove.GetNumerator();

        Destroy(elementToRemove.GetTransform().parent.gameObject);
        uISlotItems[panelIndex].RemoveAt(slotIndex);

        NewPlayer.Instance.RemoveSkillSlotAt(slotIndex, panelIndex, uISlotItems[1].Count);

        // Do not need to slide slots in this panel down
        if (slotIndex == uISlotItems[panelIndex].Count)
        {
            if (uISlotItems[panelIndex].Count % rowLength == 0)
            {
                uISkillSelectedDenominator -= 2;
                for (int i = panelIndex + 1; i < uISlotItems.Length; i++)
                {
                    if (uISlotItems[i] == null)
                        uISlotItems[i] = new List<UIItem>();
                    for (int j = 0; j < uISlotItems[i].Count; j++)
                    {
                        uISlotItems[i][j].DecrementNumerator();
                    }
                }
                UpdateYAnchors();
            }
        }
        else
        {
            if (uISlotItems[panelIndex].Count % rowLength == 0)
            {
                uISkillSelectedDenominator -= 2;
                UpdateYAnchors();
            }
            for (int j = slotIndex; j < uISlotItems[panelIndex].Count; j++)
            {
                RectTransform slotRectTransform = uISlotItems[panelIndex][j].GetTransform().parent.GetComponent<RectTransform>();
                if (uISlotItems[panelIndex][j].GetNumerator() > removedNumerator || (uISlotItems[panelIndex][j].GetNumerator() >= removedNumerator && slotRectTransform.anchorMin.x > removedAnchorMin.x))
                {
                    if (slotRectTransform.anchorMin.x < (0.5f / (rowLength * 1f)))
                    {
                        uISlotItems[panelIndex][j].DecrementNumerator();
                        slotRectTransform.anchorMin = new Vector2((rowLength - 1f) / (rowLength * 1f), (1 - uISlotItems[panelIndex][j].GetNumerator()/uISkillSelectedDenominator));
                        slotRectTransform.anchorMax = new Vector2(1f, (1 - uISlotItems[panelIndex][j].GetNumerator()/uISkillSelectedDenominator));
                        slotRectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
                    }
                    else
                    {
                        float tmpSlotAnchorMinX = slotRectTransform.anchorMin.x;
                        slotRectTransform.anchorMin = new Vector2(slotRectTransform.anchorMin.x - (1f / (rowLength * 1f)), (1 - uISlotItems[panelIndex][j].GetNumerator()/uISkillSelectedDenominator));
                        slotRectTransform.anchorMax = new Vector2(tmpSlotAnchorMinX, (1 - uISlotItems[panelIndex][j].GetNumerator()/uISkillSelectedDenominator));
                        slotRectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
                    }
                }
            }
        }   
    }

    public void UpdateYAnchors()
    {
        foreach (Transform slotTranform in selectedPanel)
        {
            UIItem slotUIItem = slotTranform.GetComponentInChildren<UIItem>();
            RectTransform slotRectTransform = slotTranform.GetComponent<RectTransform>();
            slotRectTransform.anchorMin = new Vector2(slotRectTransform.anchorMin.x, (1 - slotUIItem.GetNumerator()/uISkillSelectedDenominator));
            slotRectTransform.anchorMax = new Vector2(slotRectTransform.anchorMax.x, (1 - slotUIItem.GetNumerator()/uISkillSelectedDenominator));
            slotRectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
        }
    }

    public bool IsEmpty()
    {
        bool result = true;
        bool changeActivePanel = false;

        for (int i = uISlotItems.Length - 1; i >= 0; i--)
        {
            if (uISlotItems[i] == null || uISlotItems[i].Count == 0)
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
