using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorParryOrigin : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [System.NonSerialized] public float existenceTimer;
    [System.NonSerialized] public float existenceTimerMax = 60f;
    [SerializeField] public int featureType; // 0 is head, -1 is right hand, 1 is left hand
    [SerializeField] public GameObject homePosition;
    public int rotation; // 0 is up, -1 is right, 2 is down, 1 is left
    [SerializeField] public Collider2D collider;

    public void DisplaySprite(float duration)
    {
        spriteRenderer.transform.localScale = new Vector3(1f, 1f, 1);
        existenceTimerMax = duration;
        existenceTimer = existenceTimerMax;
        Color tmpParryMeshColour = spriteRenderer.color;
        tmpParryMeshColour.a = 1f;
        spriteRenderer.color = tmpParryMeshColour;
        transform.position = homePosition.transform.position;
        transform.rotation = Quaternion.Euler(0,0,featureType*90f + 90f);
        collider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (existenceTimer > existenceTimerMax - 2f && existenceTimer > 0)
        {
            transform.position = homePosition.transform.position;
            existenceTimer -= Time.deltaTime;
        }
        else if (existenceTimer > 0)
        {
            transform.position = homePosition.transform.position;
            transform.rotation = Quaternion.Euler(0,0,rotation * 90f + 90f);
            existenceTimer -= Time.deltaTime;
            if (existenceTimer <= 0)
            {
                existenceTimer = 0;
                collider.enabled = false;
                Color tmpParryMeshColour = spriteRenderer.color;
                tmpParryMeshColour.a = 0f;
                spriteRenderer.color = tmpParryMeshColour;
            }
        }
    }
}
