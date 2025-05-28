using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class used to help calibrate the light and pupil response with distances
public class CircleTester : MonoBehaviour
{
    public GameObject myLight;
    public GameObject me;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        me.transform.position = myLight.transform.position;
    }
}
