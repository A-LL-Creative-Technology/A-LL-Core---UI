using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    // Start is called before the first frame update
    private void OnEnable()
    {
        progressBar.fillAmount = 0;
    }

    public void SetFillAmount(float amount)
    {
        progressBar.fillAmount = amount;
    }
}
