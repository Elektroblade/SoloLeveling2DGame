using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeismicWave : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void DisplaySprite(Vector3 playerPos, bool facingRight)
    {
        float horizontalShift = 0f;
        if (!facingRight)
        {
            horizontalShift *= -1;
        }

        transform.position = new Vector3(playerPos.x + horizontalShift, playerPos.y + 1.2f, 0);

        animator.SetTrigger("SW");
    }
}
