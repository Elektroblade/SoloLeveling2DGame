using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingThunderTrailOrigin : MonoBehaviour
{
    [System.NonSerialized] public GameObject objectToFollow;

    public void SetObjectToFollow(int objectToFollowInt, float scaleX)
    {
        this.transform.localScale = new Vector3(scaleX, 1, 1);

        NewPlayer player = NewPlayer.Instance;
        if (objectToFollowInt == 0)
            objectToFollow = player.headTopLoc.gameObject;
        if (objectToFollowInt == 1)
            objectToFollow = player.frontFootLoc.gameObject;
        if (objectToFollowInt == 2)
            objectToFollow = player.siphonOrigin.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (objectToFollow != null)
        {
            this.transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, 0);
        }
    }
}
