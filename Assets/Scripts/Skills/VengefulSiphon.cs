using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VengefulSiphon : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [System.NonSerialized] public float existenceTimer;
    [System.NonSerialized] public float existenceTimerMax = 0.5f;
    [System.NonSerialized] public bool deleteNextFrame = false;
    [System.NonSerialized] private Vector2 attackerPosition;
    [System.NonSerialized] private Vector2 victimPosition;
    [System.NonSerialized] private float distanceToTargetX;
    [System.NonSerialized] private float distanceToTargetY;
    [System.NonSerialized] private float totalDistanceToTarget;

    private float maxWidth = 16f;
    private float midWidth = 8f;
    private float minWidth = 2f;
    private float startStretch = 0.6f;
    private float enterMidStretch = 0.4f;
    private float startCompress = 0.2f;

    public void DisplaySprite(Vector2 attackerPosition, Vector2 victimPosition)
    {
        this.attackerPosition = attackerPosition;
        this.victimPosition = victimPosition;
        //Debug.Log("Siphon!");

        distanceToTargetX = victimPosition.x - attackerPosition.x;
        distanceToTargetY = victimPosition.y - attackerPosition.y;
        totalDistanceToTarget = Mathf.Sqrt(Mathf.Pow(distanceToTargetX, 2f) + Mathf.Pow(distanceToTargetY, 2f));

        //Debug.Log("totalDistanceToTarget = " + totalDistanceToTarget);

        float angle = Mathf.Atan(distanceToTargetY/distanceToTargetX) * 57.2958f;

        Vector2 myPosition = new Vector2(attackerPosition.x + distanceToTargetX/2, attackerPosition.y + distanceToTargetY/2);

        transform.localScale = new Vector3(totalDistanceToTarget*5f, 8f, 1);
        transform.position = myPosition;
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);

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

        /*
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
         */

        float scaleX = 1;
        float scaleY = 1;

        if (existenceTimer > startStretch * existenceTimerMax)
        {
            Vector2 myPosition = new Vector2(attackerPosition.x + distanceToTargetX*0.8f + ((existenceTimer-(startStretch*existenceTimerMax))*distanceToTargetX*0.5f)/existenceTimerMax, 
                attackerPosition.y + distanceToTargetY*0.8f + ((existenceTimer-(startStretch*existenceTimerMax))*distanceToTargetY*0.5f)/existenceTimerMax);
            
            scaleX = 1 - (existenceTimer - startStretch * existenceTimerMax)*2.5f/existenceTimerMax;
            scaleY = 8 + 2 * (existenceTimer - startStretch * existenceTimerMax)/existenceTimerMax;

            transform.position = myPosition;

            existenceTimer -= Time.deltaTime;
        }
        else if (existenceTimer > enterMidStretch * existenceTimerMax)
        {
            
            Vector2 myPosition = new Vector2(attackerPosition.x + distanceToTargetX*0.5f + (existenceTimer-(enterMidStretch*existenceTimerMax))*distanceToTargetX*1.5f/existenceTimerMax, 
                attackerPosition.y + distanceToTargetY*0.5f + (existenceTimer-(enterMidStretch*existenceTimerMax))*distanceToTargetY*1.5f/existenceTimerMax);
            
            scaleY = 2 + 3 * (existenceTimer - enterMidStretch * existenceTimerMax)/existenceTimerMax;
            
            transform.position = myPosition;

            existenceTimer -= Time.deltaTime;
        }
        else if (existenceTimer > startCompress * existenceTimerMax)
        {
            Vector2 myPosition = new Vector2(attackerPosition.x + distanceToTargetX*0.2f + (existenceTimer-(startCompress*existenceTimerMax))*distanceToTargetX*1.5f/existenceTimerMax, 
                attackerPosition.y + distanceToTargetY*0.2f + (existenceTimer-(startCompress*existenceTimerMax))*distanceToTargetY*1.5f/existenceTimerMax);
            
            scaleY = 8 + 3 * (startCompress * existenceTimerMax - existenceTimer)/existenceTimerMax;
            
            transform.position = myPosition;

            existenceTimer -= Time.deltaTime;
        }
        else if (existenceTimer > 0)
        {
            Vector2 myPosition = new Vector2(attackerPosition.x + (existenceTimer)*distanceToTargetX*1f/existenceTimerMax, 
                attackerPosition.y + (existenceTimer)*distanceToTargetY*1f/existenceTimerMax);
            
            scaleX = (existenceTimer)*5f/existenceTimerMax;
            scaleY = 16 - 4 * existenceTimer/existenceTimerMax;
            
            transform.position = myPosition;

            existenceTimer -= Time.deltaTime;
        }
        else if (existenceTimer < 0)
        {
            existenceTimer = 0;
            deleteNextFrame = true;
        }

        transform.localScale = new Vector3(scaleX*totalDistanceToTarget*5f, scaleY, 1);
    }
}
