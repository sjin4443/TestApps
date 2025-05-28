using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameController : MonoBehaviour
{

    public static GameController instance;
    // holds left and right eye - FROM THE EYES PERSPECTIVE AS OPTHALMOLOGISTS ALWAYS REFER TO RIGHT EYE AS THE PATIENTS RIGHT EYE ETC
    public GameObject leftEye; 
    public GameObject rightEye;

    public GameObject torch;

    EyeController leftController; // the controllers for the eyes
    EyeController rightController;

    public float constrictionSpeedMod = 1; // modifiers to tune constirction and dilation speeds
    public float dilationSpeedMod = 1;

    // The buttons to handle the RAPD
    // KNOWN BUG - Can currently turn on BOTH right and left RAPD at same time and this kinda breaks slider appearing and dissappearing logic - it will likely be an easy fix 
    public GameObject applyRandomButton;
    public GameObject revealButton;
    public GameObject leftRAPD;
    public GameObject rightRAPD;
    public GameObject slider;


    public GameObject rightButton;
    public GameObject leftButton;
    public GameObject normalButton;

    public GameObject cam;
    public GameObject endCanv;

    public GameObject qNumText;

    public GameObject menuButton;

    private int[] sidesTrain =         {1,    2,    1,    1,    2,    1,    0,  1,    2,    2,    2,    0,  1,    1,    2,    2,    0,  0,  2,    1,    0,  0,  1,    2};
    private float[] intensitiesTrain = {0.3f, 0.1f, 0.1f, 0.1f, 0.2f, 0.3f, 0f, 0.2f, 0.1f, 0.2f, 0.2f, 0f, 0.1f, 0.2f, 0.3f, 0.3f, 0f, 0f, 0.3f, 0.3f, 0f, 0f, 0.2f, 0.1f};

    private int[] sidesTest =         { 1,    0,    0,    1,    2,    2,    2,    1,    2,    0,    2,    0,  1,    1,    1,    1,    0,  1,    2,    2,    0,  2,    1,    2};
    private float[] intensitiesTest = { 0.2f, 0.1f, 0.1f, 0.1f, 0.1f, 0.3f, 0.2f, 0.3f, 0.3f, 0.2f, 0.3f, 0f, 0.1f, 0.2f, 0.2f, 0.3f, 0f, 0.3f, 0.1f, 0.1f, 0f, 0.2f, 0.1f, 0.2f };

    private int numQuestions = 24;

    private int[] sides;
    private float[] intensities;

    private int testIndex = 0;

    // The standard, and min/max values of the RAPD slider
    float RAPDintensity = 0.2f;
    const float RAPDMax = 0.4f;
    const float RAPDMin = 0.05f;

    private bool practiceTestMode = false;

    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_EDITOR
        Application.targetFrameRate = 60; //  I am not convinced this actually works, either way, animation should now be framerate invariant
        #endif

        // Initial setup
        instance = this;
        leftController = leftEye.GetComponent<EyeController>();
        rightController = rightEye.GetComponent<EyeController>();

        setSpeeds(leftEye);
        setSpeeds(rightEye);

        slider.GetComponent<Slider>().maxValue = RAPDMax;
        slider.GetComponent<Slider>().minValue = RAPDMin;

        practiceTestMode = false;
        correct = 0;

        if (ModeTracker.mode == 0) // init test
        {
            PlayerPrefs.SetInt("useCount", 0);
            PlayerPrefs.Save();
            sides = sidesTrain;
            intensities = intensitiesTrain;
            setupTrain(false);
            numQuestions = 24;
            testIndex = 0;
        }
        else if (ModeTracker.mode == 1) // Final Test
        {
            sides = sidesTest;
            intensities = intensitiesTest;
            setupTrain(false);
            numQuestions = 24;
            testIndex = 0;
        }
        else if (ModeTracker.mode == 2) // Open Practice
        {
            if (!PlayerPrefs.HasKey("useCount"))
            {
                PlayerPrefs.SetInt("useCount", 1);
                PlayerPrefs.Save();
            }
            else
            {
                PlayerPrefs.SetInt("useCount", PlayerPrefs.GetInt("useCount") + 1);
                PlayerPrefs.Save();
            }
        }
        else // Practice Test
        {
            numQuestions = 8;
            setupTrain(true);
            testIndex = 0;
            applyRandomTest(testIndex);
            practiceTestMode = true;
        }
    }

    private void setupTrain(bool practice)
    {
        applyRandomButton.SetActive(false);
        rightRAPD.SetActive(false);
        leftRAPD.SetActive(false);

        rightButton.SetActive(true);
        leftButton.SetActive(true);
        normalButton.SetActive(true);
        qNumText.SetActive(true);

        if (!practice)
        {
            applyIndex(testIndex);
        }
    }

    private void applyRandomTest(int idx)
    {
        applyRandom(true);
        qNumText.GetComponent<Text>().text = (idx + 1) + " of " + numQuestions;
    }

    private void applyIndex(int idx)
    {
        int side = sides[idx];
        if (side == 1)
        {
            leftController.RAPD = true;
            rightController.RAPD = false;
        }
        else if (side == 2)
        {
            leftController.RAPD = false;
            rightController.RAPD = true;
        }
        else
        {
            leftController.RAPD = false;
            rightController.RAPD = false;
        }
        leftController.RAPDScaler = intensities[idx];
        rightController.RAPDScaler = intensities[idx];

        qNumText.GetComponent<Text>().text = (idx + 1) + " of 24";
    }

    private int correct = 0;

    public void makeChoice(int choice)
    {
        if (practiceTestMode)
        {
            if ((leftRAPD.GetComponent<Toggle>().isOn && choice == 1) || (rightRAPD.GetComponent<Toggle>().isOn && choice == 2) || (choice == 0 && !rightRAPD.GetComponent<Toggle>().isOn && !leftRAPD.GetComponent<Toggle>().isOn))
            {
                correct++;
            }
            testIndex++;

            if (testIndex == numQuestions)
            {
                leftButton.SetActive(false);
                rightButton.SetActive(false);
                normalButton.SetActive(false);
                qNumText.SetActive(false);

                endCanv.SetActive(true);
                menuButton.SetActive(false);
                endCanv.GetComponentInChildren<Text>().text = "Score: " + correct + " of " + numQuestions;
            }
            else
            {
                leftEye.SetActive(false);
                rightEye.SetActive(false);
                leftButton.SetActive(false);
                rightButton.SetActive(false);
                normalButton.SetActive(false);
                qNumText.SetActive(false);
                StartCoroutine(delayCo());
                applyRandomTest(testIndex);
            }

        } 
        else
        {
            makeTestChoice(choice);
        }
    }

    // 1 = Left
    // 2 = Right
    // 0 = Normal
    private void makeTestChoice(int choice)
    {
        if (choice == sides[testIndex])
        {
            correct++;
        }
        testIndex++;

        if (testIndex == numQuestions) {

            leftButton.SetActive(false);
            rightButton.SetActive(false);
            normalButton.SetActive(false);
            qNumText.SetActive(false);

            endCanv.SetActive(true);
            menuButton.SetActive(false);
            endCanv.GetComponentInChildren<Text>().text = "Score: " + correct + " of " + numQuestions;
        }
        else
        {
            leftEye.SetActive(false);
            rightEye.SetActive(false);
            leftButton.SetActive(false);
            rightButton.SetActive(false);
            normalButton.SetActive(false);
            qNumText.SetActive(false);
            StartCoroutine(delayCo());
            applyIndex(testIndex);
        }
    }
    private IEnumerator delayCo()
    {
        yield return new WaitForSeconds(1.5f);
        leftEye.SetActive(true);
        rightEye.SetActive(true);
        leftButton.SetActive(true);
        rightButton.SetActive(true);
        normalButton.SetActive(true);
        qNumText.SetActive(true);
    }

    public void returnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    void setSpeeds(GameObject eye)
    {
        Animator anim = eye.GetComponent<Animator>();
        anim.SetFloat("ConstrictSpeedMult", constrictionSpeedMod);
        anim.SetFloat("DilationSpeedMult", dilationSpeedMod);
    }

    // Applies a random RAPD when button is pressed
    public void applyRandom(bool testMode)
    {
        if (!testMode)
        {
            leftRAPD.SetActive(false);
            rightRAPD.SetActive(false);
            applyRandomButton.SetActive(false);
            revealButton.SetActive(true);
            slider.SetActive(false);
        }

        if (Random.value > 0.3) // 70% chance to turn on an RAPD, it isn't always guarenteed to be there
        {
            RAPDintensity = Random.Range(RAPDMin, RAPDMax); // random intensity
            slider.GetComponent<Slider>().value = RAPDintensity;

            if (Random.value > 0.5) // 50% chance for the eye
            {
                leftRAPD.GetComponent<Toggle>().isOn = true;
                rightRAPD.GetComponent<Toggle>().isOn = false;

                leftController.RAPDScaler = RAPDintensity;
            }
            else
            {
                leftRAPD.GetComponent<Toggle>().isOn = false;
                rightRAPD.GetComponent<Toggle>().isOn = true;

                rightController.RAPDScaler = RAPDintensity;
            }
        } 
        else
        {
            leftRAPD.GetComponent<Toggle>().isOn = false;
            rightRAPD.GetComponent<Toggle>().isOn = false;
        }
    }

    // Reveal the eye and the intensity (a crude but simple way of doing so)
    public void reveal()
    {
        leftRAPD.SetActive(true);
        rightRAPD.SetActive(true);
        applyRandomButton.SetActive(true);
        revealButton.SetActive(false);
        if (leftRAPD.GetComponent<Toggle>().isOn || rightRAPD.GetComponent<Toggle>().isOn)
        {
            slider.SetActive(true);
        }
    }

    // Toggle button for RAPD buttons
    public void leftRAPDToggle(bool input)
    {
        leftController.RAPD = input;
        if (leftRAPD.activeSelf)
        {
            slider.SetActive(input);
        }
    }

    public void rightRAPDToggle(bool input)
    {
        rightController.RAPD = input;
        if (rightRAPD.activeSelf)
        {
            slider.SetActive(input);
        }
    }
    // RAPD slider input
    public void sliderInput()
    {
        RAPDintensity = slider.GetComponent<Slider>().value;
        leftController.RAPDScaler = RAPDintensity;
        rightController.RAPDScaler = RAPDintensity;
    }

    // Normal eye driving the other
    void normalUpdate(EyeController master, EyeController slave)
    {
        master.overridden = false; // state which eye is overridding the other
        slave.overridden = true;

        slave.overriddenTarget = master.targetDilation; // set the other eyes target to be the same
    }

    // Determine whether to switch control of one eye driving the other
    bool switchControl(EyeController currentMaster, EyeController currentSlave)
    {
        if (currentMaster.litStat == EyeController.litStatus.RegUNLit && currentSlave.litStat != EyeController.litStatus.RegUNLit) // if the currently non-driving eye IS REGISTERING not being unlit while the current master eye IS REGISTERING being unlit the switch which eye is in control
        {
            return true;
        }
        else if (currentMaster.calculatedTarget > currentSlave.calculatedTarget)
        {
            return true;
        }
        return false;
    }


    bool leftControl = true; // whether the left eye is currently in control (the master)
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0) // if there are currently touch inputs
        {
            Touch touch = Input.GetTouch(0);

            if (!torch.activeSelf && touch.phase == TouchPhase.Began) // if torch is inactive and it is a new touch then activate torch
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                torch.transform.position = new Vector3(touchPos.x, touchPos.y + TorchController.touchOffset, 1); //moves torch back to correct place before reactivating it
                torch.SetActive(true);
            }
            else if (torch.activeSelf && touch.phase == TouchPhase.Ended) // if torch is active and no longer finger on screen the deactivate
            {
                torch.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) // sued for calibration and debugging in editor
        {
            torch.SetActive(true);
        }

        if (leftControl) // determine whether or not to switch control or not
        {
            if (switchControl(leftController, rightController))
            {
                leftControl = false;
                Debug.Log("Giving Right Control");
            }
        } 
        else
        {
            if (switchControl(rightController, leftController)) {
                leftControl = true;
                Debug.Log("Giving Left Control");
            }
        }
        
        if (leftControl) // update depending on which eye is in control
        {
            normalUpdate(leftController, rightController);
        } 
        else
        {
            normalUpdate(rightController, leftController);
        }
    }
}
