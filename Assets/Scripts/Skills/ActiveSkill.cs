using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : Skill
{
    public string name;
    public string id;
    public string source;
    public Sprite icon;
    public int slotCost;
    public double instantCost;
    public double durationCost;
    public string description;

    public int xp;

    public int level;

    public ActiveSkill(string name, string id, string source, int slotCost, double instantCost, double durationCost, string description)
    {
        this.name = name;
        this.id = id;
        this.source = source;
        this.icon = Resources.Load<Sprite>("UI/Classes/" + source);
        this.slotCost = slotCost;
        this.instantCost = instantCost;
        this.durationCost = durationCost;
        this.description = "SOURCE: " + source + "\n";
        this.description += "TYPE: ACTIVE";

        this.description += "\n\n" + description + "\n\n";

        if (instantCost > 0.0)
        {
            this.description += "INSTANT COST -" + instantCost + "\n";
        }
        if (durationCost > 0.0)
        {
            this.description += "DURATION COST -" + durationCost + " PER SECOND";
        }

        this.icon = Resources.Load<Sprite>("UI/Skills/" + id);

        this.xp = 0;
        this.level = 0;
    }

    public void AddXp(int xpAmount)
    {
        this.xp += xpAmount;
    }

    public string ToString()
    {
        return name;
    }

    public int CompareTo(object incomingObject)
    {
        Skill incomingSkill = incomingObject as Skill;

        return this.ToString().CompareTo(incomingSkill.ToString());
    }

    public string GetId()
    {
        return id;
    }

    public string GetSource()
    {
        return source;
    }

    public int GetLevel()
    {
        return level;
    }

    public Sprite GetSprite()
    {
        return icon;
    }

    public string GetDescription()
    {
        return description;
    }
}
