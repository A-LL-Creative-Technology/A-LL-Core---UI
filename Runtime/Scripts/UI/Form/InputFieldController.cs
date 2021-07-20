using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputFieldController : MonoBehaviour
{
#pragma warning disable 0649

    private readonly float INPUT_FIELD_ANIMATION_DURATION = 0.4f;
    private readonly float INPUT_FIELD_LABEL_MOVEMENT_DISTANCE = 80f;
    private readonly float INPUT_FIELD_LABEL_MOVEMENT_SCALE = 0.7f;

    private FormController formController;

    public bool isOptionalField; // usefull for fields that are not required

    [Header("Password Parameters")]
    public bool isPassword;
    [ConditionalHide("isPassword", true)] [SerializeField] private GameObject eyeButton;
    [ConditionalHide("isPassword", true)] [SerializeField] private Texture2D openedEye;
    [ConditionalHide("isPassword", true)] [SerializeField] private Texture2D closedEye;

    private RawImage eyeButtonRawImage;

    private GameObject inputField;
    private GameObject label;
    private TMP_Text labelTMP;
    [NonSerialized] public TMP_InputField inputFieldTMP;

    private Button togglePasswordButton;

#pragma warning restore 0649

    private void Awake()
    {
        // search for the parent Form
        Transform currentTransform = transform;
        while (currentTransform.name != "Form")
        {
            currentTransform = currentTransform.transform.parent;
        }

        formController = currentTransform.GetComponent<FormController>();

        inputField = transform.Find("Input Field").gameObject;
        inputFieldTMP = inputField.GetComponent<TMP_InputField>();

        label = transform.Find("Label").gameObject;
        labelTMP = label.GetComponent<TMP_Text>();


        if (isPassword)
        {

            eyeButtonRawImage = eyeButton.transform.Find("Eye Toggle").gameObject.GetComponent<RawImage>();

            togglePasswordButton = transform.Find("Hit Zone - Eye").GetComponent<Button>();

            // check the structure
            if (!togglePasswordButton)
                GlobalController.LogMe("The toggle password cannot be found in hierarchy.");
        }

        // check the structure
        if (!inputField || !label)
            GlobalController.LogMe("The input field or its label cannot be found in hierarchy.");
    }

    private void OnEnable()
    {

        inputFieldTMP.onValueChanged.AddListener(OnValueChangedCheck);
        inputFieldTMP.onDeselect.AddListener(OnInputFieldHide);

        label.GetComponent<Button>().onClick.AddListener(OnInputFieldTouched);

        if (isPassword)
            togglePasswordButton.onClick.AddListener(OnTogglePasswordVisibility);
    }

    private void OnDisable()
    {
        inputFieldTMP.onValueChanged.RemoveListener(OnValueChangedCheck);
        inputFieldTMP.onDeselect.RemoveListener(OnInputFieldHide);

        label.GetComponent<Button>().onClick.RemoveListener(OnInputFieldTouched);

        if (isPassword)
            togglePasswordButton.onClick.RemoveListener(OnTogglePasswordVisibility);

        // make sure UI is down
        NavigationController.GetInstance().currentView.transform.localPosition = new Vector3(NavigationController.GetInstance().currentView.transform.localPosition.x, NavigationController.GetInstance().overViewsInitialYPosition, NavigationController.GetInstance().currentView.transform.localPosition.z);
    }

    private void OnInputFieldTouched()
    {
        StartCoroutine(WaitForDoingTouch());
    }

    private IEnumerator WaitForDoingTouch()
    {

        while (FormController.GetInstance().isDeselectionInProgress)
            yield return null;

        if (FormController.GetInstance().isSelectionInProgress) // to prevent double-click on the label
            yield break;

        FormController.GetInstance().isSelectionInProgress = true;

        // if there is text in the input field, we don't hide it anymore
        if (inputFieldTMP.text == "")
        {
            MoveUpLabel(() =>
            {

                inputField.SetActive(true);
                if (isPassword)
                    eyeButton.SetActive(true);

                inputFieldTMP.ActivateInputField();

                MoveUpInputFieldUI();
            });
        }
        else
        {
            labelTMP.raycastTarget = false;

            inputFieldTMP.ActivateInputField();

            MoveUpInputFieldUI();
        }
    }

    private void MoveUpLabel(Action callback = null)
    {
        LeanTween.moveLocalY(label, label.transform.localPosition.y + INPUT_FIELD_LABEL_MOVEMENT_DISTANCE, INPUT_FIELD_ANIMATION_DURATION).setEaseInOutExpo();
        LeanTween.scale(label, INPUT_FIELD_LABEL_MOVEMENT_SCALE * Vector3.one, INPUT_FIELD_ANIMATION_DURATION).setEaseInOutExpo().setOnComplete(() =>
        {
            callback?.Invoke();

        });
    }

    private void MoveUpInputFieldUI()
    {

        // decide where to put the input field on the screen (ratio from the top)
        float inputFieldScreenRatio = 0.25f;

        // get parent of the input field which is a canvas
        GameObject parentView = inputField.GetComponentInParent<Canvas>().gameObject;

        // visible area height
        int visibleAreaHeight = Screen.height;

        // compute y position of the input field with respect to parent (with respect to the pivot y: 1=top, 0.5=middle, etc)
        float inputFieldPositionFromTopBeforePivotCorrection = -parentView.transform.InverseTransformPoint(inputField.transform.position).y;

        // correction for the pivot
        int inputFieldPositionFromTop = (int)(inputFieldPositionFromTopBeforePivotCorrection + (1 - parentView.GetComponent<RectTransform>().pivot.y) * visibleAreaHeight);

        // move the input field to its final location in the visible area
        int inputFieldFinalPosition = (int)(inputFieldPositionFromTop - visibleAreaHeight * inputFieldScreenRatio);

        // if input field is in scroll view, we adjust the viewport
        bool isInScrollView = true;

        Transform viewportTransform = transform;
        Transform contentTransform = transform;
        while (viewportTransform.name != "Viewport")
        {
            viewportTransform = viewportTransform.parent;

            if (viewportTransform == null)
            {
                isInScrollView = false;
                break;
            }

            if (viewportTransform.name == "Content")
                contentTransform = viewportTransform;
        }



        if (isInScrollView)
        {
            Transform scrollRectFaster = viewportTransform.parent;
            scrollRectFaster.GetComponent<ScrollRectFaster>().movementType = ScrollRectFaster.MovementType.Unrestricted;

            if (inputFieldFinalPosition > 0)
            {
                ((RectTransform)viewportTransform).offsetMin = new Vector2(((RectTransform)viewportTransform).offsetMin.x, -inputFieldFinalPosition);
            }
            else
            {
                ((RectTransform)viewportTransform).offsetMax = new Vector2(((RectTransform)viewportTransform).offsetMax.x, -inputFieldFinalPosition);
                ((RectTransform)contentTransform).localPosition = new Vector3(((RectTransform)contentTransform).localPosition.x, ((RectTransform)contentTransform).localPosition.y + inputFieldFinalPosition, ((RectTransform)contentTransform).localPosition.z);
            }
        }


        LeanTween.moveLocalY(NavigationController.GetInstance().currentView, NavigationController.GetInstance().overViewsInitialYPosition + inputFieldFinalPosition, INPUT_FIELD_ANIMATION_DURATION).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() => {
            FormController.GetInstance().isSelectionInProgress = false;
        });
    }

    private void OnInputFieldHide(string text)
    {
        StartCoroutine(WaitForDoingHide());
    }

    private IEnumerator WaitForDoingHide()
    {
        while (FormController.GetInstance().isSelectionInProgress)
            yield return null;

        if (FormController.GetInstance().isDeselectionInProgress) // to prevent double-click on the label
            yield break;

        FormController.GetInstance().isDeselectionInProgress = true;

        // reactivate label
        labelTMP.raycastTarget = true;

        // if there is text in the input field, we don't hide it anymore
        if (inputFieldTMP.text == "")
        {

            inputField.SetActive(false);
            if (isPassword)
                eyeButton.SetActive(false);

            MoveDownLabel(() => {

                MoveDownInputFieldUI();
            });


        }
        else
        {
            MoveDownInputFieldUI();
        }
    }

    private void MoveDownLabel(Action callback = null)
    {
        LeanTween.moveLocalY(label, label.transform.localPosition.y - INPUT_FIELD_LABEL_MOVEMENT_DISTANCE, INPUT_FIELD_ANIMATION_DURATION).setEaseInOutExpo();
        LeanTween.scale(label, Vector3.one, INPUT_FIELD_ANIMATION_DURATION).setEaseInOutExpo().setOnComplete(() =>
        {
            callback?.Invoke();
        });
    }

    private void MoveDownInputFieldUI()
    {

        // reset the scroll view
        LeanTween.moveLocalY(NavigationController.GetInstance().currentView, NavigationController.GetInstance().overViewsInitialYPosition, INPUT_FIELD_ANIMATION_DURATION).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() => {

            FormController.GetInstance().isDeselectionInProgress = false;

            // if input field is in scroll view, we adjust the viewport
            bool isInScrollView = true;
            Transform viewportTransform = transform;
            Transform contentTransform = transform;
            while (viewportTransform.name != "Viewport")
            {
                viewportTransform = viewportTransform.parent;

                if (viewportTransform == null)
                {
                    isInScrollView = false;
                    break;
                }

                if (viewportTransform.name == "Content")
                    contentTransform = viewportTransform;
            }

            if (isInScrollView)
            {
                Transform scrollRectFaster = viewportTransform.parent;
                scrollRectFaster.GetComponent<ScrollRectFaster>().movementType = ScrollRectFaster.MovementType.Elastic;

                //Reset the bottom of Viewport at position 0
                ((RectTransform)contentTransform).localPosition = new Vector3(((RectTransform)contentTransform).localPosition.x, ((RectTransform)contentTransform).localPosition.y + ((RectTransform)viewportTransform).offsetMax.y, ((RectTransform)contentTransform).localPosition.z);
                ((RectTransform)viewportTransform).offsetMax = new Vector2(((RectTransform)viewportTransform).offsetMax.x, 0);
                ((RectTransform)viewportTransform).offsetMin = new Vector2(((RectTransform)viewportTransform).offsetMin.x, 0);
            }

        });
    }

    private void OnTogglePasswordVisibility()
    {
        inputFieldTMP.contentType = inputFieldTMP.contentType == TMP_InputField.ContentType.Standard ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
        eyeButtonRawImage.texture = eyeButtonRawImage.texture == openedEye ? closedEye : openedEye;

        // HACK - TO BE FIXED !!!
        // we force refresh of the password field (bug in TMP_InputField)
        string pwd = inputFieldTMP.text;
        inputFieldTMP.text = "dummy";
        inputFieldTMP.text = pwd;
    }

    private void OnValueChangedCheck(string text)
    {
        if (inputFieldTMP.isFocused)
        {
            if (inputFieldTMP.text == "")
                labelTMP.raycastTarget = true;
            else
                labelTMP.raycastTarget = false;
        }

        formController.OnInputFieldValueChangedCheck();

    }

    public void SetInputFieldValue(string value)
    {
        // we set an empty value to an empty input field => we don't do anything
        if (String.IsNullOrEmpty(inputFieldTMP.text) && String.IsNullOrEmpty(value))
            return;

        // we set an empty value to a non-empty input field => we reset the input field
        if (!String.IsNullOrEmpty(inputFieldTMP.text) && String.IsNullOrEmpty(value))
        {
            MoveDownLabel();
            inputField.SetActive(false);

            inputFieldTMP.text = value;
            return;
        }

        // we set a non-empty value to a previously empty input field => we init the input field
        if (String.IsNullOrEmpty(inputFieldTMP.text) && !String.IsNullOrEmpty(value))
        {
            MoveUpLabel();
            inputField.SetActive(true);

            inputFieldTMP.text = value;
            return;
        }

        // we set a non-empty value to a non-empty input field => we update the input field value
        if (!String.IsNullOrEmpty(inputFieldTMP.text) && !String.IsNullOrEmpty(value))
        {
            inputFieldTMP.text = value;
            return;
        }


    }
}
