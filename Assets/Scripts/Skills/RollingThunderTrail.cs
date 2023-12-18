using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingThunderTrail : MonoBehaviour
{
    public void DestroySelf()
    {
        Transform myParent = gameObject.transform.parent;
        Destroy(myParent.gameObject);
    }
}
