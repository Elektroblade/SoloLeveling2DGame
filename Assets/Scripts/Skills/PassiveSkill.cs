using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : MonoBehaviour, Skill
{
    public string name;
    public string id;
    public string source;
    public bool isAutomatic;
    public string description;
    public Sprite icon;

    public int xp;

    public int level;

    public PassiveSkill(string name, string id, string source, bool isAutomatic, string description)
    {
        this.name = name;
        this.id = id;
        this.source = source;
        this.isAutomatic = isAutomatic;
        this.description = "SOURCE: " + source + "\n";
        this.description += "TYPE: PASSIVE";

        this.description += "\n\n" + description + "\n\n";

        if (isAutomatic)
        {
            this.description += "This skill is automatically applied and does not cost a slot.";
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
}
