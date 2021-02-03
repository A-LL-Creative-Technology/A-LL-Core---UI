using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuController : MonoBehaviour
{
    private static UIMenuController instance;

    public static UIMenuController GetInstance()
    {
        return instance;
    }

    [HideInInspector] public Dictionary<string, GameObject> menuButtons = new Dictionary<string, GameObject>();

    public GameObject currentMenuButton;

    private void Awake()
    {
        instance = this;

        InitMenu();
    }

    private void InitMenu()
    {
        // Detect menu buttons in hierarchy
        foreach (Transform child in transform)
        {            
            // store the buttons in a dictionary
            menuButtons.Add(child.name, child.gameObject);

        }
    }

    public void ChangeMenuButtonState(GameObject menuButton)
    {
        SetMenuButtonActiveState(currentMenuButton, false);
        SetMenuButtonActiveState(menuButton, true);

        currentMenuButton = menuButton;
    }

    private void SetMenuButtonActiveState(GameObject menuButton, bool isActiveState)
    {
        Transform menuButtonChild = menuButton.transform.GetChild(0);

        if (isActiveState)
        {
            menuButtonChild.GetComponent<Image>().sprite = menuButtonChild.GetComponent<UIMenuButtonController>().selectedState;
        }
        else
        {
            menuButtonChild.GetComponent<Image>().sprite = menuButtonChild.GetComponent<UIMenuButtonController>().normalState;
        }
    }
}
