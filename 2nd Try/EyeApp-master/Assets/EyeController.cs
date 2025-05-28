using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/**
 * This is where it gets pretty complicated
 * The main issue is that there is a lag for the eye input but the lag for constriction is different for dilation
 * There may be simpler ways to do this, but not by much without savrificing functionality
 */

public class EyeController : MonoBehaviour
{
    public bool debugMode = false; // debug mode

    public const float constrictResponseLag = 0.2f; // the time (in seconds) of lag on the constriction response
    public const float dilationResponseLag = 0.4f; // the time (in seconds) of lag of the dilation response

    public const float minDilation = 0.7f; // min dilation possible - going much further than this breaks the shader
    public const float maxDilation = 1.3f; // max dilation possible - going much further than this breaks the shader
    public const float maxDistance = 1.285f; // max distance that an eye response to the torch from
    public const float minDistance = 0.8f; // min distance that the eye bothers to measure, any close than this is counted as as close as possible

    public float RAPDScaler = 0.2f; 

    Queue<positionHistStruct> actionQueue = new Queue<positionHistStruct>(); // queue of all actions (all torch positions)
    List<positionHistStruct> dilationQueue = new List<positionHistStruct>(); // must be an list due to necessary actions performed on all items in queue
    positionHistStruct? previousPosition = null; // whether there is a previous torch position (if the user has NOT just put their finger down)

    Material eyeMat;

    public Animator myAnimator;
    enum dilationStatus // what state the dilation is currently in
    {
        DILATING, // if the eye is currently dilating
        CONSTRICTING,  // if the eye is currently constricting
        STATIC // if the eye is currently not actively doing either
    }
    dilationStatus dilationStat;

    public enum litStatus
    {
        RegUNLit, UNRegLit, UNRegUNLit, RegLit // ORDER MATTERS, highest priority for which eye has control on the right
    }
    public litStatus litStat = litStatus.RegUNLit;
    private float timeEnteredUNRegLit; // Time that eye entered UNRegLit state
    private float timeEnteredUnRegUnLit; // Time that eye entered UnRegUnLit

    public float dilationPathPercentage = 0.0f; // how far along dilation curve the current animation is
    private float animationStartPosition = 1.3f; // the dilation value for start of animation
    private float animationEndPosition = 1.3f; // the dilation value for end of animation
    private float movement = 0.0f; // the movement from the last torch positon recorded
    public const float movementThreshold = 0.0f; // the amount of movement required to register movement happened

    [SerializeField]
    private float finalDilation = 1.3f; // the final dilation animation is trying to get to
    [SerializeField]
    private float calculationDilation = 1.3f; // the calculated dilation (this may not be the same as the calculated dilation due to various modifiers)
    
    public bool overridden = false; // whether this eye is currently being overridden or not
    public float overrideDil = 0f; // what this eye is being told to dilate to as it is being overridden



    public float targetDilation = maxDilation; // the used target dilation
    public float overriddenTarget = maxDilation; // the target dilation they are told to use
    public float calculatedTarget = maxDilation; // the target calculated by this eye


    public bool RAPD = false; // whether this eye currently has RAPD

    public GameObject myLight;
    
    public GameObject myEyeObj;

    private void Awake() // some basic setup
    { 
        eyeMat = GetComponent<Renderer>().materials[0];
        eyeMat.SetFloat(eyeMat.shader.GetPropertyName(0), finalDilation);

        targetDilation = maxDilation;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float getDilation() // getter
    {
        return finalDilation;
    }

    float scale_range(float initLow, float initHigh, float newLow, float newHigh, float value) // helper function to scale the range of a value in a range into another range
    {
        float initRange = initHigh - initLow;
        float newRange = newHigh - newLow;

        return (((value - initLow) * newRange) / initRange) + newLow;
    }

    private float getDilationForDistance(float distance) // with a calibrated function, calculate the dilation for the eye at the current distance it is at
    {
        if (distance < minDistance)
        {
            return minDilation;
        }
        else if (distance > maxDistance)
        {
            return maxDilation;
        }
        else
        {
            return (float)((1.73 * Mathf.Pow(distance, 2)) + (-2.56 * distance) + 1.65);
        }
    }

    // Used to help calibrate distances to get function in line above
    //private void distanceTesting(float distance)
    //{
    //    if (Input.GetKey(KeyCode.UpArrow) && dilation < maxDilation)
    //    {
    //        dilation += +0.001f;
    //    }
    //    if (Input.GetKey(KeyCode.DownArrow) && dilation > minDilation)
    //    {
    //        dilation -= 0.001f;
    //    }
    //    eyeMat.SetFloat(eyeMat.shader.GetPropertyName(0), dilation);

    //    if (myEyeObj.transform.position.x > 0 && Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log("Distance: " + distance + " Dilation: " + dilation);
    //    }
    //}


    // Update is called once per frame
    void Update()
    {
        // helps debug as it can be hard to keep track of
        if (Input.GetKeyDown(KeyCode.D) && debugMode)
        {
            print("entering debug");
            myLight.GetComponent<TorchController>().freeze = true;
        }

        float distance;

        if (myLight.activeSelf) // if torch is active
        {
            distance = Vector3.Magnitude(myLight.transform.position - myEyeObj.transform.position);
            if (distance <= maxDistance) {
                if (litStat == litStatus.UNRegLit && (Time.time - timeEnteredUNRegLit) >= constrictResponseLag)
                {
                    litStat = litStatus.RegLit;
                }
                if (!previousPosition.HasValue || previousPosition.Value.distance != distance) { // illuminating this eye, AND it is new illumination OR illumination at a different distance
                    previousPosition = new positionHistStruct(Time.time, true, distance);
                    actionQueue.Enqueue(previousPosition.Value);

                    if (litStat == litStatus.RegUNLit || litStat == litStatus.UNRegUNLit)
                    {
                        litStat = litStatus.UNRegLit; // lit but not registered yet
                        timeEnteredUNRegLit = Time.time; // resets each time if it hasn't been lit for long enough yet
                    }
                }
            } 
            else if (previousPosition.HasValue && distance > maxDistance) // has newly moved off this eye
            {
                previousPosition = null;
                actionQueue.Enqueue(new positionHistStruct(Time.time, false, maxDistance + 1));
                litStat = litStatus.UNRegUNLit; // unlit but not registered yet
                timeEnteredUnRegUnLit = Time.time;
            }
            // fails both conditions when torch is still on eye (distance not changing) OR is off eye and hasn't just moved off 
        } 
        else if (previousPosition.HasValue) // was previously active, now turned off
        {
            litStat = litStatus.UNRegUNLit; // unlit but not registered yet
            timeEnteredUnRegUnLit = Time.time;
            previousPosition = null;
            actionQueue.Enqueue(new positionHistStruct(Time.time, false, maxDistance + 1));
        }
        if (litStat == litStatus.UNRegUNLit && (Time.time - timeEnteredUnRegUnLit) >= dilationResponseLag)
        {
            litStat = litStatus.RegUNLit;
        }
        
        /**
         * Check action queue
         * then check dilation queue if no action queue actions
         */
        bool actionPerformed = false;

        while (actionQueue.Count > 0)
        {
            positionHistStruct top = actionQueue.Peek();
            if ((Time.time - top.time) >= constrictResponseLag) // look at top item in queue and ensure that enough time has passed since it was recorded
            { //if enough time has passed
                actionQueue.Dequeue();

                if (debugMode)
                {
                    //myLight.GetComponent<TorchController>().freeze = true;
                }

                if (getDilationForDistance(top.distance) > calculationDilation || !top.active) // wants to dilate
                {
                    if (dilationQueue.Count > 0)
                    {
                        bool largerFound = false;
                        double timeClosest = 0;
                        for (int i = dilationQueue.Count - 1; i >= 0; i--)
                        {
                            if (top.distance < dilationQueue[i].distance) // top has a smaller dilation
                            {
                                timeClosest = dilationQueue[i].time;
                                dilationQueue.RemoveAt(i);
                                largerFound = true;
                            }
                            else if (largerFound) // come to a smaller dilation, but larger ones had previously been found
                            {
                                top.time = timeClosest;
                                dilationQueue.Add(top);
                            }
                        }
                        if (!largerFound)
                        {
                            dilationQueue.Add(top);
                        }
                    } 
                    else
                    {
                        dilationQueue.Add(top);
                    }
                } 
                else // wants to constrict
                {
                    calculatedTarget = getDilationForDistance(top.distance);
                    dilationQueue.Clear(); // clear/overwrite all pending dilations
                    actionPerformed = true;
                }
            }
            else
            {
                break;
            }
        } 

        while (!actionPerformed && dilationQueue.Count > 0) // If constriction action not performed and there are dilation actions to do
        {
            positionHistStruct topDil = dilationQueue[0];
            if ((Time.time - topDil.time) >= dilationResponseLag) // if enough time has passed to dilate
            {
                dilationQueue.RemoveAt(0);
                calculatedTarget = getDilationForDistance(topDil.distance);
                actionPerformed = true;
            }
            else
            {
                break;
            }
        }

        if (RAPD && actionPerformed)
        {
            calculatedTarget = scale_range(minDilation, maxDilation, minDilation +(minDilation * RAPDScaler), maxDilation, calculatedTarget);
        }


        if (overridden)
        {
            targetDilation = overriddenTarget;
        }
        else
        {
            targetDilation = calculatedTarget;
        }

        // if there is a big enough change to start another animation movement to next position
        // handle what animations are currently running
        // (actionPerformed || overridden) &&
        if (Mathf.Abs(targetDilation - animationEndPosition) > movementThreshold)
        {

            if (overridden && debugMode)
            {
                //Debug.Log("Being overridden");
            }

            animationStartPosition = calculationDilation;
            animationEndPosition = targetDilation;

            myAnimator.StopPlayback();
            dilationPathPercentage = 0;
            if (targetDilation > calculationDilation)
            {
                
                if (dilationStat == dilationStatus.DILATING)
                {
                    myAnimator.SetTrigger("NewDilating");
                } 
                else
                {
                    myAnimator.SetBool("IsDilating", true);
                    myAnimator.SetBool("IsConstricting", false);
                    myAnimator.SetTrigger("StartDilating");
                }
                
                dilationStat = dilationStatus.DILATING;
                
            } 
            else
            {

                if (dilationStat == dilationStatus.CONSTRICTING)
                {
                    myAnimator.SetTrigger("NewConstrict");
                }
                else
                {
                    myAnimator.SetBool("IsDilating", false);
                    myAnimator.SetBool("IsConstricting", true);
                    myAnimator.SetTrigger("StartConstricting");
                }

                dilationStat = dilationStatus.CONSTRICTING;
                
            }
            movement = Mathf.Abs(animationEndPosition - animationStartPosition);
        }


        // set the current dilation to be whatever was calcualted
        if (dilationStat == dilationStatus.CONSTRICTING)
        {
            calculationDilation = animationStartPosition - (movement * dilationPathPercentage);
        }
        else if (dilationStat == dilationStatus.DILATING)
        {
            calculationDilation = animationStartPosition + (movement * dilationPathPercentage);
        }

        finalDilation = calculationDilation;

        eyeMat.SetFloat(eyeMat.shader.GetPropertyName(0), finalDilation);
    }

}
