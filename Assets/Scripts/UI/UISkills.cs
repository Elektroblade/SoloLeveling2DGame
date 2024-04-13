using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkills : MonoBehaviour, SubMenu
{
    // Start is called before the first frame update
    private int activePanel = 0;
    [SerializeField] public GameObject[] panels;
    [SerializeField] public GameObject descriptionPanel;
    [System.NonSerialized] public bool isDoingStuff = false;

    public void WakeMeUp()
    {
        if (panels[activePanel] != null)
        {
            descriptionPanel.SetActive(true);
            panels[activePanel].GetComponent<SubMenu>().WakeMeUp();
        }
        for (int i = 0; i < panels.Length; i++)
        {
            if (i != activePanel)
            {
                panels[i].GetComponent<SubMenu>().Goodbye();
            }
        }

        isDoingStuff = true;
    }

    public void Goodbye()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].GetComponent<SubMenu>().Goodbye();
        }
        isDoingStuff = false;
        descriptionPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SetActivePanel(int panel)
    {
        if (!panels[panel].GetComponent<SubMenu>().IsEmpty())
        {
            if (panels[activePanel] != null)
            {
                panels[activePanel].GetComponent<SubMenu>().Goodbye();
            }
            activePanel = panel;
            if (panels[activePanel] != null)
            {
                panels[activePanel].GetComponent<SubMenu>().WakeMeUp();
            }
        }
    }

    public bool IsEmpty()
    {
        bool result = true;
        bool changeActivePanel = false;

        for (int i = panels.Length - 1; i >= 0; i--)
        {
            if (panels[i].GetComponent<SubMenu>().IsEmpty())
            {
                if (i == activePanel)
                {
                    changeActivePanel = true;
                }
            }
            else
            {
                result = false;

                if (changeActivePanel)
                {
                    SetActivePanel(i);
                    changeActivePanel = false;
                }
            }
        }

        return result;
    }
}
