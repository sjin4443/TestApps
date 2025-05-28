using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{ 
    private int thisPass;

    // I am pretty sure this is incredibly insecure, but it doesn't realistically need to be more secure for the purposes it is being used for 
    private readonly string[] passwords = { "IPass1", "FPass2" };

    public GameObject testButton;
    public GameObject finalButton;

    public GameObject practiceFreeButton;
    public GameObject practiceTestButton;

    public GameObject backPassButton;
    public GameObject passwordField;
    public GameObject enterButton;

    public GameObject topPractice;
    public GameObject topTest;

    public GameObject backButton;


    public void activateTest(bool a)
    {
        testButton.SetActive(a);
        finalButton.SetActive(a);
        backButton.SetActive(a);
        
        backPassButton.SetActive(!a);
        passwordField.SetActive(!a);
        enterButton.SetActive(!a);
    }


    public void checkPassword()
    {
        string input = passwordField.GetComponent<UnityEngine.UI.InputField>().text;
        Debug.Log("Input: '" + input + "', target: '" + passwords[thisPass] + "'");
        if (input.Equals(passwords[thisPass]))
        {
            ModeTracker.mode = thisPass;
            SceneManager.LoadScene(1);
        } 
        else
        {
            passwordField.GetComponent<UnityEngine.UI.InputField>().text = "";
        }
    }


    public void backPass()
    {
        activateTest(true);
        passwordField.GetComponent<UnityEngine.UI.InputField>().text = "";
    }

    public void InitialTesting()
    {
        activateTest(false);
        thisPass = 0;
    }



    public void FinalTest()
    {
        activateTest(false);
        thisPass = 1;
    }
    public void PracticeMode()
    {
        ModeTracker.mode = 2;
        SceneManager.LoadScene(1);
    }

    public void PracticeTest()
    {
        ModeTracker.mode = 3;
        SceneManager.LoadScene(1);
    }

    public void testSection()
    {
        topPractice.SetActive(false);
        topTest.SetActive(false);

        backButton.SetActive(true);
        testButton.SetActive(true);
        finalButton.SetActive(true);
    }

    public void practiceSection()
    {
        topPractice.SetActive(false);
        topTest.SetActive(false);

        backButton.SetActive(true);
        practiceFreeButton.SetActive(true);
        practiceTestButton.SetActive(true);
    }

    public void back()
    {
        topTest.SetActive(true);
        topPractice.SetActive(true);
        backButton.SetActive(false);

        testButton.SetActive(false);
        finalButton.SetActive(false);

        practiceFreeButton.SetActive(false);
        practiceTestButton.SetActive(false);
    }
}
