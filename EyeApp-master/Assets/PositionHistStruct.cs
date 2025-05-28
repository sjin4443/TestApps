using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a struct is created for each position that the torch was in at each frame
public struct positionHistStruct
{
    public double time; // to hold time that this struct is created
    public bool active; // whether at the time the torch was active or not
    public float distance; // The distance from the nearest eye

    public positionHistStruct(double time, bool active, float distance)
    {
        this.time = time;
        this.active = active;
        this.distance = distance;
    }
}
