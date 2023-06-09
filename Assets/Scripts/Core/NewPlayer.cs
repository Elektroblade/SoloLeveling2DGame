﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*Adds player functionality to a physics object*/

[RequireComponent(typeof(RecoveryCounter))]

public class NewPlayer : PhysicsObject
{
    enum NextAction {Idle, Dodge, Attack, Jump};
    [Header ("Reference")]
    public AudioSource audioSource;
    [SerializeField] private Animator animator;
    private AnimatorFunctions animatorFunctions;
    public GameObject attackHit;
    private CapsuleCollider2D capsuleCollider;
    public CameraEffects cameraEffects;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private AudioSource flameParticlesAudioSource;
    [SerializeField] private GameObject graphic;
    [SerializeField] private Component[] graphicSprites;
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] public LightningFist lightningFist;
    [SerializeField] public AerodynamicHeating aerodynamicHeating;
    [SerializeField] public SeismicWave seismicWave;
    [SerializeField] public EarthPrism earthPrism;
    [SerializeField] private GameObject pauseMenu;
    public RecoveryCounter recoveryCounter;
    private System.Random random = new System.Random();

    // Singleton instantiation
    private static NewPlayer instance;
    public static NewPlayer Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<NewPlayer>();
            return instance;
        }
    }

    [Header("Properties")]
    [SerializeField] private string[] cheatItems;
    public bool dead = false;
    public bool frozen = false;
    private float fallForgivenessCounter; //Counts how long the player has fallen off a ledge
    [SerializeField] private float fallForgiveness = .2f; //How long the player can fall from a ledge and still jump
    private float jumpEarlinessCounter;   // Counts how long it has been since player unsuccessfully jumped
    [SerializeField] private float jumpEarliness = .2f;   // How long, after the player tried to jump, that the jump may still be successful
    [System.NonSerialized] public string groundType = "grass";
    [System.NonSerialized] public RaycastHit2D ground; 
    [SerializeField] Vector2 hurtLaunchPower; //How much force should be applied to the player when getting hurt?
    private float launch; //The float added to x and y moveSpeed. This is set with hurtLaunchPower, and is always brought back to zero
    [SerializeField] private float launchRecovery; //How slow should recovering from the launch be? (Higher the number, the longer the launch will last)
    public float baseSpeed = 4f; // Max move speed at 0 agility
    public float maxSpeed = 4f; //Max move speed
    [System.NonSerialized] private float baseJumpPower = 17f;
    private bool jumping;
    public float dodgePower = 17f;
    public float dodgeVelocity = 17f;
    private bool canDodge = true;
    private bool dodging;                   // If false, invincibility may continue, but speed is no longer going to be calculated in this manner.
    private float dodgeDistanceSpent = 0f;
    private float dodgeDistance = 6.8f;
    private float dodgeTimer = 0;
    private Vector3 origLocalScale;
    [System.NonSerialized] public bool pounded;
    [System.NonSerialized] public bool pounding;
    [System.NonSerialized] public bool shooting = false;
    [System.NonSerialized] public bool isLightningImbued = false;
    [System.NonSerialized] public bool isLightningDash = true;
    [SerializeField] bool facingRight;
    private int ferocityTotal = 0;
    private int ferocityCounter = 0;
    private int comboIndex = 0;
    NextAction nextAction = NextAction.Idle;
    public float[] parryTimer = {0f, 0f, 0f, 0f};      // [0] = top left, [1] = top right, [2] = bottom left, [3] = bottom right
    private float parryForgiveness = 0.2f;        // How early the player can parry an attack
    [System.NonSerialized] public bool hasInventoryOpen = false;

    [Header ("Attributes")]
    // strength, stamina, agility, intellect, perception
    public int[] attributes = {10, 10, 10, 10, 10};

    [Header ("Default Stat Caps")]
    // Player can modify these values but the default setting shown here is the highest possible
    public double movementSpeedCap = 1000;                                  // Default and absolute maximum movement speed percentage
    public double attackRateCap = 1000;                                     // Default and absolute maximum attack rate percentage
    public double critRateIntrinsicCap = 100;                               // Default and absolute maximum intrinsic crit rate percentage
    public double critRatePerceptionPointsCap = 10000;                      // Calculated when critRateIntrinsicCap is adjusted (points to not spend on cd)

    [Header ("Stats")]
    // These are stats intrinsic to the player. The actual values after considering items etc. may be different.
    // health pool, defence, mana pool, movement speed, attack rate, physical power, magical power, ferocity, intrinsic crit rate, crit damage, jump power
    public double[] intrinsicStats = new double[11];
    // Crit rate can be modified by items but those modifications do not require recalculation of the above

    public double[] externalStats = new double[11];

    [Header ("Inventory")]
    public int coins;
    public double health;
    public double mana;
    public int xp;
    [SerializeField] public int level;
    [SerializeField] public int attributePoints;
    public EnemyBase[] myReanimated = new EnemyBase[10];

    [Header ("Sounds")]
    public AudioClip deathSound;
    public AudioClip equipSound;
    public AudioClip grassSound;
    public AudioClip hurtSound;
    public AudioClip[] hurtSounds;
    public AudioClip holsterSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip poundSound;
    public AudioClip punchSound;
    public AudioClip[] poundActivationSounds;
    public AudioClip outOfAmmoSound;
    public AudioClip stepSound;
    [System.NonSerialized] public int whichHurtSound;

    void Start()
    {
        // Test
        level = GameManager.Instance.testingLocalDifficulty;
        attributePoints = 5*GameManager.Instance.testingLocalDifficulty;

        Debug.Log("2 ^ (1/100) = " + (System.Math.Pow(2,0.01)));
        Cursor.visible = false;
        SetUpCheatItems();
        animatorFunctions = GetComponent<AnimatorFunctions>();
        origLocalScale = transform.localScale;
        recoveryCounter = GetComponent<RecoveryCounter>();

        // Debug; Remember to remove this
        movementSpeedCap = 300;
        //attackRateCap = 300;

        recalculateIntrinsicStats();
        recalculateExternalStats();

        health = (int) intrinsicStats[0];
        mana = (int) intrinsicStats[2];

        //Find all sprites so we can hide them when the player dies.
        graphicSprites = GetComponentsInChildren<SpriteRenderer>();
        SetGroundType();
    }

    public void recalculateIntrinsicStats()
    {
        // health pool, defence, mana pool, movement speed, attack rate, physical power, magical power, ferocity, intrinsic crit rate, crit damage, jump power
        //critRatePerceptionPointsCap = -(System.Math.Log(1-(critRateIntrinsicCap)/100))/1.01;
        critRatePerceptionPointsCap = (attributes[4] - 10) / 2;
        //Debug.Log("critRatePerceptionPointsCap = " + critRatePerceptionPointsCap);

        intrinsicStats[0] = 100*System.Math.Pow(2,(attributes[0] - 10)/100.0);      // Health pool
        intrinsicStats[1] = 0;                                                      // Defence
        intrinsicStats[2] = 80 + 2*attributes[1];                                   // Mana pool
        double movementSpeed = 90 + attributes[2];
        double attackRate = 90 + attributes[2];
        intrinsicStats[5] = 10*System.Math.Pow(2,(attributes[0] - 10)/100.0);
        intrinsicStats[6] = 10*System.Math.Pow(2,(attributes[3] - 10)/100.0);
        intrinsicStats[7] = 0;
        intrinsicStats[9] = 0;
        intrinsicStats[10] = baseJumpPower * (1 + (attributes[1] / 2000f));
        
        if (movementSpeed > movementSpeedCap)
        {
            intrinsicStats[3] = movementSpeedCap;
            intrinsicStats[7] += (movementSpeed - movementSpeedCap)/2;
        }
        else
            intrinsicStats[3] = movementSpeed;

        if (attackRate > attackRateCap)
        {
            intrinsicStats[4] = attackRateCap;
            intrinsicStats[7] += (attackRate - attackRateCap)/2;
        }
        else
            intrinsicStats[4] = attackRate;
        if (attributes[4] > System.Math.Ceiling(critRatePerceptionPointsCap))
        {
            intrinsicStats[8] = 100*(1-System.Math.Exp(-0.01*System.Math.Ceiling(critRatePerceptionPointsCap)));
            intrinsicStats[9] = (attributes[4] - 10) - System.Math.Ceiling(critRatePerceptionPointsCap);
        }
        else
        {
            intrinsicStats[8] = 100*(1-System.Math.Exp(-0.01*attributes[4]));
            intrinsicStats[9] = 0;
        }
    }

    public void recalculateExternalStats()
    {
        for (int i = 0; i < 11; i++)
        {
            externalStats[i] = intrinsicStats[i];
        }

        // For balance during testing; remove later
        externalStats[1] = 100.0*System.Math.Pow(level/100.0 + 1.0, 2) - 100.0;

        // Apply attack rate and movement speed
        animator.SetFloat("animAttackRate", (float)(externalStats[4]/100f));
        maxSpeed = (float) (externalStats[3]/100f) * baseSpeed;
    }

    private void Update()
    {
        // We do NOT need to recalculate stats on every update, only when attribute points are spent or items are equipped or unequipped

        // Replenish health by 1% per second
        if (health < externalStats[0])
        {
            health += 0.01*externalStats[0]*Time.deltaTime;
        }
        // Avoid over-filling health
        if (health > externalStats[0])
        {
            health = externalStats[0];
        }
        
        // Apply cost of duration abilities
        if (mana > 0)
        {
            if (isLightningImbued)
            {
                mana -= 1.5*Time.deltaTime;
            }
            // Other abilities
        }
        // Stop duration abilities if there are insufficient resources
        else if (isLightningImbued)
        {
            isLightningImbued = false;
            // Other abilities
        }
        
        // Avoid dropping below 0 mana
        if (mana < 0)
            mana = 0;
        
        // Replenish mana by 1% per second
        if (mana < externalStats[2])
        {
            mana += 0.01*externalStats[2]*Time.deltaTime;
        }
        // Avoid over-filling mana
        if (mana > externalStats[2])
        {
            mana = externalStats[2];
        }

        if (attributePoints/5 > 0)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                attributes[i]++;
            }
            attributePoints -= 5;

            recalculateIntrinsicStats();
            recalculateExternalStats();
        }

        // Start or end duration abilities
        if (!hasInventoryOpen && Input.GetKeyDown(KeyCode.Q))
        {
            if (isLightningImbued)
                isLightningImbued = false;
            else
                isLightningImbued = true;
        }

        for (int i = 0; i < parryTimer.Length; i++)
        {
            if (parryTimer[i] > 0)
                parryTimer[i] -= Time.deltaTime;
            else
                parryTimer[i] = 0;
        }
        
        ComputeVelocity();
    }

    protected void ComputeVelocity()
    {
        //Player movement & attack
        Vector2 move = Vector2.zero;
        ground = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -Vector2.up);

        //Lerp launch back to zero at all times
        launch += (0 - launch) * Time.deltaTime * launchRecovery;

        if (!hasInventoryOpen && Input.GetButtonDown("Cancel"))
        {
            pauseMenu.SetActive(true);
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            Cursor.visible = false;
            NewPlayer.Instance.hasInventoryOpen = false;
            GameManager.Instance.inventoryItems.inventoryUI.gameObject.SetActive(false);
        }

        //Movement, jumping, and attacking!
        if (!frozen && !dodging)
        {
            // Now that dodging invincibility can outlast the speed of dodging, can need to continue calculating invincibility after controls are available
            if (dodgeTimer != 0)
            {
                dodgeTimer -= Time.deltaTime;

                if (dodgeTimer <= 0)
                {
                    dodgeTimer = 0;
                    dodgeDistanceSpent = 0;
                    Physics2D.IgnoreLayerCollision(10, 12, false);
                    Physics2D.IgnoreLayerCollision(10, 15, false);
                }
            }
            
            if (!hasInventoryOpen)
                move.x = Input.GetAxis("Horizontal") + launch;

            if (((!hasInventoryOpen && Input.GetButtonDown("Jump")) || jumpEarlinessCounter > 0)
                && (animator.GetBool("grounded") == true || fallForgivenessCounter < fallForgiveness) && !jumping)
            {
                animator.SetBool("pounded", false);
                jumpEarlinessCounter = 0;
                Jump(1f);
            }
            else if ((!hasInventoryOpen && Input.GetButtonDown("Jump")) && animator.GetBool("grounded") == false && !jumping)
            {
                jumpEarlinessCounter = jumpEarliness;
            }

            if ((!hasInventoryOpen && Input.GetButtonUp("Jump")) && animator.GetBool("grounded") == false && jumping && velocity.y > 0.01)
            {
                // AerodynamicHeating start

                // If an attack animation has yet to hit, recalculate ferocity. Change 1 to a modifier if such a modifier exists.
                if (ferocityCounter < ((int) (1 * externalStats[7] + 100)/100))
                {
                    DetermineFerocity();
                    Debug.Log("Calculated new ferocity: " + ferocityCounter);
                }
                aerodynamicHeating.DisplaySprite();

                // AerodynamicHeating end

                velocity.y = 0;
                jumping = false;
            }
            else if ((!hasInventoryOpen && Input.GetButtonUp("Jump")) && !jumping)
            {
                jumpEarlinessCounter = 0;
            }

            if (Input.GetButton("Dodge") && canDodge)
            {
                canDodge = false;
                animator.SetBool("pounded", false);
                if (isLightningDash)
                    Dodge(2f);
                else
                    Dodge(1f);
            }

            //Flip the graphic's localScale
            if (!hasInventoryOpen && Input.GetAxis("Horizontal") > 0.01f)
            {
                graphic.transform.localScale = new Vector3(origLocalScale.x, transform.localScale.y, transform.localScale.z);
                facingRight = true;
            }
            else if (!hasInventoryOpen && Input.GetAxis("Horizontal") < -0.01f)
            {
                facingRight = false;
                graphic.transform.localScale = new Vector3(-origLocalScale.x, transform.localScale.y, transform.localScale.z);
            }

            //Punch
            if (Input.GetButtonDown("Fire1"))
            {
                nextAction = NextAction.Attack;
                animator.SetBool("attackBool", true);
                //animator.SetTrigger("attack");
                Shoot(false);
            }

            //Secondary attack (currently shooting) with right click
            if (Input.GetButtonDown("Fire2"))
            {
                Shoot(true);
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                Shoot(false);
            }

            if (shooting)
            {
                SubtractAmmo();
            }

            //Allow the player to jump even if they have just fallen off an edge ("fall forgiveness")
            if (!grounded)
            {
                if (fallForgivenessCounter < fallForgiveness && !jumping)
                {
                    fallForgivenessCounter += Time.deltaTime;
                }
                else
                {
                    animator.SetBool("grounded", false);
                }
            }
            else
            {
                canDodge = true;
                fallForgivenessCounter = 0;
                animator.SetBool("grounded", true);
            }

            // Allow the player to cue a jump even if they have not quite reached the ground ("jump earliness")
            if (jumpEarlinessCounter > 0)
                jumpEarlinessCounter -= Time.deltaTime;
            else if (jumpEarlinessCounter < 0)
                jumpEarlinessCounter = 0;

            //Set each animator float, bool, and trigger to it knows which animation to fire
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / baseSpeed);
            animator.SetFloat("velocityY", velocity.y);
            if (!hasInventoryOpen)
            {
                animator.SetInteger("attackDirectionY", (int)Input.GetAxis("VerticalDirection"));
                animator.SetInteger("moveDirection", (int)Input.GetAxis("HorizontalDirection"));
            }
            animator.SetBool("hasChair", GameManager.Instance.CheckForInventoryItem("chair") != null);
            targetVelocity = move * maxSpeed;
        }
        else if (!frozen)
        {
            // Dodge maintenance

            dodgeTimer -= Time.deltaTime;
            if (dodgeDistanceSpent + Time.deltaTime * Mathf.Abs(dodgeVelocity) < dodgeDistance)
            {
                dodgeDistanceSpent += Time.deltaTime * Mathf.Abs(dodgeVelocity);
                targetVelocity = new Vector2(dodgeVelocity, 0);
                velocity.x = dodgeVelocity;
                velocity.y = 0;
            }
            else if (dodgeDistanceSpent < dodgeDistance)
            {
                dodgeDistanceSpent += dodgeDistance - dodgeDistanceSpent;
                targetVelocity = new Vector2((dodgeDistance - dodgeDistanceSpent) * (dodgeVelocity / Mathf.Abs(dodgeVelocity)), 0);
                velocity.x = (dodgeDistance - dodgeDistanceSpent) * (dodgeVelocity / Mathf.Abs(dodgeVelocity));
                velocity.y = 0;
                dodging = false;    // Invincibility may continue, but speed is no longer going to be calculated in this manner.
            }
            else
                dodging = false;    // Invincibility may continue, but speed is no longer going to be calculated in this manner.

            if (dodgeTimer <= 0)
            {
                dodgeTimer = 0;
                dodgeDistanceSpent = 0;
                Physics2D.IgnoreLayerCollision(10, 12, false);
                Physics2D.IgnoreLayerCollision(10, 15, false);
            }

            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / baseSpeed);
            animator.SetFloat("velocityY", velocity.y);
            animator.SetInteger("attackDirectionY", (int)Input.GetAxis("VerticalDirection"));
            animator.SetInteger("moveDirection", (int)Input.GetAxis("HorizontalDirection"));
            animator.SetBool("hasChair", GameManager.Instance.CheckForInventoryItem("chair") != null);
        }
        else
        {
            //If the player is set to frozen, his launch should be zeroed out!
            launch = 0;
        }
    }

    public void SetGroundType()
    {
        //If we want to add variable ground types with different sounds, it can be done here
        switch (groundType)
        {
            case "Grass":
                stepSound = grassSound;
                break;
        }
    }

    public void Freeze(bool freeze)
    {
        //Set all animator params to ensure the player stops running, jumping, etc and simply stands
        if (freeze)
        {
            animator.SetInteger("moveDirection", 0);
            animator.SetBool("grounded", true);
            animator.SetFloat("velocityX", 0f);
            animator.SetFloat("velocityY", 0f);
            GetComponent<PhysicsObject>().targetVelocity = Vector2.zero;
        }

        frozen = freeze;
        shooting = false;
        launch = 0;
    }


    public void GetHurt(int hurtDirection, double[] hitPower)
    {
        //If the player is not frozen (ie talking, spawning, etc), recovering, and pounding, get hurt!
        if (!frozen && !recoveryCounter.recoveringAtAll && !pounding)
        {
            HurtEffect();
            cameraEffects.Shake(100, 1);
            animator.SetTrigger("hurt");
            velocity.y = hurtLaunchPower.y;
            launch = hurtDirection * (hurtLaunchPower.x);
            recoveryCounter.ResetAllCounters();

            for (int i = 0; i < hitPower.Length - 1; i++)
            {
                
                health -= 100*hitPower[i]/(externalStats[1] + 100);
                if (health <= 0)
                {
                    Debug.Log("I should be dead.");
                    StartCoroutine(Die());
                }
            }
            GameManager.Instance.hud.HealthBarHurt();
        }
    }

    private void HurtEffect()
    {
        GameManager.Instance.audioSource.PlayOneShot(hurtSound);
        StartCoroutine(FreezeFrameEffect());
        GameManager.Instance.audioSource.PlayOneShot(hurtSounds[whichHurtSound]);

        if (whichHurtSound >= hurtSounds.Length - 1)
        {
            whichHurtSound = 0;
        }
        else
        {
            whichHurtSound++;
        }
        cameraEffects.Shake(100, 1f);
    }

    public IEnumerator FreezeFrameEffect(float length = .007f)
    {
        Time.timeScale = .1f;
        yield return new WaitForSeconds(length);
        Time.timeScale = 1f;
    }


    public IEnumerator Die()
    {
        if (!frozen)
        {
            dead = true;
            deathParticles.Emit(10);
            GameManager.Instance.audioSource.PlayOneShot(deathSound);
            Hide(true);
            Time.timeScale = .6f;
            yield return new WaitForSeconds(5f);
            GameManager.Instance.hud.animator.SetTrigger("coverScreen");
            GameManager.Instance.hud.loadSceneName = SceneManager.GetActiveScene().name;
            Time.timeScale = 1f;
        }
    }

    public void ResetLevel()
    {
        Freeze(true);
        dead = false;
        health = externalStats[0];
    }

    public void SubtractAmmo()
    {
        if (mana > 0)
        {
            mana -= 1 * Time.deltaTime;
        }
    }

    public void addXp(int xpAmount)
    {
        //Debug.Log("Adding xp = " + xpAmount + ", Next level requires " + (10*System.Math.Pow(level + 1,2)) + " total.");
        xp += xpAmount;
        while (xp != 0 && xp >= 10*System.Math.Pow(level + 1,2))
        {
            level++;
            attributePoints += 5;
            xp -= 10*level^2;
        }
    }

    public void DetermineFerocity()
    {
        double tempModifier = 1.0;
        double ferocRoll = GameManager.Instance.GetRandomDouble(0.0, 1.0);
        // Find the modulus of ferocity
        double ferocityMod;
        for (ferocityMod = externalStats[7]*tempModifier + 100; ferocityMod >= 100; ferocityMod-=100) {}
        if (ferocRoll < ferocityMod/100)
        {
            ferocityTotal = (int) (tempModifier*externalStats[7] + 100)/100 + 1;
        }
        else
        {
            ferocityTotal = (int) (tempModifier*externalStats[7] + 100)/100 + 0;
        }
        ferocityCounter = ferocityTotal;
        //Debug.Log("Determining Ferocity for index " + comboIndex + ": ferocityTotal = " + ferocityTotal);
        animator.SetFloat("animAttackRate", (float)(externalStats[4]/100f));
        animator.SetFloat("animFerocRate", (float)((externalStats[4]/100f)*ferocityTotal));
        animator.SetInteger("ferocityCounter", ferocityTotal);
    }

    public void decrementFerocityCounter()
    {
        if (ferocityCounter > 0)
        {
            ferocityCounter--;
            animator.SetInteger("ferocityCounter", ferocityCounter);
            Debug.Log("Remaining Ferocity for index " + comboIndex + ": ferocityCounter = " + ferocityCounter + ", anim: " + animator.GetInteger("ferocityCounter"));
        }
        //Debug.Log("Decremented ferocity to " + ferocityCounter + " from " + ferocityTotal);
    }

    public void Jump(float jumpMultiplier)
    {
        if (velocity.y != externalStats[10])
        {
            // SeismicWave
            seismicWave.DisplaySprite(new Vector3(transform.position.x, transform.position.y, 0), facingRight);
            earthPrism.DisplaySprite(new Vector3(transform.position.x, transform.position.y, 0), facingRight);

            velocity.y = (float) externalStats[10] * jumpMultiplier; //The jumpMultiplier allows us to use the Jump function to also launch the player from bounce platforms
            fallForgivenessCounter = fallForgiveness;
            //Debug.Log("velocity.y = " + velocity.y);
            PlayJumpSound();
            PlayStepSound();
            JumpEffect();
            jumping = true;
        }
    }

    public void Dodge(float dodgeMultiplier)
    {
        // Dodging costs 20 mana but you can dodge with 10 mana.
        if (Mathf.Abs(velocity.x) < dodgePower && mana >= 10)
        {
            mana -= 20;
            if (mana < 0)
                mana = 0;

            dodgeTimer = 0.4f;
            Physics2D.IgnoreLayerCollision(10, 12, true);
            Physics2D.IgnoreLayerCollision(10, 15, true);
            if (facingRight)
            {
                dodgeVelocity = dodgePower * dodgeMultiplier; //The dodgeMultiplier allows us to use the Dodge function to also launch the player from bounce platforms
            }
            else
            {
                dodgeVelocity = -dodgePower * dodgeMultiplier; //The dodgeMultiplier allows us to use the Dodge function to also launch the player from bounce platforms
            }
            PlayDodgeSound();
            PlayStepSound();
            DodgeEffect();
            dodging = true;

            if (velocity.y < 0)
            {
                velocity.y = 0;
            }
        }
    }

    public void PlayStepSound()
    {
        //Play a step sound at a random pitch between two floats, while also increasing the volume based on the Horizontal axis
        audioSource.pitch = (Random.Range(0.9f, 1.1f));
        audioSource.PlayOneShot(stepSound, Mathf.Abs(Input.GetAxis("Horizontal") / 10));
    }

    public void PlayJumpSound()
    {
        audioSource.pitch = (Random.Range(1f, 1f));
        GameManager.Instance.audioSource.PlayOneShot(jumpSound, .1f);
    }

    public void PlayDodgeSound()
    {
        audioSource.pitch = (Random.Range(1f, 1f));
        GameManager.Instance.audioSource.PlayOneShot(jumpSound, .1f);
    }


    public void JumpEffect()
    {
        jumpParticles.Emit(1);
        audioSource.pitch = (Random.Range(0.6f, 1f));
        audioSource.PlayOneShot(landSound);
    }

    public void DodgeEffect()
    {
        jumpParticles.Emit(1);
        audioSource.pitch = (Random.Range(0.6f, 1f));
        audioSource.PlayOneShot(landSound);
    }

    public void LandEffect()
    {
        if (jumping)
        {
            //Debug.Log(this.name + ", emitting jump particles");
            jumpParticles.Emit(1);
            //Debug.Log(this.name + ", setting audio pitch");
            audioSource.pitch = (Random.Range(0.6f, 1f));
            //Debug.Log(this.name + ", playing land sound");
            audioSource.PlayOneShot(landSound);
            jumping = false;
        }
    }

    public void PunchEffect()
    {
        GameManager.Instance.audioSource.PlayOneShot(punchSound);
        cameraEffects.Shake(100, 1f);
    }

    public void ActivatePound()
    {
        //A series of events needs to occur when the player activates the pound ability
        if (!pounding)
        {
            animator.SetBool("pounded", false);

            if (velocity.y <= 0)
            {
                velocity = new Vector3(velocity.x, hurtLaunchPower.y / 2, 0.0f);
            }

            GameManager.Instance.audioSource.PlayOneShot(poundActivationSounds[Random.Range(0, poundActivationSounds.Length)]);
            pounding = true;
            FreezeFrameEffect(.3f);
        }
    }
    public void PoundEffect()
    {
        //As long as the player as activated the pound in ActivatePound, the following will occur when hitting the ground.
        if (pounding)
        {
            animator.ResetTrigger("attack");
            velocity.y = (float) externalStats[10] / 1.4f;
            animator.SetBool("pounded", true);
            GameManager.Instance.audioSource.PlayOneShot(poundSound);
            cameraEffects.Shake(200, 1f);
            pounding = false;
            //recoveryCounter.counter = 0;
            animator.SetBool("pounded", true);
        }
    }

    public void FlashEffect()
    {
        //Flash the player quickly
        animator.SetTrigger("flash");
    }

    public void Hide(bool hide)
    {
        Freeze(hide);
        foreach (SpriteRenderer sprite in graphicSprites)
            sprite.gameObject.SetActive(!hide);
    }

    public void Shoot(bool equip)
    {
        //Flamethrower ability
        if (GameManager.Instance.CheckForInventoryItem("flamethrower") != null)
        {
            if (equip)
            {
                if (!shooting)
                {
                    animator.SetBool("shooting", true);
                    GameManager.Instance.audioSource.PlayOneShot(equipSound);
                    flameParticlesAudioSource.Play();
                    shooting = true;
                }
            }
            else
            {
                if (shooting)
                {
                    animator.SetBool("shooting", false);
                    flameParticlesAudioSource.Stop();
                    GameManager.Instance.audioSource.PlayOneShot(holsterSound);
                    shooting = false;
                }
            }
        }
    }

    public double[] CalculateDamage(int[] modifiers)
    {
        double[] damage;

        damage = new double[ferocityTotal + 1];

        double physicalDamage;
        double magicalDamage;
        // Combo-specific multipliers
        if (comboIndex == 3)
        {
            physicalDamage = modifiers[5]*externalStats[5] * 2;
            magicalDamage = modifiers[6]*externalStats[6] * 2;
        }
        else
        {
            physicalDamage = modifiers[5]*externalStats[5];
            magicalDamage = modifiers[6]*externalStats[6];
        }

        double singleHitDamage = physicalDamage + magicalDamage;

        //Debug.Log("singleHitDamage = " + singleHitDamage + ", mod[6] = " + modifiers[6]);

        // Apply crit
        double critRoll = GameManager.Instance.GetRandomDouble(0.0, 1.0);
        damage[damage.Length - 1] = 0;                      // Last int describes whether crit or not
        if (critRoll < externalStats[8]*modifiers[8]/100)
        {
            damage[damage.Length - 1] = 1;
            singleHitDamage *= (externalStats[9]*modifiers[9] + 100)/100;
            //Debug.Log("Crit with a roll of " + critRoll + " for a total of " + singleHitDamage);
        }

        // ferocity loop
        int i;
        for (i = 0; i < damage.Length - 1; i++)
        {
            damage[i] = singleHitDamage;
        }

        return damage;
    }

    public void SetUpCheatItems()
    {
        //Allows us to get various items immediately after hitting play, allowing for testing. 
        for (int i = 0; i < cheatItems.Length; i++)
        {
            GameManager.Instance.GiveItem(cheatItems[i]);
        }
    }

    public void RecoverByMelee()
    {
        if (mana < externalStats[2])
            mana += externalStats[2]*.05;
        if (health < externalStats[0])
            health += externalStats[0]*.03;
    }

    public void DisplayLightningFist()
    {
        if (isLightningImbued)
        {
            lightningFist.DisplaySprite(new Vector3(transform.position.x, transform.position.y, 0), facingRight);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("layer = " + collision.gameObject.layer + ", dodging = " + dodging);    
    }

    public void GiveNewReanimated(EnemyBase newReanimated)
    {
        bool foundEmptySlot = false;
        int[] lowestLevel = new int[2];
        for (int i = 0; i < myReanimated.Length; i++)
        {
            if (myReanimated[i] == null)
            {
                foundEmptySlot = true;
                myReanimated[i] = newReanimated;
                newReanimated.reanimatedSlotIndex = i;
                break;
            }
            else
            // Identify an undead of lowest level
            {
                if (myReanimated[i].GetLevel() < lowestLevel[1])
                {
                    lowestLevel[0] = i;
                    lowestLevel[1] = myReanimated[i].GetLevel();
                }
            }
        }
        if (!foundEmptySlot)
        {
            myReanimated[lowestLevel[0]].Delete();
            myReanimated[lowestLevel[0]] = newReanimated;
            newReanimated.reanimatedSlotIndex = lowestLevel[0];
        }
    }

    public void SetComboIndex(int index)
    {
        comboIndex = index;
    }

    public void PrepareIdle()
    {
        nextAction = NextAction.Idle;
        animator.SetBool("attackBool", false);
    }

    public void PrepareNextAction()
    {
        if (nextAction == NextAction.Attack && animator.GetInteger(ferocityCounter) == 0)
        {
            animator.SetBool("attackBool", true);

            Debug.Log("attack = " + animator.GetBool("attack") + ", ferocityCounter = " + animator.GetInteger(ferocityCounter));
        }
    }

    // attackCode: >=0 = horizontal, -1 = up, -2 = down
    public void StartParryTimers()
    {
        if (comboIndex >= 0 && !facingRight)
        {
            parryTimer[0] = parryForgiveness;
            parryTimer[2] = parryForgiveness;
        }
        else if (comboIndex >= 0)
        {
            parryTimer[1] = parryForgiveness;
            parryTimer[3] = parryForgiveness;
        }
        else if (comboIndex == -1)
        {
            parryTimer[0] = parryForgiveness;
            parryTimer[1] = parryForgiveness;
        }
        else if (comboIndex == -2)
        {
            parryTimer[2] = parryForgiveness;
            parryTimer[3] = parryForgiveness;
        }
    }

    public bool GetFacingRight()
    {
        return facingRight;
    }

    public int GetComboIndex()
    {
        return comboIndex;
    }
}