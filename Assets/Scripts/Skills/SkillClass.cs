using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SkillClass
{
    public string className;
    public List<ActiveSkill> activeSkills = new List<ActiveSkill>();
    public List<PassiveSkill> slottablePassiveSkills = new List<PassiveSkill>();
    public List<PassiveSkill> autoPassiveSkills = new List<PassiveSkill>();

    public SkillClass(string className)
    {
        this.className = className;
    }

    public void AddSkill(Skill skill)
    {
        if (skill is ActiveSkill)
        {
            activeSkills = AddSkillHelper(skill, activeSkills.Cast<Skill>().ToList()).Cast<ActiveSkill>().ToList();
        }
        else if (skill is PassiveSkill)
        {
            PassiveSkill passiveSkill = skill as PassiveSkill;
            if (passiveSkill.isAutomatic)
            {
                autoPassiveSkills = AddSkillHelper(passiveSkill, autoPassiveSkills.Cast<Skill>().ToList()).Cast<PassiveSkill>().ToList();
            }
            else
            {
                slottablePassiveSkills = AddSkillHelper(passiveSkill, slottablePassiveSkills.Cast<Skill>().ToList()).Cast<PassiveSkill>().ToList();
            }
        }
    }

    private List<Skill> AddSkillHelper(Skill skill, List<Skill> skills)
    {
        bool isNew = true;
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i].ToString().Equals(skill.ToString()))
            {
                isNew = false;
            }
        }

        if (isNew)
        {
            skills.Add(skill);
            Debug.Log(skill.ToString() + " was added to the " + skill.GetSource() + " class.");
            return SortSkills(skills);
        }

        return skills;
    }

    public List<Skill> SortSkills(List<Skill> skills)
    {
        skills.Sort();
        return skills;
    }

    public string ToString()
    {
        return className;
    }

    public List<List<Skill>> GetSkills()
    {
        List<List<Skill>> result = new List<List<Skill>>();
        result.Add(new List<Skill>());
        foreach(ActiveSkill activeSkill in activeSkills)
        {
            result[0].Add((Skill) activeSkill);
        }
        result.Add(new List<Skill>());
        foreach(PassiveSkill passiveSkill in slottablePassiveSkills)
        {
            result[1].Add((Skill) passiveSkill);
        }
        result.Add(new List<Skill>());
        foreach(PassiveSkill passiveSkill in autoPassiveSkills)
        {
            result[2].Add((Skill) passiveSkill);
        }
        return result;
    }
}
