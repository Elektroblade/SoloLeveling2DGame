using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeMenuButton : MonoBehaviour
{
    [System.NonSerialized] private bool isHighlighted = false;
    [System.NonSerialized] private bool isPossible = true;
    [System.NonSerialized] private bool isSelected = false;
    [SerializeField] private bool isDecrease;
    [SerializeField] private Image spriteImage;

    private void Awake()
    {
        spriteImage.enabled = true;
        UnhighlightMe();
        ImpossibleMe();
    }

    public bool IsHighlighted()
    {
        return isHighlighted;
    }

    public bool IsPossible()
    {
        return isPossible;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void HighlightMe()
    {
        isHighlighted = true;

        if (!isSelected)
        {
            spriteImage.color = new Color(46/256f, 186/256f, 239/256f, 0.8f);
        }
    }

    public void UnhighlightMe()
    {
        isHighlighted = false;

        if (!isSelected)
        {
            spriteImage.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void ImpossibleMe()
    {
        if (isDecrease)
        {
            isPossible = false;
            spriteImage.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }
    
    public void PossibleMe()
    {
        isPossible = true;
        UnhighlightMe();
    }

    public void SelectMe()
    {
        isSelected = true;
        spriteImage.color = new Color(0f, 63/256f, 96/256f, 128/256f);
    }

    public void DeselectMe()
    {
        isSelected = false;
        if (isHighlighted)
        {
            HighlightMe();
        }
        else
        {
            UnhighlightMe();
        }
    }
}
