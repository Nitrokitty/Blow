using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using System.ComponentModel;
public class Login : MonoBehaviour
{

    bool oldShow = true;
    public bool show = false;
    public byte backgroundAlpha = 50;
    public GUIStyle backgroundStyle;
    public GUIStyle LargeButtons;
    public GUIStyle SmallLabel;
    public GUIStyle Submit;
    public GUIStyle validationBox;
    public float midOffset = 10f;
    SpriteRenderer backgroundSprite;
    LoginState currentLoginState = LoginState.Deciding;
    Rect backgroundRect;
    Rect topRect;
    Rect bottomRect;
    Sprite boxBackground;

    GameObject loginUserName;
    GameObject loginPassword;
    RectTransform loginUserNameRect;
    RectTransform loginPasswordRect;

    GameObject signupUserName;
    GameObject signupPassword1;
    GameObject signupEmail;
    GameObject signupPassword2;
    RectTransform signupUserNameRect;
    RectTransform signupPassword1Rect;
    RectTransform signupEmailRect;
    RectTransform signupPassword2Rect;
    Vector3 topLeft;
    Vector3 topRight;
    Vector3 bottomLeft;
    Vector3 bottomRight;

    NetworkManager networkManager;
    static string specialCharacters = "*./?><-_=+#$%^&!~@";

    ValidationMessage currentValidationMessage;


    bool errorsExistSignup = false;
    bool singupHasSubmittedAtleastOnce = false;
    bool errorsExistLogin = false;
    bool loginHasSubmittedAtleastOnce = false;
    int serverRequestsInUse = 0;
    GameObject backButton;

    // Use this for initialization
    void Start()
    {
        backgroundSprite = GetComponent<SpriteRenderer>();
        backButton = transform.Find("BackButton").gameObject;
        backButton.transform.localScale = new Vector3(backButton.transform.lossyScale.x, backButton.transform.lossyScale.y, backButton.transform.lossyScale.z);
        networkManager = GameObject.Find("UserProfile").GetComponent<NetworkManager>();

        ConfigureLogin();
        ConfigureSignup();

        ResetAll();

        tryGetProfileServerRequests = new BitArray(31);
        //GameObject.Find("UserProfile").GetComponent<NetworkManager>().PostProfile(new ProfileData("USER", "PWORD", "p@d.com", "123456"));
    }

    void ResetAll()
    {
        ResetLogin();
        ResetSignup();
        ResetGuestWarning();

        backButton.GetComponent<SpriteRenderer>().enabled = false;
    }

    void ResetGuestWarning()
    {
        var button = transform.Find("ContinueButton").GetComponent<SubmitButton>();
        button.isEnabled = false;
    }

    void ResetLogin()
    {
        loginHasSubmittedAtleastOnce = false;

        loginPassword.GetComponent<InputField>().text = string.Empty;
        loginUserName.GetComponent<InputField>().text = string.Empty;

        currentValidationMessage = loginPassword.GetComponent<ValidationMessage>();
        currentValidationMessage.isEnabled = false;

        loginUserName.SetActive(false);
        loginPassword.SetActive(false);

        var button = transform.Find("LoginButton").GetComponent<SubmitButton>();
        button.isEnabled = false;

        DisableInputBackground();
    }

    void ConfigureLogin()
    {
        var canvas = GameObject.Find("LoginCanvas").transform;

        loginUserName = canvas.Find("Login_UserName_InputField").gameObject;
        loginPassword = canvas.Find("Login_Password_InputField").gameObject;
        loginUserNameRect = loginUserName.GetComponent<RectTransform>();
        loginPasswordRect = loginPassword.GetComponent<RectTransform>();
    }

    void ResetSignup()
    {
        singupHasSubmittedAtleastOnce = false;

        signupUserName.GetComponent<InputField>().text = string.Empty;
        signupPassword1.GetComponent<InputField>().text = string.Empty;
        signupPassword2.GetComponent<InputField>().text = string.Empty;
        signupEmail.GetComponent<InputField>().text = string.Empty;

        signupUserName.SetActive(false);
        signupPassword1.SetActive(false);
        signupPassword2.SetActive(false);
        signupEmail.SetActive(false);

        var button = transform.Find("SignupButton").GetComponent<SubmitButton>();
        button.isEnabled = false;

        DisableInputBackground();
    }

    void ConfigureSignup()
    {
        var canvas = GameObject.Find("SignupCanvas").transform;

        signupUserName = canvas.Find("Signup_UserName_InputField").gameObject;
        signupPassword1 = canvas.Find("Signup_Password1_InputField").gameObject;
        signupPassword2 = canvas.Find("Signup_Password2_InputField").gameObject;
        signupEmail = canvas.Find("Signup_Email_InputField").gameObject;

        signupUserNameRect = signupUserName.GetComponent<RectTransform>();
        signupPassword1Rect = signupPassword1.GetComponent<RectTransform>();
        signupPassword2Rect = signupPassword2.GetComponent<RectTransform>();
        signupEmailRect = signupEmail.GetComponent<RectTransform>();


        signupUserName.GetComponent<InputField>().onValueChanged.AddListener((d) => signupUserName.GetComponent<ValidationMessage>().RestProperties(true));
        signupPassword1.GetComponent<InputField>().onValueChanged.AddListener((d) => signupPassword1.GetComponent<ValidationMessage>().RestProperties(true));
        signupPassword2.GetComponent<InputField>().onValueChanged.AddListener((d) => signupPassword2.GetComponent<ValidationMessage>().RestProperties(true));
        signupEmail.GetComponent<InputField>().onValueChanged.AddListener((d) => signupEmail.GetComponent<ValidationMessage>().RestProperties(true));
    }

    // Update is called once per frame
    void Update()
    {

        //if (UserProfile.UserName == "Guest")
        //    show = true;

        SetRectangles();

        if (!show && !oldShow)
        {
            return;
        }
        if (show)
        {
            backButton.GetComponent<SpriteRenderer>().enabled = true;

            backButton.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(backgroundRect.x - backgroundRect.width * .2f, backgroundRect.y + backgroundRect.height * .5f, 1));
        }

        if (!show && oldShow)
        {
            transform.localScale = new Vector3(0, 0, 1);
            ResetLogin();
            ResetSignup();
            backButton.GetComponent<SpriteRenderer>().enabled = false;
            backgroundSprite.enabled = false;
        }
        if (show && !oldShow)
        {
            transform.localScale = new Vector3(backgroundRect.width / backgroundRect.height, backgroundRect.height / backgroundRect.width, 1);
            var pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 1));
            transform.position = pos;
            backButton.GetComponent<SpriteRenderer>().enabled = true;
            backgroundSprite.enabled = true;
        }

        oldShow = show;

        if (show && Size(backgroundSprite).y < backgroundRect.height && Size(backgroundSprite).x < backgroundRect.width)
        {
            Grow(gameObject, ratio: backgroundRect.width / backgroundRect.height);
        }



        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            var screenButton = Camera.main.WorldToScreenPoint(backButton.transform.position);

            var extends = backButton.GetComponent<SpriteRenderer>().bounds.extents;
            var delta = new Vector3(screenButton.x * extends.x, screenButton.y * extends.y, screenButton.z * extends.z);

            if (mousePosition.x > (screenButton.x - delta.x) &&
                mousePosition.x < (screenButton.x + delta.x) &&
                mousePosition.y > (screenButton.y - delta.y) &&
                mousePosition.y < (screenButton.y + delta.y))
            {
                Back();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            Back();
    }

    private void OnGUI()
    {

        if (show)
        {
            GUI.color = new Color32(255, 255, 255, backgroundAlpha);
            switch (currentLoginState)
            {
                case LoginState.Deciding:
                    ShowLoginSignupButtons();
                    break;
                case LoginState.EnteringLoginInfo:
                    ShowLogIn();
                    break;
                case LoginState.EnteringSignupInfo:
                    ShowSignUp();
                    break;
                case LoginState.GuestWarning:
                    GuestWarningScreen();
                    break;
            }
        }
    }

    void Back()
    {
        ResetAll();
        switch (currentLoginState)
        {
            case LoginState.Deciding:
                show = false;
                break;
            case LoginState.EnteringLoginInfo:
            case LoginState.EnteringSignupInfo:
            case LoginState.GuestWarning:
                currentLoginState = LoginState.Deciding;
                break;

        }
    }

    void ShowLoginSignupButtons()
    {
        if (GUI.Button(new Rect(Screen.width * .5f - topRect.width * .5f, topRect.y * .8f, topRect.width, topRect.height * .7f), "Log In", LargeButtons))
        {
            currentLoginState = LoginState.EnteringLoginInfo;
            SetLoginFields();
        }
        if (GUI.Button(new Rect(Screen.width * .5f - topRect.width * .5f, topRect.y * .8f + topRect.height + 5, bottomRect.width, bottomRect.height * .7f), "Sign Up", LargeButtons))
        {
            currentLoginState = LoginState.EnteringSignupInfo;
            SetSignUpFields();
            singupHasSubmittedAtleastOnce = false;
        }
        if (GUI.Button(new Rect(Screen.width * .5f - topRect.width * .5f, topRect.y * .8f + topRect.height * 2 + 5, bottomRect.width, bottomRect.height * .7f), "Guest", LargeButtons))
        {
            //EnableInputBackground();
            currentLoginState = LoginState.GuestWarning;

        }
    }

    void GuestWarningScreen()
    {
        var tempStyle = new GUIStyle(LargeButtons);
        tempStyle.wordWrap = true;
        tempStyle.alignment = TextAnchor.MiddleCenter;
        tempStyle.fontSize = LargeButtons.fontSize - 15;

        var size = Camera.main.WorldToScreenPoint(backgroundSprite.transform.localScale);
        size = new Vector3(size.x * .7f, size.y * .6f);
        var pos = Camera.main.WorldToScreenPoint(backgroundSprite.transform.position) - new Vector3(size.x * .5f, size.y * .6f);

        GUI.Label(new Rect(pos, size), "If you continue to play as a guest, your progress will not be saved. Press 'Continue' to play as Guest.", tempStyle);

        var button = transform.Find("ContinueButton").GetComponent<SubmitButton>();
        button.isEnabled = true;
        //button.position = new Vector3(backgroundRect.x + backgroundRect.width * .18f, backgroundRect.y + backgroundRect.height * .75f);
        //button.size = new Vector2(backgroundRect.width * .6f, backgroundRect.height * .15f);

        if (button.WasPressed)
        {
            UserProfile.ContinueAsGuest = true;
            show = false;
            button.isEnabled = false;
        }
    }

    void ShowSignUp(bool overrideDelay = false)
    {
        SmallLabel.fontSize = Screen.width / 30;
        var spriteRenderer = Camera.main.WorldToScreenPoint(new Vector3(backgroundSprite.bounds.min.x, backgroundSprite.bounds.min.y));
        var size = Camera.main.WorldToScreenPoint(new Vector3(backgroundSprite.bounds.max.x, backgroundSprite.bounds.max.y)) - Camera.main.WorldToScreenPoint(new Vector3(backgroundSprite.bounds.min.x, backgroundSprite.bounds.min.y));
        spriteRenderer = new Vector3(spriteRenderer.x + size.x * .05f, spriteRenderer.y + size.y * .15f);
       
        GUI.Label(new Rect(new Vector3(spriteRenderer.x, spriteRenderer.y), topRect.size), "Email", SmallLabel);
        GUI.Label(new Rect(bottomRight - new Vector3(topRect.size.x * .3f, topRect.size.y * .75f), topRect.size), "UserName", SmallLabel);
        GUI.Label(new Rect(topLeft - new Vector3(topRect.size.x * .3f, topRect.size.y * .75f), topRect.size), "Password", SmallLabel);
        GUI.Label(new Rect(topRight - new Vector3(topRect.size.x * .3f, topRect.size.y * .75f), topRect.size), "Confirm", SmallLabel);

        SetSignupVectors();

        signupEmail.transform.position = topLeft;
        signupUserName.transform.position = topRight;
        signupPassword1.transform.position = bottomLeft;
        signupPassword2.transform.position = bottomRight;

        var username = signupUserName.GetComponent<InputField>().text;
        var password1 = signupPassword1.GetComponent<InputField>().text;
        var password2 = signupPassword2.GetComponent<InputField>().text;
        var email = signupEmail.GetComponent<InputField>().text;

        //EnableInputBackground();

        if (singupHasSubmittedAtleastOnce)
            HasEmptySignupFields(username, email, password1, password2);

        var button = transform.Find("SignupButton").GetComponent<SubmitButton>();
        button.isEnabled = true;
        //button.position = new Vector3(backgroundRect.x + backgroundRect.width * .18f, backgroundRect.y + backgroundRect.height * .75f);
        //button.size = new Vector2(backgroundRect.width * .6f, backgroundRect.height * .15f);

        if (button.WasPressed || SignupKeys())
        {
            button.WasPressed = false;
            var errors = new List<SignupErrors>();
            if (!TrySubmitSignup(username, email, password1, password2, out errors))
            {
                ShowSignupValidationMessages(errors);
            }
            else
            {
                show = false;
                UserProfile.IsLoggedIn = true;
            }
        }

    }

    void ShowSignupValidationMessages(List<SignupErrors> errors)
    {
        Debug.Log("?");

        if (errors.Contains(SignupErrors.HasEmptyFields))
            return;
        else if (errors.Contains(SignupErrors.EmailAlreadyInUse))
        {
            currentValidationMessage = signupEmail.GetComponent<ValidationMessage>();
            currentValidationMessage.isEnabled = true;
            currentValidationMessage.text = "This email is already in use";
            return;
        }
        else if (errors.Contains(SignupErrors.UsernameAlreadyExists))
        {
            currentValidationMessage = signupUserName.GetComponent<ValidationMessage>();
            currentValidationMessage.isEnabled = true;
            currentValidationMessage.text = "Sorry, this username is taken";
            return;
        }
        else if (errors.Contains(SignupErrors.IncorrectEmailFormat))
        {
            currentValidationMessage = signupEmail.GetComponent<ValidationMessage>();
            currentValidationMessage.isEnabled = true;
            currentValidationMessage.wordWrap = true;
            currentValidationMessage.height = 35;
            currentValidationMessage.padding = "";
            currentValidationMessage.text = "Email must be formated as email@domain.com";
            return;
        }
        else if (errors.Contains(SignupErrors.PasswordNotStrongEnough))
        {
            currentValidationMessage = signupPassword1.GetComponent<ValidationMessage>();
            currentValidationMessage.isEnabled = true;
            currentValidationMessage.widthPercent = 1.3f;
            currentValidationMessage.wordWrap = true;
            currentValidationMessage.height = 50;
            currentValidationMessage.padding = "";
            currentValidationMessage.text = "Must be atleast 5 characters, contain atleat 1 uppercase letter and atleat 1 special character (" + specialCharacters + ").";
            return;
        }
        else if (errors.Contains(SignupErrors.PasswordsDoNotMatch))
        {
            currentValidationMessage = signupPassword1.GetComponent<ValidationMessage>();
            currentValidationMessage.isEnabled = true;
            currentValidationMessage.text = "These don't match!";

            currentValidationMessage = signupPassword2.GetComponent<ValidationMessage>();
            currentValidationMessage.isEnabled = true;
            currentValidationMessage.text = "These don't match!";
            return;
        }
    }

    bool TrySubmitSignup(string username, string email, string password1, string password2, out List<SignupErrors> errors)
    {
        singupHasSubmittedAtleastOnce = true;
        errorsExistSignup = false;

        errors = new List<SignupErrors>();

        if (HasEmptySignupFields(username, email, password1, password2))
        {
            errorsExistSignup = true;
            errors.Add(SignupErrors.HasEmptyFields);
            return false;
        }
        if (!HasUpperChar(password1) || !HasSpecialCharacter(password1) || password1.Length < 5)
        {
            errorsExistSignup = true;
            errors.Add(SignupErrors.PasswordNotStrongEnough);
        }
        if (password1 != password2)
        {
            errorsExistSignup = true;
            //signupPassword1.GetComponent<InputField>().text = string.Empty;
            //signupPassword2.GetComponent<InputField>().text = string.Empty;
            errors.Add(SignupErrors.EmailAlreadyInUse);
        }
        if (email.Split('@').Length != 2 || email.Split('.').Length != 2 || email.IndexOf('@') > email.IndexOf('.'))
        {
            errorsExistSignup = true;
            errors.Add(SignupErrors.IncorrectEmailFormat);
        }

        foreach (var e in errors)
            Debug.Log(e);
        if (errors.Count > 0)
            return false;

        int index = serverRequestsInUse;
        serverRequestsInUse++;
        TryGetProfile(index, username: username);
        if (tryGetProfileServerRequests[index])
        {
            errorsExistSignup = true;
            errors.Add(SignupErrors.UsernameAlreadyExists);
        }
        Debug.Log("User: " + tryGetProfileServerRequests[index]);
        serverRequestsInUse--;

        index = serverRequestsInUse;
        serverRequestsInUse++;
        TryGetProfile(index, email: email);
        if (tryGetProfileServerRequests[index])
        {
            errorsExistSignup = true;
            errors.Add(SignupErrors.EmailAlreadyInUse);
        }
        Debug.Log("Email: " + tryGetProfileServerRequests[index]);
        serverRequestsInUse--;

        if (errors.Count > 0)
            return false;

        if (!CreateProfile(username, password1, email, setUserProfile: true, sendToServer: true))
            throw new Exception("Unable to create profile");

        return true;
    }

    void ShowLogIn()
    {
        var tempBottomRect = bottomRect;
        tempBottomRect.yMin += tempBottomRect.height * .1f;
        SmallLabel.fontSize = Screen.width / 28; ;
        GUI.Label(topRect, "UserName", SmallLabel);
        GUI.Label(tempBottomRect, "Password", SmallLabel);

        loginUserName.transform.position = new Vector3(backgroundRect.x + backgroundRect.width * .5f, Screen.height - topRect.y - topRect.height * .5f, 1);
        loginPassword.transform.position = new Vector3(backgroundRect.x + backgroundRect.width * .5f, Screen.height - bottomRect.y - bottomRect.height * .6f, 1);

        //EnableInputBackground();

        var username = loginUserName.GetComponent<InputField>().text;
        var password = loginPassword.GetComponent<InputField>().text;

        if (singupHasSubmittedAtleastOnce)
        {
            HasEmptyLoginFields(username, password);
        }

        var button = transform.Find("LoginButton").GetComponent<SubmitButton>();

        button.isEnabled = true;
        //button.position = new Vector3(backgroundRect.x + backgroundRect.width * .18f, backgroundRect.y + backgroundRect.height * .75f);
        //button.size = new Vector2(backgroundRect.width * .6f, backgroundRect.height * .15f);

        if (button.WasPressed)
        {
            button.WasPressed = false;
            loginHasSubmittedAtleastOnce = true;

            currentValidationMessage = loginPassword.GetComponent<ValidationMessage>();
            currentValidationMessage.isEnabled = false;

            if (HasEmptyLoginFields(username, password))
                return;

            int index = serverRequestsInUse;
            serverRequestsInUse++;
            TryGetProfile(index, username: username, password: password, setUserProfile: true);

            if (!tryGetProfileServerRequests[index])
            {

                Debug.Log("Cannont find profile with Username =  '" + loginUserName.GetComponent<InputField>().text + "' and Password '" + loginPassword.GetComponent<InputField>().text + "'.");
                loginPassword.GetComponent<InputField>().text = string.Empty;

                currentValidationMessage = loginPassword.GetComponent<ValidationMessage>();
                currentValidationMessage.isEnabled = true;
            }
            else
            {
                show = false;
                UserProfile.IsLoggedIn = true;
            }

            serverRequestsInUse--;
        }
    }

    bool CreateProfile(string username, string password, string email, bool setUserProfile = false, bool sendToServer = false)
    {
        var profile = new ProfileData(username, password, email, UserProfile.GenereateID(ensureIsUnique: true, setId: true));

        FileManager.Append<ProfileData>(profile);

        if (setUserProfile)
            UserProfile.SetUserProfile(profile);

        if (sendToServer)
            networkManager.PostProfile(profile);

        return true;
    }

    BitArray tryGetProfileServerRequests;

    IEnumerator NullIEnumerator()
    {
        yield return new WaitForSeconds(0);
    }

    bool TryGetProfile(int index, string username = null, string email = null, string password = null, bool setUserProfile = false)
    {
        tryGetProfileServerRequests[index] = false;

        if (username == null && email == null)
            StartCoroutine(NullIEnumerator());

        ProfileData profile = null;



        if (!FileManager.TryGetUser(out profile, userName: username, password: password, email: email))
        {
            var criteria = new Dictionary<ProfileSearchCriteria, string>();
            if (username != null)
                criteria.Add(ProfileSearchCriteria.Username, username);
            if (email != null)
                criteria.Add(ProfileSearchCriteria.Email, email);
            if (password != null)
                criteria.Add(ProfileSearchCriteria.Password, password);

            Debug.Log("?");



            TryGetProfileOnServer(criteria, index);
        }
        else
            tryGetProfileServerRequests[index] = true;

        if (!tryGetProfileServerRequests[index])
            return false;


        if (setUserProfile)
            UserProfile.SetUserProfile(NetworkManager.profileReferenceData);

        return true;
    }

    bool isOnServer = false;

    void TryGetProfileOnServer(Dictionary<ProfileSearchCriteria, string> criteria, int index)
    {

        isOnServer = false;
        networkManager.RetrieveProfileNow(criteria, () =>
        {
            tryGetProfileServerRequests[index] = true;
            Debug.Log("HELLLLLOOOOO");

        }, true);
    }

    void SetSignUpFields()
    {
        signupUserName.SetActive(true);
        signupPassword1.SetActive(true);
        signupPassword2.SetActive(true);
        signupEmail.SetActive(true);
    }

    void SetLoginFields()
    {
        loginUserName.SetActive(true);
        loginPassword.SetActive(true);
    }

    void Grow(GameObject obj, float ratio, float tolerance = .01f)
    {
        Vector2 growByPercentage;

        if (ratio >= 1 - tolerance || ratio <= tolerance)
            growByPercentage = new Vector2(1, 1);
        else if (ratio < .5)
            growByPercentage = new Vector2(ratio, 1 - ratio);
        else
            growByPercentage = new Vector2(1 - ratio, ratio);

        obj.transform.localScale = new Vector3(obj.transform.localScale.x * (growByPercentage.x + 1f), obj.transform.localScale.y * (growByPercentage.y + 1f), obj.transform.localScale.z);
    }

    void Shrink(GameObject obj, float ratio, float tolerance = .01f)
    {
        Vector2 growByPercentage;

        if (ratio >= 1 - tolerance || ratio <= tolerance)
            growByPercentage = new Vector2(1, 1);
        else if (ratio < .5)
            growByPercentage = new Vector2(ratio, 1 - ratio);
        else
            growByPercentage = new Vector2(1 - ratio, ratio);

        obj.transform.localScale = new Vector3(obj.transform.localScale.x * (growByPercentage.x), obj.transform.localScale.y * (growByPercentage.y), obj.transform.localScale.z);
    }

    Vector2 Size(SpriteRenderer renderer)
    {
        return Camera.main.WorldToScreenPoint(renderer.bounds.center + renderer.bounds.extents) - Camera.main.WorldToScreenPoint(renderer.bounds.center - renderer.bounds.extents);
    }

    void SetRectangles()
    {
        backgroundRect = new Rect(Screen.width * .2f, Screen.height * .1f, Screen.width * .6f, Screen.height * .8f);
        topRect = new Rect(backgroundRect.xMin + backgroundRect.width * .15f, backgroundRect.yMin + backgroundRect.height * .15f, backgroundRect.width * .7f, backgroundRect.height * .3f);
        bottomRect = new Rect(backgroundRect.xMin + backgroundRect.width * .15f, backgroundRect.yMin + backgroundRect.height * .45f, backgroundRect.width * .7f, backgroundRect.height * .3f);
    }

    void SetSignupVectors()
    {
        topLeft = new Vector3(backgroundRect.x + backgroundRect.width * .25f, Screen.height - topRect.y - topRect.height * .5f, 1);
        topRight = new Vector3(backgroundRect.x + backgroundRect.width * .75f, Screen.height - topRect.y - topRect.height * .5f, 1);
        bottomLeft = new Vector3(backgroundRect.x + backgroundRect.width * .25f, Screen.height - bottomRect.y - bottomRect.height * .5f, 1);
        bottomRight = new Vector3(backgroundRect.x + backgroundRect.width * .75f, Screen.height - bottomRect.y - bottomRect.height * .5f, 1);
        //signupUserName.transform.position = new Vector3(backgroundRect.x + backgroundRect.width * .75f, Screen.height - topRect.y - topRect.height * .5f, 1);
        //signupPassword1.transform.position = new Vector3(backgroundRect.x + backgroundRect.width * .25f, Screen.height - bottomRect.y - bottomRect.height * .5f, 1);
        //signupPassword2.transform.position = new Vector3(backgroundRect.x + backgroundRect.width * .75f, Screen.height - bottomRect.y - bottomRect.height * .5f, 1);
    }

    bool HasEmptySignupFields(string username, string email, string password1, string password2)
    {
        bool hasEmptyFields = false;
        if (string.IsNullOrEmpty(username))
        {
            signupUserName.GetComponent<Image>().color = Color.red;
            hasEmptyFields = true;
        }
        else
            signupUserName.GetComponent<Image>().color = Color.white;
        if (string.IsNullOrEmpty(email))
        {
            signupEmail.GetComponent<Image>().color = Color.red;
            hasEmptyFields = true;
        }
        else
            signupEmail.GetComponent<Image>().color = Color.white;
        if (string.IsNullOrEmpty(password1))
        {
            signupPassword1.GetComponent<Image>().color = Color.red;
            hasEmptyFields = true;
        }
        else
            signupPassword1.GetComponent<Image>().color = Color.white;
        if (string.IsNullOrEmpty(password2))
        {
            signupPassword2.GetComponent<Image>().color = Color.red;
            hasEmptyFields = true;
        }
        else
            signupPassword2.GetComponent<Image>().color = Color.white;
        return hasEmptyFields;
    }

    bool HasEmptyLoginFields(string username, string password)
    {
        bool hasEmptyFields = false;
        if (string.IsNullOrEmpty(username))
        {
            loginUserName.GetComponent<Image>().color = Color.red;
            hasEmptyFields = true;
        }
        else
            loginUserName.GetComponent<Image>().color = Color.white;

        if (string.IsNullOrEmpty(password))
        {
            loginPassword.GetComponent<Image>().color = Color.red;
            hasEmptyFields = true;
        }
        else
            loginPassword.GetComponent<Image>().color = Color.white;
        return hasEmptyFields;
    }

    bool SignupKeys()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (signupEmail.GetComponent<InputField>().isFocused)
            {
                signupUserName.GetComponent<InputField>().ActivateInputField();
            }
            else if (signupUserName.GetComponent<InputField>().isFocused)
            {
                signupPassword1.GetComponent<InputField>().ActivateInputField();
            }
            else if (signupPassword1.GetComponent<InputField>().isFocused)
            {
                signupPassword2.GetComponent<InputField>().ActivateInputField();
            }
            else if (signupPassword2.GetComponent<InputField>().isFocused)
            {
                signupEmail.GetComponent<InputField>().ActivateInputField();
            }

        }

        return Input.GetKeyDown(KeyCode.Return);
    }

    bool HasUpperChar(string s)
    {
        foreach (var c in s)
        {
            if (Char.IsUpper(c))
                return true;
        }
        return false;
    }

    bool HasSpecialCharacter(string s)
    {
        foreach (var c in s)
        {
            if (specialCharacters.Contains(c))
                return true;
        }
        return false;
    }

    void EnableInputBackground()
    {

        var spriteobj = transform.Find("Background Sprite");
        var color = spriteobj.GetComponent<SpriteRenderer>().color;
        color.a = backgroundAlpha;
        spriteobj.GetComponent<SpriteRenderer>().color = color;
        var worldSize = Camera.main.ScreenToWorldPoint(signupPassword1.transform.position + new Vector3(Screen.width * .15f, Screen.height * .12f, 0));

        spriteobj.transform.position = worldSize;
        spriteobj.transform.localScale = new Vector3(.18f, .16f);
    }

    void DisableInputBackground()
    {
        var spriteobj = transform.Find("Background Sprite");
        spriteobj.transform.localScale = new Vector3(0, 0);
    }
}

public enum LoginState
{
    Deciding,
    EnteringLoginInfo,
    EnteringSignupInfo,
    LoggingIn,
    GuestWarning
}

public enum SignupErrors
{
    UsernameAlreadyExists,
    IncorrectEmailFormat,
    PasswordsDoNotMatch,
    PasswordNotStrongEnough,
    HasEmptyFields,
    EmailAlreadyInUse
}