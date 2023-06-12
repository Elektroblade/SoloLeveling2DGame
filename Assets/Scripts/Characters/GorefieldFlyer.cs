using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*The functionality for flying enemies*/

public class GorefieldFlyer : MonoBehaviour
{

    [Header ("References")]
    private Rigidbody2D rigidbody2D;
    [System.NonSerialized] public EnemyBase enemyBase;
    private Transform lookAtTarget; //If I'm a bomb, I will point to a transform, like the player
    private Vector3 origLocalScale;
    [SerializeField] private GameObject graphic;
    [SerializeField] private Component[] graphicSprites;

    [Header ("Ground Avoidance")]
    [SerializeField] private float rayCastWidth = 5;
    [SerializeField] private float rayCastOffsetY = 1;
    [SerializeField] private LayerMask layerMask; //What will I be looking to avoid?
    private RaycastHit2D rayCastHit;

    [Header ("Flight")]
    [SerializeField] private bool avoidGround; //Should I steer away from the ground?
    private Vector3 distanceFromTarget;
    [SerializeField] private float maxSpeedDeviation;
    [SerializeField] private float easing = 1; //How intense should we ease when changing speed? The higher the number, the less air control!
    private float bombCounter = 0;
    [SerializeField] private float bombCounterMax = 5; //How many seconds before shooting another bomb?
    public float attentionRange; //How far can I see?
    public float lifeSpan; //Keep at zero if you don't want to explode after a certain period of time.
    [System.NonSerialized] public float lifeSpanCounter;
    private bool sawPlayer = false; //Have I seen the player?
    [SerializeField] private float speedMultiplier; 
    [System.NonSerialized] public Vector3 speed;
    [System.NonSerialized] public Vector3 speedEased;
    [SerializeField] private bool shootsBomb;
    private Vector2 origTargetOffset;
    [SerializeField] private Vector2 targetOffset = new Vector2(0, 0);

    [Header ("Combat")]
    private Vector2 currentAttackTarget = new Vector2(0, 0);
    [System.NonSerialized] public float[] origAttackCooldown = {3.7f};
    [System.NonSerialized] public float[] attackCooldown = {4f};
    [System.NonSerialized] public bool isAttacking = false;
    private int previousFrameState = 0;
    [System.NonSerialized] public EnemyBase targetEnemy;

    // Use this for initialization
    void Start()
    {
        origLocalScale = transform.localScale;
        origTargetOffset = targetOffset;
        enemyBase = GetComponent<EnemyBase>();
        rigidbody2D = GetComponent<Rigidbody2D>();  // Verrry important!

        if (enemyBase.isBomb)
        {
            lookAtTarget = NewPlayer.Instance.gameObject.transform;
        }

        speedMultiplier += Random.Range(-maxSpeedDeviation, maxSpeedDeviation);

        //Find all sprites so we can hide them when the player dies.
        graphicSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position indicating the attentionRange
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attentionRange);
        if (enemyBase.reanimated && targetEnemy != null)
            Gizmos.DrawWireSphere(targetEnemy.transform.position, 0.5f);
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
                doAttack1(target);
            }
        }
        else
        {
            DoIdle(NewPlayer.Instance.transform);
        }
        
      
        if (lifeSpan != 0)
        {
            if (lifeSpanCounter < lifeSpan)
            {
                lifeSpanCounter += Time.deltaTime;
            }
            else
            {
                enemyBase.Die();
            }
        }
    }

    void FindTarget()
    {
        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, attentionRange);
        foreach (Collider2D collider2D in colliderArray)
        {
            if (collider2D.transform.parent != null && collider2D.transform.parent.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            {
                if (!enemy.reanimated)
                    targetEnemy = enemy;
            }
        }
    }

    void DoIdle(Transform target)
    {
        distanceFromTarget.x = (target.position.x + targetOffset.x) - transform.position.x;
        distanceFromTarget.y = (target.position.y + targetOffset.y) - transform.position.y;
        speedEased += (speed - speedEased) * Time.deltaTime * easing;
        transform.position += speedEased * Time.deltaTime;
        //if (enemyBase.reanimated)
            //Debug.Log("reanimated idle, dFT.x = " + distanceFromTarget.x + ", dFT.y = " + distanceFromTarget.y 
            //    + ", t.p.x = " + target.position.x + ", t.p.y = " + target.position.y);

        if (distanceFromTarget.x - targetOffset.x > 0.02f)
        {
            graphic.transform.localScale = new Vector3(origLocalScale.x, transform.localScale.y, transform.localScale.z);
            targetOffset = new Vector2(-origTargetOffset.x, targetOffset.y);
        }
        else if (distanceFromTarget.x - targetOffset.y < -0.02f)
        {
            graphic.transform.localScale = new Vector3(-origLocalScale.x, transform.localScale.y, transform.localScale.z);
            targetOffset = new Vector2(origTargetOffset.x, targetOffset.y);
        }

        //Debug.Log("Updayte! distanceFromTarget.x = " + distanceFromTarget.x + ", distanceFromTarget.y = " + distanceFromTarget.y);

        if (Mathf.Abs(distanceFromTarget.x) <= attentionRange && Mathf.Abs(distanceFromTarget.y) <= attentionRange || lookAtTarget != null)
        {
            sawPlayer = true;
            speed.x = (Mathf.Abs(distanceFromTarget.x) / distanceFromTarget.x) * speedMultiplier;
            speed.y = (Mathf.Abs(distanceFromTarget.y) / distanceFromTarget.y) * speedMultiplier;

            if (!NewPlayer.Instance.frozen || enemyBase.reanimated)
            {
                if (attackCooldown[0] == 0f && Mathf.Abs(distanceFromTarget.x) - 0.3f < 0 && Mathf.Abs(distanceFromTarget.x) + 0.3f > 0
                    && Mathf.Abs(distanceFromTarget.y) - 0.3f < 0 && Mathf.Abs(distanceFromTarget.y) + 0.3f > 0
                    && (!enemyBase.reanimated || target != NewPlayer.Instance.transform))
                {
                    previousFrameState = 0;
                    // For testing purposes
                    //transform.position = new Vector3(target.position.x + targetOffset.x, target.position.y + targetOffset.y, 0);
                    enemyBase.determineFerocity();
                    enemyBase.animator.SetTrigger("bite");
                }
            }
            else
            {
                speedEased = Vector3.zero;
            }
        }
        else
        {
            speed = Vector2.zero;
            if (transform.position.y > (target.position.y + targetOffset.y) && sawPlayer)
            {
                speed = new Vector2(0f, -.05f);
            }

        }

        // Check for walls and ground
        if (avoidGround)
        {
            rayCastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffsetY), Vector2.right, rayCastWidth, layerMask);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffsetY), Vector2.right * rayCastWidth, Color.yellow);

            if (rayCastHit.collider != null)
            {
                speed.x = -(Mathf.Abs(speed.x));
            }

            rayCastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffsetY), Vector2.left, rayCastWidth, layerMask);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffsetY), Vector2.left * rayCastWidth, Color.blue);

            if (rayCastHit.collider != null)
            {
                speed.x = Mathf.Abs(speed.x);

            }

            rayCastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffsetY), Vector2.down, rayCastWidth, layerMask);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffsetY), Vector2.down * rayCastWidth, Color.red);

            if (rayCastHit.collider != null)
            {
                speed.y = Mathf.Abs(speed.x);
                if (speed.y == 0f)
                {
                    speed.y += 0.01f;
                }
            }
        }

        if (lookAtTarget != null)
        {
            LookAt2D();
        }
    }

    void doAttack1(Transform target)
    {
        distanceFromTarget.x = currentAttackTarget.x - transform.position.x;
        distanceFromTarget.y = currentAttackTarget.y - transform.position.y;
        Vector3 displacementModifer = Vector3.zero;
        if (attackCooldown[0] > origAttackCooldown[0] - 5f/60f)
        {
            speedEased = Vector2.zero;
            previousFrameState = 1;
            // Freeze
        }
        else if (attackCooldown[0] > origAttackCooldown[0] - 30f/60f)
        {
            float intervalWithinThisState;
            if (previousFrameState == 1)
            {
                intervalWithinThisState = attackCooldown[0] - origAttackCooldown[0] + 5f/60f;
            }
            else
            {
                intervalWithinThisState = Time.deltaTime;
            }

            speed.x = -targetOffset.x * 5.5f;
            speed.y = -targetOffset.y * 6.5f;
            speedEased += (speed - speedEased) * intervalWithinThisState * easing;
            previousFrameState = 2;
            //Debug.Log("end of update speed.x = " + speed.x + ", speed.y = " + speed.y);
        }
        else if (attackCooldown[0] > origAttackCooldown[0] - 55f/60f)
        {
            float intervalWithinThisState;
            if (previousFrameState == 2)
            {
                intervalWithinThisState = attackCooldown[0] - origAttackCooldown[0] + 30f/60f;

                speed.x = -targetOffset.x * 5.5f;
                speed.y = -targetOffset.y * 6.5f;
                speedEased += (speed - speedEased) * (Time.deltaTime - intervalWithinThisState) * easing;
                displacementModifer = speedEased * (Time.deltaTime - intervalWithinThisState);

                //Debug.Log("deltaTime = " + Time.deltaTime + ", intInDisSta = " + intervalWithinThisState + ", T-i = " + (Time.deltaTime - intervalWithinThisState));
            }
            else
            {
                intervalWithinThisState = Time.deltaTime;
            }

            speed.x = -targetOffset.x * 5.0f;
            speed.y = -targetOffset.y * -7.5f;
            speedEased += (speed - speedEased) * intervalWithinThisState * easing;
            previousFrameState = 3;
            //Debug.Log("end of update speed.x = " + speed.x + ", speed.y = " + speed.y);
        }
        else
        {
            speed.x = targetOffset.x * 4f;
            speed.y = 0.1f;
            speedEased /= 600*Time.deltaTime;
            //Debug.Log("end of update speed.x = " + speed.x + ", speed.y = " + speed.y);
        }
        transform.position += speedEased * Time.deltaTime + displacementModifer;
        //Debug.Log("Do attack!");
    }

    void LookAt2D()
    {
        float angle = Mathf.Atan2(speedEased.y, speedEased.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }

    public void TrueIsAttacking()
    {
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
