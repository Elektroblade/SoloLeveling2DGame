using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIClassItem : MonoBehaviour
{
    public ClassItem classItem;
    private Image spriteImage;
    private UIClassItem selectedClassItem;
    private Tooltip tooltip;
    [System.NonSerialized] public float numerator;
    [System.NonSerialized] public Vector3 origScale = Vector3.zero;
    [System.NonSerialized] public bool highlighted = false;
    [System.NonSerialized] public bool selected = false;

    private void Awake()
    {
        spriteImage = GetComponent<Image>();
        if (GameObject.Find("SelectedClassItem") != null)
        {
            selectedClassItem = GameObject.Find("SelectedClassItem").GetComponent<UIClassItem>();
            tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>();
        }
        else
        {
            selectedClassItem = null;
        }

        if (origScale == Vector3.zero)
        {
            origScale = this.transform.localScale;
        }
    }

    public void UpdateClassItem(ClassItem classItem, float numerator)
    {
        this.numerator = numerator;

        if (classItem != null)
            this.classItem = classItem;
        else
            this.classItem = null;
        if (this.classItem != null)
        {
            if (!selected)
            {
                Color tempMyColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                tempMyColor.a = 1f;
                this.spriteImage.color = tempMyColor;
            }
            else
            {
                Color tmpImageColour = spriteImage.color;
                tmpImageColour.a = 1f;
                spriteImage.color = tmpImageColour;
            }

            spriteImage.sprite = this.classItem.icon;
            spriteImage.enabled = true;

            if (highlighted)
            {
                HighlightMe();
            }
            if (selected)
            {
                SelectMe();
            }
        }
        else
        {
            spriteImage.sprite = null;
            Color tmpImageColour = new Color(0f, 0f, 0f, 0f);
            tmpImageColour.a = 0f;
            spriteImage.color = tmpImageColour;
            spriteImage.enabled = false;

            Debug.Log("slot should be invisible now");
        }
    }

    public void HighlightMe()
    {
        highlighted = true;
        Color tempParentColor = new Color(1f, 1f, 1f, 1f);
        tempParentColor.a = 0.1f;
        this.transform.parent.GetComponent<Image>().color = tempParentColor;
        this.transform.localScale = new Vector3(origScale.x + 0.1f, origScale.y + 0.1f, 1);
    }

    public void UnhighlightMe()
    {
        highlighted = false;
        Color tempParentColor = new Color(1f, 1f, 1f, 1f);
        tempParentColor.a = 1f;
        this.transform.parent.GetComponent<Image>().color = tempParentColor;
        this.transform.localScale = new Vector3(origScale.x, origScale.y, 1);
    }

    public void SelectMe()
    {
        selected = true;
        Color tempMyColor = new Color(1f, 1f, 1f, 1f);
        tempMyColor.a = 1f;
        this.spriteImage.color = tempMyColor;
    }

    public void UnselectMe()
    {
        selected = false;
        Color tempMyColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        tempMyColor.a = 1f;
        this.spriteImage.color = tempMyColor;
    }
}