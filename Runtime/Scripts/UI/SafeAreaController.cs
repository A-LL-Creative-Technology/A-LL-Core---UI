using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaController : MonoBehaviour
{
#pragma warning disable 0649

    [HideInInspector]
    public enum SafeAreaType
    {
        TopBackground,
        TopContent,
        LeftBackground,
        Body,
        RightBackground,
        BottomBackground,
        BottomContent
    };

    public SafeAreaType safeAreaType = SafeAreaType.TopBackground;

    private Rect safeArea;


#pragma warning restore 0649

    private void Start()
    {
        if (ALLCoreConfig.GetInstance() != null && !ALLCoreConfig.GetInstance().activateSafeArea) // to make it possible to use SafeAreaController script even on scenes where A-LL Core is not loaded
            return;

        safeArea = Screen.safeArea;

        //safeArea = new Rect(100, 100, 1000, 2000);

        switch (safeAreaType)
        {
            case SafeAreaType.TopBackground:

                AdjustSafeAreaTopBackground();

                break;

            case SafeAreaType.TopContent:

                AdjustSafeAreaTopContent();

                break;

            case SafeAreaType.LeftBackground:

                AdjustSafeAreaLeftBackground();

                break;

            case SafeAreaType.Body:

                AdjustSafeAreaBody();

                break;

            case SafeAreaType.RightBackground:

                AdjustSafeAreaRightBackground();

                break;

            case SafeAreaType.BottomBackground:

                AdjustSafeAreaBottomBackground();

                break;

            case SafeAreaType.BottomContent:

                AdjustSafeAreaBottomContent();

                break;
        }
    }

    private void AdjustSafeAreaTopBackground()
    {
        // adjust header
        RectTransform topContainerRectTransform = transform.GetComponent<RectTransform>();
        topContainerRectTransform.anchorMin = new Vector2(topContainerRectTransform.anchorMin.x, topContainerRectTransform.anchorMin.y - (Screen.height - safeArea.position.y - safeArea.size.y) / Screen.height);

        topContainerRectTransform.offsetMin = Vector2.zero;
        topContainerRectTransform.offsetMax = Vector2.zero;
    }


    private void AdjustSafeAreaTopContent()
    {
        // adjust header
        RectTransform topContainerRectTransform = transform.GetComponent<RectTransform>();

        topContainerRectTransform.anchorMin = new Vector2(safeArea.position.x / Screen.width, topContainerRectTransform.anchorMin.y - (Screen.height - safeArea.position.y - safeArea.size.y) / Screen.height);
        topContainerRectTransform.anchorMax = new Vector2((safeArea.position.x + safeArea.size.x) / Screen.width, (safeArea.position.y + safeArea.size.y) / Screen.height);

        topContainerRectTransform.offsetMin = Vector2.zero;
        topContainerRectTransform.offsetMax = Vector2.zero;
    }

    private void AdjustSafeAreaLeftBackground()
    {
        // adjust header
        RectTransform leftContainerRectTransform = transform.GetComponent<RectTransform>();
        leftContainerRectTransform.anchorMax = new Vector2(leftContainerRectTransform.anchorMin.x + (safeArea.position.x / Screen.width), leftContainerRectTransform.anchorMin.y);

        leftContainerRectTransform.offsetMin = Vector2.zero;
        leftContainerRectTransform.offsetMax = Vector2.zero;
    }

    private void AdjustSafeAreaBody()
    {
        // adjust body
        RectTransform bodyRectTransform = transform.GetComponent<RectTransform>();
        bodyRectTransform.anchorMin = new Vector2(bodyRectTransform.anchorMin.x + (safeArea.position.x / Screen.width), (bodyRectTransform.anchorMin.y + (safeArea.position.y / Screen.height)));
        bodyRectTransform.anchorMax = new Vector2(bodyRectTransform.anchorMax.x - (Screen.width - safeArea.position.x - safeArea.size.x) / Screen.width, bodyRectTransform.anchorMax.y - (Screen.height - safeArea.position.y - safeArea.size.y) / Screen.height);

        bodyRectTransform.offsetMin = Vector2.zero;
        bodyRectTransform.offsetMax = Vector2.zero;
    }

    private void AdjustSafeAreaRightBackground()
    {
        // adjust header
        RectTransform rightContainerRectTransform = transform.GetComponent<RectTransform>();
        rightContainerRectTransform.anchorMin = new Vector2(rightContainerRectTransform.anchorMin.x - (Screen.width - safeArea.position.x - safeArea.size.x) / Screen.height, rightContainerRectTransform.anchorMin.y);

        rightContainerRectTransform.offsetMin = Vector2.zero;
        rightContainerRectTransform.offsetMax = Vector2.zero;
    }

    private void AdjustSafeAreaBottomBackground()
    {
        // adjust footer
        RectTransform bottomContainerRectTransform = transform.GetComponent<RectTransform>();
        bottomContainerRectTransform.anchorMax = new Vector2(bottomContainerRectTransform.anchorMax.x, (bottomContainerRectTransform.anchorMax.y + (safeArea.position.y / Screen.height)));

        bottomContainerRectTransform.offsetMin = Vector2.zero;
        bottomContainerRectTransform.offsetMax = Vector2.zero;
    }

    private void AdjustSafeAreaBottomContent()
    {
        // adjust footer
        RectTransform bottomContainerRectTransform = transform.GetComponent<RectTransform>();
        bottomContainerRectTransform.anchorMin = new Vector2(safeArea.position.x / Screen.width, safeArea.position.y / Screen.height);
        bottomContainerRectTransform.anchorMax = new Vector2((safeArea.position.x + safeArea.size.x) / Screen.width, (bottomContainerRectTransform.anchorMax.y + (safeArea.position.y / Screen.height)));

        bottomContainerRectTransform.offsetMin = Vector2.zero;
        bottomContainerRectTransform.offsetMax = Vector2.zero;
    }
}
