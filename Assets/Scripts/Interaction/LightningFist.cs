using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningFist : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AttackHit attackHit;
    public float existenceTimer;
    [SerializeField] public Collider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        Color tmpLightningFistColour = spriteRenderer.color;
        tmpLightningFistColour.a = 0f;
        spriteRenderer.color = tmpLightningFistColour;
    }

    // Update is called once per frame
    void Update()
    {
        Color tmpLightningFistColour = spriteRenderer.color;
        if (tmpLightningFistColour.a > 0)
        {
            tmpLightningFistColour.a -= Time.deltaTime;
            if (tmpLightningFistColour.a < 0)
            {
                tmpLightningFistColour.a = 0;
            }
            spriteRenderer.color = tmpLightningFistColour;
        }

        if (existenceTimer > 0)
        {
            existenceTimer -= Time.deltaTime;
        }
        else if (existenceTimer < 0)
        {
            collider.enabled = false;
            existenceTimer = 0;
        }
    }

    public void DisplaySprite(Vector3 playerPos, bool facingRight)
    {
        float horizontalShift = 10.5f;
        if (!facingRight)
        {
            horizontalShift *= -1;
            spriteRenderer.transform.localScale = new Vector3(2, 2f, 1);
        }
        else
        {
            spriteRenderer.transform.localScale = new Vector3(-2, 2f, 1);
        }
        spriteRenderer.transform.position = new Vector3(playerPos.x + horizontalShift, playerPos.y + 1.57f, 0);
        Color tmp = spriteRenderer.color;
        tmp.a = 1f;
        spriteRenderer.color = tmp;
        collider.enabled = true;
        existenceTimer = 0.03f;
    }
}
