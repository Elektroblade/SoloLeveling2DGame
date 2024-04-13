using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UIItem
{
    public void HighlightMe();
    public void UnhighlightMe();
    public void SelectMe();
    public void UnselectMe();
    public void DecrementNumerator();
    public bool GetHighlighted();
    public bool GetSelected();
    public string GetDescription();
    public bool ItemIsNull();
    public string GetItemId();
    public Transform GetTransform();
    public float GetNumerator();
    public GameObject GetGameObject();
}
