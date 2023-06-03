using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerodynamicHeating : MonoBehaviour
{
    [SerializeField] AttackHit attackHit;
    [SerializeField] public Collider2D collider;
    [SerializeField] Animator animator;

    public void DisplaySprite()
    {
        animator.SetTrigger("AH");
    }
}
