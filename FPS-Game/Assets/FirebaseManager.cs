using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Photon.Pun;


public class FirebaseManager : MonoBehaviour
{

    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public static FirebaseUser User;
    public DatabaseReference DBReference;

    

    private static FirebaseManager _singleton;


    private void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });

        if (User == null)
        {
            AccountUIManager.instance.menuCanvas.SetActive(false);
            AccountUIManager.instance.accountCanvas.SetActive(true);
        }
        else if (User != null)
        {
            //StartCoroutine(LoadUsernameData());
            User = auth.CurrentUser;
            AccountUIManager.instance.menuCanvas.SetActive(true);
            AccountUIManager.instance.accountCanvas.SetActive(false);
        }
        Debug.Log(PhotonNetwork.NickName);
        //usernameInputField.text = PhotonNetwork.NickName;
        //PhotonNetwork.NickName = usernameInputField.text;
        // && accountCanvas.transform.GetChild(1).gameObject.activeInHierarchy == true

        Singleton = this;

        DontDestroyOnLoad(this.gameObject);

    }

    public static FirebaseManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(FirebaseManager)} instance already exists, destroying object!");
                Destroy(value.gameObject);
            }
        }
    }

    private void Update()
    {
        //if (User == null && accountCanvas.transform.GetChild(1).gameObject.activeInHierarchy == true)
        //{
        //   menuCanvas.SetActive(false);
        //   accountCanvas.SetActive(true);
        //}
        //else if (User != null && accountCanvas.transform.GetChild(1).gameObject.activeInHierarchy == true)
        //{
        //    StartCoroutine(LoadUsernameData());
        //    User = auth.CurrentUser;
        //    menuCanvas.SetActive(true);
        //    accountCanvas.SetActive(false);
        //}
        //Debug.Log(PhotonNetwork.NickName);
        ////usernameInputField.text = PhotonNetwork.NickName;
        //PhotonNetwork.NickName = usernameInputField.text;
    }

    public void SaveUsernameData()
    {
        StartCoroutine(UpdateUsernameAuth(AccountUIManager.instance.usernameInputField.text));
        StartCoroutine(UpdateUsernameDatabase(AccountUIManager.instance.usernameInputField.text));
        
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBReference = FirebaseDatabase.DefaultInstance.RootReference;

        //StartCoroutine(CheckAutoLogin());
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(AccountUIManager.instance.emailLoginField.text, AccountUIManager.instance.passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(AccountUIManager.instance.emailRegisterField.text, AccountUIManager.instance.passwordRegisterField.text, AccountUIManager.instance.usernameRegisterField.text));
    }

    private IEnumerator CheckAutoLogin()
    {
        yield return new WaitForEndOfFrame();
        if (User != null)
        {
            var reloadUserTask = User.ReloadAsync();
            yield return new WaitUntil(predicate: () => reloadUserTask.IsCompleted);
            AutoLogin();
        }
        else
        {
            AccountUIManager.instance.LoginScreen();
        }
    }

    private void AutoLogin()
    {
        if (User == null)
        {
            AccountUIManager.instance.LoginScreen();
        }
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            AccountUIManager.instance.warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            AccountUIManager.instance.warningLoginText.text = "";
            AccountUIManager.instance.confirmLoginText.text = "Logged In";
            StartCoroutine(LoadUsernameData());

            yield return new WaitForSeconds(2);

            //SceneManager.LoadScene(1);
            AccountUIManager.instance.menuCanvas.SetActive(true);
            AccountUIManager.instance.accountCanvas.SetActive(false);
            AccountUIManager.instance.mainCamera.transform.Find("PlayerViewer").gameObject.SetActive(true);
            AccountUIManager.instance.confirmLoginText.text = "";
            AccountUIManager.instance.emailLoginField.text = "";
            AccountUIManager.instance.passwordLoginField.text = "";
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            AccountUIManager.instance.warningRegisterText.text = "Missing Username";
        }
        else if (AccountUIManager.instance.passwordRegisterField.text != AccountUIManager.instance.passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            AccountUIManager.instance.warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                AccountUIManager.instance.warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        AccountUIManager.instance.warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        AccountUIManager.instance.LoginScreen();
                        AccountUIManager.instance.warningRegisterText.text = "";
                    }
                }
            }
        }
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Call the Firebase auth update user profile function passing the profile with the username
        var DBTask = DBReference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    public  IEnumerator LoadUsernameData()
    {
        var DBTask = DBReference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            AccountUIManager.instance.usernameInputField.text = "Guest " + Random.Range(0, 1000).ToString("000");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            AccountUIManager.instance.usernameInputField.text = snapshot.Child("username").Value.ToString();
            PhotonNetwork.NickName = snapshot.Child("username").Value.ToString();
        }
    }
}
