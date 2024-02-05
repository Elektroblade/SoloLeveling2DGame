using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SkillClass : MonoBehaviour
{
    public string className;
    public List<ActiveSkill> activeSkills;
    public List<PassiveSkill> slottablePassiveSkills;
    public List<PassiveSkill> autoPassiveSkills;

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
            return SortSkills(skills);
        }

        return skills;
    }

    public List<Skill> SortSkills(List<Skill> skills)
    {
        skills.Sort();
        return skills;
    }
}
