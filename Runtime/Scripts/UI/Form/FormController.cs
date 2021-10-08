using System;
using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FormController : MonoBehaviour
{
    private static FormController instance;

    public static FormController GetInstance()
    {
        return instance;
    }

#pragma warning disable 0649

    public List<InputFieldController> inputFields;

    [SerializeField] private Button validationButton;

    [HideInInspector] public Canvas screenReference;
    public GameObject keyboardParentView;


    [NonSerialized] public bool isSelectionInProgress = false;
    [NonSerialized] public bool isDeselectionInProgress = false;

#pragma warning restore 0649

    private void Awake()
    {
        instance = this;

        validationButton.interactable = false;
    }

    private void Start()
    {
        screenReference = NavigationController.GetInstance().viewsCanvas.GetComponent<Canvas>();
    }

    public void OnInputFieldValueChangedCheck()
    {

        bool areAllEntriesValid = true;
        foreach (InputFieldController currentInputField in inputFields)
        {
            if (!currentInputField.isOptionalField && currentInputField.inputFieldTMP.text == "")
                areAllEntriesValid = false;
        }

        validationButton.interactable = areAllEntriesValid;

    }

}
