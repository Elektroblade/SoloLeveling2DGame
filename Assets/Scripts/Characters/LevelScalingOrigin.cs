using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScalingOrigin : MonoBehaviour
{
    [SerializeField] double horizontalLevelMultiplier;

    public double GetHorizontalLevelMultiplier()
    {
        return horizontalLevelMultiplier;
    }
}
