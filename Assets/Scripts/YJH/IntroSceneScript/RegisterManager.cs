using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterManager : MonoBehaviour
{
    public Toggle tg1;
    public Toggle tg2;
    public Toggle tg3;

    public GameObject nextButton;
    public GameObject tgController;

    public GameObject CreateAccount;
    public InputField email;
    public InputField password;
    void Start()
    {
        nextButton.SetActive(false);
        CreateAccount.SetActive(false);
    }

    public void OnXbuttonClick()
    {
        tg1.isOn = false;
        tg2.isOn = false;
        tg3.isOn = false;
        tgController.SetActive(true);
        CreateAccount.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnNextButtonClick()
    {
        tgController.SetActive(false);
        CreateAccount.SetActive(true);
    }

    private void Update()
    {
        if (tg1.isOn && tg2.isOn && tg3.isOn)
        {
            nextButton.SetActive(true);
        }
        else
        {
            nextButton.SetActive(false);
        }
    }
}
