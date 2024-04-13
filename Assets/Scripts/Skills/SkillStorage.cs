using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStorage : MonoBehaviour
{
    List<List<ClassItem>> classItemCategories = new List<List<ClassItem>>();
    int inventorySize = 9;
    public UISkills uISkills;

    public List<SkillClass>[] skillClasses;

    public void BuildSkillClasses()
    {
        skillClasses = new List<SkillClass>[6];
        skillClasses[0] = new List<SkillClass>(3);
        skillClasses[0].Add(new SkillClass("KNIGHT"));
        skillClasses[0].Add(new SkillClass("GEOMANCER"));
        skillClasses[0].Add(new SkillClass("BLOODMAGE"));
        skillClasses[1] = new List<SkillClass>(2);
        skillClasses[1].Add(new SkillClass("TANK"));
        skillClasses[1].Add(new SkillClass("WARRIOR"));
        skillClasses[2] = new List<SkillClass>(2);
        skillClasses[2].Add(new SkillClass("ASSASSIN"));
        skillClasses[2].Add(new SkillClass("ELECTROMANCER"));
        skillClasses[3] = new List<SkillClass>(2);
        skillClasses[3].Add(new SkillClass("HEALER"));
        skillClasses[3].Add(new SkillClass("PYROMANCER"));
        skillClasses[4] = new List<SkillClass>(1);
        skillClasses[4].Add(new SkillClass("RANGER"));
        skillClasses[5] = new List<SkillClass>(1);
        skillClasses[5].Add(new SkillClass("WORLD"));
        Debug.Log("skillClasses[0].Count = " + skillClasses[0].Count + ", skillClasses[0][0] == null is " + (skillClasses[0][0] == null));
    }

    
    public void AddClassItem(ClassItem classItem)
    {
        string baseAttribute = classItem.baseAttribute;
        bool fitsExistingCategory = false;

        // TODO: Change all code below
        for (int i = 0; i < classItemCategories.Count; i++)
        {
            if (baseAttribute.Equals(GameManager.Instance.inventoryDatabase.GetClassItem(classItemCategories[i][0].ToString()).baseAttribute))
            {
                for (int j = 0; j < classItemCategories[i].Count; j++)
                {
                    if (classItemCategories[i][j].ToString().Equals(classItem.ToString()))
                    {
                        Debug.Log("Newly obtained class " + classItem.ToString() + " is redundant.");
                        return;
                    }
                }

                // If not redundant, add class
                classItemCategories[i].Add(classItem);
                Debug.Log(classItem.ToString() + " was added to the existing " + baseAttribute + " category.");
                fitsExistingCategory = true;
                break;
            }
        }

        // If we did not find any classes in the same category, proceed.
        if (!fitsExistingCategory)
            classItemCategories.Add(new List<ClassItem>() {classItem});

        InventoryDatabase iDB = GameManager.Instance.inventoryDatabase;
        Dictionary<string, int> categoryOrder = iDB.categoryOrder;
        classItemCategories.Sort((a,b) => categoryOrder[iDB.GetClassItem(a[0].ToString()).baseAttribute].CompareTo(categoryOrder[iDB.GetClassItem(b[0].ToString()).baseAttribute]));
        
        uISkills.panels[0].GetComponent<UISkillCategory>().RebuildList(classItemCategories);
    }

    public ClassItem FindClass(string id)
    {
        string baseAttribute = GameManager.Instance.inventoryDatabase.GetClassItem(id).baseAttribute;
        for (int i = 0; i < classItemCategories.Count; i++)
        {
            if (baseAttribute.Equals(GameManager.Instance.inventoryDatabase.GetClassItem(classItemCategories[i][0].ToString()).baseAttribute))
            {
                for (int j = 0; j < classItemCategories[i].Count; j++)
                {
                    if (classItemCategories[i][j].ToString().Equals(id))
                    {
                        return classItemCategories[i][j];
                    }
                }
            }
        }
        return null;
    }

    public void AddSkillItem(Skill skill)
    {
        for (int i = 0; i < skillClasses.Length; i++)
        {
            for (int j = 0; j < skillClasses[i].Count; j++)
            {
                if (skill.GetSource().Equals(skillClasses[i][j].className))
                {
                    skillClasses[i][j].AddSkill(skill);
                    return;
                }
            }
        }
    }

    public void RebuildUISkillListFor(string source)
    {
        SkillClass skillClass = GetSkillClass(source);
        if (skillClass != null)
        {
            uISkills.panels[1].GetComponent<UIClass>().RebuildList(skillClass.GetSkills());
        }
    }

    public SkillClass GetSkillClass(string source)
    {
        for (int i = 0; i < skillClasses.Length; i++)
        {
            for (int j = 0; j < skillClasses[i].Count; j++)
            {
                if (skillClasses[i][j].className.Equals(source))
                    return skillClasses[i][j];
            }
        }
        return null;
    }
}
