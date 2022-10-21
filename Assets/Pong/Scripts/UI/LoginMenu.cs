using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public static partial class Events
{
    public static event Action OnLoginSuccess = null;
    public static void DoFireLoginSuccess() => OnLoginSuccess?.Invoke();
    public static event Action<string> OnLoginFailed = null;
    public static void DoFireLoginFailed(string reason) => OnLoginFailed?.Invoke(reason);

    public static event Action OnForgetPassword = null;
    public static void DoFireForgetPassword() => OnForgetPassword?.Invoke();
}

public class LoginMenu : MonoBehaviour
{
    [SerializeField] private Button LoginButton;
    [SerializeField] private Button RegisterButton;
    [SerializeField] private Button PlayAsGuestButton;
    [SerializeField] private TMP_InputField EmailField, PasswordField;
    [SerializeField] private TMP_Text StatusText;
    [SerializeField] private Toggle RememberMeTog;
    public UIManager UIRef;
    private Coroutine animateRoutine;
    // Start is called before the first frame update
    void OnEnable()
    {
        UIRef.DisableObjects();
        UIRef.DisableObsImages();
        getSavedCredData();
        RememberMeTog.isOn = Constants.RememberMe;
        this.LoginButton.onClick.AddListener(OnLoginButtonClicked);
        this.RegisterButton.onClick.AddListener(OnRegisterButtonClicked);
        this.PlayAsGuestButton.onClick.AddListener(OnPlayAsGuest);
        this.RememberMeTog.onValueChanged.AddListener(delegate {
            OnRemeberMeToggleChanged(this.RememberMeTog);
        });

        Events.OnLoginFailed += OnFailure;
        Events.OnLoginSuccess += OnLoginSuccess;
        Events.OnForgetPassword += OnForgetPassword;
    }

    private void OnDisable()
    {
        this.LoginButton.onClick.RemoveListener(OnLoginButtonClicked);
        this.RegisterButton.onClick.RemoveListener(OnRegisterButtonClicked);
        this.PlayAsGuestButton.onClick.RemoveListener(OnPlayAsGuest);
        this.RememberMeTog.onValueChanged.RemoveListener(delegate {
            OnRemeberMeToggleChanged(this.RememberMeTog);
        });

        Events.OnLoginFailed -= OnFailure;
        Events.OnLoginSuccess -= OnLoginSuccess;
        Events.OnForgetPassword -= OnForgetPassword;
    }

    public void getSavedCredData()
    {
        string _data = PlayerPrefs.GetString(Constants.CredKey, "");
        if (!string.IsNullOrEmpty(_data))
        {
            Constants.RememberMe = true;
            AuthCredentials _cred = JsonConvert.DeserializeObject<AuthCredentials>(_data);
            EmailField.text = _cred.Email;
            PasswordField.text = _cred.Password;
        }
    }
  
    public void OnForgetPassword()
    {
        if (!string.IsNullOrEmpty(EmailField.text))
        {
            apiRequestHandler.Instance.onForgetPassword(EmailField.text);
            Events.DoReportMessage(new messageInfo("Instructions have been sent, please check your email."));
        }else
        {
            Events.DoReportMessage(new messageInfo("Please enter email."));
        }
    }

    private void OnFailure(string error)
    {
        MakeInteractable(true);
        this.StatusText.text = $"";
        StopCoroutine(animateRoutine);
        Events.DoReportMessage(new messageInfo($"Error: {error}"));
    }

    private void OnLoginSuccess()
    {
        if(Constants.RememberMe)
        {
            string _json = JsonConvert.SerializeObject(FirebaseManager.Instance.Credentails);
            PlayerPrefs.SetString(Constants.CredKey, _json);
        }else
        {
            PlayerPrefs.DeleteKey(Constants.CredKey);
        }

        Constants.PlayingAsGuest = false;
        MakeInteractable(true);
        this.StatusText.text = $"";
        StopCoroutine(animateRoutine);
        Managers.UI.ActivateUI(Menus.MAIN);
        AudioManager.Audio.PlayLobbyMusic();
    }

    private void OnPlayAsGuest()
    {
        Constants.PlayingAsGuest = true;

        if (FirebaseManager.Instance)
        {
            FirebaseManager.Instance.PlayerData = new UserData();
            FirebaseManager.Instance.PlayerData.UserName = "USER_" + UnityEngine.Random.Range(111, 999).ToString();
        }
        else
            Debug.LogError("FM instance is null");

        AudioManager.Audio.PlayClickSound();
        MakeInteractable(true);
        this.StatusText.text = $"";
        Managers.UI.ActivateUI(Menus.MAIN);
        AudioManager.Audio.PlayLobbyMusic();
    }


    private void MakeInteractable(bool value)
    {
        LoginButton.interactable = value;
        RegisterButton.interactable = value;
        EmailField.interactable = value;
        PasswordField.interactable = value;
        this.PlayAsGuestButton.interactable = value;
    }

    IEnumerator AnimateStatus()
    {
        var delay = 0.5f;
        yield return new WaitForSeconds(delay);
        this.StatusText.text = $"please wait.";
        yield return new WaitForSeconds(delay);
        this.StatusText.text = $"please wait..";
        yield return new WaitForSeconds(delay);
        this.StatusText.text = $"please wait...";
        yield return new WaitForSeconds(delay);
        this.StatusText.text = $"please wait";

        animateRoutine = StartCoroutine(AnimateStatus());
    }

    private void OnLoginButtonClicked()
    {
        AudioManager.Audio.PlayClickSound();

        if (!Constants.WalletConnected && !Constants.IsTest)
        {
            Events.DoReportMessage(new messageInfo($"Error: Please connect your wallet first."));
            return;
        }

        MakeInteractable(false);
        animateRoutine = StartCoroutine(AnimateStatus());
        apiRequestHandler.Instance.signInWithEmail(EmailField.text, PasswordField.text);
    }

    private void OnRegisterButtonClicked()
    {
        AudioManager.Audio.PlayClickSound();

        if (!Constants.WalletConnected && !Constants.IsTest)
        {
            Events.DoReportMessage(new messageInfo($"Error: Please connect your wallet first."));
            return;
        }

        Managers.UI.ActivateUI(Menus.REGISTER);
    }

    public void OnRemeberMeToggleChanged(Toggle _togg)
    {
        Constants.RememberMe = _togg.isOn;
    }
}
