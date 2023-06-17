using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : MonoBehaviour
{
    public string name;
    public string id;
    public string source;
    public int slotCost;
    public double instantCost;
    public double durationCost;
    public string description;
    public Sprite icon;

    public ActiveSkill(string name, string id, string source, int slotCost, double instantCost, double durationCost, string description)
    {
        this.name = name;
        this.id = id;
        this.source = source;
        this.slotCost = slotCost;
        this.instantCost = instantCost;
        this.durationCost = durationCost;
        this.description = "SOURCE: " + source + "\n";
        this.description += "TYPE: ACTIVE";

        this.description += "\n\n" + description + "\n\n";

        if (instantCost > 0.0)
        {
            this.description += "INSANT COST -" + instantCost + "\n";
        }
        if (durationCost > 0.0)
        {
            this.description += "DURATION COST -" + durationCost + " PER SECOND";
        }

        this.icon = Resources.Load<Sprite>("UI/Skills/" + id);
    }
}
