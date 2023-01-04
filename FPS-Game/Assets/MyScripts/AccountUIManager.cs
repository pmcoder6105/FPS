using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AccountUIManager : MonoBehaviour
{
    public static AccountUIManager instance;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;




    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    public GameObject menuCanvas;
    public GameObject accountCanvas;
    public GameObject tutCanvas;
    public GameObject mainCamera;

    public TMP_InputField usernameInputField;

    public GameObject titleMenu;

    public GameObject mainMenuBeanObject;
    public Material healthyMat;
    public FlexibleColorPicker fcp;

    public GameObject verifyEmailUI;
    public TMP_Text verifyEmailText;

    public GameObject resetPassword_UI;
    public TMP_Text resetPasswordOutputText;
    public TMP_InputField resetPasswordEmailText;

    public TMP_Text accountDetailsEmail;
    public TMP_Text accountDetailsUsername;

    public TMP_InputField resetEmailVerifyEmail;
    public TMP_InputField resetEmailVerifyPassword;
    public TMP_Text resetEmailErrorText;

    public TMP_InputField resetEmailNewEmail;
    public TMP_Text resetEmailNewEmailErrorText;

    public GameObject resetEmailNewEmailMenu;
    public GameObject reauthenticateUserForEmailResetMenu;

    public Slider levelBar;
    public TMP_Text XPText;
    public TMP_Text levelText;

    public Toggle quality, music, fullscreenMode, crosshair;

    public Toggle[] settings;

    [HideInInspector] public GameObject process;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad <= Mathf.Epsilon)
            process = GameObject.Find("Post-process Volume");
    }

    //Functions to change the login screen UI
    public void LoginScreen() //Back button
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
        verifyEmailUI.SetActive(false);
        resetPassword_UI.SetActive(false);
        resetPasswordOutputText.text = "";
    }
    public void RegisterScreen() // Regester button
    {
        loginUI.SetActive(false);
        verifyEmailUI.SetActive(false);

        registerUI.SetActive(true);
    }

    public void AwaitVerification(bool _emailSent, string _email, string _output)
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        accountCanvas.SetActive(true);
        emailLoginField.text = "";
        passwordLoginField.text = "";
        warningLoginText.text = "";
        confirmLoginText.text = "";
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
        warningRegisterText.text = "";
        resetPasswordOutputText.text = "";
        resetPasswordEmailText.text = "";

        verifyEmailUI.SetActive(true);
        if (_emailSent)
        {
            verifyEmailText.text = $"Sent Email!\nPlease Verify {_email}. Make sure to check the Spam Folder if you can't find it";
        }
        else
        {
            verifyEmailText.text = $"Email Not Send: {_output}\nPlease Verify {_email}.";
        }
    }

    public void AwaitReset(bool _emailSent, string _email, string _output)
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        accountCanvas.SetActive(true);
        emailLoginField.text = "";
        passwordLoginField.text = "";
        warningLoginText.text = "";
        confirmLoginText.text = "";
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
        warningRegisterText.text = "";
        resetPasswordOutputText.text = "";
        resetPasswordEmailText.text = "";

        resetPassword_UI.SetActive(true);

        if (_emailSent)
        {
            resetPasswordOutputText.text = $"Sent Email!\nPlease Reset at {_email}. Make sure to check the Spam Folder if you can't find it";
        }
        else
        {
            resetPasswordOutputText.text = $"Email Not Send: {_output}";
        }
    }
}
