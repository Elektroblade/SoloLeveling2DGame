using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthDisk : MonoBehaviour
{
    public float existenceTimer;
    int direction = 0;
    [SerializeField] private LayerMask layersToBreakDisk;
    private RaycastHit2D target;
    [SerializeField] private int attackType;
    private int targetSide = 1;
    [SerializeField] private int[] statMods;
    private float moveSpeed = 35f;

    // Start is called before the first frame update
    void Start()
    {
        // Right
        if (NewPlayer.Instance.GetFacingRight() && NewPlayer.Instance.GetComboIndex() >= 0)
        {
            direction = 1;
        }
        // Left
        else if (NewPlayer.Instance.GetComboIndex() >= 0)
        {
            direction = -1;
        }
        // Up
        else {
            direction = 0;
        }

        existenceTimer = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (existenceTimer > 0)
        {
            existenceTimer -= Time.deltaTime;
            float raycastY = 0;

            if (direction == 0)
            {
                raycastY = 1;
            }

            target = Physics2D.Raycast(transform.position, new Vector2(direction, raycastY), moveSpeed * Time.deltaTime * 1.5f, layersToBreakDisk);

            if (target.collider != null)
            {
                if (target.collider.transform.parent != null && target.collider.transform.parent.GetComponent<EnemyBase>() != null)
                {
                    //Determine which side the attack is on
                    if (transform.position.x < target.collider.transform.position.x)
                    {
                        targetSide = 1;
                    }
                    else
                    {
                        targetSide = -1;
                    }

                    target.collider.transform.parent.GetComponent<EnemyBase>().GetHurt(targetSide, NewPlayer.Instance.CalculateDamage(statMods, attackType), attackType, 0);
                }

                existenceTimer -= 1f;
            }
            
            transform.position += new Vector3(moveSpeed * direction * Time.deltaTime, moveSpeed * raycastY * Time.deltaTime, 0);
        }
        else if (existenceTimer < 0)
        {
            existenceTimer = 0;

            Destroy(gameObject);
        }
    }
}