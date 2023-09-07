using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryCounter : MonoBehaviour
{
    //This script can be attached to any gameObject that has an EnemyBase or Breakable script attached to it.
    //It ensures the EnemyBase or Breakable must recover by a certain length of time before the player can attack it again.

    //[System.NonSerialized] 

    [System.NonSerialized] public float recoveryTime = 0.05f;
    [System.NonSerialized] public float[] counter = new float[9];
    [System.NonSerialized] public bool[] recovering = new bool[9];
    [System.NonSerialized] public bool recoveringAtAll = false;

    // Update is called once per frame
    void Update()
    {
        bool recoveringAtAllTemp = false;
        for (int i = 0; i < counter.Length; i++)
        {
            if(counter[i] <= recoveryTime)
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
