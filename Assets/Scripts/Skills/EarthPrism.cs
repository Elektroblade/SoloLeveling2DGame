using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthPrism : MonoBehaviour
{
    [SerializeField] GameObject disksRemoved;
    [SerializeField] SpriteRenderer earthPrismTop;
    [SerializeField] SpriteRenderer earthPrismSide;
    [System.NonSerialized] float maxPrismHeight = 1.82f;
    [System.NonSerialized] float prismHeight = 1.82f;
    [System.NonSerialized] int currentReceivedHitCount = 0;
    public float existenceTimer;
    [System.NonSerialized] public float existenceTimerMax = 0.15f;
    [SerializeField] public Collider2D attackCollider;
    [SerializeField] public Collider2D receiveCollider;
    [SerializeField] public RecoveryCounter recoveryCounter;
    [SerializeField] public GameObject earthDisk;

    void Start()
    {
        earthPrismSide.enabled = false;
        earthPrismTop.enabled = false;
    }

    private void Update() {
        if (existenceTimer > existenceTimerMax - 0.03f)
        {
            existenceTimer -= Time.deltaTime;
            disksRemoved.transform.position = new Vector3(transform.position.x,
                transform.position.y + 1.522f - maxPrismHeight * (existenceTimer / existenceTimerMax) + maxPrismHeight - prismHeight, 0);
        }
        else if (existenceTimer > 0)
        {
            attackCollider.enabled = false;
            existenceTimer -= Time.deltaTime;
            disksRemoved.transform.position = new Vector3(transform.position.x, 
                transform.position.y + 1.522f - maxPrismHeight * (existenceTimer / existenceTimerMax) + maxPrismHeight - prismHeight, 0);
        }
        else if (existenceTimer < 0)
        {
            disksRemoved.transform.position = new Vector3(transform.position.x, transform.position.y + 1.522f + maxPrismHeight - prismHeight, 0);
            existenceTimer = 0;

            /*
            Debug.Log("Original positions: " + (disksRemoved.transform.position.y - transform.position.y) + ", " 
                    + (earthPrismTop.transform.position.y - disksRemoved.transform.position.y) + ", " + (maxPrismHeight - prismHeight));
             */
        }
    }

    public void DisplaySprite(Vector3 playerPos, bool facingRight)
    {
        float horizontalShift = 0f;
        if (!facingRight)
        {
            horizontalShift *= -1;
        }

        transform.position = new Vector3(playerPos.x + horizontalShift, playerPos.y + 1.8f, 0);
        disksRemoved.transform.position = new Vector3(transform.position.x, transform.position.y + 1.522f - maxPrismHeight, 0);

        prismHeight = maxPrismHeight;
        currentReceivedHitCount = 0;

        earthPrismSide.enabled = true;
        earthPrismTop.enabled = true;

        receiveCollider.enabled = true;
        attackCollider.enabled = true;
        existenceTimer = existenceTimerMax;
    }

    public void Slice(int abilityLevel, bool facingRight, int comboIndex, int attackType)
    {
        if (recoveryCounter.counter[attackType] > recoveryCounter.recoveryTime)
        {
            prismHeight -= maxPrismHeight / (abilityLevel + 1f);
            currentReceivedHitCount++;

            Vector3 diskPosition = new Vector3(transform.position.x, transform.position.y + 0.8f - maxPrismHeight + prismHeight, 0);
            GameObject thisEarthDisk = Instantiate(earthDisk, diskPosition, transform.rotation);

            //Debug.Log("prismHeight = " + prismHeight + ", 1.523f - maxPrismHeight = " + (1.523f - maxPrismHeight));
            if (currentReceivedHitCount > abilityLevel)
            {
                earthPrismSide.enabled = false;
                earthPrismTop.enabled = false;

                receiveCollider.enabled = false;
            }
            else
            {
                disksRemoved.transform.position = new Vector3(transform.position.x, transform.position.y + prismHeight - 0.289f, 0);
                recoveryCounter.counter[attackType] = 0;

                /*
                Debug.Log("Slice positions: " + (disksRemoved.transform.position.y - transform.position.y) + ", " 
                    + (earthPrismTop.transform.position.y - disksRemoved.transform.position.y) + ", " + (maxPrismHeight - prismHeight) 
                    + (disksRemoved.transform.position.y - maxPrismHeight - transform.position.y));
                 */
            }
        }
    }
}
