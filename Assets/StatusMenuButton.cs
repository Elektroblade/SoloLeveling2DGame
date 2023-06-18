using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusMenuButton : MonoBehaviour
{
    [System.NonSerialized] private bool isHighlighted = false;
    [SerializeField] private Image spriteImage;

    private void Awake()
    {
        spriteImage.enabled = true;
        UnhighlightMe();
    }

    public bool IsHighlighted()
    {
        return isHighlighted;
    }

    public void HighlightMe()
    {
        isHighlighted = true;
        spriteImage.color = new Color(1f, 1f, 1f, 0.1f);
    }

    public void UnhighlightMe()
    {
        isHighlighted = false;
        spriteImage.color = new Color(1f, 1f, 1f, 0f);
    }
}
