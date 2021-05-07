using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
public class UIMenuController : MonoBehaviour
{
    private static UIMenuController instance;
    public Color32 activeColor = new Color32(79, 188, 226, 255);
    public Color32 inactiveColor = new Color32(117, 117, 117, 255);
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
        //Get Icon and/or Text
        SVGImage menuButtonIconChild = menuButton.transform.GetComponentInChildren<SVGImage>();
        TextMeshProUGUI menuButtonTextChild = menuButton.transform.GetComponentInChildren<TextMeshProUGUI>();
        if (isActiveState)
        {
            if (menuButtonIconChild != null)
                menuButtonIconChild.color = activeColor;
            if (menuButtonTextChild != null)
                menuButtonTextChild.color = activeColor;
        }
        else
        {
            if (menuButtonIconChild != null)
                menuButtonIconChild.color = inactiveColor;
            if (menuButtonTextChild != null)
                menuButtonTextChild.color = inactiveColor;
        }
    }
}