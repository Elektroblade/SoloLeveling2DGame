using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIAttributes : MonoBehaviour
{
    [SerializeField] public UIStatus uIStatus;
    [SerializeField] List<AttributeMenuButton> attributeModifyButtons = new List<AttributeMenuButton>();
    [SerializeField] List<StatusMenuButton> otherButtons = new List<StatusMenuButton>();
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject skillsAndEquipmentPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private List<TextMeshProUGUI> attributeDisplays = new List<TextMeshProUGUI>();
    [System.NonSerialized] private int[] tempAttributes = new int[5];
    [System.NonSerialized] private int tempSpentAP = 0;
    [System.NonSerialized] private int highlightedIndex = 0;
    [System.NonSerialized] private bool isDoingStuff = false;
    [System.NonSerialized] private bool waitAFrame = false;

    public void WakeMeUp()
    {
        isDoingStuff = true;
        waitAFrame = true;

        for (int i = 0; i < tempAttributes.Length; i++)
        {
            tempAttributes[i] = 0;
        }

        tempSpentAP = 0;

        for (int i = 0; i < attributeModifyButtons.Count; i++)
        {
            if ((i % 2) == 1)
            {
                attributeModifyButtons[i].ImpossibleMe();
            }
        }

        otherButtons[0].gameObject.SetActive(false);

        statsPanel.SetActive(true);
        statusPanel.SetActive(true);
        skillsAndEquipmentPanel.SetActive(false);

        if (attributeModifyButtons != null)
        {
            attributeModifyButtons[highlightedIndex].HighlightMe();
        }
    }

    public void Goodbye()
    {
        attributeModifyButtons[prevHighlightedIndex].UnhighlightMe();
        HideTempBonuses();
        isDoingStuff = false;

        statsPanel.SetActive(false);
        statusPanel.SetActive(false);
        skillsAndEquipmentPanel.SetActive(false);
    }

    private void Update()
    {
        if (isDoingStuff && !waitAFrame)
        {
            DoUpdate();
        }

        if (waitAFrame)
            waitAFrame = false;
    }

    private void DoUpdate()
    {
        int prevHighlightedIndex = highlightedIndex;

        if (Input.GetKeyDown(KeyCode.UpArrow) && (highlightedIndex > 0 && highlightedIndex != 6))
            highlightedIndex--;
            if (highlightedIndex > 0 && highlightedIndex < 10 && !attributeModifyButtons[highlightedIndex].IsPossible())
                highlightedIndex--;
        
        if (Input.GetKeyDown(KeyCode.RightArrow) && highlightedIndex < 5)
        {
            highlightedIndex += 6;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex != 5 && highlightedIndex < 10)
        {
            highlightedIndex++;
            if (highlightedIndex == 5 && !attributeModifyButtons[highlightedIndex].IsPossible())
                highlightedIndex--;
            else if (highlightedIndex < 10 && !attributeModifyButtons[highlightedIndex].IsPossible())
                highlightedIndex++;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && highlightedIndex > 5)
        {
            highlightedIndex -= 6;
        }

        if (Input.GetButtonDown("Jump"))
        {
            NewPlayer player = NewPlayer.Instance;
            if (prevHighlightedIndex < 10 && prevHighlightedIndex % 2 == 0 && player.attributePoints > tempSpentAP)
            {
                tempSpentAP++;
                tempAttributes[prevHighlightedIndex / 2]++;
                MakeDecreaseAvailableAt(prevHighlightedIndex + 1);
            }
            else if (prevHighlightedIndex < 10 && prevHighlightedIndex % 2 == 1)
            {
                tempSpentAP--;
                if (tempAttributes[prevHighlightedIndex / 2] - 1 == 0)
                {
                    MakeDecreaseUnavailableAt(prevHighlightedIndex);
                    prevHighlightedIndex--;
                }

                tempAttributes[prevHighlightedIndex / 2]--;
            }
            else if (prevHighlightedIndex == 10)
            {
                CommitChanges();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            attributeModifyButtons[prevHighlightedIndex].UnhighlightMe();
            HideTempBonuses();
            isDoingStuff = false;
            uIStatus.WakeMeUp();
        }

        // Make sure index is available
        if (highlightedIndex == 10 && !otherButtons[highlightedIndex - 10].gameObject.activeSelf)
        {
            highlightedIndex = prevHighlightedIndex;
        }

        if (prevHighlightedIndex != highlightedIndex && isDoingStuff)
        {
            if (highlightedIndex == 10)
                DisplayTempOutcomes();

            if (prevHighlightedIndex == 10 && otherButtons[prevHighlightedIndex - 10].gameObject.activeSelf)
                DisplayTempBonuses();

            if (prevHighlightedIndex < 10)
                attributeModifyButtons[prevHighlightedIndex].UnhighlightMe();
            else
                otherButtons[prevHighlightedIndex - 10].UnhighlightMe();

            if (highlightedIndex < 10)
                attributeModifyButtons[highlightedIndex].HighlightMe();
            else
                otherButtons[highlightedIndex - 10].HighlightMe();
        }
    }

    private void MakeDecreaseAvailableAt(int index)
    {
        DisplayTempBonuses();
        attributeModifyButtons[index].PossibleMe();
        otherButtons[0].gameObject.SetActive(true);
    }

    private void MakeDecreaseUnavailableAt(int index)
    {
        attributeModifyButtons[index].ImpossibleMe();

        if (tempSpentAP == 0)
        {
            HideTempBonuses();
            otherButtons[0].gameObject.SetActive(false);
        }
    }

    private void CommitChanges()
    {
        NewPlayer player = NewPlayer.Instance;

        if (player.attributePoints >= tempSpentAP)
        {
            player.attributePoints -= tempSpentAP;
            player.attributes[0] += tempAttributes[0];  // Strength
            player.attributes[1] += tempAttributes[3];  // Stamina
            player.attributes[2] += tempAttributes[1];  // Agility
            player.attributes[3] += tempAttributes[4];  // Intelligence
            player.attributes[4] += tempAttributes[2];  // Perception

            tempSpentAP = 0;

            for (int i = 0; i < attributeModifyButtons.Count; i++)
            {
                if (i % 2 == 1)
                {
                    attributeModifyButtons[i].ImpossibleMe();
                }
            }

            for (int i = 0; i < 5; i++)
            {
                tempAttributes[i] = 0;
            }

            highlightedIndex = 8;

            otherButtons[0].gameObject.SetActive(false);
            HideTempBonuses();

            player.RecalculateIntrinsicStats();
            player.RecalculateExternalStats();
        }
    }

    private void DisplayTempBonuses()
    {
        NewPlayer player = NewPlayer.Instance;

        for (int i = 0; i < 5; i++)
        {
            attributeDisplays[i].color = new Color(0f, 63/256f, 128f, 0.8f);
        }

        attributeDisplays[0].text = "STRENGTH: " + player.attributes[0] + " (+" + tempAttributes[0] + ")";
        attributeDisplays[1].text = "AGILITY: " + player.attributes[2] + " (+" + tempAttributes[1] + ")";
        attributeDisplays[2].text = "PERCEPTION: " + player.attributes[4] + " (+" + tempAttributes[2] + ")";
        attributeDisplays[3].text = "STAMINA: " + player.attributes[1] + " (+" + tempAttributes[3] + ")";
        attributeDisplays[4].text = "INTELLIGENCE: " + player.attributes[3] + " (+" + tempAttributes[4] + ")";
    }

    private void HideTempBonuses()
    {
        NewPlayer player = NewPlayer.Instance;

        for (int i = 0; i < 5; i++)
        {
            attributeDisplays[i].color = new Color(1f, 1f, 1f, 1f);
        }

        attributeDisplays[0].text = "STRENGTH: " + player.attributes[0];
        attributeDisplays[1].text = "AGILITY: " + player.attributes[2];
        attributeDisplays[2].text = "PERCEPTION: " + player.attributes[4];
        attributeDisplays[3].text = "STAMINA: " + player.attributes[1];
        attributeDisplays[4].text = "INTELLIGENCE: " + player.attributes[3];
    }

    public void DisplayTempOutcomes()
    {
        NewPlayer player = NewPlayer.Instance;

        for (int i = 0; i < 5; i++)
        {
            attributeDisplays[i].color = new Color(1f, 0f, 0f, 0.8f);
        }

        attributeDisplays[0].text = "STRENGTH: " + (player.attributes[0] + tempAttributes[0]);
        attributeDisplays[1].text = "AGILITY: " + (player.attributes[2] + tempAttributes[1]);
        attributeDisplays[2].text = "PERCEPTION: " + (player.attributes[4] + tempAttributes[2]);
        attributeDisplays[3].text = "STAMINA: " + (player.attributes[1] + tempAttributes[3]);
        attributeDisplays[4].text = "INTELLIGENCE: " + (player.attributes[3] + tempAttributes[4]);
    }
}
