using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Manages inventory, keeps several component references, and any other future control of the game itself you may need*/

public class GameManager : MonoBehaviour
{
    public AudioSource audioSource; //A primary audioSource a large portion of game sounds are passed through
    public DialogueBoxController dialogueBoxController;
    public HUD hud; //A reference to the HUD holding your health UI, coins, dialogue, etc.
    private static GameManager instance;
    [SerializeField] public AudioTrigger gameMusic;
    [SerializeField] public AudioTrigger gameAmbience;
    [SerializeField] public LevelScalingOrigin levelScalingOrigin;
    [System.NonSerialized] public InventoryStorage inventoryItems;
    [System.NonSerialized] public SkillStorage skillStorage;
    [SerializeField] public UIStatus uIStatus;
    public InventoryDatabase inventoryDatabase;
    public Transform pfDamagePopup;
    [System.NonSerialized] public int testingLocalDifficulty = 1;
    [System.NonSerialized] public int testingLocalDifficultyVariance = 0;
    [System.NonSerialized] System.Random random = new System.Random();

    // Singleton instantiation
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<GameManager>();
            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start Called");

        audioSource = GetComponent<AudioSource>();
        inventoryItems = GetComponent<InventoryStorage>();
        skillStorage = GetComponent<SkillStorage>();
        inventoryItems.inventoryUI.gameObject.SetActive(true);
        inventoryItems.inventoryUI.gameObject.SetActive(false);
        skillStorage.uISkills = skillStorage.uISkills.GetComponent<UISkills>();
        skillStorage.BuildSkillClasses();
        skillStorage.uISkills.gameObject.SetActive(false);
        uIStatus.Goodbye();

        GiveItem("BarcasDagger");
        GiveItem("MorgulFlail");
        GiveItem("CrimsonKnightsHelmet");
        GiveItem("WardensCollar");
        GiveItem("HighRankKnightsChestplate");
        GiveItem("HighRankKnightsGauntlets");
        GiveItem("HighRankMagesRing");
        GiveItem("MidRankAssassinsBoots");
        GiveItem("TheOneRing");

        GiveClass("ELECTROMANCER");
        GiveClass("KNIGHT");
        GiveClass("GEOMANCER");
        GiveClass("TANK");
        GiveClass("HEALER");
        GiveClass("ASSASSIN");
        GiveClass("BLOODMAGE");
        GiveClass("WARRIOR");
        GiveClass("RANGER");
        GiveClass("NECROMANCER");
        GiveClass("PYROMANCER");

        GiveSkill("Rakurai");
        GiveSkill("AerodynamicHeating");
        GiveSkill("LightningDash");
        GiveSkill("TumultuousTakeoff");
        GiveSkill("EarthPrism");
        GiveSkill("MaiarStrike");
        GiveSkill("VehementFerocity");
        GiveSkill("GrislyComeuppance");
        GiveSkill("EnduringFrenzy");
        GiveSkill("Ambidextrous");
        GiveSkill("VitalProtraction");
        GiveSkill("RewardingProficiency");
        GiveSkill("StrengthTraining");
        GiveSkill("ShatteringMorale");
        GiveSkill("TensileSiphoning");
        GiveSkill("BloodAlliance");
        GiveSkill("Regenerate");
        GiveSkill("SummonShadow");
        GiveSkill("ShadowExtraction");
        GiveSkill("SeismicTsunami");
        GiveSkill("Bloodbend");
        GiveSkill("GreatswordHuck");
        GiveSkill("EyeOfSung");
        GiveSkill("FlamingHand");
        GiveSkill("PolishedTechnique");
        GiveSkill("FireBreathing");
        GiveSkill("EnhancedLifeSteal");
        GiveSkill("CostAutonomy");
        GiveSkill("Redirect");
        GiveSkill("EarthCapsule");
        GiveSkill("BarrierOfResistance");
        GiveSkill("BarrierOfInvulnerability");
        GiveSkill("PatientEndurance");
        GiveSkill("DefensiveStance");

        //RemoveInventoryItem("MidRankAssassinsBoots");
        //RemoveInventoryItem("CrimsonKnightsHelmet");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inventoryItems.inventoryUI.gameObject.activeSelf)
            {
                NewPlayer.Instance.hasInventoryOpen = true;
            }
            else
            {
                NewPlayer.Instance.hasInventoryOpen = false;
            }
            
            inventoryItems.inventoryUI.gameObject.SetActive(!inventoryItems.inventoryUI.gameObject.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!uIStatus.gameObject.activeSelf)
            {
                NewPlayer.Instance.hasStatusOpen = true;
                uIStatus.gameObject.SetActive(true);
                uIStatus.WakeMeUp();
            }
            else
            {
                NewPlayer.Instance.hasStatusOpen = false;
                uIStatus.Goodbye();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!skillStorage.uISkills.gameObject.activeSelf)
            {
                NewPlayer.Instance.hasSkillOpen = true;
                skillStorage.uISkills.gameObject.SetActive(true);
                if (!skillStorage.uISkills.GetComponent<SubMenu>().IsEmpty())
                {
                    skillStorage.uISkills.WakeMeUp();
                }
                else
                {
                    NewPlayer.Instance.hasSkillOpen = false;
                }

            }
            else
            {
                NewPlayer.Instance.hasSkillOpen = false;
                skillStorage.uISkills.Goodbye();
            }
        }
    }

    public void GiveItem(string id)
    {
        //Debug.Log("Trying to add item: " + name);
        InventoryItem inventoryItemToAdd = inventoryDatabase.GetInventoryItem(id);
        inventoryItems.AddItem(inventoryItemToAdd, false, -1);
        //Debug.Log("Added item: " + inventoryItemToAdd.name);

        //hud.SetInventoryImage(Resources.Load<Sprite>("UI/InventoryItems/" + name));
    }

    public InventoryItem CheckForInventoryItem(string id)
    {
        return inventoryItems.Find(id);
    }

    public void RemoveInventoryItem(string id)
    {
        InventoryItem inventoryItemToRemove = CheckForInventoryItem(id);
        if (inventoryItemToRemove != null)
        {
            Debug.Log("id to remove = " + id + ", found id " + inventoryItemToRemove.id);
            inventoryItems.RemoveItem(inventoryItemToRemove);
            Debug.Log("Item removed: " + inventoryItemToRemove.id);
        }

        //hud.SetInventoryImage(hud.blankUI);
    }

    public void ClearInventory()
    {   
        inventoryItems.Clear();
        //hud.SetInventoryImage(hud.blankUI);
    }

    public double GetRandomDouble(double minimum, double maximum)
    {
        return random.NextDouble() * (maximum - minimum) + minimum;
    }

    public int GetRandomInt(int minimum, int maximum)
    {
        return random.Next(minimum, maximum);
    }

    public static double GetDamageTotal(double[] damage)
    {
        double output = 0;
        double defence = NewPlayer.Instance.externalStats[1];
        for (int i = 0; i < damage.Length - 2; i++)
        {
            output += 100*damage[i]/(defence + 100);
        }

        return output;
    }

    public void GiveClass(string id)
    {
        ClassItem classItemToAdd = inventoryDatabase.GetClassItem(id);
        skillStorage.AddClassItem(classItemToAdd);
    }

    public void GiveSkill(string id)
    {
        Skill skillItemToAdd = inventoryDatabase.GetSkillItem(id);
        if (skillItemToAdd == null)
        {
            Debug.Log("ERROR: There is no skill with id \"" + id + "\".");
        }
        else
        {
            if (skillItemToAdd.GetSource().Equals("WORLD"))
                GiveClass("WORLD");
            skillStorage.AddSkillItem(skillItemToAdd);
        }
    }
}
