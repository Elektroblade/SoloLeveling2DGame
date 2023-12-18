using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryMesh : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [System.NonSerialized] public float existenceTimer;
    [System.NonSerialized] public float existenceTimerMax = 0.15f;
    [System.NonSerialized] public bool deleteNextFrame = false;

    public void DisplaySprite()
    {
        spriteRenderer.transform.localScale = new Vector3(2, 2f, 1);
        existenceTimer = existenceTimerMax;
        Color tmpParryMeshColour = spriteRenderer.color;
        tmpParryMeshColour.a = 1f;
        spriteRenderer.color = tmpParryMeshColour;
    }

    // Update is called once per frame
    void Update()
    {
        if (deleteNextFrame)
        {
            Destroy(gameObject);
        }

        Color tmpLightningFistColour = spriteRenderer.color;
        if (tmpLightningFistColour.a > 0)
        {
            tmpLightningFistColour.a -= Time.deltaTime/existenceTimerMax;
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
            existenceTimer = 0;
            deleteNextFrame = true;
        }
    }
}