using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface Skill : IComparable
{
    public string ToString();

    public int CompareTo(object incomingObject);
}
