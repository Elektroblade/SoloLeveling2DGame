using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryCounter : MonoBehaviour
{
    //This script can be attached to any gameObject that has an EnemyBase or Breakable script attached to it.
    //It ensures the EnemyBase or Breakable must recover by a certain length of time before the player can attack it again.

    //[System.NonSerialized] 

    [System.NonSerialized] public float recoveryTime0 = 0.05f;  // Instant damage
    [System.NonSerialized] public float recoveryTime1 = 1f;   // DoT
    [System.NonSerialized] public float[] counter = new float[10];      // 0 Player melee, 1 player lightningFist, 2 shadow melee, 3 player aerodynamicHeating, 4 player seismicWave,
                                                        // 5 Player earthPrism, 6 Player earthDisk, 7 player vengefulSiphon, 8 player rollingThunder, 9 player doctorRegen
    [System.NonSerialized] public bool[] recovering = new bool[10];
    [System.NonSerialized] public bool recoveringAtAll = false;

    // Update is called once per frame
    void Update()
    {
        bool recoveringAtAllTemp = false;
        for (int i = 0; i < counter.Length; i++)
        {
            if (i == 9)
            {
                if(counter[i] <= recoveryTime1)
                {
                    counter[i] += Time.deltaTime;
                    recovering[i] = true;
                    recoveringAtAllTemp = true; 
                }
                else
                {
                    recovering[i] = false;
                }
            }
            else
            {
                if(counter[i] <= recoveryTime0)
                {
                    counter[i] += Time.deltaTime;
                    recovering[i] = true;
                    recoveringAtAllTemp = true; 
                }
                else
                {
                    recovering[i] = false;
                }
            }
        }
        recoveringAtAll = recoveringAtAllTemp;
    }

    public void ResetAllCounters()
    {
        for (int i = 0; i < counter.Length; i++)
        {
            counter[i] = 0;
        }
    }
}
