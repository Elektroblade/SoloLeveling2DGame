using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private Text tooltip;

    void Start()
    {
        tooltip = GetComponentInChildren<Text>();
        tooltip.gameObject.SetActive(false);
    }

    public void GenerateTooltip(InventoryItem inventoryItem)
    {
        string statText = "";
        if (inventoryItem.stats.Count > 0)
        {
            foreach(var stat in inventoryItem.stats)
            {
                statText += stat.Key.ToString() + ": " + stat.Value.ToString() + "\n";
            }
        }
        string tooltip = string.Format("<b>{0}</b>\n{1}\n\n<b>{2}</b>",
            inventoryItem.name, inventoryItem.description, statText);
        //statText.text = tooltip;
        gameObject.SetActive(true);
    }
}
