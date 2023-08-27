using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*The core functionality of both the EnemyFlyer and the EnemyWalker*/

[RequireComponent(typeof(RecoveryCounter))]

public class EnemyBase : MonoBehaviour
{
    public enum EnemyType { GorefieldFlyer, GorefieldTall };
    [Header ("Reference")]
    [SerializeField] EnemyType enemyType;
    private double avgHitsToKill = 10;
    private double avgAttacksToDie = 10;
    [System.NonSerialized] public AudioSource audioSource;
    public Animator animator;
    private AnimatorFunctions animatorFunctions;
    [SerializeField] Instantiator instantiator;
    [System.NonSerialized] public RecoveryCounter recoveryCounter;
    [System.NonSerialized] private int level;
    [System.NonSerialized] private int xp;
    public double[] intrinsicStats = new double[11];
    [SerializeField] public bool reanimated;
    [System.NonSerialized] public int reanimatedSlotIndex;
    [SerializeField] private GameObject ariseContainer;
    [SerializeField] public Transform siphonOrigin;
    private int ferocityTotal = 0;
    private int ferocityCounter = 0;
    private float staggerTimer = 0;

    [Header ("Properties")]
    [SerializeField] private GameObject deathParticles;
    [System.NonSerialized] public double health;
    [SerializeField] public GameObject healthBarUI;
    [SerializeField] public Slider healthBarSlider;
    [SerializeField] public Slider xpBarSlider;
    [SerializeField] public TextMeshPro levelUI;
    private Color levelUIColor;
    private string baseLevelUIText;
    public AudioClip hitSound;
    public bool isBomb;
    [SerializeField] bool requirePoundAttack; //Requires the player to use the down attack to hurt

    void Start()
    {
        // Testing
        level = (int) GameManager.Instance.GetRandomDouble(GameManager.Instance.testingLocalDifficulty + 1, 
        GameManager.Instance.testingLocalDifficulty + GameManager.Instance.testingLocalDifficultyVariance);

        recoveryCounter = GetComponent<RecoveryCounter>();
        audioSource = GetComponent<AudioSource>();
        animatorFunctions = GetComponent<AnimatorFunctions>();

        //Debug.Log("starting level = " + level);

        if (enemyType == EnemyType.GorefieldFlyer)
        {
            avgHitsToKill = 2.5;
            avgAttacksToDie = 2.5;
        }
        else if (enemyType == EnemyType.GorefieldTall)
        {
            avgHitsToKill = 7;
            avgAttacksToDie = 5;
        }

        recalculateIntrinsicStats();

        health = intrinsicStats[0];
    }

    public void SummonReanimated(int newLevel, int newXp, double newHealth)
    {
        level = newLevel;
        xp = newXp;
        recalculateIntrinsicStats();
        health = newHealth;         // Consider the correct maxHealth!
    }

    public int GetLevel()
    {
        return level;
    }

    void recalculateIntrinsicStats()
    {
        intrinsicStats[0] = 10*avgHitsToKill*(1+level/100.0)*(1+level/75.0)*System.Math.Pow(2,level/(100.0+level*0.1*(1-System.Math.Exp(-0.01*level))));
        intrinsicStats[1] = 0;
        intrinsicStats[5] = (10 / avgAttacksToDie) * System.Math.Pow(2,level/(100.0+level*0.1*(1-System.Math.Exp(-0.01*level))));
        intrinsicStats[6] = (10 / avgAttacksToDie) * System.Math.Pow(2,level/(100.0+level*0.1*(1-System.Math.Exp(-0.01*level))));

        if (level >= 1800)
        {
            intrinsicStats[3] = 1000;                                  // Default and absolute maximum movement speed percentage
            intrinsicStats[4] = 1000;
            intrinsicStats[7] = level - 900;
        }
        else
        {
            intrinsicStats[3] = level/2 + 100;
            intrinsicStats[4] = level/2 + 100;
            intrinsicStats[7] = level/0.5;
        }

        double critRatePerceptionPointsCap = level/2;
        intrinsicStats[8] = 100*(1-System.Math.Pow(0.5,System.Math.Ceiling(critRatePerceptionPointsCap)/(100.0)));
        intrinsicStats[9] = 1.5*(level - System.Math.Ceiling(critRatePerceptionPointsCap));

        baseLevelUIText = "Lvl " + level;
        levelUI.text = baseLevelUIText + "\n" + (int) health + "/" + (int) intrinsicStats[0];

        //Debug.Log("player level = " + NewPlayer.Instance.level + ", my level = " + level);
    }

    void Update()
    {
        healthBarSlider.value = (float) CalculateHealth();
        if (xpBarSlider)
            xpBarSlider.value = (float) CalculateXp();
        levelUI.text = baseLevelUIText + "\n" + (int) health + "/" + (int) intrinsicStats[0];

        if (health < intrinsicStats[0])
        {
            healthBarUI.SetActive(true);
        }

        if (health <= 0)
        {
            Die();
        }

        if (staggerTimer > 0)
            staggerTimer -= Time.deltaTime;
        else
            staggerTimer = 0;

        // Replenish health by 1% per second
        if (health < intrinsicStats[0] && reanimated)
        {
            health += 0.01*intrinsicStats[0]*Time.deltaTime;
        }
        // Avoid over-filling health
        if (health > intrinsicStats[0])
        {
            health = intrinsicStats[0];
        }

        if (level < NewPlayer.Instance.level - 5)
        {
            //Debug.Log("low");
            levelUIColor = new Color(73f/256, 73f/256, 73f, 255f/256);
        }
        else if (level <= NewPlayer.Instance.level + 5)
        {
            //Debug.Log("Med");
            levelUIColor = new Color(255f/256, 67f/256, 0f, 255f/256);
        }
        else
        {
            //Debug.Log("High");
            levelUIColor = new Color(150f/256, 2f/256, 0f, 255f/256);
        }

        levelUI.color = levelUIColor;
    }

    public void GetHurt(int launchDirection, double[] hitPower, int attackType, int attackerType)
    {
        //Debug.Log("recoveryCounter[" + attackType + "] before = " + recoveryCounter.counter[attackType] + ", recovering = " + recoveryCounter.recovering[attackType]);

        //Hit the enemy, causing a damage effect, and decreasing health. Allows for requiring a downward pound attack
        if ((GetComponent<Walker>() != null || GetComponent<Flyer>() != null || GetComponent<GorefieldFlyer>() != null 
            || GetComponent<GorefieldTall>() != null) && recoveryCounter.counter[attackType] > recoveryCounter.recoveryTime)
        {
            if (!requirePoundAttack || (requirePoundAttack && NewPlayer.Instance.pounding))
            {
                // Only melee attacks trigger active recovery
                if (attackerType == 0)
                {
                    
                    NewPlayer.Instance.RecoverByMelee(attackType);
                    
                    NewPlayer.Instance.cameraEffects.Shake(100, 1);

                    double critDamage = NewPlayer.Instance.externalStats[9];
                    if (staggerTimer > 0)
                    {
                        for (int i = 0; i < hitPower.Length - 2; i++)
                        {
                            hitPower[i] *= 1.5;
                            if ((int) hitPower[hitPower.Length - 2] == 1)
                            {
                                hitPower[i] *= (2.0*critDamage+100.0)/(critDamage+100.0);
                            }
                        }
                    }
                }
                else
                {
                    if (staggerTimer > 0)
                    {
                        for (int i = 0; i < hitPower.Length - 2; i++)
                        {
                            hitPower[i] *= 1.5;
                        }
                    }
                }

                string damagePopupString = "";
                
                for (int i = 0; i < hitPower.Length - 2; i++)
                {
                    double loss = 100*hitPower[i]/(intrinsicStats[1] + 100);
                    
                    //Debug.Log("Enemy position = " + transform.position.ToString() + ", loss = " + loss);

                    if (i != 0)
                    {
                        damagePopupString += "\n";
                    }

                    damagePopupString += (int) loss;
                    
                    health -= loss;
                }

                DamagePopup.Create(new Vector3(transform.position.x, transform.position.y, transform.position.z), damagePopupString, (int) hitPower[hitPower.Length - 1]);

                animator.SetTrigger("hurt");

                audioSource.pitch = (1);
                audioSource.PlayOneShot(hitSound);

                //Ensure the enemy and also the player cannot engage in hitting each other for the max recoveryTime for each
                recoveryCounter.counter[attackType] = 0;
                
                if (attackerType == 0)
                {
                    NewPlayer.Instance.recoveryCounter.counter[attackType] = 0;

                    if (NewPlayer.Instance.pounding)
                    {
                        NewPlayer.Instance.PoundEffect();
                    }
                }

                //Debug.Log("recoveryCounter after = " + recoveryCounter.counter[attackType] + "");

                

                //The Walker being launched after getting hit is a little different than the Flyer getting launched by a hit.
                if (GetComponent<Walker>() != null)
                {
                    Walker walker = GetComponent<Walker>();
                    walker.launch = launchDirection * walker.hurtLaunchPower / 25;
                    walker.velocity.y = walker.hurtLaunchPower / 5;
                    walker.directionSmooth = launchDirection;
                    walker.direction = walker.directionSmooth;
                }

                if (GetComponent<GorefieldTall>() != null)
                {
                    GorefieldTall walker = GetComponent<GorefieldTall>();
                    walker.launch = launchDirection * walker.hurtLaunchPower / 25;
                    walker.velocity.y = walker.hurtLaunchPower / 5;
                    walker.directionSmooth = launchDirection;
                    walker.direction = walker.directionSmooth;
                }

                if (GetComponent<Flyer>() != null)
                {
                    Flyer flyer = GetComponent<Flyer>();
                    flyer.speedEased.x = launchDirection * 2;
                    flyer.speedEased.y = 4;
                    flyer.speed.x = flyer.speedEased.x;
                    flyer.speed.y = flyer.speedEased.y;
                }

                if (GetComponent<GorefieldFlyer>() != null)
                {
                    GorefieldFlyer flyer = GetComponent<GorefieldFlyer>();
                    flyer.speedEased.x = launchDirection * 2;
                    flyer.speedEased.y = 4;
                    flyer.speed.x = flyer.speedEased.x;
                    flyer.speed.y = flyer.speedEased.y;
                }

                if (attackerType == 0)
                    NewPlayer.Instance.FreezeFrameEffect();
            }
        }
    }

    public void Die()
    {
        if (healthBarUI)
        {
            healthBarUI.SetActive(false);
        }

        if (xpBarSlider)
        {
            //Debug.Log("reanimated = " + reanimated + ", xp = " + xp);
        }
        
        if (NewPlayer.Instance.pounding)
        {
            NewPlayer.Instance.PoundEffect();
        }

        NewPlayer.Instance.cameraEffects.Shake(200, 1);
        health = 0;
        deathParticles.SetActive(true);
        deathParticles.transform.parent = transform.parent;
        instantiator.InstantiateObjects();
        Time.timeScale = 1f;
        // Reduce this later
        if ((GetComponent<Flyer>() == null || !isBomb) && !reanimated)
        {
            NewPlayer.Instance.addXp(2*(int)System.Math.Pow(level,2));

            // Necromancy
            EnemyBase[] theReanimated = NewPlayer.Instance.myReanimated;
            for (int i = 0; i < theReanimated.Length; i++)
            {
                if (theReanimated[i] != null)
                {
                    // Change multiplier back to 2
                    theReanimated[i].addXp(2*(int)System.Math.Pow(level,2));
                }
            }
            EnemyContainer myCorpseContainer;
            EnemyBase myCorpse;
            if (ariseContainer)
            {
                myCorpseContainer = Instantiate(ariseContainer, transform.position, Quaternion.identity).GetComponent<EnemyContainer>();
                if (myCorpseContainer)
                {
                    myCorpse = myCorpseContainer.GetEnemyBase();
                    myCorpse.SummonReanimated(level, 0, intrinsicStats[0]);
                    NewPlayer.Instance.GiveNewReanimated(myCorpse);
                }
            }
        }
        else if (reanimated)
        {
            NewPlayer.Instance.myReanimated[reanimatedSlotIndex] = null;
        }
        Destroy(gameObject);
    }

    public void Delete()
    {
        if (healthBarUI)
        {
            healthBarUI.SetActive(false);
        }

        Destroy(gameObject);
    }

    public double[] CalculateDamage()
    {
        //Debug.Log("Enemy calc damag at level " + level);

        int[] modifiers = {0, 0, 0, 0, 1, 1, 0, 1, 1, 1};
        double[] damage = new double[ferocityTotal + 2];

        //Debug.Log("damage.Length = " + damage.Length + "");

        // ferocity loop
        double i;

        // Here, weapon modifiers would be applied.
        double singleHitDamage = (10*modifiers[5]*intrinsicStats[5] + 10*modifiers[6]*intrinsicStats[6])*System.Math.Pow((intrinsicStats[9]*modifiers[9] + 100)/100,(intrinsicStats[8]*modifiers[8]/100));

        //Debug.Log("enemy singleHitDamage = " + singleHitDamage);

        for (i = (double) ferocityTotal; i >= 1.0; i--)
        {
            //Debug.Log("Index = " + (((int) -i) + damage.Length) + ", i = " + i);
            damage[((int) -i) + damage.Length - 2] = singleHitDamage;
        }

        // The final ferocity proc is basically multiplied by (ferocity + 100) mod 100
        if (i>0)
        {
            damage[damage.Length-3] = singleHitDamage*i;
        }

        if (reanimated)
            damage[damage.Length - 2] = -2;  // Unused value identifying critical hits
        else
            damage[damage.Length - 2] = -1;

        damage[damage.Length - 1] = 0;

        /*
        string damageStr = "" + damage[0];
        for (int j = 1; j < damage.Length; j++)
        {
            damageStr += ", " + damage[j];
        }
        Debug.Log(damageStr);
         */

        return damage;
    }

    public double CalculateHealth()
    {
        return health / intrinsicStats[0];
    }

    public double CalculateXp()
    {
        //Debug.Log("Calculated xp = " + xp / (10 * System.Math.Pow(level+1, 2)));
        return xp / (10 * System.Math.Pow(level+1, 2));
    }

    public void addXp(int xpAmount)
    {
        if (reanimated && gameObject.active)
        {
            int oldLevel = level;
            //Debug.Log("Adding xp = " + xpAmount + ", Next level requires " + (10*System.Math.Pow(level + 1,2)) + " total.");
            xp += xpAmount;
            while (xp != 0 && xp >= 10*System.Math.Pow(level + 1,2))
            {
                level++;
                xp -= 10*level^2;
            }

            if (level > oldLevel)
            {
                recalculateIntrinsicStats();
            }
        }
    }

    public void determineFerocity()
    {
        ferocityTotal = (int) (intrinsicStats[7] + 100) / 100;
        ferocityCounter = ferocityTotal;
    }

    public void decrementFerocityCounter()
    {
        if (ferocityCounter > 0)
            ferocityCounter--;
    }

    public void Stagger()
    {
        staggerTimer = 2f;
        animator.SetTrigger("Stagger");
    }
}