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
    [SerializeField] private int attackType;            // Player melee, player lightningFist, shadow melee, player aerodynamicHeating
    [SerializeField] private bool oneHitKill;
    [SerializeField] private float startCollisionDelay; //Some enemy types, like EnemyBombs, should not be able blow up until a set amount of time
    private int targetSide = 1; //Is the attack target on the left or right side of this object?
    [SerializeField] private GameObject parent; //This must be specified manually, as some objects will have a parent that is several layers higher
    [SerializeField] private bool isBomb = false; //Is the object a bomb that blows up when touching the player?
    [SerializeField] private double[] hitPower;    // Effects such as ferocity can make an attack hit multiple times at once
    [SerializeField] private int[] statMods;

    // Use this for initialization
    void Start()
    {
        /*If isBomb = true, we want to be sure the collider is disabled when first launched,
        otherwise it will blow up when touching the object shooting it!*/
        if (isBomb) StartCoroutine(TempColliderDisable());
    }

    void OnTriggerStay2D(Collider2D col)
    {
        int attackerType = -1;
        bool playerStaggeredEnemy = false;

        //Determine which side the attack is on
        if (parent.transform.position.x < col.transform.position.x)
        {
            targetSide = 1;
        }
        else
        {
            targetSide = -1;
        }
        
        //Determine how much damage the attack does
        if (parent.GetComponent<EnemyBase>() != null && col.GetComponent<NewPlayer>() != null)
        {
            float[] parryTimer = col.GetComponent<NewPlayer>().parryTimer;
            if ((parent.transform.position.x - col.transform.position.x < 0 && ((parent.transform.position.y - col.transform.position.y > 0 && parryTimer[0] <= 0) 
            || (parent.transform.position.y - col.transform.position.y <= 0 && parryTimer[2] <= 0))) 
            || (parent.transform.position.x - col.transform.position.x >= 0 && ((parent.transform.position.y - col.transform.position.y > 0 && parryTimer[1] <= 0) 
            || (parent.transform.position.y - col.transform.position.y <= 0 && parryTimer[3] <= 0))))
            {
                hitPower = parent.GetComponent<EnemyBase>().CalculateDamage();
            }
            else
            {
                Debug.Log("Parry!");
                playerStaggeredEnemy = true;
                parent.GetComponent<EnemyBase>().Stagger();
            }
        }
        else if (parent.GetComponent<NewPlayer>() != null && col.GetComponent<EnemyBase>() != null)
        {
            attackerType = 0;
            hitPower = parent.GetComponent<NewPlayer>().CalculateDamage(statMods);
        }
        else if (parent.GetComponent<EnemyBase>() != null && col.GetComponent<EnemyBase>() != null)
        {
            //Debug.Log("Before enemy-enemy collision. parent is " + parent.GetComponent<EnemyBase>().reanimated + " reanimated and col is " + col.GetComponent<EnemyBase>().reanimated + " reanimated.");
            if ((parent.GetComponent<EnemyBase>().reanimated && !col.GetComponent<EnemyBase>().reanimated)
                || (!parent.GetComponent<EnemyBase>().reanimated && col.GetComponent<EnemyBase>().reanimated))
            {
                //Debug.Log("Calculating enemy-enemy collision. parent is " + parent.GetComponent<EnemyBase>().reanimated + " reanimated and col is " + col.GetComponent<EnemyBase>().reanimated + " reanimated.");
                attackerType = 1;
                hitPower = parent.GetComponent<EnemyBase>().CalculateDamage();
            }
        }

        //Determine what components we're hitting

        //Attack Player
        if (attacksWhat == AttacksWhat.NewPlayer)
        {
            if (col.GetComponent<NewPlayer>() != null)
            {
                if (!playerStaggeredEnemy)
                {
                    NewPlayer.Instance.GetHurt(targetSide, hitPower);
                    if (isBomb) transform.parent.GetComponent<EnemyBase>().Die();
                }
            }
            else if (col.GetComponent<EnemyBase>() != null)
            {
                if (col.GetComponent<EnemyBase>().reanimated)
                {
                    col.GetComponent<EnemyBase>().GetHurt(targetSide, hitPower, attackType, attackerType);
                }
            }
        }
        //Attack Enemies
        else if (attacksWhat == AttacksWhat.EnemyBase && col.GetComponent<EnemyBase>() != null)
        {
            if (col.GetComponent<GorefieldTall>() != null)
            {
                Debug.Log("damaging a tall gorefield!, dealing " + hitPower[0] + " per hit to non-reanimated.");
            }
            if (parent.GetComponent<EnemyBase>())
            {
                if (parent.GetComponent<EnemyBase>().reanimated)
                    Debug.Log("Reanimated is dealing " + hitPower[0] + " per hit to non-reanimated.");
            }

            if (!col.GetComponent<EnemyBase>().reanimated)
            {
                //for (int i = 0; i < hitPower.Length; i++)
                //{
                    col.GetComponent<EnemyBase>().GetHurt(targetSide, hitPower, attackType, attackerType);
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
        if (isBomb && col.gameObject.layer == 8)
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
