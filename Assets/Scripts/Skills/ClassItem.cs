using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassItem
{
    public string[] names;
    public string id;
    public string baseAttribute;
    public Sprite icon;
    public string description;

    public int xp;

    public int level;

    public ClassItem(string[] names, string id, string baseAttribute, string description)
    {
        this.names = new string[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            this.names[i] = names[i];
        }
        this.id = id;
        this.baseAttribute = baseAttribute;
        this.icon = Resources.Load<Sprite>("UI/Classes/" + id);
        this.description = description;
        
        this.xp = 0;
        this.level = 0;
    }

    public void AddXp(int xpAmount)
    {
        this.xp += xpAmount;
    }

    public string ToString()
    {
        return this.id;
    }
}