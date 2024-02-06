using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDatabase : MonoBehaviour
{
    [System.NonSerialized] public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    [System.NonSerialized] public List<ActiveSkill> activeSkills = new List<ActiveSkill>();
    [System.NonSerialized] public List<PassiveSkill> passiveSkills = new List<PassiveSkill>();
    [System.NonSerialized] public List<ClassItem> classItems = new List<ClassItem>();
    [System.NonSerialized] public Dictionary<string, int> categoryOrder;

    private void Awake()
    {
        Debug.Log("Inventory Database is awake.");
        BuildDatabase();
        Debug.Log("Database has been built.");
    }

    public InventoryItem GetInventoryItem(string id)
    {
        foreach (InventoryItem item in inventoryItems)
        {
            //Debug.Log(item.name);
        }

        return inventoryItems.Find(inventoryItem => (inventoryItem.id.CompareTo(id) == 0));
    }

    public ClassItem GetClassItem(string id)
    {
        return classItems.Find(classItem => (classItem.ToString().CompareTo(id) == 0));
    }

    void BuildDatabase()
    {
        categoryOrder = new Dictionary<string, int>
        {
            {"STRENGTH", 0},
            {"STAMINA", 1},
            {"AGILITY", 2},
            {"INTELLIGENCE", 3},
            {"PERCEPTION", 4},
            {"WORLD", 5}
        };

        inventoryItems = new List<InventoryItem>() {

            // Weapons
            new InventoryItem("Barca's Dagger", "BarcasDagger", "WEAPON", "DAGGER",
            "A dagger used by the great warrior Barca. A powerful spell of weightlessness imbued in the dagger allows the user to be more agile while wielding it.\nATTACK POWER +100\nAGILITY +10",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 50.0},
                {"Agility==", 50.0},
                {"PhysicalPower", 100.0},
                {"Agility+", 10.0}
            }),

            new InventoryItem("Morgul Flail", "MorgulFlail", "WEAPON", "FLAIL",
            "An unweildy flail wrought from cursed steel with a great, deadly head capable of shattering shields or killing enemies in one swing.",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 65.0},
                {"Strength==", 65.0},               // = means Threshold. First Threshold is used for attackRate scaling.
                {"PhysicalPower", (65.0 / 4.0) * 3.3 * 0.95},    // (Level / combo value) * combo time * stagger modifier
                {"Stagger*", 1.5}
            }),

            new InventoryItem("Gartana", "Gartana", "WEAPON", "LONGSWORD",
            "A hungry blade.",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 70.0},
                {"Agility==", 70.0},
                {"PhysicalPower", (70.0 / 4.0) * 1.0 * 0.95},
                {"Bleed*", 1.0}
            }),

            //Helmets
            new InventoryItem("Crimson Knight's Helmet", "CrimsonKnightsHelmet", "ARMOUR", "HELMET", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 60.0}
            }),

            // Collars
            new InventoryItem("Warden's Collar", "WardensCollar", "ADORNMENT", "COLLAR", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 40.0}
            }),

            // Chestplates
            new InventoryItem("High-Rank Knight's Chestplate", "HighRankKnightsChestplate", "ARMOUR", "CHESTPLATE", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 30.0}
            }),

            // Gloves
            new InventoryItem("High-Rank Knight's Gauntlets", "HighRankKnightsGauntlets", "ARMOUR", "GLOVES", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 30.0}
            }),

            // Rings
            new InventoryItem("High-Rank Mage's Ring", "HighRankMagesRing", "ADORNMENT", "RING", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 30.0}
            }),

            new InventoryItem("The One Ring", "TheOneRing", "ADORNMENT", "RING",
            "<i>Ash nazg durbatulûk, ash nazg gimbatul, ash nazg thrakatulûk, agh burzum-ishi krimpatul.</i>\n\n"
            + "A Ring of made of gold, forged by the Dark Lord Sauron in the fires of Mouth Doom. While wearing the One Ring, "
            + "your Intelligence and Perception are increased dramatically, but because the Ring is endowed with malevolent agency, "
            + "you are unable to avoid harming innocent civilians and hunters while wearing it.",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 90.0},
                {"Intelligence+", 200.0},
                {"Perception+", 200.0}
            }),

            // Boots
            new InventoryItem("Mid-Rank Assassin's Boots", "MidRankAssassinsBoots", "ARMOUR", "BOOTS", "",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 20.0}
            }),
            
            // Smithing
            new InventoryItem("Garfield's Appetite", "GarfieldsAppetite", "MATERIAL", "SMITHING",
            "The all-consuming hunger of Garfield, used to forge weapons with a sadistic appetite for blood and lasagna, "
            + "and armour whose bearer shall be suave and implacable.",
            new Dictionary<string, double>
            {
                {"AcquisitionDifficulty", 70.0}
            })
        };

        activeSkills = new List<ActiveSkill>() {
            
            // Electromancer
            new ActiveSkill("Rakurai", "Rakurai", "ELECTROMANCER", 1, -1.0, 1.5,
                "While channeling this skill, imbue all weapon attacks with a bolt of directional lightning. Tap to start channeling, tap again to stop."),

            // Geomancer
            new ActiveSkill("Seismic Tsunami", "SeismicTsumani", "GEOMANCER", 1, 20.0, -1.0,
                "Summon a giant seismic wave in front of you, launching enemies diagonally and dealing massive physical damage."),

            // Necromancer
            new ActiveSkill("Shadow Extraction", "ShadowExtraction", "NECROMANCER", 1, -1.0, -1.0,
                "A Shadow Soldier is created from a body without life by taking out its mana. The chance of failure increases the higher the target's attributes are, "
                + "and the more time passed since the target's death."),

            // Shadow Monarch
            new ActiveSkill("Summon Shadow", "SummonShadow", "SHADOW MONARCH", 2, -1.0, -1.0,
                "Saved Soldiers can be summoned and reabsorbed whenever and wherever the animator desires. You must set another Active Skill slot to reabsorb summoned Shadows."),
            
            // Scourge of Vitality
            new ActiveSkill("Bloodbend", "Bloodbend", "SCOURGE OF VITALITY", 1, 20.0, 2.0,
                "Cast at the closest enemy you are facing. The enemy attacks wildly and their attacks can hit other enemies. Other enemies can be hit by an affected enemy "
                + "and these attacks trigger 50% of your life steal. Each new enemy you bloodbend has an instant cost and a drains your HP over time. You can die from this drain "
                + "and the only ways to stop bloodbending are to kill all affected creatures or leave the room. Upgrade to increase maximum enemies under your control."),

            // Great Ranger
            new ActiveSkill("Eye of Sung", "EyeOfSung", "TELEPATH", 1, 100.0, -1.0,
                "Obtained by felling Sauron, Lord of the Rings. Tap to open ally selection menu. Select an ally to monitor. If there are enemies in range, "
                + "tap again to teleport to the enemy nearest to that ally. Monitoring has no cost, but teleport has large instant cost."),

            // World
            new ActiveSkill("Flaming Hand", "FlamingHand", "WORLD", 1, 10.0, -1.0,
                "Obtained by felling Sauron, Lord of the Rings. For 30 seconds after using this skill, your Morgul weapon attacks set enemies ablaze."),

            new ActiveSkill("Maiar Strike", "MaiarStrike", "WORLD", 1, -1.0, 2.0,
                "Obtained by felling Sauron, Lord of the Rings. While channeling this skill, adds 2% of Magic Damage to Crit Rate "
                + "and multiplies Crit Damage by Crit Rate if Crit Rate exceeds 100%.")
        };

        passiveSkills = new List<PassiveSkill>() {
            // Pyromancer
            new PassiveSkill("Fire Breathing", "FireBreathing", "PYROMANCER", false,
                "When charging normal attacks, breathe fire in a cone. Contact damage falls off with distance."),

            // Firelord
            new PassiveSkill("Aerodynamic Heating", "AerodynamicHeating", "FIRELORD", true,
                "When ending your jump early, a burst of intense thermal energy flares above you as you begin your descent towards earth."),

            // Thunderlord
            new PassiveSkill("Lightning Dash", "LightningDash", "THUNDERLORD", false,
                "You move the same distance twice as fast while dodging, but still retain the same invincibility window. Enemies in your wake are electrocuted."),

            // Geomancer
            new PassiveSkill("Tumultuous Takeoff", "TumultuousTakeoff", "GEOMANCER", true,
                "When jumping, your feet emit an tremor that deals physical damage."),

            // Tectonic Emperor
            new PassiveSkill("Earth Prism", "EarthPrism", "TECTONIC EMPEROR", false,
                "When jumping, in addition to a tremor, a rock prism emerges from the ground. Strike this rock with your weapon to hurl disks of rock. "
                + "The maximum number of disks you can produce from a single prism is based on your Strength stat."),

            // Bloodmage
            new PassiveSkill("Enhanced Life Steal", "EnhancedLifeSteal", "BLOODMAGE", true,
                "You regenerate 2.5/3.75/5/6.25/7.5% HP from landing weapon attacks."),
            
            new PassiveSkill("Cost Autonomy", "CostAutonomy", "BLOODMAGE", true,
                "While you have the Bloodmage job slotted, you can use sliders for each Active Skill to choose between spending MP or HP."),

            new PassiveSkill("Tensile Siphoning", "TensileSiphoning", "BLOODMAGE", false,
                "While below 34% HP, each enemy hit by your weapon attacks spawns a tendril that extends your weapon attack to another enemy. "
                + "This tendril gives you the same regeneration as your weapon attacks."),

            // Scourge of Vitality
            new PassiveSkill("Blood Alliance", "BloodAlliance", "SCOURGE OF VITALITY", true,
                "Weapon and tendril life stealing also heals allies within a small radius. Upgrade to increase healing range."),

            new PassiveSkill("Vital Protraction", "Vital Protraction", "SCOURGE OF VITALITY", false,
                "While below 50% HP, reduces the amount of damage taken multiplicatively by 0-1% of your Strength, increasing linearly based on amount of HP missing below 50%.")
        };

        classItems = new List<ClassItem>() {
            // Electromancer
            new ClassItem(new string[2] {"ELECTROMANCER", "THUNDERLORD"}, "ELECTROMANCER", "AGILITY", "Harnesses AGILITY to generate deadly lightning attacks."),

            // Geomancer
            new ClassItem(new string[2] {"GEOMANCER", "TECTONIC EMPEROR"}, "GEOMANCER", "STRENGTH", "Harnesses STRENGTH to manipulate terrain."),

            // Pyromancer
            new ClassItem(new string[2] {"PYROMANCER", "FIRELORD"}, "PYROMANCER", "INTELLIGENCE", "Harnesses INTELLIGENCE to increase the temperature."),

            // Healer
            new ClassItem(new string[2] {"HEALER", "DOCTOR"}, "HEALER", "INTELLIGENCE", "Harnesses INTELLIGENCE to heal self and allies."),

            // Bloodmage
            new ClassItem(new string[2] {"BLOODMAGE", "SCOURGE OF VITALITY"}, "BLOODMAGE", "STRENGTH", "Harnesses STRENGTH to dynamically control one's own innards " 
                + "and turn the blood of opponents against them."),

            // Warrior
            new ClassItem(new string[2] {"WARRIOR", "BERSERKER"}, "WARRIOR", "STAMINA", "Harnesses STAMINA to ferociously overwhelm foes"),

            // Tank
            new ClassItem(new string[2] {"TANK", "IMPERVIOUS MASTODON"}, "TANK", "STAMINA", "Harnesses STAMINA to fortify oneself and draw foes away from allies."),

            // Knight
            new ClassItem(new string[2] {"KNIGHT", "SWORDMASTER"}, "KNIGHT", "STRENGTH", "Harnesses STRENGTH and technique with large, sharpened pieces of metal to sunder foes"),

            // Assassin
            new ClassItem(new string[2] {"ASSASSIN", "SILENT DEATH"}, "ASSASSIN", "AGILITY", "Harnesses AGILITY to strike unseen."),

            // Ranger
            new ClassItem(new string[2] {"RANGER", "TELEPATH"}, "RANGER", "PERCEPTION", "Harnesses PERCEPTION to increase the bounds of knowledge."),

            // Necromancer
            new ClassItem(new string[2] {"NECROMANCER", "SHADOW MONARCH"}, "NECROMANCER", "INTELLIGENCE", "Harnesses INTELLIGENCE to summon an army of shadows from the dead.")
        };
    }
}
