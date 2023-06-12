using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryItem inventoryItem;
    private Image spriteImage;
    private UIInventoryItem selectedInventoryItem;
    private Tooltip tooltip;
    [SerializeField] public bool isHotbarSlot;
    [System.NonSerialized] public Vector3 origScale;

    private void Start()
    {
        origScale = this.transform.localScale;
    }

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
    }

    public void UpdateInventoryItem(InventoryItem inventoryItem)
    {
        if (inventoryItem != null)
            this.inventoryItem = inventoryItem;
        else
            this.inventoryItem = null;
        if (this.inventoryItem != null)
        {
            Color tmpImageColour = spriteImage.color;
            tmpImageColour.a = 1f;
            spriteImage.color = tmpImageColour;
            spriteImage.sprite = this.inventoryItem.icon;
        }
        else
        {
            Color tmpImageColour = spriteImage.color;
            tmpImageColour.a = 0f;
            spriteImage.color = tmpImageColour;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Detected Pointer Click!");
        if (this.inventoryItem != null)
        {
            if (selectedInventoryItem.inventoryItem != null)
            {
                InventoryItem clone = new InventoryItem(selectedInventoryItem.inventoryItem);
                selectedInventoryItem.UpdateInventoryItem(this.inventoryItem);
                UpdateInventoryItem(clone);
            }
            else
            {
                selectedInventoryItem.UpdateInventoryItem(this.inventoryItem);
                UpdateInventoryItem(null);
            }
        }
        else if (selectedInventoryItem.inventoryItem != null)
        {
            UpdateInventoryItem(selectedInventoryItem.inventoryItem);
            selectedInventoryItem.UpdateInventoryItem(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.inventoryItem != null)
        {
            tooltip.GenerateTooltip(this.inventoryItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false);
    }

    public void HighlightMe()
    {
        Color tempParentColor = new Color(1f, 1f, 1f, 1f);
        tempParentColor.a = 0.1f;
        this.transform.parent.GetComponent<Image>().color = tempParentColor;
        this.transform.localScale = new Vector3(origScale.x + 0.1f, origScale.y + 0.1f, 1);
    }

    public void UnhighlightMe()
    {
        Color tempParentColor = new Color(1f, 1f, 1f, 1f);
        tempParentColor.a = 1f;
        this.transform.parent.GetComponent<Image>().color = tempParentColor;
        this.transform.localScale = new Vector3(origScale.x - 0.1f, origScale.y - 0.1f, 1);
    }
}
