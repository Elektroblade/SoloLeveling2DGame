using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIStatus : MonoBehaviour
{
    [SerializeField] public UIAttributes uIAttributes;
    [SerializeField] List<StatusMenuButton> buttons = new List<StatusMenuButton>();
    [SerializeField] List<TextMeshProUGUI> valueDisplays = new List<TextMeshProUGUI>();
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject skillsAndEquipmentPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject manaBar;
    [System.NonSerialized] public int highlightedIndex = 0;
    [System.NonSerialized] public bool isDoingStuff = false;

    public void WakeMeUp()
    {
        isDoingStuff = true;

        statsPanel.SetActive(false);
        statusPanel.SetActive(true);
        skillsAndEquipmentPanel.SetActive(true);

        NewPlayer player = NewPlayer.Instance;
        if (valueDisplays != null)
        {
            valueDisplays[0].text = "NAME: SUNG JIN-WOO";
            valueDisplays[1].text = "LEVEL: " + player.level;
            valueDisplays[4].text = "HP: " + player.health;
            valueDisplays[5].text = "MP: " + player.mana;
            valueDisplays[6].text = "STRENGTH: " + player.attributes[0];
            valueDisplays[7].text = "AGILITY: " + player.attributes[2];
            valueDisplays[8].text = "PERCEPTION: " + player.attributes[4];
            valueDisplays[9].text = "STAMINA: " + player.attributes[1];
            valueDisplays[10].text = "INTELLIGENCE: " + player.attributes[3];
            valueDisplays[11].text = "PHYSICAL DAMAGE REDUCTION: " + player.externalStats[1] + "%";
            valueDisplays[12].text = "REMAINING POINTS: " + player.attributePoints;
        }
        
        if (buttons != null)
        {
            buttons[highlightedIndex].HighlightMe();
        }
    }

    public void Goodbye()
    {
        uIAttributes.Goodbye();
        isDoingStuff = false;

        statusPanel.SetActive(false);
        skillsAndEquipmentPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDoingStuff)
        {
            DoUpdate();
        }
    }

    private void DoUpdate()
    {
        // Text/slider updates
        NewPlayer player = NewPlayer.Instance;
        valueDisplays[4].text = "HP: " + ((int)(10*player.health))/10f;
        valueDisplays[5].text = "MP: " + ((int)(10*player.mana))/10f;

        if (player.health < 0)
            healthBar.transform.localScale = new Vector2(0f, 1);
        else
            healthBar.transform.localScale = new Vector2((float) (player.health / player.externalStats[0]), 1);
        manaBar.transform.localScale = new Vector2((float) (player.mana / player.externalStats[2]), 1);

        // Button updates
        int prevHighlightedIndex = highlightedIndex;

        if (Input.GetKeyDown(KeyCode.UpArrow) && highlightedIndex > 0 && highlightedIndex < 4)
        {
            highlightedIndex--;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && highlightedIndex == 4)
        {
            highlightedIndex = 0;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && highlightedIndex != 4)
        {
            highlightedIndex = 4;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex < 3)
        {
            highlightedIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && highlightedIndex == 4)
        {
            highlightedIndex--;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && highlightedIndex == 4)
        {
            highlightedIndex = 1;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (highlightedIndex == 3)
            {
                buttons[prevHighlightedIndex].UnhighlightMe();
                isDoingStuff = false;
                uIAttributes.WakeMeUp();
            }
        }

        if (highlightedIndex != prevHighlightedIndex && isDoingStuff)
        {
            buttons[prevHighlightedIndex].UnhighlightMe();
            buttons[highlightedIndex].HighlightMe();
            Debug.Log("highlightedIndex = " + highlightedIndex);
        }
    }
}
