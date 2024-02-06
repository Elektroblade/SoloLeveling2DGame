using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventoryItem : MonoBehaviour
{
    public InventoryItem inventoryItem;
    private Image spriteImage;
    private UIInventoryItem selectedInventoryItem;
    private Tooltip tooltip;
    [SerializeField] public bool isHotbarSlot;
    [System.NonSerialized] public Vector3 origScale = Vector3.zero;
    [System.NonSerialized] public bool highlighted = false;
    [System.NonSerialized] public bool selected = false;

    private void Awake()
    {
        spriteImage = GetComponent<Image>();
        UpdateInventoryItem(null);  // Testing purposes
        if (GameObject.Find("SelectedInventoryItem") != null)
        {
            selectedInventoryItem = GameObject.Find("SelectedInventoryItem").GetComponent<UIInventoryItem>();
            tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>();
        }
        else
        {
            selectedInventoryItem = null;
        }

        if (origScale == Vector3.zero)
        {
            origScale = this.transform.localScale;
        }
    }

    public void UpdateInventoryItem(InventoryItem inventoryItem)
    {
        if (inventoryItem != null)
            this.inventoryItem = inventoryItem;
        else
            this.inventoryItem = null;
        if (this.inventoryItem != null)
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

            spriteImage.sprite = this.inventoryItem.icon;
            spriteImage.enabled = true;

            if (highlighted)
            {
                HighlightMe();
                GameManager.Instance.inventoryItems.inventoryUI.highlightedDescription.text = this.inventoryItem.description;
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

            //Debug.Log("slot should be invisible now");
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