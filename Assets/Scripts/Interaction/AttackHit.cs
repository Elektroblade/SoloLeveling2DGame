using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This script can be placed on any collider that is a trigger. It can hurt enemies or the player, 
so we use it for both player attacks and enemy attacks. 
*/

public class AttackHit : MonoBehaviour
{
    public enum AttacksWhat { EnemyBase, NewPlayer };
    [SerializeField] private AttacksWhat attacksWhat;
    [SerializeField] private int attackType;            // Player melee, player lightningFist, shadow melee, player aerodynamicHeating, player seismicWave,
                                                        // Player earthPrism, Player earthDisk, player vengefulSiphon, player rollingThunder
    [SerializeField] private bool oneHitKill;
    [SerializeField] private float startCollisionDelay; //Some enemy types, like EnemyBombs, should not be able blow up until a set amount of time
    private int targetSide = 1; //Is the attack target on the left or right side of this object?
    [SerializeField] private GameObject parent; //This must be specified manually, as some objects will have a parent that is several layers higher
    [SerializeField] private bool isBomb = false; //Is the object a bomb that blows up when touching the player?
    [SerializeField] private double[] hitPower;    // Effects such as ferocity can make an attack hit multiple times at once
    [SerializeField] private int[] statMods;
    [SerializeField] private GameObject parryMeshPrefab;
    [SerializeField] private GameObject vengefulSiphonPrefab;
    [SerializeField] private LayerMask layersToGetSiphoned;

    // Use this for initialization
    void Start()
    {
        /*If isBomb = true, we want to be sure the collider is disabled when first launched,
        otherwise it will blow up when touching the object shooting it!*/
        if (isBomb) StartCoroutine(TempColliderDisable());
    }

    void OnTriggerStay2D(Collider2D col)
    {
        DoAttackHit(col, true, this.attackType);
    }

    void DoAttackHit(Collider2D col, bool canDoRaycastSyphon, int attackType)
    {
        // Do not damage invincible player
        if (col.transform.parent && col.transform.parent.GetComponent<NewPlayer>() != null && col.transform.parent.GetComponent<NewPlayer>().GetInvincible())
            return;

        int attackerType = -1;
        bool playerStaggeredEnemy = false;

        //Determine which side the target is on in relation to the parent
        if (parent.transform.position.x < col.transform.position.x)
        {
            targetSide = 1;
        }
        else
        {
            targetSide = -1;
        }
        
        //Determine how much damage the attack does

        // Enemy attack Player
        if (parent.GetComponent<EnemyBase>() != null && col.transform.parent && col.transform.parent.GetComponent<NewPlayer>() != null)
        {
            float[] parryTimer = col.transform.parent.GetComponent<NewPlayer>().parryTimer;
            if ((parent.transform.position.x - col.transform.parent.transform.position.x < 0 && ((parent.transform.position.y - col.transform.parent.transform.position.y > 0 && parryTimer[0] <= 0) 
            || (parent.transform.position.y - col.transform.parent.transform.position.y <= 0 && parryTimer[2] <= 0))) 
            || (parent.transform.position.x - col.transform.parent.transform.position.x >= 0 && ((parent.transform.position.y - col.transform.parent.transform.position.y > 0 && parryTimer[1] <= 0) 
            || (parent.transform.position.y - col.transform.parent.transform.position.y <= 0 && parryTimer[3] <= 0))))
            {
                hitPower = parent.GetComponent<EnemyBase>().CalculateDamage();
            }
            else
            {
                //Debug.Log("Parry!");
                playerStaggeredEnemy = true;

                ParryMesh parryMesh;
                
                if (parryMeshPrefab)
                {
                    parryMesh = Instantiate(parryMeshPrefab, gameObject.transform.position, Quaternion.identity).GetComponent<ParryMesh>();
                    if (parryMesh)
                    {
                        parryMesh.DisplaySprite();
                    }
                }
                else
                {
                    Debug.Log("ERROR: Missing Parry Mesh Prefab");
                }

                parent.GetComponent<EnemyBase>().Stagger();
            }
        }

        // Player attack Enemy
        else if (parent.GetComponent<NewPlayer>() != null && col.transform.parent != null && col.transform.parent.GetComponent<EnemyBase>() != null)
        {
            NewPlayer player = parent.GetComponent<NewPlayer>();
            attackerType = 0;
            hitPower = player.CalculateDamage(statMods, attackType);

            if ((attacksWhat == AttacksWhat.EnemyBase && col.transform.parent != null && col.transform.parent.GetComponent<EnemyBase>() != null) 
                && attackType == 0 && hitPower[hitPower.Length - 1] == 1)
            {
                RaycastHit2D[] raycastSiphon = new RaycastHit2D[64];
                col.Raycast(new Vector2(targetSide, 0), raycastSiphon, 10f, layersToGetSiphoned);
                Debug.DrawRay(new Vector2(col.transform.position.x, col.transform.position.y), new Vector2(targetSide*10f, 0), Color.green, 30f);

                // ahaha

                int siphonIndex = 0;
                bool foundCandidate = false;

                while (siphonIndex < raycastSiphon.Length && !foundCandidate)
                {
                    if (raycastSiphon[siphonIndex] != null && raycastSiphon[siphonIndex].collider != null)
                    {
                        // This enemy's hitbox currently being tested
                        RaycastHit2D raycastSiphonHit = raycastSiphon[siphonIndex];
                        if (raycastSiphonHit.collider.transform.parent != null && raycastSiphonHit.collider.transform.parent.GetComponent<EnemyBase>() != null)
                        {
                            RecoveryCounter enemyHitRecoveryCounter = raycastSiphonHit.collider.transform.parent.GetComponent<EnemyBase>().recoveryCounter;

                            // The test:
                            if (enemyHitRecoveryCounter.counter[7] > enemyHitRecoveryCounter.recoveryTime)
                            {
                                EnemyBase siphonedEnemy = raycastSiphonHit.collider.transform.parent.GetComponent<EnemyBase>();
                                foundCandidate = true;
                                DoAttackHit(raycastSiphonHit.collider, false, 7);

                                VengefulSiphon vengefulSiphonMesh;
                
                                if (vengefulSiphonPrefab)
                                {
                                    vengefulSiphonMesh = Instantiate(vengefulSiphonPrefab, gameObject.transform.position, Quaternion.identity).GetComponent<VengefulSiphon>();
                                    if (vengefulSiphonMesh)
                                    {
                                        vengefulSiphonMesh.DisplaySprite(player.siphonOrigin.position, siphonedEnemy.siphonOrigin.position);
                                    }
                                }
                                else
                                {
                                    Debug.Log("ERROR: Missing Vengeful Siphon Prefab");
                                }
                            }
                        }
                    }
                    siphonIndex++;
                }
            }
        }

        // Enemy attack Enemy
        else if (parent.GetComponent<EnemyBase>() != null && col.transform.parent && col.transform.parent.GetComponent<EnemyBase>() != null)
        {
            //Debug.Log("Before enemy-enemy collision. parent is " + parent.GetComponent<EnemyBase>().reanimated + " reanimated and col is " + col.transform.parent.GetComponent<EnemyBase>().reanimated + " reanimated.");
            if ((parent.GetComponent<EnemyBase>().reanimated && !col.transform.parent.GetComponent<EnemyBase>().reanimated)
                || (!parent.GetComponent<EnemyBase>().reanimated && col.transform.parent.GetComponent<EnemyBase>().reanimated))
            {
                //Debug.Log("Calculating enemy-enemy collision. parent is " + parent.GetComponent<EnemyBase>().reanimated + " reanimated and col is " + col.transform.parent.GetComponent<EnemyBase>().reanimated + " reanimated.");
                attackerType = 1;
                hitPower = parent.GetComponent<EnemyBase>().CalculateDamage();
            }
        }

        //Determine what components we're hitting

        //Attack Player
        if (attacksWhat == AttacksWhat.NewPlayer)
        {
            if (col.transform.parent && col.transform.parent.GetComponent<NewPlayer>() != null)
            {
                if (!playerStaggeredEnemy)
                {
                    NewPlayer.Instance.GetHurt(targetSide, hitPower);
                    if (isBomb) transform.parent.GetComponent<EnemyBase>().Die();
                }
            }
            else if (col.transform.parent && col.transform.parent.GetComponent<EnemyBase>() != null)
            {
                if (col.transform.parent.GetComponent<EnemyBase>().reanimated)
                {
                    col.transform.parent.GetComponent<EnemyBase>().GetHurt(targetSide, hitPower, attackType, attackerType);
                }
            }
        }

        // Attack EarthPrism
        else if (attacksWhat == AttacksWhat.EnemyBase && attackType == 0 && col.GetComponent<EarthPrism>() != null)
        {
            col.GetComponent<EarthPrism>().Slice((int) NewPlayer.Instance.attributes[0] / 100, NewPlayer.Instance.GetFacingRight(), 
            NewPlayer.Instance.GetComboIndex(), attackType);
        }

        //Attack Enemies
        else if (attacksWhat == AttacksWhat.EnemyBase && col.transform.parent != null && col.transform.parent.GetComponent<EnemyBase>() != null)
        {
            if (col.transform.parent.GetComponent<GorefieldTall>() != null)
            {
                //Debug.Log("damaging a tall gorefield!, dealing " + hitPower[0] + " per hit to non-reanimated.");
            }
            if (parent.GetComponent<EnemyBase>())
            {
                if (parent.GetComponent<EnemyBase>().reanimated) {}
                    //Debug.Log("Reanimated is dealing " + hitPower[0] + " per hit to non-reanimated.");
            }

            if (!col.transform.parent.GetComponent<EnemyBase>().reanimated)
            {
                //for (int i = 0; i < hitPower.Length; i++)
                //{
                    col.transform.parent.GetComponent<EnemyBase>().GetHurt(targetSide, hitPower, attackType, attackerType);
                //}
            }
        }
        //Attack Breakables
        else if (attacksWhat == AttacksWhat.EnemyBase && col.GetComponent<EnemyBase>() == null && col.GetComponent<Breakable>() != null)
        {
            for (int i = 0; i < hitPower.Length; i++)
            {
                col.GetComponent<Breakable>().GetHurt(hitPower[i], attackType);
            }
        }
        //Blow up bombs if they touch walls
        if (isBomb && col.transform.parent.gameObject.layer == 8)
        {
            transform.parent.GetComponent<EnemyBase>().Die();
        }
    }

    //Temporarily disable this collider to ensure bombs can launch from inside enemies without blowing up!
    IEnumerator TempColliderDisable()
    {
        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
        yield return new WaitForSeconds(startCollisionDelay);
        GetComponent<Collider2D>().enabled = true;
    }
}
