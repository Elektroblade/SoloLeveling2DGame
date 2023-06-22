using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    [SerializeField] private List<TextMeshProUGUI> statDisplays = new List<TextMeshProUGUI>();
    [SerializeField] private Canvas mSCDropdownCanvas;
    [SerializeField] private List<StatusMenuButton> mSCDropdownButtons = new List<StatusMenuButton>();
    [SerializeField] private List<TextMeshProUGUI> capDropdownDisplays = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> inputTextDisplays = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> statCapDisplays = new List<TextMeshProUGUI>();
    [System.NonSerialized] private int[] tempAttributes = new int[5];
    [System.NonSerialized] private int tempSpentAP = 0;
    [System.NonSerialized] private int highlightedIndex = 0;
    [System.NonSerialized] private int highlightedDropdownIndex = 0;
    [System.NonSerialized] private float holdArrowKeyCooldown = 0f;
    [System.NonSerialized] private float holdArrowKeyCooldownMax = 0.08f;
    [System.NonSerialized] private float holdArrowKeyCooldownOrig = 0.08f;
    [System.NonSerialized] private bool isDoingStuff = false;
    [System.NonSerialized] private bool waitAFrame = false;
    [System.NonSerialized] private bool isInputingText = false;
    [System.NonSerialized] private string currentInputText = "";
    [System.NonSerialized] private int currentPeriodCount = 0;

    private void start()
    {
        
    }

    public void WakeMeUp()
    {
        mSCDropdownCanvas.enabled = false;
        isDoingStuff = true;
        waitAFrame = true;
        holdArrowKeyCooldown = 10f * holdArrowKeyCooldownMax;

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

        DisplayTempStatBonuses();

        for (int i = 1; i < 22; i += 2)
        {
            statDisplays[i].color = new Color(46/256f, 186/256f, 239/256f, 0.8f);
        }

        otherButtons[0].gameObject.SetActive(false);

        statsPanel.SetActive(true);
        statusPanel.SetActive(true);
        skillsAndEquipmentPanel.SetActive(false);

        if (highlightedIndex < 10 && attributeModifyButtons != null)
        {
            attributeModifyButtons[highlightedIndex].HighlightMe();
        }

        NewPlayer player = NewPlayer.Instance;
        if (player.capValues[0,highlightedDropdownIndex] < 0.0)
            player.capValues[0,highlightedDropdownIndex] = NewPlayer.Instance.intrinsicStats[3];
        statCapDisplays[0].text = "MOVEMENT SPEED CAP: " + player.capValues[0,highlightedDropdownIndex] + "%";
        inputTextDisplays[0].text = "" + player.capValues[0, player.capPreferences[0]];
    }

    public void Goodbye()
    {
        mSCDropdownCanvas.enabled = false;
        if (highlightedIndex < 10)
            attributeModifyButtons[highlightedIndex].UnhighlightMe();
        HideTempBonuses();
        isDoingStuff = false;
        otherButtons[0].gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDoingStuff && !waitAFrame)
        {
            if (!mSCDropdownCanvas.enabled && !isInputingText)
                DoUpdate();
            else if (mSCDropdownCanvas.enabled && !isInputingText)
                DoMSCDropdown();
            else if (isInputingText)
                DoInputingText();
        }

        if (waitAFrame)
            waitAFrame = false;
    }

    private void DoUpdate()
    {
        if (holdArrowKeyCooldown > 0f)
        {
            holdArrowKeyCooldown -= Time.deltaTime;
        }
        else if (holdArrowKeyCooldown < 0f)
        {
            holdArrowKeyCooldown = 0f;
        }

        int prevHighlightedIndex = highlightedIndex;

        if (Input.GetKeyDown(KeyCode.UpArrow) && (highlightedIndex > 0 && highlightedIndex != 6 && highlightedIndex < 11))
            highlightedIndex--;
            if (highlightedIndex > 0 && highlightedIndex < 10 && !attributeModifyButtons[highlightedIndex].IsPossible())
                highlightedIndex--;
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (highlightedIndex < 5)
                highlightedIndex += 6;
            else if (highlightedIndex < 11 && highlightedIndex > 5)
            {
                highlightedIndex = 11;
            }
            else if (highlightedIndex >= 11 && highlightedIndex < 14)
            {
                highlightedIndex += 3;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex != 5 && highlightedIndex < 10)
        {
            highlightedIndex++;
            if (highlightedIndex == 5 && !attributeModifyButtons[highlightedIndex].IsPossible())
                highlightedIndex--;
            else if (highlightedIndex < 10 && !attributeModifyButtons[highlightedIndex].IsPossible())
                highlightedIndex++;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (highlightedIndex > 5 && highlightedIndex < 11)
                highlightedIndex -= 6;
            else if (highlightedIndex == 11)
            {
                highlightedIndex = 6;
            }
            else if (highlightedIndex >= 14 && highlightedIndex < 17)
            {
                highlightedIndex -= 3;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (prevHighlightedIndex < 10)
                attributeModifyButtons[prevHighlightedIndex].SelectMe();
            holdArrowKeyCooldownMax = holdArrowKeyCooldownOrig;
            NewPlayer player = NewPlayer.Instance;
            if (prevHighlightedIndex < 10 && prevHighlightedIndex % 2 == 0 && player.attributePoints > tempSpentAP)
            {
                holdArrowKeyCooldown = 4f * holdArrowKeyCooldownMax;
                tempSpentAP++;
                tempAttributes[prevHighlightedIndex / 2]++;
                MakeDecreaseAvailableAt(prevHighlightedIndex + 1);
            }
            else if (prevHighlightedIndex < 10 && prevHighlightedIndex % 2 == 1)
            {
                holdArrowKeyCooldown = 4f * holdArrowKeyCooldownMax;
                tempSpentAP--;
                tempAttributes[prevHighlightedIndex / 2]--;
                
                if (tempAttributes[prevHighlightedIndex / 2] == 0)
                {
                    MakeDecreaseUnavailableAt(prevHighlightedIndex);
                    prevHighlightedIndex--;
                }
                else
                {
                    DisplayTempBonuses();
                }
            }
            else if (prevHighlightedIndex == 10)
            {
                holdArrowKeyCooldown = 10f * holdArrowKeyCooldownMax;
                CommitChanges();
            }
            else if (prevHighlightedIndex >= 11 && prevHighlightedIndex < 14)
            {
                Debug.Log("here we are (down)");
                mSCDropdownButtons[prevHighlightedIndex - 11].HighlightMe();
                for (int i = 1; i < mSCDropdownButtons.Count; i++)
                {
                    mSCDropdownButtons[i].UnhighlightMe();
                }

                mSCDropdownCanvas.enabled = true;
                waitAFrame = true;
                highlightedDropdownIndex = 0;
            }
            else if (prevHighlightedIndex >= 14 && prevHighlightedIndex < 17)
            {
                Debug.Log("input index + 14 = " + (prevHighlightedIndex));
                highlightedIndex = prevHighlightedIndex;
                isInputingText = true;
                waitAFrame = true;
            }
        }

        if (Input.GetButton("Jump") && holdArrowKeyCooldown == 0)
        {
            if (prevHighlightedIndex < 10 && !attributeModifyButtons[prevHighlightedIndex].IsSelected())
            {
                for (int i = 0; i < attributeModifyButtons.Count; i++)
                {
                    if (attributeModifyButtons[i].IsPossible())
                        attributeModifyButtons[i].DeselectMe();
                }

                holdArrowKeyCooldownMax = holdArrowKeyCooldownOrig;
                attributeModifyButtons[prevHighlightedIndex].SelectMe();
            }

            NewPlayer player = NewPlayer.Instance;
            holdArrowKeyCooldownMax -= holdArrowKeyCooldownMax / 50f;

            if (prevHighlightedIndex < 10 && prevHighlightedIndex % 2 == 0 && player.attributePoints > tempSpentAP)
            {
                holdArrowKeyCooldown = holdArrowKeyCooldownMax;

                if (holdArrowKeyCooldownMax < holdArrowKeyCooldownOrig * 0.0625f)
                {
                    tempSpentAP += 50;
                    tempAttributes[prevHighlightedIndex / 2] += 50;
                }
                else if (holdArrowKeyCooldownMax < holdArrowKeyCooldownOrig * 0.125f)
                {
                    tempSpentAP += 10;
                    tempAttributes[prevHighlightedIndex / 2] += 10;
                }
                else if (holdArrowKeyCooldownMax < holdArrowKeyCooldownOrig * 0.25f)
                {
                    tempSpentAP += 4;
                    tempAttributes[prevHighlightedIndex / 2] += 4;
                }
                else if (holdArrowKeyCooldownMax < holdArrowKeyCooldownOrig * 0.5f)
                {
                    tempSpentAP += 2;
                    tempAttributes[prevHighlightedIndex / 2] += 2;
                }
                else
                {
                    tempSpentAP++;
                    tempAttributes[prevHighlightedIndex / 2]++;
                }

                while (player.attributePoints < tempSpentAP)
                {
                    tempSpentAP--;
                    tempAttributes[prevHighlightedIndex / 2]--;
                }

                MakeDecreaseAvailableAt(prevHighlightedIndex + 1);
            }
            else if (prevHighlightedIndex < 10 && prevHighlightedIndex % 2 == 1)
            {
                holdArrowKeyCooldown = holdArrowKeyCooldownMax;

                if (holdArrowKeyCooldownMax < holdArrowKeyCooldownOrig * 0.0625f)
                {
                    tempSpentAP -= 50;
                    tempAttributes[prevHighlightedIndex / 2] -= 50;
                }
                else if (holdArrowKeyCooldownMax < holdArrowKeyCooldownOrig * 0.125f)
                {
                    tempSpentAP -= 10;
                    tempAttributes[prevHighlightedIndex / 2] -= 10;
                }
                else if (holdArrowKeyCooldownMax < holdArrowKeyCooldownOrig * 0.25f)
                {
                    tempSpentAP -= 4;
                    tempAttributes[prevHighlightedIndex / 2] -= 4;
                }
                else if (holdArrowKeyCooldownMax < holdArrowKeyCooldownOrig * 0.5f)
                {
                    tempSpentAP -= 2;
                    tempAttributes[prevHighlightedIndex / 2] -= 2;
                }
                else
                {
                    tempSpentAP--;
                    tempAttributes[prevHighlightedIndex / 2]--;
                }

                while (tempAttributes[prevHighlightedIndex / 2] < 0)
                {
                    tempSpentAP++;
                    tempAttributes[prevHighlightedIndex / 2]++;
                }
                
                if (tempAttributes[prevHighlightedIndex / 2] == 0)
                {
                    holdArrowKeyCooldownMax = holdArrowKeyCooldownOrig;
                    holdArrowKeyCooldown = 10f * holdArrowKeyCooldownMax;
                    MakeDecreaseUnavailableAt(prevHighlightedIndex);
                    prevHighlightedIndex--;
                }
                else
                {
                    DisplayTempBonuses();
                }
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            for (int i = 0; i < attributeModifyButtons.Count; i++)
            {
                if (attributeModifyButtons[i].IsPossible())
                    attributeModifyButtons[i].DeselectMe();
            }

            if (highlightedIndex != 10)
                DisplayTempStatBonuses();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (prevHighlightedIndex < 10)
                attributeModifyButtons[prevHighlightedIndex].UnhighlightMe();
            HideTempBonuses();
            isDoingStuff = false;
            otherButtons[0].gameObject.SetActive(false);
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
            {
                DisplayTempOutcomes();
                DisplayTempStatOutcomes();
            }

            if (prevHighlightedIndex == 10 && otherButtons[prevHighlightedIndex - 10].gameObject.activeSelf)
            {
                DisplayTempBonuses();
                DisplayTempStatBonuses();
            }

            if (prevHighlightedIndex < 10)
                attributeModifyButtons[prevHighlightedIndex].UnhighlightMe();
            else if (prevHighlightedIndex < 17)
                otherButtons[prevHighlightedIndex - 10].UnhighlightMe();

            Debug.Log("highlightedIndex = " + highlightedIndex + ", prevHighlightedIndex = " + prevHighlightedIndex);

            if (highlightedIndex < 10)
                attributeModifyButtons[highlightedIndex].HighlightMe();
            else if (highlightedIndex < 17)
                otherButtons[highlightedIndex - 10].HighlightMe();
        }
    }

    private void DoMSCDropdown()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Down");

            if (highlightedDropdownIndex < mSCDropdownButtons.Count - 1)
            {
                mSCDropdownButtons[highlightedDropdownIndex].UnhighlightMe();
                mSCDropdownButtons[highlightedDropdownIndex + 1].HighlightMe();

                highlightedDropdownIndex++;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Up, " + highlightedDropdownIndex);

            if (highlightedDropdownIndex > 0)
            {
                mSCDropdownButtons[highlightedDropdownIndex].UnhighlightMe();
                mSCDropdownButtons[highlightedDropdownIndex - 1].HighlightMe();

                highlightedDropdownIndex--;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            NewPlayer player = NewPlayer.Instance;
            player.capPreferences[0] = highlightedDropdownIndex;
            capDropdownDisplays[0].text = mSCDropdownButtons[highlightedDropdownIndex].transform.GetComponentsInChildren<TextMeshProUGUI>()[0].text;
            if (player.capValues[0,highlightedDropdownIndex] < 0.0)
                player.capValues[0,highlightedDropdownIndex] = NewPlayer.Instance.intrinsicStats[3];
            statCapDisplays[0].text = "MOVEMENT SPEED CAP: " + player.capValues[0,highlightedDropdownIndex] + "%";
            inputTextDisplays[0].text = "" + player.capValues[0, player.capPreferences[0]];
            mSCDropdownCanvas.enabled = false;

            mSCDropdownButtons[0].HighlightMe();
            for (int i = 1; i < mSCDropdownButtons.Count; i++)
            {
                mSCDropdownButtons[i].UnhighlightMe();
            }
        }
            

        if (Input.GetKeyDown(KeyCode.Q))
        {
            mSCDropdownCanvas.enabled = false;

            mSCDropdownButtons[0].HighlightMe();
            for (int i = 1; i < mSCDropdownButtons.Count; i++)
            {
                mSCDropdownButtons[i].UnhighlightMe();
            }
        }
    }

    private void DoInputingText()
    {
        string prevInputText = currentInputText;
        bool weAreDoneHere = false;

        if (Input.GetKeyDown("0"))
            currentInputText += "0";
        else if (Input.GetKeyDown("1"))
            currentInputText += "1";
        else if (Input.GetKeyDown("2"))
            currentInputText += "2";
        else if (Input.GetKeyDown("3"))
            currentInputText += "3";
        else if (Input.GetKeyDown("4"))
            currentInputText += "4";
        else if (Input.GetKeyDown("5"))
            currentInputText += "5";
        else if (Input.GetKeyDown("6"))
            currentInputText += "6";
        else if (Input.GetKeyDown("7"))
            currentInputText += "7";
        else if (Input.GetKeyDown("8"))
            currentInputText += "8";
        else if (Input.GetKeyDown("9"))
            currentInputText += "9";
        else if (Input.GetKeyDown("."))
        {
            if (currentPeriodCount < 1)
            {
                currentPeriodCount++;
                currentInputText += ".";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("confirming and exiting input");
            ReadStringInput(currentInputText);
            currentInputText = "";
            currentPeriodCount = 0;
            isDoingStuff = true;
            isInputingText = false;
            weAreDoneHere = true;
        }
        else if (Input.GetKeyDown("backspace"))
        {
            string tempInputText = "";

            for (int i = 0; i < currentInputText.Length - 1; i++)
            {
                tempInputText += currentInputText[i];
            }

            if (currentInputText[currentInputText.Length - 1] == '.')
                currentPeriodCount--;

            currentInputText = tempInputText;
        }
        else if (Input.GetKeyDown("q"))
        {
            NewPlayer player = NewPlayer.Instance;
            Debug.Log("discarding input");
            currentInputText = "";
            currentPeriodCount = 0;
            isDoingStuff = true;
            isInputingText = false;
            weAreDoneHere = true;

            inputTextDisplays[highlightedIndex-14].text = "" + player.capValues[highlightedIndex-14,player.capPreferences[highlightedIndex-14]];
        }

        if (prevInputText.CompareTo(currentInputText) != 0 && !weAreDoneHere)
        {
            inputTextDisplays[highlightedIndex - 14].text = currentInputText;
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
        else
        {
            DisplayTempBonuses();
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

            statCapDisplays[0].text = "MOVEMENT SPEED CAP: " + player.movementSpeedCap + "%";
        }
    }

    private void DisplayTempBonuses()
    {
        NewPlayer player = NewPlayer.Instance;

        attributeDisplays[0].text = "STRENGTH: " + player.attributes[0];
        attributeDisplays[1].text = "AGILITY: " + player.attributes[2];
        attributeDisplays[2].text = "PERCEPTION: " + player.attributes[4];
        attributeDisplays[3].text = "STAMINA: " + player.attributes[1];
        attributeDisplays[4].text = "INTELLIGENCE: " + player.attributes[3];

        for (int i = 0; i < 5; i++)
        {
            if (tempAttributes[i] != 0)
            {
                attributeDisplays[i].text += " (+" + tempAttributes[i] + ")";
                attributeDisplays[i].color = new Color(46/256f, 186/256f, 239/256f, 0.8f);
            }
            else
            {
                attributeDisplays[i].color = new Color(1f, 1f, 1f, 1f);
            }
        }

        attributeDisplays[5].color = new Color(46/256f, 186/256f, 239/256f, 0.8f);
        attributeDisplays[5].text = "REMAINING POINTS: " + player.attributePoints + " -" + tempSpentAP;
    }

    private void HideTempBonuses()
    {
        NewPlayer player = NewPlayer.Instance;

        for (int i = 0; i < 6; i++)
        {
            attributeDisplays[i].color = new Color(1f, 1f, 1f, 1f);
        }

        attributeDisplays[0].text = "STRENGTH: " + player.attributes[0];
        attributeDisplays[1].text = "AGILITY: " + player.attributes[2];
        attributeDisplays[2].text = "PERCEPTION: " + player.attributes[4];
        attributeDisplays[3].text = "STAMINA: " + player.attributes[1];
        attributeDisplays[4].text = "INTELLIGENCE: " + player.attributes[3];

        attributeDisplays[5].text = "REMAINING POINTS: " + player.attributePoints;
    }

    public void DisplayTempOutcomes()
    {
        NewPlayer player = NewPlayer.Instance;

        for (int i = 0; i < 6; i++)
        {
            attributeDisplays[i].color = new Color(1f, 0f, 0f, 0.8f);
        }

        attributeDisplays[0].text = "STRENGTH: " + (player.attributes[0] + tempAttributes[0]);
        attributeDisplays[1].text = "AGILITY: " + (player.attributes[2] + tempAttributes[1]);
        attributeDisplays[2].text = "PERCEPTION: " + (player.attributes[4] + tempAttributes[2]);
        attributeDisplays[3].text = "STAMINA: " + (player.attributes[1] + tempAttributes[3]);
        attributeDisplays[4].text = "INTELLIGENCE: " + (player.attributes[3] + tempAttributes[4]);

        attributeDisplays[5].text = "REMAINING POINTS: " + (player.attributePoints - tempSpentAP);
    }

    public void DisplayTempStatBonuses()
    {
        NewPlayer player = NewPlayer.Instance;

        if (tempAttributes[0] > 0)
        {
            statDisplays[1].text = " +" + ((int)(10*(100*System.Math.Pow(2,(tempAttributes[0])/(100.0+tempAttributes[0]/11.25)) - player.intrinsicStats[0])))/10f;
            statDisplays[11].text = " +" + ((int)(10*(10*System.Math.Pow(2,(tempAttributes[0])/(100.0+tempAttributes[0]/11.25)) - player.intrinsicStats[5])))/10f;
        }
        else
        {
            statDisplays[1].text = "";
            statDisplays[11].text = "";
        }

        if (tempAttributes[1] > 0)
        {
            double prospectiveMovementSpeedCap;
            if (player.capPreferences[0] == 0)
            {
                prospectiveMovementSpeedCap = System.Math.Ceiling(player.capValues[0,0]/100.0*(90.0 + player.attributes[2] + tempAttributes[1] - 100.0) + 100.0);
            }
            else
                prospectiveMovementSpeedCap = System.Math.Ceiling(player.capValues[0,1]);

            double prospectiveMovementSpeed = player.intrinsicStats[3] + tempAttributes[1];
            double prospectiveAttackRate = player.intrinsicStats[4] + tempAttributes[1];
            double prospectiveFerocity = player.intrinsicStats[7];

            if (prospectiveMovementSpeed > prospectiveMovementSpeedCap)
            {
                statDisplays[7].text = " +" + ((int)(10*(prospectiveMovementSpeedCap - player.intrinsicStats[3])))/10f;
                prospectiveFerocity += (prospectiveMovementSpeed - prospectiveMovementSpeedCap)/2 - player.intrinsicStats[7];
            }
            else
                statDisplays[7].text = " +" + ((int)(10*(prospectiveMovementSpeed - player.intrinsicStats[3])))/10f;

            if (prospectiveAttackRate > player.attackRateCap)
            {
                statDisplays[9].text = " +" + (player.attackRateCap - player.intrinsicStats[4]);
                prospectiveFerocity += (prospectiveAttackRate - player.attackRateCap)/2 - player.intrinsicStats[7];
            }
            else
                statDisplays[9].text = " +" + (prospectiveAttackRate - player.intrinsicStats[4]);

            statDisplays[15].text = " +" + prospectiveFerocity;
        }
        else
        {
            statDisplays[7].text = "";
            statDisplays[9].text = "";
            statDisplays[15].text = "";
        }

        if (tempAttributes[2] > 0)
        {
            // If set to 'half'
            double tempCritRatePerceptionPointsCap = (player.attributes[4] - 10 + tempAttributes[2]) / 2;

            if (tempAttributes[2] + player.attributes[4] > System.Math.Ceiling(tempCritRatePerceptionPointsCap))
            {
                statDisplays[17].text = " +" + ((int)(10*(100*(1-System.Math.Pow(0.5,System.Math.Ceiling(tempCritRatePerceptionPointsCap)/(100.0))) - player.intrinsicStats[8])))/10f;
                statDisplays[19].text = " +" + ((int)(10*(1.5*((tempAttributes[2]) - System.Math.Ceiling(tempCritRatePerceptionPointsCap)) - player.intrinsicStats[9])))/10f;
            }
            else
            {
                statDisplays[17].text = " +" + ((int)(10*(100*(1-System.Math.Pow(0.5,tempAttributes[2]/(100.0))) - player.intrinsicStats[8])))/10f;
                statDisplays[19].text = "";
            }
        }
        else
        {
            statDisplays[17].text = "";
            statDisplays[19].text = "";
        }

        if (tempAttributes[3] > 0)
        {
            statDisplays[5].text = " +" + (100 + 2*tempAttributes[3] - player.intrinsicStats[2]);
            statDisplays[21].text = " +" + ((int)(10*(100 * (1 + (tempAttributes[3] / 2000f)) - (100.0 * player.intrinsicStats[10] / 17.0))))/10f;
        }
        else
        {
            statDisplays[5].text = "";
            statDisplays[21].text = "";
        }

        if (tempAttributes[4] > 0)
        {
            statDisplays[13].text = " +" + ((int)(10*(10*System.Math.Pow(2,(tempAttributes[4])/(100.0+tempAttributes[0]/11.25)) - player.intrinsicStats[6])))/10f;
        }
        else
        {
            statDisplays[13].text = "";
        }

        // Stat line colours
        for (int i = 0; i < 22; i += 2)
        {
            statDisplays[i].color = new Color(1f, 1f, 1f, 1f);
        }

        statDisplays[0].text = "HEALTH POINTS: " + ((int)(10*player.intrinsicStats[0]))/10f;
        statDisplays[4].text = "MANA POINTS: " + player.intrinsicStats[2];
        statDisplays[6].text = "MOVEMENT SPEED: " + ((int)(10*player.intrinsicStats[3]))/10f + "%";
        statDisplays[8].text = "ATTACK RATE: " + ((int)(10*player.intrinsicStats[4]))/10f + "%";
        statDisplays[10].text = "PHYSICAL DAMAGE: " + ((int)(10*player.intrinsicStats[5]))/10f;
        statDisplays[12].text = "MAGICAL DAMAGE: " + ((int)(10*player.intrinsicStats[6]))/10f;
        statDisplays[14].text = "FEROCITY: " + player.intrinsicStats[7];
        statDisplays[16].text = "CRIT RATE: " + ((int)(10*player.intrinsicStats[8]))/10f + "%";
        statDisplays[18].text = "CRIT DAMAGE: " + ((int)(10*player.intrinsicStats[9]))/10f;
        statDisplays[20].text = "JUMP POWER: " + ((int)(100/1.7*player.intrinsicStats[10]))/10f;
    }

    public void DisplayTempStatOutcomes()
    {
        NewPlayer player = NewPlayer.Instance;

        if (tempAttributes[0] > 0)
        {
            statDisplays[0].text = "HEALTH POINTS: " + ((int)(10*(100*System.Math.Pow(2,(player.attributes[0] - 10.0 + tempAttributes[0])/(100.0+player.attributes[0] - 10.0 + tempAttributes[0]/11.25)))))/10f;
            statDisplays[10].text = "PHYSICAL DAMAGE: " + ((int)(10*(10*System.Math.Pow(2,(player.attributes[0] - 10.0 + tempAttributes[0])/(100.0+player.attributes[0] - 10.0 + tempAttributes[0]/11.25)))))/10f;
        }
        else
        {
            statDisplays[0].text = "HEALTH POINTS: " + player.intrinsicStats[0];
            statDisplays[10].text = "PHYSICAL DAMAGE: " + player.intrinsicStats[5];
        }

        if (tempAttributes[1] > 0)
        {
            double prospectiveMovementSpeedCap;
            if (player.capPreferences[0] == 0)
            {
                prospectiveMovementSpeedCap = System.Math.Ceiling(player.capValues[0,0]/100.0*(90.0 + player.attributes[2] + tempAttributes[1] - 100.0) + 100.0);
            }
            else
                prospectiveMovementSpeedCap = System.Math.Ceiling(player.capValues[0,1]);

            double prospectiveMovementSpeed = player.intrinsicStats[3] + tempAttributes[1];
            double prospectiveAttackRate = player.intrinsicStats[4] + tempAttributes[1];
            double prospectiveFerocity = player.intrinsicStats[7];

            if (prospectiveMovementSpeed > player.movementSpeedCap)
            {
                statDisplays[6].text = "MOVEMENT SPEED: " + ((int)(10*(prospectiveMovementSpeedCap)))/10f + "%";
                prospectiveFerocity += (prospectiveMovementSpeed - prospectiveMovementSpeedCap)/2;
            }
            else
                statDisplays[6].text = "MOVEMENT SPEED: " + ((int)(10*(prospectiveMovementSpeed)))/10f + "%";

            if (prospectiveAttackRate > player.attackRateCap)
            {
                statDisplays[8].text = "ATTACK RATE: " + (player.attackRateCap) + "%";
                prospectiveFerocity += (prospectiveAttackRate - player.attackRateCap)/2;
            }
            else
                statDisplays[8].text = "ATTACK RATE: " + (prospectiveAttackRate) + "%";

            statDisplays[14].text = "FEROCITY: " + prospectiveFerocity;
        }
        else
        {
            statDisplays[6].text = "MOVEMENT SPEED: " + player.intrinsicStats[3] + "%";
            statDisplays[8].text = "ATTACK RATE: " + player.intrinsicStats[4] + "%";
            statDisplays[14].text = "FEROCITY: " + player.intrinsicStats[7];
        }

        if (tempAttributes[2] > 0)
        {
            // If set to 'half'
            player.critRatePerceptionPointsCap = (player.attributes[4] - 10 + tempAttributes[2]) / 2;

            if (tempAttributes[2] + player.attributes[4] > System.Math.Ceiling(player.critRatePerceptionPointsCap))
            {
                statDisplays[16].text = "CRIT RATE: " + ((int)(10*(100*(1-System.Math.Exp(-0.01*System.Math.Ceiling(player.critRatePerceptionPointsCap))))))/10f;
                statDisplays[18].text = "CRIT DAMAGE: " + ((int)(10*((tempAttributes[2]) - System.Math.Ceiling(player.critRatePerceptionPointsCap))))/10f;
            }
            else
            {
                statDisplays[16].text = "CRIT RATE: " + ((int)(10*(100*(1-System.Math.Exp(-0.01*tempAttributes[2])))))/10f + "%";
                statDisplays[18].text = "CRIT DAMAGE: " + player.intrinsicStats[9];
            }
        }
        else
        {
            statDisplays[16].text = "CRIT RATE: " + player.intrinsicStats[8] + "%";
            statDisplays[18].text = "CRIT DAMAGE: " + player.intrinsicStats[9];
        }

        if (tempAttributes[3] > 0)
        {
            statDisplays[4].text = "MANA POINTS: " + (100 + 2*tempAttributes[3]);
            statDisplays[20].text = "JUMP POWER: " + ((int)(10*(100 * (1 + (tempAttributes[3] / 2000f)))))/10f;
        }
        else
        {
            statDisplays[4].text = "MANA POINTS: " + player.intrinsicStats[2];
            statDisplays[20].text = "JUMP POWER: " + player.intrinsicStats[10];
        }

        if (tempAttributes[4] > 0)
        {
            statDisplays[12].text = "MAGICAL DAMAGE: " + ((int)(10*(10*System.Math.Pow(2,(tempAttributes[4])/(100.0+tempAttributes[0]/11.25)))))/10f;
        }
        else
        {
            statDisplays[12].text = "MAGICAL DAMAGE: " + player.intrinsicStats[6];
        }

        for (int i = 0; i < 22; i += 2)
        {
            statDisplays[i].color = new Color(1f, 0f, 0f, 0.8f);
        }

        statDisplays[1].text = "";
        statDisplays[5].text = "";
        statDisplays[7].text = "";
        statDisplays[9].text = "";
        statDisplays[11].text = "";
        statDisplays[13].text = "";
        statDisplays[15].text = "";
        statDisplays[17].text = "";
        statDisplays[19].text = "";
        statDisplays[21].text = "";
    }

    public void ReadStringInput(string sInput)
    {
        NewPlayer player = NewPlayer.Instance;

        if (player.capPreferences[highlightedIndex-14] != null)
            Debug.Log("player.capPreferences not null at " + (highlightedIndex - 14) + ". value = " + player.capPreferences[highlightedIndex-14]);

        double nInput = double.Parse(sInput);

        Debug.Log("parsed value: " + nInput);
        
        if (highlightedIndex-14 == 0)
        {
            if (player.capPreferences[0] == 0)
            {
                if (nInput > 100.0)
                    nInput = 100.0;
                player.capValues[0,0] = nInput;
                Debug.Log("player.capValues[0,0] = " + player.capValues[0,0] + ", (nInput*(player.intrinsicStats[3] - 100.0) + 100.0) = " + (nInput*(player.intrinsicStats[3] - 100.0) + 100.0));
                statCapDisplays[0].text = "MOVEMENT SPEED CAP: " + System.Math.Ceiling(nInput*0.01*(player.intrinsicStats[3] - 100.0) + 100.0) + "%";
            }
            else if (player.capPreferences[highlightedIndex-14] == 1)
            {
                player.capValues[0,1] = nInput;
                Debug.Log("player.capValues[0,1] = " + player.capValues[0,1]);
                statCapDisplays[0].text = "MOVEMENT SPEED CAP: " + nInput + "%";
            }
        }

        inputTextDisplays[highlightedIndex-14].text = "" + player.capValues[highlightedIndex-14,player.capPreferences[highlightedIndex-14]];
        DisplayTempBonuses();
        DisplayTempStatBonuses();
    }
}
