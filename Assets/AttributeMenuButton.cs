using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeMenuButton : MonoBehaviour
{
    [System.NonSerialized] private bool isHighlighted = false;
    [System.NonSerialized] private bool isPossible = true;
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

    public void HighlightMe()
    {
        isHighlighted = true;
        spriteImage.color = new Color(46/256f, 186/256f, 239/255f, 0.8f);
    }

    public void UnhighlightMe()
    {
        isHighlighted = false;
        spriteImage.color = new Color(1f, 1f, 1f, 1f);
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
}
