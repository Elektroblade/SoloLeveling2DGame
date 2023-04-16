using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    [SerializeField] EnemyBase enemyBase;
    public EnemyBase GetEnemyBase()
    {
        return enemyBase;
    }
}
