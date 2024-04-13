using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface Skill : IComparable
{
    public string ToString();

    public int CompareTo(object incomingObject);
    public void AddXp(int xpAmount);
    
    public string GetId();
    public string GetSource();
    public int GetLevel();
    public Sprite GetSprite();
    public string GetDescription();
}
