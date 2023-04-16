using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorefieldTall : PhysicsObject
{
    [Header ("Reference")]
    private Rigidbody2D rigidbody2D;
    public EnemyBase enemyBase;
    [SerializeField] private GameObject graphic;

    [Header ("Properties")]
    [SerializeField] private LayerMask layerMask; //What can the Walker actually touch?
    [SerializeField] enum EnemyBehaviourType { Bug, Zombie }; //Bugs will simply patrol. Zombie's will immediately start chasing you forever until you defeat them.
    [SerializeField] EnemyBehaviourType enemyType;
   
    public float attentionRange;
    public float changeDirectionEase = 1; //How slowly should we change directions? A higher number is slower!
    [System.NonSerialized] public float direction = 1;
    private Vector2 distanceFromTarget; //How far is this enemy from the player?
    [System.NonSerialized] public float directionSmooth = 1; //The float value that lerps to the direction integer.
    [SerializeField] private bool followPlayer;
    [SerializeField] private bool flipWhenTurning = false; //Should the graphic flip along localScale.x?
    private RaycastHit2D ground;
    public float hurtLaunchPower = 10; //How much force should be applied to the player when getting hurt?
    [SerializeField] private bool jumping;
    public float jumpPower = 7;
    [System.NonSerialized] public bool jump = false;
    [System.NonSerialized] public float launch = 1; //The float added to x and y moveSpeed. This is set with hurtLaunchPower, and is always brought back to zero
    public float maxSpeed = 7;
    [SerializeField] private float maxSpeedDeviation; //How much should we randomly deviate from maxSpeed? Ensures enemies don't move at exact same speed, thus syncing up.
    [SerializeField] private bool neverStopFollowing = false; //Once the player is seen by an enemy, it will forever follow the player.
    private Vector3 origScale;
    [SerializeField] private Vector2 rayCastSize = new Vector2(1.5f, 1); //The raycast size: (Width, height)
    private Vector2 rayCastSizeOrig;
    [SerializeField] private Vector2 rayCastOffset;
    private RaycastHit2D rightWall;
    private RaycastHit2D leftWall;
    private RaycastHit2D rightLedge;
    private RaycastHit2D leftLedge;

    private float sitStillMultiplier = 1; //If 1, the enemy will move normally. But, if it is set to 0, the enemy will stop completely. 
    [SerializeField] private bool sitStillWhenNotFollowing = false; //Controls the sitStillMultiplier

    [Header("Sounds")]
    public AudioClip jumpSound;
    public AudioClip stepSound;

    [Header("Combat")]
    private Vector2 currentAttackTarget = new Vector2(0, 0);
    [System.NonSerialized] public float[] origAttackCooldown = {1.7f};
    [System.NonSerialized] public float[] attackCooldown = {3f};
    [System.NonSerialized] public bool isAttacking = false;
    private int previousFrameState = 0;
    public EnemyBase targetEnemy;
    
    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        rigidbody2D = GetComponent<Rigidbody2D>();  // Verrry important!
        origScale = transform.localScale;
        rayCastSizeOrig = rayCastSize;
        maxSpeed -= Random.Range(0, maxSpeedDeviation);
        launch = 0;
        if (enemyType == EnemyBehaviourType.Zombie)
        {
            direction = 0;
            directionSmooth = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attentionRange);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("new update speed.x = " + speed.x + ", speed.y = " + speed.y);
        if (attackCooldown[0] > 0)
        {
            attackCooldown[0] -= Time.deltaTime;
        }
        if (attackCooldown[0] < 0)
        {
            attackCooldown[0] = 0;
        }

        if (enemyBase.reanimated && targetEnemy == null)
        {
            FindTarget();
        }
        if (!enemyBase.reanimated || targetEnemy != null)
        {
            Transform target;
            if (enemyBase.reanimated)
            {
                target = targetEnemy.transform;
            }
            else
            {
                target = NewPlayer.Instance.transform;
            }

            if (!isAttacking)
            {
                DoIdle(target);
            }
            else
            {
                //Debug.Log("Time since state 2 = " + (attackCooldown[0] - origAttackCooldown[0] + 45f/60f));
                DoAttack1(target);
            }
        }
        else
        {
            DoIdle(NewPlayer.Instance.transform);
        }
    }

    void FindTarget()
    {
        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, attentionRange);
        foreach (Collider2D collider2D in colliderArray)
        {
            if (collider2D.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            {
                if (!enemy.reanimated)
                    targetEnemy = enemy;
            }
        }
    }

    protected void DoIdle(Transform target)
    {
        Vector2 move = Vector2.zero;

        if (!NewPlayer.Instance.frozen)
        {
            distanceFromTarget = new Vector2 (NewPlayer.Instance.gameObject.transform.position.x - transform.position.x, NewPlayer.Instance.gameObject.transform.position.y - transform.position.y);
            directionSmooth += ((direction * sitStillMultiplier) - directionSmooth) * Time.deltaTime * changeDirectionEase;
            move.x = (1 * directionSmooth) + launch;
            launch += (0 - launch) * Time.deltaTime;
            
            if (move.x < 0)
            {
                transform.localScale = new Vector3(-origScale.x, origScale.y, origScale.z);
            }
            else
            {
                transform.localScale = new Vector3(origScale.x, origScale.y, origScale.z);
            }

            if (!enemyBase.recoveryCounter.recoveringAtAll)
            {
                //Flip the graphic depending on the speed
                if (move.x > 0.01f)
                {
                    if (graphic.transform.localScale.x == 1)
                    {
                        graphic.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                    }
                }
                else if (move.x < -0.01f)
                {
                    if (graphic.transform.localScale.x == -1)
                    {
                        graphic.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                    }
                }

                //Check floor type
                ground = Physics2D.Raycast(transform.position, -Vector2.up);
                Debug.DrawRay(transform.position, -Vector2.up, Color.green);

                //Check if player is within range to follow
                if (enemyType == EnemyBehaviourType.Zombie)
                {
                    if ((Mathf.Abs(distanceFromTarget.x) < attentionRange) && (Mathf.Abs(distanceFromTarget.y) < attentionRange))
                    {
                        followPlayer = true;
                        sitStillMultiplier = 1;

                        if (neverStopFollowing)
                        {
                            attentionRange = 10000000000;
                        }
                    }
                    else
                    {
                        if (sitStillWhenNotFollowing)
                        {
                            sitStillMultiplier = 0;
                        }
                        else
                        {
                            sitStillMultiplier = 1;
                        }
                    }
                }

                if (followPlayer)
                {
                    rayCastSize.y = 200;
                    if (distanceFromTarget.x < 0)
                    {
                        direction = -1;
                    }
                    else
                    {
                        direction = 1;
                    }
                }
                else
                {
                    rayCastSize.y = rayCastSizeOrig.y;
                }

                if (attackCooldown[0] == 0f)
                {
                    previousFrameState = 0;
                    if (distanceFromTarget.x > 0.02f)
                    {
                        if (graphic.transform.localScale.x == 1)
                        {
                            graphic.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                        }
                    }
                    else if (distanceFromTarget.x < -0.02f)
                    {
                        if (graphic.transform.localScale.x == -1)
                        {
                            graphic.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                        }
                    }
                    enemyBase.animator.SetTrigger("slash");
                }

                //Check for walls
                rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, rayCastSize.x, layerMask);
                Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right * rayCastSize.x, Color.yellow);

                if (rightWall.collider != null)
                {
                    if (!followPlayer)
                    {
                        direction = -1;
                    }
                    else if (direction == 1)
                    {
                        Jump();
                    }

                }

                leftWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, rayCastSize.x, layerMask);
                Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left * rayCastSize.x, Color.blue);

                if (leftWall.collider != null)
                {
                    if (!followPlayer)
                    {
                        direction = 1;
                    }
                    else if (direction == -1)
                    {
                        Jump();
                    }
                }

                //Check for ledges. Walker's height check is much higher! They will fall pretty far, but will not fall to death. 
                rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), Vector2.down, rayCastSize.y, layerMask);
                Debug.DrawRay(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), Vector2.down * rayCastSize.y, Color.blue);
                if ((rightLedge.collider == null || rightLedge.collider.gameObject.layer == 14) && direction == 1)
                {
                    direction = -1;
                }

                leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), Vector2.down, rayCastSize.y, layerMask);
                Debug.DrawRay(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), Vector2.down * rayCastSize.y, Color.blue);
                if ((leftLedge.collider == null || leftLedge.collider.gameObject.layer == 14) && direction == -1)
                {
                    direction = 1;
                }
            }
        }

        enemyBase.animator.SetBool("grounded", grounded);
        enemyBase.animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        targetVelocity = move * maxSpeed;
    }

    void DoAttack1(Transform target)
    {
        distanceFromTarget.x = currentAttackTarget.x - transform.position.x;
        distanceFromTarget.y = currentAttackTarget.y - transform.position.y;
        targetVelocity = Vector2.zero;
    }

    public void Jump()
    {
        if (grounded)
        {
            velocity.y = jumpPower;
            PlayJumpSound();
            PlayStepSound();
        }
    }

    public void PlayStepSound()
    {
        enemyBase.audioSource.pitch = (Random.Range(0.6f, 1f));
        enemyBase.audioSource.PlayOneShot(stepSound);
    }

    public void PlayJumpSound()
    {
        enemyBase.audioSource.pitch = (Random.Range(0.6f, 1f));
        enemyBase.audioSource.PlayOneShot(jumpSound);
    }

    public void TrueIsAttacking()
    {
        distanceFromTarget = new Vector2 (NewPlayer.Instance.gameObject.transform.position.x - transform.position.x, NewPlayer.Instance.gameObject.transform.position.y - transform.position.y);

        if (distanceFromTarget.x > 0.02f)
        {
            if (graphic.transform.localScale.x == 1)
            {
                graphic.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (distanceFromTarget.x < -0.02f)
        {
            if (graphic.transform.localScale.x == -1)
            {
                graphic.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
        }
        attackCooldown[0] = origAttackCooldown[0];
        isAttacking = true;
        if (enemyBase.reanimated && targetEnemy != null)
        {
            currentAttackTarget = new Vector2(targetEnemy.transform.position.x, targetEnemy.transform.position.y);
        }
        else if (!enemyBase.reanimated)
        {
            currentAttackTarget = new Vector2(NewPlayer.Instance.transform.position.x, NewPlayer.Instance.transform.position.y);
        }
        else
        {
            isAttacking = false;
            attackCooldown[0] = 0f;
        }
        
    }

    public void NotIsAttacking()
    {
        isAttacking = false;
        //Debug.Log("notIsAttacking()");
    }
}