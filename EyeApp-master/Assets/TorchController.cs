using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Simple class that was mainly used for calibration of the pupil response to the light with the distances
public class TorchController : MonoBehaviour
{
    public const float touchOffset = 0.75f;
   
    public UnityEngine.Experimental.Rendering.Universal.Light2D myLight;

    public bool freeze = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    bool s = false;

    // Update is called once per frame
    void Update()
    {

        //Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //myLight.transform.position = new Vector3(touchPos.x, touchPos.y + 0.75f, 1);
        //return;

        if (!s)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            myLight.transform.position = new Vector3(touchPos.x, touchPos.y + touchOffset, 1);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            s = false;
        }
        if (Input.GetKey(KeyCode.M) || freeze)
        {
            s = true;
        }


        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    myLight.transform.position = myLight.transform.position + new Vector3(-0.001f, 0, 0);
        //}
        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    myLight.transform.position = myLight.transform.position + new Vector3(0.001f, 0, 0);
        //}
    }
}
