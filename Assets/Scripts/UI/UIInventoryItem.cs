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

    private void Awake()
    {
        spriteImage = GetComponent<Image>();
        UpdateInventoryItem(null);  // Testing purposes
        selectedInventoryItem = GameObject.Find("SelectedInventoryItem").GetComponent<UIInventoryItem>();
        tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>();
    }

    public void UpdateInventoryItem(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
        if (this.inventoryItem != null)
        {
            spriteImage.color = Color.white;
            spriteImage.sprite = this.inventoryItem.icon;
        }
        else
        {
            spriteImage.color = Color.clear;
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
}
