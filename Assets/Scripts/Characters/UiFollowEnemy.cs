using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFollowEnemy : MonoBehaviour
{
    public Transform objectToFollow;
    RectTransform rectTransform;
    [SerializeField] public float verticalOffset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        {
            if (objectToFollow != null)
            {
                rectTransform.transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y + verticalOffset, 0);
            }
        }
    }
}
